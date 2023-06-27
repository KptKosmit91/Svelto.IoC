using System;

#if !SIMPLIFY_SVELTO_NAMESPACES
namespace Svelto.Context
{
#endif
	public interface IOnFrameworkInitialized
	{
		void OnFrameworkInitialized();
	}
#if !SIMPLIFY_SVELTO_NAMESPACES
}
#endif

namespace Svelto.Context
{
	[Obsolete("Use Svelto.Context.IOnFrameworkInitialized instead.")]
	public interface IWaitForFrameworkInitialization : IOnFrameworkInitialized
	{
	}
}

