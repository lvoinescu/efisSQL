namespace DBMS.MySQL
{
    partial class AlterTableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlterTableForm));
            this.dg1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.collation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.unsigned = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.avgRowLen = new System.Windows.Forms.TextBox();
            this.pwd = new System.Windows.Forms.TextBox();
            this.comment = new System.Windows.Forms.TextBox();
            this.autoincrement = new System.Windows.Forms.TextBox();
            this.checksum = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.raidType = new System.Windows.Forms.ComboBox();
            this.rowFormat = new System.Windows.Forms.ComboBox();
            this.delaykeywrite = new System.Windows.Forms.ComboBox();
            this.engine = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.table = new System.Windows.Forms.TextBox();
            this.outPut = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dg1
            // 
            this.dg1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dg1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column6,
            this.collation,
            this.Column4,
            this.Column7,
            this.Column5,
            this.Column8,
            this.unsigned,
            this.Column9});
            this.dg1.Location = new System.Drawing.Point(12, 50);
            this.dg1.Name = "dg1";
            this.dg1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dg1.Size = new System.Drawing.Size(734, 258);
            this.dg1.TabIndex = 0;
            this.dg1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg1_CellValueChanged);
            this.dg1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dg1_UserDeletingRow);
            this.dg1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg1_CellEndEdit);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "id";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "type";
            this.Column2.Items.AddRange(new object[] {
            "bigint",
            "binary",
            "bit",
            "blob",
            "bool",
            "boolean",
            "char",
            "date",
            "datetime",
            "decimal",
            "double",
            "enum",
            "float",
            "int",
            "longblob",
            "longtext",
            "mediumblob",
            "mediumint",
            "mediumtext",
            "numeric",
            "real",
            "set",
            "smallint",
            "text",
            "time",
            "timestamp",
            "tinyblob",
            "tinyint",
            "tinytext",
            "varbinary",
            "varchar",
            "year"});
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "length/set";
            this.Column3.Name = "Column3";
            // 
            // Column6
            // 
            this.Column6.HeaderText = "default";
            this.Column6.Name = "Column6";
            // 
            // collation
            // 
            this.collation.HeaderText = "collation";
            this.collation.Items.AddRange(new object[] {
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
            this.collation.Name = "collation";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "primary key";
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "auto increment";
            this.Column7.Name = "Column7";
            // 
            // Column5
            // 
            this.Column5.FalseValue = "false";
            this.Column5.HeaderText = "not null";
            this.Column5.Name = "Column5";
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column5.TrueValue = "bool";
            // 
            // Column8
            // 
            this.Column8.HeaderText = "zero fill";
            this.Column8.Name = "Column8";
            // 
            // unsigned
            // 
            this.unsigned.HeaderText = "unsigned";
            this.unsigned.Name = "unsigned";
            // 
            // Column9
            // 
            this.Column9.HeaderText = "comment";
            this.Column9.Name = "Column9";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(653, 503);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "Alter";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 503);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 33);
            this.button2.TabIndex = 2;
            this.button2.Text = "Refresh";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.avgRowLen);
            this.groupBox1.Controls.Add(this.pwd);
            this.groupBox1.Controls.Add(this.comment);
            this.groupBox1.Controls.Add(this.autoincrement);
            this.groupBox1.Controls.Add(this.checksum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.raidType);
            this.groupBox1.Controls.Add(this.rowFormat);
            this.groupBox1.Controls.Add(this.delaykeywrite);
            this.groupBox1.Controls.Add(this.engine);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 347);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(733, 150);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced";
            // 
            // avgRowLen
            // 
            this.avgRowLen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.avgRowLen.Location = new System.Drawing.Point(123, 112);
            this.avgRowLen.Name = "avgRowLen";
            this.avgRowLen.Size = new System.Drawing.Size(121, 20);
            this.avgRowLen.TabIndex = 2;
            // 
            // pwd
            // 
            this.pwd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pwd.Location = new System.Drawing.Point(323, 112);
            this.pwd.Name = "pwd";
            this.pwd.PasswordChar = '*';
            this.pwd.Size = new System.Drawing.Size(165, 20);
            this.pwd.TabIndex = 2;
            this.pwd.UseSystemPasswordChar = true;
            // 
            // comment
            // 
            this.comment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comment.Location = new System.Drawing.Point(267, 38);
            this.comment.Multiline = true;
            this.comment.Name = "comment";
            this.comment.Size = new System.Drawing.Size(221, 63);
            this.comment.TabIndex = 2;
            // 
            // autoincrement
            // 
            this.autoincrement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autoincrement.Location = new System.Drawing.Point(123, 81);
            this.autoincrement.Name = "autoincrement";
            this.autoincrement.Size = new System.Drawing.Size(121, 20);
            this.autoincrement.TabIndex = 2;
            // 
            // checksum
            // 
            this.checksum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checksum.FormattingEnabled = true;
            this.checksum.Items.AddRange(new object[] {
            "0",
            "1",
            "DEFAULT"});
            this.checksum.Location = new System.Drawing.Point(123, 53);
            this.checksum.Name = "checksum";
            this.checksum.Size = new System.Drawing.Size(121, 21);
            this.checksum.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(-6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "CheckSum";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // raidType
            // 
            this.raidType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.raidType.FormattingEnabled = true;
            this.raidType.Items.AddRange(new object[] {
            "1",
            "DEFAULT",
            "RAID0",
            "STRIPED"});
            this.raidType.Location = new System.Drawing.Point(593, 85);
            this.raidType.Name = "raidType";
            this.raidType.Size = new System.Drawing.Size(121, 21);
            this.raidType.TabIndex = 1;
            // 
            // rowFormat
            // 
            this.rowFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rowFormat.FormattingEnabled = true;
            this.rowFormat.Items.AddRange(new object[] {
            "COMPACT",
            "DYNAMIC",
            "DEFAULT",
            "FIXED"});
            this.rowFormat.Location = new System.Drawing.Point(593, 53);
            this.rowFormat.Name = "rowFormat";
            this.rowFormat.Size = new System.Drawing.Size(121, 21);
            this.rowFormat.TabIndex = 1;
            // 
            // delaykeywrite
            // 
            this.delaykeywrite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.delaykeywrite.FormattingEnabled = true;
            this.delaykeywrite.Items.AddRange(new object[] {
            "0",
            "1",
            "DEFAULT"});
            this.delaykeywrite.Location = new System.Drawing.Point(593, 20);
            this.delaykeywrite.Name = "delaykeywrite";
            this.delaykeywrite.Size = new System.Drawing.Size(121, 21);
            this.delaykeywrite.TabIndex = 1;
            // 
            // engine
            // 
            this.engine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.engine.FormattingEnabled = true;
            this.engine.Items.AddRange(new object[] {
            "InnoDB",
            "MyISAM",
            "Archive"});
            this.engine.Location = new System.Drawing.Point(123, 16);
            this.engine.Name = "engine";
            this.engine.Size = new System.Drawing.Size(121, 21);
            this.engine.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(264, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Password";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(-6, 115);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(121, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Average row length";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.UseCompatibleTextRendering = true;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.Location = new System.Drawing.Point(491, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 17);
            this.label9.TabIndex = 0;
            this.label9.Text = "Raid type";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(264, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Coment";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(491, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 17);
            this.label8.TabIndex = 0;
            this.label8.Text = "Row format";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(491, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Delay key write";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(-6, 84);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(121, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Autoincrement";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.UseCompatibleTextRendering = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(-6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Engine";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(17, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Table name";
            // 
            // table
            // 
            this.table.Location = new System.Drawing.Point(86, 12);
            this.table.Name = "table";
            this.table.Size = new System.Drawing.Size(180, 20);
            this.table.TabIndex = 2;
            // 
            // outPut
            // 
            this.outPut.AutoSize = true;
            this.outPut.Location = new System.Drawing.Point(321, 15);
            this.outPut.Name = "outPut";
            this.outPut.Size = new System.Drawing.Size(39, 13);
            this.outPut.TabIndex = 4;
            this.outPut.Text = "Output";
            this.outPut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(12, 318);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(107, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Insert";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(135, 318);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(107, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Advanced";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // AlterTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 542);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.outPut);
            this.Controls.Add(this.table);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dg1);
            this.Controls.Add(this.label10);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AlterTableForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AlterForm";
            this.Load += new System.EventHandler(this.AlterTableForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dg1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox avgRowLen;
        private System.Windows.Forms.TextBox pwd;
        private System.Windows.Forms.TextBox comment;
        private System.Windows.Forms.TextBox autoincrement;
        private System.Windows.Forms.ComboBox checksum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox engine;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox raidType;
        private System.Windows.Forms.ComboBox rowFormat;
        private System.Windows.Forms.ComboBox delaykeywrite;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox table;
        private System.Windows.Forms.Label outPut;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewComboBoxColumn collation;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column7;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column5;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column8;
        private System.Windows.Forms.DataGridViewCheckBoxColumn unsigned;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}