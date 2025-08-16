using System;

namespace ProxyBsquad
{
	// Token: 0x02000007 RID: 7
	internal class Stata
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000066A8 File Offset: 0x000048A8
		// (set) Token: 0x0600004A RID: 74 RVA: 0x000066B0 File Offset: 0x000048B0
		public static int LoadUrls { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000066B8 File Offset: 0x000048B8
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000066C0 File Offset: 0x000048C0
		public static int LoadProxy { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000066C8 File Offset: 0x000048C8
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000066D0 File Offset: 0x000048D0
		public static int WorkChats { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000066D8 File Offset: 0x000048D8
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000066E0 File Offset: 0x000048E0
		public static int BpsLimit { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000066E8 File Offset: 0x000048E8
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000066F0 File Offset: 0x000048F0
		public static int WorkChannels { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000066F8 File Offset: 0x000048F8
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00006700 File Offset: 0x00004900
		public static int BadURLS { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00006708 File Offset: 0x00004908
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00006710 File Offset: 0x00004910
		public static int ErrorProxy { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00006718 File Offset: 0x00004918
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00006720 File Offset: 0x00004920
		public static int TgUsers { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00006728 File Offset: 0x00004928
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00006730 File Offset: 0x00004930
		public static int UnknownError { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00006738 File Offset: 0x00004938
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00006740 File Offset: 0x00004940
		public static int IncorrectLines { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00006748 File Offset: 0x00004948
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00006750 File Offset: 0x00004950
		public string Description { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005F RID: 95 RVA: 0x0000675C File Offset: 0x0000495C
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00006764 File Offset: 0x00004964
		public string Title { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00006770 File Offset: 0x00004970
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00006778 File Offset: 0x00004978
		public string Online { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00006784 File Offset: 0x00004984
		// (set) Token: 0x06000064 RID: 100 RVA: 0x0000678C File Offset: 0x0000498C
		public string No_Online { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00006798 File Offset: 0x00004998
		// (set) Token: 0x06000066 RID: 102 RVA: 0x000067A0 File Offset: 0x000049A0
		public string Member_Count { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000067AC File Offset: 0x000049AC
		// (set) Token: 0x06000068 RID: 104 RVA: 0x000067B4 File Offset: 0x000049B4
		public string Member_Count_Channel { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000067C0 File Offset: 0x000049C0
		// (set) Token: 0x0600006A RID: 106 RVA: 0x000067C8 File Offset: 0x000049C8
		public string lang_chat { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600006B RID: 107 RVA: 0x000067D4 File Offset: 0x000049D4
		// (set) Token: 0x0600006C RID: 108 RVA: 0x000067DC File Offset: 0x000049DC
		public string lang_channel { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000067E8 File Offset: 0x000049E8
		// (set) Token: 0x0600006E RID: 110 RVA: 0x000067F0 File Offset: 0x000049F0
		public string my_ip { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006F RID: 111 RVA: 0x000067FC File Offset: 0x000049FC
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00006804 File Offset: 0x00004A04
		public string testbot { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00006810 File Offset: 0x00004A10
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00006818 File Offset: 0x00004A18
		public string tbot { get; set; }

		// Token: 0x06000073 RID: 115 RVA: 0x00006824 File Offset: 0x00004A24
		public void ClearStata()
		{
			Stata.LoadUrls = 0;
			Stata.WorkChats = 0;
			Stata.WorkChannels = 0;
			Stata.BadURLS = 0;
			Stata.ErrorProxy = 0;
			Stata.TgUsers = 0;
			Stata.UnknownError = 0;
			Stata.IncorrectLines = 0;
		}
	}
}
