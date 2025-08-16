using System;
using System.Resources;
using System.Runtime.CompilerServices;
using FxResources.System.Numerics.Vectors;

namespace System
{
	// Token: 0x0200008F RID: 143
	internal static class System.Numerics.Vectors159800.SR
	{
		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x00024900 File Offset: 0x00022B00
		private static ResourceManager ResourceManager
		{
			get
			{
				ResourceManager resourceManager;
				if ((resourceManager = System.Numerics.Vectors159800.SR.s_resourceManager) == null)
				{
					resourceManager = (System.Numerics.Vectors159800.SR.s_resourceManager = new ResourceManager(System.Numerics.Vectors159800.SR.ResourceType));
				}
				return resourceManager;
			}
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00024920 File Offset: 0x00022B20
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool UsingResourceKeys()
		{
			return false;
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x00024924 File Offset: 0x00022B24
		internal static string GetResourceString(string resourceKey, string defaultString)
		{
			string text = null;
			try
			{
				text = System.Numerics.Vectors159800.SR.ResourceManager.GetString(resourceKey);
			}
			catch (MissingManifestResourceException)
			{
			}
			if (defaultString != null && resourceKey.Equals(text, StringComparison.Ordinal))
			{
				return defaultString;
			}
			return text;
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00024970 File Offset: 0x00022B70
		internal static string Format(string resourceFormat, params object[] args)
		{
			if (args == null)
			{
				return resourceFormat;
			}
			if (System.Numerics.Vectors159800.SR.UsingResourceKeys())
			{
				return resourceFormat + string.Join(", ", args);
			}
			return string.Format(resourceFormat, args);
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x000249A0 File Offset: 0x00022BA0
		internal static string Format(string resourceFormat, object p1)
		{
			if (System.Numerics.Vectors159800.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1 });
			}
			return string.Format(resourceFormat, p1);
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x000249CC File Offset: 0x00022BCC
		internal static string Format(string resourceFormat, object p1, object p2)
		{
			if (System.Numerics.Vectors159800.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2 });
			}
			return string.Format(resourceFormat, p1, p2);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00024A00 File Offset: 0x00022C00
		internal static string Format(string resourceFormat, object p1, object p2, object p3)
		{
			if (System.Numerics.Vectors159800.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2, p3 });
			}
			return string.Format(resourceFormat, p1, p2, p3);
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x00024A38 File Offset: 0x00022C38
		internal static Type ResourceType { get; } = typeof(FxResources.System.Numerics.Vectors.SR);

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00024A40 File Offset: 0x00022C40
		internal static string Arg_ArgumentOutOfRangeException
		{
			get
			{
				return System.Numerics.Vectors159800.SR.GetResourceString("Arg_ArgumentOutOfRangeException", null);
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00024A50 File Offset: 0x00022C50
		internal static string Arg_ElementsInSourceIsGreaterThanDestination
		{
			get
			{
				return System.Numerics.Vectors159800.SR.GetResourceString("Arg_ElementsInSourceIsGreaterThanDestination", null);
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00024A60 File Offset: 0x00022C60
		internal static string Arg_NullArgumentNullRef
		{
			get
			{
				return System.Numerics.Vectors159800.SR.GetResourceString("Arg_NullArgumentNullRef", null);
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00024A70 File Offset: 0x00022C70
		internal static string Arg_TypeNotSupported
		{
			get
			{
				return System.Numerics.Vectors159800.SR.GetResourceString("Arg_TypeNotSupported", null);
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00024A80 File Offset: 0x00022C80
		internal static string Arg_InsufficientNumberOfElements
		{
			get
			{
				return System.Numerics.Vectors159800.SR.GetResourceString("Arg_InsufficientNumberOfElements", null);
			}
		}

		// Token: 0x040002F2 RID: 754
		private static ResourceManager s_resourceManager;
	}
}
