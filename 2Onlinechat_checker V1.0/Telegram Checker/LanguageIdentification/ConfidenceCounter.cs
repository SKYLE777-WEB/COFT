using System;
using System.Runtime.CompilerServices;

namespace LanguageIdentification
{
	// Token: 0x02000011 RID: 17
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ConfidenceCounter
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00006E18 File Offset: 0x00005018
		public ConfidenceCounter(IFixedSizeArrayPool<float> confidenceArrayPool, LanguageIdentificationModel langIdModel)
		{
			if (confidenceArrayPool == null)
			{
				throw new ArgumentNullException("confidenceArrayPool");
			}
			this._confidenceArrayPool = confidenceArrayPool;
			if (langIdModel == null)
			{
				throw new ArgumentNullException("langIdModel");
			}
			this._langIdModel = langIdModel;
			this._featuresCount = this._langIdModel.FeaturesCount;
			this._classesCount = this._langIdModel.ClassesCount;
			this._sparse = new int[this._featuresCount + 1];
			this._dense = new int[this._featuresCount];
			this._counts = new int[this._featuresCount];
			this._origin_nb_pc = this._langIdModel.nb_pc;
			this._origin_nb_ptc = this._langIdModel.nb_ptc;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006EDC File Offset: 0x000050DC
		public void Clear()
		{
			this._elementsCount = 0;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006EE8 File Offset: 0x000050E8
		public void Increment(int key)
		{
			int num = this._sparse[key];
			if (num < this._elementsCount && this._dense[num] == key)
			{
				this._counts[num]++;
				return;
			}
			int elementsCount = this._elementsCount;
			this._elementsCount = elementsCount + 1;
			num = elementsCount;
			this._sparse[key] = num;
			this._dense[num] = key;
			this._counts[num] = 1;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006F5C File Offset: 0x0000515C
		public float[] NaiveBayesClassConfidence()
		{
			float[] array = this._confidenceArrayPool.Rent();
			Array.Copy(this._origin_nb_pc, 0, array, 0, array.Length);
			int i = 0;
			int num = 0;
			while (i < this._classesCount)
			{
				float num2 = 0f;
				for (int j = 0; j < this._elementsCount; j++)
				{
					int num3 = this._dense[j];
					num2 += (float)this._counts[j] * this._origin_nb_ptc[num + num3];
				}
				array[i] += num2;
				i++;
				num += this._featuresCount;
			}
			return array;
		}

		// Token: 0x04000085 RID: 133
		private readonly int _classesCount;

		// Token: 0x04000086 RID: 134
		private readonly IFixedSizeArrayPool<float> _confidenceArrayPool;

		// Token: 0x04000087 RID: 135
		private readonly int[] _counts;

		// Token: 0x04000088 RID: 136
		private readonly int[] _dense;

		// Token: 0x04000089 RID: 137
		private readonly int _featuresCount;

		// Token: 0x0400008A RID: 138
		private readonly LanguageIdentificationModel _langIdModel;

		// Token: 0x0400008B RID: 139
		private readonly float[] _origin_nb_pc;

		// Token: 0x0400008C RID: 140
		private readonly float[] _origin_nb_ptc;

		// Token: 0x0400008D RID: 141
		private readonly int[] _sparse;

		// Token: 0x0400008E RID: 142
		private int _elementsCount;
	}
}
