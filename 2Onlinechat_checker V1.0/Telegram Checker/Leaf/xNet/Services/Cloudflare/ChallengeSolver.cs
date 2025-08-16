using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Leaf.xNet.Services.Cloudflare
{
	// Token: 0x0200007A RID: 122
	[ComVisible(true)]
	public static class ChallengeSolver
	{
		// Token: 0x0600063A RID: 1594 RVA: 0x00022EFC File Offset: 0x000210FC
		public static ChallengeSolution Solve(string challengePageContent, string targetHost, int targetPort)
		{
			bool flag;
			double num = ChallengeSolver.DecodeSecretNumber(challengePageContent, targetHost, targetPort, out flag);
			string value = Regex.Match(challengePageContent, "name=\"jschl_vc\" value=\"(?<jschl_vc>[^\"]+)").Groups["jschl_vc"].Value;
			string value2 = Regex.Match(challengePageContent, "name=\"pass\" value=\"(?<pass>[^\"]+)").Groups["pass"].Value;
			string value3 = Regex.Match(challengePageContent, "id=\"challenge-form\" action=\"(?<action>[^\"]+)").Groups["action"].Value;
			string value4 = Regex.Match(challengePageContent, "name=\"s\" value=\"(?<s>[^\"]+)").Groups["s"].Value;
			return new ChallengeSolution(value3, value, value2, num, value4, flag);
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00022FA4 File Offset: 0x000211A4
		private static double DecodeSecretNumber(string challengePageContent, string targetHost, int targetPort, out bool containsIntegerTag)
		{
			string text = (from Match m in Regex.Matches(challengePageContent, "<script\\b[^>]*>(?<Content>.*?)<\\/script>", RegexOptions.Singleline)
				select m.Groups["Content"].Value).First((string c) => c.Contains("jschl-answer"));
			List<Tuple<string, double>> list = (from g in text.Split(new char[] { ';' }).Select(new Func<string, IEnumerable<Tuple<string, double>>>(ChallengeSolver.GetSteps))
				where g.Any<Tuple<string, double>>()
				select g).ToList<IEnumerable<Tuple<string, double>>>().Select(new Func<IEnumerable<Tuple<string, double>>, Tuple<string, double>>(ChallengeSolver.ResolveStepGroup)).ToList<Tuple<string, double>>();
			double item = list.First<Tuple<string, double>>().Item2;
			double num = Math.Round(list.Skip(1).Aggregate(item, new Func<double, Tuple<string, double>, double>(ChallengeSolver.ApplyDecodingStep)), 10) + (double)targetHost.Length;
			if (targetPort != 80 && targetPort != 443)
			{
				num += (double)(targetPort.ToString().Length + 1);
			}
			containsIntegerTag = text.Contains("parseInt(");
			if (!containsIntegerTag)
			{
				return num;
			}
			return (double)((int)num);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x000230EC File Offset: 0x000212EC
		private static Tuple<string, double> ResolveStepGroup(IEnumerable<Tuple<string, double>> group)
		{
			List<Tuple<string, double>> list = group.ToList<Tuple<string, double>>();
			string item = list.First<Tuple<string, double>>().Item1;
			double item2 = list.First<Tuple<string, double>>().Item2;
			double num = list.Skip(1).Aggregate(item2, new Func<double, Tuple<string, double>, double>(ChallengeSolver.ApplyDecodingStep));
			return Tuple.Create<string, double>(item, num);
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0002313C File Offset: 0x0002133C
		private static IEnumerable<Tuple<string, double>> GetSteps(string text)
		{
			return (from Match s in Regex.Matches(text, "((?<Operator>[\\+|\\-|\\*|\\/])\\=?)??(?<Number>\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?)")
				select Tuple.Create<string, double>(s.Groups["Operator"].Value, ChallengeSolver.DeobfuscateNumber(s.Groups["Number"].Value))).ToList<Tuple<string, double>>();
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x0002317C File Offset: 0x0002137C
		private static double DeobfuscateNumber(string obfuscatedNumber)
		{
			IEnumerable<int> enumerable = from Capture c in Regex.Match(obfuscatedNumber, "\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?").Groups["Digits"].Captures
				select Regex.Matches(c.Value, "\\!\\+\\[\\]|\\!\\!\\[\\]").Count;
			return double.Parse(string.Join<int>(string.Empty, enumerable));
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x000231EC File Offset: 0x000213EC
		private static double ApplyDecodingStep(double number, Tuple<string, double> step)
		{
			string item = step.Item1;
			double item2 = step.Item2;
			if (item != null)
			{
				if (item == "+")
				{
					return number + item2;
				}
				if (item == "-")
				{
					return number - item2;
				}
				if (item == "*")
				{
					return number * item2;
				}
				if (item == "/")
				{
					return number / item2;
				}
			}
			throw new ArgumentOutOfRangeException("Unknown operator: " + item);
		}

		// Token: 0x040002BC RID: 700
		private const string IntegerSolutionTag = "parseInt(";

		// Token: 0x040002BD RID: 701
		private const string ScriptPattern = "<script\\b[^>]*>(?<Content>.*?)<\\/script>";

		// Token: 0x040002BE RID: 702
		private const string ZeroPattern = "\\[\\]";

		// Token: 0x040002BF RID: 703
		private const string OnePattern = "\\!\\+\\[\\]|\\!\\!\\[\\]";

		// Token: 0x040002C0 RID: 704
		private const string DigitPattern = "\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?";

		// Token: 0x040002C1 RID: 705
		private const string NumberPattern = "\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?";

		// Token: 0x040002C2 RID: 706
		private const string OperatorPattern = "(?<Operator>[\\+|\\-|\\*|\\/])\\=?";

		// Token: 0x040002C3 RID: 707
		private const string StepPattern = "((?<Operator>[\\+|\\-|\\*|\\/])\\=?)??(?<Number>\\+?\\(?(?<Digits>\\+?\\(?(\\+?(\\!\\+\\[\\]|\\!\\!\\[\\]|\\[\\]))+\\)?)+\\)?)";
	}
}
