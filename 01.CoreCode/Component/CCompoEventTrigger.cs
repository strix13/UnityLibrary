using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 5:21:49
   Description : 
   Edit Log    : 
   ============================================ */

public class CCompoEventTrigger : CObjectBase, IPointerDownHandler, IPointerUpHandler
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EInputType
    {
        None = 0,
        
        OnEnable,
        OnClick,
        OnPress,

		OnTriggerEnter,

		OnDestroy,
		OnDisable,
		OnCollisionEnter,
		OnPress_True,

		OnShow,
		OnHide,
	}
	
	/* public - Field declaration            */

	public event System.Action<GameObject> p_OnPhysicsEnter;
	public event System.Action<bool> p_OnPress;

	public EInputType p_eInputType_Main = EInputType.None;
	public float p_fDelayTrigger = 0f;
	public UnityEngine.Events.UnityEvent p_listEvent_Main = new UnityEvent();

	/* protected - Field declaration         */

	/* private - Field declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoRecieveMessage_OnShow()
	{
		if (p_eInputType_Main == EInputType.OnShow)
			DoPlayEvent_Main();
	}

	public void DoRecieveMessage_OnHide()
	{
		if (p_eInputType_Main == EInputType.OnHide)
			DoPlayEvent_Main();
	}
	
	public void DoAddEvent_Main( UnityAction CallBack )
	{
		p_listEvent_Main.AddListener( CallBack );
	}
	
	public void DoPlayEvent_Main()
	{
		if (p_fDelayTrigger != 0f)
			EventDelayExcuteCallBack( ProcPlayEvent, p_fDelayTrigger );
		else
			ProcPlayEvent();
	}
	
    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    virtual protected void OnPlayEventMain() { }

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

		if (p_eInputType_Main == EInputType.OnEnable)
	        DoPlayEvent_Main();
    }

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		if (p_eInputType_Main == EInputType.OnDisable)
			DoPlayEvent_Main();
	}
	
	void OnClick()
	{
		if (p_eInputType_Main == EInputType.OnClick)
			DoPlayEvent_Main();
	}

	void OnPress( bool bPress )
	{
		if (p_eInputType_Main == EInputType.OnPress)
		{
			DoPlayEvent_Main();
			if (p_OnPress != null)
				p_OnPress( bPress );
		}

		if(bPress)
		{
			if (p_eInputType_Main == EInputType.OnPress_True)
			{
				DoPlayEvent_Main();
				if (p_OnPress != null)
					p_OnPress( bPress );
			}
		}
	}

	private void OnTriggerEnter(Collider collision )
	{
		if (p_eInputType_Main == EInputType.OnTriggerEnter)
		{
			DoPlayEvent_Main();
			if (p_OnPhysicsEnter != null)
				p_OnPhysicsEnter( collision.gameObject );
		}
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if (p_eInputType_Main == EInputType.OnTriggerEnter)
		{
			DoPlayEvent_Main();
			if (p_OnPhysicsEnter != null)
				p_OnPhysicsEnter( collision.gameObject );
		}
	}

	private void OnCollisionEnter( Collision collision )
	{
		if (p_eInputType_Main == EInputType.OnCollisionEnter)
		{
			DoPlayEvent_Main();
			if (p_OnPhysicsEnter != null)
				p_OnPhysicsEnter( collision.gameObject );
		}
	}

	private void OnCollisionEnter2D( Collision2D collision )
	{
		if (p_eInputType_Main == EInputType.OnCollisionEnter)
		{
			DoPlayEvent_Main();
			if (p_OnPhysicsEnter != null)
				p_OnPhysicsEnter( collision.gameObject );
		}
	}


	private void OnDestroy()
	{
		if (p_eInputType_Main == EInputType.OnDestroy)
			DoPlayEvent_Main();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcPlayEvent()
	{
		p_listEvent_Main.Invoke();

		OnPlayEventMain();
	}

	public void OnPointerDown( PointerEventData eventData )
	{
		OnPress( true );
	}

	public void OnPointerUp( PointerEventData eventData )
	{
		OnPress( false );
	}

	/* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
