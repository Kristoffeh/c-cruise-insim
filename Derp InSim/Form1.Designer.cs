namespace Derp_InSim
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
            this.chatbox = new System.Windows.Forms.TextBox();
            this.typebox = new System.Windows.Forms.TextBox();
            this.SQL_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chatbox
            // 
            this.chatbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatbox.Font = new System.Drawing.Font("Georgia", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatbox.Location = new System.Drawing.Point(12, 34);
            this.chatbox.Multiline = true;
            this.chatbox.Name = "chatbox";
            this.chatbox.ReadOnly = true;
            this.chatbox.Size = new System.Drawing.Size(485, 362);
            this.chatbox.TabIndex = 2;
            this.chatbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            // 
            // typebox
            // 
            this.typebox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typebox.Location = new System.Drawing.Point(11, 402);
            this.typebox.Name = "typebox";
            this.typebox.Size = new System.Drawing.Size(486, 20);
            this.typebox.TabIndex = 1;
            this.typebox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.typebox_KeyDown);
            this.typebox.Validated += new System.EventHandler(this.typebox_TextChanged);
            // 
            // SQL_label
            // 
            this.SQL_label.AutoSize = true;
            this.SQL_label.Font = new System.Drawing.Font("Georgia", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SQL_label.Location = new System.Drawing.Point(12, 9);
            this.SQL_label.Name = "SQL_label";
            this.SQL_label.Size = new System.Drawing.Size(97, 16);
            this.SQL_label.TabIndex = 3;
            this.SQL_label.Text = "MySQL : -------";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 434);
            this.Controls.Add(this.SQL_label);
            this.Controls.Add(this.typebox);
            this.Controls.Add(this.chatbox);
            this.Name = "Form1";
            this.Text = "EC InSim";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatbox;
        private System.Windows.Forms.TextBox typebox;
        private System.Windows.Forms.Label SQL_label;
    }
}

