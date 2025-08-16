using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000146 RID: 326
	public class JObject : JContainer, IDictionary<string, JToken>, ICollection<KeyValuePair<string, JToken>>, IEnumerable<KeyValuePair<string, JToken>>, IEnumerable, INotifyPropertyChanged, ICustomTypeDescriptor, INotifyPropertyChanging
	{
		// Token: 0x0600114E RID: 4430 RVA: 0x00064474 File Offset: 0x00062674
		public override async Task WriteToAsync(JsonWriter writer, CancellationToken cancellationToken, params JsonConverter[] converters)
		{
			await writer.WriteStartObjectAsync(cancellationToken).ConfigureAwait(false);
			for (int i = 0; i < this._properties.Count; i++)
			{
				await this._properties[i].WriteToAsync(writer, cancellationToken, converters).ConfigureAwait(false);
			}
			await writer.WriteEndObjectAsync(cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x000644D8 File Offset: 0x000626D8
		public new static Task<JObject> LoadAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return JObject.LoadAsync(reader, null, cancellationToken);
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x000644E4 File Offset: 0x000626E4
		public new static async Task<JObject> LoadAsync(JsonReader reader, JsonLoadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None && !(await reader.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader.");
			}
			await reader.MoveToContentAsync(cancellationToken).ConfigureAwait(false);
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JObject o = new JObject();
			o.SetLineInfo(reader as IJsonLineInfo, settings);
			await o.ReadTokenFromAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			return o;
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06001151 RID: 4433 RVA: 0x00064540 File Offset: 0x00062740
		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._properties;
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06001152 RID: 4434 RVA: 0x00064548 File Offset: 0x00062748
		// (remove) Token: 0x06001153 RID: 4435 RVA: 0x00064584 File Offset: 0x00062784
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06001154 RID: 4436 RVA: 0x000645C0 File Offset: 0x000627C0
		// (remove) Token: 0x06001155 RID: 4437 RVA: 0x000645FC File Offset: 0x000627FC
		public event PropertyChangingEventHandler PropertyChanging;

		// Token: 0x06001156 RID: 4438 RVA: 0x00064638 File Offset: 0x00062838
		public JObject()
		{
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x0006464C File Offset: 0x0006284C
		public JObject(JObject other)
			: base(other)
		{
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x00064660 File Offset: 0x00062860
		public JObject(params object[] content)
			: this(content)
		{
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0006466C File Offset: 0x0006286C
		public JObject(object content)
		{
			this.Add(content);
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x00064688 File Offset: 0x00062888
		internal override bool DeepEquals(JToken node)
		{
			JObject jobject = node as JObject;
			return jobject != null && this._properties.Compare(jobject._properties);
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x000646BC File Offset: 0x000628BC
		internal override int IndexOfItem(JToken item)
		{
			return this._properties.IndexOfReference(item);
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x000646CC File Offset: 0x000628CC
		internal override void InsertItem(int index, JToken item, bool skipParentCheck)
		{
			if (item != null && item.Type == JTokenType.Comment)
			{
				return;
			}
			base.InsertItem(index, item, skipParentCheck);
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x000646EC File Offset: 0x000628EC
		internal override void ValidateToken(JToken o, JToken existing)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type != JTokenType.Property)
			{
				throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), base.GetType()));
			}
			JProperty jproperty = (JProperty)o;
			if (existing != null)
			{
				JProperty jproperty2 = (JProperty)existing;
				if (jproperty.Name == jproperty2.Name)
				{
					return;
				}
			}
			if (this._properties.TryGetValue(jproperty.Name, out existing))
			{
				throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, jproperty.Name, base.GetType()));
			}
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0006479C File Offset: 0x0006299C
		internal override void MergeItem(object content, JsonMergeSettings settings)
		{
			JObject jobject = content as JObject;
			if (jobject == null)
			{
				return;
			}
			foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
			{
				JProperty jproperty = this.Property(keyValuePair.Key);
				if (jproperty == null)
				{
					this.Add(keyValuePair.Key, keyValuePair.Value);
				}
				else if (keyValuePair.Value != null)
				{
					JContainer jcontainer = jproperty.Value as JContainer;
					if (jcontainer == null || jcontainer.Type != keyValuePair.Value.Type)
					{
						if (keyValuePair.Value.Type != JTokenType.Null || (settings != null && settings.MergeNullValueHandling == MergeNullValueHandling.Merge))
						{
							jproperty.Value = keyValuePair.Value;
						}
					}
					else
					{
						jcontainer.Merge(keyValuePair.Value, settings);
					}
				}
			}
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x000648A4 File Offset: 0x00062AA4
		internal void InternalPropertyChanged(JProperty childProperty)
		{
			this.OnPropertyChanged(childProperty.Name);
			if (this._listChanged != null)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, this.IndexOfItem(childProperty)));
			}
			if (this._collectionChanged != null)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, childProperty, childProperty, this.IndexOfItem(childProperty)));
			}
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x00064900 File Offset: 0x00062B00
		internal void InternalPropertyChanging(JProperty childProperty)
		{
			this.OnPropertyChanging(childProperty.Name);
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x00064910 File Offset: 0x00062B10
		internal override JToken CloneToken()
		{
			return new JObject(this);
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06001162 RID: 4450 RVA: 0x00064918 File Offset: 0x00062B18
		public override JTokenType Type
		{
			get
			{
				return JTokenType.Object;
			}
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0006491C File Offset: 0x00062B1C
		public IEnumerable<JProperty> Properties()
		{
			return this._properties.Cast<JProperty>();
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0006492C File Offset: 0x00062B2C
		public JProperty Property(string name)
		{
			if (name == null)
			{
				return null;
			}
			JToken jtoken;
			this._properties.TryGetValue(name, out jtoken);
			return (JProperty)jtoken;
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0006495C File Offset: 0x00062B5C
		public JEnumerable<JToken> PropertyValues()
		{
			return new JEnumerable<JToken>(from p in this.Properties()
				select p.Value);
		}

		// Token: 0x17000329 RID: 809
		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this[text];
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this[text] = value;
			}
		}

		// Token: 0x1700032A RID: 810
		public JToken this[string propertyName]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
				JProperty jproperty = this.Property(propertyName);
				if (jproperty == null)
				{
					return null;
				}
				return jproperty.Value;
			}
			set
			{
				JProperty jproperty = this.Property(propertyName);
				if (jproperty != null)
				{
					jproperty.Value = value;
					return;
				}
				this.OnPropertyChanging(propertyName);
				this.Add(new JProperty(propertyName, value));
				this.OnPropertyChanged(propertyName);
			}
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x00064A90 File Offset: 0x00062C90
		public new static JObject Load(JsonReader reader)
		{
			return JObject.Load(reader, null);
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x00064A9C File Offset: 0x00062C9C
		public new static JObject Load(JsonReader reader, JsonLoadSettings settings)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader.");
			}
			reader.MoveToContent();
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JObject jobject = new JObject();
			jobject.SetLineInfo(reader as IJsonLineInfo, settings);
			jobject.ReadTokenFrom(reader, settings);
			return jobject;
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x00064B28 File Offset: 0x00062D28
		public new static JObject Parse(string json)
		{
			return JObject.Parse(json, null);
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x00064B34 File Offset: 0x00062D34
		public new static JObject Parse(string json, JsonLoadSettings settings)
		{
			JObject jobject2;
			using (JsonReader jsonReader = new JsonTextReader(new StringReader(json)))
			{
				JObject jobject = JObject.Load(jsonReader, settings);
				while (jsonReader.Read())
				{
				}
				jobject2 = jobject;
			}
			return jobject2;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x00064B84 File Offset: 0x00062D84
		public new static JObject FromObject(object o)
		{
			return JObject.FromObject(o, JsonSerializer.CreateDefault());
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x00064B94 File Offset: 0x00062D94
		public new static JObject FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jtoken = JToken.FromObjectInternal(o, jsonSerializer);
			if (jtoken != null && jtoken.Type != JTokenType.Object)
			{
				throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, jtoken.Type));
			}
			return (JObject)jtoken;
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00064BE8 File Offset: 0x00062DE8
		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartObject();
			for (int i = 0; i < this._properties.Count; i++)
			{
				this._properties[i].WriteTo(writer, converters);
			}
			writer.WriteEndObject();
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00064C34 File Offset: 0x00062E34
		public JToken GetValue(string propertyName)
		{
			return this.GetValue(propertyName, StringComparison.Ordinal);
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00064C40 File Offset: 0x00062E40
		public JToken GetValue(string propertyName, StringComparison comparison)
		{
			if (propertyName == null)
			{
				return null;
			}
			JProperty jproperty = this.Property(propertyName);
			if (jproperty != null)
			{
				return jproperty.Value;
			}
			if (comparison != StringComparison.Ordinal)
			{
				foreach (JToken jtoken in this._properties)
				{
					JProperty jproperty2 = (JProperty)jtoken;
					if (string.Equals(jproperty2.Name, propertyName, comparison))
					{
						return jproperty2.Value;
					}
				}
			}
			return null;
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x00064CDC File Offset: 0x00062EDC
		public bool TryGetValue(string propertyName, StringComparison comparison, out JToken value)
		{
			value = this.GetValue(propertyName, comparison);
			return value != null;
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x00064CF0 File Offset: 0x00062EF0
		public void Add(string propertyName, JToken value)
		{
			this.Add(new JProperty(propertyName, value));
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x00064D00 File Offset: 0x00062F00
		bool IDictionary<string, JToken>.ContainsKey(string key)
		{
			return this._properties.Contains(key);
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06001176 RID: 4470 RVA: 0x00064D10 File Offset: 0x00062F10
		ICollection<string> IDictionary<string, JToken>.Keys
		{
			get
			{
				return this._properties.Keys;
			}
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x00064D20 File Offset: 0x00062F20
		public bool Remove(string propertyName)
		{
			JProperty jproperty = this.Property(propertyName);
			if (jproperty == null)
			{
				return false;
			}
			jproperty.Remove();
			return true;
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x00064D48 File Offset: 0x00062F48
		public bool TryGetValue(string propertyName, out JToken value)
		{
			JProperty jproperty = this.Property(propertyName);
			if (jproperty == null)
			{
				value = null;
				return false;
			}
			value = jproperty.Value;
			return true;
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06001179 RID: 4473 RVA: 0x00064D78 File Offset: 0x00062F78
		ICollection<JToken> IDictionary<string, JToken>.Values
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x00064D80 File Offset: 0x00062F80
		void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
		{
			this.Add(new JProperty(item.Key, item.Value));
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x00064D9C File Offset: 0x00062F9C
		void ICollection<KeyValuePair<string, JToken>>.Clear()
		{
			base.RemoveAll();
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00064DA4 File Offset: 0x00062FA4
		bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
		{
			JProperty jproperty = this.Property(item.Key);
			return jproperty != null && jproperty.Value == item.Value;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00064DDC File Offset: 0x00062FDC
		void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
			}
			if (arrayIndex >= array.Length && arrayIndex != 0)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
			}
			if (base.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			foreach (JToken jtoken in this._properties)
			{
				JProperty jproperty = (JProperty)jtoken;
				array[arrayIndex + num] = new KeyValuePair<string, JToken>(jproperty.Name, jproperty.Value);
				num++;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x0600117E RID: 4478 RVA: 0x00064EB0 File Offset: 0x000630B0
		bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00064EB4 File Offset: 0x000630B4
		bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
		{
			if (!((ICollection<KeyValuePair<string, JToken>>)this).Contains(item))
			{
				return false;
			}
			((IDictionary<string, JToken>)this).Remove(item.Key);
			return true;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00064ED4 File Offset: 0x000630D4
		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00064EDC File Offset: 0x000630DC
		public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
		{
			foreach (JToken jtoken in this._properties)
			{
				JProperty jproperty = (JProperty)jtoken;
				yield return new KeyValuePair<string, JToken>(jproperty.Name, jproperty.Value);
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00064EEC File Offset: 0x000630EC
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x00064F08 File Offset: 0x00063108
		protected virtual void OnPropertyChanging(string propertyName)
		{
			PropertyChangingEventHandler propertyChanging = this.PropertyChanging;
			if (propertyChanging == null)
			{
				return;
			}
			propertyChanging(this, new PropertyChangingEventArgs(propertyName));
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00064F24 File Offset: 0x00063124
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x00064F30 File Offset: 0x00063130
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			foreach (KeyValuePair<string, JToken> keyValuePair in this)
			{
				propertyDescriptorCollection.Add(new JPropertyDescriptor(keyValuePair.Key));
			}
			return propertyDescriptorCollection;
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00064F98 File Offset: 0x00063198
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x00064FA0 File Offset: 0x000631A0
		string ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x00064FA4 File Offset: 0x000631A4
		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x00064FA8 File Offset: 0x000631A8
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return new TypeConverter();
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x00064FB0 File Offset: 0x000631B0
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00064FB4 File Offset: 0x000631B4
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x00064FB8 File Offset: 0x000631B8
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00064FBC File Offset: 0x000631BC
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00064FC4 File Offset: 0x000631C4
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x00064FCC File Offset: 0x000631CC
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return null;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00064FD0 File Offset: 0x000631D0
		protected override DynamicMetaObject GetMetaObject(Expression parameter)
		{
			return new DynamicProxyMetaObject<JObject>(parameter, this, new JObject.JObjectDynamicProxy());
		}

		// Token: 0x04000691 RID: 1681
		private readonly JPropertyKeyedCollection _properties = new JPropertyKeyedCollection();

		// Token: 0x02000296 RID: 662
		private class JObjectDynamicProxy : DynamicProxy<JObject>
		{
			// Token: 0x06001795 RID: 6037 RVA: 0x000856BC File Offset: 0x000838BC
			public override bool TryGetMember(JObject instance, GetMemberBinder binder, out object result)
			{
				result = instance[binder.Name];
				return true;
			}

			// Token: 0x06001796 RID: 6038 RVA: 0x000856D0 File Offset: 0x000838D0
			public override bool TrySetMember(JObject instance, SetMemberBinder binder, object value)
			{
				JToken jtoken = value as JToken;
				if (jtoken == null)
				{
					jtoken = new JValue(value);
				}
				instance[binder.Name] = jtoken;
				return true;
			}

			// Token: 0x06001797 RID: 6039 RVA: 0x00085704 File Offset: 0x00083904
			public override IEnumerable<string> GetDynamicMemberNames(JObject instance)
			{
				return from p in instance.Properties()
					select p.Name;
			}
		}
	}
}
