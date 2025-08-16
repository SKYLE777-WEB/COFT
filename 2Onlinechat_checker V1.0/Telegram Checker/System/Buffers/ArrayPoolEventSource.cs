using System;
using System.Diagnostics.Tracing;

namespace System.Buffers
{
	// Token: 0x0200008A RID: 138
	[EventSource(Name = "System.Buffers.ArrayPoolEventSource")]
	internal sealed class ArrayPoolEventSource : EventSource
	{
		// Token: 0x060006A4 RID: 1700 RVA: 0x0002438C File Offset: 0x0002258C
		[Event(1, Level = EventLevel.Verbose)]
		internal unsafe void BufferRented(int bufferId, int bufferSize, int poolId, int bucketId)
		{
			EventSource.EventData* ptr;
			checked
			{
				ptr = stackalloc EventSource.EventData[unchecked((UIntPtr)4) * (UIntPtr)sizeof(EventSource.EventData)];
			}
			*ptr = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&bufferId))
			};
			ptr[1] = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&bufferSize))
			};
			ptr[2] = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&poolId))
			};
			ptr[3] = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&bucketId))
			};
			base.WriteEventCore(1, 4, ptr);
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00024468 File Offset: 0x00022668
		[Event(2, Level = EventLevel.Informational)]
		internal unsafe void BufferAllocated(int bufferId, int bufferSize, int poolId, int bucketId, ArrayPoolEventSource.BufferAllocatedReason reason)
		{
			EventSource.EventData* ptr;
			checked
			{
				ptr = stackalloc EventSource.EventData[unchecked((UIntPtr)5) * (UIntPtr)sizeof(EventSource.EventData)];
			}
			*ptr = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&bufferId))
			};
			ptr[1] = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&bufferSize))
			};
			ptr[2] = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&poolId))
			};
			ptr[3] = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&bucketId))
			};
			ptr[4] = new EventSource.EventData
			{
				Size = 4,
				DataPointer = (IntPtr)((void*)(&reason))
			};
			base.WriteEventCore(2, 5, ptr);
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x00024574 File Offset: 0x00022774
		[Event(3, Level = EventLevel.Verbose)]
		internal void BufferReturned(int bufferId, int bufferSize, int poolId)
		{
			base.WriteEvent(3, bufferId, bufferSize, poolId);
		}

		// Token: 0x040002EC RID: 748
		internal static readonly ArrayPoolEventSource Log = new ArrayPoolEventSource();

		// Token: 0x020001E8 RID: 488
		internal enum BufferAllocatedReason
		{
			// Token: 0x04000887 RID: 2183
			Pooled,
			// Token: 0x04000888 RID: 2184
			OverMaximumSize,
			// Token: 0x04000889 RID: 2185
			PoolExhausted
		}
	}
}
