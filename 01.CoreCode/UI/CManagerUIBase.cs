using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NGUI 기반으로 돌아가는 UI Manager Base 클래스
/// </summary>
/// <typeparam name="Class_Instance">상속받을 Class, Instance Type명</typeparam>
/// <typeparam name="ENUM_FRAME_Name">Frame 이름의 Enum</typeparam>
/// <typeparam name="ENUM_POPUP_Name">Popup 이름의 Enum</typeparam>
abstract public class CManagerUIBase<Class_Instance, ENUM_FRAME_Name, ENUM_POPUP_Name> : CSingletonBase<Class_Instance>
    where Class_Instance : CManagerUIBase<Class_Instance, ENUM_FRAME_Name, ENUM_POPUP_Name>
    where ENUM_FRAME_Name : System.IFormattable, System.IConvertible, System.IComparable
    where ENUM_POPUP_Name : System.IFormattable, System.IConvertible, System.IComparable
{
    private CDictionary_ForEnumKey<ENUM_FRAME_Name, CUIFrameBase> _mapFrameInstance = new CDictionary_ForEnumKey<ENUM_FRAME_Name, CUIFrameBase>();
	private CDictionary_ForEnumKey<ENUM_POPUP_Name, CUIPopupBase> _mapPopupInstance = new CDictionary_ForEnumKey<ENUM_POPUP_Name, CUIPopupBase>();

    private Camera _pUIcamera;  public Camera p_pUICamera {  get { return _pUIcamera; } }

    private int _iSortOrderTop; public int p_iSortOrderTop { get { return _iSortOrderTop; } }

    // ========================== [ Division ] ========================== //

    /// <summary>
    /// 지정된 기본 UI외의 Frame, Popup은 모두 끕니다.
    /// </summary>
    public void DoShowDefaultFrame()
    {
        List<CUIFrameBase> listFrameAll = _mapFrameInstance.Values.ToList();
        for (int i = 0; i < listFrameAll.Count; i++)
			listFrameAll[i].gameObject.SetActive( listFrameAll[i].p_bAlwaysShow );

		List<CUIPopupBase> listPopupAll = _mapPopupInstance.Values.ToList();
		for (int i = 0; i < listPopupAll.Count; i++)
			listPopupAll[i].gameObject.SetActive( listPopupAll[i].p_bAlwaysShow );

		OnDefaultFrameShow();
    }

	/// <summary>
	/// 지정된 Frame을 열거나 끕니다.
	/// </summary>
	/// <param name="eFrame">Frame 이름의 Enum</param>
	/// <param name="bShow"></param>
	public CUIFrameBase DoShowHide_Frame(ENUM_FRAME_Name eFrame, bool bShow)
    {
        CUIFrameBase pFrame = _mapFrameInstance[eFrame];
		if(pFrame == null)
		{
			Debug.LogWarning( eFrame + "이 없습니다.. 하이어라키를 확인해주세요" );
			return null;
		}

        if (bShow)
        {
            int iSortOrder = 0;
			if (pFrame.p_bFixedSortOrder == false)
				iSortOrder = CaculateSortLayer();

			pFrame.EventShow(iSortOrder);
			OnShowFrame(eFrame);
		}
		else
            pFrame.DoHide();

		return pFrame;
	}
	
	public void DoChangeFrame_FadeInout(ENUM_FRAME_Name eFrameHide, ENUM_FRAME_Name eFrameShow, float fFadeTime = 1f)
	{
		CUIFrameBase pFrameHide = _mapFrameInstance[eFrameHide];
		CUIFrameBase pFrameShow = _mapFrameInstance[eFrameShow];
		int iSortOrder = 0;
		if (pFrameShow.p_bFixedSortOrder == false)
			iSortOrder = CaculateSortLayer();

		pFrameShow.EventUIFrame_SetOrder( iSortOrder );
		OnShowFrame( eFrameShow );

		AutoFade.DoStartFade( fFadeTime, Color.black, pFrameHide.DoHide, pFrameShow.DoShow );
	}

	public void DoShowFrame_FadeIn(ENUM_FRAME_Name eFrameShow, float fFadeTime = 1f)
	{
		CUIFrameBase pFrameShow = _mapFrameInstance[eFrameShow];

		int iSortOrder = 0;
		if (pFrameShow.p_bFixedSortOrder == false)
			iSortOrder = CaculateSortLayer();

		pFrameShow.EventUIFrame_SetOrder(iSortOrder);
		OnShowFrame(eFrameShow);

		AutoFade.DoStartFade(fFadeTime, Color.black, pFrameShow.DoShow);
	}

	public void DoHideFrame_FadeOut(ENUM_FRAME_Name eFrameShow, float fFadeTime = 1f)
	{
		CUIFrameBase pFrameShow = _mapFrameInstance[eFrameShow];

		AutoFade.DoStartFade(fFadeTime, Color.black, pFrameShow.DoHide);
	}

	public void DoShowHideFrame_Delayed(ENUM_FRAME_Name eFrame, bool bShow, float fDelay)
	{
		CUIFrameBase pFrame = _mapFrameInstance[eFrame];
		pFrame.DoEnableFrameButtons(bShow);

		StartCoroutine(CoProcShowHideFrame_Delayed(pFrame, bShow, fDelay));
	}

	private IEnumerator CoProcShowHideFrame_Delayed(CUIFrameBase pFrame, bool bShow, float fDelay)
	{
		yield return new WaitForSecondsRealtime(fDelay);

		pFrame.DoShowHideFrame(bShow);
	}

	/// <summary>
	/// 지정된 Popup을 열거나 끕니다.
	/// </summary>
	/// <param name="ePopup">Popup 이름의 Enum</param>
	/// <param name="bShow"></param>
	public CUIPopupBase DoShowHide_Popup(ENUM_POPUP_Name ePopup, bool bShow)
    {
        CUIPopupBase pPopup = _mapPopupInstance[ePopup];
        int iSortOrder = 0;
        if (bShow)
        {
            iSortOrder = ++_iSortOrderTop;
            pPopup.EventShow(iSortOrder);
			OnShowPopup(ePopup);
        }
        else
            pPopup.DoHide();

		return pPopup;
	}

    /// <summary>
    /// UI Frame을 얻어옵니다.
    /// </summary>
    /// <typeparam name="CUIFRAME">얻고자 하는 Frame 타입</typeparam>
    /// <returns></returns>
    public CUIFRAME GetUIFrame<CUIFRAME>() where CUIFRAME : CUIFrameBase
    {
		CUIFrameBase pFindFrame = null;
        ENUM_FRAME_Name strKey = typeof(CUIFRAME).ToString().ConvertEnum<ENUM_FRAME_Name>();
        if (_mapFrameInstance.TryGetValue(strKey, out pFindFrame) == false)
            Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", strKey));

        return pFindFrame as CUIFRAME;
    }


	/// <summary>
	/// UI Popup을 얻어옵니다.
	/// </summary>
	/// <typeparam name="CUIPOPUP">얻고자 하는 Popup 타입</typeparam>
	/// <returns></returns>
	public CUIPOPUP GetUIPopup<CUIPOPUP>() where CUIPOPUP : CUIPopupBase
    {
        CUIPopupBase pFindPopup = null;
        ENUM_POPUP_Name strKey = typeof(CUIPOPUP).ToString().ConvertEnum<ENUM_POPUP_Name>();
        if (_mapPopupInstance.TryGetValue(strKey, out pFindPopup) == false)
            Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", strKey));

        return pFindPopup as CUIPOPUP;
    }

	/// <summary>
	/// UI Frame을 얻어옵니다.
	public CUIFrameBase GetUIFrame(ENUM_FRAME_Name eUIFrameName)
	{
		CUIFrameBase pFindFrame = null;
		if (_mapFrameInstance.TryGetValue(eUIFrameName, out pFindFrame) == false)
			Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eUIFrameName));

		return pFindFrame;
	}


	/// <summary>
	/// UI Popup을 얻어옵니다.
	public CUIPOPUP GetUIPopup<CUIPOPUP>(ENUM_POPUP_Name eUIPopup) where CUIPOPUP : CUIPopupBase
	{
		CUIPopupBase pFindPopup = null;
		if (_mapPopupInstance.TryGetValue(eUIPopup, out pFindPopup) == false)
			Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eUIPopup));

		return pFindPopup as CUIPOPUP;
	}

	/// <summary>
	/// 타겟 카메라의 좌표로 UI 오브젝트를 이동합니다. ( 타겟 카메라의 WorldToView -> UI 카메라의 WorldToView )
	/// </summary>
	/// <param name="pCameraTarget">타겟 카메라</param>
	/// <param name="pTransTarget">이동시킬 UI 오브젝트</param>
	/// <param name="vecTargetPos">이동시킬 좌표</param>
	/// <returns></returns>
	public Vector3 DoSetUIToInGame3D(Camera pCameraTarget, Transform pTransTarget, Vector3 vecTargetPos)
    {
        Vector3 v3UIpos = _pUIcamera.ViewportToWorldPoint(pCameraTarget.WorldToViewportPoint(vecTargetPos));
        pTransTarget.position = new Vector3(v3UIpos.x, v3UIpos.y, 0);
        pTransTarget.localPosition = new Vector3(pTransTarget.localPosition.x, pTransTarget.localPosition.y, 0);

        return pTransTarget.localPosition;
    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        InitUIFrameInstance();
		InitUIPopupInstance();

        CUIFrameBase[] arrFrameInstance = GetComponentsInChildren<CUIFrameBase>(true);
        for (int i = 0; i < arrFrameInstance.Length; i++)
            arrFrameInstance[i].EventUIFrame_OnAwake();

        UIRoot pRoot = FindObjectOfType<UIRoot>();
        if(pRoot != null)
            _pUIcamera = pRoot.GetComponentInChildren<Camera>();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        DoShowDefaultFrame();
    }

	// ========================== [ Division ] ========================== //

	virtual protected void OnShowFrame(ENUM_FRAME_Name eFrame) { }
	virtual protected void OnShowPopup(ENUM_POPUP_Name ePopup) { }
	abstract protected void OnDefaultFrameShow();

    // ========================== [ Division ] ========================== //

    private void InitUIFrameInstance()
    {
        CUIFrameBase[] arrFrameInstance = GetComponentsInChildren<CUIFrameBase>(true);
        for (int i = 0; i < arrFrameInstance.Length; i++)
        {
            CUIFrameBase pFrame = arrFrameInstance[i];
            if ((pFrame is CUIPopupBase) == false)
                _mapFrameInstance.Add(ConvertStringToEnum_Frame(arrFrameInstance[i].gameObject.name), arrFrameInstance[i]);
        }
    }

	private void InitUIPopupInstance()
    {
        CUIPopupBase[] arrPopupInstance = GetComponentsInChildren<CUIPopupBase>(true);
        for (int i = 0; i < arrPopupInstance.Length; i++)
        {
            CUIPopupBase pPopup = arrPopupInstance[i];
            _mapPopupInstance.Add(ConvertStringToEnum_Popup(pPopup.gameObject.name), pPopup);
            if (pPopup.gameObject.activeSelf)
                pPopup.gameObject.SetActive(false);
        }
    }

    private int CaculateSortLayer()
    {
        _iSortOrderTop = 0;
        List<CUIFrameBase> listUIFrame = _mapFrameInstance.Values.ToList();

        for (int i = 0; i < listUIFrame.Count; i++)
        {
            if (listUIFrame[i].p_bShowCurrent)
            {
                if (listUIFrame[i].p_bFixedSortOrder)
                    ++_iSortOrderTop;
                else
                    listUIFrame[i].EventShow(++_iSortOrderTop);
            }
        }

        return _iSortOrderTop;
    }

    // ========================== [ Division ] ========================== //

    public ENUM_FRAME_Name ConvertStringToEnum_Frame(string strObjectName)
    {
        ENUM_FRAME_Name eFrame = default(ENUM_FRAME_Name);
        try
        {
            eFrame = (ENUM_FRAME_Name)System.Enum.Parse(typeof(ENUM_FRAME_Name), strObjectName);
        }
        catch
        {
            Debug.LogWarning(string.Format("오브젝트 이름 {0}을 Enum UI Frame으로 변경 중 오류", strObjectName), this);
        }

        return eFrame;
    }

    public ENUM_POPUP_Name ConvertStringToEnum_Popup(string strObjectName)
    {
        ENUM_POPUP_Name ePopup = default(ENUM_POPUP_Name);
        try
        {
            ePopup = (ENUM_POPUP_Name)System.Enum.Parse(typeof(ENUM_POPUP_Name), strObjectName);
        }
        catch
        {
            Debug.LogWarning(string.Format("오브젝트 이름 {0}을 Enum UI Popup으로 변경 중 오류", strObjectName));
        }

        return ePopup;
    }
}
