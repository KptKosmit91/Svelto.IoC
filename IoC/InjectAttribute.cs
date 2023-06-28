using System;

#if !SIMPLIFY_SVELTO_NAMESPACES
namespace Svelto.IoC
{
#endif
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class InjectAttribute: Attribute
	{
        public string name { get; set; }
	}
#if !SIMPLIFY_SVELTO_NAMESPACES
}
#endif
