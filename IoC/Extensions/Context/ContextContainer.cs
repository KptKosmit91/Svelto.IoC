#region

using System;
using Svelto.Context;

#endregion

namespace Svelto.IoC.Extensions.Context
{
    public class ContextContainer : Container
    {
        private class ContextBinder<Contractor> : IBinder<Contractor> where Contractor : class
        {
            private Action<Type> _onRegister;

            private IInternalContainer _container;

            private Type _interfaceType;

            public ContextBinder(Action<Type> onRegister)
            {
                _onRegister = onRegister;
            }

            public void Bind<ToBind>(IInternalContainer container) where ToBind : class
            {
                _container = container;
                _interfaceType = typeof(ToBind);
            }

            public void AsSingle<T>() where T : Contractor, new()
            {
                _container.Register<T, StandardProvider<T>>(_interfaceType, new StandardProvider<T>());
                OnRegister(typeof(T));
            }

            public void AsInstance<T>(T instance) where T : class, Contractor
            {
                _container.Register<T, SelfProvider<T>>(_interfaceType, new SelfProvider<T>(instance));
                OnRegister(typeof(T));
            }

            public void ToProvider<T>(IProvider<T> provider) where T : class, Contractor
            {
                _container.Register<T, IProvider<T>>(_interfaceType, provider);
                OnRegister(typeof(T));
            }

            private void OnRegister(Type implementationType)
            {
                if (typeof(IOnFrameworkInitialized).IsAssignableFrom(implementationType) || typeof(IOnFrameworkDestroyed).IsAssignableFrom(implementationType))
                {
                    _onRegister(_interfaceType);
                }
            }
        }

        public ContextContainer(IContextNotifer contextNotifier)
        {
            _contextNotifier = contextNotifier;
        }

        protected override IBinder<TContractor> BinderProvider<TContractor>()
        {
            return new ContextBinder<TContractor>(AddType);
        }

        private void AddType(Type type)
        {
            var instance = Get(type);
            OnInstanceGenerated(instance);
        }

        override protected void OnInstanceGenerated<TContractor>(TContractor instance)
        {
            if (instance is IOnFrameworkInitialized)
                _contextNotifier.AddFrameworkInitializationListener(instance as IOnFrameworkInitialized);

            if (instance is IOnFrameworkDestroyed)
                _contextNotifier.AddFrameworkDestructionListener(instance as IOnFrameworkDestroyed);
        }

        IContextNotifer _contextNotifier;
    }
}
