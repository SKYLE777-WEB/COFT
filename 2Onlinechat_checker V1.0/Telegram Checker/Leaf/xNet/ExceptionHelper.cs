using System;

namespace Leaf.xNet
{
	// Token: 0x0200006E RID: 110
	internal static class ExceptionHelper
	{
		// Token: 0x060005C8 RID: 1480 RVA: 0x000214EC File Offset: 0x0001F6EC
		internal static ArgumentException EmptyString(string paramName)
		{
			return new ArgumentException(Resources.ArgumentException_EmptyString, paramName);
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x000214FC File Offset: 0x0001F6FC
		internal static ArgumentOutOfRangeException CanNotBeLess<T>(string paramName, T value) where T : struct
		{
			return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeLess, value));
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00021514 File Offset: 0x0001F714
		internal static ArgumentOutOfRangeException CanNotBeGreater<T>(string paramName, T value) where T : struct
		{
			return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeGreater, value));
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0002152C File Offset: 0x0001F72C
		internal static ArgumentException WrongPath(string paramName, Exception innerException = null)
		{
			return new ArgumentException(Resources.ArgumentException_WrongPath, paramName, innerException);
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x0002153C File Offset: 0x0001F73C
		internal static ArgumentOutOfRangeException WrongTcpPort(string paramName)
		{
			return new ArgumentOutOfRangeException(paramName, string.Format(Resources.ArgumentOutOfRangeException_CanNotBeLessOrGreater, 1, 65535));
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00021560 File Offset: 0x0001F760
		internal static bool ValidateTcpPort(int port)
		{
			return port >= 1 && port <= 65535;
		}
	}
}
