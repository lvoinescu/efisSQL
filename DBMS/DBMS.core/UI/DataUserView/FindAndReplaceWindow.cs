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
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using System.Text.RegularExpressions;

namespace DBMS.core
{
    public partial class FindAndReplaceWindow : Form
    {
        private TextEditorControl editor;
        private bool replaceMode;
        private int currentPosition = 0;
        private int initialPosition;
        private Regex reg;
        private RegexOptions options;
        public bool ReplaceMode
        {
            get { return replaceMode; }
            set { replaceMode = value; }
        }

        private void SetMode(bool replaceMode)
        {
            btnReplace.Visible = replaceMode;
            btnReplaceAll.Visible = replaceMode;
            txtReplaceWith.Visible = replaceMode;
            lblReplaceWith.Visible = replaceMode;
        }

        private static FindAndReplaceWindow instance;

        public static void ShowWindow(TextEditorControl editor, bool replace, string word)
        {
            if (instance == null)
            {
                instance = new FindAndReplaceWindow(editor, replace);
               
            }
            else
            {
                instance.editor = editor;
                instance.replaceMode = replace;
            }
            instance.txtLookFor.Text = editor.ActiveTextAreaControl.SelectionManager.SelectedText;
            string text = replace ? "Find & replace" : "Find";
            instance.Text = text;
            instance.SetMode(replace);
            instance.currentPosition = editor.ActiveTextAreaControl.Caret.Offset;
            instance.initialPosition = instance.currentPosition;
            instance.startPos = instance.initialPosition;
            instance.endPos = instance.initialPosition;
            instance.ShowInTaskbar = false;
            instance.Visible = true;
        }

        protected FindAndReplaceWindow(TextEditorControl editor, bool replace)
        {
            InitializeComponent();
            this.editor = editor;
            this.replaceMode = replace;
            string text = replace ? "Find & replace" : "Find";
            this.Text = text;
            this.SetMode(replace);
            this.currentPosition = editor.ActiveTextAreaControl.Caret.Offset;
            this.initialPosition = this.currentPosition;
            this.startPos = this.initialPosition;
            this.endPos = this.initialPosition;
        }


        private void btnFindNext_Click(object sender, EventArgs e)
        {
            
            string word = txtLookFor.Text;
            SetOptions();
            int index = FindForward( word, true);
            if (index > -1)
            {
                TextLocation locStart = editor.ActiveTextAreaControl.Document.OffsetToPosition(index);
                TextLocation locEnd = editor.ActiveTextAreaControl.Document.OffsetToPosition(index + word.Length);
                editor.ActiveTextAreaControl.SelectionManager.SetSelection(locStart, locEnd);
                editor.ActiveTextAreaControl.ScrollTo(locStart.Line, locStart.Column);
            }
        }

        private void SetOptions()
        {
            if (!chkMatchCase.Checked)
                options = RegexOptions.IgnoreCase;
            else
                options = RegexOptions.None;
        }

  
        bool secondPart = false;
        private int startPos, endPos, lastIndexFound;
        int bufferSize = 1024;

        private int FindForward(string word, bool holeWordOnly)
        {
            lastIndexFound = -1;
            if (word.Length > bufferSize)
                return -1;
            bool gasit = false;
            for (; ; )
            {
                endPos = startPos + bufferSize + word.Length < editor.Document.TextLength ? startPos + bufferSize + word.Length : editor.Document.TextLength;
                if (startPos >= currentPosition && secondPart)
                {
                    MessageBox.Show("Find reached starting point of searching", Application.ProductName, MessageBoxButtons.OK);
                    secondPart = false;
                    startPos = currentPosition;
                    return -1;
                }
                if (startPos >= editor.Document.TextLength)
                {
                    secondPart = true;
                    startPos = 0;
                }
                string aux = editor.Document.GetText(startPos, endPos - startPos);
                string patter = chkMatchWholeWord.Checked ? "\\b" + word + "\\b" : word;
                Match m = Regex.Match(aux, patter, options);
                if (!m.Success)
                {
                    gasit = false;
                    startPos += bufferSize;

                    return FindForward(word, holeWordOnly);
                }
                else
                {
                    gasit = true;
                    lastIndexFound = startPos + m.Index;
                    startPos += m.Index + 1;
                    if (startPos > endPos)
                    {
                        startPos += bufferSize;
                        if (startPos >= editor.Document.TextLength)
                        {
                            secondPart = true;
                            startPos = 0;
                        }
                    }
                    return lastIndexFound;
                }
            }
            return lastIndexFound;
        }



        private void FindAndReplaceWindow_Load(object sender, EventArgs e)
        {
            if(chkMatchCase.Checked)
                options = RegexOptions.RightToLeft;
            else
                options = RegexOptions.RightToLeft|RegexOptions.IgnoreCase;
        }

        private void chkMatchWholeWord_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            int index = 0;
            string word = txtLookFor.Text;
            string replace = txtReplaceWith.Text;
            SetOptions();
            while (index >= 0)
            {
                index = FindForward(word, true);
                if(index>=0)
                    editor.Document.Replace(index, word.Length, replace);
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            int index = 0;
            string word = txtLookFor.Text;
            string replace = txtReplaceWith.Text;
            SetOptions();
            index = FindForward(word, true);
                if (index >= 0)
                    editor.Document.Replace(index, word.Length, replace);
        }

        private void FindAndReplaceWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
            instance.Visible = false;
        }

        private void FindAndReplaceWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

    }
}