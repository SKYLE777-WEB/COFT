using System;
using System.ComponentModel;
using System.Numerics.Hashing;
using System.Runtime.InteropServices;

namespace System
{
	// Token: 0x02000023 RID: 35
	[ComVisible(true)]
	public readonly struct SequencePosition : IEquatable<SequencePosition>
	{
		// Token: 0x0600010A RID: 266 RVA: 0x00008060 File Offset: 0x00006260
		public SequencePosition(object @object, int integer)
		{
			this._object = @object;
			this._integer = integer;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00008070 File Offset: 0x00006270
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetObject()
		{
			return this._object;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00008078 File Offset: 0x00006278
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int GetInteger()
		{
			return this._integer;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00008080 File Offset: 0x00006280
		public bool Equals(SequencePosition other)
		{
			return this._integer == other._integer && object.Equals(this._object, other._object);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000080A8 File Offset: 0x000062A8
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			if (obj is SequencePosition)
			{
				SequencePosition sequencePosition = (SequencePosition)obj;
				return this.Equals(sequencePosition);
			}
			return false;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000080D8 File Offset: 0x000062D8
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			object @object = this._object;
			return HashHelpers.Combine((@object != null) ? @object.GetHashCode() : 0, this._integer);
		}

		// Token: 0x040000B8 RID: 184
		private readonly object _object;

		// Token: 0x040000B9 RID: 185
		private readonly int _integer;
	}
}
