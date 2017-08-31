// Class OriginName : cnguitweenextendbase
// Date 			: 2016.3.10
// Author			: Goolligo_Strix13
// Code Shcema		: 여러개의 Tween 정보를 저장 및 플레이 하기위한 기본 객체.
//
//				      - 기존의 Tween과 다르게 Play만 해도 자동으로 Factor가 리셋 됩니다.
//					  -
//                    -
 

using UnityEngine;
using System.Collections.Generic;

public static class CGameObjectExtends
{
	public static void DoEnable(this GameObject pObj)
	{
		pObj.SetActive(true);
	}

	public static void DoDisable(this GameObject pObj)
	{
		pObj.SetActive(false);
	}
}

[System.Serializable]
abstract public class STweenInfoBase
{
	public bool bStartOnEnable = true;
	public bool bAutoDisableThis = false;
	public bool bAutoDisableFrame = false;
	public float fDuration = 1f;
	public float fStartDelay;
	public UITweener.Style eStyle;
	public AnimationCurve pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public List<EventDelegate> listOnFinished = new List<EventDelegate>();

	protected float _fGap = -1f;

	public abstract float GetFromToGap(bool bCalculate = false);
}

abstract public class CNGUITweenExtendBase<TEMPLATE> : UITweener
    where TEMPLATE : STweenInfoBase, new()
{
    public List<TEMPLATE> listTweenInfo = new List<TEMPLATE>(2);
    protected TEMPLATE m_pCurrentTweenInfo;

	private CNGUITweenExtendListener _pListener = null;
	private event System.Action _EVENT_OnFinish = null;

	[HideInInspector]
	public float p_fTweenSpeed
	{
		get
		{
			return _fTweenSpeed;
		}

		set
		{
			_fTweenSpeed = value;
			if (Application.isPlaying && m_pCurrentTweenInfo != null)
			{
				m_pCurrentTweenInfo.fDuration = m_pCurrentTweenInfo.GetFromToGap() / _fTweenSpeed;
				duration = m_pCurrentTweenInfo.fDuration;
			}
		}
	}

	public bool _bCheckTweenAmount = true;

	[HideInInspector]
	public float p_fTweenAmount;

	private float _fTweenSpeed;

	//=============================== [1. Start Public] ===============================//
	#region Public

	protected override void Start()
	{
		ProcSettingBeforePlay(0);

		_pListener = GetComponent<CNGUITweenExtendListener>();
		if (_pListener == null)
			_pListener = gameObject.AddComponent<CNGUITweenExtendListener>();

		if (m_pCurrentTweenInfo.bStartOnEnable)
			_pListener.p_EVENT_OnEnabled += EventOnEnable;

		if (m_pCurrentTweenInfo.bAutoDisableFrame)
		{
			CUIFrameBase[] arrFrames = GetComponentsInParent<CUIFrameBase>();
			if (arrFrames.Length != 0)
				_EVENT_OnFinish += arrFrames[0].DoHide;
			else
				Debug.LogWarning("부모 프레임 스크립트를 등록해주세요.");
		}

		if (m_pCurrentTweenInfo.bAutoDisableThis)
			_EVENT_OnFinish += gameObject.DoDisable;

		ResetToBeginning();
	}

	private void OnDisable()
	{
		if (_EVENT_OnFinish != null)
			_EVENT_OnFinish();
	}

	private void EventOnEnable()
	{
		ResetToBeginning();
		enabled = true;
	}

    public void SetTweenInfoSize(int iTweenInfoSize)
    {
        ProcEditListCount(iTweenInfoSize);
    }

    public void DoObjectActiveTrue() { gameObject.SetActive(true); }
    public void DoObjectActiveFalse() { gameObject.SetActive(false); }

    public void DoPlayTween_Forward_0() { DoPlayTween_Forward(0); }
    public void DoPlayTween_Forward_1() { DoPlayTween_Forward(1); }
    public void DoPlayTween_Forward_2() { DoPlayTween_Forward(2); }
    public void DoPlayTween_Forward_3() { DoPlayTween_Forward(3); }

    public void DoPlayTween_Reverse_0() { DoPlayTween_Reverse(0); }
    public void DoPlayTween_Reverse_1() { DoPlayTween_Reverse(1); }
    public void DoPlayTween_Reverse_2() { DoPlayTween_Reverse(2); }

    public void DoPlayTween_Forward(int iGroupNumber)
    {
		ProcSettingBeforePlay(iGroupNumber);
		ResetToBeginning();
		Play(true);
    }

	public void DoPlayTween_Reverse(int iGroupNumber)
	{
		ProcSettingBeforePlay(iGroupNumber);
		Play(false);
	}

	public void DoSetTween_StartTime(int iGroupNumber, float fStartTime)
	{
		if (m_pCurrentTweenInfo == null)
			m_pCurrentTweenInfo = listTweenInfo[iGroupNumber];

		m_pCurrentTweenInfo.fStartDelay = fStartTime;
	}

	public void DoPlayOrStop_CheckTweenAmount(bool bPlay)
	{
		_bCheckTweenAmount = bPlay;
	}

    #endregion

    //=============================== [2. Start Overriding] =================================//
    #region Overriding

	abstract protected void UpdateTweenValue(float fFactor, bool bIsFinished);

	private void Reset()
	{
		ProcEditListCount( 1 );
	}

	protected override void OnUpdate(float factor, bool isFinished)
    {
		if (m_pCurrentTweenInfo == null)
        {
            ProcSettingBeforePlay(0);
        }

        if (m_pCurrentTweenInfo != null)
        {
            UpdateTweenValue(factor, isFinished);
        }
    }

    #endregion

    //=============================== [3. Start CoreLogic] ===============================//
    #region CoreLogic

    protected void ProcSettingBeforePlay(int iGroupID)
    {
        if (iGroupID >= 0 && iGroupID < listTweenInfo.Count)
        {
			m_pCurrentTweenInfo = listTweenInfo[iGroupID];
            duration = m_pCurrentTweenInfo.fDuration;

			_fTweenSpeed = m_pCurrentTweenInfo.GetFromToGap(true) / duration;

			delay = m_pCurrentTweenInfo.fStartDelay;
            animationCurve = m_pCurrentTweenInfo.pAnimationCurve;
			style = m_pCurrentTweenInfo.eStyle;

			for (int i = 0; i < m_pCurrentTweenInfo.listOnFinished.Count; i++)
            {
                EventDelegate.Add(onFinished, m_pCurrentTweenInfo.listOnFinished[i], true);
                if (m_pCurrentTweenInfo.listOnFinished[i].oneShot)
                    EventDelegate.Remove(m_pCurrentTweenInfo.listOnFinished, m_pCurrentTweenInfo.listOnFinished[i]);
            }
        }
        else
        {
            Debug.Log("Error! Not Found Group ID : " + iGroupID, this);
        }
    }

    protected void ProcEditListCount(int iListSize)
    {
        if (listTweenInfo.Count < iListSize)
        {
            int iAddCount = iListSize - listTweenInfo.Count;
            for (int i = 0; i < iAddCount; i++)
            {
                listTweenInfo.Add(new TEMPLATE());
            }
        }
        else if (listTweenInfo.Count > iListSize)
        {
            int iDeleteCount = listTweenInfo.Count - iListSize;
            for (int i = 0; i < iDeleteCount; i++)
            {
                listTweenInfo.RemoveAt(i);
            }
        }
    }

    #endregion
}
