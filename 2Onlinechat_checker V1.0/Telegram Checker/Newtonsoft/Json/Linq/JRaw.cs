using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x0200014A RID: 330
	public class JRaw : JValue
	{
		// Token: 0x060011CD RID: 4557 RVA: 0x000659D4 File Offset: 0x00063BD4
		public static async Task<JRaw> CreateAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			JRaw jraw;
			using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
				{
					await jsonWriter.WriteTokenSyncReadingAsync(reader, cancellationToken).ConfigureAwait(false);
					jraw = new JRaw(sw.ToString());
				}
			}
			return jraw;
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x00065A28 File Offset: 0x00063C28
		public JRaw(JRaw other)
			: base(other)
		{
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x00065A34 File Offset: 0x00063C34
		public JRaw(object rawJson)
			: base(rawJson, JTokenType.Raw)
		{
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x00065A40 File Offset: 0x00063C40
		public static JRaw Create(JsonReader reader)
		{
			JRaw jraw;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
				{
					jsonTextWriter.WriteToken(reader);
					jraw = new JRaw(stringWriter.ToString());
				}
			}
			return jraw;
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x00065AB0 File Offset: 0x00063CB0
		internal override JToken CloneToken()
		{
			return new JRaw(this);
		}
	}
}
