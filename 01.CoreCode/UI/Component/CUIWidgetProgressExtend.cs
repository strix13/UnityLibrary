using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-05-29 오후 1:53:11
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(CNGUITweenProgress))]
public class CUIWidgetProgressExtend : CUIObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public event System.Action<int> p_EVENT_OnValueChange
    {
        add { _pLabelAnimation.p_EVENT_OnChangeNumber += value; }
        remove { _pLabelAnimation.p_EVENT_OnChangeNumber -= value; }
    }

    /* protected - Field declaration         */

    /* private - Field declaration           */

    private UILabel _pLabelUnit = null;

    private CNGUITweenProgress _pTweenProgress;
    private CUILabelAnimation _pLabelAnimation;

	private float _fLastValue;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoSetUILabel(UILabel pUILabel)
	{
		_pLabelAnimation = pUILabel.GetComponent<CUILabelAnimation>();
		if (_pLabelAnimation == null)
			_pLabelAnimation = pUILabel.gameObject.AddComponent<CUILabelAnimation>();

		_pLabelUnit = pUILabel;
	}

    public void DoStartTween(float fDestValue)
    {
        _pTweenProgress.ResetToBeginning();
        _pTweenProgress.to = fDestValue;
        _pTweenProgress.PlayForward();
    }

	public void DoStartTween(float fFrom, float fTo, float fMax)
	{
		float fCalc_From = ProcCalculatePercentage(fFrom, fMax);
		float fCalc_To = ProcCalculatePercentage(fTo, fMax);

		_pTweenProgress.ResetToBeginning();
		_pTweenProgress.from = fCalc_From;
		_pTweenProgress.to = fCalc_To;
		_pTweenProgress.PlayForward();
	}

	public void DoStartTween(float fDestValue, string strLabel)
    {
        _pLabelUnit.text = strLabel;
        DoStartTween(fDestValue);
    }

    public void DoStartTween(float fDestValue, int iStartNumber, int iDestNumber, string strLabelFormat)
    {
		EventOnAwake();

        if (fDestValue > 1f)
            fDestValue = 1f;

		if (_pLabelUnit != null)
			_pLabelAnimation.DoPlayAnimation_Number(iStartNumber, iDestNumber, strLabelFormat, _pTweenProgress.duration);

        DoStartTween(fDestValue);
    }

	public void DoStartTween_Percent(float fFrom, float fTo, float fMax, int iMaxFloatNum, string strFormat = "")
	{
		EventOnAwake();

		if (_pLabelUnit != null)
			_pLabelAnimation.DoPlayAnimation_Float(fFrom, fTo, _pTweenProgress.duration, iMaxFloatNum, strFormat);

		DoStartTween(fFrom, fTo, fMax);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - Override & Unity API         */

	protected override void OnAwake()
    {
        base.OnAwake();

		if (_pTweenProgress == null)
			_pTweenProgress = GetComponent<CNGUITweenProgress>();
		
		if(_pLabelUnit == null)
			_pLabelUnit = GetComponentInChildren<UILabel>();

		if(_pLabelUnit != null)
			DoSetUILabel(_pLabelUnit);
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		_fLastValue = 0f;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private float ProcCalculatePercentage(float fCur, float fMax)
	{
		float fCalc = (fCur / fMax);
		if (float.IsNaN(fCalc))
			return 0f;

		return fCalc;
	}

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
