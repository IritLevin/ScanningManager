/*
 * Created by SharpDevelop.
 * User: Irit
 * Date: 18/02/09
 * Time: 4:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace ScaningManager
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
