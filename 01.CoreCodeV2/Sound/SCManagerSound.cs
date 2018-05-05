using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0414

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 1:23:00
// Description : BGM 및 SoundEffect의 볼륨 등을 쉽게 제어하고, 변경할 수 있게 하기 위함.
//               SoundEffect는 풀링 방식을 사용.
// Edit Log    : 
// ============================================ 

[System.Serializable]
public struct SINI_Sound : IDictionaryItem<string>
{
	public SINI_Sound( string strSoundName , float fVolume)
	{
		this.strSoundName = strSoundName;
		this.fVolume = fVolume;
		iGroupNumber = 0;
	}

	public string IDictionaryItem_GetKey()
	{
		return strSoundName;
	}

	public string strSoundName;
	public float fVolume;
	public int iGroupNumber;
}

public class SCManagerSound<ENUM_SOUND_NAME> : SCManagerResourceBase<SCManagerSound<ENUM_SOUND_NAME>, ENUM_SOUND_NAME, AudioClip>
    where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
{
    private const int const_iDefault_SoundSlot_PoolingCount = 10;

    // ===================================== //
    // public - Variable declaration         //
    // ===================================== //

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    private List<CSoundSlot> _listSoundSlotAll = new List<CSoundSlot>();
    private HashSet<CSoundSlot> _queueNotUseSlot = new HashSet<CSoundSlot>();
    private Dictionary<string, CSoundSlot> _mapCurrentPlayingSound = new Dictionary<string, CSoundSlot>();

    private Dictionary<ENUM_SOUND_NAME, float> _mapSoundVolume = new Dictionary<ENUM_SOUND_NAME, float>();
    private Dictionary<int, List<ENUM_SOUND_NAME>> _mapGroupSound = new Dictionary<int, List<ENUM_SOUND_NAME>>();

    private EVolumeOff _eVolumeOff;

    private bool _bIsMute = false;
    private bool _bPause = false;

    private float _fVolumeEffect = 1f; public float p_fVolumeEffect { get { return _fVolumeEffect; } }
    private float _fVolumeBGM; public float p_fVolumeBGM { get { return _fVolumeBGM; } }

    private float _fVolumeBackup_Effect;
    private float _fVolumeBackup_BGM;

    private float _fVolumeBackUp_CurrentBGM;

    private CSoundSlot _pSlotBGM;
    static private System.Action _CallBackOnFinishBGM;

    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //

    public bool DoCheckIsPlayingBGM(ENUM_SOUND_NAME eSound)
    {
        bool bIsPlaying = false;
        if (_pSlotBGM.CheckIsPlaying())
            bIsPlaying = _pSlotBGM.p_pAudioSource.clip.name == DoGetResource_Origin(eSound).name;

        return bIsPlaying;
    }

    public void DoPlayBGM(ENUM_SOUND_NAME eSound, System.Action CallBackOnFinishBGM = null)
    {
        float fVolume = 0f;
        if (_mapSoundVolume.ContainsKey(eSound))
            fVolume = _mapSoundVolume[eSound] * _fVolumeBGM;
        else
            fVolume = _fVolumeBGM;

        //Debug.Log( eSound + "Volume : " + fVolume );

        if (_pSlotBGM.CheckIsPlaying())
            _pSlotBGM.DoSetFadeOut(DoGetResource_Origin(eSound), fVolume);
        else
            _pSlotBGM.DoPlaySound(DoGetResource_Origin(eSound), fVolume);

        _CallBackOnFinishBGM = CallBackOnFinishBGM;
    }

    public CSoundSlot DoPlaySoundEffect_OrNull(AudioClip pClip, float fVolume = 1f)
    {
        if (_bIsMute) return null;

        //Debug.Log("Play Sound : " + eSound);
        CSoundSlot pSoundSlot = FindDisableSlot_OrMakeSlot();
        if (pSoundSlot == null) return null;

        if (pSoundSlot != null)
            pSoundSlot.DoPlaySound(pClip, fVolume * _fVolumeEffect);

        return pSoundSlot;
    }
    
    public CSoundSlot DoPlaySoundEffect_OrNull(ENUM_SOUND_NAME eSound, float fVolume = 1f)
    {
		if (_bIsMute) return null;

		// string strSoundName = eSound.ToString();
        //Debug.Log("Play Sound : " + eSound);
        CSoundSlot pSoundSlot = FindDisableSlot_OrMakeSlot();
		if (pSoundSlot == null) return null;

        if (pSoundSlot != null)
        {
            if (_mapSoundVolume.ContainsKey(eSound))
                pSoundSlot.DoPlaySound(DoGetResource_Origin(eSound), _mapSoundVolume[eSound] * fVolume * _fVolumeEffect );
            else
                pSoundSlot.DoPlaySound(DoGetResource_Origin(eSound), fVolume * _fVolumeEffect);
		}

        return pSoundSlot;
    }

	public CSoundSlot DoPlaySoundEffect_OnlySinglePlay_OrNull( ENUM_SOUND_NAME eSound, float fVolume = 1f )
	{
		if (_bIsMute) return null;

		string strSoundName = eSound.ToString();
		if (_mapCurrentPlayingSound.ContainsKey( strSoundName ))
		{
			CSoundSlot pSlotCurrentPlaying = _mapCurrentPlayingSound[strSoundName];
			//pSlotCurrentPlaying.DoStopSound();
			pSlotCurrentPlaying.DoPlaySound();

			return pSlotCurrentPlaying;
		}

		//Debug.Log("Play Sound : " + eSound);
		CSoundSlot pSoundSlot = FindDisableSlot_OrMakeSlot();
		if (pSoundSlot == null) return null;
		//{
		//    MakeSoundSlot();
		//    pSoundSlot = FindDisableSlot();
		//}

		if (pSoundSlot != null)
		{
			if (_mapSoundVolume.ContainsKey( eSound ))
				pSoundSlot.DoPlaySound( DoGetResource_Origin( eSound ), _mapSoundVolume[eSound] * fVolume * _fVolumeEffect );
			else
				pSoundSlot.DoPlaySound( DoGetResource_Origin( eSound ), fVolume * _fVolumeEffect );

			_mapCurrentPlayingSound.Add( strSoundName, pSoundSlot );
		}

		return pSoundSlot;
	}

	public CSoundSlot DoPlaySoundEffect_Loop(ENUM_SOUND_NAME eSound, float fVolume)
    {
        CSoundSlot pSoundSlot = DoPlaySoundEffect_OrNull(eSound, fVolume );
		if(pSoundSlot != null)
	        pSoundSlot.DoPlaySoundLoop();

        return pSoundSlot;
    }

    public CSoundSlot DoPlaySoundEffect_RandomGroup<ENUM_SOUNDGROUP_NAME>(ENUM_SOUNDGROUP_NAME eGroup)
    {
        int iHashCode = eGroup.GetHashCode();
        if (_mapGroupSound[iHashCode] == null)
        {
            Debug.LogWarning("그룹이 없습니다. 그룹에 사운드를 등록해주세요 " + eGroup.ToString());
            return null;
        }
        else
        {
            int iRandomCount = _mapGroupSound[iHashCode].Count;
            int iRandomIndex = Random.Range(0, iRandomCount);

            return DoPlaySoundEffect_OrNull(_mapGroupSound[iHashCode][iRandomIndex]);
        }
    }

    // ===================================== //
    // public - [Getter And Setter] Function //
    // ===================================== //

    public void EventRegist_Group<ENUM_SOUNDGROUP_NAME>(ENUM_SOUND_NAME eSound, ENUM_SOUNDGROUP_NAME eGroup)
        where ENUM_SOUNDGROUP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        int iHashCode = eGroup.GetHashCode();
        if (_mapGroupSound.ContainsKey(iHashCode) == false)
            _mapGroupSound.Add(iHashCode, new List<ENUM_SOUND_NAME>());

        _mapGroupSound[iHashCode].Add(eSound);
    }


    public AudioClip GetAudioClip(ENUM_SOUND_NAME eSound)
    {
        return DoGetResource_Origin(eSound);
    }

    public void DoSetMute(bool bMute, bool bAudioSourceControl = false)
	{
		if (_bIsMute == bMute) return;
		_bIsMute = bMute;

        if (bAudioSourceControl)
        {
            AudioSource[] arrAudioSource = GameObject.FindObjectsOfType<AudioSource>();
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].mute = bMute;
        }

        if (bMute)
		{
			_fVolumeBackup_BGM = _fVolumeBGM;
			_fVolumeBackup_Effect = _fVolumeEffect;

			_fVolumeBGM = 0f;
			_fVolumeEffect = 0f;

			_fVolumeBackUp_CurrentBGM = _pSlotBGM.DoGetVolume();
		}
		else
		{
			_fVolumeBGM = _fVolumeBackup_BGM;
			_fVolumeEffect = _fVolumeBackup_Effect;

			_pSlotBGM.DoSetVolume( _fVolumeBackUp_CurrentBGM );
		}
    }

    public void DoSetPause(bool bPause)
    {
        _bPause = bPause;

        AudioSource[] arrAudioSource = GameObject.FindObjectsOfType<AudioSource>();
        if(_bPause)
        {
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].Pause();
        }
        else
        {
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].Play();
        }
    }

    public void DoSetVolumeEffect(float fVolumeEffect)
    {
        _fVolumeEffect = fVolumeEffect;
	}

	public void DoSetVolumeBGM( float fVolumeBGM )
	{
		float fVolume = fVolumeBGM;

		if (_pSlotBGM.p_pAudioSource.clip != null)
		{
			ENUM_SOUND_NAME eSoundName = _pSlotBGM.p_pAudioSource.clip.name.ToString().ConvertEnum<ENUM_SOUND_NAME>();
			if (_mapSoundVolume.ContainsKey( eSoundName ))
				fVolume = _mapSoundVolume[eSoundName] * fVolumeBGM;
		}

		_fVolumeBGM = fVolumeBGM;
		_pSlotBGM.p_pAudioSource.volume = fVolume;
	}

	public void DoEnableSoundBGM(bool bEnable)
	{
		DoSetVolumeBGM(bEnable ? 0.5f : 0);
	}

	public void DoEnableSoundEffect(bool bEnable)
	{
		DoSetVolumeEffect(bEnable ? 0.5f : 0);
	}

	public void DoStopAllSound(bool bIsBGMSoundOff = true, bool bAudioSourceControl = false)
    {
        if (bAudioSourceControl)
        {
            AudioSource[] arrAudioSource = GameObject.FindObjectsOfType<AudioSource>();
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].Stop();
        }

        if (bIsBGMSoundOff)
	        _pSlotBGM.DoStopSound();

        for (int i = 0; i < _listSoundSlotAll.Count; i++)
            _listSoundSlotAll[i].DoStopSound();
    }

    // ===================================== //
    // public - [Event] Function             //
    // 프랜드 객체가 요청                    //
    // ===================================== //

    public void EventSetINI(SINI_Sound[] arrSoundINI, float fMainVolume, EVolumeOff eVolumeOff)
    {
        if (arrSoundINI == null)
        {
            Debug.LogWarning("에러, INI 세팅 바람");
            return;
        }

        _fVolumeEffect = fMainVolume;
        _eVolumeOff = eVolumeOff;
        for (int i = 0; i < arrSoundINI.Length; i++)
        {
			SINI_Sound sSoundInfo = arrSoundINI[i];
			if (sSoundInfo.strSoundName == "")
                continue;

            try
            {
                ENUM_SOUND_NAME eSoundName = (ENUM_SOUND_NAME)System.Enum.Parse(typeof(ENUM_SOUND_NAME), arrSoundINI[i].strSoundName);
                _mapSoundVolume[eSoundName] = sSoundInfo.fVolume;
				if (_mapGroupSound.ContainsKey( sSoundInfo.iGroupNumber) == false )
					_mapGroupSound.Add( sSoundInfo.iGroupNumber, new List<ENUM_SOUND_NAME>() );

				_mapGroupSound[sSoundInfo.iGroupNumber].Add( eSoundName );
			}
            catch
            {
                Debug.LogWarning(string.Format("Sound INI 파일에 {0}에 없는 파일이 존재합니다. {1}", typeof(ENUM_SOUND_NAME).ToString(), arrSoundINI[i].strSoundName));
            }
        }
    }

    public void EventOnSlotPlayClip(CSoundSlot pSlot)
    {
        _queueNotUseSlot.Remove(pSlot);
	}

    public void EventOnSlotFinishClip(CSoundSlot pSlot)
    {
        if (pSlot == _pSlotBGM)
		{
			if(_CallBackOnFinishBGM != null)
			{
				var OnFinishBackup = _CallBackOnFinishBGM;
				_CallBackOnFinishBGM = null;
				OnFinishBackup();
			}
		}
		else
		{
			_queueNotUseSlot.Add( pSlot );
			if (pSlot.p_pAudioSource.clip != null)
			{
				string strClipName = pSlot.p_pAudioSource.clip.name;
				if (_mapCurrentPlayingSound.ContainsKey( strClipName ))
					_mapCurrentPlayingSound.Remove( strClipName );
			}
		}
    }

    // ========================================================================== //

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //


    protected override void OnMakeClass(MonoBehaviour pBaseClass, ref bool bIsMultipleResource)
    {
        base.OnMakeClass(pBaseClass, ref bIsMultipleResource);
        
        for (int i = 0; i < const_iDefault_SoundSlot_PoolingCount; i++)
            MakeSoundSlot();

        _pSlotBGM = FindDisableSlot_OrMakeSlot();
        EventOnSlotPlayClip(_pSlotBGM);
    }

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    private CSoundSlot MakeSoundSlot()
    {
		if (_pBase == null) return null;

		GameObject pObject = new GameObject(string.Format("SoundSlot_{0}", _listSoundSlotAll.Count));
        Transform pTrans = pObject.transform;

        pTrans.SetParent(_pBase.transform);
        pTrans.localRotation = Quaternion.identity;
        pTrans.gameObject.SetActive(false);

        CSoundSlot pSlot = pObject.AddComponent<CSoundSlot>();
        pSlot.EventInitSoundSlot(_pBase, EventOnSlotPlayClip, EventOnSlotFinishClip);
        _listSoundSlotAll.Add(pSlot);
        _queueNotUseSlot.Add(pSlot);

		return pSlot;
	}

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

    private CSoundSlot FindDisableSlot_OrMakeSlot()
    {
        CSoundSlot pFindSlot = null;
		IEnumerator<CSoundSlot> pEnum = _queueNotUseSlot.GetEnumerator();
		if (pEnum.MoveNext())
		{
			pFindSlot = pEnum.Current;
			_queueNotUseSlot.Remove( pFindSlot );
		}

		if (pFindSlot == null)
			pFindSlot = MakeSoundSlot();

		return pFindSlot;
    }
}
