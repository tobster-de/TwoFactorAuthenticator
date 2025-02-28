﻿namespace TwoFactorAuthenticator.WinformsExample
{
    partial class Form1
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
            this.txtAccountTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSecretKey = new System.Windows.Forms.TextBox();
            this.pbQR = new System.Windows.Forms.PictureBox();
            this.btnSetup = new System.Windows.Forms.Button();
            this.btnGetCurrentCode = new System.Windows.Forms.Button();
            this.txtSetupCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtCurrentCodes = new System.Windows.Forms.TextBox();
            this.btnDebugTest = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIssuer = new System.Windows.Forms.TextBox();
            this.factorControl = new TwoFactorAuthenticator.WinForms.FactorControl();
            ((System.ComponentModel.ISupportInitialize)(this.pbQR)).BeginInit();
            this.SuspendLayout();
            // 
            // txtAccountTitle
            // 
            this.txtAccountTitle.Location = new System.Drawing.Point(91, 45);
            this.txtAccountTitle.Name = "txtAccountTitle";
            this.txtAccountTitle.Size = new System.Drawing.Size(155, 21);
            this.txtAccountTitle.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Account Title:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Secret:";
            // 
            // txtSecretKey
            // 
            this.txtSecretKey.Location = new System.Drawing.Point(91, 71);
            this.txtSecretKey.Name = "txtSecretKey";
            this.txtSecretKey.Size = new System.Drawing.Size(155, 21);
            this.txtSecretKey.TabIndex = 2;
            // 
            // pbQR
            // 
            this.pbQR.BackColor = System.Drawing.Color.Transparent;
            this.pbQR.Location = new System.Drawing.Point(27, 117);
            this.pbQR.Name = "pbQR";
            this.pbQR.Size = new System.Drawing.Size(231, 223);
            this.pbQR.TabIndex = 4;
            this.pbQR.TabStop = false;
            // 
            // btnSetup
            // 
            this.btnSetup.Location = new System.Drawing.Point(337, 15);
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size(243, 46);
            this.btnSetup.TabIndex = 5;
            this.btnSetup.Text = "Generate Setup / Get QR Code";
            this.btnSetup.UseVisualStyleBackColor = true;
            this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
            // 
            // btnGetCurrentCode
            // 
            this.btnGetCurrentCode.Location = new System.Drawing.Point(396, 250);
            this.btnGetCurrentCode.Name = "btnGetCurrentCode";
            this.btnGetCurrentCode.Size = new System.Drawing.Size(173, 27);
            this.btnGetCurrentCode.TabIndex = 6;
            this.btnGetCurrentCode.Text = "Get Current";
            this.btnGetCurrentCode.UseVisualStyleBackColor = true;
            this.btnGetCurrentCode.Click += new System.EventHandler(this.btnGetCurrentCode_Click);
            // 
            // txtSetupCode
            // 
            this.txtSetupCode.Location = new System.Drawing.Point(337, 67);
            this.txtSetupCode.Multiline = true;
            this.txtSetupCode.Name = "txtSetupCode";
            this.txtSetupCode.ReadOnly = true;
            this.txtSetupCode.Size = new System.Drawing.Size(243, 105);
            this.txtSetupCode.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(330, 182);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Test Code:";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(396, 217);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(173, 27);
            this.btnTest.TabIndex = 10;
            this.btnTest.Text = "Test Two-Factor Code";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtCurrentCodes
            // 
            this.txtCurrentCodes.Location = new System.Drawing.Point(337, 283);
            this.txtCurrentCodes.Multiline = true;
            this.txtCurrentCodes.Name = "txtCurrentCodes";
            this.txtCurrentCodes.ReadOnly = true;
            this.txtCurrentCodes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCurrentCodes.Size = new System.Drawing.Size(243, 105);
            this.txtCurrentCodes.TabIndex = 11;
            // 
            // btnDebugTest
            // 
            this.btnDebugTest.Location = new System.Drawing.Point(27, 361);
            this.btnDebugTest.Name = "btnDebugTest";
            this.btnDebugTest.Size = new System.Drawing.Size(173, 27);
            this.btnDebugTest.TabIndex = 12;
            this.btnDebugTest.Text = "Misc Test";
            this.btnDebugTest.UseVisualStyleBackColor = true;
            this.btnDebugTest.Click += new System.EventHandler(this.btnDebugTest_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Issuer:";
            // 
            // txtIssuer
            // 
            this.txtIssuer.Location = new System.Drawing.Point(91, 18);
            this.txtIssuer.Name = "txtIssuer";
            this.txtIssuer.Size = new System.Drawing.Size(155, 21);
            this.txtIssuer.TabIndex = 13;
            // 
            // factorControl
            // 
            this.factorControl.AllowCopyToClipboard = false;
            this.factorControl.BorderColor = System.Drawing.Color.Gray;
            this.factorControl.BoxGap = 5;
            this.factorControl.BoxSize = new System.Drawing.Size(25, 25);
            this.factorControl.CornerRadius = 5;
            this.factorControl.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.factorControl.Location = new System.Drawing.Point(396, 178);
            this.factorControl.Margin = new System.Windows.Forms.Padding(0);
            this.factorControl.Name = "factorControl";
            this.factorControl.PasteFromClipboardMode = TwoFactorAuthenticator.WinForms.PasteFromClipboardMode.PasteFullToken;
            this.factorControl.Size = new System.Drawing.Size(176, 26);
            this.factorControl.TabIndex = 15;
            this.factorControl.TokenLength = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 408);
            this.Controls.Add(this.factorControl);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtIssuer);
            this.Controls.Add(this.btnDebugTest);
            this.Controls.Add(this.txtCurrentCodes);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSetupCode);
            this.Controls.Add(this.btnGetCurrentCode);
            this.Controls.Add(this.btnSetup);
            this.Controls.Add(this.pbQR);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSecretKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAccountTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Authenticator Test App";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbQR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtIssuer;

        #endregion

        private System.Windows.Forms.TextBox txtAccountTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSecretKey;
        private System.Windows.Forms.PictureBox pbQR;
        private System.Windows.Forms.Button btnSetup;
        private System.Windows.Forms.Button btnGetCurrentCode;
        private System.Windows.Forms.TextBox txtSetupCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox txtCurrentCodes;
        private System.Windows.Forms.Button btnDebugTest;
        private TwoFactorAuthenticator.WinForms.FactorControl factorControl;
    }
}

