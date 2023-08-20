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

        List<Type> _toInit = new List<Type>();
        List<Type> _toDeInit = new List<Type>();

        public ContainerNotifierWrapper(IContainer container, IContainerPlugin[] containerPlugins)
        {
            _container = container;
            _plugins = containerPlugins;
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

            foreach (Type t in _toInit)
            {
                var instance = _container.Build(t);
                ((IOnFrameworkInitialized)instance).OnFrameworkInitialized();
            }

            _toInit = null;
        }

        public void OnFrameworkDestroyed()
        {
            _plugins.ForeachPlugin(x =>
            {
                x.OnFrameworkDestroyed(_container);
            });

            foreach (Type t in _toDeInit)
            {
                var instance = _container.Build(t);
                ((IOnFrameworkDestroyed)instance).OnFrameworkDestroyed();
            }

            _toDeInit = null;
        }
    }
}
