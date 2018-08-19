using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 5:32:20
   Description : 
   Edit Log    : 
   ============================================ */

public class CSoundPlayer : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	[Rename_Inspector("현재 사용중인 사운드 슬롯", false)]
	public CSoundSlot _pSlotCurrentPlaying;

	[Header("사운드 끝날때 이벤트 - 루프시에도 적용")]
	public UnityEngine.Events.UnityEvent p_listEvent_FinishSound = new UnityEngine.Events.UnityEvent();

    [Rename_Inspector("Disable 시 사운드 Off 유무")]
    public bool _bPlayOff_OnDisable = false;
    [Header("플레이할 사운드 목록")]
    public List<AudioClip> _listPlayAudioClip;

	[Range( 0f, 1f )]
	public float _fSoundVolume = 1f;
	[Rename_Inspector( "반복 횟수" )]
	public int _iLoopCount = 0;
	[Rename_Inspector( "반복시 딜레이시간" )]
	public float _fLoopDelay = 0f;
	[Rename_Inspector("루프유무")]
	public bool _bIsLoop = false;
	[Rename_Inspector( "3D사운드 유무" )]
	public bool _bIs3DSound = false;
	[Rename_Inspector( "3D사운드시 최소들리는거리" )]
	public float _fMinDistance_On3DSound = 1f;
	[Rename_Inspector( "3D사운드시 최대들리는거리" )]
	public float _fMaxDistance_On3DSound = 500f;

    [GetComponent]
    [Rename_Inspector("설정값을 복사할 오디오소스")]
    public AudioSource _pAudioSource;

	/* protected - Field declaration         */

	/* private - Field declaration           */

#if UNITY_EDITOR
	private string _strOriginName;
#endif

	private CManagerSound _pManagerSound;
	private int _iLoopCountCurrent;
	private bool _bIsPlaying = false;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

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

        _pManagerSound = CManagerSound.instance;

#if UNITY_EDITOR
		_strOriginName = name;
#endif
	}

#if UNITY_EDITOR
    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        name = _strOriginName;

        if (_pSlotCurrentPlaying != null && _bPlayOff_OnDisable)
            _pSlotCurrentPlaying.DoStopSound();
    }
#endif

#if UNITY_EDITOR
    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        if (_bIsPlaying)
		{
			if(_iLoopCount != 0)
				name = string.Format("{0} 재생중.. Repeat : {1}", _strOriginName, _iLoopCountCurrent);
			else
				name = string.Format("{0} 재생중..", _strOriginName);
		}
		else
			name = _strOriginName;
	}
#endif

	protected override void OnPlayEventMain()
    {
        base.OnPlayEventMain();

#if UNITY_EDITOR
		if (Application.isPlaying == false) return;
#endif

		if (_pManagerSound == null)
            _pManagerSound = CManagerSound.instance;

		if (_pManagerSound != null)
		{
            ProcPlaySound();

            _iLoopCountCurrent = _iLoopCount;
			_bIsPlaying = true;
		}
		else
		{
			EventExcuteDelay(DoPlayEventTrigger, 0.1f);
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcFinishSound()
	{
		if (_iLoopCount != 0 && _iLoopCountCurrent-- > 0) // 반복 횟수가 0이 아니고 반복 횟수가 아직 0이 아니라면..
            EventExcuteDelay( ProcPlaySound, _fLoopDelay );
		else
		{
            // 반복 횟수가 0이거나 반복 횟수가 다 끝났다면..

			p_listEvent_FinishSound.Invoke();
			_bIsPlaying = false;
            
			if (_bIsLoop)
            {
                if (_fLoopDelay != 0f)
                    EventExcuteDelay(DoPlayEventTrigger, _fLoopDelay);
                else
                    DoPlayEventTrigger();
            }
			else
            {
                if (_pSlotCurrentPlaying != null)
                {
                    _pManagerSound.EventOnSlotFinishClip(_pSlotCurrentPlaying);
                    _pSlotCurrentPlaying = null;
                }

#if UNITY_EDITOR
                if (this != null)
					name = _strOriginName;
#endif
			}
		}
	}

	private void ProcPlaySound()
	{
        _pSlotCurrentPlaying = ProcPlaySound_GetSlot();
        if (_pSlotCurrentPlaying == null) return;

        _pSlotCurrentPlaying.DoSetFinishEvent_OneShot(ProcFinishSound);
        if (_bIs3DSound)
            _pSlotCurrentPlaying.DoSet3DSound(transform.position, _fMinDistance_On3DSound, _fMaxDistance_On3DSound);
    }
    
    private CSoundSlot ProcPlaySound_GetSlot()
    {
        if (_pSlotCurrentPlaying != null)
            _pManagerSound.EventOnSlotFinishClip(_pSlotCurrentPlaying);

        CSoundSlot pSlot = null;
        if (_listPlayAudioClip != null)
        {
            AudioClip pClipRandom = _listPlayAudioClip.GetRandom();
            pSlot = _pManagerSound.DoPlaySoundEffect_OrNull(pClipRandom, _fSoundVolume);
        }

        if (pSlot != null && _pAudioSource != null)
        {
            AudioSource pSlotSource = pSlot.p_pAudioSource;
            pSlotSource.rolloffMode = _pAudioSource.rolloffMode;
            for (int i = 0; i < 3; i++)
            {
                AnimationCurve pCurve = _pAudioSource.GetCustomCurve((AudioSourceCurveType)i);
                pSlotSource.SetCustomCurve((AudioSourceCurveType)i, pCurve);
            }
        }

        return pSlot;
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
