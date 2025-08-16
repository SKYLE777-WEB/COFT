using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LanguageIdentification.Internal;

namespace LanguageIdentification
{
	// Token: 0x02000016 RID: 22
	[NullableContext(1)]
	[Nullable(0)]
	[ComVisible(true)]
	public sealed class LanguageIdentificationClassifier : ILanguageIdentificationClassifier
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00006FF8 File Offset: 0x000051F8
		public LanguageIdentificationClassifier()
			: this(LanguageIdentificationModel.Default)
		{
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00007008 File Offset: 0x00005208
		public LanguageIdentificationClassifier(params string[] languageCodes)
			: this(languageCodes)
		{
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00007014 File Offset: 0x00005214
		public LanguageIdentificationClassifier(IEnumerable<string> languageCodes)
			: this(LanguageIdentificationModel.Create(languageCodes))
		{
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00007024 File Offset: 0x00005224
		public LanguageIdentificationClassifier(LanguageIdentificationModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			this._model = model;
			this._confidenceArrayPool = new FixedSizeArrayPool<float>(model.ClassesCount, Environment.ProcessorCount);
			this._counter = new ConfidenceCounter(this._confidenceArrayPool, model);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000707C File Offset: 0x0000527C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(string text)
		{
			this.Append(text.AsSpan());
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000708C File Offset: 0x0000528C
		[NullableContext(0)]
		public void Append(ReadOnlySpan<char> text)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(text.Length * 4);
			try
			{
				Span<byte> span = array;
				int bytes = LanguageIdentificationClassifier.s_encoding.GetBytes(text.ToArray(), 0, text.Length, array, 0);
				this.Append(span.Slice(0, bytes));
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00007108 File Offset: 0x00005308
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(byte[] buffer, int start, int length)
		{
			this.Append(new ReadOnlySpan<byte>(buffer, start, length));
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00007118 File Offset: 0x00005318
		[NullableContext(0)]
		public unsafe void Append(ReadOnlySpan<byte> buffer)
		{
			int length = buffer.Length;
			short num = 0;
			int[][] dsaOutput = this._model.dsaOutput;
			short[] dsa = this._model.dsa;
			for (int i = 0; i < length; i++)
			{
				num = dsa[((int)num << 8) + (int)(*buffer[i] & byte.MaxValue)];
				int[] array = dsaOutput[(int)num];
				if (array != null)
				{
					foreach (int num2 in array)
					{
						this._counter.Increment(num2);
					}
				}
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000071B4 File Offset: 0x000053B4
		public ILanguageDetectionResult Classify()
		{
			float[] array = this._counter.NaiveBayesClassConfidence();
			int num = 0;
			float num2 = array[num];
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] > num2)
				{
					num = i;
					num2 = array[i];
				}
			}
			return new PooledConfidenceLanguageDetectionResult(this._model.LanguageClasses[num], num, array, this._confidenceArrayPool);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00007218 File Offset: 0x00005418
		public ILanguageDetectionResultRank CreateRank()
		{
			float[] array = this._counter.NaiveBayesClassConfidence();
			return new LanguageDetectionResultRank(this._model.LanguageClasses, array, this._confidenceArrayPool);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000724C File Offset: 0x0000544C
		public IEnumerable<string> GetSupportedLanguages()
		{
			return this._model.GetSupportedLanguages();
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000725C File Offset: 0x0000545C
		public void Reset()
		{
			this._counter.Clear();
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000726C File Offset: 0x0000546C
		public static ILanguageDetectionResult Classify(string text)
		{
			return LanguageIdentificationClassifier.Classify(text.AsSpan());
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000727C File Offset: 0x0000547C
		[NullableContext(0)]
		[return: Nullable(1)]
		public static ILanguageDetectionResult Classify(ReadOnlySpan<char> text)
		{
			ILanguageIdentificationClassifier languageIdentificationClassifier = LanguageIdentificationClassifierPool.Default.Rent();
			ILanguageDetectionResult languageDetectionResult;
			try
			{
				languageIdentificationClassifier.Append(text);
				languageDetectionResult = languageIdentificationClassifier.Classify();
			}
			finally
			{
				LanguageIdentificationClassifierPool.Default.Return(languageIdentificationClassifier);
			}
			return languageDetectionResult;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000072C4 File Offset: 0x000054C4
		public static ILanguageDetectionResult Classify(byte[] buffer, int start, int length)
		{
			return LanguageIdentificationClassifier.Classify(new ReadOnlySpan<byte>(buffer, start, length));
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000072D4 File Offset: 0x000054D4
		[NullableContext(0)]
		[return: Nullable(1)]
		public static ILanguageDetectionResult Classify(ReadOnlySpan<byte> buffer)
		{
			ILanguageIdentificationClassifier languageIdentificationClassifier = LanguageIdentificationClassifierPool.Default.Rent();
			ILanguageDetectionResult languageDetectionResult;
			try
			{
				languageIdentificationClassifier.Append(buffer);
				languageDetectionResult = languageIdentificationClassifier.Classify();
			}
			finally
			{
				LanguageIdentificationClassifierPool.Default.Return(languageIdentificationClassifier);
			}
			return languageDetectionResult;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000731C File Offset: 0x0000551C
		public static ILanguageDetectionResultRank CreateRank(string text)
		{
			return LanguageIdentificationClassifier.CreateRank(text.AsSpan());
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000732C File Offset: 0x0000552C
		[NullableContext(0)]
		[return: Nullable(1)]
		public static ILanguageDetectionResultRank CreateRank(ReadOnlySpan<char> text)
		{
			ILanguageIdentificationClassifier languageIdentificationClassifier = LanguageIdentificationClassifierPool.Default.Rent();
			ILanguageDetectionResultRank languageDetectionResultRank;
			try
			{
				languageIdentificationClassifier.Append(text);
				languageDetectionResultRank = languageIdentificationClassifier.CreateRank();
			}
			finally
			{
				LanguageIdentificationClassifierPool.Default.Return(languageIdentificationClassifier);
			}
			return languageDetectionResultRank;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00007374 File Offset: 0x00005574
		public static ILanguageDetectionResultRank CreateRank(byte[] buffer, int start, int length)
		{
			return LanguageIdentificationClassifier.CreateRank(new ReadOnlySpan<byte>(buffer, start, length));
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00007384 File Offset: 0x00005584
		[NullableContext(0)]
		[return: Nullable(1)]
		public static ILanguageDetectionResultRank CreateRank(ReadOnlySpan<byte> buffer)
		{
			ILanguageIdentificationClassifier languageIdentificationClassifier = LanguageIdentificationClassifierPool.Default.Rent();
			ILanguageDetectionResultRank languageDetectionResultRank;
			try
			{
				languageIdentificationClassifier.Append(buffer);
				languageDetectionResultRank = languageIdentificationClassifier.CreateRank();
			}
			finally
			{
				LanguageIdentificationClassifierPool.Default.Return(languageIdentificationClassifier);
			}
			return languageDetectionResultRank;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000073CC File Offset: 0x000055CC
		public static IEnumerable<string> GetAllSupportedLanguages()
		{
			return LanguageIdentificationModel.Default.GetSupportedLanguages();
		}

		// Token: 0x0400008F RID: 143
		private static readonly Encoding s_encoding = Encoding.UTF8;

		// Token: 0x04000090 RID: 144
		private readonly FixedSizeArrayPool<float> _confidenceArrayPool;

		// Token: 0x04000091 RID: 145
		private readonly ConfidenceCounter _counter;

		// Token: 0x04000092 RID: 146
		private readonly LanguageIdentificationModel _model;
	}
}
