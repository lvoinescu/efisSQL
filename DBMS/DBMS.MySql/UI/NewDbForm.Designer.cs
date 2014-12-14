namespace DBMS.MySQL
{
    partial class NewDbForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewDbForm));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.charSet = new System.Windows.Forms.ComboBox();
            this.collate = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox1.Location = new System.Drawing.Point(182, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(179, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.Location = new System.Drawing.Point(125, 108);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button2.Location = new System.Drawing.Point(215, 108);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Charset";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Collate";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // charSet
            // 
            this.charSet.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.charSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.charSet.FormattingEnabled = true;
            this.charSet.Items.AddRange(new object[] {
            "default",
            "armscii8",
            "ascii",
            "big5",
            "binary",
            "cp1250",
            "cp1251",
            "cp1256",
            "cp1257",
            "cp850",
            "cp852",
            "cp866",
            "cp932",
            "dec8",
            "eucimps",
            "euckr",
            "gb2312",
            "gbk",
            "geostd8",
            "greek",
            "hebrew",
            "hp8",
            "keybcs2",
            "koi8r",
            "koi8u",
            "latin1",
            "latin2",
            "latin5",
            "latin7",
            "macce",
            "macroma",
            "sjis",
            "swe7",
            "tis620",
            "ucs2",
            "ujis",
            "utf8"});
            this.charSet.Location = new System.Drawing.Point(182, 38);
            this.charSet.MaxDropDownItems = 15;
            this.charSet.Name = "charSet";
            this.charSet.Size = new System.Drawing.Size(179, 21);
            this.charSet.TabIndex = 7;
            // 
            // collate
            // 
            this.collate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.collate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.collate.FormattingEnabled = true;
            this.collate.Items.AddRange(new object[] {
            "default",
            "big5_chinese_ci",
            "big5_bin",
            "dec8_swedish_ci",
            "dec8_bin",
            "cp850_general_ci",
            "cp850_bin",
            "hp8_english_ci",
            "hp8_bin",
            "koi8r_general_ci",
            "koi8r_bin",
            "latin1_german1_ci",
            "latin1_swedish_ci",
            "latin1_danish_ci",
            "latin1_german2_ci",
            "latin1_bin",
            "latin1_general_ci",
            "latin1_general_cs",
            "latin1_spanish_ci",
            "latin2_czech_cs",
            "latin2_general_ci",
            "latin2_hungarian_ci",
            "latin2_croatian_ci",
            "latin2_bin",
            "swe7_swedish_ci",
            "swe7_bin",
            "ascii_general_ci",
            "ascii_bin",
            "ujis_japanese_ci",
            "ujis_bin",
            "sjis_japanese_ci",
            "sjis_bin",
            "hebrew_general_ci",
            "hebrew_bin",
            "tis620_thai_ci",
            "tis620_bin",
            "euckr_korean_ci",
            "euckr_bin",
            "koi8u_general_ci",
            "koi8u_bin",
            "gb2312_chinese_ci",
            "gb2312_bin",
            "greek_general_ci",
            "greek_bin",
            "cp1250_general_ci",
            "cp1250_czech_cs",
            "cp1250_croatian_ci",
            "cp1250_bin",
            "gbk_chinese_ci",
            "gbk_bin",
            "latin5_turkish_ci",
            "latin5_bin",
            "armscii8_general_ci",
            "armscii8_bin",
            "utf8_general_ci",
            "utf8_bin",
            "utf8_unicode_ci",
            "utf8_icelandic_ci",
            "utf8_latvian_ci",
            "utf8_romanian_ci",
            "utf8_slovenian_ci",
            "utf8_polish_ci",
            "utf8_estonian_ci",
            "utf8_spanish_ci",
            "utf8_swedish_ci",
            "utf8_turkish_ci",
            "utf8_czech_ci",
            "utf8_danish_ci",
            "utf8_lithuanian_ci",
            "utf8_slovak_ci",
            "utf8_spanish2_ci",
            "utf8_roman_ci",
            "utf8_persian_ci",
            "utf8_esperanto_ci",
            "utf8_hungarian_ci",
            "ucs2_general_ci",
            "ucs2_bin",
            "ucs2_unicode_ci",
            "ucs2_icelandic_ci",
            "ucs2_latvian_ci",
            "ucs2_romanian_ci",
            "ucs2_slovenian_ci",
            "ucs2_polish_ci",
            "ucs2_estonian_ci",
            "ucs2_spanish_ci",
            "ucs2_swedish_ci",
            "ucs2_turkish_ci",
            "ucs2_czech_ci",
            "ucs2_danish_ci",
            "ucs2_lithuanian_ci",
            "ucs2_slovak_ci",
            "ucs2_spanish2_ci",
            "ucs2_roman_ci",
            "ucs2_persian_ci",
            "ucs2_esperanto_ci",
            "ucs2_hungarian_ci",
            "cp866_general_ci",
            "cp866_bin",
            "keybcs2_general_ci",
            "keybcs2_bin",
            "macce_general_ci",
            "macce_bin",
            "macroman_general_ci",
            "macroman_bin",
            "cp852_general_ci",
            "cp852_bin",
            "latin7_estonian_cs",
            "latin7_general_ci",
            "latin7_general_cs",
            "latin7_bin",
            "cp1251_bulgarian_c\t",
            "cp1251_ukrainian_ci",
            "cp1251_bin",
            "cp1251_general_ci",
            "cp1251_general_cs",
            "cp1256_general_ci",
            "cp1256_bin",
            "cp1257_lithuanian_ci",
            "cp1257_bin",
            "cp1257_general_ci",
            "binary",
            "geostd8_general_ci",
            "geostd8_bin",
            "cp932_japanese_ci",
            "cp932_bin",
            "eucjpms_japanese_ci",
            "eucjpms_bin"});
            this.collate.Location = new System.Drawing.Point(182, 71);
            this.collate.MaxDropDownItems = 15;
            this.collate.Name = "collate";
            this.collate.Size = new System.Drawing.Size(179, 21);
            this.collate.TabIndex = 7;
            // 
            // NewDbForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 143);
            this.Controls.Add(this.collate);
            this.Controls.Add(this.charSet);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewDbForm";
            this.Text = "New database";
            this.Load += new System.EventHandler(this.NewDbForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox charSet;
        private System.Windows.Forms.ComboBox collate;
    }
}