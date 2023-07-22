#region

using System;
using System.Collections.Generic;
using Svelto.Context;

#endregion

namespace Svelto.IoC.Extensions.Context
{
    public class ContextContainer : Container
    {
        /// <summary>
        /// Handles init/deinit for all bound classes that inherit IOnFrameworkInitialized or IOnFrameworkDestroyed
        /// </summary>
        protected class NotifierWrapper : IOnFrameworkInitialized, IOnFrameworkDestroyed
        {
            ContextContainer _c;

            List<Type> _toInit = new List<Type>();
            List<Type> _toDeInit = new List<Type>();

            public NotifierWrapper(ContextContainer c)
            {
                _c = c;
            }

            public void AddInitType(Type type)
            {
                _toInit.Add(type);
            }

            public void AddDeInitType(Type type)
            {
                _toDeInit.Add(type);
            }

            public void OnFrameworkInitialized()
            {
                foreach (Type t in _toInit)
                {
                    var instance = _c.Get(t);
                    ((IOnFrameworkInitialized)instance).OnFrameworkInitialized();
                }
                _toInit = null;
            }

            public void OnFrameworkDestroyed()
            {
                foreach (Type t in _toDeInit)
                {
                    var instance = _c.Get(t);
                    ((IOnFrameworkDestroyed)instance).OnFrameworkDestroyed();
                }
                _toDeInit = null;
            }
        }

        private class ContextBinder<Contractor> : IBinder<Contractor> where Contractor : class
        {
            public delegate void RegisterCallback(Type interfaceType, Type implementationType);

            private RegisterCallback _onRegister;

            private IInternalContainer _container;

            private Type _interfaceType;

            public ContextBinder(RegisterCallback onRegister)
            {
                DesignByContract.Check.Require(onRegister != null, "onRegister callback was null");

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
                _onRegister(_interfaceType, implementationType);

                /*
                if (typeof(IOnFrameworkInitialized).IsAssignableFrom(implementationType) || typeof(IOnFrameworkDestroyed).IsAssignableFrom(implementationType))
                {
                    _onRegister(_interfaceType);
                }
                */
            }
        }

        protected NotifierWrapper _notifierWrapper;

        public ContextContainer(IContextNotifer contextNotifier)
        {
            _notifierWrapper = new NotifierWrapper(this);
            _contextNotifier = contextNotifier;

            _contextNotifier.AddFrameworkInitializationListener(_notifierWrapper);
            _contextNotifier.AddFrameworkDestructionListener(_notifierWrapper);
        }

        protected override IBinder<TContractor> BinderProvider<TContractor>()
        {
            return new ContextBinder<TContractor>(AddType);
        }

        protected virtual void AddType(Type interfaceType, Type implementationType)
        {
            if (typeof(IOnFrameworkInitialized).IsAssignableFrom(implementationType))
            {
                _notifierWrapper.AddInitType(interfaceType);
            }
            if (typeof(IOnFrameworkDestroyed).IsAssignableFrom(implementationType))
            {
                _notifierWrapper.AddDeInitType(interfaceType);
            }
        }

        IContextNotifer _contextNotifier;
    }
}
