using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-04-09 오후 11:11:22
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(UILabel))]
public class CUILabelAnimation : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    public event System.Action p_EVENT_OnAnimationFinish;
    public event System.Action<int> p_EVENT_OnChangeNumber;

    public string p_pText { get { return _pLabel.text; } set { _pLabel.text = value; } }

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    [SerializeField]
    private bool _bIgnoreTimeScale = true;

    private UILabel _pLabel;
    private CSoundSlot _pSoundSlot_OnChangeText;

    private string _strLabelContent;
    private float _fCharPerSec;
    private int _iCharLengthIndex;
    private float _fPrevTime;

    private int _iStartNumber;
    private int _iDestNumber;

	private float _fSmoothNumber;
	private float _fSmoothTo;
	private Coroutine pCoProcAnimateNumberSmooth;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoPlayAnimation(string strLabelContent, float fDurationSec, CSoundSlot pSoundSlot_OnChangeText = null)
    {
        if (_bIsExcuteAwake == false)
            OnAwake();

        if (strLabelContent == null || strLabelContent.Length == 0)
        {
            Debug.LogWarning("라벨의 길이가 0입니다.");
            return;
        }

        if (pSoundSlot_OnChangeText != null)
        {
            _pSoundSlot_OnChangeText = pSoundSlot_OnChangeText;
            pSoundSlot_OnChangeText.DoPlaySound();
        }

        _strLabelContent = strLabelContent;
        _fCharPerSec = fDurationSec / _strLabelContent.Length;
        _pLabel.text = "";
        _iCharLengthIndex = 0;
        _fPrevTime = _bIgnoreTimeScale ? RealTime.time : Time.time;

        StartCoroutine(CoPlayUILabelAnimation());
    }

    public void DoPlayAnimation_Number(int iStartNubmer, int iDestNumber, string strNumberFormat, float fDurationSec, CSoundSlot pSoundSlot_OnChangeText = null)
    {
        if (_bIsExcuteAwake == false)
            OnAwake();

        if (pSoundSlot_OnChangeText != null)
        {
            _pSoundSlot_OnChangeText = pSoundSlot_OnChangeText;
            pSoundSlot_OnChangeText.DoPlaySound();
        }

        _strLabelContent = strNumberFormat;
        _iCharLengthIndex = iStartNubmer;
        _pLabel.text = string.Format(strNumberFormat, iStartNubmer);
        _fPrevTime = _bIgnoreTimeScale ? RealTime.time : Time.time;
        _iStartNumber = iStartNubmer;
        _iDestNumber = iDestNumber;
        _fCharPerSec = fDurationSec;

        StartCoroutine(CoPlayUILabelAnimation_Number());
    }

	public void DoPlayAnimation_Float(float fFrom, float fTo, float fDuration, int iMaxFloat = 0, string strFormat = "")
	{
		_fSmoothTo = fTo;

		if (pCoProcAnimateNumberSmooth == null)
			pCoProcAnimateNumberSmooth = StartCoroutine(CoProcAnimateNumberSmooth(fFrom, fDuration, iMaxFloat, strFormat));
	}

	private IEnumerator CoProcAnimateNumberSmooth(float fFrom, float fDuration, int iMaxFloat, string strFormat = "")
	{
		string fMaxFloatFormat = string.Format("{0}0:f{1}{2}{3}", "{", iMaxFloat, "}", strFormat);
		_pLabel.text = string.Format(fMaxFloatFormat, _fSmoothNumber);

		if (_fSmoothTo <= 0)
			_pLabel.text = string.Format(fMaxFloatFormat, 0f);

		while (true)
		{
			_fSmoothNumber = Mathf.SmoothStep(_fSmoothNumber, _fSmoothTo, Time.deltaTime * fDuration * 15f);
			_pLabel.text = string.Format(fMaxFloatFormat, _fSmoothNumber);

			if (_fSmoothNumber == _fSmoothTo)
			{
				_pLabel.text = string.Format(fMaxFloatFormat, _fSmoothTo);
				pCoProcAnimateNumberSmooth = null;
				yield break;
			}

			yield return null;
		}
	}

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pLabel = GetComponent<UILabel>();
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private IEnumerator CoPlayUILabelAnimation()
    {
        while (true)
        {
            yield return null;

            float fTime = _bIgnoreTimeScale ? RealTime.time : Time.time;
            float fTimeSec = fTime - _fPrevTime;

            if (fTimeSec > _fCharPerSec)
            {
                _pLabel.text = _strLabelContent.Substring(0, _iCharLengthIndex);
                _fPrevTime = fTime;

                if (_pSoundSlot_OnChangeText != null)
                    _pSoundSlot_OnChangeText.DoPlaySound();

                if (_iCharLengthIndex++ == _strLabelContent.Length)
                {
                    if (_pSoundSlot_OnChangeText != null)
                        _pSoundSlot_OnChangeText.DoStopSound();

                    _pLabel.text = _strLabelContent;
                    if (p_EVENT_OnAnimationFinish != null)
                        p_EVENT_OnAnimationFinish();

                    break;
                }
            }
        }
    }

    private IEnumerator CoPlayUILabelAnimation_Number()
    {
        while (true)
        {
            yield return null;

            float fTime = _bIgnoreTimeScale ? RealTime.time : Time.time;
            float fTimeSec = fTime - _fPrevTime;

            _iCharLengthIndex = (int)(Mathf.Lerp(_iStartNumber, _iDestNumber, fTimeSec / _fCharPerSec));
            _pLabel.text = string.Format(_strLabelContent, _iCharLengthIndex);

            if (p_EVENT_OnChangeNumber != null)
                p_EVENT_OnChangeNumber(_iCharLengthIndex);

            if (_pSoundSlot_OnChangeText != null)
                _pSoundSlot_OnChangeText.DoPlaySound();

            if (_iCharLengthIndex >= _iDestNumber)
            {
                if (_pSoundSlot_OnChangeText != null)
                    _pSoundSlot_OnChangeText.DoStopSound();

                _pLabel.text = string.Format(_strLabelContent, _iDestNumber);
                if (p_EVENT_OnAnimationFinish != null)
                    p_EVENT_OnAnimationFinish();

                break;
            }
        }
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}