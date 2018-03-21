using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if CUSTOMLOG

public class CustomLog_Test : MonoBehaviour
{
    public enum ETestLogWriter
    {
        Programmer_Senior = 1,
        Programmer_Junior = 2,
        Programmer_Newbie = 4,
    }

    public enum ETestLogLevelCustom
    {
        UI = 1,
        UI_ForDebug = 2,

        InGame = 4,
        InGame_ForDebug = 8,
    }

    void Start()
    {
        TestCase_1();
        TestCase_2();
    }

    void TestCase_1()
    {
        Strix.Debug.SetFileExportType(Strix.Debug.EFlagFileExportType.TXT, Strix.Debug.EFlagFileExportType.CSV);
        Strix.Debug.SetFileNameType(Strix.Debug.EFileNameType.OnMinute);

        Strix.Debug.Log(ETestLogWriter.Programmer_Senior, "Senior Work", this);
        Strix.Debug.Log(ETestLogWriter.Programmer_Junior, "Junior Work", this);
        Strix.Debug.Log(ETestLogWriter.Programmer_Newbie, "Newbie Work", this);

        // 특정 작성자 로그만 보고싶다 하시면 이것을 통해 필터링합니다.
        Debug.LogWarning("SetIgnoreLogWriter - Programmer_Senior, Programmer_Junior", this);
        Strix.Debug.SetIgnoreLogWriter(ETestLogWriter.Programmer_Senior, ETestLogWriter.Programmer_Junior);

        // 그다음 다시 로그를 출력하면 뉴비 로그만 출력
        Strix.Debug.Log(ETestLogWriter.Programmer_Senior, "Senior Work 2", this);
        Strix.Debug.Log(ETestLogWriter.Programmer_Junior, "Junior Work 2", this);
        Strix.Debug.Log(ETestLogWriter.Programmer_Newbie, "Newbie Work 2", this);

        // 커스텀 로그 레벨도 가능합니다.
        // 만약 UI 로그만 보고싶다면
        // 그 외 로그는 다 무시
        Debug.LogWarning("SetIgnoreLogLevel_Custom - InGame, InGame_ForDebug", this);
        Strix.Debug.SetIgnoreLogLevel_Custom(ETestLogLevelCustom.InGame, ETestLogLevelCustom.InGame_ForDebug);

        Strix.Debug.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame, "Work Ingame", this);
        Strix.Debug.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame_ForDebug, "Work Ingame Debuging", this);
        Strix.Debug.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.UI, "Work UI", this);
        Strix.Debug.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.UI_ForDebug, "Work UI Debuging", this);
    }

    void TestCase_2()
    {
        // 일반 유니티 로그도 메세지만 등록하면 똑같이 사용 가능합니다.
        Application.logMessageReceived += Strix.Debug.OnUnityDebugLogCallBack;

        Debug.Log("Unity Log!!", this);
        Debug.LogWarning("Unity Warning!!", this);
        Debug.LogError("Unity Error!!", this);
    }
}
#endif