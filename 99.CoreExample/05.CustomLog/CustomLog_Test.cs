using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLog_Test : MonoBehaviour {

	public enum ELogWriter
	{
		Programmer_Senior,
		Programmer_Junior,
		Programmer_Newbie,
	}

	public enum ELogLevelCustom
	{
		UI,
		UI_ForDebug,

		InGame,
		InGame_ForDebug,
	}

	// Use this for initialization
	void Start ()
	{
		LogTestCase_1();
		LogTestCase_2();
	}

	void LogTestCase_1()
	{
		Strix.Debug.Log( ELogWriter.Programmer_Senior, Strix.EDebugLevel.System, "Senior Work" );
		Strix.Debug.Log( ELogWriter.Programmer_Junior, Strix.EDebugLevel.System, "Junior Work" );
		Strix.Debug.Log( ELogWriter.Programmer_Newbie, Strix.EDebugLevel.System, "Newbie Work" );

		// 난 내 로그만 보고싶다 하면 이것 호출
		Strix.Debug.AddIgnore_LogWriterList( ELogWriter.Programmer_Senior );
		Strix.Debug.AddIgnore_LogWriterList( ELogWriter.Programmer_Junior );

		Strix.Debug.Log_Custom( ELogWriter.Programmer_Senior, ELogLevelCustom.InGame, "Senior Work Ingame" );
		Strix.Debug.Log_Custom( ELogWriter.Programmer_Junior, ELogLevelCustom.InGame_ForDebug, "Junior Work Ingame Debuging" );
		Strix.Debug.Log_Custom( ELogWriter.Programmer_Newbie, ELogLevelCustom.UI, "Newbie Work UI" );

		// 커스텀 로그 레벨도 가능하다
		// 만약 UI 로그만 보고싶다면
		// 그 외 로그는 다 무시
		Strix.Debug.AddIgnore_LogLevel( ELogLevelCustom.InGame );
		Strix.Debug.AddIgnore_LogLevel( ELogLevelCustom.InGame_ForDebug );

		// 그다음 다시 일하면 뉴비 로그만 출력
		Strix.Debug.Log_Custom( ELogWriter.Programmer_Senior, ELogLevelCustom.InGame, "Senior Work Ingame" );
		Strix.Debug.Log_Custom( ELogWriter.Programmer_Junior, ELogLevelCustom.InGame_ForDebug, "Junior Work Ingame Debuging" );
		Strix.Debug.Log_Custom( ELogWriter.Programmer_Newbie, ELogLevelCustom.UI, "Newbie Work 3 - UI 일중" );
		Strix.Debug.Log_CustomWarning( ELogWriter.Programmer_Newbie, ELogLevelCustom.UI_ForDebug, "Newbie Work 3 - UI 디버그용 로그" );
	}

	List<int> listTest = new List<int>();
	void LogTestCase_2()
	{
		listTest.Add( 1 );
		listTest.Add( 2 );

		Strix.Debug.Log( ELogWriter.Programmer_Newbie, Strix.EDebugLevel.System, "만약 에러가 나면 줄번호를 여기다 표기하고싶다면 오프셋을 조정" );
		listTest.Contains_PrintOnError( 1 );
		listTest.Contains_PrintOnError( 3 ); // 여기 따라가서 코드 확인 해볼것
		Function_Core();
	}

	void Function_Core()
	{
		// 코어 코드에서는 코어에서 확인하기보단 프로젝트 레벨에서 확인해야 하기때문에..
		// 라인 넘버 표기를 프로젝트로 조정
		Strix.Debug.Log(Strix.EDebugLevel.Error_Core, Strix.EDebugLevel.System, "Function_Core", null, 1 );
	}
}
