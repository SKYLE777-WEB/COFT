using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000D4 RID: 212
	internal static class DateTimeUtils
	{
		// Token: 0x06000BC2 RID: 3010 RVA: 0x0004B078 File Offset: 0x00049278
		public static TimeSpan GetUtcOffset(this DateTime d)
		{
			return TimeZoneInfo.Local.GetUtcOffset(d);
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x0004B088 File Offset: 0x00049288
		public static XmlDateTimeSerializationMode ToSerializationMode(DateTimeKind kind)
		{
			switch (kind)
			{
			case DateTimeKind.Unspecified:
				return XmlDateTimeSerializationMode.Unspecified;
			case DateTimeKind.Utc:
				return XmlDateTimeSerializationMode.Utc;
			case DateTimeKind.Local:
				return XmlDateTimeSerializationMode.Local;
			default:
				throw MiscellaneousUtils.CreateArgumentOutOfRangeException("kind", kind, "Unexpected DateTimeKind value.");
			}
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0004B0BC File Offset: 0x000492BC
		internal static DateTime EnsureDateTime(DateTime value, DateTimeZoneHandling timeZone)
		{
			switch (timeZone)
			{
			case DateTimeZoneHandling.Local:
				value = DateTimeUtils.SwitchToLocalTime(value);
				break;
			case DateTimeZoneHandling.Utc:
				value = DateTimeUtils.SwitchToUtcTime(value);
				break;
			case DateTimeZoneHandling.Unspecified:
				value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
				break;
			case DateTimeZoneHandling.RoundtripKind:
				break;
			default:
				throw new ArgumentException("Invalid date time handling value.");
			}
			return value;
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0004B124 File Offset: 0x00049324
		private static DateTime SwitchToLocalTime(DateTime value)
		{
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
				return new DateTime(value.Ticks, DateTimeKind.Local);
			case DateTimeKind.Utc:
				return value.ToLocalTime();
			case DateTimeKind.Local:
				return value;
			default:
				return value;
			}
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x0004B170 File Offset: 0x00049370
		private static DateTime SwitchToUtcTime(DateTime value)
		{
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
				return new DateTime(value.Ticks, DateTimeKind.Utc);
			case DateTimeKind.Utc:
				return value;
			case DateTimeKind.Local:
				return value.ToUniversalTime();
			default:
				return value;
			}
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x0004B1BC File Offset: 0x000493BC
		private static long ToUniversalTicks(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			return DateTimeUtils.ToUniversalTicks(dateTime, dateTime.GetUtcOffset());
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x0004B1E0 File Offset: 0x000493E0
		private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
		{
			if (dateTime.Kind == DateTimeKind.Utc || dateTime == DateTime.MaxValue || dateTime == DateTime.MinValue)
			{
				return dateTime.Ticks;
			}
			long num = dateTime.Ticks - offset.Ticks;
			if (num > 3155378975999999999L)
			{
				return 3155378975999999999L;
			}
			if (num < 0L)
			{
				return 0L;
			}
			return num;
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x0004B25C File Offset: 0x0004945C
		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
		{
			return DateTimeUtils.UniversialTicksToJavaScriptTicks(DateTimeUtils.ToUniversalTicks(dateTime, offset));
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x0004B26C File Offset: 0x0004946C
		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
		{
			return DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTime, true);
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x0004B278 File Offset: 0x00049478
		internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
		{
			return DateTimeUtils.UniversialTicksToJavaScriptTicks(convertToUtc ? DateTimeUtils.ToUniversalTicks(dateTime) : dateTime.Ticks);
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x0004B298 File Offset: 0x00049498
		private static long UniversialTicksToJavaScriptTicks(long universialTicks)
		{
			return (universialTicks - DateTimeUtils.InitialJavaScriptDateTicks) / 10000L;
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x0004B2A8 File Offset: 0x000494A8
		internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
		{
			return new DateTime(javaScriptTicks * 10000L + DateTimeUtils.InitialJavaScriptDateTicks, DateTimeKind.Utc);
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x0004B2C0 File Offset: 0x000494C0
		internal static bool TryParseDateTimeIso(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
		{
			DateTimeParser dateTimeParser = default(DateTimeParser);
			if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
			{
				dt = default(DateTime);
				return false;
			}
			DateTime dateTime = DateTimeUtils.CreateDateTime(dateTimeParser);
			switch (dateTimeParser.Zone)
			{
			case ParserTimeZone.Utc:
				dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
				break;
			case ParserTimeZone.LocalWestOfUtc:
			{
				TimeSpan timeSpan = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
				long num = dateTime.Ticks + timeSpan.Ticks;
				if (num <= DateTime.MaxValue.Ticks)
				{
					dateTime = new DateTime(num, DateTimeKind.Utc).ToLocalTime();
				}
				else
				{
					num += dateTime.GetUtcOffset().Ticks;
					if (num > DateTime.MaxValue.Ticks)
					{
						num = DateTime.MaxValue.Ticks;
					}
					dateTime = new DateTime(num, DateTimeKind.Local);
				}
				break;
			}
			case ParserTimeZone.LocalEastOfUtc:
			{
				TimeSpan timeSpan2 = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
				long num = dateTime.Ticks - timeSpan2.Ticks;
				if (num >= DateTime.MinValue.Ticks)
				{
					dateTime = new DateTime(num, DateTimeKind.Utc).ToLocalTime();
				}
				else
				{
					num += dateTime.GetUtcOffset().Ticks;
					if (num < DateTime.MinValue.Ticks)
					{
						num = DateTime.MinValue.Ticks;
					}
					dateTime = new DateTime(num, DateTimeKind.Local);
				}
				break;
			}
			}
			dt = DateTimeUtils.EnsureDateTime(dateTime, dateTimeZoneHandling);
			return true;
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x0004B47C File Offset: 0x0004967C
		internal static bool TryParseDateTimeOffsetIso(StringReference text, out DateTimeOffset dt)
		{
			DateTimeParser dateTimeParser = default(DateTimeParser);
			if (!dateTimeParser.Parse(text.Chars, text.StartIndex, text.Length))
			{
				dt = default(DateTimeOffset);
				return false;
			}
			DateTime dateTime = DateTimeUtils.CreateDateTime(dateTimeParser);
			TimeSpan utcOffset;
			switch (dateTimeParser.Zone)
			{
			case ParserTimeZone.Utc:
				utcOffset = new TimeSpan(0L);
				break;
			case ParserTimeZone.LocalWestOfUtc:
				utcOffset = new TimeSpan(-dateTimeParser.ZoneHour, -dateTimeParser.ZoneMinute, 0);
				break;
			case ParserTimeZone.LocalEastOfUtc:
				utcOffset = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
				break;
			default:
				utcOffset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
				break;
			}
			long num = dateTime.Ticks - utcOffset.Ticks;
			if (num < 0L || num > 3155378975999999999L)
			{
				dt = default(DateTimeOffset);
				return false;
			}
			dt = new DateTimeOffset(dateTime, utcOffset);
			return true;
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0004B578 File Offset: 0x00049778
		private static DateTime CreateDateTime(DateTimeParser dateTimeParser)
		{
			bool flag;
			if (dateTimeParser.Hour == 24)
			{
				flag = true;
				dateTimeParser.Hour = 0;
			}
			else
			{
				flag = false;
			}
			DateTime dateTime = new DateTime(dateTimeParser.Year, dateTimeParser.Month, dateTimeParser.Day, dateTimeParser.Hour, dateTimeParser.Minute, dateTimeParser.Second);
			dateTime = dateTime.AddTicks((long)dateTimeParser.Fraction);
			if (flag)
			{
				dateTime = dateTime.AddDays(1.0);
			}
			return dateTime;
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0004B5FC File Offset: 0x000497FC
		internal static bool TryParseDateTime(StringReference s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
		{
			if (s.Length > 0)
			{
				int startIndex = s.StartIndex;
				if (s[startIndex] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(") && s.EndsWith(")/") && DateTimeUtils.TryParseDateTimeMicrosoft(s, dateTimeZoneHandling, out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[startIndex]) && s[startIndex + 10] == 'T' && DateTimeUtils.TryParseDateTimeIso(s, dateTimeZoneHandling, out dt))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeExact(s.ToString(), dateTimeZoneHandling, dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = default(DateTime);
			return false;
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0004B6F4 File Offset: 0x000498F4
		internal static bool TryParseDateTime(string s, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
		{
			if (s.Length > 0)
			{
				if (s[0] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal) && DateTimeUtils.TryParseDateTimeMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), dateTimeZoneHandling, out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T' && DateTime.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt))
				{
					dt = DateTimeUtils.EnsureDateTime(dt, dateTimeZoneHandling);
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeExact(s, dateTimeZoneHandling, dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = default(DateTime);
			return false;
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0004B804 File Offset: 0x00049A04
		internal static bool TryParseDateTimeOffset(StringReference s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
		{
			if (s.Length > 0)
			{
				int startIndex = s.StartIndex;
				if (s[startIndex] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(") && s.EndsWith(")/") && DateTimeUtils.TryParseDateTimeOffsetMicrosoft(s, out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[startIndex]) && s[startIndex + 10] == 'T' && DateTimeUtils.TryParseDateTimeOffsetIso(s, out dt))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeOffsetExact(s.ToString(), dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = default(DateTimeOffset);
			return false;
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0004B8F4 File Offset: 0x00049AF4
		internal static bool TryParseDateTimeOffset(string s, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
		{
			if (s.Length > 0)
			{
				if (s[0] == '/')
				{
					if (s.Length >= 9 && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal) && DateTimeUtils.TryParseDateTimeOffsetMicrosoft(new StringReference(s.ToCharArray(), 0, s.Length), out dt))
					{
						return true;
					}
				}
				else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T' && DateTimeOffset.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt) && DateTimeUtils.TryParseDateTimeOffsetIso(new StringReference(s.ToCharArray(), 0, s.Length), out dt))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateTimeOffsetExact(s, dateFormatString, culture, out dt))
				{
					return true;
				}
			}
			dt = default(DateTimeOffset);
			return false;
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x0004BA04 File Offset: 0x00049C04
		private static bool TryParseMicrosoftDate(StringReference text, out long ticks, out TimeSpan offset, out DateTimeKind kind)
		{
			kind = DateTimeKind.Utc;
			int num = text.IndexOf('+', 7, text.Length - 8);
			if (num == -1)
			{
				num = text.IndexOf('-', 7, text.Length - 8);
			}
			if (num != -1)
			{
				kind = DateTimeKind.Local;
				if (!DateTimeUtils.TryReadOffset(text, num + text.StartIndex, out offset))
				{
					ticks = 0L;
					return false;
				}
			}
			else
			{
				offset = TimeSpan.Zero;
				num = text.Length - 2;
			}
			return ConvertUtils.Int64TryParse(text.Chars, 6 + text.StartIndex, num - 6, out ticks) == ParseResult.Success;
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x0004BA9C File Offset: 0x00049C9C
		private static bool TryParseDateTimeMicrosoft(StringReference text, DateTimeZoneHandling dateTimeZoneHandling, out DateTime dt)
		{
			long num;
			TimeSpan timeSpan;
			DateTimeKind dateTimeKind;
			if (!DateTimeUtils.TryParseMicrosoftDate(text, out num, out timeSpan, out dateTimeKind))
			{
				dt = default(DateTime);
				return false;
			}
			DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime(num);
			if (dateTimeKind != DateTimeKind.Unspecified)
			{
				if (dateTimeKind != DateTimeKind.Local)
				{
					dt = dateTime;
				}
				else
				{
					dt = dateTime.ToLocalTime();
				}
			}
			else
			{
				dt = DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
			}
			dt = DateTimeUtils.EnsureDateTime(dt, dateTimeZoneHandling);
			return true;
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x0004BB24 File Offset: 0x00049D24
		private static bool TryParseDateTimeExact(string text, DateTimeZoneHandling dateTimeZoneHandling, string dateFormatString, CultureInfo culture, out DateTime dt)
		{
			DateTime dateTime;
			if (DateTime.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out dateTime))
			{
				dateTime = DateTimeUtils.EnsureDateTime(dateTime, dateTimeZoneHandling);
				dt = dateTime;
				return true;
			}
			dt = default(DateTime);
			return false;
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0004BB64 File Offset: 0x00049D64
		private static bool TryParseDateTimeOffsetMicrosoft(StringReference text, out DateTimeOffset dt)
		{
			long num;
			TimeSpan timeSpan;
			DateTimeKind dateTimeKind;
			if (!DateTimeUtils.TryParseMicrosoftDate(text, out num, out timeSpan, out dateTimeKind))
			{
				dt = default(DateTime);
				return false;
			}
			dt = new DateTimeOffset(DateTimeUtils.ConvertJavaScriptTicksToDateTime(num).Add(timeSpan).Ticks, timeSpan);
			return true;
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0004BBC4 File Offset: 0x00049DC4
		private static bool TryParseDateTimeOffsetExact(string text, string dateFormatString, CultureInfo culture, out DateTimeOffset dt)
		{
			DateTimeOffset dateTimeOffset;
			if (DateTimeOffset.TryParseExact(text, dateFormatString, culture, DateTimeStyles.RoundtripKind, out dateTimeOffset))
			{
				dt = dateTimeOffset;
				return true;
			}
			dt = default(DateTimeOffset);
			return false;
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0004BBFC File Offset: 0x00049DFC
		private static bool TryReadOffset(StringReference offsetText, int startIndex, out TimeSpan offset)
		{
			bool flag = offsetText[startIndex] == '-';
			int num;
			if (ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 1, 2, out num) != ParseResult.Success)
			{
				offset = default(TimeSpan);
				return false;
			}
			int num2 = 0;
			if (offsetText.Length - startIndex > 5 && ConvertUtils.Int32TryParse(offsetText.Chars, startIndex + 3, 2, out num2) != ParseResult.Success)
			{
				offset = default(TimeSpan);
				return false;
			}
			offset = TimeSpan.FromHours((double)num) + TimeSpan.FromMinutes((double)num2);
			if (flag)
			{
				offset = offset.Negate();
			}
			return true;
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x0004BC9C File Offset: 0x00049E9C
		internal static void WriteDateTimeString(TextWriter writer, DateTime value, DateFormatHandling format, string formatString, CultureInfo culture)
		{
			if (string.IsNullOrEmpty(formatString))
			{
				char[] array = new char[64];
				int num = DateTimeUtils.WriteDateTimeString(array, 0, value, null, value.Kind, format);
				writer.Write(array, 0, num);
				return;
			}
			writer.Write(value.ToString(formatString, culture));
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0004BCF4 File Offset: 0x00049EF4
		internal static int WriteDateTimeString(char[] chars, int start, DateTime value, TimeSpan? offset, DateTimeKind kind, DateFormatHandling format)
		{
			int num2;
			if (format == DateFormatHandling.MicrosoftDateFormat)
			{
				TimeSpan timeSpan = offset ?? value.GetUtcOffset();
				long num = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(value, timeSpan);
				"\\/Date(".CopyTo(0, chars, start, 7);
				num2 = start + 7;
				string text = num.ToString(CultureInfo.InvariantCulture);
				text.CopyTo(0, chars, num2, text.Length);
				num2 += text.Length;
				if (kind != DateTimeKind.Unspecified)
				{
					if (kind == DateTimeKind.Local)
					{
						num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, timeSpan, format);
					}
				}
				else if (value != DateTime.MaxValue && value != DateTime.MinValue)
				{
					num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, timeSpan, format);
				}
				")\\/".CopyTo(0, chars, num2, 3);
				num2 += 3;
			}
			else
			{
				num2 = DateTimeUtils.WriteDefaultIsoDate(chars, start, value);
				if (kind != DateTimeKind.Utc)
				{
					if (kind == DateTimeKind.Local)
					{
						num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, offset ?? value.GetUtcOffset(), format);
					}
				}
				else
				{
					chars[num2++] = 'Z';
				}
			}
			return num2;
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x0004BE2C File Offset: 0x0004A02C
		internal static int WriteDefaultIsoDate(char[] chars, int start, DateTime dt)
		{
			int num = 19;
			int num2;
			int num3;
			int num4;
			DateTimeUtils.GetDateValues(dt, out num2, out num3, out num4);
			DateTimeUtils.CopyIntToCharArray(chars, start, num2, 4);
			chars[start + 4] = '-';
			DateTimeUtils.CopyIntToCharArray(chars, start + 5, num3, 2);
			chars[start + 7] = '-';
			DateTimeUtils.CopyIntToCharArray(chars, start + 8, num4, 2);
			chars[start + 10] = 'T';
			DateTimeUtils.CopyIntToCharArray(chars, start + 11, dt.Hour, 2);
			chars[start + 13] = ':';
			DateTimeUtils.CopyIntToCharArray(chars, start + 14, dt.Minute, 2);
			chars[start + 16] = ':';
			DateTimeUtils.CopyIntToCharArray(chars, start + 17, dt.Second, 2);
			int num5 = (int)(dt.Ticks % 10000000L);
			if (num5 != 0)
			{
				int num6 = 7;
				while (num5 % 10 == 0)
				{
					num6--;
					num5 /= 10;
				}
				chars[start + 19] = '.';
				DateTimeUtils.CopyIntToCharArray(chars, start + 20, num5, num6);
				num += num6 + 1;
			}
			return start + num;
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x0004BF1C File Offset: 0x0004A11C
		private static void CopyIntToCharArray(char[] chars, int start, int value, int digits)
		{
			while (digits-- != 0)
			{
				chars[start + digits] = (char)(value % 10 + 48);
				value /= 10;
			}
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x0004BF40 File Offset: 0x0004A140
		internal static int WriteDateTimeOffset(char[] chars, int start, TimeSpan offset, DateFormatHandling format)
		{
			chars[start++] = ((offset.Ticks >= 0L) ? '+' : '-');
			int num = Math.Abs(offset.Hours);
			DateTimeUtils.CopyIntToCharArray(chars, start, num, 2);
			start += 2;
			if (format == DateFormatHandling.IsoDateFormat)
			{
				chars[start++] = ':';
			}
			int num2 = Math.Abs(offset.Minutes);
			DateTimeUtils.CopyIntToCharArray(chars, start, num2, 2);
			start += 2;
			return start;
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x0004BFB8 File Offset: 0x0004A1B8
		internal static void WriteDateTimeOffsetString(TextWriter writer, DateTimeOffset value, DateFormatHandling format, string formatString, CultureInfo culture)
		{
			if (string.IsNullOrEmpty(formatString))
			{
				char[] array = new char[64];
				int num = DateTimeUtils.WriteDateTimeString(array, 0, (format == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, format);
				writer.Write(array, 0, num);
				return;
			}
			writer.Write(value.ToString(formatString, culture));
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x0004C028 File Offset: 0x0004A228
		private static void GetDateValues(DateTime td, out int year, out int month, out int day)
		{
			int i = (int)(td.Ticks / 864000000000L);
			int num = i / 146097;
			i -= num * 146097;
			int num2 = i / 36524;
			if (num2 == 4)
			{
				num2 = 3;
			}
			i -= num2 * 36524;
			int num3 = i / 1461;
			i -= num3 * 1461;
			int num4 = i / 365;
			if (num4 == 4)
			{
				num4 = 3;
			}
			year = num * 400 + num2 * 100 + num3 * 4 + num4 + 1;
			i -= num4 * 365;
			int[] array = ((num4 == 3 && (num3 != 24 || num2 == 3)) ? DateTimeUtils.DaysToMonth366 : DateTimeUtils.DaysToMonth365);
			int num5 = i >> 6;
			while (i >= array[num5])
			{
				num5++;
			}
			month = num5;
			day = i - array[num5 - 1] + 1;
		}

		// Token: 0x040004DC RID: 1244
		internal static readonly long InitialJavaScriptDateTicks = 621355968000000000L;

		// Token: 0x040004DD RID: 1245
		private const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

		// Token: 0x040004DE RID: 1246
		private const int DaysPer100Years = 36524;

		// Token: 0x040004DF RID: 1247
		private const int DaysPer400Years = 146097;

		// Token: 0x040004E0 RID: 1248
		private const int DaysPer4Years = 1461;

		// Token: 0x040004E1 RID: 1249
		private const int DaysPerYear = 365;

		// Token: 0x040004E2 RID: 1250
		private const long TicksPerDay = 864000000000L;

		// Token: 0x040004E3 RID: 1251
		private static readonly int[] DaysToMonth365 = new int[]
		{
			0, 31, 59, 90, 120, 151, 181, 212, 243, 273,
			304, 334, 365
		};

		// Token: 0x040004E4 RID: 1252
		private static readonly int[] DaysToMonth366 = new int[]
		{
			0, 31, 60, 91, 121, 152, 182, 213, 244, 274,
			305, 335, 366
		};
	}
}
