using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 5:32:20
   Description : 본래 커스텀 인스펙터를 통해 세팅해야 하나, 제네릭 함수라서 여기에 세팅..
   Edit Log    : 
   ============================================ */

[ExecuteInEditMode]
public class CSoundPlayerBase<ENUM_SOUND_NAME> : CCompoEventTrigger
    where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	[Rename_Inspector("현재 사용중인 사운드 슬롯", false)]
	public CSoundSlot _pSlotCurrentPlaying;

	[Header("사운드 끝날때 이벤트 - 루프시에도 적용")]
	public UnityEngine.Events.UnityEvent p_listEvent_FinishSound = new UnityEngine.Events.UnityEvent();

    [Header("AudioClip이 있을경우 Clip먼저 실행, Clip이 없으면 Enum으로 실행 ")]
    [Rename_Inspector("플레이할 사운드")]
    public AudioClip _pPlayAudioClip;

    [Rename_Inspector("플레이할 사운드 목록 - 랜덤 재생시 실행")]
    public AudioClip[] _arrPlayAudioClip;



    [Rename_Inspector( "플레이할 사운드" )]
	public ENUM_SOUND_NAME _eSoundName;
	[Rename_Inspector( "플레이할 사운드 - 이넘이 많을시 사용" )] // Enum 밀렸을때를 대비한 백업
	public string _strSoundName = "";
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
	private ENUM_SOUND_NAME _eSoundNamePrev;
	private string _strOriginName;
#endif

	private SCManagerSound<ENUM_SOUND_NAME> _pManagerSound;
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

        _pManagerSound = SCManagerSound<ENUM_SOUND_NAME>.instance;

#if UNITY_EDITOR
		_strOriginName = name;
#endif
	}

#if UNITY_EDITOR
	protected override void OnEnableObject()
    {
        ENUM_SOUND_NAME eSoundName;
        if (_strSoundName.ConvertEnum(out eSoundName))
            _eSoundName = eSoundName;
        else
            _strSoundName = eSoundName.ToString();

		base.OnEnableObject();
	}
#endif

#if UNITY_EDITOR
    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        name = _strOriginName;
    }
#endif

#if UNITY_EDITOR
    protected override void OnUpdate()
    {
        base.OnUpdate();

        if(Application.isPlaying)
        {
			if (_bIsPlaying)
			{
				if(_iLoopCount != 0)
					name = $" {_strOriginName} 재생중.. Repeat : {_iLoopCountCurrent}";
				else
					name = $" {_strOriginName} 재생중..";
			}
			else
				name = _strOriginName;
		}
		else
		{
			// Enum이 다를경우 Enum으로 세팅한 것이기 때문에 Enum을 String에 대입
			if (_eSoundName.CompareTo( _eSoundNamePrev ) != 0)
			{
				_eSoundNamePrev = _eSoundName;
				string strSoundName = _eSoundName.ToString();
				if (_strSoundName.CompareTo( strSoundName ) != 0)
					_strSoundName = _eSoundName.ToString();
			}
			// Enum이 같을 경우 String과 Enum이 다른지 체크후 다르면 String을 기준으로 Enum 대입
			else
			{
				ENUM_SOUND_NAME eSoundName;
				if (_eSoundName.ToString() != _strSoundName)
				{
					if (_strSoundName.ConvertEnum( out eSoundName ))
					{
						_eSoundName = eSoundName;
						_eSoundNamePrev = eSoundName;
					}
				}
			}
		}
	}
#endif

	protected override void OnPlayEventMain()
    {
        base.OnPlayEventMain();

#if UNITY_EDITOR
		if (Application.isPlaying == false) return;
#endif

		if (_pManagerSound == null)
            _pManagerSound = SCManagerSound<ENUM_SOUND_NAME>.instance;

		if (_pManagerSound != null)
		{
            ProcPlaySound();

            _iLoopCountCurrent = _iLoopCount;
			_bIsPlaying = true;
		}
		else
		{
			EventDelayExcuteCallBack(DoPlayEvent_Main, 0.1f);
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcFinishSound()
	{
		if (_iLoopCount != 0 && _iLoopCountCurrent-- > 0) // 반복 횟수가 0이 아니고 반복 횟수가 아직 0이 아니라면..
            EventDelayExcuteCallBack( ProcPlaySound, _fLoopDelay );
		else
		{
            // 반복 횟수가 0이거나 반복 횟수가 다 끝났다면..

			p_listEvent_FinishSound.Invoke();
			_bIsPlaying = !_bIsLoop;
            
			if (_bIsLoop)
            {
                if (_fLoopDelay != 0f)
                    EventDelayExcuteCallBack(DoPlayEvent_Main, _fLoopDelay);
                else
                    DoPlayEvent_Main();
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
        // 어레이가 있으면 어레이부터 실행한다.
        // 현재 프로젝트에 이미 탑재된 코드가 있어서 병행.
        // 차후 어레이만 체크 후 플레이 하도록..
        if (_arrPlayAudioClip != null && _arrPlayAudioClip.Length >= 1)
        {
            AudioClip pClipRandom = _arrPlayAudioClip.GetRandomItem();
            pSlot = _pManagerSound.DoPlaySoundEffect_OrNull(pClipRandom, _fSoundVolume);
        }
        else
        {
            if (_pPlayAudioClip != null)
                pSlot = _pManagerSound.DoPlaySoundEffect_OrNull(_pPlayAudioClip, _fSoundVolume);
            else
                pSlot = _pManagerSound.DoPlaySoundEffect_OrNull(_eSoundName, _fSoundVolume);
        }

        if(pSlot != null && _pAudioSource != null)
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
