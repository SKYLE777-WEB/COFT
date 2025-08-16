using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x0200011E RID: 286
	internal abstract class JsonSerializerInternalBase
	{
		// Token: 0x06000E91 RID: 3729 RVA: 0x00057210 File Offset: 0x00055410
		protected JsonSerializerInternalBase(JsonSerializer serializer)
		{
			ValidationUtils.ArgumentNotNull(serializer, "serializer");
			this.Serializer = serializer;
			this.TraceWriter = serializer.TraceWriter;
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000E92 RID: 3730 RVA: 0x00057238 File Offset: 0x00055438
		internal BidirectionalDictionary<string, object> DefaultReferenceMappings
		{
			get
			{
				if (this._mappings == null)
				{
					this._mappings = new BidirectionalDictionary<string, object>(EqualityComparer<string>.Default, new JsonSerializerInternalBase.ReferenceEqualsEqualityComparer(), "A different value already has the Id '{0}'.", "A different Id has already been assigned for value '{0}'.");
				}
				return this._mappings;
			}
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x0005726C File Offset: 0x0005546C
		private ErrorContext GetErrorContext(object currentObject, object member, string path, Exception error)
		{
			if (this._currentErrorContext == null)
			{
				this._currentErrorContext = new ErrorContext(currentObject, member, path, error);
			}
			if (this._currentErrorContext.Error != error)
			{
				throw new InvalidOperationException("Current error context error is different to requested error.");
			}
			return this._currentErrorContext;
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x000572AC File Offset: 0x000554AC
		protected void ClearErrorContext()
		{
			if (this._currentErrorContext == null)
			{
				throw new InvalidOperationException("Could not clear error context. Error context is already null.");
			}
			this._currentErrorContext = null;
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x000572CC File Offset: 0x000554CC
		protected bool IsErrorHandled(object currentObject, JsonContract contract, object keyValue, IJsonLineInfo lineInfo, string path, Exception ex)
		{
			ErrorContext errorContext = this.GetErrorContext(currentObject, keyValue, path, ex);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Error && !errorContext.Traced)
			{
				errorContext.Traced = true;
				string text = ((base.GetType() == typeof(JsonSerializerInternalWriter)) ? "Error serializing" : "Error deserializing");
				if (contract != null)
				{
					text = text + " " + contract.UnderlyingType;
				}
				text = text + ". " + ex.Message;
				if (!(ex is JsonException))
				{
					text = JsonPosition.FormatMessage(lineInfo, path, text);
				}
				this.TraceWriter.Trace(TraceLevel.Error, text, ex);
			}
			if (contract != null && currentObject != null)
			{
				contract.InvokeOnError(currentObject, this.Serializer.Context, errorContext);
			}
			if (!errorContext.Handled)
			{
				this.Serializer.OnError(new ErrorEventArgs(currentObject, errorContext));
			}
			return errorContext.Handled;
		}

		// Token: 0x040005D5 RID: 1493
		private ErrorContext _currentErrorContext;

		// Token: 0x040005D6 RID: 1494
		private BidirectionalDictionary<string, object> _mappings;

		// Token: 0x040005D7 RID: 1495
		internal readonly JsonSerializer Serializer;

		// Token: 0x040005D8 RID: 1496
		internal readonly ITraceWriter TraceWriter;

		// Token: 0x040005D9 RID: 1497
		protected JsonSerializerProxy InternalSerializer;

		// Token: 0x02000278 RID: 632
		private class ReferenceEqualsEqualityComparer : IEqualityComparer<object>
		{
			// Token: 0x06001733 RID: 5939 RVA: 0x00083AE8 File Offset: 0x00081CE8
			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return x == y;
			}

			// Token: 0x06001734 RID: 5940 RVA: 0x00083AF0 File Offset: 0x00081CF0
			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}
	}
}
