using System;
using System.Collections.Generic;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200011B RID: 283
	public class JsonPrimitiveContract : JsonContract
	{
		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00056B40 File Offset: 0x00054D40
		// (set) Token: 0x06000E48 RID: 3656 RVA: 0x00056B48 File Offset: 0x00054D48
		internal PrimitiveTypeCode TypeCode { get; set; }

		// Token: 0x06000E49 RID: 3657 RVA: 0x00056B54 File Offset: 0x00054D54
		public JsonPrimitiveContract(Type underlyingType)
			: base(underlyingType)
		{
			this.ContractType = JsonContractType.Primitive;
			this.TypeCode = ConvertUtils.GetTypeCode(underlyingType);
			this.IsReadOnlyOrFixedSize = true;
			ReadType readType;
			if (JsonPrimitiveContract.ReadTypeMap.TryGetValue(this.NonNullableUnderlyingType, out readType))
			{
				this.InternalReadType = readType;
			}
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x00056BA4 File Offset: 0x00054DA4
		// Note: this type is marked as 'beforefieldinit'.
		static JsonPrimitiveContract()
		{
			Dictionary<Type, ReadType> dictionary = new Dictionary<Type, ReadType>();
			Type typeFromHandle = typeof(byte[]);
			dictionary[typeFromHandle] = ReadType.ReadAsBytes;
			Type typeFromHandle2 = typeof(byte);
			dictionary[typeFromHandle2] = ReadType.ReadAsInt32;
			Type typeFromHandle3 = typeof(short);
			dictionary[typeFromHandle3] = ReadType.ReadAsInt32;
			Type typeFromHandle4 = typeof(int);
			dictionary[typeFromHandle4] = ReadType.ReadAsInt32;
			Type typeFromHandle5 = typeof(decimal);
			dictionary[typeFromHandle5] = ReadType.ReadAsDecimal;
			Type typeFromHandle6 = typeof(bool);
			dictionary[typeFromHandle6] = ReadType.ReadAsBoolean;
			Type typeFromHandle7 = typeof(string);
			dictionary[typeFromHandle7] = ReadType.ReadAsString;
			Type typeFromHandle8 = typeof(DateTime);
			dictionary[typeFromHandle8] = ReadType.ReadAsDateTime;
			Type typeFromHandle9 = typeof(DateTimeOffset);
			dictionary[typeFromHandle9] = ReadType.ReadAsDateTimeOffset;
			Type typeFromHandle10 = typeof(float);
			dictionary[typeFromHandle10] = ReadType.ReadAsDouble;
			Type typeFromHandle11 = typeof(double);
			dictionary[typeFromHandle11] = ReadType.ReadAsDouble;
			JsonPrimitiveContract.ReadTypeMap = dictionary;
		}

		// Token: 0x040005B1 RID: 1457
		private static readonly Dictionary<Type, ReadType> ReadTypeMap;
	}
}
