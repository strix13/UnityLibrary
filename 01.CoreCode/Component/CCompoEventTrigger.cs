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

public class CCompoEventTrigger : CObjectBase, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public struct SColliderWrapper
    {
        public GameObject p_pGameObject { get; private set; }
        public Rigidbody p_pRigidbody { get; private set; }
        public Rigidbody2D p_pRigidbody2D { get; private set; }

        public SColliderWrapper(GameObject pObject)
        {
            p_pGameObject = pObject;
            p_pRigidbody = null;
            p_pRigidbody2D = null;
        }

        public SColliderWrapper(GameObject pObject, Rigidbody pRigidbody)
        {
            p_pGameObject = pObject;
            p_pRigidbody = pRigidbody;
            p_pRigidbody2D = null;
        }

        public SColliderWrapper(GameObject pObject, Rigidbody2D pRigidbody)
        {
            p_pGameObject = pObject;
            p_pRigidbody = null;
            p_pRigidbody2D = pRigidbody;
        }
    }

    public class CYield_IsWaitingEventTrigger : CustomYieldInstruction
    {
        static System.Func<bool> g_OnCheckIsWaiting;

        public static void SetIsWaiting(System.Func<bool> OnCheckIsWaiting)
        {
            g_OnCheckIsWaiting = OnCheckIsWaiting;
        }

        public override bool keepWaiting
        {
            get
            {
                if (g_OnCheckIsWaiting == null)
                    return false;
                else
                    return g_OnCheckIsWaiting();
            }
        }
    }

    [System.Flags]
    public enum EConditionTypeFlags
    {
        None = 0,

        OnAwake = 1 << 1,
        OnEnable = 1 << 2,
        OnDisable = 1 << 3,
        OnDestroy = 1 << 4,

        OnPress_True = 1 << 5,
        OnClick = 1 << 6,
        OnPress = 1 << 7,

        OnUIEvent_Show = 1 << 8,
		OnUIEvent_Hide = 1 << 9,

        OnTriggerEnter = 1 << 10,
        OnTriggerStay = 1 << 11,
        OnTriggerExit = 1 << 12,

        OnCollisionEnter = 1 << 13,
        OnCollisionStay = 1 << 14,
        OnCollisionExit = 1 << 15,
    }

    public enum EPhysicsEvent
    {
        Collision_Enter,
        Collision_Stay,
        Collision_Exit,

        Trigger_Enter,
        Trigger_Stay,
        Trigger_Exit,

    }

    /* public - Field declaration            */

    public delegate void OnPhysicsEvent(SColliderWrapper pObjectEventer, EPhysicsEvent ePhysicsEvent);
    public event OnPhysicsEvent p_Event_OnPhysicsEvent;
	public event System.Action<bool> p_OnPress;

    [Rename_Inspector("디버그 모드")]
    public bool p_bIsDebuging = false;

	[Rename_Inspector( "트리거 작동 조건" )]
	public EConditionTypeFlags p_eConditionType = EConditionTypeFlags.None;
	[Rename_Inspector("트리거 작동 시 처음 딜레이")]
	public float p_fDelayTrigger = 0f;

	public UnityEngine.Events.UnityEvent p_listEvent = new UnityEvent();
    public event System.Action<GameObject> p_Event_IncludeThisObject;

    /* protected - Field declaration         */

    /* private - Field declaration           */
    
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoRecieveMessage_OnShow()
	{
		if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnUIEvent_Show))
			DoPlayEventTrigger();
	}

	public void DoRecieveMessage_OnHide()
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnUIEvent_Hide))
			DoPlayEventTrigger();
	}
	
	public void DoAddEventTrigger( UnityAction CallBack )
	{
		p_listEvent.AddListener( CallBack );
	}
	
	public void DoPlayEventTrigger()
	{
		if (this == null) return;

		if (p_fDelayTrigger != 0f)
            ProcDelayExcuteCallBack( ProcPlayEvent, p_fDelayTrigger );
		else
			ProcPlayEvent();
	}

    public void DoSetActiveObject_True()
    {
        gameObject.SetActive(true);
    }

    public void DoSetActiveObject_False()
    {
        gameObject.SetActive(false);
    }


    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    virtual protected void OnPlayEvent() { }

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnAwake))
			DoPlayEventTrigger();
	}

	protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnEnable))
	        DoPlayEventTrigger();
    }

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnDisable))
			DoPlayEventTrigger();
	}
	
	void OnClick()
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnClick))
			DoPlayEventTrigger();
	}

	void OnPress( bool bPress )
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnPress))
		{
			DoPlayEventTrigger();
			if (p_OnPress != null)
				p_OnPress( bPress );
		}

        if (bPress)
		{
            if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnPress_True))
			{
				DoPlayEventTrigger();
				if (p_OnPress != null)
					p_OnPress( bPress );
			}
		}
	}

	private void OnTriggerEnter(Collider collision )
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnTriggerEnter))
		{
			DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.attachedRigidbody), EPhysicsEvent.Trigger_Enter);
		}
	}

    private void OnTriggerEnter2D( Collider2D collision )
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnTriggerEnter))
		{
			DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.attachedRigidbody), EPhysicsEvent.Trigger_Enter);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnTriggerStay))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.attachedRigidbody), EPhysicsEvent.Trigger_Stay);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnTriggerStay))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.attachedRigidbody), EPhysicsEvent.Trigger_Stay);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnTriggerExit))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.attachedRigidbody), EPhysicsEvent.Trigger_Exit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnTriggerExit))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.attachedRigidbody), EPhysicsEvent.Trigger_Exit);
        }
    }

    private void OnCollisionEnter( Collision collision )
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnCollisionEnter))
		{
			DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.rigidbody), EPhysicsEvent.Collision_Enter);
        }
    }

    private void OnCollisionEnter2D( Collision2D collision )
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnCollisionEnter))
		{
			DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.rigidbody), EPhysicsEvent.Collision_Enter);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnCollisionStay))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.rigidbody), EPhysicsEvent.Collision_Stay);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnCollisionStay))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.rigidbody), EPhysicsEvent.Collision_Stay);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnCollisionExit))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.rigidbody), EPhysicsEvent.Collision_Exit);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnCollisionExit))
        {
            DoPlayEventTrigger();
            p_Event_OnPhysicsEvent.Invoke(new SColliderWrapper(collision.gameObject, collision.rigidbody), EPhysicsEvent.Collision_Exit);
        }
    }



    private void OnDestroy()
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnDestroy))
			DoPlayEventTrigger();
	}

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void ProcPlayEvent()
	{
		p_listEvent.Invoke();
        if (p_Event_IncludeThisObject != null)
            p_Event_IncludeThisObject(gameObject);

        if (p_bIsDebuging)
            Debug.Log(name + " " + this.GetType().Name + " Play Event", this);

        OnPlayEvent();
	}

	public void OnPointerDown( PointerEventData eventData )
	{
		OnPress( true );
	}

	public void OnPointerUp( PointerEventData eventData )
	{
		OnPress( false );
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

    protected void ProcDelayExcuteCallBack(System.Action OnAfterDelayAction, float fDelaySec)
    {
        if (this != null && gameObject.activeInHierarchy)
            StartCoroutine(CoDelayActionEventTrigger(OnAfterDelayAction, fDelaySec));
    }

    private IEnumerator CoDelayActionEventTrigger(System.Action OnAfterDelayAction, float fDelaySec)
    {
        yield return SCManagerYield.GetWaitForSecond(fDelaySec);
        yield return new CYield_IsWaitingEventTrigger();
        OnAfterDelayAction();
    }
}
