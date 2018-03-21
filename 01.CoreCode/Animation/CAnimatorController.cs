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
 *	기능 : 컴포넌트 방식으로 구성요소로 사용하는 Animator Wrapping 클래스
 *	
 *	애니메이션 레이어 관련 기능 해야함
 *	
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EAnimationEvent
{
	None = 0,
	AnimationStart = 1,

	AttackStart = 2,
	AttackFinish = 3,

	AnimationFinish = 4,
}

public interface IAnimationController
{
	event System.Action<EAnimationEvent> p_Event_OnAnimationEvent;

	void DoPlayAnimation<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName, System.Action OnFinishAnimation = null )
		where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;

	void DoPlayAnimation_Loop<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName )
		where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;

	void DoStopAnimation();

	void DoResetAnimationEvent();

	bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName )
		where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;
}

public class CAnimatorController : CObjectBase, IAnimationController
{	
	public bool bPlayDefaultOnAwake = false;
	public string strDefaultAnimation;
	public bool bDefaultIsLoop = true;

	public event Action<EAnimationEvent> p_Event_OnAnimationEvent;

	private System.Action _OnFinishAnimation;
	private Animator _pAnimator;	public Animator p_pAnimator {  get { return _pAnimator; } }

	private float _fCurrentAnimation_NomalizeTime; public float p_fCurrentAnimation_NomalizeTime { get { return _fCurrentAnimation_NomalizeTime; } }
	private string _strCurrentAnimName;
	private bool _bIsLoop;

	// ========================== [ Division ] ========================== //

	public void DoStopAnimation()
	{
		if (_pAnimator.isActiveAndEnabled == false) return;

		for (int i = 0; i < _pAnimator.layerCount; i++)
			_pAnimator.Play( 0, i, 0f );

		_pAnimator.gameObject.SetActive( false );
		_pAnimator.gameObject.SetActive( true );
		StopAllCoroutines();
	}

	public void DoSetLayerWeight(int iLayerIndex, float fWeight)
	{
		_pAnimator.SetLayerWeight( iLayerIndex, fWeight );
	}

	public void DoResetAnimationEvent()
	{
		p_Event_OnAnimationEvent = null;
	}

	/// <summary>
	/// 사용하기 전에 반드시 실행 // Animator를 Getcomponent 합니다 (없을경우 자식 에서 찾음)
	/// </summary>
	public void DoInitAnimator()
	{
		_pAnimator = GetComponent<Animator>();
		if (_pAnimator == null)
			_pAnimator = GetComponentInChildren<Animator>();
	}

