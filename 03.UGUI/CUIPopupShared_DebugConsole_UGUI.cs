using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-03-04 오전 12:20:30
   Description : 
   Edit Log    : 
   ============================================ */

public class CUIPopupShared_DebugConsole_UGUI : CUGUIPanelHasInputBase<CUIPopupShared_DebugConsole_UGUI.EUIInput>, IPointerDownHandler,IDragHandler
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIInput
	{
		Button_Close,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	[GetComponentInChildren]
	private ScrollRect _pScrollView = null;
	[GetComponentInChildren( "Content" )]
	static private Text _pUIText = null;

    private Vector3 _vecPos_DragStart;
    private Vector3 _vecOriginPos;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _vecPos_DragStart = GetInputPos();
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 vecCurrentInputPos = GetInputPos();

        transform.position += vecCurrentInputPos - _vecPos_DragStart;
        _vecPos_DragStart = vecCurrentInputPos;
    }

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    private void OnCallbackUnity_DebugLog(string strLog, string strStackTrace, LogType eLogType)
	{
		string strLogType = "";

		switch (eLogType)
		{
			case LogType.Warning:
				strLogType = "<color=#ffa500ff>[경고] "; break;
			case LogType.Log:
				strLogType = "<color=black>[로그] "; break;
            case LogType.Error:
                strLogType = "<color=red>[에러] "; break;
            case LogType.Exception:
				strLogType = "<color=red>[심각한 에러] "; break;
		}

		string strDebugLog = string.Format("\n[시간]{0} -> {1}\n{2}\n[스택 트레이스]\n{3}", Time.time, strLogType, strLog, strStackTrace);
        strDebugLog += "</color>";

		if(_pUIText != null)
	        _pUIText.text += strDebugLog;
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		if (_pUIText == null)
		{
			Debug.LogWarning( "UITextList 가 없습니다." );
			return;
		}

		_vecOriginPos = transform.position;
		_pUIText.text = "";
		Application.logMessageReceived += OnCallbackUnity_DebugLog;

	}

	public override void OnClick_Buttons( EUIInput eButtonName )
	{
		switch (eButtonName)
		{
			case EUIInput.Button_Close:
				CManagerUIShared_UGUI.instance.DoShowHide_Panel(CManagerUIShared_UGUI.EFrame.CUIPopupShared_DebugConsole_UGUI, false);
				break;
		}
	}

	protected override IEnumerator OnShowPanel_PlayingAnimation( int iSortOrder )
	{
        transform.position = _vecOriginPos;
        _pScrollView.verticalNormalizedPosition = 0f;

		return base.OnShowPanel_PlayingAnimation( iSortOrder );
	}


    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private Vector3 GetInputPos()
    {
        Vector3 vecInputPos = Vector3.zero;
#if UNITY_ANDROID
		vecInputPos = Input.mousePosition;
#else
        vecInputPos = Input.mousePosition;
#endif

        return vecInputPos;
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
