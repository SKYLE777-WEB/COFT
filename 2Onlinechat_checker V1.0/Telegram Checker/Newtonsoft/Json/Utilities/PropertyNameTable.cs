using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000ED RID: 237
	internal class PropertyNameTable
	{
		// Token: 0x06000CB6 RID: 3254 RVA: 0x0005063C File Offset: 0x0004E83C
		public PropertyNameTable()
		{
			this._entries = new PropertyNameTable.Entry[this._mask + 1];
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x00050660 File Offset: 0x0004E860
		public string Get(char[] key, int start, int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			int num = length + PropertyNameTable.HashCodeRandomizer;
			num += (num << 7) ^ (int)key[start];
			int num2 = start + length;
			for (int i = start + 1; i < num2; i++)
			{
				num += (num << 7) ^ (int)key[i];
			}
			num -= num >> 17;
			num -= num >> 11;
			num -= num >> 5;
			for (PropertyNameTable.Entry entry = this._entries[num & this._mask]; entry != null; entry = entry.Next)
			{
				if (entry.HashCode == num && PropertyNameTable.TextEquals(entry.Value, key, start, length))
				{
					return entry.Value;
				}
			}
			return null;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x0005070C File Offset: 0x0004E90C
		public string Add(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int length = key.Length;
			if (length == 0)
			{
				return string.Empty;
			}
			int num = length + PropertyNameTable.HashCodeRandomizer;
			for (int i = 0; i < key.Length; i++)
			{
				num += (num << 7) ^ (int)key[i];
			}
			num -= num >> 17;
			num -= num >> 11;
			num -= num >> 5;
			for (PropertyNameTable.Entry entry = this._entries[num & this._mask]; entry != null; entry = entry.Next)
			{
				if (entry.HashCode == num && entry.Value.Equals(key))
				{
					return entry.Value;
				}
			}
			return this.AddEntry(key, num);
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x000507D0 File Offset: 0x0004E9D0
		private string AddEntry(string str, int hashCode)
		{
			int num = hashCode & this._mask;
			PropertyNameTable.Entry entry = new PropertyNameTable.Entry(str, hashCode, this._entries[num]);
			this._entries[num] = entry;
			int count = this._count;
			this._count = count + 1;
			if (count == this._mask)
			{
				this.Grow();
			}
			return entry.Value;
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x00050834 File Offset: 0x0004EA34
		private void Grow()
		{
			PropertyNameTable.Entry[] entries = this._entries;
			int num = this._mask * 2 + 1;
			PropertyNameTable.Entry[] array = new PropertyNameTable.Entry[num + 1];
			foreach (PropertyNameTable.Entry entry in entries)
			{
				while (entry != null)
				{
					int num2 = entry.HashCode & num;
					PropertyNameTable.Entry next = entry.Next;
					entry.Next = array[num2];
					array[num2] = entry;
					entry = next;
				}
			}
			this._entries = array;
			this._mask = num;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x000508C4 File Offset: 0x0004EAC4
		private static bool TextEquals(string str1, char[] str2, int str2Start, int str2Length)
		{
			if (str1.Length != str2Length)
			{
				return false;
			}
			for (int i = 0; i < str1.Length; i++)
			{
				if (str1[i] != str2[str2Start + i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000522 RID: 1314
		private static readonly int HashCodeRandomizer = Environment.TickCount;

		// Token: 0x04000523 RID: 1315
		private int _count;

		// Token: 0x04000524 RID: 1316
		private PropertyNameTable.Entry[] _entries;

		// Token: 0x04000525 RID: 1317
		private int _mask = 31;

		// Token: 0x02000262 RID: 610
		private class Entry
		{
			// Token: 0x060016FE RID: 5886 RVA: 0x00083538 File Offset: 0x00081738
			internal Entry(string value, int hashCode, PropertyNameTable.Entry next)
			{
				this.Value = value;
				this.HashCode = hashCode;
				this.Next = next;
			}

			// Token: 0x04000B18 RID: 2840
			internal readonly string Value;

			// Token: 0x04000B19 RID: 2841
			internal readonly int HashCode;

			// Token: 0x04000B1A RID: 2842
			internal PropertyNameTable.Entry Next;
		}
	}
}
