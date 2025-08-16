using System;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x0200014F RID: 335
	public class JTokenReader : JsonReader, IJsonLineInfo
	{
		// Token: 0x1700034B RID: 843
		// (get) Token: 0x0600127C RID: 4732 RVA: 0x00068588 File Offset: 0x00066788
		public JToken CurrentToken
		{
			get
			{
				return this._current;
			}
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x00068590 File Offset: 0x00066790
		public JTokenReader(JToken token)
		{
			ValidationUtils.ArgumentNotNull(token, "token");
			this._root = token;
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x000685AC File Offset: 0x000667AC
		internal JTokenReader(JToken token, string initialPath)
			: this(token)
		{
			this._initialPath = initialPath;
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x000685BC File Offset: 0x000667BC
		public override bool Read()
		{
			if (base.CurrentState == JsonReader.State.Start)
			{
				this._current = this._root;
				this.SetToken(this._current);
				return true;
			}
			if (this._current == null)
			{
				return false;
			}
			JContainer jcontainer = this._current as JContainer;
			if (jcontainer != null && this._parent != jcontainer)
			{
				return this.ReadInto(jcontainer);
			}
			return this.ReadOver(this._current);
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x00068634 File Offset: 0x00066834
		private bool ReadOver(JToken t)
		{
			if (t == this._root)
			{
				return this.ReadToEnd();
			}
			JToken next = t.Next;
			if (next != null && next != t && t != t.Parent.Last)
			{
				this._current = next;
				this.SetToken(this._current);
				return true;
			}
			if (t.Parent == null)
			{
				return this.ReadToEnd();
			}
			return this.SetEnd(t.Parent);
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x000686B0 File Offset: 0x000668B0
		private bool ReadToEnd()
		{
			this._current = null;
			base.SetToken(JsonToken.None);
			return false;
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x000686C4 File Offset: 0x000668C4
		private JsonToken? GetEndToken(JContainer c)
		{
			switch (c.Type)
			{
			case JTokenType.Object:
				return new JsonToken?(JsonToken.EndObject);
			case JTokenType.Array:
				return new JsonToken?(JsonToken.EndArray);
			case JTokenType.Constructor:
				return new JsonToken?(JsonToken.EndConstructor);
			case JTokenType.Property:
				return null;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
			}
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x00068738 File Offset: 0x00066938
		private bool ReadInto(JContainer c)
		{
			JToken first = c.First;
			if (first == null)
			{
				return this.SetEnd(c);
			}
			this.SetToken(first);
			this._current = first;
			this._parent = c;
			return true;
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00068774 File Offset: 0x00066974
		private bool SetEnd(JContainer c)
		{
			JsonToken? endToken = this.GetEndToken(c);
			if (endToken != null)
			{
				base.SetToken(endToken.GetValueOrDefault());
				this._current = c;
				this._parent = c;
				return true;
			}
			return this.ReadOver(c);
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x000687C0 File Offset: 0x000669C0
		private void SetToken(JToken token)
		{
			switch (token.Type)
			{
			case JTokenType.Object:
				base.SetToken(JsonToken.StartObject);
				return;
			case JTokenType.Array:
				base.SetToken(JsonToken.StartArray);
				return;
			case JTokenType.Constructor:
				base.SetToken(JsonToken.StartConstructor, ((JConstructor)token).Name);
				return;
			case JTokenType.Property:
				base.SetToken(JsonToken.PropertyName, ((JProperty)token).Name);
				return;
			case JTokenType.Comment:
				base.SetToken(JsonToken.Comment, ((JValue)token).Value);
				return;
			case JTokenType.Integer:
				base.SetToken(JsonToken.Integer, ((JValue)token).Value);
				return;
			case JTokenType.Float:
				base.SetToken(JsonToken.Float, ((JValue)token).Value);
				return;
			case JTokenType.String:
				base.SetToken(JsonToken.String, ((JValue)token).Value);
				return;
			case JTokenType.Boolean:
				base.SetToken(JsonToken.Boolean, ((JValue)token).Value);
				return;
			case JTokenType.Null:
				base.SetToken(JsonToken.Null, ((JValue)token).Value);
				return;
			case JTokenType.Undefined:
				base.SetToken(JsonToken.Undefined, ((JValue)token).Value);
				return;
			case JTokenType.Date:
				base.SetToken(JsonToken.Date, ((JValue)token).Value);
				return;
			case JTokenType.Raw:
				base.SetToken(JsonToken.Raw, ((JValue)token).Value);
				return;
			case JTokenType.Bytes:
				base.SetToken(JsonToken.Bytes, ((JValue)token).Value);
				return;
			case JTokenType.Guid:
				base.SetToken(JsonToken.String, this.SafeToString(((JValue)token).Value));
				return;
			case JTokenType.Uri:
			{
				object value = ((JValue)token).Value;
				Uri uri = value as Uri;
				base.SetToken(JsonToken.String, (uri != null) ? uri.OriginalString : this.SafeToString(value));
				return;
			}
			case JTokenType.TimeSpan:
				base.SetToken(JsonToken.String, this.SafeToString(((JValue)token).Value));
				return;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
			}
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x000689AC File Offset: 0x00066BAC
		private string SafeToString(object value)
		{
			if (value == null)
			{
				return null;
			}
			return value.ToString();
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x000689BC File Offset: 0x00066BBC
		bool IJsonLineInfo.HasLineInfo()
		{
			if (base.CurrentState == JsonReader.State.Start)
			{
				return false;
			}
			IJsonLineInfo current = this._current;
			return current != null && current.HasLineInfo();
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06001288 RID: 4744 RVA: 0x000689F0 File Offset: 0x00066BF0
		int IJsonLineInfo.LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start)
				{
					return 0;
				}
				IJsonLineInfo current = this._current;
				if (current != null)
				{
					return current.LineNumber;
				}
				return 0;
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06001289 RID: 4745 RVA: 0x00068A24 File Offset: 0x00066C24
		int IJsonLineInfo.LinePosition
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start)
				{
					return 0;
				}
				IJsonLineInfo current = this._current;
				if (current != null)
				{
					return current.LinePosition;
				}
				return 0;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x0600128A RID: 4746 RVA: 0x00068A58 File Offset: 0x00066C58
		public override string Path
		{
			get
			{
				string text = base.Path;
				if (this._initialPath == null)
				{
					this._initialPath = this._root.Path;
				}
				if (!string.IsNullOrEmpty(this._initialPath))
				{
					if (string.IsNullOrEmpty(text))
					{
						return this._initialPath;
					}
					if (text.StartsWith('['))
					{
						text = this._initialPath + text;
					}
					else
					{
						text = this._initialPath + "." + text;
					}
				}
				return text;
			}
		}

		// Token: 0x040006AB RID: 1707
		private readonly JToken _root;

		// Token: 0x040006AC RID: 1708
		private string _initialPath;

		// Token: 0x040006AD RID: 1709
		private JToken _parent;

		// Token: 0x040006AE RID: 1710
		private JToken _current;
	}
}
