using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000145 RID: 325
	public struct JEnumerable<T> : IJEnumerable<T>, IEnumerable<T>, IEnumerable, IEquatable<JEnumerable<T>> where T : JToken
	{
		// Token: 0x06001146 RID: 4422 RVA: 0x000643A8 File Offset: 0x000625A8
		public JEnumerable(IEnumerable<T> enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, "enumerable");
			this._enumerable = enumerable;
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x000643BC File Offset: 0x000625BC
		public IEnumerator<T> GetEnumerator()
		{
			return (this._enumerable ?? JEnumerable<T>.Empty).GetEnumerator();
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x000643DC File Offset: 0x000625DC
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x17000326 RID: 806
		public IJEnumerable<JToken> this[object key]
		{
			get
			{
				if (this._enumerable == null)
				{
					return JEnumerable<JToken>.Empty;
				}
				return new JEnumerable<JToken>(this._enumerable.Values(key));
			}
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00064414 File Offset: 0x00062614
		public bool Equals(JEnumerable<T> other)
		{
			return object.Equals(this._enumerable, other._enumerable);
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x00064428 File Offset: 0x00062628
		public override bool Equals(object obj)
		{
			return obj is JEnumerable<T> && this.Equals((JEnumerable<T>)obj);
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x00064444 File Offset: 0x00062644
		public override int GetHashCode()
		{
			if (this._enumerable == null)
			{
				return 0;
			}
			return this._enumerable.GetHashCode();
		}

		// Token: 0x0400068F RID: 1679
		public static readonly JEnumerable<T> Empty = new JEnumerable<T>(Enumerable.Empty<T>());

		// Token: 0x04000690 RID: 1680
		private readonly IEnumerable<T> _enumerable;
	}
}
