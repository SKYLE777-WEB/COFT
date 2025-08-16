using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000147 RID: 327
	public class JProperty : JContainer
	{
		// Token: 0x06001191 RID: 4497 RVA: 0x00064FE0 File Offset: 0x000631E0
		public override Task WriteToAsync(JsonWriter writer, CancellationToken cancellationToken, params JsonConverter[] converters)
		{
			Task task = writer.WritePropertyNameAsync(this._name, cancellationToken);
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return this.WriteValueAsync(writer, cancellationToken, converters);
			}
			return this.WriteToAsync(task, writer, cancellationToken, converters);
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00065020 File Offset: 0x00063220
		private async Task WriteToAsync(Task task, JsonWriter writer, CancellationToken cancellationToken, params JsonConverter[] converters)
		{
			await task.ConfigureAwait(false);
			await this.WriteValueAsync(writer, cancellationToken, converters).ConfigureAwait(false);
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x0006508C File Offset: 0x0006328C
		private Task WriteValueAsync(JsonWriter writer, CancellationToken cancellationToken, JsonConverter[] converters)
		{
			JToken value = this.Value;
			if (value == null)
			{
				return writer.WriteNullAsync(cancellationToken);
			}
			return value.WriteToAsync(writer, cancellationToken, converters);
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x000650BC File Offset: 0x000632BC
		public new static Task<JProperty> LoadAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return JProperty.LoadAsync(reader, null, cancellationToken);
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x000650C8 File Offset: 0x000632C8
		public new static async Task<JProperty> LoadAsync(JsonReader reader, JsonLoadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (reader.TokenType == JsonToken.None && !(await reader.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader.");
			}
			await reader.MoveToContentAsync(cancellationToken).ConfigureAwait(false);
			if (reader.TokenType != JsonToken.PropertyName)
			{
				throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader. Current JsonReader item is not a property: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JProperty p = new JProperty((string)reader.Value);
			p.SetLineInfo(reader as IJsonLineInfo, settings);
			await p.ReadTokenFromAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			return p;
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06001196 RID: 4502 RVA: 0x00065124 File Offset: 0x00063324
		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._content;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06001197 RID: 4503 RVA: 0x0006512C File Offset: 0x0006332C
		public string Name
		{
			[DebuggerStepThrough]
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001198 RID: 4504 RVA: 0x00065134 File Offset: 0x00063334
		// (set) Token: 0x06001199 RID: 4505 RVA: 0x00065144 File Offset: 0x00063344
		public new JToken Value
		{
			[DebuggerStepThrough]
			get
			{
				return this._content._token;
			}
			set
			{
				base.CheckReentrancy();
				JToken jtoken = value ?? JValue.CreateNull();
				if (this._content._token == null)
				{
					this.InsertItem(0, jtoken, false);
					return;
				}
				this.SetItem(0, jtoken);
			}
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0006518C File Offset: 0x0006338C
		public JProperty(JProperty other)
			: base(other)
		{
			this._name = other.Name;
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x000651AC File Offset: 0x000633AC
		internal override JToken GetItem(int index)
		{
			if (index != 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.Value;
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x000651C0 File Offset: 0x000633C0
		internal override void SetItem(int index, JToken item)
		{
			if (index != 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (JContainer.IsTokenUnchanged(this.Value, item))
			{
				return;
			}
			JObject jobject = (JObject)base.Parent;
			if (jobject != null)
			{
				jobject.InternalPropertyChanging(this);
			}
			base.SetItem(0, item);
			JObject jobject2 = (JObject)base.Parent;
			if (jobject2 == null)
			{
				return;
			}
			jobject2.InternalPropertyChanged(this);
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00065230 File Offset: 0x00063430
		internal override bool RemoveItem(JToken item)
		{
			throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00065250 File Offset: 0x00063450
		internal override void RemoveItemAt(int index)
		{
			throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00065270 File Offset: 0x00063470
		internal override int IndexOfItem(JToken item)
		{
			return this._content.IndexOf(item);
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00065280 File Offset: 0x00063480
		internal override void InsertItem(int index, JToken item, bool skipParentCheck)
		{
			if (item != null && item.Type == JTokenType.Comment)
			{
				return;
			}
			if (this.Value != null)
			{
				throw new JsonException("{0} cannot have multiple values.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
			}
			base.InsertItem(0, item, false);
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x000652D8 File Offset: 0x000634D8
		internal override bool ContainsItem(JToken item)
		{
			return this.Value == item;
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x000652E4 File Offset: 0x000634E4
		internal override void MergeItem(object content, JsonMergeSettings settings)
		{
			JProperty jproperty = content as JProperty;
			JToken jtoken = ((jproperty != null) ? jproperty.Value : null);
			if (jtoken != null && jtoken.Type != JTokenType.Null)
			{
				this.Value = jtoken;
			}
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x00065328 File Offset: 0x00063528
		internal override void ClearItems()
		{
			throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x00065348 File Offset: 0x00063548
		internal override bool DeepEquals(JToken node)
		{
			JProperty jproperty = node as JProperty;
			return jproperty != null && this._name == jproperty.Name && base.ContentsEqual(jproperty);
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x00065388 File Offset: 0x00063588
		internal override JToken CloneToken()
		{
			return new JProperty(this);
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x060011A6 RID: 4518 RVA: 0x00065390 File Offset: 0x00063590
		public override JTokenType Type
		{
			[DebuggerStepThrough]
			get
			{
				return JTokenType.Property;
			}
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x00065394 File Offset: 0x00063594
		internal JProperty(string name)
		{
			ValidationUtils.ArgumentNotNull(name, "name");
			this._name = name;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x000653BC File Offset: 0x000635BC
		public JProperty(string name, params object[] content)
			: this(name, content)
		{
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x000653C8 File Offset: 0x000635C8
		public JProperty(string name, object content)
		{
			ValidationUtils.ArgumentNotNull(name, "name");
			this._name = name;
			this.Value = (base.IsMultiContent(content) ? new JArray(content) : JContainer.CreateFromContent(content));
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x00065420 File Offset: 0x00063620
		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WritePropertyName(this._name);
			JToken value = this.Value;
			if (value != null)
			{
				value.WriteTo(writer, converters);
				return;
			}
			writer.WriteNull();
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x0006545C File Offset: 0x0006365C
		internal override int GetDeepHashCode()
		{
			return this._name.GetHashCode() ^ ((this.Value != null) ? this.Value.GetDeepHashCode() : 0);
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00065498 File Offset: 0x00063698
		public new static JProperty Load(JsonReader reader)
		{
			return JProperty.Load(reader, null);
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x000654A4 File Offset: 0x000636A4
		public new static JProperty Load(JsonReader reader, JsonLoadSettings settings)
		{
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader.");
			}
			reader.MoveToContent();
			if (reader.TokenType != JsonToken.PropertyName)
			{
				throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader. Current JsonReader item is not a property: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JProperty jproperty = new JProperty((string)reader.Value);
			jproperty.SetLineInfo(reader as IJsonLineInfo, settings);
			jproperty.ReadTokenFrom(reader, settings);
			return jproperty;
		}

		// Token: 0x04000694 RID: 1684
		private readonly JProperty.JPropertyList _content = new JProperty.JPropertyList();

		// Token: 0x04000695 RID: 1685
		private readonly string _name;

		// Token: 0x0200029B RID: 667
		private class JPropertyList : IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
		{
			// Token: 0x060017A7 RID: 6055 RVA: 0x00085DA4 File Offset: 0x00083FA4
			public IEnumerator<JToken> GetEnumerator()
			{
				if (this._token != null)
				{
					yield return this._token;
				}
				yield break;
			}

			// Token: 0x060017A8 RID: 6056 RVA: 0x00085DB4 File Offset: 0x00083FB4
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			// Token: 0x060017A9 RID: 6057 RVA: 0x00085DBC File Offset: 0x00083FBC
			public void Add(JToken item)
			{
				this._token = item;
			}

			// Token: 0x060017AA RID: 6058 RVA: 0x00085DC8 File Offset: 0x00083FC8
			public void Clear()
			{
				this._token = null;
			}

			// Token: 0x060017AB RID: 6059 RVA: 0x00085DD4 File Offset: 0x00083FD4
			public bool Contains(JToken item)
			{
				return this._token == item;
			}

			// Token: 0x060017AC RID: 6060 RVA: 0x00085DE0 File Offset: 0x00083FE0
			public void CopyTo(JToken[] array, int arrayIndex)
			{
				if (this._token != null)
				{
					array[arrayIndex] = this._token;
				}
			}

			// Token: 0x060017AD RID: 6061 RVA: 0x00085DFC File Offset: 0x00083FFC
			public bool Remove(JToken item)
			{
				if (this._token == item)
				{
					this._token = null;
					return true;
				}
				return false;
			}

			// Token: 0x17000421 RID: 1057
			// (get) Token: 0x060017AE RID: 6062 RVA: 0x00085E14 File Offset: 0x00084014
			public int Count
			{
				get
				{
					if (this._token == null)
					{
						return 0;
					}
					return 1;
				}
			}

			// Token: 0x17000422 RID: 1058
			// (get) Token: 0x060017AF RID: 6063 RVA: 0x00085E24 File Offset: 0x00084024
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			// Token: 0x060017B0 RID: 6064 RVA: 0x00085E28 File Offset: 0x00084028
			public int IndexOf(JToken item)
			{
				if (this._token != item)
				{
					return -1;
				}
				return 0;
			}

			// Token: 0x060017B1 RID: 6065 RVA: 0x00085E3C File Offset: 0x0008403C
			public void Insert(int index, JToken item)
			{
				if (index == 0)
				{
					this._token = item;
				}
			}

			// Token: 0x060017B2 RID: 6066 RVA: 0x00085E4C File Offset: 0x0008404C
			public void RemoveAt(int index)
			{
				if (index == 0)
				{
					this._token = null;
				}
			}

			// Token: 0x17000423 RID: 1059
			public JToken this[int index]
			{
				get
				{
					if (index != 0)
					{
						return null;
					}
					return this._token;
				}
				set
				{
					if (index == 0)
					{
						this._token = value;
					}
				}
			}

			// Token: 0x04000BD3 RID: 3027
			internal JToken _token;
		}
	}
}
