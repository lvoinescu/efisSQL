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
using System.Text;
using DBMS.core;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System.Text.RegularExpressions;
namespace DBMS.MySQL
{
    public class MySQLCodeCompletionProvider : CodeCompletionProvider
    {






        private string currentDataBase = null;

        public string CurrentDataBase
        {
            get { return currentDataBase; }
            set { currentDataBase = value; }
        }



        public MySQLCodeCompletionProvider(ImageList iList, List<ICompletionData> data):base(iList,data)
        {
            PreselectionData = this.CompletionData;
        }
     
        public override int FindItemWithCurrentWord(string word)
        {
            if (word.Length < 1)
            {
                return -1;
            }
            int selectedIndex = -1;
            for (int i = 0; i < this.CompletionData.Count; i++)
            {
                if (CompletionData[i].Text.ToLower().StartsWith(word.ToLower()))
                {
                    if (selectedIndex == -1)
                    {
                        selectedIndex = i;
                        return selectedIndex;
                    }
                }
            }
            return selectedIndex;
        }


        public override bool FindAndPreparePreselection(string lastWord, Keys keyData, char c, char lastChar, int carretPos)
        {
            string word = lastWord.TrimEnd('.');
            if (word!="" && WordIsDatabase(word))
            {
                PreselectionData = GenerateDataBaseCompletion(currentDataBase);
                this.OnKeywordFound(this, new KeywordFoundEventArgs(word, PreselectionData, -1, c, carretPos));
                return true;
            }  

            if (c == ' ')
            {
                if (lastWord.ToLower() == "from" || lastWord.ToLower() == "join" || lastWord.ToLower() == "on")
                {
                    PreselectionData = GenerateDataBaseCompletion(currentDataBase);
                    if (PreselectionData != null)
                    {
                        this.OnKeywordFound(this, new KeywordFoundEventArgs(lastWord, PreselectionData, -1, c, carretPos));
                        return true;
                    }
                }
                return true;
            }

            if (c == '.')
            {
                if (WordIsDatabase(lastWord))
                {
                    PreselectionData = GenerateDataBaseCompletion(currentDataBase);
                    this.OnKeywordFound(this, new KeywordFoundEventArgs(lastWord, PreselectionData, -1, c, carretPos));
                    return true;
                }

                if (WordIsTable(lastWord))
                {
                    string table = lastWord.TrimEnd('\'', '"', '`').TrimStart('\'', '"', '`');
                    PreselectionData = GenerateTableCompletion(currentDataBase, table);
                    int index = FindItemWithCurrentWord(lastWord);
                    this.OnKeywordFound(this, new KeywordFoundEventArgs(lastWord, PreselectionData, index, c, carretPos));
                    return true;
                }
                return true;
            }

            if (keyData == (Keys.Control | Keys.Space))
            {
                bool isKeyWord = false;
                    if (WordIsDatabase(word))
                    {
                        PreselectionData = GenerateDataBaseCompletion(currentDataBase);
                        isKeyWord = true;
                    }

                    string w1 = word.TrimEnd('.');
                    string table = w1.TrimEnd('\'', '"', '`').TrimStart('\'', '"', '`');
                    if (WordIsTable(table))
                    {
                        PreselectionData = GenerateTableCompletion(currentDataBase,table);
                        isKeyWord = true;
                    }
                    else
                    {
                        PreselectionData = new List<ICompletionData>(CompletionData.ToArray());
                        //if (SpecificCompletionData.ContainsKey("database"))
                        //{
                            List<ICompletionData> tables = GenerateDataBaseCompletion(currentDataBase);//  (List < ICompletionData >) SpecificCompletionData["database"];
                            PreselectionData.AddRange(tables.ToArray());
                        //}
                    }
                    if (isKeyWord)
                    {
                        this.OnKeywordFound(this, new KeywordFoundEventArgs(lastWord, PreselectionData, -1, c, carretPos));
                        return true;
                    }
                int index =-1;
                if (lastChar == ' ' || lastChar == '.' ||lastChar=='\t')
                {
                }
                else
                {
                    index = FindItemWithCurrentWord(lastWord);
                }
                if (index == -1)
                {
                    PreselectionData = new List<ICompletionData>(CompletionData.ToArray());
                    //if (SpecificCompletionData.ContainsKey("database"))
                    //{
                        List<ICompletionData> tables =  GenerateDataBaseCompletion(currentDataBase);//(List<ICompletionData>)SpecificCompletionData["database"];
                        PreselectionData.AddRange(tables.ToArray());
                    //}
                    return false;
                }
                this.OnKeywordFound(this, new KeywordFoundEventArgs(lastWord, PreselectionData, index, c, carretPos));
                return true;
            }

            PreselectionData = this.CompletionData;
            return false;
        }

        protected bool WordIsDatabase(string word)
        {
            if (currentDataBase == null)
                return false;
            if (word == currentDataBase)
                return true;
            string pattern = currentDataBase;
            if (word.Length > 2)
            {
                pattern = "[\\\'\\\"\\`]" + pattern + "[\\\'\\\"\\`]";
            }
            return Regex.Match(word, pattern).Success;
        }

        protected bool WordIsTable(string word)
        {
            if (currentDataBase == null)
                return false;
            string aux = word.TrimEnd('\'', '"', '`');
            aux = aux.TrimStart('\'', '"', '`');
            if (SpecificCompletionData[currentDataBase].ContainsKey(aux.ToLower()))
                return true;
            return false;
        }
        
        protected List<ICompletionData> GenerateCompletionData(string key)
        {
            if (SpecificCompletionData.ContainsKey(key))
            {
                //List<ICompletionData> data = (List<ICompletionData>) SpecificCompletionData[key];
                //return data;
            }
            return null;
        }

        public void AddDataBase(string database)
        {
            SpecificCompletionData[database] = new Dictionary<string,string[]>();
        }
  
        public void AddTableCompletionToDB(string database, string[] tables)
        {
            if(!SpecificCompletionData.ContainsKey(database))
                SpecificCompletionData[database] = new Dictionary<string,string[]>();
            for (int i = 0; i < tables.Length; i++)
                if (!SpecificCompletionData[database].ContainsKey(tables[i]))
                    SpecificCompletionData[database][tables[i]] = new string[] { };
        }

        public void AddColumnCompletionToTableDB(string database, string table, string[] columns)
        {
            if (!SpecificCompletionData.ContainsKey(database))
                SpecificCompletionData[database] = new Dictionary<string,string[]>();
            if (!SpecificCompletionData[database].ContainsKey(table))
                SpecificCompletionData[database][table] = new string[] { };
            SpecificCompletionData[database][table] = columns;
        }


        private List<ICompletionData> GenerateDataBaseCompletion(string database)
        {
            
            List<ICompletionData> ret = new List<ICompletionData>();
            if (database == null)
                return ret;
            if (!SpecificCompletionData.ContainsKey(database))
                return null;
            foreach (string table in SpecificCompletionData[database].Keys)
            {
                ret.Add(new DefaultCompletionData(table,"table",2));
            }
            return ret;
        }

        private List<ICompletionData> GenerateTableCompletion(string database, string table)
        {
            List<ICompletionData> ret = new List<ICompletionData>();
            if (!SpecificCompletionData.ContainsKey(database))
                return null;
            if (!SpecificCompletionData[database].ContainsKey(table))
                return null;
            foreach (string column in SpecificCompletionData[database][table])
            {
                ret.Add(new DefaultCompletionData(column, "column", 3));
            }
            return ret;
        }



    }
}

