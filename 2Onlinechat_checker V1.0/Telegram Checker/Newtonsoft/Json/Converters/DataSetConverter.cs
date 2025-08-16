using System;
using System.Data;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000168 RID: 360
	public class DataSetConverter : JsonConverter
	{
		// Token: 0x0600135A RID: 4954 RVA: 0x0006C444 File Offset: 0x0006A644
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			DataSet dataSet = (DataSet)value;
			DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
			DataTableConverter dataTableConverter = new DataTableConverter();
			writer.WriteStartObject();
			foreach (object obj in dataSet.Tables)
			{
				DataTable dataTable = (DataTable)obj;
				writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName(dataTable.TableName) : dataTable.TableName);
				dataTableConverter.WriteJson(writer, dataTable, serializer);
			}
			writer.WriteEndObject();
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x0006C4F4 File Offset: 0x0006A6F4
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			DataSet dataSet = ((objectType == typeof(DataSet)) ? new DataSet() : ((DataSet)Activator.CreateInstance(objectType)));
			DataTableConverter dataTableConverter = new DataTableConverter();
			reader.ReadAndAssert();
			while (reader.TokenType == JsonToken.PropertyName)
			{
				DataTable dataTable = dataSet.Tables[(string)reader.Value];
				bool flag = dataTable != null;
				dataTable = (DataTable)dataTableConverter.ReadJson(reader, typeof(DataTable), dataTable, serializer);
				if (!flag)
				{
					dataSet.Tables.Add(dataTable);
				}
				reader.ReadAndAssert();
			}
			return dataSet;
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x0006C5A8 File Offset: 0x0006A7A8
		public override bool CanConvert(Type valueType)
		{
			return typeof(DataSet).IsAssignableFrom(valueType);
		}
	}
}
