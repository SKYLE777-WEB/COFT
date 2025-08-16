using System;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000117 RID: 279
	internal class JsonFormatterConverter : IFormatterConverter
	{
		// Token: 0x06000E1B RID: 3611 RVA: 0x00056710 File Offset: 0x00054910
		public JsonFormatterConverter(JsonSerializerInternalReader reader, JsonISerializableContract contract, JsonProperty member)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			ValidationUtils.ArgumentNotNull(contract, "contract");
			this._reader = reader;
			this._contract = contract;
			this._member = member;
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x00056744 File Offset: 0x00054944
		private T GetTokenValue<T>(object value)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			return (T)((object)global::System.Convert.ChangeType(((JValue)value).Value, typeof(T), CultureInfo.InvariantCulture));
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x00056778 File Offset: 0x00054978
		public object Convert(object value, Type type)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JToken jtoken = value as JToken;
			if (jtoken == null)
			{
				throw new ArgumentException("Value is not a JToken.", "value");
			}
			return this._reader.CreateISerializableItem(jtoken, type, this._contract, this._member);
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x000567CC File Offset: 0x000549CC
		public object Convert(object value, TypeCode typeCode)
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			if (value is JValue)
			{
				value = ((JValue)value).Value;
			}
			return global::System.Convert.ChangeType(value, typeCode, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x00056800 File Offset: 0x00054A00
		public bool ToBoolean(object value)
		{
			return this.GetTokenValue<bool>(value);
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x0005680C File Offset: 0x00054A0C
		public byte ToByte(object value)
		{
			return this.GetTokenValue<byte>(value);
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x00056818 File Offset: 0x00054A18
		public char ToChar(object value)
		{
			return this.GetTokenValue<char>(value);
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x00056824 File Offset: 0x00054A24
		public DateTime ToDateTime(object value)
		{
			return this.GetTokenValue<DateTime>(value);
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x00056830 File Offset: 0x00054A30
		public decimal ToDecimal(object value)
		{
			return this.GetTokenValue<decimal>(value);
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x0005683C File Offset: 0x00054A3C
		public double ToDouble(object value)
		{
			return this.GetTokenValue<double>(value);
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x00056848 File Offset: 0x00054A48
		public short ToInt16(object value)
		{
			return this.GetTokenValue<short>(value);
		}

		// Token: 0x06000E26 RID: 3622 RVA: 0x00056854 File Offset: 0x00054A54
		public int ToInt32(object value)
		{
			return this.GetTokenValue<int>(value);
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x00056860 File Offset: 0x00054A60
		public long ToInt64(object value)
		{
			return this.GetTokenValue<long>(value);
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x0005686C File Offset: 0x00054A6C
		public sbyte ToSByte(object value)
		{
			return this.GetTokenValue<sbyte>(value);
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x00056878 File Offset: 0x00054A78
		public float ToSingle(object value)
		{
			return this.GetTokenValue<float>(value);
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x00056884 File Offset: 0x00054A84
		public string ToString(object value)
		{
			return this.GetTokenValue<string>(value);
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x00056890 File Offset: 0x00054A90
		public ushort ToUInt16(object value)
		{
			return this.GetTokenValue<ushort>(value);
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x0005689C File Offset: 0x00054A9C
		public uint ToUInt32(object value)
		{
			return this.GetTokenValue<uint>(value);
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x000568A8 File Offset: 0x00054AA8
		public ulong ToUInt64(object value)
		{
			return this.GetTokenValue<ulong>(value);
		}

		// Token: 0x040005A0 RID: 1440
		private readonly JsonSerializerInternalReader _reader;

		// Token: 0x040005A1 RID: 1441
		private readonly JsonISerializableContract _contract;

		// Token: 0x040005A2 RID: 1442
		private readonly JsonProperty _member;
	}
}
