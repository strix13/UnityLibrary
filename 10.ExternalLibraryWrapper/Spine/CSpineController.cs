#if Spine

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Spine.Unity;
using Spine;
using System;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CSpineController : CObjectBase, IAnimationController
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public event Action<string> p_Event_OnAnimationEvent;

    public string p_strCurrentAnimationName { get { return _pAnimation.AnimationName; } }

    [Rename_Inspector("애니메이션 이벤트 이름 출력")]
    public bool p_bPrintAnimationEvent = false;

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    private SkeletonAnimation _pAnimation;
	private SkeletonData _pSkeletonData;
	private Skeleton _pSkeleton;

	private System.Action _OnFinishAnimation;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/// <summary>
	/// 현재 플레이하는 애니메이션 이름과 재생하고자 할 애니메이션 이름이 같을 경우 안될 수 있음
	/// </summary>
	/// <typeparam name="ENUM_ANIM_NAME"></typeparam>
	/// <param name="eAnimName"></param>
	/// <returns></returns>
	public bool DoPlayAnimation<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName )
	{
		string strAnimName = eAnimName.ToString();
		bool bSuccess = _pSkeletonData.FindAnimation( strAnimName ) != null;
		if (bSuccess)
		{
            _pAnimation.loop = false;
            _pAnimation.AnimationName = "";
			_pAnimation.AnimationName = strAnimName;
            _pAnimation.state.Event -= State_Event;
            _pAnimation.state.Event += State_Event;
        }

		return bSuccess;
	}

	public void DoPlayAnimation<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName, System.Action OnFinishAnimation )
			where ENUM_ANIM_NAME : System.IConvertible, System.IComparable
	{
		bool bSuccess = DoPlayAnimation( eAnimName );
		if(bSuccess)
		{
			_pAnimation.state.Complete += State_End;
			_OnFinishAnimation = OnFinishAnimation;
		}
	}
	
	public void DoPlayAnimation_Loop<ENUM_ANIM_NAME>( ENUM_ANIM_NAME eAnimName )
		where ENUM_ANIM_NAME : System.IConvertible, System.IComparable
	{
		string strAnimName = eAnimName.ToString();

		if(_pSkeletonData == null)
		{
			Debug.LogWarning( name + " _pSkeletonData == null", this );
			return;
		}
		
		// 애니메이션이 같으면 루프 설정을 무시하기때문에 일부러 틀린 애니메이션 삽입
		if(_pAnimation.AnimationName == strAnimName && _pAnimation.loop )
			_pAnimation.AnimationName = "";

		_pAnimation.loop = true;
		_pAnimation.AnimationName = strAnimName;
        _pAnimation.state.Event -= State_Event;
        _pAnimation.state.Event += State_Event;
    }

    public bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>( ENUM_ANIMATION_NAME eAnimName )
		where ENUM_ANIMATION_NAME : IConvertible, IComparable
	{
		return _pAnimation.AnimationName == eAnimName.ToString();
	}


	public void DoSetOrderInLayer(int iOrder)
	{
		MeshRenderer pRenderer = _pAnimation.GetComponent<MeshRenderer>();
		pRenderer.sortingOrder = iOrder;
	}

	public void DoResetAnimationEvent()
	{
		p_Event_OnAnimationEvent = null;
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
			Debug.LogWarning( name + "스켈레톤 데이터 에셋이 없다", this );
			return;
		}

		_pSkeletonData = _pAnimation.SkeletonDataAsset.GetSkeletonData(false);
		_pSkeleton = _pAnimation.skeleton;
	}

    private void State_Event( TrackEntry trackEntry, Spine.Event e )
	{
		string strKeyName = e.Data.Name;
		if (p_Event_OnAnimationEvent != null)
			p_Event_OnAnimationEvent(strKeyName);

        if (p_bPrintAnimationEvent)
            Debug.Log(name + " Animation Event Name : " + strKeyName, this);
    }

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void State_End( TrackEntry trackEntry )
	{
		if (_OnFinishAnimation != null)
		{
            var pEntry = _pAnimation.state.GetCurrent(0);
            if(pEntry != null)
                pEntry.TimeScale = 0f;

			StartCoroutine( CoDealyCallBack(_OnFinishAnimation ));
			_OnFinishAnimation = null;
		}
		
		_pAnimation.state.Complete -= State_End;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private IEnumerator CoDealyCallBack( System.Action OnFinishAnimation )
	{
		yield return null;

		OnFinishAnimation();
	}

	public void DoStopAnimation()
	{
		_pAnimation.AnimationName = "";
	}
}
#endif