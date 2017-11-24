using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 5:32:20
   Description : 
   Edit Log    : 
   ============================================ */

[ExecuteInEditMode]
public class CSoundPlayerBase<ENUM_SOUND_NAME> : CCompoEventTrigger
    where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [SerializeField]
    private ENUM_SOUND_NAME _eSoundName;
    [SerializeField]
    private string _strSoundName = "";

    /* protected - Field declaration         */

    /* private - Field declaration           */

    private SCManagerSound<ENUM_SOUND_NAME> _pManagerSound;

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
    }

    protected override void OnEnableObject()
    {
        ENUM_SOUND_NAME eSoundName;
        if (_strSoundName.ConvertEnum(out eSoundName))
            _eSoundName = eSoundName;
        else
            _strSoundName = eSoundName.ToString();

		base.OnEnableObject();
	}

	protected override void OnUpdate()
    {
        base.OnUpdate();

        if(Application.isEditor)
        {
            string strSoundName = _eSoundName.ToString();
            if (_strSoundName.CompareTo(strSoundName) != 0)
                _strSoundName = _eSoundName.ToString();
        }
    }

    protected override void OnPlayEventMain()
    {
        base.OnPlayEventMain();

        if(_pManagerSound == null)
            _pManagerSound = SCManagerSound<ENUM_SOUND_NAME>.instance;

        if(_pManagerSound != null)
            _pManagerSound.DoPlaySoundEffect(_eSoundName);
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
