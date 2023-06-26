using Svelto.IoC;
using System;

namespace Svelto.Context
{
    public class ContextMonoBehaviourFactory : MonoBehaviourFactory
    {
        public ContextMonoBehaviourFactory(IContainer container):base()
        {
            _container = container;
        }

        override public M Build<M>(Func<M> constructor)
        {
            var copy =  base.Build(constructor);
            
            _container.Inject(copy);

            return copy;
        }
 
        IContainer _container;
    }
}
