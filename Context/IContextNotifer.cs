namespace Svelto.Context
{
    public interface IContextNotifer
    {
        void NotifyFrameworkInitialized();
        void NotifyFrameworkDeinitialized();

        void AddFrameworkInitializationListener(IOnFrameworkInitialized obj); 
        void AddFrameworkDestructionListener(IOnFrameworkDestroyed obj);
    }
}
