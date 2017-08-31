using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 5:21:49
   Description : 
   Edit Log    : 
   ============================================ */

public class CCompoEventTrigger : CUIObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EInputType
    {
        None,
        
        OnEnable,
        OnClick,
        OnPress,

		OnTriggerEnter,

		OnDestroy,
    }

	/* public - Field declaration            */

	public event System.Action<Collider> p_OnTriggerEnter;
	public event System.Action<bool> p_OnPress;

	public List<EventDelegate> p_OnEventAction = new List<EventDelegate>();

    public EInputType p_eInputType = EInputType.None;

    /* protected - Field declaration         */

    /* private - Field declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoPlayEvent()
    {
        EventDelegate.Execute(p_OnEventAction);

        OnPlayEvent();
    }
    
    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    virtual protected void OnPlayEvent() { }

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_eInputType == EInputType.OnEnable)
            DoPlayEvent();
    }

    protected override void OnUIClick()
    {
        base.OnUIClick();

        if(p_eInputType == EInputType.OnClick)
            DoPlayEvent();
    }

    protected override void OnUIPress(bool bPress)
    {
        base.OnUIPress(bPress);

        if (bPress && p_eInputType == EInputType.OnPress)
		{
			if (p_OnPress != null)
				p_OnPress(bPress);

			DoPlayEvent();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (p_eInputType != EInputType.OnTriggerEnter) return;
		if (p_OnTriggerEnter == null) return;

		p_OnTriggerEnter(other);
		DoPlayEvent();
	}

	private void OnDestroy()
	{
		if (p_eInputType != EInputType.OnDestroy) return;

		DoPlayEvent();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
