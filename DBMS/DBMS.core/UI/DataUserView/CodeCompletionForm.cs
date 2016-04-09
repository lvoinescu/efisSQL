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
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor;
using System.Text.RegularExpressions;
namespace DBMS.core
{
    public partial class CodeCompletionForm : Form
    {
        private Control parentControl; 
        public Control ParentControl
        {
            get { return parentControl; }
            set { parentControl = value; }
        }
        private int carretInitialPosition;
        private List<ICompletionData> completionData;
        private int selectedIndex;
        private TextEditorControl textEditor;
        public int SelectedIndex
        {
            get { return selectedIndex; }
        }
        Form parentForm;
        private string writtenWord;
        public List<ICompletionData> CompletionData
        {
            get { return completionData; }
            set { completionData = value; }
        }
        private char prevChar = ' ';
        CodeCompletionProvider provider;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern uint MapVirtualKeyW(uint uCode, uint uMapType);
        private int noOfItemDisplayed = 0;
            
        private char KeyToChar(Keys key)
        {
            return unchecked((char)MapVirtualKeyW((uint)key, MAPVK_VK_TO_CHAR)); // Ignore high word.  
        }

        private const uint MAPVK_VK_TO_CHAR = 2;  

       

        public CodeCompletionForm(Form parentForm, TextEditorControl textEditor, CodeCompletionProvider provider)
        {
            this.provider = provider;
            provider.KeywordFound+=new KeywordFoundHandler(provider_KeywordFound);
            writtenWord = "";
            this.parentForm = parentForm;
            this.textEditor = textEditor;
            InitializeComponent();
            codeCompletionListView.SmallImageList = provider.ImageList;
            codeCompletionListView.Dock = DockStyle.Fill;
            imageList1.Images.Clear();
            foreach (Image img in provider.ImageList.Images)
            {
                imageList1.Images.Add(img);
            }

            completionData = provider.CompletionData;
            this.Controls.Add(codeCompletionListView);
            foreach (ICompletionData data in completionData)
            {
                ListViewItem item = new ListViewItem(data.Text, data.ImageIndex);
                item.Tag = data;
                codeCompletionListView.Items.Add(item);
            }
            if(codeCompletionListView.Items.Count>0)
                noOfItemDisplayed = codeCompletionListView.Bounds.Height / codeCompletionListView.Items[0].Bounds.Height;
        }

        protected void provider_KeywordFound(object sender, KeywordFoundEventArgs e)
        {
            this.SuspendLayout();
            codeCompletionListView.Items.Clear();
            completionData = e.Preselection;
            foreach (ICompletionData data in e.Preselection)
            {
                ListViewItem item = new ListViewItem(data.Text, data.ImageIndex);
                item.Tag = data;
                codeCompletionListView.Items.Add(item);
            }
            if (!Visible)
                AutomaticShow();
            if (e.IndexInPreselection > -1)
            {
                selectedIndex = e.IndexInPreselection;
                codeCompletionListView.Items[e.IndexInPreselection].Selected = true;
                codeCompletionListView.EnsureVisible(e.IndexInPreselection);
            }
            this.ResumeLayout();
        }

        public void AutomaticShow()
        {
            writtenWord = "";
            SetLocation();
            carretInitialPosition = textEditor.ActiveTextAreaControl.Caret.Offset;
            selectedIndex = -1;
            this.Show(parentForm);
            this.ParentControl.Focus();
        }

        public void ShowAtCarret()
        {
            if (completionData == null)
                return;
            if (!Visible)
            {
                codeCompletionListView.Items.Clear();
                foreach (ICompletionData data in completionData)
                {
                    ListViewItem item = new ListViewItem(data.Text, data.ImageIndex);
                    item.Tag = data;
                    codeCompletionListView.Items.Add(item);
                }
                this.SetLocation();
                this.Show(parentForm);
                parentControl.Focus();
            }
        }
        
        protected virtual void SetLocation()
        {
            TextArea textArea = textEditor.ActiveTextAreaControl.TextArea;
            TextLocation caretPos = textArea.Caret.Position;

            int xpos = textArea.TextView.GetDrawingXPos(caretPos.Y, caretPos.X);
            int rulerHeight = textArea.TextEditorProperties.ShowHorizontalRuler ? textArea.TextView.FontHeight : 0;
            Point pos = new Point(textArea.TextView.DrawingPosition.X + xpos,
                                  textArea.TextView.DrawingPosition.Y + (textArea.Document.GetVisibleLine(caretPos.Y)) * textArea.TextView.FontHeight
                                  - textArea.TextView.TextArea.VirtualTop.Y + textArea.TextView.FontHeight + rulerHeight);

            Point location = textEditor.ActiveTextAreaControl.PointToScreen(pos);

            // set bounds
            Size drawingSize = this.Size;
            Rectangle bounds = new Rectangle(location, drawingSize);
            Rectangle workingScreen = Screen.GetWorkingArea(parentForm);
            if (!workingScreen.Contains(bounds))
            {
                if (bounds.Right > workingScreen.Right)
                {
                    bounds.X = workingScreen.Right - bounds.Width;
                }
                if (bounds.Left < workingScreen.Left)
                {
                    bounds.X = workingScreen.Left;
                }
                if (bounds.Top < workingScreen.Top)
                {
                    bounds.Y = workingScreen.Top;
                }
                if (bounds.Bottom > workingScreen.Bottom)
                {
                    bounds.Y = bounds.Y - bounds.Height - textEditor.ActiveTextAreaControl.TextArea.TextView.FontHeight;
                    if (bounds.Bottom > workingScreen.Bottom)
                    {
                        bounds.Y = workingScreen.Bottom - bounds.Height;
                    }
                }
            }
            Bounds = bounds;
        }

