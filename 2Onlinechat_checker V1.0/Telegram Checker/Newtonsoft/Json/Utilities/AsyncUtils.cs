using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000C8 RID: 200
	internal static class AsyncUtils
	{
		// Token: 0x06000B56 RID: 2902 RVA: 0x000483D8 File Offset: 0x000465D8
		internal static Task<bool> ToAsync(this bool value)
		{
			if (!value)
			{
				return AsyncUtils.False;
			}
			return AsyncUtils.True;
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x000483EC File Offset: 0x000465EC
		public static Task CancelIfRequestedAsync(this CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return null;
			}
			return cancellationToken.FromCanceled();
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x00048404 File Offset: 0x00046604
		public static Task<T> CancelIfRequestedAsync<T>(this CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return null;
			}
			return cancellationToken.FromCanceled<T>();
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0004841C File Offset: 0x0004661C
		public static Task FromCanceled(this CancellationToken cancellationToken)
		{
			return new Task(delegate
			{
			}, cancellationToken);
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x00048448 File Offset: 0x00046648
		public static Task<T> FromCanceled<T>(this CancellationToken cancellationToken)
		{
			return new Task<T>(() => default(T), cancellationToken);
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x00048474 File Offset: 0x00046674
		public static Task WriteAsync(this TextWriter writer, char value, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return writer.WriteAsync(value);
			}
			return cancellationToken.FromCanceled();
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x00048490 File Offset: 0x00046690
		public static Task WriteAsync(this TextWriter writer, string value, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return writer.WriteAsync(value);
			}
			return cancellationToken.FromCanceled();
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x000484AC File Offset: 0x000466AC
		public static Task WriteAsync(this TextWriter writer, char[] value, int start, int count, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return writer.WriteAsync(value, start, count);
			}
			return cancellationToken.FromCanceled();
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x000484CC File Offset: 0x000466CC
		public static Task<int> ReadAsync(this TextReader reader, char[] buffer, int index, int count, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return reader.ReadAsync(buffer, index, count);
			}
			return cancellationToken.FromCanceled<int>();
		}

		// Token: 0x04000476 RID: 1142
		public static readonly Task<bool> False = Task.FromResult<bool>(false);

		// Token: 0x04000477 RID: 1143
		public static readonly Task<bool> True = Task.FromResult<bool>(true);

		// Token: 0x04000478 RID: 1144
		internal static readonly Task CompletedTask = Task.Delay(0);
	}
}
