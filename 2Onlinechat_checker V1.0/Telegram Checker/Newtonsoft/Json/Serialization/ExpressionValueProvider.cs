using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000106 RID: 262
	public class ExpressionValueProvider : IValueProvider
	{
		// Token: 0x06000DAD RID: 3501 RVA: 0x00055474 File Offset: 0x00053674
		public ExpressionValueProvider(MemberInfo memberInfo)
		{
			ValidationUtils.ArgumentNotNull(memberInfo, "memberInfo");
			this._memberInfo = memberInfo;
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x00055490 File Offset: 0x00053690
		public void SetValue(object target, object value)
		{
			try
			{
				if (this._setter == null)
				{
					this._setter = ExpressionReflectionDelegateFactory.Instance.CreateSet<object>(this._memberInfo);
				}
				this._setter(target, value);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, this._memberInfo.Name, target.GetType()), ex);
			}
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x00055508 File Offset: 0x00053708
		public object GetValue(object target)
		{
			object obj;
			try
			{
				if (this._getter == null)
				{
					this._getter = ExpressionReflectionDelegateFactory.Instance.CreateGet<object>(this._memberInfo);
				}
				obj = this._getter(target);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, this._memberInfo.Name, target.GetType()), ex);
			}
			return obj;
		}

		// Token: 0x0400055A RID: 1370
		private readonly MemberInfo _memberInfo;

		// Token: 0x0400055B RID: 1371
		private Func<object, object> _getter;

		// Token: 0x0400055C RID: 1372
		private Action<object, object> _setter;
	}
}
