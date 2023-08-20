using System;
using System.Collections.Generic;

namespace Svelto.Context
{
    using WeakReferenceI = DataStructures.HashableWeakRef<IOnFrameworkInitialized>;
    using WeakReferenceD = DataStructures.HashableWeakRef<IOnFrameworkDestroyed>;

    public class ContextNotifier : IContextNotifer
    {
        public ContextNotifier()
        {
            _toInitialize = new HashSet<WeakReferenceI>();
            _toDeinitialize = new HashSet<WeakReferenceD>();
        }

        public void AddFrameworkInitializationListener(IOnFrameworkInitialized obj)
        {
            DesignByContract.Check.Require(obj != null, "Attempted to add null framework initialization listener");

            if (_toInitialize != null)
                _toInitialize.Add(new WeakReferenceI(obj));
            else
                throw new ContextException("An object is expected to be initialized after the framework has been initialized. Type: ".FastConcat(obj.GetType().ToString()));
        }

        public void AddFrameworkDestructionListener(IOnFrameworkDestroyed obj)
        {
            DesignByContract.Check.Require(obj != null, "Attempted to add null framework destruction listener");

            if (_toDeinitialize != null)
                _toDeinitialize.Add(new WeakReferenceD(obj));
            else
                throw new ContextException("An object is expected to be deinitialized after the framework has been deinitialized. Type: ".FastConcat(obj.GetType().ToString()));
        }

        /// <summary>
        /// A Context is meant to be deinitialized only once in its timelife
        /// </summary>
        public void NotifyFrameworkDeinitialized()
        {
            //for (var i = _toDeinitialize.Count - 1; i >= 0; --i)
            foreach (var obj in _toDeinitialize)
                try
                {
                    //var obj = _toDeinitialize[i];
                    if (obj.IsAlive)
                        obj.Target.OnFrameworkDestroyed();
                }
                catch (Exception e)
                {
                    Utility.Console.LogException(e);
                }

            _toDeinitialize = null;
        }

        /// <summary>
        /// A Context is meant to be initialized only once in its timelife
        /// </summary>
        public void NotifyFrameworkInitialized()
        {
            //for (var i = _toInitialize.Count - 1; i >= 0; --i)
            foreach(var obj in _toInitialize)
            try
            {
                //var obj = _toInitialize[i];
                if (obj.IsAlive)
                    obj.Target.OnFrameworkInitialized();
            }
            catch (Exception e)
            {
                Utility.Console.LogException(e);
            }

            _toInitialize = null;
        }

        HashSet<WeakReferenceD> _toDeinitialize;
        HashSet<WeakReferenceI> _toInitialize;
    }
}
