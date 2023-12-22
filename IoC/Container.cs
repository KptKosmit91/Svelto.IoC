using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC.Plugins;

namespace Svelto.IoC
{
    public class Container : IContainer, IInternalContainer
    {
        IContainerPlugin[] _plugins;

        protected IContextNotifer _contextNotifier;

        protected ContainerNotifierWrapper _notifierWrapper;

        public Container(IContextNotifer contextNotifier, IContainerPlugin[] plugins)
        {
            _providers = ProviderBehaviour();
            _delegateToSearchCriteria = DelegateToSearchCriteria;

            if (plugins == null)
            {
                _plugins = new IContainerPlugin[0];
            }
            else
            {
                _plugins = plugins.Where(x => x != null).ToArray();
            }

            _notifierWrapper = new ContainerNotifierWrapper(this, _plugins, contextNotifier);

            if (contextNotifier != null)
            {
                _contextNotifier = contextNotifier;
                _contextNotifier.AddFrameworkInitializationListener(_notifierWrapper);
                _contextNotifier.AddFrameworkDestructionListener(_notifierWrapper);
            }
        }

        //
        // IContainer interface
        //

        public IBinder<TContractor> Bind<TContractor>() where TContractor : class
        {
            IBinder<TContractor> binder = InternalBind<TContractor>();

            return binder;
        }

        public void BindSelf<TContractor>() where TContractor : class, new()
        {
            IBinder<TContractor> binder = InternalBind<TContractor>();

            binder.AsSingle<TContractor>();
        }

        public TContractor Build<TContractor>() where TContractor : class
        {
            Type contract = typeof(TContractor);

            return Build(contract) as TContractor;
        }

        public object Build(Type contract)
        {
            object instance = Get(contract);

            DesignByContract.Check.Ensure(instance != null, $"IoC.Container instance for type {contract} failed to be built (contractor not found - must be registered)");

            return instance;
        }

        public void Release<TContractor>() where TContractor : class
        {
            Type type = typeof(TContractor);

            _providers.Remove(type);
        }

        public TContractor Inject<TContractor>(TContractor instance)
        {
            if (instance != null)
                InternalInject(instance);

            return instance;
        }

        //
        // IInternalContainer interface
        //

        public void Register<T, K>(Type type, K provider) where K : IProvider<T>
        {
            _providers.Register<T>(type, provider);
        }

        public bool TryGetProvider<T>(out IProvider provider)
        {
            return _providers.Retrieve(typeof(T), out provider);
        }

        protected virtual IProviderContainer ProviderBehaviour()
        {
            return new ProviderContainer();
        }

        /// <summary>
        /// Users can define their own IBinder and override this function to use it
        /// </summary>
        /// <typeparam name="TContractor"></typeparam>
        /// <returns></returns>

        protected virtual IBinder<TContractor> BinderProvider<TContractor>() where TContractor : class
        {
            return new ContainerBinder<TContractor>(OnTypeBound);
        }

        protected virtual void OnTypeBound(Type interfaceType, Type implementationType)
        {
            _plugins.ForeachPlugin(x => 
            { 
                x.OnTypeBound(interfaceType, implementationType); 
            });

            if (implementationType.InheritsType(typeof(IOnFrameworkInitialized)))
            {
                _notifierWrapper.AddInitType(interfaceType);
            }

            if (implementationType.InheritsType(typeof(IOnFrameworkDestroyed)))
            {
                _notifierWrapper.AddDeInitType(interfaceType);
            }
        }

        //
        // protected Members
        //

        protected object Get(Type contract)
        {
            return CreateDependency(contract, null, null);
        }

        /// <summary>
        /// Called when an instance is first built. Useful to add new Container behaviours,
        /// but not needed for the final user since OnDependenciesInjected must be used instead.
        /// </summary>
        /// <typeparam name="TContractor"></typeparam>
        /// <param name="instance"></param>

        protected virtual void OnInstanceGenerated<TContractor>(TContractor instance) where TContractor : class
        { }
#if TO_COMPLETE
        void CallInjection(object injectable, MethodInfo info, Type contract)
        {
            ParameterInfo[] parameters = info.GetParameters();
            object[] parameterBuffer = new object[parameters.Length];

            for (int i = parameters.Length - 1; i >= 0; --i)
            {
                ParameterInfo parameter = parameters[i];

                object valueObj = Get(parameter.ParameterType, contract);

                //inject in Injectable the valueObj
                if (valueObj != null)
                    parameterBuffer[i] = valueObj;
            }

            info.Invoke(injectable, parameterBuffer);
        }
#endif
        object CreateDependency(Type contract, Type containerContract, PropertyInfo info)
        {
            IProvider provider = null;

            if (_providers.Retrieve(contract, out provider))
            {
                object obj;

                if (provider.Create(containerContract, info, out obj) == true)
                {
                    InternalInject(obj);
                    OnInstanceGenerated(obj);
                }

                return obj;
            }

            return null;
        }

        static bool DelegateToSearchCriteria(MemberInfo objMemberInfo, object objSearch)
        {
            return objMemberInfo.IsDefined((Type)objSearch, true);
        }

        object InternalGet(Type contract, Type containerContract, PropertyInfo info)
        {
            return CreateDependency(contract, containerContract, info);
        }

