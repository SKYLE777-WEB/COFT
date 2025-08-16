using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000120 RID: 288
	internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
	{
		// Token: 0x06000EC5 RID: 3781 RVA: 0x0005B288 File Offset: 0x00059488
		public JsonSerializerInternalWriter(JsonSerializer serializer)
			: base(serializer)
		{
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0005B29C File Offset: 0x0005949C
		public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
		{
			if (jsonWriter == null)
			{
				throw new ArgumentNullException("jsonWriter");
			}
			this._rootType = objectType;
			this._rootLevel = this._serializeStack.Count + 1;
			JsonContract contractSafe = this.GetContractSafe(value);
			try
			{
				if (this.ShouldWriteReference(value, null, contractSafe, null, null))
				{
					this.WriteReference(jsonWriter, value);
				}
				else
				{
					this.SerializeValue(jsonWriter, value, contractSafe, null, null, null);
				}
			}
			catch (Exception ex)
			{
				if (!base.IsErrorHandled(null, contractSafe, null, null, jsonWriter.Path, ex))
				{
					base.ClearErrorContext();
					throw;
				}
				this.HandleError(jsonWriter, 0);
			}
			finally
			{
				this._rootType = null;
			}
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x0005B360 File Offset: 0x00059560
		private JsonSerializerProxy GetInternalSerializer()
		{
			if (this.InternalSerializer == null)
			{
				this.InternalSerializer = new JsonSerializerProxy(this);
			}
			return this.InternalSerializer;
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x0005B380 File Offset: 0x00059580
		private JsonContract GetContractSafe(object value)
		{
			if (value == null)
			{
				return null;
			}
			return this.Serializer._contractResolver.ResolveContract(value.GetType());
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x0005B3A0 File Offset: 0x000595A0
		private void SerializePrimitive(JsonWriter writer, object value, JsonPrimitiveContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			if (contract.TypeCode == PrimitiveTypeCode.Bytes && this.ShouldWriteType(TypeNameHandling.Objects, contract, member, containerContract, containerProperty))
			{
				writer.WriteStartObject();
				this.WriteTypeProperty(writer, contract.CreatedType);
				writer.WritePropertyName("$value", false);
				JsonWriter.WriteValue(writer, contract.TypeCode, value);
				writer.WriteEndObject();
				return;
			}
			JsonWriter.WriteValue(writer, contract.TypeCode, value);
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0005B414 File Offset: 0x00059614
		private void SerializeValue(JsonWriter writer, object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			JsonConverter jsonConverter;
			if ((jsonConverter = ((member != null) ? member.Converter : null)) == null && (jsonConverter = ((containerProperty != null) ? containerProperty.ItemConverter : null)) == null && (jsonConverter = ((containerContract != null) ? containerContract.ItemConverter : null)) == null && (jsonConverter = valueContract.Converter) == null)
			{
				jsonConverter = this.Serializer.GetMatchingConverter(valueContract.UnderlyingType) ?? valueContract.InternalConverter;
			}
			JsonConverter jsonConverter2 = jsonConverter;
			if (jsonConverter2 != null && jsonConverter2.CanWrite)
			{
				this.SerializeConvertable(writer, jsonConverter2, value, valueContract, containerContract, containerProperty);
				return;
			}
			switch (valueContract.ContractType)
			{
			case JsonContractType.Object:
				this.SerializeObject(writer, value, (JsonObjectContract)valueContract, member, containerContract, containerProperty);
				return;
			case JsonContractType.Array:
			{
				JsonArrayContract jsonArrayContract = (JsonArrayContract)valueContract;
				if (!jsonArrayContract.IsMultidimensionalArray)
				{
					this.SerializeList(writer, (IEnumerable)value, jsonArrayContract, member, containerContract, containerProperty);
					return;
				}
				this.SerializeMultidimensionalArray(writer, (Array)value, jsonArrayContract, member, containerContract, containerProperty);
				return;
			}
			case JsonContractType.Primitive:
				this.SerializePrimitive(writer, value, (JsonPrimitiveContract)valueContract, member, containerContract, containerProperty);
				return;
			case JsonContractType.String:
				this.SerializeString(writer, value, (JsonStringContract)valueContract);
				return;
			case JsonContractType.Dictionary:
			{
				JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)valueContract;
				IDictionary dictionary2;
				if (!(value is IDictionary))
				{
					IDictionary dictionary = jsonDictionaryContract.CreateWrapper(value);
					dictionary2 = dictionary;
				}
				else
				{
					dictionary2 = (IDictionary)value;
				}
				this.SerializeDictionary(writer, dictionary2, jsonDictionaryContract, member, containerContract, containerProperty);
				return;
			}
			case JsonContractType.Dynamic:
				this.SerializeDynamic(writer, (IDynamicMetaObjectProvider)value, (JsonDynamicContract)valueContract, member, containerContract, containerProperty);
				return;
			case JsonContractType.Serializable:
				this.SerializeISerializable(writer, (ISerializable)value, (JsonISerializableContract)valueContract, member, containerContract, containerProperty);
				return;
			case JsonContractType.Linq:
				((JToken)value).WriteTo(writer, this.Serializer.Converters.ToArray<JsonConverter>());
				return;
			default:
				return;
			}
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0005B5FC File Offset: 0x000597FC
		private bool? ResolveIsReference(JsonContract contract, JsonProperty property, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			bool? flag = null;
			if (property != null)
			{
				flag = property.IsReference;
			}
			if (flag == null && containerProperty != null)
			{
				flag = containerProperty.ItemIsReference;
			}
			if (flag == null && collectionContract != null)
			{
				flag = collectionContract.ItemIsReference;
			}
			if (flag == null)
			{
				flag = contract.IsReference;
			}
			return flag;
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0005B66C File Offset: 0x0005986C
		private bool ShouldWriteReference(object value, JsonProperty property, JsonContract valueContract, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			if (value == null)
			{
				return false;
			}
			if (valueContract.ContractType == JsonContractType.Primitive || valueContract.ContractType == JsonContractType.String)
			{
				return false;
			}
			bool? flag = this.ResolveIsReference(valueContract, property, collectionContract, containerProperty);
			if (flag == null)
			{
				if (valueContract.ContractType == JsonContractType.Array)
				{
					flag = new bool?(this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays));
				}
				else
				{
					flag = new bool?(this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects));
				}
			}
			return flag.GetValueOrDefault() && this.Serializer.GetReferenceResolver().IsReferenced(this, value);
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0005B71C File Offset: 0x0005991C
		private bool ShouldWriteProperty(object memberValue, JsonProperty property)
		{
			return (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) != NullValueHandling.Ignore || memberValue != null) && (!this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) || !MiscellaneousUtils.ValueEquals(memberValue, property.GetResolvedDefaultValue()));
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0005B790 File Offset: 0x00059990
		private bool CheckForCircularReference(JsonWriter writer, object value, JsonProperty property, JsonContract contract, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			if (value == null || contract.ContractType == JsonContractType.Primitive || contract.ContractType == JsonContractType.String)
			{
				return true;
			}
			ReferenceLoopHandling? referenceLoopHandling = null;
			if (property != null)
			{
				referenceLoopHandling = property.ReferenceLoopHandling;
			}
			if (referenceLoopHandling == null && containerProperty != null)
			{
				referenceLoopHandling = containerProperty.ItemReferenceLoopHandling;
			}
			if (referenceLoopHandling == null && containerContract != null)
			{
				referenceLoopHandling = containerContract.ItemReferenceLoopHandling;
			}
			if ((this.Serializer._equalityComparer != null) ? this._serializeStack.Contains(value, this.Serializer._equalityComparer) : this._serializeStack.Contains(value))
			{
				string text = "Self referencing loop detected";
				if (property != null)
				{
					text += " for property '{0}'".FormatWith(CultureInfo.InvariantCulture, property.PropertyName);
				}
				text += " with type '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType());
				switch (referenceLoopHandling.GetValueOrDefault(this.Serializer._referenceLoopHandling))
				{
				case ReferenceLoopHandling.Error:
					throw JsonSerializationException.Create(null, writer.ContainerPath, text, null);
				case ReferenceLoopHandling.Ignore:
					if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
					{
						this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, text + ". Skipping serializing self referenced value."), null);
					}
					return false;
				case ReferenceLoopHandling.Serialize:
					if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
					{
						this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, text + ". Serializing self referenced value."), null);
					}
					return true;
				}
			}
			return true;
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x0005B950 File Offset: 0x00059B50
		private void WriteReference(JsonWriter writer, object value)
		{
			string reference = this.GetReference(writer, value);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Writing object reference to Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, reference, value.GetType())), null);
			}
			writer.WriteStartObject();
			writer.WritePropertyName("$ref", false);
			writer.WriteValue(reference);
			writer.WriteEndObject();
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0005B9D4 File Offset: 0x00059BD4
		private string GetReference(JsonWriter writer, object value)
		{
			string reference;
			try
			{
				reference = this.Serializer.GetReferenceResolver().GetReference(this, value);
			}
			catch (Exception ex)
			{
				throw JsonSerializationException.Create(null, writer.ContainerPath, "Error writing object reference for '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), ex);
			}
			return reference;
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x0005BA30 File Offset: 0x00059C30
		internal static bool TryConvertToString(object value, Type type, out string s)
		{
			TypeConverter typeConverter;
			if (JsonTypeReflector.CanTypeDescriptorConvertString(type, out typeConverter))
			{
				s = typeConverter.ConvertToInvariantString(value);
				return true;
			}
			type = value as Type;
			if (type != null)
			{
				s = type.AssemblyQualifiedName;
				return true;
			}
			s = null;
			return false;
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x0005BA7C File Offset: 0x00059C7C
		private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
		{
			this.OnSerializing(writer, contract, value);
			string text;
			JsonSerializerInternalWriter.TryConvertToString(value, contract.UnderlyingType, out text);
			writer.WriteValue(text);
			this.OnSerialized(writer, contract, value);
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x0005BAB8 File Offset: 0x00059CB8
		private void OnSerializing(JsonWriter writer, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Started serializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnSerializing(value, this.Serializer._context);
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x0005BB28 File Offset: 0x00059D28
		private void OnSerialized(JsonWriter writer, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Finished serializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnSerialized(value, this.Serializer._context);
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0005BB98 File Offset: 0x00059D98
		private void SerializeObject(JsonWriter writer, object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			this.OnSerializing(writer, contract, value);
			this._serializeStack.Add(value);
			this.WriteObjectStart(writer, value, contract, member, collectionContract, containerProperty);
			int top = writer.Top;
			for (int i = 0; i < contract.Properties.Count; i++)
			{
				JsonProperty jsonProperty = contract.Properties[i];
				try
				{
					JsonContract jsonContract;
					object obj;
					if (this.CalculatePropertyValues(writer, value, contract, member, jsonProperty, out jsonContract, out obj))
					{
						jsonProperty.WritePropertyName(writer);
						this.SerializeValue(writer, obj, jsonContract, jsonProperty, contract, member);
					}
				}
				catch (Exception ex)
				{
					if (!base.IsErrorHandled(value, contract, jsonProperty.PropertyName, null, writer.ContainerPath, ex))
					{
						throw;
					}
					this.HandleError(writer, top);
				}
			}
			ExtensionDataGetter extensionDataGetter = contract.ExtensionDataGetter;
			IEnumerable<KeyValuePair<object, object>> enumerable = ((extensionDataGetter != null) ? extensionDataGetter(value) : null);
			if (enumerable != null)
			{
				foreach (KeyValuePair<object, object> keyValuePair in enumerable)
				{
					JsonContract contractSafe = this.GetContractSafe(keyValuePair.Key);
					JsonContract contractSafe2 = this.GetContractSafe(keyValuePair.Value);
					bool flag;
					string text = this.GetPropertyName(writer, keyValuePair.Key, contractSafe, out flag);
					text = ((contract.ExtensionDataNameResolver != null) ? contract.ExtensionDataNameResolver(text) : text);
					if (this.ShouldWriteReference(keyValuePair.Value, null, contractSafe2, contract, member))
					{
						writer.WritePropertyName(text);
						this.WriteReference(writer, keyValuePair.Value);
					}
					else if (this.CheckForCircularReference(writer, keyValuePair.Value, null, contractSafe2, contract, member))
					{
						writer.WritePropertyName(text);
						this.SerializeValue(writer, keyValuePair.Value, contractSafe2, null, contract, member);
					}
				}
			}
			writer.WriteEndObject();
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, value);
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0005BDB8 File Offset: 0x00059FB8
		private bool CalculatePropertyValues(JsonWriter writer, object value, JsonContainerContract contract, JsonProperty member, JsonProperty property, out JsonContract memberContract, out object memberValue)
		{
			if (!property.Ignored && property.Readable && this.ShouldSerialize(writer, property, value) && this.IsSpecified(writer, property, value))
			{
				if (property.PropertyContract == null)
				{
					property.PropertyContract = this.Serializer._contractResolver.ResolveContract(property.PropertyType);
				}
				memberValue = property.ValueProvider.GetValue(value);
				memberContract = (property.PropertyContract.IsSealed ? property.PropertyContract : this.GetContractSafe(memberValue));
				if (this.ShouldWriteProperty(memberValue, property))
				{
					if (this.ShouldWriteReference(memberValue, property, memberContract, contract, member))
					{
						property.WritePropertyName(writer);
						this.WriteReference(writer, memberValue);
						return false;
					}
					if (!this.CheckForCircularReference(writer, memberValue, property, memberContract, contract, member))
					{
						return false;
					}
					if (memberValue == null)
					{
						JsonObjectContract jsonObjectContract = contract as JsonObjectContract;
						Required required = property._required ?? ((jsonObjectContract != null) ? jsonObjectContract.ItemRequired : null) ?? Required.Default;
						if (required == Required.Always)
						{
							throw JsonSerializationException.Create(null, writer.ContainerPath, "Cannot write a null value for property '{0}'. Property requires a value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName), null);
						}
						if (required == Required.DisallowNull)
						{
							throw JsonSerializationException.Create(null, writer.ContainerPath, "Cannot write a null value for property '{0}'. Property requires a non-null value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName), null);
						}
					}
					return true;
				}
			}
			memberContract = null;
			memberValue = null;
			return false;
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x0005BF78 File Offset: 0x0005A178
		private void WriteObjectStart(JsonWriter writer, object value, JsonContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			writer.WriteStartObject();
			if ((this.ResolveIsReference(contract, member, collectionContract, containerProperty) ?? this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects)) && (member == null || member.Writable || this.HasCreatorParameter(collectionContract, member)))
			{
				this.WriteReferenceIdProperty(writer, contract.UnderlyingType, value);
			}
			if (this.ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionContract, containerProperty))
			{
				this.WriteTypeProperty(writer, contract.UnderlyingType);
			}
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0005C01C File Offset: 0x0005A21C
		private bool HasCreatorParameter(JsonContainerContract contract, JsonProperty property)
		{
			JsonObjectContract jsonObjectContract = contract as JsonObjectContract;
			return jsonObjectContract != null && jsonObjectContract.CreatorParameters.Contains(property.PropertyName);
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x0005C050 File Offset: 0x0005A250
		private void WriteReferenceIdProperty(JsonWriter writer, Type type, object value)
		{
			string reference = this.GetReference(writer, value);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "Writing object reference Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, reference, type)), null);
			}
			writer.WritePropertyName("$id", false);
			writer.WriteValue(reference);
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x0005C0C4 File Offset: 0x0005A2C4
		private void WriteTypeProperty(JsonWriter writer, Type type)
		{
			string typeName = ReflectionUtils.GetTypeName(type, this.Serializer._typeNameAssemblyFormatHandling, this.Serializer._serializationBinder);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "Writing type name '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, typeName, type)), null);
			}
			writer.WritePropertyName("$type", false);
			writer.WriteValue(typeName);
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x0005C14C File Offset: 0x0005A34C
		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x0005C154 File Offset: 0x0005A354
		private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
		{
			return (value & flag) == flag;
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0005C15C File Offset: 0x0005A35C
		private bool HasFlag(TypeNameHandling value, TypeNameHandling flag)
		{
			return (value & flag) == flag;
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x0005C164 File Offset: 0x0005A364
		private void SerializeConvertable(JsonWriter writer, JsonConverter converter, object value, JsonContract contract, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			if (this.ShouldWriteReference(value, null, contract, collectionContract, containerProperty))
			{
				this.WriteReference(writer, value);
				return;
			}
			if (!this.CheckForCircularReference(writer, value, null, contract, collectionContract, containerProperty))
			{
				return;
			}
			this._serializeStack.Add(value);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Started serializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, value.GetType(), converter.GetType())), null);
			}
			converter.WriteJson(writer, value, this.GetInternalSerializer());
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Finished serializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, value.GetType(), converter.GetType())), null);
			}
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x0005C278 File Offset: 0x0005A478
		private void SerializeList(JsonWriter writer, IEnumerable values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			IWrappedCollection wrappedCollection = values as IWrappedCollection;
			object obj = ((wrappedCollection != null) ? wrappedCollection.UnderlyingCollection : values);
			this.OnSerializing(writer, contract, obj);
			this._serializeStack.Add(obj);
			bool flag = this.WriteStartArray(writer, obj, contract, member, collectionContract, containerProperty);
			writer.WriteStartArray();
			int top = writer.Top;
			int num = 0;
			foreach (object obj2 in values)
			{
				try
				{
					JsonContract jsonContract = contract.FinalItemContract ?? this.GetContractSafe(obj2);
					if (this.ShouldWriteReference(obj2, null, jsonContract, contract, member))
					{
						this.WriteReference(writer, obj2);
					}
					else if (this.CheckForCircularReference(writer, obj2, null, jsonContract, contract, member))
					{
						this.SerializeValue(writer, obj2, jsonContract, null, contract, member);
					}
				}
				catch (Exception ex)
				{
					if (!base.IsErrorHandled(obj, contract, num, null, writer.ContainerPath, ex))
					{
						throw;
					}
					this.HandleError(writer, top);
				}
				finally
				{
					num++;
				}
			}
			writer.WriteEndArray();
			if (flag)
			{
				writer.WriteEndObject();
			}
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, obj);
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x0005C408 File Offset: 0x0005A608
		private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			this.OnSerializing(writer, contract, values);
			this._serializeStack.Add(values);
			bool flag = this.WriteStartArray(writer, values, contract, member, collectionContract, containerProperty);
			this.SerializeMultidimensionalArray(writer, values, contract, member, writer.Top, CollectionUtils.ArrayEmpty<int>());
			if (flag)
			{
				writer.WriteEndObject();
			}
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, values);
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x0005C480 File Offset: 0x0005A680
		private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, int initialDepth, int[] indices)
		{
			int num = indices.Length;
			int[] array = new int[num + 1];
			for (int i = 0; i < num; i++)
			{
				array[i] = indices[i];
			}
			writer.WriteStartArray();
			int j = values.GetLowerBound(num);
			while (j <= values.GetUpperBound(num))
			{
				array[num] = j;
				if (array.Length == values.Rank)
				{
					object value = values.GetValue(array);
					try
					{
						JsonContract jsonContract = contract.FinalItemContract ?? this.GetContractSafe(value);
						if (this.ShouldWriteReference(value, null, jsonContract, contract, member))
						{
							this.WriteReference(writer, value);
						}
						else if (this.CheckForCircularReference(writer, value, null, jsonContract, contract, member))
						{
							this.SerializeValue(writer, value, jsonContract, null, contract, member);
						}
						goto IL_00F9;
					}
					catch (Exception ex)
					{
						if (base.IsErrorHandled(values, contract, j, null, writer.ContainerPath, ex))
						{
							this.HandleError(writer, initialDepth + 1);
							goto IL_00F9;
						}
						throw;
					}
					goto IL_00E9;
				}
				goto IL_00E9;
				IL_00F9:
				j++;
				continue;
				IL_00E9:
				this.SerializeMultidimensionalArray(writer, values, contract, member, initialDepth + 1, array);
				goto IL_00F9;
			}
			writer.WriteEndArray();
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x0005C5B0 File Offset: 0x0005A7B0
		private bool WriteStartArray(JsonWriter writer, object values, JsonArrayContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			bool flag = this.ResolveIsReference(contract, member, containerContract, containerProperty) ?? this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays);
			flag = flag && (member == null || member.Writable || this.HasCreatorParameter(containerContract, member));
			bool flag2 = this.ShouldWriteType(TypeNameHandling.Arrays, contract, member, containerContract, containerProperty);
			bool flag3 = flag || flag2;
			if (flag3)
			{
				writer.WriteStartObject();
				if (flag)
				{
					this.WriteReferenceIdProperty(writer, contract.UnderlyingType, values);
				}
				if (flag2)
				{
					this.WriteTypeProperty(writer, values.GetType());
				}
				writer.WritePropertyName("$values", false);
			}
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.Serializer._contractResolver.ResolveContract(contract.CollectionItemType ?? typeof(object));
			}
			return flag3;
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x0005C6B4 File Offset: 0x0005A8B4
		[SecuritySafeCritical]
		private void SerializeISerializable(JsonWriter writer, ISerializable value, JsonISerializableContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				string text = "Type '{0}' implements ISerializable but cannot be serialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data." + Environment.NewLine + "To fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true." + Environment.NewLine;
				text = text.FormatWith(CultureInfo.InvariantCulture, value.GetType());
				throw JsonSerializationException.Create(null, writer.ContainerPath, text, null);
			}
			this.OnSerializing(writer, contract, value);
			this._serializeStack.Add(value);
			this.WriteObjectStart(writer, value, contract, member, collectionContract, containerProperty);
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new FormatterConverter());
			value.GetObjectData(serializationInfo, this.Serializer._context);
			foreach (SerializationEntry serializationEntry in serializationInfo)
			{
				JsonContract contractSafe = this.GetContractSafe(serializationEntry.Value);
				if (this.ShouldWriteReference(serializationEntry.Value, null, contractSafe, contract, member))
				{
					writer.WritePropertyName(serializationEntry.Name);
					this.WriteReference(writer, serializationEntry.Value);
				}
				else if (this.CheckForCircularReference(writer, serializationEntry.Value, null, contractSafe, contract, member))
				{
					writer.WritePropertyName(serializationEntry.Name);
					this.SerializeValue(writer, serializationEntry.Value, contractSafe, null, contract, member);
				}
			}
			writer.WriteEndObject();
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, value);
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x0005C818 File Offset: 0x0005AA18
		private void SerializeDynamic(JsonWriter writer, IDynamicMetaObjectProvider value, JsonDynamicContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			this.OnSerializing(writer, contract, value);
			this._serializeStack.Add(value);
			this.WriteObjectStart(writer, value, contract, member, collectionContract, containerProperty);
			int top = writer.Top;
			for (int i = 0; i < contract.Properties.Count; i++)
			{
				JsonProperty jsonProperty = contract.Properties[i];
				if (jsonProperty.HasMemberAttribute)
				{
					try
					{
						JsonContract jsonContract;
						object obj;
						if (this.CalculatePropertyValues(writer, value, contract, member, jsonProperty, out jsonContract, out obj))
						{
							jsonProperty.WritePropertyName(writer);
							this.SerializeValue(writer, obj, jsonContract, jsonProperty, contract, member);
						}
					}
					catch (Exception ex)
					{
						if (!base.IsErrorHandled(value, contract, jsonProperty.PropertyName, null, writer.ContainerPath, ex))
						{
							throw;
						}
						this.HandleError(writer, top);
					}
				}
			}
			foreach (string text in value.GetDynamicMemberNames())
			{
				object obj2;
				if (contract.TryGetMember(value, text, out obj2))
				{
					try
					{
						JsonContract contractSafe = this.GetContractSafe(obj2);
						if (this.ShouldWriteDynamicProperty(obj2))
						{
							if (this.CheckForCircularReference(writer, obj2, null, contractSafe, contract, member))
							{
								string text2 = ((contract.PropertyNameResolver != null) ? contract.PropertyNameResolver(text) : text);
								writer.WritePropertyName(text2);
								this.SerializeValue(writer, obj2, contractSafe, null, contract, member);
							}
						}
					}
					catch (Exception ex2)
					{
						if (!base.IsErrorHandled(value, contract, text, null, writer.ContainerPath, ex2))
						{
							throw;
						}
						this.HandleError(writer, top);
					}
				}
			}
			writer.WriteEndObject();
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, value);
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0005CA20 File Offset: 0x0005AC20
		private bool ShouldWriteDynamicProperty(object memberValue)
		{
			return (this.Serializer._nullValueHandling != NullValueHandling.Ignore || memberValue != null) && (!this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Ignore) || (memberValue != null && !MiscellaneousUtils.ValueEquals(memberValue, ReflectionUtils.GetDefaultValue(memberValue.GetType()))));
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0005CA80 File Offset: 0x0005AC80
		private bool ShouldWriteType(TypeNameHandling typeNameHandlingFlag, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			TypeNameHandling typeNameHandling = ((member != null) ? member.TypeNameHandling : null) ?? ((containerProperty != null) ? containerProperty.ItemTypeNameHandling : null) ?? ((containerContract != null) ? containerContract.ItemTypeNameHandling : null) ?? this.Serializer._typeNameHandling;
			if (this.HasFlag(typeNameHandling, typeNameHandlingFlag))
			{
				return true;
			}
			if (this.HasFlag(typeNameHandling, TypeNameHandling.Auto))
			{
				if (member != null)
				{
					if (contract.NonNullableUnderlyingType != member.PropertyContract.CreatedType)
					{
						return true;
					}
				}
				else if (containerContract != null)
				{
					if (containerContract.ItemContract == null || contract.NonNullableUnderlyingType != containerContract.ItemContract.CreatedType)
					{
						return true;
					}
				}
				else if (this._rootType != null && this._serializeStack.Count == this._rootLevel)
				{
					JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(this._rootType);
					if (contract.NonNullableUnderlyingType != jsonContract.CreatedType)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0005CC08 File Offset: 0x0005AE08
		private void SerializeDictionary(JsonWriter writer, IDictionary values, JsonDictionaryContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			IWrappedDictionary wrappedDictionary = values as IWrappedDictionary;
			object obj = ((wrappedDictionary != null) ? wrappedDictionary.UnderlyingDictionary : values);
			this.OnSerializing(writer, contract, obj);
			this._serializeStack.Add(obj);
			this.WriteObjectStart(writer, obj, contract, member, collectionContract, containerProperty);
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.Serializer._contractResolver.ResolveContract(contract.DictionaryValueType ?? typeof(object));
			}
			if (contract.KeyContract == null)
			{
				contract.KeyContract = this.Serializer._contractResolver.ResolveContract(contract.DictionaryKeyType ?? typeof(object));
			}
			int top = writer.Top;
			using (IDictionaryEnumerator enumerator = values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry entry = enumerator.Entry;
					bool flag;
					string text = this.GetPropertyName(writer, entry.Key, contract.KeyContract, out flag);
					text = ((contract.DictionaryKeyResolver != null) ? contract.DictionaryKeyResolver(text) : text);
					try
					{
						object value = entry.Value;
						JsonContract jsonContract = contract.FinalItemContract ?? this.GetContractSafe(value);
						if (this.ShouldWriteReference(value, null, jsonContract, contract, member))
						{
							writer.WritePropertyName(text, flag);
							this.WriteReference(writer, value);
						}
						else if (this.CheckForCircularReference(writer, value, null, jsonContract, contract, member))
						{
							writer.WritePropertyName(text, flag);
							this.SerializeValue(writer, value, jsonContract, null, contract, member);
						}
					}
					catch (Exception ex)
					{
						if (!base.IsErrorHandled(obj, contract, text, null, writer.ContainerPath, ex))
						{
							throw;
						}
						this.HandleError(writer, top);
					}
				}
			}
			writer.WriteEndObject();
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, obj);
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0005CE4C File Offset: 0x0005B04C
		private string GetPropertyName(JsonWriter writer, object name, JsonContract contract, out bool escape)
		{
			if (contract.ContractType == JsonContractType.Primitive)
			{
				switch (((JsonPrimitiveContract)contract).TypeCode)
				{
				case PrimitiveTypeCode.Single:
				case PrimitiveTypeCode.SingleNullable:
				{
					float num = (float)name;
					escape = false;
					return num.ToString("R", CultureInfo.InvariantCulture);
				}
				case PrimitiveTypeCode.Double:
				case PrimitiveTypeCode.DoubleNullable:
				{
					double num2 = (double)name;
					escape = false;
					return num2.ToString("R", CultureInfo.InvariantCulture);
				}
				case PrimitiveTypeCode.DateTime:
				case PrimitiveTypeCode.DateTimeNullable:
				{
					DateTime dateTime = DateTimeUtils.EnsureDateTime((DateTime)name, writer.DateTimeZoneHandling);
					escape = false;
					StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
					DateTimeUtils.WriteDateTimeString(stringWriter, dateTime, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
					return stringWriter.ToString();
				}
				case PrimitiveTypeCode.DateTimeOffset:
				case PrimitiveTypeCode.DateTimeOffsetNullable:
				{
					escape = false;
					StringWriter stringWriter2 = new StringWriter(CultureInfo.InvariantCulture);
					DateTimeUtils.WriteDateTimeOffsetString(stringWriter2, (DateTimeOffset)name, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
					return stringWriter2.ToString();
				}
				default:
					escape = true;
					return Convert.ToString(name, CultureInfo.InvariantCulture);
				}
			}
			else
			{
				string text;
				if (JsonSerializerInternalWriter.TryConvertToString(name, name.GetType(), out text))
				{
					escape = true;
					return text;
				}
				escape = true;
				return name.ToString();
			}
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0005CF84 File Offset: 0x0005B184
		private void HandleError(JsonWriter writer, int initialDepth)
		{
			base.ClearErrorContext();
			if (writer.WriteState == WriteState.Property)
			{
				writer.WriteNull();
			}
			while (writer.Top > initialDepth)
			{
				writer.WriteEnd();
			}
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x0005CFC4 File Offset: 0x0005B1C4
		private bool ShouldSerialize(JsonWriter writer, JsonProperty property, object target)
		{
			if (property.ShouldSerialize == null)
			{
				return true;
			}
			bool flag = property.ShouldSerialize(target);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "ShouldSerialize result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, flag)), null);
			}
			return flag;
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x0005D048 File Offset: 0x0005B248
		private bool IsSpecified(JsonWriter writer, JsonProperty property, object target)
		{
			if (property.GetIsSpecified == null)
			{
				return true;
			}
			bool flag = property.GetIsSpecified(target);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "IsSpecified result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, flag)), null);
			}
			return flag;
		}

		// Token: 0x040005DA RID: 1498
		private Type _rootType;

		// Token: 0x040005DB RID: 1499
		private int _rootLevel;

		// Token: 0x040005DC RID: 1500
		private readonly List<object> _serializeStack = new List<object>();
	}
}
