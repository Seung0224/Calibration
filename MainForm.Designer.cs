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
            this.btnLoadCalibration = new System.Windows.Forms.Button();
            this.btnMultiCalibrate = new System.Windows.Forms.Button();
            this.BTN_COMPARE = new System.Windows.Forms.Button();
            this.btnBarrelDistortionVerify = new System.Windows.Forms.Button();
            this.btnCompanyVerify = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnVerify = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.dgvDistances = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvHomography = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDistortion = new System.Windows.Forms.DataGridView();
            this.imageBox1 = new Cyotek.Windows.Forms.ImageBox();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistances)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHomography)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistortion)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnLoadCalibration);
            this.panelTop.Controls.Add(this.btnMultiCalibrate);
            this.panelTop.Controls.Add(this.BTN_COMPARE);
            this.panelTop.Controls.Add(this.btnBarrelDistortionVerify);
            this.panelTop.Controls.Add(this.btnCompanyVerify);
            this.panelTop.Controls.Add(this.lblInfo);
            this.panelTop.Controls.Add(this.btnVerify);
            this.panelTop.Controls.Add(this.btnOpen);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(2035, 60);
            this.panelTop.TabIndex = 0;
            // 
            // BTN_COMPARE
            // 
            this.BTN_COMPARE.Location = new System.Drawing.Point(600, 12);
            this.BTN_COMPARE.Name = "BTN_COMPARE";
            this.BTN_COMPARE.Size = new System.Drawing.Size(140, 35);
            this.BTN_COMPARE.TabIndex = 5;
            this.BTN_COMPARE.Text = "회사 DLL 비교";
            this.BTN_COMPARE.UseVisualStyleBackColor = true;
            this.BTN_COMPARE.Click += new System.EventHandler(this.BTN_COMPARE_Click);
            // 
            // btnBarrelDistortionVerify
            // 
            this.btnBarrelDistortionVerify.Location = new System.Drawing.Point(454, 12);
            this.btnBarrelDistortionVerify.Name = "btnBarrelDistortionVerify";
            this.btnBarrelDistortionVerify.Size = new System.Drawing.Size(140, 35);
            this.btnBarrelDistortionVerify.TabIndex = 4;
            this.btnBarrelDistortionVerify.Text = "왜곡 보정 오차 검증";
            this.btnBarrelDistortionVerify.UseVisualStyleBackColor = true;
            this.btnBarrelDistortionVerify.Click += new System.EventHandler(this.BTN_Barrel_Distrotion_Verify_Click);
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
            // btnMultiCalibrate
            //
            this.btnMultiCalibrate.Location = new System.Drawing.Point(759, 12);
            this.btnMultiCalibrate.Name = "btnMultiCalibrate";
            this.btnMultiCalibrate.Size = new System.Drawing.Size(150, 35);
            this.btnMultiCalibrate.TabIndex = 9;
            this.btnMultiCalibrate.Text = "다중 이미지 캘리브레이션";
            this.btnMultiCalibrate.UseVisualStyleBackColor = true;
            this.btnMultiCalibrate.Click += new System.EventHandler(this.BTN_MultiCalibrate_Click);
            //
            // btnLoadCalibration
            //
            this.btnLoadCalibration.Location = new System.Drawing.Point(921, 12);
            this.btnLoadCalibration.Name = "btnLoadCalibration";
            this.btnLoadCalibration.Size = new System.Drawing.Size(150, 35);
            this.btnLoadCalibration.TabIndex = 10;
            this.btnLoadCalibration.Text = "캘리브레이션 데이터 로드";
            this.btnLoadCalibration.UseVisualStyleBackColor = true;
            this.btnLoadCalibration.Click += new System.EventHandler(this.BTN_LoadCalibration_Click);
            //
            // lblInfo
            //
            this.lblInfo.AutoSize = true;
            this.lblInfo.ForeColor = System.Drawing.Color.Blue;
            this.lblInfo.Location = new System.Drawing.Point(1083, 23);
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
            this.lstLog.Location = new System.Drawing.Point(694, 60);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(280, 540);
            this.lstLog.TabIndex = 1;
            // 
            // dgvDistances
            // 
            this.dgvDistances.AllowUserToAddRows = false;
            this.dgvDistances.AllowUserToDeleteRows = false;
            this.dgvDistances.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDistances.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
            this.dgvDistances.Dock = System.Windows.Forms.DockStyle.Right;
            this.dgvDistances.Location = new System.Drawing.Point(974, 60);
            this.dgvDistances.Name = "dgvDistances";
            this.dgvDistances.ReadOnly = true;
            this.dgvDistances.RowHeadersVisible = false;
            this.dgvDistances.Size = new System.Drawing.Size(361, 540);
            this.dgvDistances.TabIndex = 6;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 30F;
            this.dataGridViewTextBoxColumn1.HeaderText = "#";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "P1 (X, Y)";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "P2 (X, Y)";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "거리(mm)";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "오차(mm)";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dgvHomography
            // 
            this.dgvHomography.AllowUserToAddRows = false;
            this.dgvHomography.AllowUserToDeleteRows = false;
            this.dgvHomography.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHomography.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10});
            this.dgvHomography.Dock = System.Windows.Forms.DockStyle.Right;
            this.dgvHomography.Location = new System.Drawing.Point(1335, 60);
            this.dgvHomography.Name = "dgvHomography";
            this.dgvHomography.ReadOnly = true;
            this.dgvHomography.RowHeadersVisible = false;
            this.dgvHomography.Size = new System.Drawing.Size(350, 540);
            this.dgvHomography.TabIndex = 7;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.FillWeight = 30F;
            this.dataGridViewTextBoxColumn6.HeaderText = "#";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "P1 (X, Y)";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "P2 (X, Y)";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "거리(mm)";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "오차(mm)";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // dgvDistortion
            // 
            this.dgvDistortion.AllowUserToAddRows = false;
            this.dgvDistortion.AllowUserToDeleteRows = false;
            this.dgvDistortion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDistortion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn13,
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn15});
            this.dgvDistortion.Dock = System.Windows.Forms.DockStyle.Right;
            this.dgvDistortion.Location = new System.Drawing.Point(1685, 60);
            this.dgvDistortion.Name = "dgvDistortion";
            this.dgvDistortion.ReadOnly = true;
            this.dgvDistortion.RowHeadersVisible = false;
            this.dgvDistortion.Size = new System.Drawing.Size(350, 540);
            this.dgvDistortion.TabIndex = 8;
            // 
            // imageBox1
            // 
            this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox1.Location = new System.Drawing.Point(0, 60);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(694, 540);
            this.imageBox1.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.FillWeight = 30F;
            this.dataGridViewTextBoxColumn11.HeaderText = "#";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.HeaderText = "P1 (X, Y)";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.HeaderText = "P2 (X, Y)";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.HeaderText = "거리(mm)";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn15
            // 
            this.dataGridViewTextBoxColumn15.HeaderText = "오차(mm)";
            this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            this.dataGridViewTextBoxColumn15.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2035, 600);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.dgvDistances);
            this.Controls.Add(this.dgvHomography);
            this.Controls.Add(this.dgvDistortion);
            this.Controls.Add(this.panelTop);
            this.Name = "MainForm";
            this.Text = "머신비전 캘리브레이션 오차 검증 툴";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistances)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHomography)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistortion)).EndInit();
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
        private System.Windows.Forms.Button BTN_COMPARE;
        private System.Windows.Forms.Button btnMultiCalibrate;
        private System.Windows.Forms.Button btnLoadCalibration;
        private System.Windows.Forms.DataGridView dgvDistances;
        private System.Windows.Forms.DataGridView dgvHomography;
        private System.Windows.Forms.DataGridView dgvDistortion;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
    }
}