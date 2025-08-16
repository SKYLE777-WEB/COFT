using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D3 RID: 211
	internal struct DateTimeParser
	{
		// Token: 0x06000BB9 RID: 3001 RVA: 0x0004AB10 File Offset: 0x00048D10
		public bool Parse(char[] text, int startIndex, int length)
		{
			this._text = text;
			this._end = startIndex + length;
			return this.ParseDate(startIndex) && this.ParseChar(DateTimeParser.Lzyyyy_MM_dd + startIndex, 'T') && this.ParseTimeAndZoneAndWhitespace(DateTimeParser.Lzyyyy_MM_ddT + startIndex);
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x0004AB68 File Offset: 0x00048D68
		private bool ParseDate(int start)
		{
			return this.Parse4Digit(start, out this.Year) && 1 <= this.Year && this.ParseChar(start + DateTimeParser.Lzyyyy, '-') && this.Parse2Digit(start + DateTimeParser.Lzyyyy_, out this.Month) && 1 <= this.Month && this.Month <= 12 && this.ParseChar(start + DateTimeParser.Lzyyyy_MM, '-') && this.Parse2Digit(start + DateTimeParser.Lzyyyy_MM_, out this.Day) && 1 <= this.Day && this.Day <= DateTime.DaysInMonth(this.Year, this.Month);
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x0004AC34 File Offset: 0x00048E34
		private bool ParseTimeAndZoneAndWhitespace(int start)
		{
			return this.ParseTime(ref start) && this.ParseZone(start);
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x0004AC4C File Offset: 0x00048E4C
		private bool ParseTime(ref int start)
		{
			if (!this.Parse2Digit(start, out this.Hour) || this.Hour > 24 || !this.ParseChar(start + DateTimeParser.LzHH, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_, out this.Minute) || this.Minute >= 60 || !this.ParseChar(start + DateTimeParser.LzHH_mm, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_mm_, out this.Second) || this.Second >= 60 || (this.Hour == 24 && (this.Minute != 0 || this.Second != 0)))
			{
				return false;
			}
			start += DateTimeParser.LzHH_mm_ss;
			if (this.ParseChar(start, '.'))
			{
				this.Fraction = 0;
				int num = 0;
				for (;;)
				{
					int num2 = start + 1;
					start = num2;
					if (num2 >= this._end || num >= 7)
					{
						break;
					}
					int num3 = (int)(this._text[start] - '0');
					if (num3 < 0 || num3 > 9)
					{
						break;
					}
					this.Fraction = this.Fraction * 10 + num3;
					num++;
				}
				if (num < 7)
				{
					if (num == 0)
					{
						return false;
					}
					this.Fraction *= DateTimeParser.Power10[7 - num];
				}
				if (this.Hour == 24 && this.Fraction != 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x0004ADC8 File Offset: 0x00048FC8
		private bool ParseZone(int start)
		{
			if (start < this._end)
			{
				char c = this._text[start];
				if (c == 'Z' || c == 'z')
				{
					this.Zone = ParserTimeZone.Utc;
					start++;
				}
				else
				{
					if (start + 2 < this._end && this.Parse2Digit(start + DateTimeParser.Lz_, out this.ZoneHour) && this.ZoneHour <= 99)
					{
						if (c != '+')
						{
							if (c == '-')
							{
								this.Zone = ParserTimeZone.LocalWestOfUtc;
								start += DateTimeParser.Lz_zz;
							}
						}
						else
						{
							this.Zone = ParserTimeZone.LocalEastOfUtc;
							start += DateTimeParser.Lz_zz;
						}
					}
					if (start < this._end)
					{
						if (this.ParseChar(start, ':'))
						{
							start++;
							if (start + 1 < this._end && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
							{
								start += 2;
							}
						}
						else if (start + 1 < this._end && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
						{
							start += 2;
						}
					}
				}
			}
			return start == this._end;
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x0004AF08 File Offset: 0x00049108
		private bool Parse4Digit(int start, out int num)
		{
			if (start + 3 < this._end)
			{
				int num2 = (int)(this._text[start] - '0');
				int num3 = (int)(this._text[start + 1] - '0');
				int num4 = (int)(this._text[start + 2] - '0');
				int num5 = (int)(this._text[start + 3] - '0');
				if (0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10 && 0 <= num4 && num4 < 10 && 0 <= num5 && num5 < 10)
				{
					num = ((num2 * 10 + num3) * 10 + num4) * 10 + num5;
					return true;
				}
			}
			num = 0;
			return false;
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x0004AFB4 File Offset: 0x000491B4
		private bool Parse2Digit(int start, out int num)
		{
			if (start + 1 < this._end)
			{
				int num2 = (int)(this._text[start] - '0');
				int num3 = (int)(this._text[start + 1] - '0');
				if (0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10)
				{
					num = num2 * 10 + num3;
					return true;
				}
			}
			num = 0;
			return false;
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x0004B01C File Offset: 0x0004921C
		private bool ParseChar(int start, char ch)
		{
			return start < this._end && this._text[start] == ch;
		}

		// Token: 0x040004C1 RID: 1217
		public int Year;

		// Token: 0x040004C2 RID: 1218
		public int Month;

		// Token: 0x040004C3 RID: 1219
		public int Day;

		// Token: 0x040004C4 RID: 1220
		public int Hour;

		// Token: 0x040004C5 RID: 1221
		public int Minute;

		// Token: 0x040004C6 RID: 1222
		public int Second;

		// Token: 0x040004C7 RID: 1223
		public int Fraction;

		// Token: 0x040004C8 RID: 1224
		public int ZoneHour;

		// Token: 0x040004C9 RID: 1225
		public int ZoneMinute;

		// Token: 0x040004CA RID: 1226
		public ParserTimeZone Zone;

		// Token: 0x040004CB RID: 1227
		private char[] _text;

		// Token: 0x040004CC RID: 1228
		private int _end;

		// Token: 0x040004CD RID: 1229
		private static readonly int[] Power10 = new int[] { -1, 10, 100, 1000, 10000, 100000, 1000000 };

		// Token: 0x040004CE RID: 1230
		private static readonly int Lzyyyy = "yyyy".Length;

		// Token: 0x040004CF RID: 1231
		private static readonly int Lzyyyy_ = "yyyy-".Length;

		// Token: 0x040004D0 RID: 1232
		private static readonly int Lzyyyy_MM = "yyyy-MM".Length;

		// Token: 0x040004D1 RID: 1233
		private static readonly int Lzyyyy_MM_ = "yyyy-MM-".Length;

		// Token: 0x040004D2 RID: 1234
		private static readonly int Lzyyyy_MM_dd = "yyyy-MM-dd".Length;

		// Token: 0x040004D3 RID: 1235
		private static readonly int Lzyyyy_MM_ddT = "yyyy-MM-ddT".Length;

		// Token: 0x040004D4 RID: 1236
		private static readonly int LzHH = "HH".Length;

		// Token: 0x040004D5 RID: 1237
		private static readonly int LzHH_ = "HH:".Length;

		// Token: 0x040004D6 RID: 1238
		private static readonly int LzHH_mm = "HH:mm".Length;

		// Token: 0x040004D7 RID: 1239
		private static readonly int LzHH_mm_ = "HH:mm:".Length;

		// Token: 0x040004D8 RID: 1240
		private static readonly int LzHH_mm_ss = "HH:mm:ss".Length;

		// Token: 0x040004D9 RID: 1241
		private static readonly int Lz_ = "-".Length;

		// Token: 0x040004DA RID: 1242
		private static readonly int Lz_zz = "-zz".Length;

		// Token: 0x040004DB RID: 1243
		private const short MaxFractionDigits = 7;
	}
}
