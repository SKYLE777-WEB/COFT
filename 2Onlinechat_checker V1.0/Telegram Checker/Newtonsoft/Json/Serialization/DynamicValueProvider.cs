using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000103 RID: 259
	public class DynamicValueProvider : IValueProvider
	{
		// Token: 0x06000D9E RID: 3486 RVA: 0x000552CC File Offset: 0x000534CC
		public DynamicValueProvider(MemberInfo memberInfo)
		{
			ValidationUtils.ArgumentNotNull(memberInfo, "memberInfo");
			this._memberInfo = memberInfo;
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x000552E8 File Offset: 0x000534E8
		public void SetValue(object target, object value)
		{
			try
			{
				if (this._setter == null)
				{
					this._setter = DynamicReflectionDelegateFactory.Instance.CreateSet<object>(this._memberInfo);
				}
				this._setter(target, value);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, this._memberInfo.Name, target.GetType()), ex);
			}
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x00055360 File Offset: 0x00053560
		public object GetValue(object target)
		{
			object obj;
			try
			{
				if (this._getter == null)
				{
					this._getter = DynamicReflectionDelegateFactory.Instance.CreateGet<object>(this._memberInfo);
				}
				obj = this._getter(target);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, this._memberInfo.Name, target.GetType()), ex);
			}
			return obj;
		}

		// Token: 0x0400054F RID: 1359
		private readonly MemberInfo _memberInfo;

		// Token: 0x04000550 RID: 1360
		private Func<object, object> _getter;

		// Token: 0x04000551 RID: 1361
		private Action<object, object> _setter;
	}
}
