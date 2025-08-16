using System;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000100 RID: 256
	internal class DefaultReferenceResolver : IReferenceResolver
	{
		// Token: 0x06000D8C RID: 3468 RVA: 0x00054E18 File Offset: 0x00053018
		private BidirectionalDictionary<string, object> GetMappings(object context)
		{
			JsonSerializerInternalBase jsonSerializerInternalBase = context as JsonSerializerInternalBase;
			if (jsonSerializerInternalBase == null)
			{
				JsonSerializerProxy jsonSerializerProxy = context as JsonSerializerProxy;
				if (jsonSerializerProxy == null)
				{
					throw new JsonException("The DefaultReferenceResolver can only be used internally.");
				}
				jsonSerializerInternalBase = jsonSerializerProxy.GetInternalSerializer();
			}
			return jsonSerializerInternalBase.DefaultReferenceMappings;
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x00054E60 File Offset: 0x00053060
		public object ResolveReference(object context, string reference)
		{
			object obj;
			this.GetMappings(context).TryGetByFirst(reference, out obj);
			return obj;
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x00054E84 File Offset: 0x00053084
		public string GetReference(object context, object value)
		{
			BidirectionalDictionary<string, object> mappings = this.GetMappings(context);
			string text;
			if (!mappings.TryGetBySecond(value, out text))
			{
				this._referenceCount++;
				text = this._referenceCount.ToString(CultureInfo.InvariantCulture);
				mappings.Set(text, value);
			}
			return text;
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x00054ED4 File Offset: 0x000530D4
		public void AddReference(object context, string reference, object value)
		{
			this.GetMappings(context).Set(reference, value);
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x00054EE4 File Offset: 0x000530E4
		public bool IsReferenced(object context, object value)
		{
			string text;
			return this.GetMappings(context).TryGetBySecond(value, out text);
		}

		// Token: 0x0400054B RID: 1355
		private int _referenceCount;
	}
}
