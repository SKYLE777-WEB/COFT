using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000114 RID: 276
	public abstract class JsonContract
	{
		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x00055D5C File Offset: 0x00053F5C
		public Type UnderlyingType { get; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000DEA RID: 3562 RVA: 0x00055D64 File Offset: 0x00053F64
		// (set) Token: 0x06000DEB RID: 3563 RVA: 0x00055D6C File Offset: 0x00053F6C
		public Type CreatedType
		{
			get
			{
				return this._createdType;
			}
			set
			{
				this._createdType = value;
				this.IsSealed = this._createdType.IsSealed();
				this.IsInstantiable = !this._createdType.IsInterface() && !this._createdType.IsAbstract();
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000DEC RID: 3564 RVA: 0x00055DC0 File Offset: 0x00053FC0
		// (set) Token: 0x06000DED RID: 3565 RVA: 0x00055DC8 File Offset: 0x00053FC8
		public bool? IsReference { get; set; }

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000DEE RID: 3566 RVA: 0x00055DD4 File Offset: 0x00053FD4
		// (set) Token: 0x06000DEF RID: 3567 RVA: 0x00055DDC File Offset: 0x00053FDC
		public JsonConverter Converter { get; set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x00055DE8 File Offset: 0x00053FE8
		// (set) Token: 0x06000DF1 RID: 3569 RVA: 0x00055DF0 File Offset: 0x00053FF0
		internal JsonConverter InternalConverter { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x00055DFC File Offset: 0x00053FFC
		public IList<SerializationCallback> OnDeserializedCallbacks
		{
			get
			{
				if (this._onDeserializedCallbacks == null)
				{
					this._onDeserializedCallbacks = new List<SerializationCallback>();
				}
				return this._onDeserializedCallbacks;
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x00055E1C File Offset: 0x0005401C
		public IList<SerializationCallback> OnDeserializingCallbacks
		{
			get
			{
				if (this._onDeserializingCallbacks == null)
				{
					this._onDeserializingCallbacks = new List<SerializationCallback>();
				}
				return this._onDeserializingCallbacks;
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x00055E3C File Offset: 0x0005403C
		public IList<SerializationCallback> OnSerializedCallbacks
		{
			get
			{
				if (this._onSerializedCallbacks == null)
				{
					this._onSerializedCallbacks = new List<SerializationCallback>();
				}
				return this._onSerializedCallbacks;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x00055E5C File Offset: 0x0005405C
		public IList<SerializationCallback> OnSerializingCallbacks
		{
			get
			{
				if (this._onSerializingCallbacks == null)
				{
					this._onSerializingCallbacks = new List<SerializationCallback>();
				}
				return this._onSerializingCallbacks;
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x00055E7C File Offset: 0x0005407C
		public IList<SerializationErrorCallback> OnErrorCallbacks
		{
			get
			{
				if (this._onErrorCallbacks == null)
				{
					this._onErrorCallbacks = new List<SerializationErrorCallback>();
				}
				return this._onErrorCallbacks;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00055E9C File Offset: 0x0005409C
		// (set) Token: 0x06000DF8 RID: 3576 RVA: 0x00055EA4 File Offset: 0x000540A4
		public Func<object> DefaultCreator { get; set; }

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000DF9 RID: 3577 RVA: 0x00055EB0 File Offset: 0x000540B0
		// (set) Token: 0x06000DFA RID: 3578 RVA: 0x00055EB8 File Offset: 0x000540B8
		public bool DefaultCreatorNonPublic { get; set; }

		// Token: 0x06000DFB RID: 3579 RVA: 0x00055EC4 File Offset: 0x000540C4
		internal JsonContract(Type underlyingType)
		{
			ValidationUtils.ArgumentNotNull(underlyingType, "underlyingType");
			this.UnderlyingType = underlyingType;
			this.IsNullable = ReflectionUtils.IsNullable(underlyingType);
			this.NonNullableUnderlyingType = ((this.IsNullable && ReflectionUtils.IsNullableType(underlyingType)) ? Nullable.GetUnderlyingType(underlyingType) : underlyingType);
			this.CreatedType = this.NonNullableUnderlyingType;
			this.IsConvertable = ConvertUtils.IsConvertible(this.NonNullableUnderlyingType);
			this.IsEnum = this.NonNullableUnderlyingType.IsEnum();
			this.InternalReadType = ReadType.Read;
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00055F58 File Offset: 0x00054158
		internal void InvokeOnSerializing(object o, StreamingContext context)
		{
			if (this._onSerializingCallbacks != null)
			{
				foreach (SerializationCallback serializationCallback in this._onSerializingCallbacks)
				{
					serializationCallback(o, context);
				}
			}
		}

		// Token: 0x06000DFD RID: 3581 RVA: 0x00055FB8 File Offset: 0x000541B8
		internal void InvokeOnSerialized(object o, StreamingContext context)
		{
			if (this._onSerializedCallbacks != null)
			{
				foreach (SerializationCallback serializationCallback in this._onSerializedCallbacks)
				{
					serializationCallback(o, context);
				}
			}
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x00056018 File Offset: 0x00054218
		internal void InvokeOnDeserializing(object o, StreamingContext context)
		{
			if (this._onDeserializingCallbacks != null)
			{
				foreach (SerializationCallback serializationCallback in this._onDeserializingCallbacks)
				{
					serializationCallback(o, context);
				}
			}
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x00056078 File Offset: 0x00054278
		internal void InvokeOnDeserialized(object o, StreamingContext context)
		{
			if (this._onDeserializedCallbacks != null)
			{
				foreach (SerializationCallback serializationCallback in this._onDeserializedCallbacks)
				{
					serializationCallback(o, context);
				}
			}
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x000560DC File Offset: 0x000542DC
		internal void InvokeOnError(object o, StreamingContext context, ErrorContext errorContext)
		{
			if (this._onErrorCallbacks != null)
			{
				foreach (SerializationErrorCallback serializationErrorCallback in this._onErrorCallbacks)
				{
					serializationErrorCallback(o, context, errorContext);
				}
			}
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x00056140 File Offset: 0x00054340
		internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo)
		{
			return delegate(object o, StreamingContext context)
			{
				callbackMethodInfo.Invoke(o, new object[] { context });
			};
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x0005615C File Offset: 0x0005435C
		internal static SerializationErrorCallback CreateSerializationErrorCallback(MethodInfo callbackMethodInfo)
		{
			return delegate(object o, StreamingContext context, ErrorContext econtext)
			{
				callbackMethodInfo.Invoke(o, new object[] { context, econtext });
			};
		}

		// Token: 0x0400057A RID: 1402
		internal bool IsNullable;

		// Token: 0x0400057B RID: 1403
		internal bool IsConvertable;

		// Token: 0x0400057C RID: 1404
		internal bool IsEnum;

		// Token: 0x0400057D RID: 1405
		internal Type NonNullableUnderlyingType;

		// Token: 0x0400057E RID: 1406
		internal ReadType InternalReadType;

		// Token: 0x0400057F RID: 1407
		internal JsonContractType ContractType;

		// Token: 0x04000580 RID: 1408
		internal bool IsReadOnlyOrFixedSize;

		// Token: 0x04000581 RID: 1409
		internal bool IsSealed;

		// Token: 0x04000582 RID: 1410
		internal bool IsInstantiable;

		// Token: 0x04000583 RID: 1411
		private List<SerializationCallback> _onDeserializedCallbacks;

		// Token: 0x04000584 RID: 1412
		private IList<SerializationCallback> _onDeserializingCallbacks;

		// Token: 0x04000585 RID: 1413
		private IList<SerializationCallback> _onSerializedCallbacks;

		// Token: 0x04000586 RID: 1414
		private IList<SerializationCallback> _onSerializingCallbacks;

		// Token: 0x04000587 RID: 1415
		private IList<SerializationErrorCallback> _onErrorCallbacks;

		// Token: 0x04000588 RID: 1416
		private Type _createdType;
	}
}
