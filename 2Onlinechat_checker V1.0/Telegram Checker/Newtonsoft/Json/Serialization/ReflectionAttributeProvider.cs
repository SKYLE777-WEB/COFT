using System;
using System.Collections.Generic;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000128 RID: 296
	public class ReflectionAttributeProvider : IAttributeProvider
	{
		// Token: 0x06000F5B RID: 3931 RVA: 0x0005DE50 File Offset: 0x0005C050
		public ReflectionAttributeProvider(object attributeProvider)
		{
			ValidationUtils.ArgumentNotNull(attributeProvider, "attributeProvider");
			this._attributeProvider = attributeProvider;
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0005DE6C File Offset: 0x0005C06C
		public IList<Attribute> GetAttributes(bool inherit)
		{
			return ReflectionUtils.GetAttributes(this._attributeProvider, null, inherit);
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x0005DE7C File Offset: 0x0005C07C
		public IList<Attribute> GetAttributes(Type attributeType, bool inherit)
		{
			return ReflectionUtils.GetAttributes(this._attributeProvider, attributeType, inherit);
		}

		// Token: 0x040005F2 RID: 1522
		private readonly object _attributeProvider;
	}
}
