using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000142 RID: 322
	public class JArray : JContainer, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
	{
		// Token: 0x060010AB RID: 4267 RVA: 0x000628B8 File Offset: 0x00060AB8
		public override async Task WriteToAsync(JsonWriter writer, CancellationToken cancellationToken, params JsonConverter[] converters)
		{
			await writer.WriteStartArrayAsync(cancellationToken).ConfigureAwait(false);
			for (int i = 0; i < this._values.Count; i++)
			{
				await this._values[i].WriteToAsync(writer, cancellationToken, converters).ConfigureAwait(false);
			}
			await writer.WriteEndArrayAsync(cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x0006291C File Offset: 0x00060B1C
		public new static Task<JArray> LoadAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return JArray.LoadAsync(reader, null, cancellationToken);
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x00062928 File Offset: 0x00060B28
		public new static async Task<JArray> LoadAsync(JsonReader reader, JsonLoadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (reader.TokenType == JsonToken.None && !(await reader.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader.");
			}
			await reader.MoveToContentAsync(cancellationToken).ConfigureAwait(false);
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JArray a = new JArray();
			a.SetLineInfo(reader as IJsonLineInfo, settings);
			await a.ReadTokenFromAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			return a;
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060010AE RID: 4270 RVA: 0x00062984 File Offset: 0x00060B84
		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._values;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060010AF RID: 4271 RVA: 0x0006298C File Offset: 0x00060B8C
		public override JTokenType Type
		{
			get
			{
				return JTokenType.Array;
			}
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00062990 File Offset: 0x00060B90
		public JArray()
		{
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x000629A4 File Offset: 0x00060BA4
		public JArray(JArray other)
			: base(other)
		{
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x000629B8 File Offset: 0x00060BB8
		public JArray(params object[] content)
			: this(content)
		{
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x000629C4 File Offset: 0x00060BC4
		public JArray(object content)
		{
			this.Add(content);
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x000629E0 File Offset: 0x00060BE0
		internal override bool DeepEquals(JToken node)
		{
			JArray jarray = node as JArray;
			return jarray != null && base.ContentsEqual(jarray);
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00062A08 File Offset: 0x00060C08
		internal override JToken CloneToken()
		{
			return new JArray(this);
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00062A10 File Offset: 0x00060C10
		public new static JArray Load(JsonReader reader)
		{
			return JArray.Load(reader, null);
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00062A1C File Offset: 0x00060C1C
		public new static JArray Load(JsonReader reader, JsonLoadSettings settings)
		{
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader.");
			}
			reader.MoveToContent();
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JArray jarray = new JArray();
			jarray.SetLineInfo(reader as IJsonLineInfo, settings);
			jarray.ReadTokenFrom(reader, settings);
			return jarray;
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00062AA0 File Offset: 0x00060CA0
		public new static JArray Parse(string json)
		{
			return JArray.Parse(json, null);
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00062AAC File Offset: 0x00060CAC
		public new static JArray Parse(string json, JsonLoadSettings settings)
		{
			JArray jarray2;
			using (JsonReader jsonReader = new JsonTextReader(new StringReader(json)))
			{
				JArray jarray = JArray.Load(jsonReader, settings);
				while (jsonReader.Read())
				{
				}
				jarray2 = jarray;
			}
			return jarray2;
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00062AFC File Offset: 0x00060CFC
		public new static JArray FromObject(object o)
		{
			return JArray.FromObject(o, JsonSerializer.CreateDefault());
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x00062B0C File Offset: 0x00060D0C
		public new static JArray FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jtoken = JToken.FromObjectInternal(o, jsonSerializer);
			if (jtoken.Type != JTokenType.Array)
			{
				throw new ArgumentException("Object serialized to {0}. JArray instance expected.".FormatWith(CultureInfo.InvariantCulture, jtoken.Type));
			}
			return (JArray)jtoken;
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00062B58 File Offset: 0x00060D58
		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartArray();
			for (int i = 0; i < this._values.Count; i++)
			{
				this._values[i].WriteTo(writer, converters);
			}
			writer.WriteEndArray();
		}

		// Token: 0x1700030A RID: 778
		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this.GetItem((int)key);
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Set JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this.SetItem((int)key, value);
			}
		}

		// Token: 0x1700030B RID: 779
		public JToken this[int index]
		{
			get
			{
				return this.GetItem(index);
			}
			set
			{
				this.SetItem(index, value);
			}
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00062C3C File Offset: 0x00060E3C
		internal override int IndexOfItem(JToken item)
		{
			return this._values.IndexOfReference(item);
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00062C4C File Offset: 0x00060E4C
		internal override void MergeItem(object content, JsonMergeSettings settings)
		{
			IEnumerable enumerable = ((base.IsMultiContent(content) || content is JArray) ? ((IEnumerable)content) : null);
			if (enumerable == null)
			{
				return;
			}
			JContainer.MergeEnumerableContent(this, enumerable, settings);
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00062C90 File Offset: 0x00060E90
		public int IndexOf(JToken item)
		{
			return this.IndexOfItem(item);
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x00062C9C File Offset: 0x00060E9C
		public void Insert(int index, JToken item)
		{
			this.InsertItem(index, item, false);
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x00062CA8 File Offset: 0x00060EA8
		public void RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x00062CB4 File Offset: 0x00060EB4
		public IEnumerator<JToken> GetEnumerator()
		{
			return this.Children().GetEnumerator();
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x00062CD4 File Offset: 0x00060ED4
		public void Add(JToken item)
		{
			this.Add(item);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x00062CE0 File Offset: 0x00060EE0
		public void Clear()
		{
			this.ClearItems();
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00062CE8 File Offset: 0x00060EE8
		public bool Contains(JToken item)
		{
			return this.ContainsItem(item);
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x00062CF4 File Offset: 0x00060EF4
		public void CopyTo(JToken[] array, int arrayIndex)
		{
			this.CopyItemsTo(array, arrayIndex);
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060010CB RID: 4299 RVA: 0x00062D00 File Offset: 0x00060F00
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00062D04 File Offset: 0x00060F04
		public bool Remove(JToken item)
		{
			return this.RemoveItem(item);
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00062D10 File Offset: 0x00060F10
		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		// Token: 0x04000687 RID: 1671
		private readonly List<JToken> _values = new List<JToken>();
	}
}
