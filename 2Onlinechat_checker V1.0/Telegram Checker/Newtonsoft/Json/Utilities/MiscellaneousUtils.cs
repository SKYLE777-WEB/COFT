using System;
using System.Globalization;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000EC RID: 236
	internal static class MiscellaneousUtils
	{
		// Token: 0x06000CAD RID: 3245 RVA: 0x000503D8 File Offset: 0x0004E5D8
		public static bool ValueEquals(object objA, object objB)
		{
			if (objA == objB)
			{
				return true;
			}
			if (objA == null || objB == null)
			{
				return false;
			}
			if (!(objA.GetType() != objB.GetType()))
			{
				return objA.Equals(objB);
			}
			if (ConvertUtils.IsInteger(objA) && ConvertUtils.IsInteger(objB))
			{
				return Convert.ToDecimal(objA, CultureInfo.CurrentCulture).Equals(Convert.ToDecimal(objB, CultureInfo.CurrentCulture));
			}
			return (objA is double || objA is float || objA is decimal) && (objB is double || objB is float || objB is decimal) && MathUtils.ApproxEquals(Convert.ToDouble(objA, CultureInfo.CurrentCulture), Convert.ToDouble(objB, CultureInfo.CurrentCulture));
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x000504B4 File Offset: 0x0004E6B4
		public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string paramName, object actualValue, string message)
		{
			string text = message + Environment.NewLine + "Actual value was {0}.".FormatWith(CultureInfo.InvariantCulture, actualValue);
			return new ArgumentOutOfRangeException(paramName, text);
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x000504E8 File Offset: 0x0004E6E8
		public static string ToString(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			if (!(value is string))
			{
				return value.ToString();
			}
			return "\"" + value.ToString() + "\"";
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x00050520 File Offset: 0x0004E720
		public static int ByteArrayCompare(byte[] a1, byte[] a2)
		{
			int num = a1.Length.CompareTo(a2.Length);
			if (num != 0)
			{
				return num;
			}
			for (int i = 0; i < a1.Length; i++)
			{
				int num2 = a1[i].CompareTo(a2[i]);
				if (num2 != 0)
				{
					return num2;
				}
			}
			return 0;
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x00050574 File Offset: 0x0004E774
		public static string GetPrefix(string qualifiedName)
		{
			string text;
			string text2;
			MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out text, out text2);
			return text;
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x00050590 File Offset: 0x0004E790
		public static string GetLocalName(string qualifiedName)
		{
			string text;
			string text2;
			MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, out text, out text2);
			return text2;
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000505AC File Offset: 0x0004E7AC
		public static void GetQualifiedNameParts(string qualifiedName, out string prefix, out string localName)
		{
			int num = qualifiedName.IndexOf(':');
			if (num == -1 || num == 0 || qualifiedName.Length - 1 == num)
			{
				prefix = null;
				localName = qualifiedName;
				return;
			}
			prefix = qualifiedName.Substring(0, num);
			localName = qualifiedName.Substring(num + 1);
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x00050600 File Offset: 0x0004E800
		internal static string FormatValueForPrint(object value)
		{
			if (value == null)
			{
				return "{null}";
			}
			if (value is string)
			{
				return "\"" + value + "\"";
			}
			return value.ToString();
		}
	}
}
