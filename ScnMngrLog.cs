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

namespace ScaningManager
{
	
	/// <summary>
	/// Description of ScnMngrLog.
	/// </summary>
	public class ScnMngrLog
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private EventLog ScnMngrEventLog;
		
		public ScnMngrLog()
		{
			log4net.Config.XmlConfigurator.Configure();
			ScnMngrEventLog = new EventLog();
			ScnMngrEventLog.Log =  "Application";
			ScnMngrEventLog.Source = "ScannerMannager";
			//<param name="ConversionPattern" value="%-5p%d{yyyy-MM-dd hh:mm:ss} - %m%n" />
		}		
		
		public ScnMngrLog(string FileName)
		{
			log4net.Config.XmlConfigurator.Configure();
			log4net.Appender.FileAppender FileApp = new log4net.Appender.FileAppender();
			FileApp.File = FileName;
			
			ScnMngrEventLog = new EventLog();
			ScnMngrEventLog.Log =  "Application";
			ScnMngrEventLog.Source = "ScannerMannager";
		}
				
		public void LogInfo(string _text)
		{
			log.Info(_text);
		}
		public void LogInfo(string _text, System.SystemException e)
		{
			log.Info(_text, e);
		}
		
		public void LogWarn(string _text)
		{
			log.Warn(_text);
		}
		public void LogWarn(string _text, System.SystemException e)
		{
			log.Warn(_text, e);
		}
		
		public void LogError(string _text)
		{
			log.Error(_text);
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
		}
		public void LogError(string _text, System.SystemException e)
		{
			log.Error(_text, e);
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
		}
		
		public void LogFatal(string _text)
		{
			log.Fatal(_text);
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
		}
		public void LogFatal(string _text, System.SystemException e)
		{
			log.Fatal(_text, e);
			ScnMngrEventLog.WriteEntry(_text, EventLogEntryType.Error);
		}
	}
}
