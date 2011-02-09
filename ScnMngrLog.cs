/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 25/02/09
 * Time: 12:56 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Diagnostics;
using System.IO;

namespace ScanningManager
{
	
	/// <summary>
	/// Logs messages from the application.
	/// </summary>
	public class ScnMngrLog
	{
		private EventLog ScnMngrEventLog;
		private string FileName;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public ScnMngrLog()
		{
			ScnMngrEventLog = new EventLog();
			ScnMngrEventLog.Log =  "Application";
			ScnMngrEventLog.Source = "ScannerMannager";
			
			FileName = "LogFile.txt";
		}		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="FileName_">Log file name</param>
		public ScnMngrLog(string FileName_)
		{
			ScnMngrEventLog = new EventLog();
			ScnMngrEventLog.Log =  "Application";
			ScnMngrEventLog.Source = "ScannerMannager";

			FileName = FileName_;
		}
			
		public void LogLine(string _text)
		{
			string Msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  "+ _text;
			System.IO.FileInfo LogFile = new FileInfo(FileName);
			StreamWriter SR = new StreamWriter(FileName,true);
			SR.WriteLine(Msg);
            SR.Close();	
		}
		
		public void LogInfo(string _text)
		{
			string Msg = "- INFO  - " + _text;
			LogLine(Msg);
		}
		
		public void LogWarn(string _text)
		{
			string Msg = "- WARN  - " + _text;
			LogLine(Msg);
		}
		
		public void LogError(string _text)
		{
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);

			string Msg = "- ERROR - " + _text;
			LogLine(Msg);
		}
		
		public void LogFatal(string _text)
		{
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);

			string Msg = "- FATAL - " + _text;
			LogLine(Msg);
		}

	}
}
