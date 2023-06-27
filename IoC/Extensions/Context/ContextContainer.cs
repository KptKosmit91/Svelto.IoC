#region

using System;
using Svelto.Context;

#endregion

namespace Svelto.IoC.Extensions.Context
{
    public class ContextContainer: Container
    {
        public ContextContainer(IContextNotifer contextNotifier)
        {
            _contextNotifier = contextNotifier;
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

