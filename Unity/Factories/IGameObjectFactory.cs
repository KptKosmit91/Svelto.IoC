using System;
using UnityEngine;

namespace Svelto.Factories
{
    public interface IGameObjectFactory
    {
        void RegisterPrefab(GameObject prefab, string type, GameObject parent = null);

        /// <param name="registeredObjId">ID of the registered object</param>
        /// <param name="preInit">Callback with gameobject, it may not be fully initialized in this state yet</param>
        GameObject Build(string registeredObjId, Action<GameObject> preInit = null);


        /// <param name="prefab">The prefab that will be spawned</param>
        /// <param name="preInit">Callback with gameobject, it may not be fully initialized in this state yet</param>
        GameObject Build(GameObject prefab, Action<GameObject> preInit = null);
    }
}
