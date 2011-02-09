/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 18/02/09
 * Time: 4:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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

namespace ScanningManager
{
	/// <summary>
	/// A general exception thrown by ScanningManager application
	/// </summary>
	public class ScnMngrException : System.Exception
	{
		public ScnMngrException(string errorMessage): base(errorMessage) {}

		public ScnMngrException(string errorMessage, Exception innerEx): base(errorMessage, innerEx) {}

	}
	
	/// <summary>
	/// An exception thrown by UsbRelay
	/// </summary>
	public class RelayException : ScnMngrException
	{
		public RelayException(string errorMessage): base(errorMessage) {}

		public RelayException(string errorMessage, Exception innerEx): base(errorMessage, innerEx) {}

	}

	/// <summary>
	/// An exception thrown by ScannerPowerMannager
	/// </summary>
	public class ScnrPwrMngrException : ScnMngrException
	{
		public ScnrPwrMngrException(string errorMessage): base(errorMessage) {}

		public ScnrPwrMngrException(string errorMessage, Exception innerEx): base(errorMessage, innerEx) {}

	}
	
	/// <summary>
	/// An exception thrown by ScannerControl
	/// </summary>
	public class ScnCtrlException : ScnMngrException
	{
		public ScnCtrlException(string errorMessage): base(errorMessage) {}

		public ScnCtrlException(string errorMessage, Exception innerEx): base(errorMessage, innerEx) {}

	}
}
