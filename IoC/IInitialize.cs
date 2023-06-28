using System;

#if !SIMPLIFY_SVELTO_NAMESPACES
namespace Svelto.IoC
{
#endif
	public interface IInitialize
	{
		void OnDependenciesInjected();
	}
#if !SIMPLIFY_SVELTO_NAMESPACES
}
#endif
