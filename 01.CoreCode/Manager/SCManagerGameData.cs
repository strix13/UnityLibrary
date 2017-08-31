using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-24 오후 8:30:30
   Description : 
   Edit Log    : 
   ============================================ */

public class SCManagerGameData<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>
    where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where ENUM_EFFECT_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where CLASS_EFFECT : CEffectBase<CLASS_EFFECT, ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_SOUNDPLAYER>
    where CLASS_SOUNDPLAYER : CSoundPlayerBase<ENUM_SOUND_NAME>
{
    /* const & readonly declaration             */

    private const string const_strLocalPath_Sound = "Sound";
    private const string const_strLocalPath_Effect = "Effect";

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    static private SCManagerSound<ENUM_SOUND_NAME> _pManagerSound;    static public SCManagerSound<ENUM_SOUND_NAME> p_ManagerSound { get { return _pManagerSound; } }
    static private SCManagerEffect<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER> _pManagerEffect; static public SCManagerEffect<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER> p_ManagerEffect {  get { return _pManagerEffect; } }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    static public SCManagerGameData<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER> DoMakeClass(MonoBehaviour pBaseClass)
    {
        _pManagerSound = SCManagerSound<ENUM_SOUND_NAME>.DoMakeClass(pBaseClass, const_strLocalPath_Sound);
        _pManagerEffect = SCManagerEffect<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>.DoMakeClass(pBaseClass, const_strLocalPath_Effect);

        return new SCManagerGameData<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>();
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
