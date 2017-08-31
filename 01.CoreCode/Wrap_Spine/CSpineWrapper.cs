#define Spine

#if Spine

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Spine.Unity;
using Spine;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CSpineWrapper : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private SkeletonAnimation _pAnimation;
	private SkeletonData _pSkeletonData;

	private System.Action _OnFinishAnimation;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public bool DoPlayAnimation<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName )
	{
		string streAnimName = eAnimName.ToString();
		bool bSuccess = _pSkeletonData.FindAnimation( streAnimName ) != null;
		if (bSuccess)
		{
			//Debug.Log( "Before : " + _pAnimation.AnimationName );
			_pAnimation.AnimationName = streAnimName;
			//Debug.Log( "After : " + _pAnimation.AnimationName );
		}

		return bSuccess;
	}

	public bool DoPlayAnimation<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName, System.Action OnFinishAnimation )
	{
		if (_pSkeletonData == null) return false;

		string streAnimName = eAnimName.ToString();
		bool bSuccess = _pSkeletonData.FindAnimation( streAnimName ) != null;
		if (bSuccess)
		{
			//Debug.Log( "Before : " + _pAnimation.AnimationName );
			_pAnimation.AnimationName = streAnimName;
			_pAnimation.state.Complete += State_End;
			_OnFinishAnimation = OnFinishAnimation;
			//Debug.Log( "After : " + _pAnimation.AnimationName );
		}

		return bSuccess;
	}

	private void State_End( TrackEntry trackEntry )
	{
		_pAnimation.state.Complete -= State_End;
		if(_OnFinishAnimation != null)
		{
			System.Action OnTemp = _OnFinishAnimation;
			_OnFinishAnimation = null;
			OnTemp();
		}
	}

	public bool DoPlayAnimation<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName, bool bIsLoop )
	{
		string streAnimName = eAnimName.ToString();

		if(_pSkeletonData == null)
		{
			Debug.Log( name + " _pSkeletonData == null", this );
			return false;
		}

		bool bSuccess = _pSkeletonData.FindAnimation( streAnimName ) != null;
		if (bSuccess)
		{
			_pAnimation.loop = bIsLoop;
			_pAnimation.AnimationName = streAnimName;
		}

		return bSuccess;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pAnimation = GetComponent<SkeletonAnimation>();
		if (_pAnimation == null)
			_pAnimation = GetComponentInChildren<SkeletonAnimation>();

		_pSkeletonData = _pAnimation.SkeletonDataAsset.GetSkeletonData(false);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */
	   
	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
#endif