
using System;

namespace projektSK2
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelChildForm = new System.Windows.Forms.Panel();
            this.buttonLogOut = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonTopcics = new System.Windows.Forms.Button();
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonRecieve = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panelChildForm
            // 
            this.panelChildForm.Location = new System.Drawing.Point(126, 1);
            this.panelChildForm.Name = "panelChildForm";
            this.panelChildForm.Size = new System.Drawing.Size(860, 663);
            this.panelChildForm.TabIndex = 0;
            this.panelChildForm.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChildForm_Paint);
            // 
            // buttonLogOut
            // 
            this.buttonLogOut.Location = new System.Drawing.Point(12, 354);
            this.buttonLogOut.Name = "buttonLogOut";
            this.buttonLogOut.Size = new System.Drawing.Size(108, 108);
            this.buttonLogOut.TabIndex = 1;
            this.buttonLogOut.Text = "Log out";
            this.buttonLogOut.UseVisualStyleBackColor = true;
            this.buttonLogOut.Visible = false;
            this.buttonLogOut.Click += new System.EventHandler(this.buttonLogOut_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(12, 468);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(108, 108);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonTopcics
            // 
            this.buttonTopcics.Location = new System.Drawing.Point(12, 240);
            this.buttonTopcics.Name = "buttonTopcics";
            this.buttonTopcics.Size = new System.Drawing.Size(108, 108);
            this.buttonTopcics.TabIndex = 0;
            this.buttonTopcics.Text = "Topics";
            this.buttonTopcics.UseVisualStyleBackColor = true;
            this.buttonTopcics.Visible = false;
            this.buttonTopcics.Click += new System.EventHandler(this.buttonTopcics_Click);
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(12, 126);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(108, 108);
            this.buttonSend.TabIndex = 3;
            this.buttonSend.Text = "Send message";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Visible = false;
            this.buttonSend.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonRecieve
            // 
            this.buttonRecieve.Location = new System.Drawing.Point(12, 12);
            this.buttonRecieve.Name = "buttonRecieve";
            this.buttonRecieve.Size = new System.Drawing.Size(108, 108);
            this.buttonRecieve.TabIndex = 4;
            this.buttonRecieve.Text = "Recieve messages";
            this.buttonRecieve.UseVisualStyleBackColor = true;
            this.buttonRecieve.Visible = false;
            this.buttonRecieve.Click += new System.EventHandler(this.buttonRecieve_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.buttonRecieve);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.buttonTopcics);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonLogOut);
            this.Controls.Add(this.panelChildForm);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.Panel panelChildForm;
        private System.Windows.Forms.Button buttonLogOut;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonTopcics;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonRecieve;
    }
}

