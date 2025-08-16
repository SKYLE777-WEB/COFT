using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000129 RID: 297
	public class ReflectionValueProvider : IValueProvider
	{
		// Token: 0x06000F5E RID: 3934 RVA: 0x0005DE8C File Offset: 0x0005C08C
		public ReflectionValueProvider(MemberInfo memberInfo)
		{
			ValidationUtils.ArgumentNotNull(memberInfo, "memberInfo");
			this._memberInfo = memberInfo;
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x0005DEA8 File Offset: 0x0005C0A8
		public void SetValue(object target, object value)
		{
			try
			{
				ReflectionUtils.SetMemberValue(this._memberInfo, target, value);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, this._memberInfo.Name, target.GetType()), ex);
			}
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x0005DF00 File Offset: 0x0005C100
		public object GetValue(object target)
		{
			object memberValue;
			try
			{
				memberValue = ReflectionUtils.GetMemberValue(this._memberInfo, target);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, this._memberInfo.Name, target.GetType()), ex);
			}
			return memberValue;
		}

		// Token: 0x040005F3 RID: 1523
		private readonly MemberInfo _memberInfo;
	}
}
