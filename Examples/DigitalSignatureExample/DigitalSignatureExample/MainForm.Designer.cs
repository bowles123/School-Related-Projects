namespace DigitalSignatureExample
{
    partial class MainForm
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
            this.publicKeyLabel = new System.Windows.Forms.Label();
            this.privateKeyLabel = new System.Windows.Forms.Label();
            this.generateKeysButton = new System.Windows.Forms.Button();
            this.publicKey = new System.Windows.Forms.TextBox();
            this.privateKey = new System.Windows.Forms.TextBox();
            this.sampleMessageLabel = new System.Windows.Forms.Label();
            this.sampleMessage = new System.Windows.Forms.TextBox();
            this.signButton = new System.Windows.Forms.Button();
            this.hash = new System.Windows.Forms.TextBox();
            this.hashLabel = new System.Windows.Forms.Label();
            this.encryptedHash = new System.Windows.Forms.TextBox();
            this.dsLabel = new System.Windows.Forms.Label();
            this.verifyButton = new System.Windows.Forms.Button();
            this.verifyResults = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // publicKeyLabel
            // 
            this.publicKeyLabel.AutoSize = true;
            this.publicKeyLabel.Location = new System.Drawing.Point(12, 48);
            this.publicKeyLabel.Name = "publicKeyLabel";
            this.publicKeyLabel.Size = new System.Drawing.Size(60, 13);
            this.publicKeyLabel.TabIndex = 0;
            this.publicKeyLabel.Text = "Public Key:";
            // 
            // privateKeyLabel
            // 
            this.privateKeyLabel.AutoSize = true;
            this.privateKeyLabel.Location = new System.Drawing.Point(12, 162);
            this.privateKeyLabel.Name = "privateKeyLabel";
            this.privateKeyLabel.Size = new System.Drawing.Size(64, 13);
            this.privateKeyLabel.TabIndex = 2;
            this.privateKeyLabel.Text = "Private Key:";
            // 
            // generateKeysButton
            // 
            this.generateKeysButton.Location = new System.Drawing.Point(105, 12);
            this.generateKeysButton.Name = "generateKeysButton";
            this.generateKeysButton.Size = new System.Drawing.Size(75, 23);
            this.generateKeysButton.TabIndex = 4;
            this.generateKeysButton.Text = "Generate Keys";
            this.generateKeysButton.UseVisualStyleBackColor = true;
            this.generateKeysButton.Click += new System.EventHandler(this.generateKeysButton_Click);
            // 
            // publicKey
            // 
            this.publicKey.Enabled = false;
            this.publicKey.Location = new System.Drawing.Point(105, 45);
            this.publicKey.Multiline = true;
            this.publicKey.Name = "publicKey";
            this.publicKey.Size = new System.Drawing.Size(708, 96);
            this.publicKey.TabIndex = 5;
            // 
            // privateKey
            // 
            this.privateKey.Enabled = false;
            this.privateKey.Location = new System.Drawing.Point(105, 159);
            this.privateKey.Multiline = true;
            this.privateKey.Name = "privateKey";
            this.privateKey.Size = new System.Drawing.Size(708, 112);
            this.privateKey.TabIndex = 6;
            // 
            // sampleMessageLabel
            // 
            this.sampleMessageLabel.AutoSize = true;
            this.sampleMessageLabel.Location = new System.Drawing.Point(12, 299);
            this.sampleMessageLabel.Name = "sampleMessageLabel";
            this.sampleMessageLabel.Size = new System.Drawing.Size(91, 13);
            this.sampleMessageLabel.TabIndex = 7;
            this.sampleMessageLabel.Text = "Sample Message:";
            // 
            // sampleMessage
            // 
            this.sampleMessage.Location = new System.Drawing.Point(105, 296);
            this.sampleMessage.Name = "sampleMessage";
            this.sampleMessage.Size = new System.Drawing.Size(708, 20);
            this.sampleMessage.TabIndex = 8;
            this.sampleMessage.Text = "This is a test message";
            // 
            // signButton
            // 
            this.signButton.Location = new System.Drawing.Point(105, 331);
            this.signButton.Name = "signButton";
            this.signButton.Size = new System.Drawing.Size(75, 23);
            this.signButton.TabIndex = 9;
            this.signButton.Text = "Sign";
            this.signButton.UseVisualStyleBackColor = true;
            this.signButton.Click += new System.EventHandler(this.signButton_Click);
            // 
            // hash
            // 
            this.hash.Enabled = false;
            this.hash.Location = new System.Drawing.Point(105, 369);
            this.hash.Multiline = true;
            this.hash.Name = "hash";
            this.hash.Size = new System.Drawing.Size(708, 53);
            this.hash.TabIndex = 11;
            // 
            // hashLabel
            // 
            this.hashLabel.AutoSize = true;
            this.hashLabel.Location = new System.Drawing.Point(12, 372);
            this.hashLabel.Name = "hashLabel";
            this.hashLabel.Size = new System.Drawing.Size(35, 13);
            this.hashLabel.TabIndex = 10;
            this.hashLabel.Text = "Hash:";
            // 
            // encryptedHash
            // 
            this.encryptedHash.Enabled = false;
            this.encryptedHash.Location = new System.Drawing.Point(105, 440);
            this.encryptedHash.Multiline = true;
            this.encryptedHash.Name = "encryptedHash";
            this.encryptedHash.Size = new System.Drawing.Size(708, 115);
            this.encryptedHash.TabIndex = 13;
            // 
            // dsLabel
            // 
            this.dsLabel.AutoSize = true;
            this.dsLabel.Location = new System.Drawing.Point(12, 443);
            this.dsLabel.Name = "dsLabel";
            this.dsLabel.Size = new System.Drawing.Size(87, 13);
            this.dsLabel.TabIndex = 12;
            this.dsLabel.Text = "Digital Signature:";
            // 
            // verifyButton
            // 
            this.verifyButton.Location = new System.Drawing.Point(105, 570);
            this.verifyButton.Name = "verifyButton";
            this.verifyButton.Size = new System.Drawing.Size(75, 23);
            this.verifyButton.TabIndex = 14;
            this.verifyButton.Text = "Verify";
            this.verifyButton.UseVisualStyleBackColor = true;
            this.verifyButton.Click += new System.EventHandler(this.verifyButton_Click);
            // 
            // verifyResults
            // 
            this.verifyResults.AutoSize = true;
            this.verifyResults.Location = new System.Drawing.Point(197, 575);
            this.verifyResults.Name = "verifyResults";
            this.verifyResults.Size = new System.Drawing.Size(0, 13);
            this.verifyResults.TabIndex = 15;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 605);
            this.Controls.Add(this.verifyResults);
            this.Controls.Add(this.verifyButton);
            this.Controls.Add(this.encryptedHash);
            this.Controls.Add(this.dsLabel);
            this.Controls.Add(this.hash);
            this.Controls.Add(this.hashLabel);
            this.Controls.Add(this.signButton);
            this.Controls.Add(this.sampleMessage);
            this.Controls.Add(this.sampleMessageLabel);
            this.Controls.Add(this.privateKey);
            this.Controls.Add(this.publicKey);
            this.Controls.Add(this.generateKeysButton);
            this.Controls.Add(this.privateKeyLabel);
            this.Controls.Add(this.publicKeyLabel);
            this.Name = "MainForm";
            this.Text = "Digital Signature Example";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label publicKeyLabel;
        private System.Windows.Forms.Label privateKeyLabel;
        private System.Windows.Forms.Button generateKeysButton;
        private System.Windows.Forms.TextBox publicKey;
        private System.Windows.Forms.TextBox privateKey;
        private System.Windows.Forms.Label sampleMessageLabel;
        private System.Windows.Forms.TextBox sampleMessage;
        private System.Windows.Forms.Button signButton;
        private System.Windows.Forms.TextBox hash;
        private System.Windows.Forms.Label hashLabel;
        private System.Windows.Forms.TextBox encryptedHash;
        private System.Windows.Forms.Label dsLabel;
        private System.Windows.Forms.Button verifyButton;
        private System.Windows.Forms.Label verifyResults;
    }
}

