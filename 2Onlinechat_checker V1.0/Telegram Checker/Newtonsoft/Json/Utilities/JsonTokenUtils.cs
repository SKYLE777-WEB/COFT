using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000E7 RID: 231
	internal static class JsonTokenUtils
	{
		// Token: 0x06000C92 RID: 3218 RVA: 0x0004FF40 File Offset: 0x0004E140
		internal static bool IsEndToken(JsonToken token)
		{
			return token - JsonToken.EndObject <= 2;
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x0004FF50 File Offset: 0x0004E150
		internal static bool IsStartToken(JsonToken token)
		{
			return token - JsonToken.StartObject <= 2;
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x0004FF60 File Offset: 0x0004E160
		internal static bool IsPrimitiveToken(JsonToken token)
		{
			return token - JsonToken.Integer <= 5 || token - JsonToken.Date <= 1;
		}
	}
}
