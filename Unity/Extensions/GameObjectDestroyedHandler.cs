using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Svelto.Context.Unity.Internal
{
    /// <summary>
    /// Internal. Automatically calls <c>IMonoBehaviourRoot.MonoBehaviourRemoved</c> on associated behaviours when the object is destroyed.
    /// </summary>
    internal class GameObjectDestroyedHandler : MonoBehaviour
    {
        MonoBehaviour _singleBehaviour;

        MonoBehaviour[] _behaviours;

        IMonoBehaviourRoot _mbRoot;

        internal void Init(MonoBehaviour behaviour, IMonoBehaviourRoot root)
        {
            _singleBehaviour = behaviour;
            _mbRoot = root;
        }

        internal void Init(MonoBehaviour[] behaviours, IMonoBehaviourRoot root)
        {
            _behaviours = behaviours;
            _mbRoot = root;
        }

        private void OnDestroy()
        {
            if(_singleBehaviour != null)
            {
                _mbRoot.MonoBehaviourRemoved(_singleBehaviour);
                return; // the array will always be null if this isn't null, so don't bother going further.
            }

            if (_behaviours != null) {
                int len = _behaviours.Length;

                for (int i = 0; i < len; i++)
                {
                    var behaviour = _behaviours[i];
                    if (behaviour != null)
                    {
                        _mbRoot.MonoBehaviourRemoved(behaviour);
                    }
                }
            }
        }
    }
}