	/// <summary>
	/// 애니메이션이 실행 중인지 체크합니다.
	/// </summary>
	/// <param name="eAnimName">체크할 애니메이션 이름의 Enum</param>
	/// <returns></returns>
	public bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName )
		where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
	{
		bool bFindAnimation = false;
		for (int i = 0; i < _pAnimator.layerCount; i++)
		{
			AnimatorStateInfo sCurrentState = _pAnimator.GetCurrentAnimatorStateInfo( i );
			if (sCurrentState.IsName( eAnimName.ToString() ))
			{
				bFindAnimation = true;
				break;
			}
		}
		return bFindAnimation;
	}

	/// <summary>
	/// 애니메이션이 실행 중인지 체크합니다.
	/// </summary>
	/// <param name="eAnimName">체크할 애니메이션 이름의 Enum</param>
	/// <returns></returns>
	public bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName, int iAnimationLayer )
		where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
	{
		AnimatorStateInfo sCurrentState = _pAnimator.GetCurrentAnimatorStateInfo( iAnimationLayer );
		return sCurrentState.IsName( eAnimName.ToString() );
	}

	/// <summary>
	/// 애니메이션을 실행합니다.
	/// </summary>
	/// <param name="eAnimName">플레이 할 애니메이션 이름의 Enum</param>
	/// <param name="OnFinishAnimation">애니메이션이 종료될 때 호출할 함수</param>
	public void DoPlayAnimation<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName, System.Action OnFinishAnimation = null )
		where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
	{
		_OnFinishAnimation = OnFinishAnimation;
		ProcPlayAnim( eAnimName, false );
	}

	public void DoPlayAnimation_Default()
	{
		if (bDefaultIsLoop)
			DoPlayAnimation_Loop( strDefaultAnimation );
		else
			DoPlayAnimation( strDefaultAnimation );
	}

	/// <summary>
	/// 애니메이션을 반복으로 실행합니다.
	/// </summary>
	/// <param name="eAnimName">반복 실행할 애니메이션 이름의 Enum</param>
	public void DoPlayAnimation_Loop<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName )
		where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
	{
		ProcPlayAnim( eAnimName, true );
	}

	/// <summary>
	/// 애니메이션 Parameter를 직접 세팅합니다.
	/// </summary>
	/// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
	public void DoSetParam_trigger<ENUM_ANIMATION_PARAMETER>( ENUM_ANIMATION_PARAMETER eParam )
		 where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
	{
		_pAnimator.SetTrigger( eParam.ToString() );
	}

	public void DoResetParam_Trigger<ENUM_ANIMATION_PARAMETER>( ENUM_ANIMATION_PARAMETER eParam )
		 where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
	{
		_pAnimator.ResetTrigger( eParam.ToString() );
	}

	/// <summary>
	/// 애니메이션 Parameter를 직접 세팅합니다.
	/// </summary>
	/// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
	public void DoSetParam_float<ENUM_ANIMATION_PARAMETER>( ENUM_ANIMATION_PARAMETER eParam, float fParameter )
		where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
	{
		_pAnimator.SetFloat( eParam.ToString(), fParameter );
	}

	/// <summary>
	/// 애니메이션 Parameter를 직접 세팅합니다.
	/// </summary>
	/// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
	public void DoSetParam_Int<ENUM_ANIMATION_PARAMETER>( ENUM_ANIMATION_PARAMETER eParam, int iParameter )
		where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
	{
		_pAnimator.SetInteger( eParam.ToString(), iParameter );
	}

	/// <summary>
	/// 애니메이션 Parameter를 직접 세팅합니다.
	/// </summary>
	/// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
	public void DoSetParam_bool<ENUM_ANIMATION_PARAMETER>( ENUM_ANIMATION_PARAMETER eParam, bool bParam )
		where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
	{
		_pAnimator.SetBool( eParam.ToString(), bParam );
	}

	public void DoSetAnimationSpeed( float fSpeed )
	{
		_pAnimator.speed = fSpeed;
	}


	public void EventAnimationListen( EAnimationEvent eAnimationEvent )
	{
		if (p_Event_OnAnimationEvent != null)
			p_Event_OnAnimationEvent( eAnimationEvent );
	}

	// ========================== [ Division ] ========================== //

	protected override void OnAwake()
	{
		base.OnAwake();

		DoInitAnimator();

		if(bPlayDefaultOnAwake)
			DoPlayAnimation_Default();
	}

	// ========================== [ Division ] ========================== //

	private void ProcPlayAnim<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName, bool bIsLoop, int iAnimationLayer = 0 )
	{
		if (gameObject.activeInHierarchy == false)
		{
			Debug.LogWarning( name + " ProcPlayAnim - gameObject.activeInHierarchy == false", this );
			return;
		}
		if (_pAnimator.isInitialized == false) // Animator does not have an AnimatorController 관련 에러 잡기 위함
		{
			Debug.LogWarning( "Before : " + name + _pAnimator.isInitialized );
			StartCoroutine( CoDelayPlayAnimation( eAnimName.ToString(), bIsLoop, iAnimationLayer ) );
			return;
		}

		// for debug
		//if(GetComponentInParent<SGEnemy>() != null)
		//{
		//	if(GetComponentInParent<CCompoStat>().p_bIsAlive == false)
		//	{
		//		Debug.Log( "Play Anim!!" + eAnimName );
		//	}
		//}
		
		_strCurrentAnimName = eAnimName.ToString();
		_pAnimator.Play( _strCurrentAnimName, iAnimationLayer, 0f );

		_pAnimator.Update(0);
		_bIsLoop = bIsLoop;
		_fCurrentAnimation_NomalizeTime = 0f;

		StopCoroutine( "CoUpdateAnimation");
		StartCoroutine( "CoUpdateAnimation" );
	}

	private IEnumerator CoDelayPlayAnimation( string strAnimationName, bool bIsLoop, int iAnimationLayer )
	{
		_pAnimator.Rebind();
		yield return null;
		ProcPlayAnim( strAnimationName, bIsLoop, iAnimationLayer );
		yield break;
	}

	private IEnumerator CoUpdateAnimation()
	{
		//// 바로 시작하면 Animator가 갱신이 안되서 플레이중인 애니메이션을 못찾는다..
		//yield return new WaitForSeconds(0.1f);
		while (true)
		{
			yield return null;

			bool bFindAnimation = false;
			AnimatorStateInfo sCurrentState = _pAnimator.GetCurrentAnimatorStateInfo( 0 );
			for (int i = 0; i < _pAnimator.layerCount; i++)
			{
				AnimatorStateInfo sState = _pAnimator.GetCurrentAnimatorStateInfo( i );
				if (sState.IsName( _strCurrentAnimName ))
				{
					sCurrentState = sState;
					bFindAnimation = true;
					break;
				}
			}

			// 에러인 경우
			if (bFindAnimation == false)
			{
				Debug.LogWarning( "Error!!" );
			}

			_fCurrentAnimation_NomalizeTime = sCurrentState.normalizedTime;
			// 플레이가 끝났을 때
			if (_fCurrentAnimation_NomalizeTime >= 0.99f || sCurrentState.IsName( _strCurrentAnimName ) == false)
			{
				if (_bIsLoop)
					DoPlayAnimation_Loop( _strCurrentAnimName );
				else if (_OnFinishAnimation != null)
				{
					// OnFinishAnimation에 DoPlayAnimation(ENUM_ANIM_NAME eAnimName, System.Action OnFinishAnimation)을 실행하면, 
					// _OnFinishAnimation을 세팅한다. 근데 그 함수가 끝나고 바로 null처리를 하기 때문에,
					// 결과적으로 세팅을 해도 _OnFinishAnimation을은 null이 된다. 따라서, 임시 객체에 일단 저장 후 미리 null처리 후 호출한다.
					System.Action OnFinishAnimation = _OnFinishAnimation;
					_OnFinishAnimation = null;
					yield return null;

					OnFinishAnimation();
				}

				break;
			}
		}


		yield break;
	}
}
