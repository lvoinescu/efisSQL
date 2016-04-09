/*
 * Created by SharpDevelop.
 * User: Sam
 * Date: 12/6/2014
 * Time: 3:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace DBMS.core.SqlParser
{
	/// <summary>
	/// Description of QuriesReadEventArgs.
	/// </summary>
	public class QueriesReadEventArgs
	{
		
		public long TotalBytes {
			get { return totalBytes; }
			set { totalBytes = value; }
		}
		public long totalBytes;
		
		public long BytesRead {
			get { return bytesRead; }
			set { bytesRead = value; }
		}
		private long bytesRead;
		
		private List<String> queries;
		
		public List<string> Queries {
			get { return queries; }
			set { queries = value; }
		}
		private float percentageRead;
		
		public float PercentageRead {
			get { return percentageRead; }
			set { percentageRead = value; }
		}
		
		
		public QueriesReadEventArgs(List<String> queries, long totalBytes, long bytesRead)
		{
			this.totalBytes = totalBytes;
			this.bytesRead = bytesRead;
			this.queries = queries;
		}
	}
}
