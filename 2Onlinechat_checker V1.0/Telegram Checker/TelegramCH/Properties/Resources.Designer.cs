using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TelegramCH.Properties
{
	// Token: 0x0200000A RID: 10
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00006A24 File Offset: 0x00004C24
		internal Resources()
		{
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00006A2C File Offset: 0x00004C2C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("TelegramCH.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00006A5C File Offset: 0x00004C5C
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00006A64 File Offset: 0x00004C64
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00006A6C File Offset: 0x00004C6C
		internal static Bitmap _120_120__1_
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("120_120 (1)", Resources.resourceCulture);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00006A88 File Offset: 0x00004C88
		internal static Bitmap favicon_120x120
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("favicon-120x120", Resources.resourceCulture);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00006AA4 File Offset: 0x00004CA4
		internal static Bitmap telegram
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("telegram", Resources.resourceCulture);
			}
		}

		// Token: 0x0400007E RID: 126
		private static ResourceManager resourceMan;

		// Token: 0x0400007F RID: 127
		private static CultureInfo resourceCulture;
	}
}
