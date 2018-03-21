using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-21 오후 7:19:05
   Description : 
   Edit Log    : 
   ============================================ */

abstract public class CEffectBase<CLASS_EFFECT, ENUM_EFFECT_NAME> : CObjectBase
	where CLASS_EFFECT : CEffectBase<CLASS_EFFECT, ENUM_EFFECT_NAME>
	where ENUM_EFFECT_NAME : System.IConvertible, System.IComparable
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

	public delegate void OnEventEffect( ENUM_EFFECT_NAME eEffect, CLASS_EFFECT pEffect, bool bEffectIsPlay);

	public event OnEventEffect p_Event_Effect_OnPlayStop;

	/* protected - Variable declaration         */

	protected EEffectType _eEffectType = EEffectType.None;

	/* private - Variable declaration           */
	[SerializeField]
	private ENUM_EFFECT_NAME _eEffectName; public ENUM_EFFECT_NAME p_eEffectName { get { return _eEffectName; } set { _eEffectName = value; } }

	private CLASS_EFFECT _pInstance;
	private ParticleSystem _pParticleSystem;
#if NGUI
	private CUI2DSpriteAnimation _p2DSpriteAnimation;
#endif
	private System.Action _OnFinishEffect;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoPlayEffect( Transform pTransParents )
	{
        transform.SetParent( pTransParents );
		Vector3 vecLocalPos = Vector3.zero;
		OnPlayEffectBefore( _eEffectName, ref vecLocalPos );

        transform.localPosition = vecLocalPos;
        transform.localRotation = Quaternion.identity;

		ProcPlayEffect();

		OnPlayEffectAfter( _eEffectName );
	}

	public void DoPlayEffect( Vector3 V3Targetpos, System.Action OnFinishEffect = null )
	{
		OnPlayEffectBefore( _eEffectName, ref V3Targetpos );

		_OnFinishEffect = OnFinishEffect;
        transform.position = V3Targetpos;
		ProcPlayEffect();

		OnPlayEffectAfter( _eEffectName );
	}
	
	public void DoPlayEffect( Transform pTransParents, Vector3 vecWorldPos )
	{
        transform.SetParent( pTransParents );
		OnPlayEffectBefore( _eEffectName, ref vecWorldPos );
        transform.position = vecWorldPos;
		ProcPlayEffect();

		OnPlayEffectAfter( _eEffectName );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void DoResetEvent()
	{
		p_Event_Effect_OnPlayStop = null;
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	virtual public void OnMakeEffect() { }
	virtual protected void OnPlayEffectBefore( ENUM_EFFECT_NAME eEffectName, ref Vector3 V3Targetpos ) { }
	virtual protected void OnPlayEffectAfter( ENUM_EFFECT_NAME eEffectName ) { }

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
			p_Event_Effect_OnPlayStop( _eEffectName, _pInstance, true );
	}

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pInstance = this as CLASS_EFFECT;
		_pParticleSystem = GetComponentInChildren<ParticleSystem>();
#if NGUI
		_p2DSpriteAnimation = GetComponentInChildren<CUI2DSpriteAnimation>();
#endif

		OnDefineEffect();
	}

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		DoResetEvent();
		CManagerPooling<ENUM_EFFECT_NAME, CLASS_EFFECT>.instance.DoPush( _pInstance );
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
		if (_OnFinishEffect != null)
		{
			_OnFinishEffect();
			_OnFinishEffect = null;
		}

		if (p_Event_Effect_OnPlayStop != null)
			p_Event_Effect_OnPlayStop( _eEffectName, _pInstance, false );

		gameObject.SetActive( false );
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
