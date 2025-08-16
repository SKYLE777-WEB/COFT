using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D7 RID: 215
	internal class DynamicProxy<T>
	{
		// Token: 0x06000C04 RID: 3076 RVA: 0x0004CB08 File Offset: 0x0004AD08
		public virtual IEnumerable<string> GetDynamicMemberNames(T instance)
		{
			return CollectionUtils.ArrayEmpty<string>();
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0004CB10 File Offset: 0x0004AD10
		public virtual bool TryBinaryOperation(T instance, BinaryOperationBinder binder, object arg, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0004CB18 File Offset: 0x0004AD18
		public virtual bool TryConvert(T instance, ConvertBinder binder, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0004CB20 File Offset: 0x0004AD20
		public virtual bool TryCreateInstance(T instance, CreateInstanceBinder binder, object[] args, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0004CB28 File Offset: 0x0004AD28
		public virtual bool TryDeleteIndex(T instance, DeleteIndexBinder binder, object[] indexes)
		{
			return false;
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0004CB2C File Offset: 0x0004AD2C
		public virtual bool TryDeleteMember(T instance, DeleteMemberBinder binder)
		{
			return false;
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0004CB30 File Offset: 0x0004AD30
		public virtual bool TryGetIndex(T instance, GetIndexBinder binder, object[] indexes, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0004CB38 File Offset: 0x0004AD38
		public virtual bool TryGetMember(T instance, GetMemberBinder binder, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0004CB40 File Offset: 0x0004AD40
		public virtual bool TryInvoke(T instance, InvokeBinder binder, object[] args, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0004CB48 File Offset: 0x0004AD48
		public virtual bool TryInvokeMember(T instance, InvokeMemberBinder binder, object[] args, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0004CB50 File Offset: 0x0004AD50
		public virtual bool TrySetIndex(T instance, SetIndexBinder binder, object[] indexes, object value)
		{
			return false;
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0004CB54 File Offset: 0x0004AD54
		public virtual bool TrySetMember(T instance, SetMemberBinder binder, object value)
		{
			return false;
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0004CB58 File Offset: 0x0004AD58
		public virtual bool TryUnaryOperation(T instance, UnaryOperationBinder binder, out object result)
		{
			result = null;
			return false;
		}
	}
}
