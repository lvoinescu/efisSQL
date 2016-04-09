/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 12/6/2014
 * Time: 1:24 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DBMS.core.SqlParser;
using NUnit.Framework;

namespace test
{
	[TestFixture]
	public class SqlProcessingTest
	{
		private String delimiter =";";
		
		/**
		 * Test the basic format of an sigle sql query
		 */
		[Test]
		public void TestSingleQueryNoSpecialCharsMethod()
		{
			
			String q1 ="select * from test" ;
			String sql = q1 + delimiter;
			byte[] byteArray = Encoding.UTF8.GetBytes(sql);
			MemoryStream stream = new MemoryStream(byteArray);
			SqlParser parser = new SqlParser(stream);
			List<String> result = new List<String>();
			parser.QueriesRead += delegate(object sender, QueriesReadEventArgs arg) {
				result.AddRange(arg.Queries);
			};
			parser.ReadQueries();

			Assert.True(result.Count==1);
			Assert.True(result.Contains(q1));
		}
		
		/*
		 * Testing two basic queries splited by ";"
		 */
		[Test]
		public void Test2SingleQueryNoSpecialCharsMethod()
		{
			String q1 = "simple query1";
			String q2 = "simple query2";
			String sql = q1 + delimiter + q2 + delimiter;
			byte[] byteArray = Encoding.UTF8.GetBytes(sql);
			MemoryStream stream = new MemoryStream(byteArray);
			SqlParser parser = new SqlParser(stream);
			List<String> result = new List<String>();
			parser.QueriesRead += delegate(object sender, QueriesReadEventArgs arg) {
				result.AddRange(arg.Queries);
			};
			try
			{
				parser.ReadQueries();
			}
			catch(Exception ex)
			{
				Console.Out.WriteLine(ex.Message);
			}
			Assert.True(result.Count==2);
			Assert.True(result.Contains(q1));
			Assert.True(result.Contains(q2));
		}
		
		
		
		/*
		 * Test simple delimitator changing
		 */
		[Test]
		public void TestDelimiterChangedMethod()
		{
			String newDlimiter = "$";
			String q1 = "simple query1";
			String q2 ="delimiter " + newDlimiter+"\r\n\r\n\r\r ";
			String q3 = "simple query2";

			String sql = q1 + delimiter + q2  + q3 + newDlimiter;
			byte[] byteArray = Encoding.UTF8.GetBytes(sql);
			MemoryStream stream = new MemoryStream(byteArray);
			SqlParser parser = new SqlParser(stream);
			List<String> result = new List<String>();
			parser.QueriesRead += delegate(object sender, QueriesReadEventArgs arg) {
				result.AddRange(arg.Queries);
			};
			parser.ReadQueries();
			Assert.True(result.Contains(q1));
			Assert.True(result.Contains(q3));
		}
		
		
		/*
		 * Test escaped sequenced
		 */
		[Test]
		public void TestEscapedCharactersMethod()
		{
			String newDelimiter = ",";
			String q1 = "select q1\"start inside quotes\\\" end inside quotes\" outside quotes\\\\";
			String q2 = "select q2\"start inside quotes;;;\\\" end inside quotes\" outside quotes";
			String q3 = "select q3\"delimiter, '' \"";          //no change of the delimitator in this case
			//because is inside quotes
			String q4 = "select q4 ";
			String q5 = "delimiter " + newDelimiter+"\r\n    "; //changing the delimitator
			String q6 = "select q6";
			String sql = q1 + delimiter +
				q2  + delimiter +
				q3  + delimiter +
				q4 + delimiter +
				q5 + "" +
				q6 + newDelimiter
				;
			byte[] byteArray = Encoding.UTF8.GetBytes(sql);
			MemoryStream stream = new MemoryStream(byteArray);
			SqlParser parser = new SqlParser(stream);
			List<String> result = new List<String>();
			
			parser.QueriesRead += delegate(object sender, QueriesReadEventArgs arg) {
				result.AddRange(arg.Queries);
			};
			
			parser.ReadQueries();
			Assert.True(result.Contains(q1));
			Assert.True(result.Contains(q2));
			Assert.True(result.Contains(q3));
			Assert.True(result.Contains(q4));
			Assert.True(!result.Contains(q5));
			Assert.True(result.Contains(q6));
		}
		
		/*
		 * Test simple delimitator changing
		 */
		[Test]
		public void TestSingleQuotesMethod()
		{
			String q1 = "'a\"'";


			String sql = q1 + delimiter ;
			byte[] byteArray = Encoding.UTF8.GetBytes(sql);
			MemoryStream stream = new MemoryStream(byteArray);
			SqlParser parser = new SqlParser(stream);
			List<String> result = new List<String>();
			parser.QueriesRead += delegate(object sender, QueriesReadEventArgs arg) {
				result.AddRange(arg.Queries);
			};
			parser.ReadQueries();
			Assert.True(result.Contains(q1));
		}
		
		
		/*
		 * Test simple delimitator changing
		 * 
		 */
		
		class SqlParserHelper : SqlParser{
			public SqlParserHelper(Stream inputStream) : base(inputStream){}
			
			public bool  IsValidEnclosingChar(byte[] s, char waitingChar, int i, bool lastCharWasSpecial)
			{
				return base.IsValidEnclosed(s, waitingChar, i, lastCharWasSpecial);
			}
		}
		
		//simple test for " inside string value field enclosed by '
		[Test]
		public void TestIsValidEncosingCharMethod()
		{
			String q1 = "'a\"'";


			String sql = q1 + delimiter ;
			byte[] byteArray = Encoding.UTF8.GetBytes(sql);
			MemoryStream stream = new MemoryStream(byteArray);
			SqlParserHelper parser = new SqlParserHelper(stream);
			
			Assert.False(parser.IsValidEnclosingChar(byteArray, '\'', 2, false));
			Assert.True(parser.IsValidEnclosingChar(byteArray, '\'', 3, false));
		}
		
		//simple test for escaped character sequence iside string encolsed by '
		[Test]
		public void TestIsValidEncosingCharWithEscapedMethod()
		{
			String q1 = "'a\"\\\\\\''";


			String sql = q1 + delimiter ;
			byte[] byteArray = Encoding.UTF8.GetBytes(sql);
			MemoryStream stream = new MemoryStream(byteArray);
			SqlParserHelper parser = new SqlParserHelper(stream);
			
			Assert.False(parser.IsValidEnclosingChar(byteArray, '\'', 6, false));
			Assert.True(parser.IsValidEnclosingChar(byteArray, '\'', 7, false));
		}
		
	}
}
