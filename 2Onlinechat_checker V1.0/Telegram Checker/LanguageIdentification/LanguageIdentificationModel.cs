using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace LanguageIdentification
{
	// Token: 0x02000018 RID: 24
	[NullableContext(1)]
	[Nullable(0)]
	[ComVisible(true)]
	public sealed class LanguageIdentificationModel : IEquatable<LanguageIdentificationModel>
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000CD RID: 205 RVA: 0x000074FC File Offset: 0x000056FC
		public static LanguageIdentificationModel Default
		{
			get
			{
				LanguageIdentificationModel languageIdentificationModel;
				if (LanguageIdentificationModel.s_defaultModelReference.TryGetTarget(out languageIdentificationModel))
				{
					return languageIdentificationModel;
				}
				WeakReference<LanguageIdentificationModel> weakReference = LanguageIdentificationModel.s_defaultModelReference;
				lock (weakReference)
				{
					if (LanguageIdentificationModel.s_defaultModelReference.TryGetTarget(out languageIdentificationModel))
					{
						return languageIdentificationModel;
					}
					languageIdentificationModel = LanguageIdentificationModel.InternalLoadDefaultModel();
					LanguageIdentificationModel.s_defaultModelReference.SetTarget(languageIdentificationModel);
				}
				return languageIdentificationModel;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000CE RID: 206 RVA: 0x0000757C File Offset: 0x0000577C
		public int ClassesCount { get; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00007584 File Offset: 0x00005784
		public int FeaturesCount { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x0000758C File Offset: 0x0000578C
		internal string[] LanguageClasses { get; }

		// Token: 0x060000D1 RID: 209 RVA: 0x00007594 File Offset: 0x00005794
		public LanguageIdentificationModel(string[] langClasses, float[] ptc, float[] pc, short[] dsa, [Nullable(new byte[] { 1, 2 })] int[][] dsaOutput)
		{
			if (langClasses == null)
			{
				throw new ArgumentNullException("langClasses");
			}
			this.LanguageClasses = langClasses;
			if (ptc == null)
			{
				throw new ArgumentNullException("ptc");
			}
			this.nb_ptc = ptc;
			if (pc == null)
			{
				throw new ArgumentNullException("pc");
			}
			this.nb_pc = pc;
			if (dsa == null)
			{
				throw new ArgumentNullException("dsa");
			}
			this.dsa = dsa;
			if (dsaOutput == null)
			{
				throw new ArgumentNullException("dsaOutput");
			}
			this.dsaOutput = dsaOutput;
			this.ClassesCount = langClasses.Length;
			this.FeaturesCount = this.nb_ptc.Length / this.ClassesCount;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007648 File Offset: 0x00005848
		public static LanguageIdentificationModel Create(params string[] languageCodes)
		{
			return LanguageIdentificationModel.Create(languageCodes);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00007650 File Offset: 0x00005850
		public static LanguageIdentificationModel Create(IEnumerable<string> languageCodes)
		{
			LanguageIdentificationModel @default = LanguageIdentificationModel.Default;
			string[] array = languageCodes.Distinct<string>().ToArray<string>();
			HashSet<string> hashSet = new HashSet<string>(@default.LanguageClasses.Intersect(array));
			if (hashSet.Count != array.Length)
			{
				string text = string.Join(", ", array.Except(hashSet));
				throw new ArgumentException("there is some languageCode not supported or error languageCode. " + text);
			}
			if (hashSet.Count < 2)
			{
				throw new ArgumentException("A model must contain at least two languages.");
			}
			float[] array2 = new float[hashSet.Count];
			float[] array3 = new float[hashSet.Count * @default.FeaturesCount];
			int i = 0;
			int num = 0;
			while (i < @default.ClassesCount)
			{
				if (hashSet.Contains(@default.LanguageClasses[i]))
				{
					array2[num] = @default.nb_pc[i];
					for (int j = 0; j < @default.FeaturesCount; j++)
					{
						int num2 = @default.FeaturesCount * i + j;
						int num3 = @default.FeaturesCount * num + j;
						array3[num3] = @default.nb_ptc[num2];
					}
					num++;
				}
				i++;
			}
			return new LanguageIdentificationModel(hashSet.ToArray<string>(), array3, array2, @default.dsa, @default.dsaOutput);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007794 File Offset: 0x00005994
		public static LanguageIdentificationModel Deserialize(Stream stream)
		{
			LanguageIdentificationModel languageIdentificationModel;
			using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
			{
				string[] array = binaryReader.ReadStringArray();
				float[] array2 = binaryReader.ReadFloatArray();
				float[] array3 = binaryReader.ReadFloatArray();
				short[] array4 = binaryReader.ReadInt16Array();
				int[][] array5;
				int num = binaryReader.ReadArrayLength(out array5);
				int[][] array6;
				if (num > 0)
				{
					array6 = new int[num][];
					for (int i = 0; i < num; i++)
					{
						array6[i] = binaryReader.ReadInt32Array();
					}
				}
				else
				{
					array6 = array5;
				}
				string[] array7 = array;
				float[] array8 = array2;
				float[] array9 = array3;
				short[] array10 = array4;
				int[][] array11 = array6;
				if (array11 == null)
				{
					throw new SerializationException("dsaOutput deserialize fail.");
				}
				languageIdentificationModel = new LanguageIdentificationModel(array7, array8, array9, array10, array11);
			}
			return languageIdentificationModel;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000785C File Offset: 0x00005A5C
		public IEnumerable<string> GetSupportedLanguages()
		{
			return this.LanguageClasses.AsEnumerable<string>();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000786C File Offset: 0x00005A6C
		public void Serialize(Stream stream)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
			{
				binaryWriter.WriteStringArray(this.LanguageClasses);
				binaryWriter.WriteFloatArray(this.nb_ptc);
				binaryWriter.WriteFloatArray(this.nb_pc);
				binaryWriter.WriteInt16Array(this.dsa);
				if (binaryWriter.WriteArrayLength(this.dsaOutput))
				{
					foreach (int[] array2 in this.dsaOutput)
					{
						binaryWriter.WriteInt32Array(array2);
					}
				}
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00007910 File Offset: 0x00005B10
		[NullableContext(2)]
		public bool Equals(LanguageIdentificationModel other)
		{
			return other != null && other.ClassesCount == this.ClassesCount && LanguageIdentificationModel.<Equals>g__SequenceEqual|22_0<short>(other.dsa, this.dsa) && other.FeaturesCount == this.FeaturesCount && LanguageIdentificationModel.<Equals>g__SequenceEqual|22_0<string>(other.LanguageClasses, this.LanguageClasses) && LanguageIdentificationModel.<Equals>g__SequenceEqual|22_0<float>(other.nb_pc, this.nb_pc) && LanguageIdentificationModel.<Equals>g__SequenceEqual|22_0<float>(other.nb_ptc, this.nb_ptc) && other.dsaOutput != null && this.dsaOutput != null && other.dsaOutput.SequenceEqual(this.dsaOutput, new LanguageIdentificationModel.ArrayEqualityComparer<int>());
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000079D0 File Offset: 0x00005BD0
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as LanguageIdentificationModel);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000079E0 File Offset: 0x00005BE0
		private static LanguageIdentificationModel InternalLoadDefaultModel()
		{
			LanguageIdentificationModel languageIdentificationModel;
			using (MemoryStream memoryStream = new MemoryStream(Resource.ModelData))
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					using (BufferedStream bufferedStream = new BufferedStream(gzipStream))
					{
						languageIdentificationModel = LanguageIdentificationModel.Deserialize(bufferedStream);
					}
				}
			}
			return languageIdentificationModel;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00007A74 File Offset: 0x00005C74
		[CompilerGenerated]
		internal static bool <Equals>g__SequenceEqual|22_0<T>([Nullable(new byte[] { 2, 0 })] T[] array1, [Nullable(new byte[] { 2, 0 })] T[] array2)
		{
			return array1 != null && array2 != null && array1.SequenceEqual(array2);
		}

		// Token: 0x04000097 RID: 151
		internal short[] dsa;

		// Token: 0x04000098 RID: 152
		[Nullable(new byte[] { 1, 2 })]
		internal int[][] dsaOutput;

		// Token: 0x04000099 RID: 153
		internal float[] nb_pc;

		// Token: 0x0400009A RID: 154
		internal float[] nb_ptc;

		// Token: 0x0400009B RID: 155
		[Nullable(new byte[] { 1, 2 })]
		private static readonly WeakReference<LanguageIdentificationModel> s_defaultModelReference = new WeakReference<LanguageIdentificationModel>(null);

		// Token: 0x020001B7 RID: 439
		[NullableContext(0)]
		private class ArrayEqualityComparer<[Nullable(2)] T> : IEqualityComparer<T[]>
		{
			// Token: 0x06001539 RID: 5433 RVA: 0x00075678 File Offset: 0x00073878
			public bool Equals([Nullable(new byte[] { 2, 1 })] T[] x, [Nullable(new byte[] { 2, 1 })] T[] y)
			{
				return (x == null && y == null) || (x != null && y != null && x.SequenceEqual(y));
			}

			// Token: 0x0600153A RID: 5434 RVA: 0x000756A0 File Offset: 0x000738A0
			[NullableContext(1)]
			public int GetHashCode(T[] obj)
			{
				int num = int.MaxValue;
				foreach (T t in obj)
				{
					num &= ((t != null) ? t.GetHashCode() : 0);
				}
				return num;
			}
		}
	}
}
