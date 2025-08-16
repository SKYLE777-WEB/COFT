using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000144 RID: 324
	public abstract class JContainer : JToken, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable, ITypedList, IBindingList, IList, ICollection, INotifyCollectionChanged
	{
		// Token: 0x060010E4 RID: 4324 RVA: 0x000630C4 File Offset: 0x000612C4
		internal async Task ReadTokenFromAsync(JsonReader reader, JsonLoadSettings options, CancellationToken cancellationToken = default(CancellationToken))
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			int startDepth = reader.Depth;
			if (!(await reader.ReadAsync(cancellationToken).ConfigureAwait(false)))
			{
				throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
			}
			await this.ReadContentFromAsync(reader, options, cancellationToken).ConfigureAwait(false);
			if (reader.Depth > startDepth)
			{
				throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
			}
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00063128 File Offset: 0x00061328
		private async Task ReadContentFromAsync(JsonReader reader, JsonLoadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			IJsonLineInfo lineInfo = reader as IJsonLineInfo;
			JContainer parent = this;
			do
			{
				JProperty jproperty = parent as JProperty;
				if (((jproperty != null) ? jproperty.Value : null) != null)
				{
					if (parent == this)
					{
						break;
					}
					parent = parent.Parent;
				}
				switch (reader.TokenType)
				{
				case JsonToken.None:
					goto IL_0369;
				case JsonToken.StartObject:
				{
					JObject jobject = new JObject();
					jobject.SetLineInfo(lineInfo, settings);
					parent.Add(jobject);
					parent = jobject;
					goto IL_0369;
				}
				case JsonToken.StartArray:
				{
					JArray jarray = new JArray();
					jarray.SetLineInfo(lineInfo, settings);
					parent.Add(jarray);
					parent = jarray;
					goto IL_0369;
				}
				case JsonToken.StartConstructor:
				{
					JConstructor jconstructor = new JConstructor(reader.Value.ToString());
					jconstructor.SetLineInfo(lineInfo, settings);
					parent.Add(jconstructor);
					parent = jconstructor;
					goto IL_0369;
				}
				case JsonToken.PropertyName:
				{
					string text = reader.Value.ToString();
					JProperty jproperty2 = new JProperty(text);
					jproperty2.SetLineInfo(lineInfo, settings);
					JProperty jproperty3 = ((JObject)parent).Property(text);
					if (jproperty3 == null)
					{
						parent.Add(jproperty2);
					}
					else
					{
						jproperty3.Replace(jproperty2);
					}
					parent = jproperty2;
					goto IL_0369;
				}
				case JsonToken.Comment:
					if (settings != null && settings.CommentHandling == CommentHandling.Load)
					{
						JValue jvalue = JValue.CreateComment(reader.Value.ToString());
						jvalue.SetLineInfo(lineInfo, settings);
						parent.Add(jvalue);
						goto IL_0369;
					}
					goto IL_0369;
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					JValue jvalue = new JValue(reader.Value);
					jvalue.SetLineInfo(lineInfo, settings);
					parent.Add(jvalue);
					goto IL_0369;
				}
				case JsonToken.Null:
				{
					JValue jvalue = JValue.CreateNull();
					jvalue.SetLineInfo(lineInfo, settings);
					parent.Add(jvalue);
					goto IL_0369;
				}
				case JsonToken.Undefined:
				{
					JValue jvalue = JValue.CreateUndefined();
					jvalue.SetLineInfo(lineInfo, settings);
					parent.Add(jvalue);
					goto IL_0369;
				}
				case JsonToken.EndObject:
					if (parent == this)
					{
						goto Block_6;
					}
					parent = parent.Parent;
					goto IL_0369;
				case JsonToken.EndArray:
					if (parent == this)
					{
						goto Block_5;
					}
					parent = parent.Parent;
					goto IL_0369;
				case JsonToken.EndConstructor:
					if (parent == this)
					{
						goto Block_7;
					}
					parent = parent.Parent;
					goto IL_0369;
				}
				goto Block_4;
				IL_0369:;
			}
			while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false));
			return;
			Block_4:
			goto IL_0344;
			Block_5:
			Block_6:
			Block_7:
			return;
			IL_0344:
			throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060010E6 RID: 4326 RVA: 0x0006318C File Offset: 0x0006138C
		// (remove) Token: 0x060010E7 RID: 4327 RVA: 0x000631A8 File Offset: 0x000613A8
		public event ListChangedEventHandler ListChanged
		{
			add
			{
				this._listChanged = (ListChangedEventHandler)Delegate.Combine(this._listChanged, value);
			}
			remove
			{
				this._listChanged = (ListChangedEventHandler)Delegate.Remove(this._listChanged, value);
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060010E8 RID: 4328 RVA: 0x000631C4 File Offset: 0x000613C4
		// (remove) Token: 0x060010E9 RID: 4329 RVA: 0x000631E0 File Offset: 0x000613E0
		public event AddingNewEventHandler AddingNew
		{
			add
			{
				this._addingNew = (AddingNewEventHandler)Delegate.Combine(this._addingNew, value);
			}
			remove
			{
				this._addingNew = (AddingNewEventHandler)Delegate.Remove(this._addingNew, value);
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x060010EA RID: 4330 RVA: 0x000631FC File Offset: 0x000613FC
		// (remove) Token: 0x060010EB RID: 4331 RVA: 0x00063218 File Offset: 0x00061418
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				this._collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(this._collectionChanged, value);
			}
			remove
			{
				this._collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(this._collectionChanged, value);
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060010EC RID: 4332
		protected abstract IList<JToken> ChildrenTokens { get; }

		// Token: 0x060010ED RID: 4333 RVA: 0x00063234 File Offset: 0x00061434
		internal JContainer()
		{
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x0006323C File Offset: 0x0006143C
		internal JContainer(JContainer other)
			: this()
		{
			ValidationUtils.ArgumentNotNull(other, "other");
			int num = 0;
			foreach (JToken jtoken in ((IEnumerable<JToken>)other))
			{
				this.AddInternal(num, jtoken, false);
				num++;
			}
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x000632A8 File Offset: 0x000614A8
		internal void CheckReentrancy()
		{
			if (this._busy)
			{
				throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
			}
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x000632D0 File Offset: 0x000614D0
		internal virtual IList<JToken> CreateChildrenCollection()
		{
			return new List<JToken>();
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x000632D8 File Offset: 0x000614D8
		protected virtual void OnAddingNew(AddingNewEventArgs e)
		{
			AddingNewEventHandler addingNew = this._addingNew;
			if (addingNew == null)
			{
				return;
			}
			addingNew(this, e);
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x000632F0 File Offset: 0x000614F0
		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			ListChangedEventHandler listChanged = this._listChanged;
			if (listChanged != null)
			{
				this._busy = true;
				try
				{
					listChanged(this, e);
				}
				finally
				{
					this._busy = false;
				}
			}
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00063338 File Offset: 0x00061538
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			NotifyCollectionChangedEventHandler collectionChanged = this._collectionChanged;
			if (collectionChanged != null)
			{
				this._busy = true;
				try
				{
					collectionChanged(this, e);
				}
				finally
				{
					this._busy = false;
				}
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060010F4 RID: 4340 RVA: 0x00063380 File Offset: 0x00061580
		public override bool HasValues
		{
			get
			{
				return this.ChildrenTokens.Count > 0;
			}
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x00063390 File Offset: 0x00061590
		internal bool ContentsEqual(JContainer container)
		{
			if (container == this)
			{
				return true;
			}
			IList<JToken> childrenTokens = this.ChildrenTokens;
			IList<JToken> childrenTokens2 = container.ChildrenTokens;
			if (childrenTokens.Count != childrenTokens2.Count)
			{
				return false;
			}
			for (int i = 0; i < childrenTokens.Count; i++)
			{
				if (!childrenTokens[i].DeepEquals(childrenTokens2[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060010F6 RID: 4342 RVA: 0x000633FC File Offset: 0x000615FC
		public override JToken First
		{
			get
			{
				IList<JToken> childrenTokens = this.ChildrenTokens;
				if (childrenTokens.Count <= 0)
				{
					return null;
				}
				return childrenTokens[0];
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060010F7 RID: 4343 RVA: 0x0006342C File Offset: 0x0006162C
		public override JToken Last
		{
			get
			{
				IList<JToken> childrenTokens = this.ChildrenTokens;
				int count = childrenTokens.Count;
				if (count <= 0)
				{
					return null;
				}
				return childrenTokens[count - 1];
			}
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00063460 File Offset: 0x00061660
		public override JEnumerable<JToken> Children()
		{
			return new JEnumerable<JToken>(this.ChildrenTokens);
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00063470 File Offset: 0x00061670
		public override IEnumerable<T> Values<T>()
		{
			return this.ChildrenTokens.Convert<JToken, T>();
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00063480 File Offset: 0x00061680
		public IEnumerable<JToken> Descendants()
		{
			return this.GetDescendants(false);
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0006348C File Offset: 0x0006168C
		public IEnumerable<JToken> DescendantsAndSelf()
		{
			return this.GetDescendants(true);
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00063498 File Offset: 0x00061698
		internal IEnumerable<JToken> GetDescendants(bool self)
		{
			if (self)
			{
				yield return this;
			}
			foreach (JToken o in this.ChildrenTokens)
			{
				yield return o;
				JContainer jcontainer = o as JContainer;
				if (jcontainer != null)
				{
					foreach (JToken jtoken in jcontainer.Descendants())
					{
						yield return jtoken;
					}
					IEnumerator<JToken> enumerator2 = null;
				}
				o = null;
			}
			IEnumerator<JToken> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x000634B0 File Offset: 0x000616B0
		internal bool IsMultiContent(object content)
		{
			return content is IEnumerable && !(content is string) && !(content is JToken) && !(content is byte[]);
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x000634E4 File Offset: 0x000616E4
		internal JToken EnsureParentToken(JToken item, bool skipParentCheck)
		{
			if (item == null)
			{
				return JValue.CreateNull();
			}
			if (skipParentCheck)
			{
				return item;
			}
			if (item.Parent != null || item == this || (item.HasValues && base.Root == item))
			{
				item = item.CloneToken();
			}
			return item;
		}

		// Token: 0x060010FF RID: 4351
		internal abstract int IndexOfItem(JToken item);

		// Token: 0x06001100 RID: 4352 RVA: 0x0006353C File Offset: 0x0006173C
		internal virtual void InsertItem(int index, JToken item, bool skipParentCheck)
		{
			IList<JToken> childrenTokens = this.ChildrenTokens;
			if (index > childrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
			}
			this.CheckReentrancy();
			item = this.EnsureParentToken(item, skipParentCheck);
			JToken jtoken = ((index == 0) ? null : childrenTokens[index - 1]);
			JToken jtoken2 = ((index == childrenTokens.Count) ? null : childrenTokens[index]);
			this.ValidateToken(item, null);
			item.Parent = this;
			item.Previous = jtoken;
			if (jtoken != null)
			{
				jtoken.Next = item;
			}
			item.Next = jtoken2;
			if (jtoken2 != null)
			{
				jtoken2.Previous = item;
			}
			childrenTokens.Insert(index, item);
			if (this._listChanged != null)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
			}
			if (this._collectionChanged != null)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
			}
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00063620 File Offset: 0x00061820
		internal virtual void RemoveItemAt(int index)
		{
			IList<JToken> childrenTokens = this.ChildrenTokens;
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
			}
			if (index >= childrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
			}
			this.CheckReentrancy();
			JToken jtoken = childrenTokens[index];
			JToken jtoken2 = ((index == 0) ? null : childrenTokens[index - 1]);
			JToken jtoken3 = ((index == childrenTokens.Count - 1) ? null : childrenTokens[index + 1]);
			if (jtoken2 != null)
			{
				jtoken2.Next = jtoken3;
			}
			if (jtoken3 != null)
			{
				jtoken3.Previous = jtoken2;
			}
			jtoken.Parent = null;
			jtoken.Previous = null;
			jtoken.Next = null;
			childrenTokens.RemoveAt(index);
			if (this._listChanged != null)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
			}
			if (this._collectionChanged != null)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, jtoken, index));
			}
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00063714 File Offset: 0x00061914
		internal virtual bool RemoveItem(JToken item)
		{
			int num = this.IndexOfItem(item);
			if (num >= 0)
			{
				this.RemoveItemAt(num);
				return true;
			}
			return false;
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00063740 File Offset: 0x00061940
		internal virtual JToken GetItem(int index)
		{
			return this.ChildrenTokens[index];
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00063750 File Offset: 0x00061950
		internal virtual void SetItem(int index, JToken item)
		{
			IList<JToken> childrenTokens = this.ChildrenTokens;
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
			}
			if (index >= childrenTokens.Count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
			}
			JToken jtoken = childrenTokens[index];
			if (JContainer.IsTokenUnchanged(jtoken, item))
			{
				return;
			}
			this.CheckReentrancy();
			item = this.EnsureParentToken(item, false);
			this.ValidateToken(item, jtoken);
			JToken jtoken2 = ((index == 0) ? null : childrenTokens[index - 1]);
			JToken jtoken3 = ((index == childrenTokens.Count - 1) ? null : childrenTokens[index + 1]);
			item.Parent = this;
			item.Previous = jtoken2;
			if (jtoken2 != null)
			{
				jtoken2.Next = item;
			}
			item.Next = jtoken3;
			if (jtoken3 != null)
			{
				jtoken3.Previous = item;
			}
			childrenTokens[index] = item;
			jtoken.Parent = null;
			jtoken.Previous = null;
			jtoken.Next = null;
			if (this._listChanged != null)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
			if (this._collectionChanged != null)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, jtoken, index));
			}
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0006387C File Offset: 0x00061A7C
		internal virtual void ClearItems()
		{
			this.CheckReentrancy();
			IList<JToken> childrenTokens = this.ChildrenTokens;
			foreach (JToken jtoken in childrenTokens)
			{
				jtoken.Parent = null;
				jtoken.Previous = null;
				jtoken.Next = null;
			}
			childrenTokens.Clear();
			if (this._listChanged != null)
			{
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
			if (this._collectionChanged != null)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0006391C File Offset: 0x00061B1C
		internal virtual void ReplaceItem(JToken existing, JToken replacement)
		{
			if (existing == null || existing.Parent != this)
			{
				return;
			}
			int num = this.IndexOfItem(existing);
			this.SetItem(num, replacement);
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x00063950 File Offset: 0x00061B50
		internal virtual bool ContainsItem(JToken item)
		{
			return this.IndexOfItem(item) != -1;
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00063960 File Offset: 0x00061B60
		internal virtual void CopyItemsTo(Array array, int arrayIndex)
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
			if (this.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			foreach (JToken jtoken in this.ChildrenTokens)
			{
				array.SetValue(jtoken, arrayIndex + num);
				num++;
			}
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00063A24 File Offset: 0x00061C24
		internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
		{
			JValue jvalue = currentValue as JValue;
			return jvalue != null && ((jvalue.Type == JTokenType.Null && newValue == null) || jvalue.Equals(newValue));
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x00063A60 File Offset: 0x00061C60
		internal virtual void ValidateToken(JToken o, JToken existing)
		{
			ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type == JTokenType.Property)
			{
				throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), base.GetType()));
			}
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00063AAC File Offset: 0x00061CAC
		public virtual void Add(object content)
		{
			this.AddInternal(this.ChildrenTokens.Count, content, false);
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x00063AD0 File Offset: 0x00061CD0
		internal void AddAndSkipParentCheck(JToken token)
		{
			this.AddInternal(this.ChildrenTokens.Count, token, true);
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x00063AF4 File Offset: 0x00061CF4
		public void AddFirst(object content)
		{
			this.AddInternal(0, content, false);
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x00063B00 File Offset: 0x00061D00
		internal void AddInternal(int index, object content, bool skipParentCheck)
		{
			if (this.IsMultiContent(content))
			{
				IEnumerable enumerable = (IEnumerable)content;
				int num = index;
				using (IEnumerator enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						this.AddInternal(num, obj, skipParentCheck);
						num++;
					}
					return;
				}
			}
			JToken jtoken = JContainer.CreateFromContent(content);
			this.InsertItem(index, jtoken, skipParentCheck);
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x00063B84 File Offset: 0x00061D84
		internal static JToken CreateFromContent(object content)
		{
			JToken jtoken = content as JToken;
			if (jtoken != null)
			{
				return jtoken;
			}
			return new JValue(content);
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x00063BAC File Offset: 0x00061DAC
		public JsonWriter CreateWriter()
		{
			return new JTokenWriter(this);
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x00063BB4 File Offset: 0x00061DB4
		public void ReplaceAll(object content)
		{
			this.ClearItems();
			this.Add(content);
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x00063BC4 File Offset: 0x00061DC4
		public void RemoveAll()
		{
			this.ClearItems();
		}

		// Token: 0x06001113 RID: 4371
		internal abstract void MergeItem(object content, JsonMergeSettings settings);

		// Token: 0x06001114 RID: 4372 RVA: 0x00063BCC File Offset: 0x00061DCC
		public void Merge(object content)
		{
			this.MergeItem(content, new JsonMergeSettings());
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x00063BDC File Offset: 0x00061DDC
		public void Merge(object content, JsonMergeSettings settings)
		{
			this.MergeItem(content, settings);
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x00063BE8 File Offset: 0x00061DE8
		internal void ReadTokenFrom(JsonReader reader, JsonLoadSettings options)
		{
			int depth = reader.Depth;
			if (!reader.Read())
			{
				throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
			}
			this.ReadContentFrom(reader, options);
			if (reader.Depth > depth)
			{
				throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, base.GetType().Name));
			}
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00063C64 File Offset: 0x00061E64
		internal void ReadContentFrom(JsonReader r, JsonLoadSettings settings)
		{
			ValidationUtils.ArgumentNotNull(r, "r");
			IJsonLineInfo jsonLineInfo = r as IJsonLineInfo;
			JContainer jcontainer = this;
			for (;;)
			{
				JProperty jproperty = jcontainer as JProperty;
				if (((jproperty != null) ? jproperty.Value : null) != null)
				{
					if (jcontainer == this)
					{
						break;
					}
					jcontainer = jcontainer.Parent;
				}
				switch (r.TokenType)
				{
				case JsonToken.None:
					goto IL_0247;
				case JsonToken.StartObject:
				{
					JObject jobject = new JObject();
					jobject.SetLineInfo(jsonLineInfo, settings);
					jcontainer.Add(jobject);
					jcontainer = jobject;
					goto IL_0247;
				}
				case JsonToken.StartArray:
				{
					JArray jarray = new JArray();
					jarray.SetLineInfo(jsonLineInfo, settings);
					jcontainer.Add(jarray);
					jcontainer = jarray;
					goto IL_0247;
				}
				case JsonToken.StartConstructor:
				{
					JConstructor jconstructor = new JConstructor(r.Value.ToString());
					jconstructor.SetLineInfo(jsonLineInfo, settings);
					jcontainer.Add(jconstructor);
					jcontainer = jconstructor;
					goto IL_0247;
				}
				case JsonToken.PropertyName:
				{
					string text = r.Value.ToString();
					JProperty jproperty2 = new JProperty(text);
					jproperty2.SetLineInfo(jsonLineInfo, settings);
					JProperty jproperty3 = ((JObject)jcontainer).Property(text);
					if (jproperty3 == null)
					{
						jcontainer.Add(jproperty2);
					}
					else
					{
						jproperty3.Replace(jproperty2);
					}
					jcontainer = jproperty2;
					goto IL_0247;
				}
				case JsonToken.Comment:
					if (settings != null && settings.CommentHandling == CommentHandling.Load)
					{
						JValue jvalue = JValue.CreateComment(r.Value.ToString());
						jvalue.SetLineInfo(jsonLineInfo, settings);
						jcontainer.Add(jvalue);
						goto IL_0247;
					}
					goto IL_0247;
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					JValue jvalue = new JValue(r.Value);
					jvalue.SetLineInfo(jsonLineInfo, settings);
					jcontainer.Add(jvalue);
					goto IL_0247;
				}
				case JsonToken.Null:
				{
					JValue jvalue = JValue.CreateNull();
					jvalue.SetLineInfo(jsonLineInfo, settings);
					jcontainer.Add(jvalue);
					goto IL_0247;
				}
				case JsonToken.Undefined:
				{
					JValue jvalue = JValue.CreateUndefined();
					jvalue.SetLineInfo(jsonLineInfo, settings);
					jcontainer.Add(jvalue);
					goto IL_0247;
				}
				case JsonToken.EndObject:
					if (jcontainer == this)
					{
						return;
					}
					jcontainer = jcontainer.Parent;
					goto IL_0247;
				case JsonToken.EndArray:
					if (jcontainer == this)
					{
						return;
					}
					jcontainer = jcontainer.Parent;
					goto IL_0247;
				case JsonToken.EndConstructor:
					if (jcontainer == this)
					{
						return;
					}
					jcontainer = jcontainer.Parent;
					goto IL_0247;
				}
				goto Block_4;
				IL_0247:
				if (!r.Read())
				{
					return;
				}
			}
			return;
			Block_4:
			throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, r.TokenType));
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00063EC8 File Offset: 0x000620C8
		internal int ContentsHashCode()
		{
			int num = 0;
			foreach (JToken jtoken in this.ChildrenTokens)
			{
				num ^= jtoken.GetDeepHashCode();
			}
			return num;
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00063F24 File Offset: 0x00062124
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			return string.Empty;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00063F2C File Offset: 0x0006212C
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			ICustomTypeDescriptor customTypeDescriptor = this.First as ICustomTypeDescriptor;
			if (customTypeDescriptor == null)
			{
				return null;
			}
			return customTypeDescriptor.GetProperties();
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00063F48 File Offset: 0x00062148
		int IList<JToken>.IndexOf(JToken item)
		{
			return this.IndexOfItem(item);
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x00063F54 File Offset: 0x00062154
		void IList<JToken>.Insert(int index, JToken item)
		{
			this.InsertItem(index, item, false);
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x00063F60 File Offset: 0x00062160
		void IList<JToken>.RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		// Token: 0x17000315 RID: 789
		JToken IList<JToken>.this[int index]
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

		// Token: 0x06001120 RID: 4384 RVA: 0x00063F84 File Offset: 0x00062184
		void ICollection<JToken>.Add(JToken item)
		{
			this.Add(item);
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x00063F90 File Offset: 0x00062190
		void ICollection<JToken>.Clear()
		{
			this.ClearItems();
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00063F98 File Offset: 0x00062198
		bool ICollection<JToken>.Contains(JToken item)
		{
			return this.ContainsItem(item);
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00063FA4 File Offset: 0x000621A4
		void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
		{
			this.CopyItemsTo(array, arrayIndex);
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06001124 RID: 4388 RVA: 0x00063FB0 File Offset: 0x000621B0
		bool ICollection<JToken>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00063FB4 File Offset: 0x000621B4
		bool ICollection<JToken>.Remove(JToken item)
		{
			return this.RemoveItem(item);
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x00063FC0 File Offset: 0x000621C0
		private JToken EnsureValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			JToken jtoken = value as JToken;
			if (jtoken != null)
			{
				return jtoken;
			}
			throw new ArgumentException("Argument is not a JToken.");
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x00063FF4 File Offset: 0x000621F4
		int IList.Add(object value)
		{
			this.Add(this.EnsureValue(value));
			return this.Count - 1;
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x0006400C File Offset: 0x0006220C
		void IList.Clear()
		{
			this.ClearItems();
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x00064014 File Offset: 0x00062214
		bool IList.Contains(object value)
		{
			return this.ContainsItem(this.EnsureValue(value));
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00064024 File Offset: 0x00062224
		int IList.IndexOf(object value)
		{
			return this.IndexOfItem(this.EnsureValue(value));
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00064034 File Offset: 0x00062234
		void IList.Insert(int index, object value)
		{
			this.InsertItem(index, this.EnsureValue(value), false);
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x0600112C RID: 4396 RVA: 0x00064048 File Offset: 0x00062248
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x0600112D RID: 4397 RVA: 0x0006404C File Offset: 0x0006224C
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x00064050 File Offset: 0x00062250
		void IList.Remove(object value)
		{
			this.RemoveItem(this.EnsureValue(value));
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x00064060 File Offset: 0x00062260
		void IList.RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		// Token: 0x17000319 RID: 793
		object IList.this[int index]
		{
			get
			{
				return this.GetItem(index);
			}
			set
			{
				this.SetItem(index, this.EnsureValue(value));
			}
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00064088 File Offset: 0x00062288
		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyItemsTo(array, index);
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06001133 RID: 4403 RVA: 0x00064094 File Offset: 0x00062294
		public int Count
		{
			get
			{
				return this.ChildrenTokens.Count;
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06001134 RID: 4404 RVA: 0x000640A4 File Offset: 0x000622A4
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06001135 RID: 4405 RVA: 0x000640A8 File Offset: 0x000622A8
		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x000640D0 File Offset: 0x000622D0
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x000640D4 File Offset: 0x000622D4
		object IBindingList.AddNew()
		{
			AddingNewEventArgs addingNewEventArgs = new AddingNewEventArgs();
			this.OnAddingNew(addingNewEventArgs);
			if (addingNewEventArgs.NewObject == null)
			{
				throw new JsonException("Could not determine new value to add to '{0}'.".FormatWith(CultureInfo.InvariantCulture, base.GetType()));
			}
			if (!(addingNewEventArgs.NewObject is JToken))
			{
				throw new JsonException("New item to be added to collection must be compatible with {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JToken)));
			}
			JToken jtoken = (JToken)addingNewEventArgs.NewObject;
			this.Add(jtoken);
			return jtoken;
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06001138 RID: 4408 RVA: 0x0006415C File Offset: 0x0006235C
		bool IBindingList.AllowEdit
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x00064160 File Offset: 0x00062360
		bool IBindingList.AllowNew
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x0600113A RID: 4410 RVA: 0x00064164 File Offset: 0x00062364
		bool IBindingList.AllowRemove
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x00064168 File Offset: 0x00062368
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x00064170 File Offset: 0x00062370
		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x0600113D RID: 4413 RVA: 0x00064178 File Offset: 0x00062378
		bool IBindingList.IsSorted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0006417C File Offset: 0x0006237C
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00064180 File Offset: 0x00062380
		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x00064188 File Offset: 0x00062388
		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				return ListSortDirection.Ascending;
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x0006418C File Offset: 0x0006238C
		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06001142 RID: 4418 RVA: 0x00064190 File Offset: 0x00062390
		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06001143 RID: 4419 RVA: 0x00064194 File Offset: 0x00062394
		bool IBindingList.SupportsSearching
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x00064198 File Offset: 0x00062398
		bool IBindingList.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x0006419C File Offset: 0x0006239C
		internal static void MergeEnumerableContent(JContainer target, IEnumerable content, JsonMergeSettings settings)
		{
			switch (settings.MergeArrayHandling)
			{
			case MergeArrayHandling.Concat:
			{
				using (IEnumerator enumerator = content.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						JToken jtoken = (JToken)obj;
						target.Add(jtoken);
					}
					return;
				}
				break;
			}
			case MergeArrayHandling.Union:
				break;
			case MergeArrayHandling.Replace:
				goto IL_00BF;
			case MergeArrayHandling.Merge:
				goto IL_0107;
			default:
				goto IL_01B9;
			}
			HashSet<JToken> hashSet = new HashSet<JToken>(target, JToken.EqualityComparer);
			using (IEnumerator enumerator = content.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj2 = enumerator.Current;
					JToken jtoken2 = (JToken)obj2;
					if (hashSet.Add(jtoken2))
					{
						target.Add(jtoken2);
					}
				}
				return;
			}
			IL_00BF:
			target.ClearItems();
			using (IEnumerator enumerator = content.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj3 = enumerator.Current;
					JToken jtoken3 = (JToken)obj3;
					target.Add(jtoken3);
				}
				return;
			}
			IL_0107:
			int num = 0;
			using (IEnumerator enumerator = content.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj4 = enumerator.Current;
					if (num < target.Count)
					{
						JContainer jcontainer = target[num] as JContainer;
						if (jcontainer != null)
						{
							jcontainer.Merge(obj4, settings);
						}
						else if (obj4 != null)
						{
							JToken jtoken4 = JContainer.CreateFromContent(obj4);
							if (jtoken4.Type != JTokenType.Null)
							{
								target[num] = jtoken4;
							}
						}
					}
					else
					{
						target.Add(obj4);
					}
					num++;
				}
				return;
			}
			IL_01B9:
			throw new ArgumentOutOfRangeException("settings", "Unexpected merge array handling when merging JSON.");
		}

		// Token: 0x0400068A RID: 1674
		internal ListChangedEventHandler _listChanged;

		// Token: 0x0400068B RID: 1675
		internal AddingNewEventHandler _addingNew;

		// Token: 0x0400068C RID: 1676
		internal NotifyCollectionChangedEventHandler _collectionChanged;

		// Token: 0x0400068D RID: 1677
		private object _syncRoot;

		// Token: 0x0400068E RID: 1678
		private bool _busy;
	}
}
