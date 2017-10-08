using System.Collections.Generic;
using UnityEngine;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

namespace UnityEngine
{
	public enum EDebugFilterDefault
	{
		System,
		Error_Project,
		Error_Core,
		Warning_Project,
		Warning_Core,
		Log,
	}

	public static class DebugCustom
	{
		// ========================================================================== //

		private static string const_strDefaultLogWriter = "Strix_Default";

		private static HashSet<string> _setLogIgnore_Writer = new HashSet<string>();
		private static HashSet<string> _setLogIgnore_Level = new HashSet<string>();

		// ========================================================================== //

		public static void AddIgnore_LogWriterList<Enum_LogWriter>( Enum_LogWriter eLogWriter )
			where Enum_LogWriter : System.IConvertible, System.IComparable
		{
			_setLogIgnore_Writer.Add( eLogWriter.ToString() );
		}

		public static void AddIgnore_LogLevel<Enum_DebugLevel>( Enum_DebugLevel eDebugLevel )
			where Enum_DebugLevel : System.IConvertible, System.IComparable
		{
			_setLogIgnore_Level.Add( eDebugLevel.ToString() );
		}


		public static void Log<Enum_LogWriter, Enum_LogLevelCustom>( Enum_LogWriter eLogWriter, Enum_LogLevelCustom eDebugLevelCustom, string strMessage, UnityEngine.Object pObjectHilight = null, int iStackOffset = 0 )
			where Enum_LogWriter : System.IConvertible, System.IComparable
			where Enum_LogLevelCustom : System.IConvertible, System.IComparable
		{
			ProcPrintLog( eLogWriter.ToString(), eDebugLevelCustom.ToString(), (int)EDebugFilterDefault.Log + 1, strMessage, pObjectHilight, iStackOffset );
		}

		public static void LogWarning<Enum_LogWriter, Enum_LogLevelCustom>( Enum_LogWriter eLogWriter, Enum_LogLevelCustom eDebugLevelCustom, string strMessage, UnityEngine.Object pObjectHilight = null, int iStackOffset = 0 )
			where Enum_LogWriter : System.IConvertible, System.IComparable
			where Enum_LogLevelCustom : System.IConvertible, System.IComparable
		{
			ProcPrintLog( eLogWriter.ToString(), eDebugLevelCustom.ToString(), (int)EDebugFilterDefault.Warning_Project, strMessage, pObjectHilight, iStackOffset );
		}

		public static void LogError<Enum_LogWriter, Enum_LogLevelCustom>( Enum_LogWriter eLogWriter, Enum_LogLevelCustom eDebugLevelCustom, string strMessage, UnityEngine.Object pObjectHilight = null, int iStackOffset = 0 )
			where Enum_LogWriter : System.IConvertible, System.IComparable
			where Enum_LogLevelCustom : System.IConvertible, System.IComparable
		{
			ProcPrintLog( eLogWriter.ToString(), eDebugLevelCustom.ToString(), (int)EDebugFilterDefault.Error_Project, strMessage, pObjectHilight, iStackOffset );
		}


		public static void Log_ForCore( EDebugFilterDefault eDebugLevel, string strMessage, UnityEngine.Object pObjectHilight = null, int iStackOffset = 0 )
		{
			ProcPrintLog( const_strDefaultLogWriter, eDebugLevel.ToString(), (int)eDebugLevel, strMessage, pObjectHilight, iStackOffset );
		}

		// ========================================================================== //

		private static void ProcPrintLog( string strWriter, string strDebugLevel, int iLogLevel, string strMessage, UnityEngine.Object pObjectHilight, int iStackOffset = 0 )
		{
			if (_setLogIgnore_Writer.Contains( strWriter ) || _setLogIgnore_Level.Contains( strDebugLevel ))
				return;

			EDebugFilterDefault eDebugLevel = (EDebugFilterDefault)iLogLevel;
			string strLogMessage = string.Format( "{0} {1}", GetLogFormat( strDebugLevel, strWriter, iStackOffset ), strMessage );
			switch (eDebugLevel)
			{
				case EDebugFilterDefault.Log:
				case EDebugFilterDefault.System:
					UnityEngine.Debug.Log( strLogMessage, pObjectHilight );
					break;

				case EDebugFilterDefault.Warning_Project:
				case EDebugFilterDefault.Warning_Core:
					UnityEngine.Debug.LogWarning( strLogMessage, pObjectHilight );
					break;

				case EDebugFilterDefault.Error_Project:
				case EDebugFilterDefault.Error_Core:
					UnityEngine.Debug.LogError( strLogMessage, pObjectHilight );
					break;

				default:	// for Custom LogLevel
					UnityEngine.Debug.Log( strLogMessage, pObjectHilight );
					break;
			}
		}

		private static string GetLogFormat( string strLogLevel, string strWriter, int iStackOffset )
		{
			return string.Format( "<b>[{0}]</b> {1} <i>Writer : {2}</i>\n ", strLogLevel, GetCurrentFileLineNumber( iStackOffset), strWriter );
		}

		private static string GetCurrentFileLineNumber(int iStackOffset)
		{
			var pStackTrace = new System.Diagnostics.StackTrace(UnityEngine.Debug.isDebugBuild);
			var pStackFrame = pStackTrace.GetFrame( 4 + iStackOffset );
			if(pStackFrame == null)
				return "";
			else
				return string.Format( "{0}.cs {1}", pStackFrame.GetMethod().DeclaringType.Name, pStackFrame.GetFileLineNumber() );
		}
	}
}
