using Svelto.Context.Unity.Internal;
using Svelto.IoC;
using UnityEngine;

namespace Svelto.Context.Unity
{
    public class ContextGameObjectFactory : GameObjectFactory
    {
        IMonoBehaviourRoot _mbRoot;

        public ContextGameObjectFactory(IContainer container) : base()
        {
            _container = container;
        }

        /// <summary>
        /// Alternative constructor, specify a <c>IMonoBehaviourRoot</c> to notify when a MonoBehaviour on a built GameObject is built. Ignored if null.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="monoBehaviourRoot"></param>
        public ContextGameObjectFactory(IContainer container, IMonoBehaviourRoot monoBehaviourRoot) : this(container)
        {
            _mbRoot = monoBehaviourRoot;
        }

        override public GameObject Build(GameObject prefab)
        {
            DesignByContract.Check.Require(prefab != null, "Svelto.Factories.IGameObjectFactory -null prefab");

            var copy =  base.Build(prefab);
            var monobehaviours = copy.GetComponentsInChildren<MonoBehaviour>(includeInactive: true);

            int len = monobehaviours.Length;
            for (int i = 0; i < len; ++i)
            {
                var mb = monobehaviours[i];
                _container.Inject(mb);

                if (_mbRoot != null)
                {
                    _mbRoot.MonoBehaviourAdded(mb);
                }
            }

            if(_mbRoot != null)
            {
                var obj = copy.AddComponent<GameObjectDestroyedHandler>();
                obj.Init(monobehaviours, _mbRoot);
            }

            return copy;
        }
        

        IContainer _container;
    }
}
