using UnityEngine;

namespace Svelto.Context.Unity
{
    public interface IMonoBehaviourRoot
    {
        void MonoBehaviourAdded(MonoBehaviour mb);
        void MonoBehaviourRemoved(MonoBehaviour mb);
    }
}
