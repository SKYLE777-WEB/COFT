using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Leaf.xNet
{
	// Token: 0x02000057 RID: 87
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x060003DF RID: 991 RVA: 0x0001A914 File Offset: 0x00018B14
		internal Resources()
		{
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x0001A91C File Offset: 0x00018B1C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("Leaf.xNet.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x0001A94C File Offset: 0x00018B4C
		// (set) Token: 0x060003E2 RID: 994 RVA: 0x0001A954 File Offset: 0x00018B54
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x0001A95C File Offset: 0x00018B5C
		internal static string ArgumentException_CanNotReadOrSeek
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentException_CanNotReadOrSeek", Resources.resourceCulture);
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x0001A974 File Offset: 0x00018B74
		internal static string ArgumentException_EmptyString
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentException_EmptyString", Resources.resourceCulture);
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x0001A98C File Offset: 0x00018B8C
		internal static string ArgumentException_HttpRequest_SetNotAvailableHeader
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentException_HttpRequest_SetNotAvailableHeader", Resources.resourceCulture);
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x0001A9A4 File Offset: 0x00018BA4
		internal static string ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex", Resources.resourceCulture);
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x0001A9BC File Offset: 0x00018BBC
		internal static string ArgumentException_OnlyAbsoluteUri
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentException_OnlyAbsoluteUri", Resources.resourceCulture);
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060003E8 RID: 1000 RVA: 0x0001A9D4 File Offset: 0x00018BD4
		internal static string ArgumentException_WrongPath
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentException_WrongPath", Resources.resourceCulture);
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x0001A9EC File Offset: 0x00018BEC
		internal static string ArgumentOutOfRangeException_CanNotBeGreater
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentOutOfRangeException_CanNotBeGreater", Resources.resourceCulture);
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060003EA RID: 1002 RVA: 0x0001AA04 File Offset: 0x00018C04
		internal static string ArgumentOutOfRangeException_CanNotBeLess
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentOutOfRangeException_CanNotBeLess", Resources.resourceCulture);
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x0001AA1C File Offset: 0x00018C1C
		internal static string ArgumentOutOfRangeException_CanNotBeLessOrGreater
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentOutOfRangeException_CanNotBeLessOrGreater", Resources.resourceCulture);
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0001AA34 File Offset: 0x00018C34
		internal static string ArgumentOutOfRangeException_StringHelper_MoreLengthString
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentOutOfRangeException_StringHelper_MoreLengthString", Resources.resourceCulture);
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x0001AA4C File Offset: 0x00018C4C
		internal static string ArgumentOutOfRangeException_StringLengthCanNotBeMore
		{
			get
			{
				return Resources.ResourceManager.GetString("ArgumentOutOfRangeException_StringLengthCanNotBeMore", Resources.resourceCulture);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x0001AA64 File Offset: 0x00018C64
		internal static string CookieStorage_SaveToFile_FileAlreadyExists
		{
			get
			{
				return Resources.ResourceManager.GetString("CookieStorage_SaveToFile_FileAlreadyExists", Resources.resourceCulture);
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060003EF RID: 1007 RVA: 0x0001AA7C File Offset: 0x00018C7C
		internal static string DirectoryNotFoundException_DirectoryNotFound
		{
			get
			{
				return Resources.ResourceManager.GetString("DirectoryNotFoundException_DirectoryNotFound", Resources.resourceCulture);
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x0001AA94 File Offset: 0x00018C94
		internal static string FormatException_ProxyClient_WrongPort
		{
			get
			{
				return Resources.ResourceManager.GetString("FormatException_ProxyClient_WrongPort", Resources.resourceCulture);
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060003F1 RID: 1009 RVA: 0x0001AAAC File Offset: 0x00018CAC
		internal static string HttpException_ClientError
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_ClientError", Resources.resourceCulture);
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x0001AAC4 File Offset: 0x00018CC4
		internal static string HttpException_ConnectTimeout
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_ConnectTimeout", Resources.resourceCulture);
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x0001AADC File Offset: 0x00018CDC
		internal static string HttpException_Default
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_Default", Resources.resourceCulture);
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x0001AAF4 File Offset: 0x00018CF4
		internal static string HttpException_FailedConnect
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_FailedConnect", Resources.resourceCulture);
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x0001AB0C File Offset: 0x00018D0C
		internal static string HttpException_FailedGetHostAddresses
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_FailedGetHostAddresses", Resources.resourceCulture);
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x0001AB24 File Offset: 0x00018D24
		internal static string HttpException_FailedReceiveMessageBody
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_FailedReceiveMessageBody", Resources.resourceCulture);
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x0001AB3C File Offset: 0x00018D3C
		internal static string HttpException_FailedReceiveResponse
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_FailedReceiveResponse", Resources.resourceCulture);
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x0001AB54 File Offset: 0x00018D54
		internal static string HttpException_FailedSendRequest
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_FailedSendRequest", Resources.resourceCulture);
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x0001AB6C File Offset: 0x00018D6C
		internal static string HttpException_FailedSslConnect
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_FailedSslConnect", Resources.resourceCulture);
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x0001AB84 File Offset: 0x00018D84
		internal static string HttpException_LimitRedirections
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_LimitRedirections", Resources.resourceCulture);
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x0001AB9C File Offset: 0x00018D9C
		internal static string HttpException_ReceivedEmptyResponse
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_ReceivedEmptyResponse", Resources.resourceCulture);
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060003FC RID: 1020 RVA: 0x0001ABB4 File Offset: 0x00018DB4
		internal static string HttpException_ReceivedWrongResponse
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_ReceivedWrongResponse", Resources.resourceCulture);
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x0001ABCC File Offset: 0x00018DCC
		internal static string HttpException_SeverError
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_SeverError", Resources.resourceCulture);
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x0001ABE4 File Offset: 0x00018DE4
		internal static string HttpException_WaitDataTimeout
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_WaitDataTimeout", Resources.resourceCulture);
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x0001ABFC File Offset: 0x00018DFC
		internal static string HttpException_WrongChunkedBlockLength
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_WrongChunkedBlockLength", Resources.resourceCulture);
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000400 RID: 1024 RVA: 0x0001AC14 File Offset: 0x00018E14
		internal static string HttpException_WrongCookie
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_WrongCookie", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x0001AC2C File Offset: 0x00018E2C
		internal static string HttpException_WrongHeader
		{
			get
			{
				return Resources.ResourceManager.GetString("HttpException_WrongHeader", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x0001AC44 File Offset: 0x00018E44
		internal static string InvalidOperationException_HttpResponse_HasError
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidOperationException_HttpResponse_HasError", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x0001AC5C File Offset: 0x00018E5C
		internal static string InvalidOperationException_NotSupportedEncodingFormat
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidOperationException_NotSupportedEncodingFormat", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x0001AC74 File Offset: 0x00018E74
		internal static string InvalidOperationException_ProxyClient_WrongHost
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongHost", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000405 RID: 1029 RVA: 0x0001AC8C File Offset: 0x00018E8C
		internal static string InvalidOperationException_ProxyClient_WrongPassword
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongPassword", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x0001ACA4 File Offset: 0x00018EA4
		internal static string InvalidOperationException_ProxyClient_WrongPort
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongPort", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x0001ACBC File Offset: 0x00018EBC
		internal static string InvalidOperationException_ProxyClient_WrongUsername
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongUsername", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x0001ACD4 File Offset: 0x00018ED4
		internal static string NetException_Default
		{
			get
			{
				return Resources.ResourceManager.GetString("NetException_Default", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x0001ACEC File Offset: 0x00018EEC
		internal static string ProxyException_CommandError
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_CommandError", Resources.resourceCulture);
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x0001AD04 File Offset: 0x00018F04
		internal static string ProxyException_ConnectTimeout
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_ConnectTimeout", Resources.resourceCulture);
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x0001AD1C File Offset: 0x00018F1C
		internal static string ProxyException_Default
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_Default", Resources.resourceCulture);
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x0001AD34 File Offset: 0x00018F34
		internal static string ProxyException_Error
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_Error", Resources.resourceCulture);
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x0001AD4C File Offset: 0x00018F4C
		internal static string ProxyException_FailedConnect
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_FailedConnect", Resources.resourceCulture);
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x0001AD64 File Offset: 0x00018F64
		internal static string ProxyException_FailedGetHostAddresses
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_FailedGetHostAddresses", Resources.resourceCulture);
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x0001AD7C File Offset: 0x00018F7C
		internal static string ProxyException_NotSupportedAddressType
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_NotSupportedAddressType", Resources.resourceCulture);
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x0001AD94 File Offset: 0x00018F94
		internal static string ProxyException_ReceivedEmptyResponse
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_ReceivedEmptyResponse", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x0001ADAC File Offset: 0x00018FAC
		internal static string ProxyException_ReceivedWrongResponse
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_ReceivedWrongResponse", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x0001ADC4 File Offset: 0x00018FC4
		internal static string ProxyException_ReceivedWrongStatusCode
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_ReceivedWrongStatusCode", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x0001ADDC File Offset: 0x00018FDC
		internal static string ProxyException_Socks5_FailedAuthOn
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_Socks5_FailedAuthOn", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x0001ADF4 File Offset: 0x00018FF4
		internal static string ProxyException_WaitDataTimeout
		{
			get
			{
				return Resources.ResourceManager.GetString("ProxyException_WaitDataTimeout", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x0001AE0C File Offset: 0x0001900C
		internal static string Socks_UnknownError
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks_UnknownError", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x0001AE24 File Offset: 0x00019024
		internal static string Socks4_CommandReplyRequestRejectedCannotConnectToIdentd
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks4_CommandReplyRequestRejectedCannotConnectToIdentd", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x0001AE3C File Offset: 0x0001903C
		internal static string Socks4_CommandReplyRequestRejectedDifferentIdentd
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks4_CommandReplyRequestRejectedDifferentIdentd", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x0001AE54 File Offset: 0x00019054
		internal static string Socks4_CommandReplyRequestRejectedOrFailed
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks4_CommandReplyRequestRejectedOrFailed", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x0001AE6C File Offset: 0x0001906C
		internal static string Socks5_AuthMethodReplyNoAcceptableMethods
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_AuthMethodReplyNoAcceptableMethods", Resources.resourceCulture);
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x0001AE84 File Offset: 0x00019084
		internal static string Socks5_CommandReplyAddressTypeNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyAddressTypeNotSupported", Resources.resourceCulture);
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x0001AE9C File Offset: 0x0001909C
		internal static string Socks5_CommandReplyCommandNotSupported
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyCommandNotSupported", Resources.resourceCulture);
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x0001AEB4 File Offset: 0x000190B4
		internal static string Socks5_CommandReplyConnectionNotAllowedByRuleset
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyConnectionNotAllowedByRuleset", Resources.resourceCulture);
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x0001AECC File Offset: 0x000190CC
		internal static string Socks5_CommandReplyConnectionRefused
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyConnectionRefused", Resources.resourceCulture);
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600041E RID: 1054 RVA: 0x0001AEE4 File Offset: 0x000190E4
		internal static string Socks5_CommandReplyGeneralSocksServerFailure
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyGeneralSocksServerFailure", Resources.resourceCulture);
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x0001AEFC File Offset: 0x000190FC
		internal static string Socks5_CommandReplyHostUnreachable
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyHostUnreachable", Resources.resourceCulture);
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x0001AF14 File Offset: 0x00019114
		internal static string Socks5_CommandReplyNetworkUnreachable
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyNetworkUnreachable", Resources.resourceCulture);
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x0001AF2C File Offset: 0x0001912C
		internal static string Socks5_CommandReplyTTLExpired
		{
			get
			{
				return Resources.ResourceManager.GetString("Socks5_CommandReplyTTLExpired", Resources.resourceCulture);
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x0001AF44 File Offset: 0x00019144
		internal static string StringExtensions_Substrings_Invalid_Index
		{
			get
			{
				return Resources.ResourceManager.GetString("StringExtensions_Substrings_Invalid_Index", Resources.resourceCulture);
			}
		}

		// Token: 0x04000192 RID: 402
		private static ResourceManager resourceMan;

		// Token: 0x04000193 RID: 403
		private static CultureInfo resourceCulture;
	}
}
