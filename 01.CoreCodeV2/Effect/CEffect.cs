using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-21 오후 7:19:05
   Description : 
   Edit Log    : 
   ============================================ */

public class CEffect : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */
	protected enum EEffectType
	{
		None,
		Particle,
		TwoD,
		Spine,
	}

    /* public - Variable declaration            */

    public delegate void OnEventEffect(string strEffect, CEffect pEffect, bool bEffectIsPlay);

    public event OnEventEffect p_Event_Effect_OnPlayStop;
    public event System.Action<CEffect> p_Event_Effect_OnDisable;

    [Header("이펙트 끝날때 이벤트")]
    public UnityEngine.Events.UnityEvent p_listEvent_FinishEffect = new UnityEngine.Events.UnityEvent();

    [Rename_Inspector("이펙트 이름", false)]
    public string _strEffectName;

    /* protected - Variable declaration         */

    protected EEffectType _eEffectType = EEffectType.None;

    /* private - Variable declaration           */

	private ParticleSystem _pParticleSystem;
#if NGUI
	private CUI2DSpriteAnimation _p2DSpriteAnimation;
#endif
	private System.Action _OnFinishEffect_OneShot;
    private bool _bIsStop = false;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoReturnEffect()
    {
        gameObject.SetActive(false);
    }

	public void DoPlayEffect( Transform pTransParents )
	{
        transform.SetParent( pTransParents );
		Vector3 vecLocalPos = Vector3.zero;
		OnPlayEffectBefore( _strEffectName, ref vecLocalPos );

        transform.localPosition = vecLocalPos;
        transform.localRotation = Quaternion.identity;

		ProcPlayEffect();

		OnPlayEffectAfter( _strEffectName );
	}

	public void DoPlayEffect( Vector3 V3Targetpos, System.Action OnFinishEffect = null )
	{
		OnPlayEffectBefore( _strEffectName, ref V3Targetpos );

		_OnFinishEffect_OneShot = OnFinishEffect;
        transform.position = V3Targetpos;
		ProcPlayEffect();

		OnPlayEffectAfter( _strEffectName );
	}
	
	public void DoPlayEffect( Transform pTransParents, Vector3 vecWorldPos )
	{
        transform.SetParent( pTransParents );
		OnPlayEffectBefore( _strEffectName, ref vecWorldPos );
        transform.position = vecWorldPos;
		ProcPlayEffect();

		OnPlayEffectAfter( _strEffectName );
	}

    public void DoStopEffect()
    {
        _bIsStop = true;
        gameObject.SetActive(false);
    }

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void DoResetEvent()
	{
		p_Event_Effect_OnPlayStop = null;
        p_Event_Effect_OnDisable = null;

    }

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	virtual public void OnMakeEffect() { }
	virtual protected void OnPlayEffectBefore( string strEffectName, ref Vector3 V3Targetpos ) { }
	virtual protected void OnPlayEffectAfter(string strEffectName ) { }

	virtual protected void OnDefineEffect()
	{
		_eEffectType = EEffectType.None;
#if NGUI
		if (_eEffectType == EEffectType.None && _p2DSpriteAnimation != null)
			_eEffectType = EEffectType.TwoD;
#endif
		if (_eEffectType == EEffectType.None && _pParticleSystem != null)
			_eEffectType = EEffectType.Particle;
	}

	virtual protected void ProcPlayEffect()
	{
        //Debug.Log(name + "Play Effect", this);
		gameObject.SetActive( true );
		switch (_eEffectType)
		{
			case EEffectType.Particle:
				StartCoroutine( CoPlayParticleSystem() );
				break;

			case EEffectType.TwoD:
				StartCoroutine( CoPlay2DSpriteAnimation() );
				break;
		}

		if (p_Event_Effect_OnPlayStop != null)
			p_Event_Effect_OnPlayStop( _strEffectName, this, true );
	}

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pParticleSystem = GetComponentInChildren<ParticleSystem>();
#if NGUI
		_p2DSpriteAnimation = GetComponentInChildren<CUI2DSpriteAnimation>();
#endif

		OnDefineEffect();
	}

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        _bIsStop = false;
    }

    protected override void OnDisableObject()
	{
		base.OnDisableObject();

        if (_bIsStop == false)
        {
            if (p_Event_Effect_OnDisable != null)
                p_Event_Effect_OnDisable(this);

            DoResetEvent();
        }
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private IEnumerator CoPlayParticleSystem()
	{
		if (_pParticleSystem.main.loop == false)
		{
			_pParticleSystem.Play();
			while (_pParticleSystem.isPlaying)
				yield return null;

			ProcOnFinishEffect();
		}
	}

	private IEnumerator CoPlay2DSpriteAnimation()
	{
#if NGUI
		if (_p2DSpriteAnimation.frames.Length == 0)
		{
			Debug.LogWarning( "Effect Sprite Frame Length가 0입니다" );
			ProcOnFinishEffect();
			yield break;
		}

		if (_p2DSpriteAnimation.loop)
			_p2DSpriteAnimation.Play();
		else
			_p2DSpriteAnimation.Play( ProcOnFinishEffect );
#endif
		yield return null;
	}
	
	private void ProcOnFinishEffect()
	{
		if (_OnFinishEffect_OneShot != null)
		{
			_OnFinishEffect_OneShot();
			_OnFinishEffect_OneShot = null;
		}

        p_listEvent_FinishEffect.Invoke();

        if (p_Event_Effect_OnPlayStop != null)
			p_Event_Effect_OnPlayStop( _strEffectName, this, false );

		gameObject.SetActive( false );
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
