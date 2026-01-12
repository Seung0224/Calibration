namespace Calibration
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnCompanyVerify = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnVerify = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.imageBox1 = new Cyotek.Windows.Forms.ImageBox();
            this.btnBarrelDistortionVerify = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnBarrelDistortionVerify);
            this.panelTop.Controls.Add(this.btnCompanyVerify);
            this.panelTop.Controls.Add(this.lblInfo);
            this.panelTop.Controls.Add(this.btnVerify);
            this.panelTop.Controls.Add(this.btnOpen);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1000, 60);
            this.panelTop.TabIndex = 0;
            // 
            // btnCompanyVerify
            // 
            this.btnCompanyVerify.Location = new System.Drawing.Point(308, 12);
            this.btnCompanyVerify.Name = "btnCompanyVerify";
            this.btnCompanyVerify.Size = new System.Drawing.Size(140, 35);
            this.btnCompanyVerify.TabIndex = 3;
            this.btnCompanyVerify.Text = "회사 방식 오차 검증";
            this.btnCompanyVerify.UseVisualStyleBackColor = true;
            this.btnCompanyVerify.Click += new System.EventHandler(this.BTN_Company_Verify_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.ForeColor = System.Drawing.Color.Blue;
            this.lblInfo.Location = new System.Drawing.Point(759, 23);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(216, 12);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "검증 규격: 9x6 코너 / 격자 크기 30mm";
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(155, 12);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(140, 35);
            this.btnVerify.TabIndex = 1;
            this.btnVerify.Text = "기존 방식 오차 검증";
            this.btnVerify.UseVisualStyleBackColor = true;
            this.btnVerify.Click += new System.EventHandler(this.BTN_Verify_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(12, 12);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(130, 35);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "이미지 불러오기";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.BTN_Open_Click);
            // 
            // lstLog
            // 
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Right;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 12;
            this.lstLog.Location = new System.Drawing.Point(720, 60);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(280, 540);
            this.lstLog.TabIndex = 1;
            // 
            // imageBox1
            // 
            this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox1.Location = new System.Drawing.Point(0, 60);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(720, 540);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.Text = "이미지를 불러와주세요";
            // 
            // btnBarrelDistortionVerify
            // 
            this.btnBarrelDistortionVerify.Location = new System.Drawing.Point(461, 12);
            this.btnBarrelDistortionVerify.Name = "btnBarrelDistortionVerify";
            this.btnBarrelDistortionVerify.Size = new System.Drawing.Size(140, 35);
            this.btnBarrelDistortionVerify.TabIndex = 4;
            this.btnBarrelDistortionVerify.Text = "왜곡 보정 오차 검증";
            this.btnBarrelDistortionVerify.UseVisualStyleBackColor = true;
            this.btnBarrelDistortionVerify.Click += new System.EventHandler(this.BTN_Barrel_Distrotion_Verify_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.panelTop);
            this.Name = "MainForm";
            this.Text = "머신비전 캘리브레이션 오차 검증 툴";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnVerify;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.ListBox lstLog;
        private Cyotek.Windows.Forms.ImageBox imageBox1;
        private System.Windows.Forms.Button btnCompanyVerify;
        private System.Windows.Forms.Button btnBarrelDistortionVerify;
    }
}