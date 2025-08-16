using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Leaf.xNet.Services.Cloudflare
{
	// Token: 0x02000079 RID: 121
	[ComVisible(true)]
	public struct ChallengeSolution : IEquatable<ChallengeSolution>
	{
		// Token: 0x1700012C RID: 300
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x00022D48 File Offset: 0x00020F48
		public string ClearancePage { get; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x00022D50 File Offset: 0x00020F50
		public string VerificationCode { get; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x00022D58 File Offset: 0x00020F58
		public string Pass { get; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x00022D60 File Offset: 0x00020F60
		public double Answer { get; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x00022D68 File Offset: 0x00020F68
		public string S { get; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x00022D70 File Offset: 0x00020F70
		public bool ContainsIntegerTag { get; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x00022D78 File Offset: 0x00020F78
		public string ClearanceQuery
		{
			get
			{
				if (string.IsNullOrEmpty(this.S))
				{
					return string.Concat(new string[]
					{
						this.ClearancePage,
						"?jschl_vc=",
						this.VerificationCode,
						"&pass=",
						this.Pass,
						"&jschl_answer=",
						this.Answer.ToString("R", CultureInfo.InvariantCulture)
					});
				}
				return string.Concat(new string[]
				{
					this.ClearancePage,
					"?s=",
					Uri.EscapeDataString(this.S),
					"&jschl_vc=",
					this.VerificationCode,
					"&pass=",
					this.Pass,
					"&jschl_answer=",
					this.Answer.ToString("R", CultureInfo.InvariantCulture)
				});
			}
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00022E64 File Offset: 0x00021064
		public ChallengeSolution(string clearancePage, string verificationCode, string pass, double answer, string s, bool containsIntegerTag)
		{
			this.ClearancePage = clearancePage;
			this.VerificationCode = verificationCode;
			this.Pass = pass;
			this.Answer = answer;
			this.S = s;
			this.ContainsIntegerTag = containsIntegerTag;
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00022E94 File Offset: 0x00021094
		public static bool operator ==(ChallengeSolution solutionA, ChallengeSolution solutionB)
		{
			return solutionA.Equals(solutionB);
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00022EA0 File Offset: 0x000210A0
		public static bool operator !=(ChallengeSolution solutionA, ChallengeSolution solutionB)
		{
			return !(solutionA == solutionB);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00022EAC File Offset: 0x000210AC
		public override bool Equals(object obj)
		{
			if (obj is ChallengeSolution)
			{
				ChallengeSolution challengeSolution = (ChallengeSolution)obj;
				return this.Equals(challengeSolution);
			}
			return false;
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00022ED8 File Offset: 0x000210D8
		public override int GetHashCode()
		{
			return this.ClearanceQuery.GetHashCode();
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00022EE8 File Offset: 0x000210E8
		public bool Equals(ChallengeSolution other)
		{
			return other.ClearanceQuery == this.ClearanceQuery;
		}
	}
}