        public void SelectNextItem()
        {
            if (selectedIndex == -1)
            {
                if (codeCompletionListView.Items.Count > 0)
                {
                    selectedIndex = 0;
                    codeCompletionListView.Items[0].Selected = true;
                }
                return;
            }
            codeCompletionListView.Items[selectedIndex].BackColor = codeCompletionListView.BackColor;
            if (selectedIndex < codeCompletionListView.Items.Count - 1)
            {
                codeCompletionListView.Items[++selectedIndex].Selected = true;
                codeCompletionListView.Items[selectedIndex].BackColor = Color.SteelBlue;
                codeCompletionListView.EnsureVisible(selectedIndex);
            }
        }

        public void SelectPrevItem()
        {
            if (selectedIndex == -1)
            {
                if (codeCompletionListView.Items.Count > 0)
                {
                    codeCompletionListView.Items[0].Selected = true;
                    selectedIndex = 0;
                }
                return;
            }
            codeCompletionListView.Items[selectedIndex].BackColor = codeCompletionListView.BackColor;
            if (selectedIndex > 0)
            {
                codeCompletionListView.Items[--selectedIndex].Selected = true;
                codeCompletionListView.Items[selectedIndex].BackColor = Color.SteelBlue;
                codeCompletionListView.EnsureVisible(selectedIndex);
            }
        }

        private void PageUp()
        {
            int i = codeCompletionListView.TopItem.Index;
            if (i - noOfItemDisplayed -1>=0)
                codeCompletionListView.EnsureVisible(i - noOfItemDisplayed-1);
            else
                codeCompletionListView.EnsureVisible(0);
        }

        private void PageDown()
        {
            int i = codeCompletionListView.TopItem.Index;
            if (i + 2 * noOfItemDisplayed - 1 < codeCompletionListView.Items.Count)
                codeCompletionListView.EnsureVisible(i +2* noOfItemDisplayed-1);
            else
                codeCompletionListView.EnsureVisible(codeCompletionListView.Items.Count-1);
        }

        private void InsertSelectedItem()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DeleteLastChar()
        {
            if (writtenWord.Length > 0)
            {
                writtenWord = writtenWord.Remove(writtenWord.Length - 1);
                FindItemInPreselectinStartingWith(writtenWord);
            }
            else
            {
                selectedIndex = -1;
                HideIt();
            }
        }

        public void HideIt()
        {
            writtenWord="";
            this.Hide();
        }

