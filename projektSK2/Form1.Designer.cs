
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
            this.buttonReceive = new System.Windows.Forms.Button();
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
            this.buttonLogOut.BackColor = System.Drawing.Color.DimGray;
            this.buttonLogOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonLogOut.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonLogOut.Location = new System.Drawing.Point(6, 354);
            this.buttonLogOut.Name = "buttonLogOut";
            this.buttonLogOut.Size = new System.Drawing.Size(114, 108);
            this.buttonLogOut.TabIndex = 1;
            this.buttonLogOut.Text = "Log out";
            this.buttonLogOut.UseVisualStyleBackColor = false;
            this.buttonLogOut.Visible = false;
            this.buttonLogOut.Click += new System.EventHandler(this.buttonLogOut_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.BackColor = System.Drawing.Color.DimGray;
            this.buttonExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonExit.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonExit.Location = new System.Drawing.Point(6, 468);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(114, 108);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = false;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonTopcics
            // 
            this.buttonTopcics.BackColor = System.Drawing.Color.DimGray;
            this.buttonTopcics.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonTopcics.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonTopcics.Location = new System.Drawing.Point(6, 240);
            this.buttonTopcics.Name = "buttonTopcics";
            this.buttonTopcics.Size = new System.Drawing.Size(114, 108);
            this.buttonTopcics.TabIndex = 0;
            this.buttonTopcics.Text = "Topics";
            this.buttonTopcics.UseVisualStyleBackColor = false;
            this.buttonTopcics.Visible = false;
            this.buttonTopcics.Click += new System.EventHandler(this.buttonTopcics_Click);
            // 
            // buttonSend
            // 
            this.buttonSend.BackColor = System.Drawing.Color.DimGray;
            this.buttonSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonSend.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonSend.Location = new System.Drawing.Point(6, 126);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(114, 108);
            this.buttonSend.TabIndex = 3;
            this.buttonSend.Text = "Send message";
            this.buttonSend.UseVisualStyleBackColor = false;
            this.buttonSend.Visible = false;
            this.buttonSend.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonReceive
            // 
            this.buttonReceive.BackColor = System.Drawing.Color.DimGray;
            this.buttonReceive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonReceive.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.buttonReceive.Location = new System.Drawing.Point(6, 12);
            this.buttonReceive.Name = "buttonReceive";
            this.buttonReceive.Size = new System.Drawing.Size(114, 108);
            this.buttonReceive.TabIndex = 4;
            this.buttonReceive.Text = "Receive messages";
            this.buttonReceive.UseVisualStyleBackColor = false;
            this.buttonReceive.Visible = false;
            this.buttonReceive.Click += new System.EventHandler(this.buttonRecieve_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.buttonReceive);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.buttonTopcics);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonLogOut);
            this.Controls.Add(this.panelChildForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client publish/subscribe";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.Panel panelChildForm;
        private System.Windows.Forms.Button buttonLogOut;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonTopcics;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonReceive;
    }
}

