using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Leaf.xNet
{
	// Token: 0x02000056 RID: 86
	[ComVisible(true)]
	public class RequestParams : List<KeyValuePair<string, string>>
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060003DC RID: 988 RVA: 0x0001A884 File Offset: 0x00018A84
		public string Query
		{
			get
			{
				return Http.ToQueryString(this, this.ValuesUnescaped, this.KeysUnescaped);
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0001A898 File Offset: 0x00018A98
		public RequestParams(bool valuesUnescaped = false, bool keysUnescaped = false)
		{
			this.ValuesUnescaped = valuesUnescaped;
			this.KeysUnescaped = keysUnescaped;
		}

		// Token: 0x1700007F RID: 127
		public object this[string paramName]
		{
			set
			{
				if (paramName == null)
				{
					throw new ArgumentNullException("paramName");
				}
				if (paramName.Length == 0)
				{
					throw ExceptionHelper.EmptyString("paramName");
				}
				string text = ((value != null) ? value.ToString() : null) ?? string.Empty;
				base.Add(new KeyValuePair<string, string>(paramName, text));
			}
		}

		// Token: 0x04000190 RID: 400
		public readonly bool ValuesUnescaped;

		// Token: 0x04000191 RID: 401
		public readonly bool KeysUnescaped;
	}
}
