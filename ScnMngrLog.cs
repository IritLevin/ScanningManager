/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 25/02/09
 * Time: 12:56 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.IO;

namespace ScaningManager
{
	
	/// <summary>
	/// Description of ScnMngrLog.
	/// </summary>
	public class ScnMngrLog
	{
		//private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private EventLog ScnMngrEventLog;
		//----
		private string FileName;
		
		public ScnMngrLog()
		{
			//log4net.Config.XmlConfigurator.Configure();
			ScnMngrEventLog = new EventLog();
			ScnMngrEventLog.Log =  "Application";
			ScnMngrEventLog.Source = "ScannerMannager";
			//-----------
			FileName = "LogFile.txt";
		}		
		
		public ScnMngrLog(string FileName_)
		{
			//log4net.Config.XmlConfigurator.Configure();
			//log4net.Appender.FileAppender FileApp = new log4net.Appender.FileAppender();
			//FileApp.File = FileName_;
			
			ScnMngrEventLog = new EventLog();
			ScnMngrEventLog.Log =  "Application";
			ScnMngrEventLog.Source = "ScannerMannager";
			
			//------------
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
			//log.Info(_text);
			//----------
			string Msg = "- INFO  - " + _text;
			LogLine(Msg);
		}
//		public void LogInfo(string _text, System.SystemException e)
//		{
//			log.Info(_text, e);
//		}
		
		public void LogWarn(string _text)
		{
			//log.Warn(_text);
			//----------
			string Msg = "- WARN  - " + _text;
			LogLine(Msg);
		}
//		public void LogWarn(string _text, System.SystemException e)
//		{
//			log.Warn(_text, e);
//		}
		
		public void LogError(string _text)
		{
			//log.Error(_text);
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
			//----------
			string Msg = "- ERROR - " + _text;
			LogLine(Msg);
		}
//		public void LogError(string _text, System.SystemException e)
//		{
//			log.Error(_text, e);
//			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
//		}
		
		public void LogFatal(string _text)
		{
			//log.Fatal(_text);
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
			//----------
			string Msg = "- FATAL - " + _text;
			LogLine(Msg);
		}
//		public void LogFatal(string _text, System.SystemException e)
//		{
//			log.Fatal(_text, e);
//			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
//		}
	}
}
