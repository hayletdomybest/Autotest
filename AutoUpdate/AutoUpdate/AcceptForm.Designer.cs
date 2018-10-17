namespace AutoUpdate
{
    partial class AcceptForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lab_title_lastest = new System.Windows.Forms.Label();
            this.labe_title_curr = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(83, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "是";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(219, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "否";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lab_title_lastest
            // 
            this.lab_title_lastest.AutoSize = true;
            this.lab_title_lastest.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_title_lastest.Location = new System.Drawing.Point(100, 51);
            this.lab_title_lastest.Name = "lab_title_lastest";
            this.lab_title_lastest.Size = new System.Drawing.Size(194, 21);
            this.lab_title_lastest.TabIndex = 2;
            this.lab_title_lastest.Text = "新版本{0}是否跟新?";
            // 
            // labe_title_curr
            // 
            this.labe_title_curr.AutoSize = true;
            this.labe_title_curr.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labe_title_curr.Location = new System.Drawing.Point(124, 9);
            this.labe_title_curr.Name = "labe_title_curr";
            this.labe_title_curr.Size = new System.Drawing.Size(122, 21);
            this.labe_title_curr.TabIndex = 3;
            this.labe_title_curr.Text = "當前版本{0}";
            // 
            // AcceptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 164);
            this.Controls.Add(this.labe_title_curr);
            this.Controls.Add(this.lab_title_lastest);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "AcceptForm";
            this.Text = "AcceptForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lab_title_lastest;
        private System.Windows.Forms.Label labe_title_curr;
    }
}