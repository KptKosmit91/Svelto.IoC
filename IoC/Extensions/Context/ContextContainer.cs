#region

using System;
using System.Collections.Generic;
using Svelto.Context;
using Svelto.IoC.Plugins;

#endregion

namespace Svelto.IoC.Extensions.Context
{
    [Obsolete("Use Svelto.IoC.Container instead")]
    public class ContextContainer : Container
    {
        public ContextContainer(IContextNotifer contextNotifier) : base(contextNotifier, null)
        {
        }
    }
}
