using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TelegramCH.Properties
{
	// Token: 0x0200000B RID: 11
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.8.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00006AC0 File Offset: 0x00004CC0
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00006AC8 File Offset: 0x00004CC8
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00006ADC File Offset: 0x00004CDC
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string lic
		{
			get
			{
				return (string)this["lic"];
			}
			set
			{
				this["lic"] = value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00006AEC File Offset: 0x00004CEC
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00006B00 File Offset: 0x00004D00
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("Asocks URL Proxy")]
		public string asocks_url
		{
			get
			{
				return (string)this["asocks_url"];
			}
			set
			{
				this["asocks_url"] = value;
			}
		}

		// Token: 0x04000080 RID: 128
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