        //return true to supress insertion in textArea
        public bool ProcessKey(Keys keyData)
        {
            bool noSpace = false;
            string word = "";
            int carretOffsetBefore = textEditor.ActiveTextAreaControl.Caret.Offset;
            char keyChar = KeyToChar(keyData);
            switch (keyData)
            {
                case Keys.Space:
                case Keys.OemPeriod:
                    if (!this.Visible)
                    {

                        word = GetLastWrittenWord(out noSpace);
                        if (word != "")
                        {
                            if (provider.FindAndPreparePreselection(word, keyData, keyChar, '\0', carretOffsetBefore))
                            {
                                carretInitialPosition = carretOffsetBefore + 1;
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        InsertCurrentWord();
                        return true;
                    }
                    return false;
                case Keys.Control | Keys.Space:
                    char lastChar = ' ';
                    noSpace = false;
                    if(carretOffsetBefore>0)
                        lastChar = textEditor.ActiveTextAreaControl.Document.TextContent[carretOffsetBefore - 1];

                    word = GetLastWrittenWord(out noSpace);
                    if (noSpace)
                    {
                        carretInitialPosition = carretOffsetBefore - word.Length; ;
                        writtenWord = word;
                    }
                    if(word.EndsWith("."))
                        word = word.TrimEnd('.');
                    bool y = provider.FindAndPreparePreselection(word, keyData, keyChar, lastChar, carretOffsetBefore);
                    completionData = provider.PreselectionData;
                    if (y)
                    {
                        if (lastChar == '.')
                        {
                            writtenWord = "";
                            carretInitialPosition = carretOffsetBefore;
                        }
                        else
                        {
                            writtenWord = word;
                            carretInitialPosition = carretOffsetBefore - word.Length;
                        }
                        return true;
                    }

                    ShowAtCarret();
                 return true;
                case Keys.Escape:
                    this.HideIt();
                    return true;
                case Keys.Back:
                    this.DeleteLastChar();
                    return false;
                case Keys.Enter:
                case Keys.Tab:
                    if (this.Visible)
                    {
                        InsertCurrentWord();
                        return true;
                    }
                    else
                        return false;
                case Keys.Up:
                    if (Visible)
                    {
                        SelectPrevItem();
                        return true;
                    }
                    return false;
                case Keys.Down:
                    if (Visible)
                    {
                        SelectNextItem();
                        return true;
                    }
                    return false;
                case Keys.PageDown:
                    PageDown();
                    return true;
                case Keys.PageUp:
                    PageUp();
                    return true;
            }

            if (this.Visible)
            {
                char c = (char)keyData;
                if (c >= 32 && c < 126)
                {
                    writtenWord += c.ToString();
                    return FindItemInPreselectinStartingWith(writtenWord);
                }
            }
            return false;

        }

        public void InsertCurrentWord()
        {
            if (codeCompletionListView.SelectedItems.Count > 0)
            {
                int i = codeCompletionListView.SelectedIndices[0];
                ICompletionData data = (ICompletionData)codeCompletionListView.Items[i].Tag;
                if(data.Description!=null)
                {
                    if (data.Description == "table" || data.Description == "column")
                        InsertWord("`" + data.Text + "`");

                }
                else
                    InsertWord(data.Text);

            }
        }

        
        private void InsertWord(String completeWord)
        {
            if (selectedIndex > -1)
            {
                if(writtenWord.Length>0)
                    textEditor.Document.Remove(carretInitialPosition, writtenWord.Length);
                textEditor.ActiveTextAreaControl.TextArea.InsertString(completeWord);
            }
            writtenWord = "";
            codeCompletionListView.SelectedItems.Clear();
            HideIt();
        }


        protected bool FindItemInPreselectinStartingWith(string word)
        {
            if (word.Length < 1)
                return false;
            selectedIndex = -1;
            for (int i = 0; i < codeCompletionListView.Items.Count; i++)
            {
                char[] x = word.ToCharArray();
                if (codeCompletionListView.Items[i].Text.ToLower().StartsWith(word.ToLower()))
                {
                    if (selectedIndex == -1)
                    {
                        codeCompletionListView.Items[i].Selected = true;
                        selectedIndex = i;
                        codeCompletionListView.EnsureVisible(i);
                        break;
                    }
                }
            }
            if (selectedIndex == -1)
            {
                this.HideIt();
                return false;
            }
            return false;
        }
     

        #region controls

        private string GetLastWrittenWord(out bool noSpaceAfter)
        {
            string rez="";
            noSpaceAfter = true;
            int width =400;
            int x = textEditor.ActiveTextAreaControl.Caret.Offset;
            string text="";
            if (x - width < 0)
                text = textEditor.ActiveTextAreaControl.Document.TextContent;
            else
                text = textEditor.Document.TextContent.Substring(x - width, width);
            if (text.EndsWith(" "))
                noSpaceAfter = false;
            text = text.TrimEnd(' ');
            if(text.Length>1)
            {
                char  last =text[text.Length-1];
                if(last=='\'' || last=='"' || last=='`')
                {
                    text = text.TrimEnd(last);
                    int li = text.LastIndexOf(last);
                    if(li>-1)
                    {
                        rez = text.Substring(li+1,text.Length-1 - li);
                        return rez;
                    }
                }
            }
            return Regex.Match(text, "[^\\s]*$").ToString();
        }

        private void codeCompletionListView_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewItem item = codeCompletionListView.GetItemAt(10, e.Y);
            if (item != null)
            {
                selectedIndex = item.Index;
                codeCompletionListView.Items[selectedIndex].Selected = true;
            }


        }

        private void codeCompletionListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (codeCompletionListView.SelectedItems.Count > 0)
            {
                InsertCurrentWord();
            }
        }

        private void CodeCompletionForm_Load(object sender, EventArgs e)
        {

        }
  
        private void codeCompletionListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            if (e.Item.Selected)
                e.Graphics.FillRectangle(Brushes.SteelBlue, e.Bounds);
            Point p = e.Bounds.Location;
            p.Offset(17, 0);
            if (e.Item.ImageIndex > -1)
                e.Graphics.DrawImage(e.Item.ImageList.Images[e.Item.ImageIndex], e.Bounds.Location);
            e.Graphics.DrawString(e.Item.Text, this.Font, Brushes.Black,p );
        }



        #endregion




    }
}