        void InjectProperty(object instanceToFullfill, PropertyInfo info, Type contract)
        {
            if (info.PropertyType == typeof(IContainer)) // self inject
            {
#if DEBUG || TESTBUILD
                Utility.Console.LogWarning($"Inject containers automatically is considered a design error. [in {instanceToFullfill.GetType()}]");
#endif

                info.SetValue(instanceToFullfill, this, null);
            }
            else
            {
                object referenceToInject;

                if (info.PropertyType.IsGenericType == true &&
                    info.PropertyType.GetGenericTypeDefinition() == _weakReferenceType)
                {
                    referenceToInject = InternalGet(info.PropertyType.GetGenericArguments()[0], contract, info);

                    if (referenceToInject != null)
                    {
                        object o = Activator.CreateInstance(info.PropertyType, referenceToInject);

                        info.SetValue(instanceToFullfill, o, null);
                    }
                }
                else
                {
                    referenceToInject = InternalGet(info.PropertyType, contract, info);

                    //inject in Injectable the valueObj
                    if (referenceToInject != null)
                        info.SetValue(instanceToFullfill, referenceToInject, null);
                }
            }
        }

        //
        // Private Members
        //

        IBinder<TContractor> InternalBind<TContractor>() where TContractor : class
        {
            IBinder<TContractor> binder = BinderProvider<TContractor>();

            binder.Bind<TContractor>(this);

            return binder;
        }

        void InternalInject(object instanceToFullfill)
        {
            DesignByContract.Check.Require(instanceToFullfill != null);

            Type contract = instanceToFullfill.GetType();
            Type injectAttributeType = typeof(InjectAttribute);

            MemberInfo[] properties = null;

            if (_cachedProperties.TryGetValue(contract, out properties) == false)
            {

                properties = contract.FindMembers(MemberTypes.Property,
                                                    BindingFlags.SetProperty |
                                                    BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance,
                                                  _delegateToSearchCriteria, injectAttributeType);

                _cachedProperties[contract] = properties;
            }

            for (int i = 0; i < properties.Length; i++)
                InjectProperty(instanceToFullfill, properties[i] as PropertyInfo, contract);

            try
            {
                var fullfill = instanceToFullfill as IInitialize;
                if (fullfill != null)
                    fullfill.OnDependenciesInjected();
            }
            catch (Exception innerException)
            {
                throw new InjectException("OnDependenciesInjected Crashes inside ".FastConcat(instanceToFullfill.GetType().ToString()), innerException);
            }
        }

        readonly Dictionary<Type, MemberInfo[]> _cachedProperties = new Dictionary<Type, MemberInfo[]>();
        readonly MemberFilter _delegateToSearchCriteria;
        readonly Type _weakReferenceType = typeof(DataStructures.WeakReference<>);

        IProviderContainer _providers;

        public interface IProviderContainer
        {
            void Remove(Type type);
            bool Retrieve(Type contract, out IProvider provider);
            void Register<T>(Type type, IProvider<T> provider);
        }

        sealed class ProviderContainer : IProviderContainer
        {
            readonly Dictionary<Type, IProvider> _providers;
            readonly Dictionary<Type, IProvider> _standardProvidersPerInstanceType;
            public ProviderContainer()
            {
                _providers = new Dictionary<Type, IProvider>();
                _standardProvidersPerInstanceType = new Dictionary<Type, IProvider>();
            }

            public void Remove(Type type)
            {
                _providers.Remove(type);
            }

            public bool Retrieve(Type contract, out IProvider provider)
            {
                return _providers.TryGetValue(contract, out provider);
            }

            public void Register<T>(Type type, IProvider<T> provider)
            {
                var providerType = provider.GetType().GetGenericTypeDefinition();

                if (providerType == _standardProviderType)
                {
                    IProvider standardProvider;
                    var instanceType = typeof(T);
                    if (_standardProvidersPerInstanceType.TryGetValue(instanceType, out standardProvider) == false)
                        standardProvider = _standardProvidersPerInstanceType[instanceType] = new WeakProviderDecorator<T>(provider); //this should be harmless and allows to query for unique providers        

                    provider = ((WeakProviderDecorator<T>)standardProvider).provider;
                }

                _providers[type] = provider; //providers are normally saved by contract, not instance type
            }

            readonly Type _standardProviderType = typeof(StandardProvider<>);
        }

        /// <summary>
        /// Use this class to register an interface
        /// or class into the container.
        /// </summary>
        sealed class ContainerBinder<Contractor> : IBinder<Contractor> where Contractor : class
        {
            public delegate void RegisterCallback(Type interfaceType, Type implementationType);

            private RegisterCallback _onTypeBound;

            private IInternalContainer _container;

            private Type _interfaceType;

            public ContainerBinder(RegisterCallback onTypeBound)
            {
                DesignByContract.Check.Require(onTypeBound != null, "onRegister callback is null");

                _onTypeBound = onTypeBound;
            }

            public void Bind<ToBind>(IInternalContainer container) where ToBind : class
            {
                _container = container;
                _interfaceType = typeof(ToBind);
            }

            public void AsSingle<T>() where T : Contractor, new()
            {
                _container.Register<T, StandardProvider<T>>(_interfaceType, new StandardProvider<T>());
                OnTypeBound(typeof(T));
            }

            public void AsInstance<T>(T instance) where T : class, Contractor
            {
                _container.Register<T, SelfProvider<T>>(_interfaceType, new SelfProvider<T>(instance));
                OnTypeBound(typeof(T));
            }

            public void ToProvider<T>(IProvider<T> provider) where T : class, Contractor
            {
                _container.Register<T, IProvider<T>>(_interfaceType, provider);
                OnTypeBound(typeof(T));
            }

            private void OnTypeBound(Type implementationType)
            {
                _onTypeBound(_interfaceType, implementationType);

                /*
                if (typeof(IOnFrameworkInitialized).IsAssignableFrom(implementationType) || typeof(IOnFrameworkDestroyed).IsAssignableFrom(implementationType))
                {
                    _onRegister(_interfaceType);
                }
                */
            }
        }
    }
}

//things to do:
//DAG detection warning
//Injection by constructor
//Hierarchical container
//Not found dependency warning
//After 4 injection, add warning about too many injection
