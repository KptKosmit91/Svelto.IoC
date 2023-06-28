using Svelto.Context.Unity.Internal;
using Svelto.IoC;
using System;

namespace Svelto.Context.Unity
{
    public class ContextMonoBehaviourFactory : MonoBehaviourFactory
    {
        public ContextMonoBehaviourFactory(IContainer container) : base()
        {
            _container = container;
        }

        /// <summary>
        /// Alternative constructor, specify a <c>IMonoBehaviourRoot</c> to notify when a MonoBehaviour is built. Ignored if null.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="monoBehaviourRoot"></param>
        public ContextMonoBehaviourFactory(IContainer container, IMonoBehaviourRoot monoBehaviourRoot) : this(container)
        {
            _mbRoot = monoBehaviourRoot;
        }

        override public M Build<M>(Func<M> constructor)
        {
            var copy =  base.Build(constructor);
            
            _container.Inject(copy);

            if(_mbRoot != null)
            {
                _mbRoot.MonoBehaviourAdded(copy);

                var obj = copy.gameObject.AddComponent<GameObjectDestroyedHandler>();
                obj.Init(copy, _mbRoot);
            }

            return copy;
        }

        readonly IMonoBehaviourRoot _mbRoot;

        readonly IContainer _container;
    }
}
