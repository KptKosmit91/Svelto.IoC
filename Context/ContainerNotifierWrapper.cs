using Svelto.Context;
using Svelto.IoC;
using Svelto.IoC.Extensions.Context;
using Svelto.IoC.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.Context
{
    /// <summary>
    /// Handles init/deinit for all bound classes that inherit IOnFrameworkInitialized or IOnFrameworkDestroyed
    /// </summary>
    public class ContainerNotifierWrapper : IOnFrameworkInitialized, IOnFrameworkDestroyed
    {
        IContainer _container;

        IContainerPlugin[] _plugins;

        IContextNotifer _contextNotifier;

        List<Type> _toInit = new List<Type>();
        List<Type> _toDeInit = new List<Type>();

        public ContainerNotifierWrapper(IContainer container, IContainerPlugin[] containerPlugins, IContextNotifer contextNotifier)
        {
            _container = container;
            _plugins = containerPlugins;
            _contextNotifier = contextNotifier;
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
            _plugins.ForeachPlugin(x =>
            {
                x.OnFrameworkInitialized(_container);
            });

            foreach (Type typeToInit in _toInit)
            {
                var instance = _container.Build(typeToInit) as IOnFrameworkInitialized;

                if (_contextNotifier == null || _contextNotifier.IsAwaitingInitialization(instance) == false)
                {
                    instance.OnFrameworkInitialized();
                }
            }

            _toInit = null;
        }

        public void OnFrameworkDestroyed()
        {
            _plugins.ForeachPlugin(x =>
            {
                x.OnFrameworkDestroyed(_container);
            });

            foreach (Type typeToDeinit in _toDeInit)
            {
                var instance = _container.Build(typeToDeinit) as IOnFrameworkDestroyed;

                if (_contextNotifier == null || _contextNotifier.IsAwaitingDestruction(instance) == false)
                {
                    instance.OnFrameworkDestroyed();
                }
            }

            _toDeInit = null;
        }
    }
}
