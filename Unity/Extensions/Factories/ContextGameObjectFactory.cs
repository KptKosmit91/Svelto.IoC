using UnityEngine;

namespace Svelto.IoC
{
    public class ContextGameObjectFactory : Context.GameObjectFactory
    {
        public ContextGameObjectFactory(IContainer container):base()
        {
            _container = container;
        }

        override public GameObject Build(GameObject prefab)
        {
            var copy =  base.Build(prefab);
            var monobehaviours = copy.GetComponentsInChildren<MonoBehaviour>();

            for (int i = 0; i < monobehaviours.Length; i++)
                _container.Inject(monobehaviours[i]);

            return copy;
        }
        

        IContainer _container;
    }
}
