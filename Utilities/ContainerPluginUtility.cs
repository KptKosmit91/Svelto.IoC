using System;

namespace Svelto.IoC.Plugins
{
    public static class ContainerPluginUtility
    {
        public static void ForeachPlugin(this IContainerPlugin[] plugins, Action<IContainerPlugin> action)
        {
            for (int i = 0; i < plugins.Length; i++)
            {
                try
                {
                    var plugin = plugins[i];
                    if (plugin != null)
                    {
                        action(plugin);
                    }
                }
                catch (Exception e)
                {
                    Utility.Console.LogException(e);
                }
            }
        }
    }
}
