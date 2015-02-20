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
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace DBMS.core
{
    public class KeywordFoundEventArgs
    {
        private string keyword;
        private List<ICompletionData> preselection;
        private char tiggerChar;


        private int carretOffset;

        public int CarretOffset
        {
            get { return carretOffset; }
            set { carretOffset = value; }
        }
        public char TiggerChar
        {
            get { return tiggerChar; }
            set { tiggerChar = value; }
        }
        public List<ICompletionData> Preselection
        {
            get { return preselection; }
            set { preselection = value; }
        }
        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }
        public KeywordFoundEventArgs(string keyword)
        {
            this.keyword = keyword;
        }
        private int indexInPreselection;

        public int IndexInPreselection
        {
            get { return indexInPreselection; }
            set { indexInPreselection = value; }
        }
        public KeywordFoundEventArgs(string keyword, List<ICompletionData> possibleWords, int indexInPreselection)
        {
            this.indexInPreselection = indexInPreselection;
            this.preselection = possibleWords;
            this.keyword = keyword;
        }
        public KeywordFoundEventArgs(string keyword, List<ICompletionData> possibleWords, int indexInPreselection, char triggerChar, int carretOffset)
        {
            this.indexInPreselection = indexInPreselection;
            this.tiggerChar = triggerChar;
            this.preselection = possibleWords;
            this.keyword = keyword;
            this.carretOffset = carretOffset;
        }
    }
}
