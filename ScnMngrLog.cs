/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 25/02/09
 * Time: 12:56 PM
 * 
  */


// ScanningManager controls an array of scanners for time lapsed serial scanning.
// Copyright 2010 Irit Levin Reisman published under GPLv3,
// this software was developed in Prof. Nathalie Q. Balaban's lab, at the Hebrew University , Jerusalem , Israel .
// 
//	  This file is part of ScanningManager.
//
//    ScanningManager is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    ScanningManager is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with ScanningManager.  If not, see <http://www.gnu.org/licenses/>.

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
