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

namespace DBMS.core.SqlParser
{
	
	
	/// <summary>
	/// Description of SqlParsers.
	/// </summary>
	public class SqlParser
	{
		
		public event QueriesReadDelegate QueriesRead;
		
		const int bufferSize = 65536;
		const String delimiterClause = "delimiter ";

		
		int readQueries;
		long fileSize;
		string analizedChunk;
		string delimiter = ";";
		Encoding encoding = Encoding.GetEncoding(1257);
		StringBuilder remainingCommand;
		StringBuilder mainCommand;
		Stream inputStream;
		Regex delimiterRegex = new Regex(@"(delimiter\s[^\s\r\n]+[ \r\n]+)", RegexOptions.IgnoreCase);
		
		
		public SqlParser()
		{
			
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
			
			if (inputStream == null)
				throw new IOException("Null imput stream");
			long totalBytes = 0;
			// don't know what's the problem with UTF8 BOM or without BOM...
			
			mainCommand = new StringBuilder(64 * 1024 * 1024);
			remainingCommand = new StringBuilder(64 * 1024 * 1024);
			BinaryReader binaryReader = new BinaryReader(inputStream, encoding);
			
			byte[] fileContents = new byte[bufferSize];
			fileSize = inputStream.Seek(0, SeekOrigin.End);
			inputStream.Seek(0, SeekOrigin.Begin);
			int charsRead = binaryReader.Read(fileContents, 0, bufferSize);
			if (charsRead == 0)
				return;
			totalBytes = charsRead;
			bool waitingForEnclose = false;
			char waitingChar = '\'';
			bool lastCharWasSpecial = false;
			bool waitingForDelimiter = false;
			readQueries = 0;
			

			while (charsRead > 0) {
				List<string> rez = GetQuery(fileContents, 0, charsRead, ref waitingForEnclose, ref waitingChar, ref delimiter, ref waitingForDelimiter, ref lastCharWasSpecial);
				readQueries += rez.Count;
				
				if (QueriesRead != null)
					QueriesRead(this, new QueriesReadEventArgs(rez, fileSize, totalBytes));
				
				charsRead = binaryReader.Read(fileContents, 0, bufferSize);
				totalBytes += charsRead;
			}
		}
		
		private List<string> GetQuery(byte[] s, int startindex, int len, ref bool waitingForEnclose, ref char waitingChar, ref string delimiter, ref bool waitingForDelimiter, ref bool lastCharWasSpecial)
		{
			
			try {
				string cmd = "";
				int i = 0, istartp = 0, istopp = 0, crt = 0;
				List<string> queries = new List<string>();

				for (i = 0; i < len; i++) {
					if ((s[i] == '\'' || s[i] == '"')) {
						if (!waitingForEnclose)
							waitingChar = (char)s[i];
						bool isEnclosing = IsValidEnclosed(s, waitingChar, i, lastCharWasSpecial);
						if (isEnclosing) {
							waitingChar = (char)s[i];
							if (!waitingForEnclose) {
								cmd = encoding.GetString(s, crt, i - crt);
								List<string> r = ProcessClearCmd(cmd);
								for (int j = 0; j < r.Count; j++)
									queries.Add(r[j]);
								istartp = i + 1;
								mainCommand.Append(remainingCommand.ToString());
								mainCommand.Append((char)s[i]);
								
							} else {
								//we clear rest of rest command to the right of ' or "
								remainingCommand.Length = 0;
								istopp = i;
								if (istopp > -1) {
									string x = encoding.GetString(s, istartp, istopp - istartp);
									mainCommand.Append(x);
								}
								mainCommand.Append((char)s[i]);
								crt = i + 1;
							}
							waitingForEnclose = !waitingForEnclose;
						}
					}
				}
				if (waitingForEnclose) {
					string y = encoding.GetString(s, istartp, len - istartp);
					mainCommand.Append(y);
				} else {
					string tmpa = encoding.GetString(s, crt, len - crt);
					List<string> r = ProcessClearCmd(tmpa);
					for (int j = 0; j < r.Count; j++)
						queries.Add(r[j]);
				}
				if (s[len - 1] == '\\') {
					int kb = len - 2;
					bool gatab = false;
					while ((kb >= 0) && (gatab == false)) {
						if (s[kb] != '\\') {
							gatab = true;
						} else
							kb--;
					}
					if ((len - 1 - kb) % 2 != 0)
						lastCharWasSpecial = true;
					else
						lastCharWasSpecial = false;
				} else
					lastCharWasSpecial = false;
				
				return queries;
			} catch (Exception ex) {
				return null;
			}
		}
		
		
		protected virtual bool IsValidEnclosed(byte[] s, char waitingChar, int i, bool lastCharWasSpecial)
		{
			if (waitingChar != s[i])
				return false;
			int k = i - 1;
			while ((k >= 0) && (s[k] == '\\')) {
				k--;
			}
			if (k > -1) {
				//is enclosing char
				if ((i - k) % 2 != 0) {
					return true;
				}
				return false;
			}
			if ((i % 2 == 0) && (!lastCharWasSpecial) || ((i % 2 != 0) && (lastCharWasSpecial))) {
				return true;
			}
			return false;

		}

		
		//processing unbounded by ' or " string
		private List<string> ProcessClearCmd(string cmd)
		{
			int start = 0;
			analizedChunk = remainingCommand.Append(cmd).ToString();
			remainingCommand.Append(cmd);
			
			MatchCollection delimiterMatches = delimiterRegex.Matches(analizedChunk, 0);
			List<string> fullQueries = new List<string>();
			
			foreach (Match m in delimiterMatches) {
				int delimiterPatternPosition = m.Index;
				String delimiterSentence = m.ToString();
				String rawDelimiter = delimiterSentence.Remove(0, delimiterClause.Length);
				
				String clauseWithoutDelimiterPattern = analizedChunk.Substring(start, delimiterPatternPosition - start);
				
				string[] splitedSentences = clauseWithoutDelimiterPattern.Split(new string[] {	delimiter }, StringSplitOptions.None);
				if (splitedSentences.Length > 1) {
					mainCommand.Append(splitedSentences[0]);
					fullQueries.Add(mainCommand.ToString());
					mainCommand = new StringBuilder(splitedSentences[splitedSentences.Length - 1]);
					int j = 0;
					for (j = 1; j < splitedSentences.Length - 1; j++) {
						fullQueries.Add(splitedSentences[j]);
					}
					remainingCommand.Length = 0;
				} else if (splitedSentences.Length == 1) {
					mainCommand = new StringBuilder(splitedSentences[0]);
					remainingCommand.Append(splitedSentences[0]);
				}
				start = delimiterPatternPosition + delimiterSentence.Length;
				delimiter = rawDelimiter.TrimEnd(new char[] { ' ', '\r', '\n' });
			}
			
			string l = analizedChunk.Substring(start, analizedChunk.Length - start);
			string[] queries = l.Split(new string[] { delimiter }, StringSplitOptions.None);
			
			if (queries.Length > 1) {
				mainCommand.Append(queries[0]);
				fullQueries.Add(mainCommand.ToString());
				mainCommand.Length = 0;
				int j = 0;
				for (j = 1; j < queries.Length - 1; j++) {
					fullQueries.Add(queries[j]);
				}
				remainingCommand = new StringBuilder(queries[queries.Length - 1]);
			} else {
				remainingCommand = new StringBuilder(l);
			}
			delimiterMatches = null;
			return fullQueries;
		}
		
	}
}
