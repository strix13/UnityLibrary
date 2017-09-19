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
	private Skeleton _pSkeleton;

	private System.Action _OnFinishAnimation;
	private int _iPriorityCurrent = -1;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetAlphaRenderer( float fAlpha)
	{
		if (_pSkeleton == null)
		{
			if(_pAnimation == null)
				_pAnimation = GetComponentInChildren<SkeletonAnimation>();

			_pSkeleton = _pAnimation.skeleton;
		}

		if(_pSkeleton != null)
			_pSkeleton.a = fAlpha;
	}

	public bool DoPlayAnimation<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName )
	{
		string streAnimName = eAnimName.ToString();
		bool bSuccess = _pSkeletonData.FindAnimation( streAnimName ) != null;
		if (bSuccess)
		{
			//Debug.Log( "Before : " + _pAnimation.AnimationName );
			_iPriorityCurrent = -1;
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
			_iPriorityCurrent = -1;
			_pAnimation.AnimationName = streAnimName;
			_pAnimation.state.Complete += State_End;
			_OnFinishAnimation = OnFinishAnimation;
			//Debug.Log( "After : " + _pAnimation.AnimationName );
		}

		return bSuccess;
	}

	public bool DoPlayAnimation<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName, bool bIsLoop )
	{
		string streAnimName = eAnimName.ToString();

		if(_pSkeletonData == null)
		{
			Strix.Debug.Log_ForCore(Strix.EDebugLevel.Warning_Core, name + " _pSkeletonData == null", this );
			return false;
		}

		bool bSuccess = _pSkeletonData.FindAnimation( streAnimName ) != null;
		if (bSuccess)
		{
			// 애니메이션이 같으면 루프 설정을 무시하기때문에 일부러 틀린 애니메이션 삽입
			if(_pAnimation.AnimationName == streAnimName && _pAnimation.loop != bIsLoop)
				_pAnimation.AnimationName = "";

			_pAnimation.loop = bIsLoop;
			_pAnimation.AnimationName = streAnimName;
			_iPriorityCurrent = -1;
		}

		return bSuccess;
	}

	public bool DoPlayAnimation<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName, int iPriority, bool bIsLoop )
	{
		string streAnimName = eAnimName.ToString();

		if (_iPriorityCurrent >= iPriority) return false;

		bool bSuccess = DoPlayAnimation( eAnimName, bIsLoop );
		if (bSuccess)
		{
			if(bIsLoop == false)
				_pAnimation.state.Complete += State_End;
			_iPriorityCurrent = iPriority;
		}

		return bSuccess;
	}

	public void DoSetOrderInLayer(int iOrder)
	{
		MeshRenderer pRenderer = _pAnimation.GetComponent<MeshRenderer>();
		pRenderer.sortingOrder = iOrder;
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

		_pAnimation = GetComponentInChildren<SkeletonAnimation>();
		if (_pAnimation.SkeletonDataAsset == null)
		{
			Strix.Debug.Log_ForCore( Strix.EDebugLevel.Error_Core, name + "스켈레톤 데이터 에셋이 없다", this );
			return;
		}

		_pSkeletonData = _pAnimation.SkeletonDataAsset.GetSkeletonData(false);
		_pSkeleton = _pAnimation.skeleton;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void State_End( TrackEntry trackEntry )
	{
		if (_OnFinishAnimation != null)
		{
			StartCoroutine( CoDealyCallBack(_OnFinishAnimation ));
			_OnFinishAnimation = null;
		}

		_iPriorityCurrent = -1;
		_pAnimation.state.Complete -= State_End;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private IEnumerator CoDealyCallBack( System.Action OnFinishAnimation )
	{
		yield return null;

		Debug.Break();

		OnFinishAnimation();
	}

}
#endif