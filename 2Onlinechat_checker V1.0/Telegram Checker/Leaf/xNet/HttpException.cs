using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Leaf.xNet
{
	// Token: 0x0200005E RID: 94
	[ComVisible(true)]
	[Serializable]
	public sealed class HttpException : NetException
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000480 RID: 1152 RVA: 0x0001C900 File Offset: 0x0001AB00
		// (set) Token: 0x06000481 RID: 1153 RVA: 0x0001C908 File Offset: 0x0001AB08
		public HttpExceptionStatus Status { get; internal set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x0001C914 File Offset: 0x0001AB14
		public HttpStatusCode HttpStatusCode { get; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x0001C91C File Offset: 0x0001AB1C
		// (set) Token: 0x06000484 RID: 1156 RVA: 0x0001C924 File Offset: 0x0001AB24
		internal bool EmptyMessageBody { get; set; }

		// Token: 0x06000485 RID: 1157 RVA: 0x0001C930 File Offset: 0x0001AB30
		public HttpException()
			: this(Resources.HttpException_Default, null)
		{
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0001C940 File Offset: 0x0001AB40
		public HttpException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001C94C File Offset: 0x0001AB4C
		public HttpException(string message, HttpExceptionStatus status, HttpStatusCode httpStatusCode = HttpStatusCode.None, Exception innerException = null)
			: base(message, innerException)
		{
			this.Status = status;
			this.HttpStatusCode = httpStatusCode;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0001C968 File Offset: 0x0001AB68
		public HttpException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
			if (serializationInfo == null)
			{
				return;
			}
			this.Status = (HttpExceptionStatus)serializationInfo.GetInt32("Status");
			this.HttpStatusCode = (HttpStatusCode)serializationInfo.GetInt32("HttpStatusCode");
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0001C9AC File Offset: 0x0001ABAC
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
			serializationInfo.AddValue("Status", (int)this.Status);
			serializationInfo.AddValue("HttpStatusCode", (int)this.HttpStatusCode);
			serializationInfo.AddValue("EmptyMessageBody", this.EmptyMessageBody);
		}
	}
}
