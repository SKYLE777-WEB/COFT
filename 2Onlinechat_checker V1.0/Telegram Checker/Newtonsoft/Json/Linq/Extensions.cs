using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000140 RID: 320
	public static class Extensions
	{
		// Token: 0x06001098 RID: 4248 RVA: 0x00062560 File Offset: 0x00060760
		public static IJEnumerable<JToken> Ancestors<T>(this IEnumerable<T> source) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany((T j) => j.Ancestors()).AsJEnumerable();
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x0006259C File Offset: 0x0006079C
		public static IJEnumerable<JToken> AncestorsAndSelf<T>(this IEnumerable<T> source) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany((T j) => j.AncestorsAndSelf()).AsJEnumerable();
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x000625D8 File Offset: 0x000607D8
		public static IJEnumerable<JToken> Descendants<T>(this IEnumerable<T> source) where T : JContainer
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany((T j) => j.Descendants()).AsJEnumerable();
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x00062614 File Offset: 0x00060814
		public static IJEnumerable<JToken> DescendantsAndSelf<T>(this IEnumerable<T> source) where T : JContainer
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany((T j) => j.DescendantsAndSelf()).AsJEnumerable();
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00062650 File Offset: 0x00060850
		public static IJEnumerable<JProperty> Properties(this IEnumerable<JObject> source)
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany((JObject d) => d.Properties()).AsJEnumerable<JProperty>();
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x0006268C File Offset: 0x0006088C
		public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source, object key)
		{
			return source.Values(key).AsJEnumerable();
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x0006269C File Offset: 0x0006089C
		public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source)
		{
			return source.Values(null);
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x000626A8 File Offset: 0x000608A8
		public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source, object key)
		{
			return source.Values(key);
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x000626B4 File Offset: 0x000608B4
		public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source)
		{
			return source.Values(null);
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x000626C0 File Offset: 0x000608C0
		public static U Value<U>(this IEnumerable<JToken> value)
		{
			return value.Value<JToken, U>();
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x000626C8 File Offset: 0x000608C8
		public static U Value<T, U>(this IEnumerable<T> value) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(value, "value");
			JToken jtoken = value as JToken;
			if (jtoken == null)
			{
				throw new ArgumentException("Source value must be a JToken.");
			}
			return jtoken.Convert<JToken, U>();
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x000626F4 File Offset: 0x000608F4
		internal static IEnumerable<U> Values<T, U>(this IEnumerable<T> source, object key) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			if (key == null)
			{
				foreach (T token in source)
				{
					JValue jvalue = token as JValue;
					if (jvalue != null)
					{
						yield return jvalue.Convert<JValue, U>();
					}
					else
					{
						foreach (JToken jtoken in token.Children())
						{
							yield return jtoken.Convert<JToken, U>();
						}
						IEnumerator<JToken> enumerator2 = null;
					}
					token = default(T);
				}
				IEnumerator<T> enumerator = null;
			}
			else
			{
				foreach (T t in source)
				{
					JToken jtoken2 = t[key];
					if (jtoken2 != null)
					{
						yield return jtoken2.Convert<JToken, U>();
					}
				}
				IEnumerator<T> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x0006270C File Offset: 0x0006090C
		public static IJEnumerable<JToken> Children<T>(this IEnumerable<T> source) where T : JToken
		{
			return source.Children<T, JToken>().AsJEnumerable();
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x0006271C File Offset: 0x0006091C
		public static IEnumerable<U> Children<T, U>(this IEnumerable<T> source) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			return source.SelectMany((T c) => c.Children()).Convert<JToken, U>();
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x00062758 File Offset: 0x00060958
		internal static IEnumerable<U> Convert<T, U>(this IEnumerable<T> source) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, "source");
			foreach (T t in source)
			{
				yield return t.Convert<JToken, U>();
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00062768 File Offset: 0x00060968
		internal static U Convert<T, U>(this T token) where T : JToken
		{
			if (token == null)
			{
				return default(U);
			}
			if (token is U && typeof(U) != typeof(IComparable) && typeof(U) != typeof(IFormattable))
			{
				return (U)((object)token);
			}
			JValue jvalue = token as JValue;
			if (jvalue == null)
			{
				throw new InvalidCastException("Cannot cast {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, token.GetType(), typeof(T)));
			}
			if (jvalue.Value is U)
			{
				return (U)((object)jvalue.Value);
			}
			Type type = typeof(U);
			if (ReflectionUtils.IsNullableType(type))
			{
				if (jvalue.Value == null)
				{
					return default(U);
				}
				type = Nullable.GetUnderlyingType(type);
			}
			return (U)((object)global::System.Convert.ChangeType(jvalue.Value, type, CultureInfo.InvariantCulture));
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00062888 File Offset: 0x00060A88
		public static IJEnumerable<JToken> AsJEnumerable(this IEnumerable<JToken> source)
		{
			return source.AsJEnumerable<JToken>();
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x00062890 File Offset: 0x00060A90
		public static IJEnumerable<T> AsJEnumerable<T>(this IEnumerable<T> source) where T : JToken
		{
			if (source == null)
			{
				return null;
			}
			if (source is IJEnumerable<T>)
			{
				return (IJEnumerable<T>)source;
			}
			return new JEnumerable<T>(source);
		}
	}
}
