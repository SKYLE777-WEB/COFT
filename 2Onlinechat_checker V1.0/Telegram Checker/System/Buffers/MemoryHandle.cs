using System;
using System.Runtime.InteropServices;

namespace System.Buffers
{
	// Token: 0x02000045 RID: 69
	[ComVisible(true)]
	public struct MemoryHandle : IDisposable
	{
		// Token: 0x060002E0 RID: 736 RVA: 0x00012678 File Offset: 0x00010878
		[CLSCompliant(false)]
		public unsafe MemoryHandle(void* pointer, GCHandle handle = default(GCHandle), IPinnable pinnable = null)
		{
			this._pointer = pointer;
			this._handle = handle;
			this._pinnable = pinnable;
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x00012690 File Offset: 0x00010890
		[CLSCompliant(false)]
		public unsafe void* Pointer
		{
			get
			{
				return this._pointer;
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00012698 File Offset: 0x00010898
		public void Dispose()
		{
			if (this._handle.IsAllocated)
			{
				this._handle.Free();
			}
			if (this._pinnable != null)
			{
				this._pinnable.Unpin();
				this._pinnable = null;
			}
			this._pointer = null;
		}

		// Token: 0x04000141 RID: 321
		private unsafe void* _pointer;

		// Token: 0x04000142 RID: 322
		private GCHandle _handle;

		// Token: 0x04000143 RID: 323
		private IPinnable _pinnable;
	}
}
