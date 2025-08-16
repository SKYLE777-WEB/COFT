using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200011D RID: 285
	public class JsonPropertyCollection : KeyedCollection<string, JsonProperty>
	{
		// Token: 0x06000E8B RID: 3723 RVA: 0x00056FF8 File Offset: 0x000551F8
		public JsonPropertyCollection(Type type)
			: base(StringComparer.Ordinal)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			this._type = type;
			this._list = (List<JsonProperty>)base.Items;
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x00057028 File Offset: 0x00055228
		protected override string GetKeyForItem(JsonProperty item)
		{
			return item.PropertyName;
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x00057030 File Offset: 0x00055230
		public void AddProperty(JsonProperty property)
		{
			if (base.Contains(property.PropertyName))
			{
				if (property.Ignored)
				{
					return;
				}
				JsonProperty jsonProperty = base[property.PropertyName];
				bool flag = true;
				if (jsonProperty.Ignored)
				{
					base.Remove(jsonProperty);
					flag = false;
				}
				else if (property.DeclaringType != null && jsonProperty.DeclaringType != null)
				{
					if (property.DeclaringType.IsSubclassOf(jsonProperty.DeclaringType) || (jsonProperty.DeclaringType.IsInterface() && property.DeclaringType.ImplementInterface(jsonProperty.DeclaringType)))
					{
						base.Remove(jsonProperty);
						flag = false;
					}
					if (jsonProperty.DeclaringType.IsSubclassOf(property.DeclaringType) || (property.DeclaringType.IsInterface() && jsonProperty.DeclaringType.ImplementInterface(property.DeclaringType)))
					{
						return;
					}
				}
				if (flag)
				{
					throw new JsonSerializationException("A member with the name '{0}' already exists on '{1}'. Use the JsonPropertyAttribute to specify another name.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, this._type));
				}
			}
			base.Add(property);
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x0005715C File Offset: 0x0005535C
		public JsonProperty GetClosestMatchProperty(string propertyName)
		{
			JsonProperty jsonProperty = this.GetProperty(propertyName, StringComparison.Ordinal);
			if (jsonProperty == null)
			{
				jsonProperty = this.GetProperty(propertyName, StringComparison.OrdinalIgnoreCase);
			}
			return jsonProperty;
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x00057188 File Offset: 0x00055388
		private bool TryGetValue(string key, out JsonProperty item)
		{
			if (base.Dictionary == null)
			{
				item = null;
				return false;
			}
			return base.Dictionary.TryGetValue(key, out item);
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x000571A8 File Offset: 0x000553A8
		public JsonProperty GetProperty(string propertyName, StringComparison comparisonType)
		{
			if (comparisonType != StringComparison.Ordinal)
			{
				for (int i = 0; i < this._list.Count; i++)
				{
					JsonProperty jsonProperty = this._list[i];
					if (string.Equals(propertyName, jsonProperty.PropertyName, comparisonType))
					{
						return jsonProperty;
					}
				}
				return null;
			}
			JsonProperty jsonProperty2;
			if (this.TryGetValue(propertyName, out jsonProperty2))
			{
				return jsonProperty2;
			}
			return null;
		}

		// Token: 0x040005D3 RID: 1491
		private readonly Type _type;

		// Token: 0x040005D4 RID: 1492
		private readonly List<JsonProperty> _list;
	}
}
