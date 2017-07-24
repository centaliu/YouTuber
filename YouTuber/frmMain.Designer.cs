namespace YouTuber
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtUrls = new System.Windows.Forms.TextBox();
			this.lblProg = new System.Windows.Forms.Label();
			this.btnGo = new System.Windows.Forms.Button();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.txtList = new System.Windows.Forms.TextBox();
			this.btnGetList = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtUrls
			// 
			this.txtUrls.Location = new System.Drawing.Point(0, 0);
			this.txtUrls.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.txtUrls.Multiline = true;
			this.txtUrls.Name = "txtUrls";
			this.txtUrls.Size = new System.Drawing.Size(269, 290);
			this.txtUrls.TabIndex = 0;
			this.txtUrls.TextChanged += new System.EventHandler(this.txtUrls_TextChanged);
			this.txtUrls.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUrls_KeyDown);
			// 
			// lblProg
			// 
			this.lblProg.AutoSize = true;
			this.lblProg.Location = new System.Drawing.Point(7, 302);
			this.lblProg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblProg.Name = "lblProg";
			this.lblProg.Size = new System.Drawing.Size(51, 13);
			this.lblProg.TabIndex = 2;
			this.lblProg.Text = "Progress:";
			// 
			// btnGo
			// 
			this.btnGo.Location = new System.Drawing.Point(9, 326);
			this.btnGo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(59, 32);
			this.btnGo.TabIndex = 3;
			this.btnGo.Text = "Go";
			this.btnGo.UseVisualStyleBackColor = true;
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// txtLog
			// 
			this.txtLog.Location = new System.Drawing.Point(266, 0);
			this.txtLog.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(266, 290);
			this.txtLog.TabIndex = 5;
			// 
			// txtList
			// 
			this.txtList.Location = new System.Drawing.Point(164, 342);
			this.txtList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.txtList.Name = "txtList";
			this.txtList.Size = new System.Drawing.Size(237, 20);
			this.txtList.TabIndex = 6;
			// 
			// btnGetList
			// 
			this.btnGetList.Location = new System.Drawing.Point(404, 340);
			this.btnGetList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnGetList.Name = "btnGetList";
			this.btnGetList.Size = new System.Drawing.Size(59, 20);
			this.btnGetList.TabIndex = 7;
			this.btnGetList.Text = "List";
			this.btnGetList.UseVisualStyleBackColor = true;
			this.btnGetList.Click += new System.EventHandler(this.btnGetList_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(532, 370);
			this.Controls.Add(this.btnGetList);
			this.Controls.Add(this.txtList);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.btnGo);
			this.Controls.Add(this.lblProg);
			this.Controls.Add(this.txtUrls);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmMain";
			this.Text = "YouTuber";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtUrls;
		private System.Windows.Forms.Label lblProg;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.TextBox txtList;
		private System.Windows.Forms.Button btnGetList;
	}
}

