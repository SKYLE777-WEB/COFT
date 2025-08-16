using System;
using System.Windows.Forms;

namespace ProxyBsquad
{
	// Token: 0x02000006 RID: 6
	internal static class Program
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00006690 File Offset: 0x00004890
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
