namespace SwsExample
{
    partial class ClientForm
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
            this.ExecuteButton = new System.Windows.Forms.Button();
            this.ResponseTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbMapTile = new System.Windows.Forms.RadioButton();
            this.rbGeocode = new System.Windows.Forms.RadioButton();
            this.rbAuthenticate = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.MapTilePictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapTilePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ExecuteButton
            // 
            this.ExecuteButton.Location = new System.Drawing.Point(453, 64);
            this.ExecuteButton.Name = "ExecuteButton";
            this.ExecuteButton.Size = new System.Drawing.Size(75, 23);
            this.ExecuteButton.TabIndex = 2;
            this.ExecuteButton.Text = "Execute";
            this.ExecuteButton.UseVisualStyleBackColor = true;
            this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // ResponseTextBox
            // 
            this.ResponseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResponseTextBox.Location = new System.Drawing.Point(12, 165);
            this.ResponseTextBox.Multiline = true;
            this.ResponseTextBox.Name = "ResponseTextBox";
            this.ResponseTextBox.ReadOnly = true;
            this.ResponseTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ResponseTextBox.Size = new System.Drawing.Size(509, 200);
            this.ResponseTextBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Response";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbMapTile);
            this.groupBox1.Controls.Add(this.rbGeocode);
            this.groupBox1.Controls.Add(this.rbAuthenticate);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(435, 96);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Action";
            // 
            // rbMapTile
            // 
            this.rbMapTile.AutoSize = true;
            this.rbMapTile.Location = new System.Drawing.Point(7, 64);
            this.rbMapTile.Name = "rbMapTile";
            this.rbMapTile.Size = new System.Drawing.Size(109, 17);
            this.rbMapTile.TabIndex = 2;
            this.rbMapTile.Text = "Request Map Tile";
            this.rbMapTile.UseVisualStyleBackColor = true;
            // 
            // rbGeocode
            // 
            this.rbGeocode.AutoSize = true;
            this.rbGeocode.Location = new System.Drawing.Point(7, 41);
            this.rbGeocode.Name = "rbGeocode";
            this.rbGeocode.Size = new System.Drawing.Size(126, 17);
            this.rbGeocode.TabIndex = 1;
            this.rbGeocode.Text = "Geocode An Address";
            this.rbGeocode.UseVisualStyleBackColor = true;
            // 
            // rbAuthenticate
            // 
            this.rbAuthenticate.AutoSize = true;
            this.rbAuthenticate.Checked = true;
            this.rbAuthenticate.Location = new System.Drawing.Point(7, 18);
            this.rbAuthenticate.Name = "rbAuthenticate";
            this.rbAuthenticate.Size = new System.Drawing.Size(85, 17);
            this.rbAuthenticate.TabIndex = 0;
            this.rbAuthenticate.TabStop = true;
            this.rbAuthenticate.Text = "Authenticate";
            this.rbAuthenticate.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 377);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Map Tile";
            // 
            // MapTilePictureBox
            // 
            this.MapTilePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MapTilePictureBox.Location = new System.Drawing.Point(12, 393);
            this.MapTilePictureBox.Name = "MapTilePictureBox";
            this.MapTilePictureBox.Size = new System.Drawing.Size(256, 256);
            this.MapTilePictureBox.TabIndex = 11;
            this.MapTilePictureBox.TabStop = false;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 655);
            this.Controls.Add(this.MapTilePictureBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ResponseTextBox);
            this.Controls.Add(this.ExecuteButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Name = "ClientForm";
            this.Text = "Rest Client";
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapTilePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ExecuteButton;
        private System.Windows.Forms.TextBox ResponseTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbMapTile;
        private System.Windows.Forms.RadioButton rbGeocode;
        private System.Windows.Forms.RadioButton rbAuthenticate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox MapTilePictureBox;
    }
}

