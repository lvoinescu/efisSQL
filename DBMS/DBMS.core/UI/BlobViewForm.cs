/*
    efisSQL - data base management tool
    Copyright (C) 2011  Lucian Voinescu

    This file is part of efisSQL

    efisSQL is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    efisSQL is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with efisSQL. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;

namespace DBMS.core
{
    public partial class BlobViewForm : Form
    {
        private static readonly string[] HexStringTable = new string[]
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
            "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
            "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
            "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
            "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
        };
        private byte[] data;
        private bool cancelForm;

        private int rowIndex;

        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }
        private int columnIndex;

        public int ColumnIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }
       
        public bool CancelForm
        {
            get { return cancelForm; }
            set { cancelForm = value; }
        }
      
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
       
        public BlobViewForm(int row, int column, bool enableSetting)
        {
            this.rowIndex = row;
            this.columnIndex = column;
            InitializeComponent();
            cancelForm = true;
            button1.Enabled = enableSetting;
            button4.Enabled = enableSetting;
        }

        public BlobViewForm(byte[] data, int row, int column, bool enableSetting)
        {
            try
            {
                this.data = data;
                this.rowIndex = row;
                this.columnIndex = column;
                this.cancelForm = true;
                InitializeComponent();
                button1.Enabled = enableSetting;
                button4.Enabled = enableSetting;
                textBox1.AppendText(Encoding.Default.GetString(data));
            }
            catch (Exception ex)
            {
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                panel1.Visible = true;
                textBox1.Visible = false;
                try
                {
                    MemoryStream ms = new MemoryStream(data);
                    Image image = Image.FromStream(ms, true);
                    if (pictureBox1.Width < image.Width || pictureBox1.Height < image.Height)
                        checkBox1.Checked = true;
                    pictureBox1.Image = image;
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                panel1.Visible = false;
                textBox1.Visible = true;
            }
        }

        private unsafe void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            if(radioButton1.Checked)
                opf.Filter = "All files|*.*|Text files|*.txt|Doc files|*.doc";
            else
                opf.Filter = "All files|*.*|Png|*.png|Bmp|*.bmp|JPEG|*.jpeg|jpg|*.jpg";
            opf.ShowDialog();
            if (opf.FileName == "")
                return;
            Image imageIn;
            FileInfo fi = new FileInfo(opf.FileName);
            int bufferSize = 8192;

            IntPtr bufferStr = Marshal.AllocHGlobal(new IntPtr(fi.Length));
            UnmanagedMemoryStream memoryStream = new UnmanagedMemoryStream((byte*)bufferStr.ToPointer(), fi.Length, fi.Length, FileAccess.ReadWrite);
            byte[] buffer = new byte[bufferSize];
            BinaryReader reader = new BinaryReader(new FileStream(opf.FileName, FileMode.Open,FileAccess.Read ,FileShare.Read));
            int charsRead = reader.Read(buffer, 0, bufferSize);
            while (charsRead > 0)
            {
                memoryStream.Write(buffer, 0, charsRead);
                charsRead = reader.Read(buffer, 0, bufferSize);
            }
            memoryStream.Close();
            if (radioButton2.Checked)
            {
                try
                {
                    imageIn = Image.FromFile(opf.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Not a valid image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                pictureBox1.Image = imageIn;
                pictureBox1.Size = imageIn.Size;
                textBox1.Clear();
            }
            UnmanagedMemoryStream memoryStreamRead = new UnmanagedMemoryStream((byte*)bufferStr.ToPointer(), fi.Length, fi.Length, FileAccess.Read);
            data = new byte[memoryStreamRead.Length];
            memoryStreamRead.Read(this.data, 0, (int)memoryStreamRead.Length);
            memoryStreamRead.Close();
            //textBox1.AppendText(Encoding.Default.GetString(data));
            if (IsValidImage(data))
            {
                radioButton2.Checked = true;
            }
            panel1.Visible = true;
            Marshal.FreeHGlobal(bufferStr);   
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cancelForm = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                this.data = Encoding.Default.GetBytes(textBox1.Text);
            cancelForm = false;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.SizeMode =  PictureBoxSizeMode.StretchImage;
            }
            else
            {
                pictureBox1.Dock = DockStyle.None;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetText();
        }

        private void SetText()
        {
            if (data == null)
                return;
            switch (comboBox1.Text.ToLower())
            {
                case "utf7":
                    textBox1.Text = Encoding.UTF7.GetString(data);
                    break;
                case "utf8":
                    textBox1.Text = Encoding.UTF8.GetString(data);
                    break;
                case "utf32":
                    textBox1.Text = Encoding.UTF32.GetString(data);
                    break;
                case "ascii":
                    textBox1.Text = Encoding.ASCII.GetString(data);
                    break;
                case "unicode":
                    textBox1.Text = Encoding.Unicode.GetString(data);
                    break;
                case "default":
                    textBox1.Text = Encoding.Default.GetString(data);
                    break;
                default:
                    textBox1.Text = Encoding.Default.GetString(data);
                    break;
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            data = null;
        }

        private  void button5_Click(object sender, EventArgs e)
        {

            SaveFileDialog sFile = new SaveFileDialog();
            sFile.ShowDialog();
            if (sFile.FileName == "")
                return;
            BinaryWriter bWriter = new BinaryWriter(new FileStream(sFile.FileName,FileMode.OpenOrCreate,FileAccess.Write));
            bWriter.Write(data);
            bWriter.Close();
        }

        private bool IsValidImage(byte [] data)
        {
            try
            {

                if (data.Length > 0)
                {
                    byte[] header = new byte[30]; // Change size if needed.
                    string[] imageHeaders = new string[]
                    {
                        "BM",   //BMP
                        "GIF",  //GIF
                        Encoding.ASCII.GetString(new byte[]{137, 80, 78, 71}),//PNG
                        "MM\x00\x2a", //TIFF
                        "II\x2a\x00" //TIFF
                    };
                    Array.Copy(data, header, 30);
                    bool isImageHeader = false;
                    for (int i = 0; i < imageHeaders.Length && !isImageHeader; i++)
                    {
                        isImageHeader = Encoding.ASCII.GetString(header).StartsWith(imageHeaders[i]);
                    }
                    if (isImageHeader == false)
                    {
                        byte[] soi = new byte[2] { data[0], data[1] };
                        byte[] jfif = new byte[2] { data[2], data[3] };
                        //return soi == 0xd8ff && (jfif == 0xe0ff || jfif == 57855);
                        return ((soi[0] == 0xff && soi[1] == 0xd8) && ((jfif[0] == 0xff && jfif[1] == 0xe0) || (jfif[0] == 0xff && jfif[1] == 0xe1)));
                    }                    

                return isImageHeader;
                }

                return false;
            }
            catch { return false; }
            finally
            {
            }
        }

        private void BlobViewForm_Load(object sender, EventArgs e)
        {
            if(IsValidImage(data))
                radioButton2.Checked=true;
        }

        public static string ToHex(byte[] value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (value != null)
            {
                foreach (byte b in value)
                {
                    stringBuilder.Append(HexStringTable[b]);
                }
            }

            return stringBuilder.ToString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                SetText();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                {
                    textBox1.Text = ToHex(data);
                }

        }
        
    }
}