using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000A5 RID: 165
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class JsonConverterAttribute : Attribute
	{
		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x000399B0 File Offset: 0x00037BB0
		public Type ConverterType
		{
			get
			{
				return this._converterType;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060007DE RID: 2014 RVA: 0x000399B8 File Offset: 0x00037BB8
		public object[] ConverterParameters { get; }

		// Token: 0x060007DF RID: 2015 RVA: 0x000399C0 File Offset: 0x00037BC0
		public JsonConverterAttribute(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			this._converterType = converterType;
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x000399E8 File Offset: 0x00037BE8
		public JsonConverterAttribute(Type converterType, params object[] converterParameters)
			: this(converterType)
		{
			this.ConverterParameters = converterParameters;
		}

		// Token: 0x0400036F RID: 879
		private readonly Type _converterType;
	}
}
