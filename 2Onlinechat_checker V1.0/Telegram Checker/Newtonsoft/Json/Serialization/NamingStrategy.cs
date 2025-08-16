using System;

namespace Newtonsoft.Json.Serialization
{
	// Token: 0x02000125 RID: 293
	public abstract class NamingStrategy
	{
		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0005DDB8 File Offset: 0x0005BFB8
		// (set) Token: 0x06000F4C RID: 3916 RVA: 0x0005DDC0 File Offset: 0x0005BFC0
		public bool ProcessDictionaryKeys { get; set; }

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06000F4D RID: 3917 RVA: 0x0005DDCC File Offset: 0x0005BFCC
		// (set) Token: 0x06000F4E RID: 3918 RVA: 0x0005DDD4 File Offset: 0x0005BFD4
		public bool ProcessExtensionDataNames { get; set; }

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06000F4F RID: 3919 RVA: 0x0005DDE0 File Offset: 0x0005BFE0
		// (set) Token: 0x06000F50 RID: 3920 RVA: 0x0005DDE8 File Offset: 0x0005BFE8
		public bool OverrideSpecifiedNames { get; set; }

		// Token: 0x06000F51 RID: 3921 RVA: 0x0005DDF4 File Offset: 0x0005BFF4
		public virtual string GetPropertyName(string name, bool hasSpecifiedName)
		{
			if (hasSpecifiedName && !this.OverrideSpecifiedNames)
			{
				return name;
			}
			return this.ResolvePropertyName(name);
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0005DE10 File Offset: 0x0005C010
		public virtual string GetExtensionDataName(string name)
		{
			if (!this.ProcessExtensionDataNames)
			{
				return name;
			}
			return this.ResolvePropertyName(name);
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0005DE28 File Offset: 0x0005C028
		public virtual string GetDictionaryKey(string key)
		{
			if (!this.ProcessDictionaryKeys)
			{
				return key;
			}
			return this.ResolvePropertyName(key);
		}

		// Token: 0x06000F54 RID: 3924
		protected abstract string ResolvePropertyName(string name);
	}
}
