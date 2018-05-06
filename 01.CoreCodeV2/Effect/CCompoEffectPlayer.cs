#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCompoEffectPlayer : CCompoEventTrigger
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Header("이펙트 끝날때 이벤트")]
    public UnityEngine.Events.UnityEvent p_listEvent_FinishEffect = new UnityEngine.Events.UnityEvent();

    [Header("플레이할 이펙트 -  다수일 경우 랜덤 재생")]
	public CEffect[] _arrEffectPlay;

    [Rename_Inspector("플레이중인 이펙트가 활성중이면 끕니다")]
    public bool _bIsDisableEffectPlayed_When_EffectPlaying = true;

    /* protected - Field declaration         */

    /* private - Field declaration           */

    CEffect _pEffectPlaying;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    protected override void OnPlayEventMain()
	{
		base.OnPlayEventMain();

        if (_bIsDisableEffectPlayed_When_EffectPlaying && _pEffectPlaying)
        {
            _pEffectPlaying.gameObject.SetActive(false);
        }

        if (_arrEffectPlay == null || _arrEffectPlay.Length == 0)
        {
            Debug.LogError(name + "이펙트 플레이어인데 이펙트할게 없습니다.", this);
            return;
        }

        CEffect pRandomEffect = _arrEffectPlay.GetRandom();
        _pEffectPlaying = CManagerEffect.instance.DoPlayEffect(pRandomEffect.name, transform.position);
        _pEffectPlaying.p_Event_Effect_OnDisable += PEffectPlaying_p_Event_Effect_OnDisable;
    }

    private void PEffectPlaying_p_Event_Effect_OnDisable(CEffect obj)
    {
        p_listEvent_FinishEffect.Invoke();
        _pEffectPlaying = null;
    }

    #endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
