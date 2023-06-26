namespace Svelto.Context.Unity
{
    public interface ICompositionRoot
    {
        void OnContextCreated(UnityContext contextHolder);
        void OnContextInitialized();
        void OnContextDestroyed();
    }
}


