using Svelto.IoC;
using UnityEngine;

namespace Svelto.Context
{
    public class ContextGameObjectFactory : GameObjectFactory
    {
        public ContextGameObjectFactory(IContainer container):base()
        {
            _container = container;
        }

        override public GameObject Build(GameObject prefab)
        {
            DesignByContract.Check.Require(prefab != null, "Svelto.Factories.IGameObjectFactory -null prefab");

            var copy =  base.Build(prefab);
            var monobehaviours = copy.GetComponentsInChildren<MonoBehaviour>();

            int len = monobehaviours.Length;
            for (int i = 0; i < len; ++i)
                _container.Inject(monobehaviours[i]);

            return copy;
        }
        

        IContainer _container;
    }
}
