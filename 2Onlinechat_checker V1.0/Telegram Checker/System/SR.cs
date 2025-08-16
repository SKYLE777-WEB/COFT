using System;
using System.Resources;
using System.Runtime.CompilerServices;
using FxResources.System.Memory;

namespace System
{
	// Token: 0x02000035 RID: 53
	internal static class SR
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000251 RID: 593 RVA: 0x00010704 File Offset: 0x0000E904
		private static ResourceManager ResourceManager
		{
			get
			{
				ResourceManager resourceManager;
				if ((resourceManager = global::System.SR.s_resourceManager) == null)
				{
					resourceManager = (global::System.SR.s_resourceManager = new ResourceManager(global::System.SR.ResourceType));
				}
				return resourceManager;
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00010724 File Offset: 0x0000E924
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool UsingResourceKeys()
		{
			return false;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00010728 File Offset: 0x0000E928
		internal static string GetResourceString(string resourceKey, string defaultString)
		{
			string text = null;
			try
			{
				text = global::System.SR.ResourceManager.GetString(resourceKey);
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

		// Token: 0x06000254 RID: 596 RVA: 0x00010774 File Offset: 0x0000E974
		internal static string Format(string resourceFormat, params object[] args)
		{
			if (args == null)
			{
				return resourceFormat;
			}
			if (global::System.SR.UsingResourceKeys())
			{
				return resourceFormat + string.Join(", ", args);
			}
			return string.Format(resourceFormat, args);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x000107A4 File Offset: 0x0000E9A4
		internal static string Format(string resourceFormat, object p1)
		{
			if (global::System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1 });
			}
			return string.Format(resourceFormat, p1);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x000107D0 File Offset: 0x0000E9D0
		internal static string Format(string resourceFormat, object p1, object p2)
		{
			if (global::System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2 });
			}
			return string.Format(resourceFormat, p1, p2);
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00010804 File Offset: 0x0000EA04
		internal static string Format(string resourceFormat, object p1, object p2, object p3)
		{
			if (global::System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2, p3 });
			}
			return string.Format(resourceFormat, p1, p2, p3);
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0001083C File Offset: 0x0000EA3C
		internal static Type ResourceType { get; } = typeof(FxResources.System.Memory.SR);

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000259 RID: 601 RVA: 0x00010844 File Offset: 0x0000EA44
		internal static string NotSupported_CannotCallEqualsOnSpan
		{
			get
			{
				return global::System.SR.GetResourceString("NotSupported_CannotCallEqualsOnSpan", null);
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600025A RID: 602 RVA: 0x00010854 File Offset: 0x0000EA54
		internal static string NotSupported_CannotCallGetHashCodeOnSpan
		{
			get
			{
				return global::System.SR.GetResourceString("NotSupported_CannotCallGetHashCodeOnSpan", null);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600025B RID: 603 RVA: 0x00010864 File Offset: 0x0000EA64
		internal static string Argument_InvalidTypeWithPointersNotSupported
		{
			get
			{
				return global::System.SR.GetResourceString("Argument_InvalidTypeWithPointersNotSupported", null);
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600025C RID: 604 RVA: 0x00010874 File Offset: 0x0000EA74
		internal static string Argument_DestinationTooShort
		{
			get
			{
				return global::System.SR.GetResourceString("Argument_DestinationTooShort", null);
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600025D RID: 605 RVA: 0x00010884 File Offset: 0x0000EA84
		internal static string MemoryDisposed
		{
			get
			{
				return global::System.SR.GetResourceString("MemoryDisposed", null);
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600025E RID: 606 RVA: 0x00010894 File Offset: 0x0000EA94
		internal static string OutstandingReferences
		{
			get
			{
				return global::System.SR.GetResourceString("OutstandingReferences", null);
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600025F RID: 607 RVA: 0x000108A4 File Offset: 0x0000EAA4
		internal static string Argument_BadFormatSpecifier
		{
			get
			{
				return global::System.SR.GetResourceString("Argument_BadFormatSpecifier", null);
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000260 RID: 608 RVA: 0x000108B4 File Offset: 0x0000EAB4
		internal static string Argument_GWithPrecisionNotSupported
		{
			get
			{
				return global::System.SR.GetResourceString("Argument_GWithPrecisionNotSupported", null);
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000261 RID: 609 RVA: 0x000108C4 File Offset: 0x0000EAC4
		internal static string Argument_CannotParsePrecision
		{
			get
			{
				return global::System.SR.GetResourceString("Argument_CannotParsePrecision", null);
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000262 RID: 610 RVA: 0x000108D4 File Offset: 0x0000EAD4
		internal static string Argument_PrecisionTooLarge
		{
			get
			{
				return global::System.SR.GetResourceString("Argument_PrecisionTooLarge", null);
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000263 RID: 611 RVA: 0x000108E4 File Offset: 0x0000EAE4
		internal static string Argument_OverlapAlignmentMismatch
		{
			get
			{
				return global::System.SR.GetResourceString("Argument_OverlapAlignmentMismatch", null);
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000264 RID: 612 RVA: 0x000108F4 File Offset: 0x0000EAF4
		internal static string EndPositionNotReached
		{
			get
			{
				return global::System.SR.GetResourceString("EndPositionNotReached", null);
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000265 RID: 613 RVA: 0x00010904 File Offset: 0x0000EB04
		internal static string UnexpectedSegmentType
		{
			get
			{
				return global::System.SR.GetResourceString("UnexpectedSegmentType", null);
			}
		}

		// Token: 0x04000121 RID: 289
		private static ResourceManager s_resourceManager;
	}
}
