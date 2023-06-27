using Svelto.Unity.Log;
using System.Collections;
using UnityEngine;

namespace Svelto.Context.Unity
{
    public abstract class UnityContext : MonoBehaviour
    {
        protected abstract void OnAwake();

        void Awake()
        {
            if (Utility.Console.logger == null || Utility.Console.logger is UnitySlowLogger == false)
            {
                Utility.Console.logger = new UnitySlowLogger();
            }

            OnAwake();
        }
    }

    public class UnityContext<T> : UnityContext where T : class, ICompositionRoot, new()
    {
        protected override void OnAwake()
        {
            _root = new T();

            _root.OnContextCreated(this);
        }

        void OnDestroy()
        {
            _root.OnContextDestroyed();
        }

        void Start()
        {
            if (Application.isPlaying == true)
                StartCoroutine(WaitForFrameworkInitialization());
        }

        IEnumerator WaitForFrameworkInitialization()
        {
            //let's wait until the end of the frame, so we are sure that all the awake and starts are called
            yield return new WaitForEndOfFrame();

            _root.OnContextInitialized();
        }

        T _root;
    }
}
