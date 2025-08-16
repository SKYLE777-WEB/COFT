using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000143 RID: 323
	public class JConstructor : JContainer
	{
		// Token: 0x060010CE RID: 4302 RVA: 0x00062D18 File Offset: 0x00060F18
		public override async Task WriteToAsync(JsonWriter writer, CancellationToken cancellationToken, params JsonConverter[] converters)
		{
			await writer.WriteStartConstructorAsync(this._name, cancellationToken).ConfigureAwait(false);
			for (int i = 0; i < this._values.Count; i++)
			{
				await this._values[i].WriteToAsync(writer, cancellationToken, converters).ConfigureAwait(false);
			}
			await writer.WriteEndConstructorAsync(cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x00062D7C File Offset: 0x00060F7C
		public new static Task<JConstructor> LoadAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			return JConstructor.LoadAsync(reader, null, cancellationToken);
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x00062D88 File Offset: 0x00060F88
		public new static async Task<JConstructor> LoadAsync(JsonReader reader, JsonLoadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (reader.TokenType == JsonToken.None && !(await reader.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader.");
			}
			await reader.MoveToContentAsync(cancellationToken).ConfigureAwait(false);
			if (reader.TokenType != JsonToken.StartConstructor)
			{
				throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader. Current JsonReader item is not a constructor: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JConstructor c = new JConstructor((string)reader.Value);
			c.SetLineInfo(reader as IJsonLineInfo, settings);
			await c.ReadTokenFromAsync(reader, settings, cancellationToken).ConfigureAwait(false);
			return c;
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060010D1 RID: 4305 RVA: 0x00062DE4 File Offset: 0x00060FE4
		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._values;
			}
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x00062DEC File Offset: 0x00060FEC
		internal override int IndexOfItem(JToken item)
		{
			return this._values.IndexOfReference(item);
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x00062DFC File Offset: 0x00060FFC
		internal override void MergeItem(object content, JsonMergeSettings settings)
		{
			JConstructor jconstructor = content as JConstructor;
			if (jconstructor == null)
			{
				return;
			}
			if (jconstructor.Name != null)
			{
				this.Name = jconstructor.Name;
			}
			JContainer.MergeEnumerableContent(this, jconstructor, settings);
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00062E3C File Offset: 0x0006103C
		// (set) Token: 0x060010D5 RID: 4309 RVA: 0x00062E44 File Offset: 0x00061044
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060010D6 RID: 4310 RVA: 0x00062E50 File Offset: 0x00061050
		public override JTokenType Type
		{
			get
			{
				return JTokenType.Constructor;
			}
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x00062E54 File Offset: 0x00061054
		public JConstructor()
		{
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x00062E68 File Offset: 0x00061068
		public JConstructor(JConstructor other)
			: base(other)
		{
			this._name = other.Name;
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x00062E88 File Offset: 0x00061088
		public JConstructor(string name, params object[] content)
			: this(name, content)
		{
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00062E94 File Offset: 0x00061094
		public JConstructor(string name, object content)
			: this(name)
		{
			this.Add(content);
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x00062EA4 File Offset: 0x000610A4
		public JConstructor(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Constructor name cannot be empty.", "name");
			}
			this._name = name;
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x00062EFC File Offset: 0x000610FC
		internal override bool DeepEquals(JToken node)
		{
			JConstructor jconstructor = node as JConstructor;
			return jconstructor != null && this._name == jconstructor.Name && base.ContentsEqual(jconstructor);
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x00062F3C File Offset: 0x0006113C
		internal override JToken CloneToken()
		{
			return new JConstructor(this);
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x00062F44 File Offset: 0x00061144
		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartConstructor(this._name);
			int count = this._values.Count;
			for (int i = 0; i < count; i++)
			{
				this._values[i].WriteTo(writer, converters);
			}
			writer.WriteEndConstructor();
		}

		// Token: 0x17000310 RID: 784
		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Accessed JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this.GetItem((int)key);
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Set JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this.SetItem((int)key, value);
			}
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00063018 File Offset: 0x00061218
		internal override int GetDeepHashCode()
		{
			return this._name.GetHashCode() ^ base.ContentsHashCode();
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0006302C File Offset: 0x0006122C
		public new static JConstructor Load(JsonReader reader)
		{
			return JConstructor.Load(reader, null);
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x00063038 File Offset: 0x00061238
		public new static JConstructor Load(JsonReader reader, JsonLoadSettings settings)
		{
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader.");
			}
			reader.MoveToContent();
			if (reader.TokenType != JsonToken.StartConstructor)
			{
				throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader. Current JsonReader item is not a constructor: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JConstructor jconstructor = new JConstructor((string)reader.Value);
			jconstructor.SetLineInfo(reader as IJsonLineInfo, settings);
			jconstructor.ReadTokenFrom(reader, settings);
			return jconstructor;
		}

		// Token: 0x04000688 RID: 1672
		private string _name;

		// Token: 0x04000689 RID: 1673
		private readonly List<JToken> _values = new List<JToken>();
	}
}
