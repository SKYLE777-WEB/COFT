using System;
using System.Runtime.CompilerServices;

namespace System.IO
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	internal static class BinarySerializerExtensions
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00006B80 File Offset: 0x00004D80
		[return: Nullable(2)]
		public static float[] ReadFloatArray(this BinaryReader reader)
		{
			float[] array;
			int num = reader.ReadArrayLength(out array);
			if (num < 1)
			{
				return array;
			}
			array = new float[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadSingle();
			}
			return array;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00006BC4 File Offset: 0x00004DC4
		[return: Nullable(2)]
		public static short[] ReadInt16Array(this BinaryReader reader)
		{
			short[] array;
			int num = reader.ReadArrayLength(out array);
			if (num < 1)
			{
				return array;
			}
			array = new short[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadInt16();
			}
			return array;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006C08 File Offset: 0x00004E08
		[return: Nullable(2)]
		public static int[] ReadInt32Array(this BinaryReader reader)
		{
			int[] array;
			int num = reader.ReadArrayLength(out array);
			if (num < 1)
			{
				return array;
			}
			array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadInt32();
			}
			return array;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00006C4C File Offset: 0x00004E4C
		[return: Nullable(2)]
		public static string[] ReadStringArray(this BinaryReader reader)
		{
			string[] array;
			int num = reader.ReadArrayLength(out array);
			if (num < 1)
			{
				return array;
			}
			array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadNullableString();
			}
			return array;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00006C94 File Offset: 0x00004E94
		public static void WriteFloatArray(this BinaryWriter writer, [Nullable(2)] float[] array)
		{
			if (writer.WriteArrayLength(array))
			{
				foreach (float num in array)
				{
					writer.Write(num);
				}
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00006CD0 File Offset: 0x00004ED0
		public static void WriteInt16Array(this BinaryWriter writer, [Nullable(2)] short[] array)
		{
			if (writer.WriteArrayLength(array))
			{
				foreach (short num in array)
				{
					writer.Write(num);
				}
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00006D0C File Offset: 0x00004F0C
		public static void WriteInt32Array(this BinaryWriter writer, [Nullable(2)] int[] array)
		{
			if (writer.WriteArrayLength(array))
			{
				foreach (int num in array)
				{
					writer.Write(num);
				}
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006D48 File Offset: 0x00004F48
		public static void WriteStringArray(this BinaryWriter writer, [Nullable(2)] string[] array)
		{
			if (writer.WriteArrayLength(array))
			{
				foreach (string text in array)
				{
					writer.WriteString(text);
				}
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006D88 File Offset: 0x00004F88
		public static int ReadArrayLength<[Nullable(2)] T>(this BinaryReader reader, [Nullable(new byte[] { 2, 1 })] out T[] array)
		{
			int num = reader.ReadInt32();
			array = ((num == 0) ? Array.Empty<T>() : null);
			return num;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006DB4 File Offset: 0x00004FB4
		public static bool WriteArrayLength<[Nullable(2)] T>(this BinaryWriter writer, [Nullable(new byte[] { 2, 1 })] [BinarySerializerExtensions.NotNullWhenAttribute(true)] T[] array)
		{
			if (array == null)
			{
				writer.Write(-1);
				return false;
			}
			if (array.Length == 0)
			{
				writer.Write(0);
				return false;
			}
			writer.Write(array.Length);
			return true;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006DE0 File Offset: 0x00004FE0
		[return: Nullable(2)]
		private static string ReadNullableString(this BinaryReader reader)
		{
			if (reader.ReadBoolean())
			{
				return reader.ReadString();
			}
			return null;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006DF8 File Offset: 0x00004FF8
		private static void WriteString(this BinaryWriter writer, [Nullable(2)] string item)
		{
			if (item == null)
			{
				writer.Write(false);
				return;
			}
			writer.Write(true);
			writer.Write(item);
		}

		// Token: 0x04000084 RID: 132
		public const int NullArrayLength = -1;

		// Token: 0x020001B6 RID: 438
		[NullableContext(0)]
		internal sealed class NotNullWhenAttribute : Attribute
		{
			// Token: 0x06001538 RID: 5432 RVA: 0x00075670 File Offset: 0x00073870
			public NotNullWhenAttribute(bool _)
			{
			}
		}
	}
}
