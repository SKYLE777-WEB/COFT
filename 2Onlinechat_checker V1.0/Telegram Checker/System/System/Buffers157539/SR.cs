using System;
using System.Resources;
using System.Runtime.CompilerServices;
using FxResources.System.Buffers;

namespace System
{
	// Token: 0x02000088 RID: 136
	internal static class System.Buffers157539.SR
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x000241D8 File Offset: 0x000223D8
		private static ResourceManager ResourceManager
		{
			get
			{
				ResourceManager resourceManager;
				if ((resourceManager = System.Buffers157539.SR.s_resourceManager) == null)
				{
					resourceManager = (System.Buffers157539.SR.s_resourceManager = new ResourceManager(System.Buffers157539.SR.ResourceType));
				}
				return resourceManager;
			}
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x000241F8 File Offset: 0x000223F8
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool UsingResourceKeys()
		{
			return false;
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x000241FC File Offset: 0x000223FC
		internal static string GetResourceString(string resourceKey, string defaultString)
		{
			string text = null;
			try
			{
				text = System.Buffers157539.SR.ResourceManager.GetString(resourceKey);
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

		// Token: 0x06000695 RID: 1685 RVA: 0x00024248 File Offset: 0x00022448
		internal static string Format(string resourceFormat, params object[] args)
		{
			if (args == null)
			{
				return resourceFormat;
			}
			if (System.Buffers157539.SR.UsingResourceKeys())
			{
				return resourceFormat + string.Join(", ", args);
			}
			return string.Format(resourceFormat, args);
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00024278 File Offset: 0x00022478
		internal static string Format(string resourceFormat, object p1)
		{
			if (System.Buffers157539.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1 });
			}
			return string.Format(resourceFormat, p1);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x000242A4 File Offset: 0x000224A4
		internal static string Format(string resourceFormat, object p1, object p2)
		{
			if (System.Buffers157539.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2 });
			}
			return string.Format(resourceFormat, p1, p2);
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x000242D8 File Offset: 0x000224D8
		internal static string Format(string resourceFormat, object p1, object p2, object p3)
		{
			if (System.Buffers157539.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[] { resourceFormat, p1, p2, p3 });
			}
			return string.Format(resourceFormat, p1, p2, p3);
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000699 RID: 1689 RVA: 0x00024310 File Offset: 0x00022510
		internal static Type ResourceType { get; } = typeof(FxResources.System.Buffers.SR);

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x00024318 File Offset: 0x00022518
		internal static string ArgumentException_BufferNotFromPool
		{
			get
			{
				return System.Buffers157539.SR.GetResourceString("ArgumentException_BufferNotFromPool", null);
			}
		}

		// Token: 0x040002E9 RID: 745
		private static ResourceManager s_resourceManager;
	}
}
