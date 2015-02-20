/*
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
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace DBMS.core
{
    public delegate void KeywordFoundHandler(object sender, KeywordFoundEventArgs e);

    public class CodeCompletionProvider
    {

        public event KeywordFoundHandler KeywordFound;


        private ImageList imageList;
        private List<ICompletionData> completionData;
        private List<ICompletionData> preselectionData;

        public List<ICompletionData> PreselectionData
        {   set { preselectionData = value; }
            get { return preselectionData; }
        }
        private string lastWrittenWord;
        private IDBBrowser dBBrowser;
        private int fixedIndexIcons;
        public string LastWrittenWord
        {
            get { return lastWrittenWord; }
            set { lastWrittenWord = value; }
        }
        public List<ICompletionData> CompletionData
        {
            get { return completionData; }
            set { completionData = value; }
        }      
        public ImageList ImageList
        {
            get { return imageList; }
            set { imageList = value; }
        }
        public IDBBrowser DBBrowser
        {
            get { return dBBrowser; }
            set { dBBrowser = value; }
        }

        private Dictionary<string, Dictionary<string, string[]>> specificCompletionData;

        protected Dictionary<string, Dictionary<string,string[]>> SpecificCompletionData
        {
            get { return specificCompletionData; }
            set { specificCompletionData = value; }
        }

        public CodeCompletionProvider(ImageList iList, List<ICompletionData> data)
        {
            this.ImageList = iList;
            this.completionData = data;
            fixedIndexIcons = data.Count;
            specificCompletionData = new Dictionary<string, Dictionary<string,string[]>>();
        }
    

        public void AddCompletionData(ICompletionData[] data)
        {
            this.completionData.AddRange(data);
        }

        public virtual bool FindAndPreparePreselection(string lastWord,Keys keyData, char c, char lastChar, int carretPos)
        {
            return false;
        }

        public virtual int FindItemWithCurrentWord(string word)
        {
            if (word.Length < 1)
                return -1;
            int selectedIndex = -1;
            for (int i = 0; i < completionData.Count; i++)
            {
                char[] x = word.ToCharArray();
                if (completionData[i].Text.ToLower().StartsWith(word.ToLower()))
                {
                    if (selectedIndex == -1)
                    {
                        OnKeywordFound(this, new KeywordFoundEventArgs(word, completionData, selectedIndex));
                        selectedIndex = i;
                        return selectedIndex;
                    }
                }
            }
            return selectedIndex;
        }

        public void ClearSpecificCompletion()
        {
            specificCompletionData.Clear();
        }

        public void SetSpecificCompletion(string key, List<ICompletionData> data)
        {
            //specificCompletionData[key] = data;
        }
        

        protected virtual void OnKeywordFound(object sender, KeywordFoundEventArgs e)
        {
            if (this.KeywordFound != null)
                this.KeywordFound(sender, e);
        }

        public static ICompletionData CompareData(ICompletionData x, ICompletionData y)
        {
            return string.Compare(x.Description, y.Description)>0?x:y;
        }

    }
}
