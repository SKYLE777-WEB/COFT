using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace LanguageIdentification
{
	// Token: 0x02000019 RID: 25
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resource
	{
		// Token: 0x060000DC RID: 220 RVA: 0x00007A8C File Offset: 0x00005C8C
		internal Resource()
		{
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00007A94 File Offset: 0x00005C94
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resource.resourceMan == null)
				{
					Resource.resourceMan = new ResourceManager("LanguageIdentification.Resource", typeof(Resource).Assembly);
				}
				return Resource.resourceMan;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00007AC4 File Offset: 0x00005CC4
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00007ACC File Offset: 0x00005CCC
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resource.resourceCulture;
			}
			set
			{
				Resource.resourceCulture = value;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00007AD4 File Offset: 0x00005CD4
		internal static byte[] ModelData
		{
			get
			{
				return (byte[])Resource.ResourceManager.GetObject("ModelData", Resource.resourceCulture);
			}
		}

		// Token: 0x0400009F RID: 159
		private static ResourceManager resourceMan;

		// Token: 0x040000A0 RID: 160
		private static CultureInfo resourceCulture;
	}
}
