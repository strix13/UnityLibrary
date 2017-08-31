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
public struct SINI_Sound
{
	public string strSoundName;
	public float fVolume;
	public int iGroupNumber;
}

public class SCManagerSound<ENUM_SOUND_NAME> : SCManagerResourceBase<SCManagerSound<ENUM_SOUND_NAME>, ENUM_SOUND_NAME, AudioClip>
    where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
{
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
    private LinkedList<CSoundSlot> _linkedListNotUseSlot = new LinkedList<CSoundSlot>();
    private Dictionary<ENUM_SOUND_NAME, float> _mapSoundVolume = new Dictionary<ENUM_SOUND_NAME, float>();
    private Dictionary<int, List<ENUM_SOUND_NAME>> _mapGroupSound = new Dictionary<int, List<ENUM_SOUND_NAME>>();

    private EVolumeOff _eVolumeOff;
    private int _iSlotPoolingCount = 100;
    private float _fMainVolume = 1f;

    private CSoundSlot _pSlotBGM;
    private bool _bMakeSlot_OnNotFoundDisable = false;

    private EventDelegate.Callback _CallBackOnFinishBGM;

    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //

	public bool DoCheckIsPlayingBGM( ENUM_SOUND_NAME eSound )
	{
		bool bIsPlaying = false;
		if (_pSlotBGM.CheckIsPlaying())
			bIsPlaying = _pSlotBGM.p_pAudioSource.clip.name == DoGetResource_Origin( eSound ).name;

		return bIsPlaying;
	}

	public void DoPlayBGM(ENUM_SOUND_NAME eSound, EventDelegate.Callback CallBackOnFinishBGM)
    {
        float fVolume = 0f;
        if (_mapSoundVolume.ContainsKey(eSound))
            fVolume = _mapSoundVolume[eSound] * _fMainVolume;
        else
            fVolume = _fMainVolume;

		//Debug.Log( eSound + "Volume : " + fVolume );

        if (_pSlotBGM.CheckIsPlaying())
            _pSlotBGM.DoSetFadeOut(DoGetResource_Origin(eSound), fVolume );
        else
            _pSlotBGM.DoPlaySound(DoGetResource_Origin(eSound), fVolume);

        _CallBackOnFinishBGM = CallBackOnFinishBGM;
    }

    public CSoundSlot DoPlaySoundEffect(ENUM_SOUND_NAME eSound, float fVolume = 1f)
    {
        //Debug.Log("Play Sound : " + eSound);
        CSoundSlot pSoundSlot = FindDisableSlot();
        if (_bMakeSlot_OnNotFoundDisable && pSoundSlot == null)
        {
            MakeSoundSlot();
            pSoundSlot = FindDisableSlot();
        }

        if (pSoundSlot != null)
        {
            if (_mapSoundVolume.ContainsKey(eSound))
                pSoundSlot.DoPlaySound(DoGetResource_Origin(eSound), _mapSoundVolume[eSound] * fVolume * _fMainVolume );
            else
                pSoundSlot.DoPlaySound(DoGetResource_Origin(eSound), fVolume * _fMainVolume);
        }

        return pSoundSlot;
    }

	public CSoundSlot DoPlaySoundEffect_Loop(ENUM_SOUND_NAME eSound)
    {
        CSoundSlot pSoundSlot = DoPlaySoundEffect(eSound);
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

            return DoPlaySoundEffect(_mapGroupSound[iHashCode][iRandomIndex]);
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

    public void DoSetVolume(float fVolume)
    {
        _fMainVolume = fVolume;
    }

    public void DoSetOption(bool bMakeSlotOnNotFoundDisable)
    {
        _bMakeSlot_OnNotFoundDisable = bMakeSlotOnNotFoundDisable;
    }

    public void DoStopAllSound(bool bIsBGMSoundOff = true)
    {
		if(bIsBGMSoundOff)
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

        _fMainVolume = fMainVolume;
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
        if (_linkedListNotUseSlot.Contains(pSlot))
            _linkedListNotUseSlot.Remove(pSlot);
    }

    public void EventOnSlotFinishClip(CSoundSlot pSlot)
    {
        if (pSlot == _pSlotBGM)
		{
			if(_CallBackOnFinishBGM != null)
				_CallBackOnFinishBGM();
		}
		else
            _linkedListNotUseSlot.AddLast(pSlot);
    }

    // ========================================================================== //

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //


    protected override void OnMakeClass(MonoBehaviour pBaseClass, ref bool bIsMultipleResource)
    {
        base.OnMakeClass(pBaseClass, ref bIsMultipleResource);
        
        for (int i = 0; i < _iSlotPoolingCount; i++)
            MakeSoundSlot();

        _pSlotBGM = FindDisableSlot();
        EventOnSlotPlayClip(_pSlotBGM);
    }

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    private void MakeSoundSlot()
    {
        GameObject pObject = new GameObject(string.Format("SoundSlot_{0}", _listSoundSlotAll.Count));
        Transform pTrans = pObject.transform;

        pTrans.SetParent(_pBase.transform);
        pTrans.localRotation = Quaternion.identity;
        pTrans.gameObject.SetActive(false);

        CSoundSlot pSlot = pObject.AddComponent<CSoundSlot>();
        pSlot.EventInitSoundSlot(_pBase, "EventOnSlotPlayClip", "EventOnSlotFinishClip");
        _listSoundSlotAll.Add(pSlot);
        _linkedListNotUseSlot.AddLast(pSlot);
    }

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

    private CSoundSlot FindDisableSlot()
    {
        CSoundSlot pFindSlot = null;
        if (_linkedListNotUseSlot.Count != 0)
        {
            pFindSlot = _linkedListNotUseSlot.First.Value;
            _linkedListNotUseSlot.RemoveFirst();
        }

        return pFindSlot;
    }
}
