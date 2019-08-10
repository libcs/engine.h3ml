namespace Browser.Forms
{
    partial class BrowserForm
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
            this.address_bar = new System.Windows.Forms.TextBox();
            this.go_button = new System.Windows.Forms.Button();
            this.html = new Browser.Forms.HtmlControl();
            this.SuspendLayout();
            // 
            // address_bar
            // 
            this.address_bar.Location = new System.Drawing.Point(12, 12);
            this.address_bar.Name = "address_bar";
            this.address_bar.Size = new System.Drawing.Size(505, 20);
            this.address_bar.TabIndex = 0;
            this.address_bar.Text = "http://www.litehtml.com/";
            this.address_bar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.on_address_key_press);
            // 
            // go_button
            // 
            this.go_button.Location = new System.Drawing.Point(523, 12);
            this.go_button.Name = "go_button";
            this.go_button.Size = new System.Drawing.Size(54, 20);
            this.go_button.TabIndex = 1;
            this.go_button.Text = "GO";
            this.go_button.UseVisualStyleBackColor = true;
            this.go_button.Click += new System.EventHandler(this.on_go_clicked);
            // 
            // html
            // 
            this.html.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.html.Location = new System.Drawing.Point(12, 38);
            this.html.Name = "html";
            this.html.Size = new System.Drawing.Size(565, 280);
            this.html.TabIndex = 2;
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 330);
            this.Controls.Add(this.html);
            this.Controls.Add(this.go_button);
            this.Controls.Add(this.address_bar);
            this.Name = "BrowserForm";
            this.Text = "Browser";
            this.Resize += new System.EventHandler(this.BrowserForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox address_bar;
        private System.Windows.Forms.Button go_button;
        private Browser.Forms.HtmlControl html;
    }
}

