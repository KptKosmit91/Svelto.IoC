using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.IoC.Plugins
{
    /// <summary>
    /// Add additional functionality to containers via container plugins
    /// </summary>
    public interface IContainerPlugin
    {
        /// <summary>
        /// Called when a new type is bound to the container.
        /// <para/>Note that you should probably not build/get the interface types here, as not all dependencies may have been bound to the container yet. 
        /// <para/>Build them in <c>OnFrameworkInitialized</c> instead
        /// </summary>
        /// <param name="interfaceType">The interface type</param>
        /// <param name="implementationType">The implementation type</param>
        void OnTypeBound(Type interfaceType, Type implementationType);

        /// <summary>
        /// Called on framework initialization, called before all bound classes have been initialized
        /// </summary>
        /// <param name="container">The Container this plugin is on</param>
        void OnFrameworkInitialized(IContainer container);

        /// <summary>
        /// Called on framework de-initialization (destruction), called before all bound classes have been de-initialized
        /// </summary>
        /// <param name="container">The Container this plugin is on</param>
        void OnFrameworkDestroyed(IContainer container);

    }
}
