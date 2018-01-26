using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 1:48:43
// Description : 
// Edit Log    : 
// ============================================ 

public class CSoundSlot : CObjectBase
{
	// ===================================== //
	// public - Variable declaration         //
	// ===================================== //

	public delegate void OnEventSoundClip( CSoundSlot pSoundSlot );

    public List<OnEventSoundClip> OnStartClip = new List<OnEventSoundClip>();
    public List<OnEventSoundClip> OnFinishedClip = new List<OnEventSoundClip>();

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    [SerializeField]
    private AudioClip _pAudioClip;
    private AudioClip _pAudioClipNext;

    private AudioSource _pAudioSource;  public AudioSource p_pAudioSource {  get { return _pAudioSource; } }
    private float _fVolume;
    private float _fVolumeNext;
    private bool _bLoopSound;

    // private bool _bIsFadeIn;

    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //
	
    public void DoPlaySound()
    {
		ProcPlaySound(_pAudioClip);
    }

    public void DoPlaySound(AudioClip pAudioClip, float fVolume)
    {
        _fVolume = fVolume;
        _bLoopSound = false;
        ProcPlaySound(pAudioClip);
    }

    public void DoPlaySoundLoop()
    {
        _bLoopSound = true;
        ProcPlaySound(_pAudioClip);
    }

    public void DoStopSound()
    {
		if (enabled == false)
			return;

		for (int i = 0; i < OnFinishedClip.Count; i++)
			OnFinishedClip[i](this);

        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public bool CheckIsPlaying()
    {
        return _pAudioSource.isPlaying;
    }

    // ===================================== //
    // public - [Event] Function             //
    // 프랜드 객체가 요청                    //
    // ===================================== //

	public float DoGetVolume()
	{
		return _pAudioSource.volume;
	}

	public void DoSetVolume(float fVolume)
	{
		_pAudioSource.volume = fVolume;
	}

    public void DoSetFadeIn()
    {
        StartCoroutine(CoPlayFadeInOut(false));
    }

    public void DoSetFadeOut(AudioClip pAudioClipNext, float fNextVolume)
    {
        _pAudioClipNext = pAudioClipNext;
        _fVolumeNext = fNextVolume;
        StartCoroutine(CoPlayFadeInOut(true));
    }

    public void EventInitSoundSlot(MonoBehaviour pManager, OnEventSoundClip OnMethodOnStart, OnEventSoundClip OnMethodOnFinish )
    {
		OnStartClip.Add( OnMethodOnStart );
		OnFinishedClip.Add( OnMethodOnFinish );

        _pAudioSource = gameObject.AddComponent<AudioSource>();
        _pAudioSource.playOnAwake = false;
    }

    // ===================================== //
    // public - [Getter And Setter] Function //
    // ===================================== //

    // ========================================================================== //

    // ===================================== //
    // protected - [Event] Function          //
    // 프랜드 객체가 요청                    //
    // ===================================== //

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //
    
    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //
    
    private void ProcPlaySound(AudioClip pAudioClip)
    {
		for (int i = 0; i < OnStartClip.Count; i++)
			OnStartClip[i](this);

        _pAudioClip = pAudioClip;

		if (gameObject != null)
			gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(CoPlaySoundEffect());
    }

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

    private IEnumerator CoPlayFadeInOut(bool bFadeOut)
    {
        float fDestVolume = bFadeOut ? 0f : _fVolume;
        _pAudioSource.volume = bFadeOut ? _fVolume : 0f;
		
		float fFadeProgress = 0f;
        while (fFadeProgress < 1f)
        {
            _pAudioSource.volume = Mathf.Lerp(_pAudioSource.volume, fDestVolume, fFadeProgress);
            fFadeProgress += 0.1f;
            yield return SCManagerYield.GetWaitForSecond(0.1f);
        }

        if(bFadeOut)
        {
            DoPlaySound(_pAudioClipNext, _fVolumeNext);
			DoSetFadeIn();
		}
	}

    private IEnumerator CoPlaySoundEffect()
    {
        _pAudioSource.clip = _pAudioClip;
        _pAudioSource.volume = _fVolume;

        do
        {
            _pAudioSource.Play();

            yield return new WaitForSecondsRealtime(_pAudioClip.length);
        } while (_bLoopSound);

        gameObject.SetActive(false);
		for (int i = 0; i < OnFinishedClip.Count; i++)
			OnFinishedClip[i]( this );
	}
}
