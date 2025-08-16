using System;
using System.ComponentModel;

namespace Newtonsoft.Json.Linq
{
	// Token: 0x02000148 RID: 328
	public class JPropertyDescriptor : PropertyDescriptor
	{
		// Token: 0x060011AE RID: 4526 RVA: 0x00065530 File Offset: 0x00063730
		public JPropertyDescriptor(string name)
			: base(name, null)
		{
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x0006553C File Offset: 0x0006373C
		private static JObject CastInstance(object instance)
		{
			return (JObject)instance;
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00065544 File Offset: 0x00063744
		public override bool CanResetValue(object component)
		{
			return false;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00065548 File Offset: 0x00063748
		public override object GetValue(object component)
		{
			return JPropertyDescriptor.CastInstance(component)[this.Name];
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x0006555C File Offset: 0x0006375C
		public override void ResetValue(object component)
		{
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00065560 File Offset: 0x00063760
		public override void SetValue(object component, object value)
		{
			JToken jtoken = (value as JToken) ?? new JValue(value);
			JPropertyDescriptor.CastInstance(component)[this.Name] = jtoken;
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00065598 File Offset: 0x00063798
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x060011B5 RID: 4533 RVA: 0x0006559C File Offset: 0x0006379C
		public override Type ComponentType
		{
			get
			{
				return typeof(JObject);
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x060011B6 RID: 4534 RVA: 0x000655A8 File Offset: 0x000637A8
		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x060011B7 RID: 4535 RVA: 0x000655AC File Offset: 0x000637AC
		public override Type PropertyType
		{
			get
			{
				return typeof(object);
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x060011B8 RID: 4536 RVA: 0x000655B8 File Offset: 0x000637B8
		protected override int NameHashCode
		{
			get
			{
				return base.NameHashCode;
			}
		}
	}
}
