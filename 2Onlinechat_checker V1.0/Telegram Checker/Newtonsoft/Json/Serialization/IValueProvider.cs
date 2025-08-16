using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200010C RID: 268
	public interface IValueProvider
	{
		// Token: 0x06000DBB RID: 3515
		void SetValue(object target, object value);

		// Token: 0x06000DBC RID: 3516
		object GetValue(object target);
	}
}
