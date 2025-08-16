using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Leaf.xNet
{
	// Token: 0x02000054 RID: 84
	[ComVisible(true)]
	[Serializable]
	public class NetException : Exception
	{
		// Token: 0x060003D6 RID: 982 RVA: 0x0001A804 File Offset: 0x00018A04
		public NetException()
			: this(Resources.NetException_Default, null)
		{
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0001A814 File Offset: 0x00018A14
		public NetException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0001A820 File Offset: 0x00018A20
		protected NetException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
