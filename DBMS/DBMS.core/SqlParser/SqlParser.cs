/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 12/6/2014
 * Time: 1:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DBMS.core.SqlParser
{
	
	
	
	/// <summary>
	/// Description of SqlParsers.
	/// </summary>
	public class SqlParser
	{
		StringBuilder restCommandBuffer ;
		StringBuilder mainCommandBuffer;
		private Stream inputStream;
		

		private long fileSize;
		private string analized;
		private bool ReadOnly = true;
		private int readQueries;
		Regex reg = new Regex(@"(delimiter\s[^\s\r\n]+[ \r\n]+)", RegexOptions.IgnoreCase);
		private string delStr, aux, sh;
		private string delimiter = ";";
		int _bufferSize = 65536;
		Encoding encoding = Encoding.GetEncoding(1257);

		
		public event QueriesReadDelegate QueriesRead;
		
		public SqlParser(){
			
		}
		
		public SqlParser(Stream inputStream)
		{
			this.inputStream = inputStream;
		}
		
		public Stream InputStream {
			get { return inputStream; }
			set { inputStream = value; }
		}		
		public void ReadQueries()
		{
			
			if (inputStream==null)
				throw new IOException("Null imput stream");
			long totalBytes=0;
			// don't know what's the problem with UTF8 BOM or without BOM...
			
			mainCommandBuffer = new StringBuilder(64*1024*1024);
			restCommandBuffer = new StringBuilder(64*1024*1024);
			BinaryReader binaryReader = new BinaryReader(inputStream, encoding);
			
			byte[] fileContents = new byte[_bufferSize];
			fileSize = inputStream.Seek(0, SeekOrigin.End);
			inputStream.Seek(0, SeekOrigin.Begin);
			int charsRead = binaryReader.Read(fileContents, 0, _bufferSize);
			if (charsRead == 0)
				return;
			totalBytes = charsRead;
			bool waitingForEnclose = false;
			char waitingChar = '\'';
			bool lastCharWasSpecial = false;
			bool waitingForDelimiter = false;
			readQueries = 0;
			

			while (charsRead > 0)
			{
				List<string> rez = GetQuery(fileContents,  0, charsRead, ref waitingForEnclose, ref waitingChar, ref delimiter, ref waitingForDelimiter, ref lastCharWasSpecial);
				readQueries += rez.Count;
				
				if(QueriesRead !=null)
					this.QueriesRead(this, new QueriesReadEventArgs(rez, fileSize, totalBytes));
				
				charsRead = binaryReader.Read(fileContents, 0, _bufferSize);
				totalBytes += charsRead;
			}
		}
		
		private List<string> GetQuery(byte[] s, int startindex, int len, ref bool waitingForEnclose, ref char waitingChar,ref string delimiter, ref bool waitingForDelimiter, ref bool lastCharWasSpecial)
		{
			
			try
			{
			string cmd="";
			int i = 0, istartp = 0 ,istopp = 0, crt = 0;
			List<string> queries = new List<string>();

			for (i = 0; i < len; i++)
			{
				if ((s[i] == '\'' || s[i] == '"'))
				{
					if (!waitingForEnclose)
						waitingChar = (char)s[i];
					bool isEnclosing = IsValidEnclosed(s,waitingChar,i,lastCharWasSpecial);
					if(isEnclosing)
					{
						waitingChar = (char)s[i];
						if (!waitingForEnclose)
						{
							cmd = encoding.GetString(s, crt, i - crt);
							List<string> r = ProcessClearCmd(cmd);
							for (int j = 0; j < r.Count; j++)
								queries.Add(r[j]);
							istartp = i + 1;
							mainCommandBuffer.Append(restCommandBuffer.ToString());
							mainCommandBuffer.Append((char)s[i]);
							
						}
						else
						{
							//we clear rest of rest command to the right of ' or "
							restCommandBuffer.Length = 0;
							istopp = i;
							if (istopp >-1)
							{
								string x = encoding.GetString(s, istartp, istopp - istartp);
								mainCommandBuffer.Append(x);
							}
							mainCommandBuffer.Append((char)s[i]);
							crt = i+1;
						}
						waitingForEnclose = !waitingForEnclose;
					}
				}
			}
			if (waitingForEnclose)
			{
				string y = encoding.GetString(s, istartp, len - istartp);
				mainCommandBuffer.Append(y);
			}
			else
			{
				string tmpa =  encoding.GetString(s,crt,len-crt);
				List<string> r  = ProcessClearCmd(tmpa);
				for (int j = 0; j < r.Count; j++)
					queries.Add(r[j]);
			}
			if (s[len - 1] == '\\')
			{
				int kb = len - 2;
				bool gatab = false;
				while ((kb >= 0) && (gatab == false))
				{
					if (s[kb] != '\\')
					{
						gatab = true;
					}
					else
						kb--;
				}
				if ((len - 1 - kb) % 2 != 0)
					lastCharWasSpecial = true;
				else
					lastCharWasSpecial = false;
			}
			else
				lastCharWasSpecial = false;
			
			return queries;
			}
			catch(Exception ex)
			{
				return null;
			}
		}
		
		
		protected virtual bool IsValidEnclosed(byte[] s, char waitingChar, int i, bool lastCharWasSpecial)
		{
			if(waitingChar != s[i])
				return false;
			int k = i - 1;
			bool gata = false;
			while ((k >= 0) && (gata == false))
			{
				if (s[k] != '\\')
				{
					gata = true;
				}
				else
					k--;
			}
			if (k > -1)
			{
				//is enclosing char
				if ((i - k) % 2 != 0)
					return true;
				else
					return false;
			}
			else
			{
				if ((i % 2 == 0) && (!lastCharWasSpecial) || ((i % 2 != 0) && (lastCharWasSpecial)))
				{
					return true;
				}
				else
					return false;
			}

		}

		
		//processing unbounded by ' or " string
		private List<string> ProcessClearCmd(string cmd)
		{
			int idDel = 0;
			int aux2 = 0;
			string[] rez = null;
			int start = 0;
			int initialLeng = restCommandBuffer.Length;
			analized = restCommandBuffer.Append(cmd).ToString();
			restCommandBuffer.Append(cmd);
			MatchCollection mColl = reg.Matches(analized,idDel);
			List<string> ret = new List<string>();
			for(int i=0;i<mColl.Count;i++)
			{
				Match m = mColl[i];
				idDel = m.Index;
				delStr = m.ToString();
				aux = delStr.Remove(0, 10);
				
				int ix = aux.Length;
				aux2 = m.Length;
				sh = analized.Substring(start, idDel - start);
				string [] dels = new string[] { delimiter};
				rez = sh.Split(dels , StringSplitOptions.None);
				if (rez.Length > 1)
				{
					mainCommandBuffer.Append(rez[0]);
					string sret = mainCommandBuffer.ToString();
					ret.Add(sret);
					mainCommandBuffer.Length = 0 ;
					mainCommandBuffer.Append(rez[rez.Length - 1]);
					int j = 0;
					for (j = 1; j < rez.Length - 1; j++)
						ret.Add(rez[j]);
					restCommandBuffer.Length=0;
				}
				else
					if (rez.Length == 1)
				{
					mainCommandBuffer.Length = 0;
					mainCommandBuffer.Append(rez[0]);
					restCommandBuffer.Append(rez[0]);
				}
				start = idDel + delStr.Length;
				delimiter = aux.TrimEnd(new char[]{' ', '\r', '\n'});
				initialLeng = start;
			}
			string l = analized.Substring(start, analized.Length - start);
			string [] srez = l.Split(new string[] { delimiter }, StringSplitOptions.None);
			if (srez.Length > 1)
			{
				
				mainCommandBuffer.Append(srez[0]);
				ret.Add(mainCommandBuffer.ToString());
				mainCommandBuffer.Length=0;
				int j = 0;
				for (j = 1; j < srez.Length - 1; j++)
				{
					ret.Add(srez[j]);
				}
				restCommandBuffer.Length = 0;
				restCommandBuffer.Append(srez[srez.Length - 1]);
			}
			else
			{
				restCommandBuffer.Length=0;
				restCommandBuffer.Append(l);
			}
			mColl = null;
			return ret;
		}
		
	}
}
