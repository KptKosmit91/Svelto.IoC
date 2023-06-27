using System;

#if !SIMPLIFY_SVELTO_NAMESPACES
namespace Svelto.IoC
{
#endif
	interface IInitialize
	{
		void OnDependenciesInjected();
	}
#if !SIMPLIFY_SVELTO_NAMESPACES
}
#endif
