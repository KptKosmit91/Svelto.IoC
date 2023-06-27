using System;

#if !SIMPLIFY_SVELTO_NAMESPACES
namespace Svelto.Context
{
#endif
	public interface IOnFrameworkDestroyed
	{
		void OnFrameworkDestroyed();
	}
#if !SIMPLIFY_SVELTO_NAMESPACES
}
#endif

namespace Svelto.Context
{
	[Obsolete("Use Svelto.Context.IOnFrameworkDestroyed instead.")]
	public interface IWaitForFrameworkDestruction : IOnFrameworkDestroyed
	{
	}
}

