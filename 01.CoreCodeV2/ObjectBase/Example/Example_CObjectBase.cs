#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-22 오전 6:59:36
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine.Assertions;

public class Example_CObjectBase : CObjectBase
{
    protected override void OnAwake()
    {
        base.OnAwake();

        PrintLog();
    }

    protected override IEnumerator OnAwakeCoroutine()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("OnAwakeCoroutine - After 1 Sec");
    }

    protected override void OnStart()
    {
        base.OnStart();

        PrintLog();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        PrintLog();
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("OnEnableObjectCoroutine - After 2 Sec - And Disable Self");

        // gameObject.SetActive(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        PrintLog();
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        PrintLog();
    }


    private void PrintLog()
    {
        System.Diagnostics.StackTrace pTrace = new System.Diagnostics.StackTrace();

        UnityEngine.Debug.Log(string.Format("{0} - CObjectBaseTest", pTrace.GetFrame(1).GetMethod()));
    }
}
