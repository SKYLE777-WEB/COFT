namespace ProxyBsquad
{
	// Token: 0x02000003 RID: 3
	public partial class auth : global::System.Windows.Forms.Form
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00005E70 File Offset: 0x00004070
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00005E98 File Offset: 0x00004098
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::ProxyBsquad.auth));
			this.key = new global::System.Windows.Forms.TextBox();
			this.label1 = new global::System.Windows.Forms.Label();
			this.button1 = new global::System.Windows.Forms.Button();
			this.label2 = new global::System.Windows.Forms.Label();
			this.label3 = new global::System.Windows.Forms.Label();
			base.SuspendLayout();
			this.key.BackColor = global::System.Drawing.SystemColors.HighlightText;
			this.key.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.key.Font = new global::System.Drawing.Font("Microsoft Tai Le", 9.75f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.key.ForeColor = global::System.Drawing.SystemColors.InfoText;
			this.key.Location = new global::System.Drawing.Point(2, 1);
			this.key.Name = "key";
			this.key.Size = new global::System.Drawing.Size(200, 24);
			this.key.TabIndex = 0;
			this.label1.BackColor = global::System.Drawing.Color.GhostWhite;
			this.label1.Cursor = global::System.Windows.Forms.Cursors.Hand;
			this.label1.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
			this.label1.Font = new global::System.Drawing.Font("Microsoft Tai Le", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.label1.ForeColor = global::System.Drawing.Color.Black;
			this.label1.Location = new global::System.Drawing.Point(1, 61);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(105, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "@dark_c0de";
			this.label1.TextAlign = global::System.Drawing.ContentAlignment.MiddleCenter;
			this.label1.Click += new global::System.EventHandler(this.label1_Click);
			this.button1.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
			this.button1.ForeColor = global::System.Drawing.SystemColors.MenuText;
			this.button1.Location = new global::System.Drawing.Point(46, 31);
			this.button1.Name = "button1";
			this.button1.Size = new global::System.Drawing.Size(111, 27);
			this.button1.TabIndex = 4;
			this.button1.Text = "Auth";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new global::System.EventHandler(this.button1_Click);
			this.label2.BackColor = global::System.Drawing.Color.GhostWhite;
			this.label2.Cursor = global::System.Windows.Forms.Cursors.Hand;
			this.label2.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
			this.label2.Font = new global::System.Drawing.Font("Microsoft Tai Le", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.label2.ForeColor = global::System.Drawing.SystemColors.Highlight;
			this.label2.Location = new global::System.Drawing.Point(109, 61);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(93, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "asocks.com";
			this.label2.TextAlign = global::System.Drawing.ContentAlignment.MiddleCenter;
			this.label2.Click += new global::System.EventHandler(this.label2_Click);
			this.label3.AutoSize = true;
			this.label3.Font = new global::System.Drawing.Font("Microsoft Tai Le", 9f);
			this.label3.Location = new global::System.Drawing.Point(97, 59);
			this.label3.Name = "label3";
			this.label3.Size = new global::System.Drawing.Size(10, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "|";
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = global::System.Drawing.SystemColors.Window;
			this.BackgroundImageLayout = global::System.Windows.Forms.ImageLayout.Zoom;
			base.ClientSize = new global::System.Drawing.Size(203, 77);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.key);
			this.DoubleBuffered = true;
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "auth";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Loader";
			base.TopMost = true;
			base.Load += new global::System.EventHandler(this.auth_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x0400005B RID: 91
		private global::System.ComponentModel.IContainer components;

		// Token: 0x0400005C RID: 92
		private global::System.Windows.Forms.TextBox key;

		// Token: 0x0400005D RID: 93
		private global::System.Windows.Forms.Label label1;

		// Token: 0x0400005E RID: 94
		private global::System.Windows.Forms.Button button1;

		// Token: 0x0400005F RID: 95
		private global::System.Windows.Forms.Label label2;

		// Token: 0x04000060 RID: 96
		private global::System.Windows.Forms.Label label3;
	}
}
