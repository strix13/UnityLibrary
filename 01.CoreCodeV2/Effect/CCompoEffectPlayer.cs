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
using UnityEditor;

public class CCompoEffectPlayer : CCompoEventTrigger
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public class SEffectPlayInfo : IDictionaryItem<string>
    {
        public string strEffectEvent;

        public List<CEffect> listEffectPlay;

        public CEffect GetRandomEffect()
        {
            return listEffectPlay.GetRandom();
        }

        public string IDictionaryItem_GetKey()
        {
            return strEffectEvent;
        }
    }

    /* public - Field declaration            */

    [Header("이벤트 이펙트 리스트")]
    public List<SEffectPlayInfo> p_listEffectPlayInfo = new List<SEffectPlayInfo>();

    [Header("이펙트 끝날때 이벤트")]
    public UnityEngine.Events.UnityEvent p_listEvent_FinishEffect = new UnityEngine.Events.UnityEvent();

    [Header("플레이할 이펙트 -  다수일 경우 랜덤 재생")]
	public CEffect[] _arrEffectPlay;

    [Rename_Inspector("플레이중인 이펙트가 활성중이면 끕니다")]
    public bool _bIsDisableEffectPlayed_When_EffectPlaying = true;

    [Rename_Inspector("이펙트 플레이 포지션 오프셋")]
    public Vector3 _vecEffectPos_Offset;

    /* protected - Field declaration         */

    /* private - Field declaration           */

    Dictionary<string, SEffectPlayInfo> _mapEffectPlayInfo = new Dictionary<string, SEffectPlayInfo>();
    CEffect _pEffectPlaying;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPlayEffect()
    {
        DoPlayEventTrigger();
    }

    public void DoPlayEffect(Vector3 vecPos)
    {
        DoPlayEventTrigger();
        if (_pEffectPlaying != null)
            _pEffectPlaying.transform.position = vecPos;
    }

    public CEffect DoPlayEffect(string strEffectEvent)
    {
        if (_mapEffectPlayInfo.ContainsKey(strEffectEvent) == false)
            return null;

        SEffectPlayInfo pEffectPlayInfo = _mapEffectPlayInfo[strEffectEvent];
        return PlayEffect(pEffectPlayInfo.GetRandomEffect());
    }

    public CEffect DoPlayEffect(string strEffectEvent, Transform pTransform)
    {
        if (_mapEffectPlayInfo.ContainsKey(strEffectEvent) == false)
            return null;

        SEffectPlayInfo pEffectPlayInfo = _mapEffectPlayInfo[strEffectEvent];
        return PlayEffect(pEffectPlayInfo.GetRandomEffect(), pTransform);
    }

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _mapEffectPlayInfo.DoAddItem(p_listEffectPlayInfo);
    }

    protected override void OnPlayEvent()
    {
        base.OnPlayEvent();

        if (_bIsQuitApplciation)
            return;

        if (_bIsDisableEffectPlayed_When_EffectPlaying && _pEffectPlaying)
        {
            _pEffectPlaying.gameObject.SetActive(false);
        }

        if (_arrEffectPlay == null || _arrEffectPlay.Length == 0)
        {
            Debug.LogError(name + "이펙트 플레이어인데 이펙트할게 없습니다.", this);
            return;
        }

        PlayEffect(_arrEffectPlay.GetRandom());
    }

    private CEffect PlayEffect(CEffect pEffectPlay)
    {
        if (pEffectPlay == null)
            return null;

        _pEffectPlaying = CManagerEffect.instance.DoPlayEffect(pEffectPlay.name, transform.position + _vecEffectPos_Offset);
        _pEffectPlaying.p_Event_Effect_OnDisable += PEffectPlaying_p_Event_Effect_OnDisable;

        return _pEffectPlaying;
    }

    private CEffect PlayEffect(CEffect pEffectPlay, Transform pTransform)
    {
        if (pEffectPlay == null)
            return null;

        _pEffectPlaying = CManagerEffect.instance.DoPlayEffect(pEffectPlay.name, pTransform, transform.position);
        _pEffectPlaying.p_Event_Effect_OnDisable += PEffectPlaying_p_Event_Effect_OnDisable;

        return _pEffectPlaying;
    }

    private void PEffectPlaying_p_Event_Effect_OnDisable(CEffect obj)
    {
        p_listEvent_FinishEffect.Invoke();
        _pEffectPlaying = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        GUIStyle pStyle = new GUIStyle();
        pStyle.normal.textColor = Color.green;
        Handles.Label(transform.position + _vecEffectPos_Offset + (Vector3.right * 3f), "Effect Pos", pStyle);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + _vecEffectPos_Offset, 1f);
    }
#endif

#endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
