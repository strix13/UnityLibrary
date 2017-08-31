using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Edit Log    : 
   ============================================ */

public class PCRoulette : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EComponentName
	{
		RouletteBoddy,
		Reel_1,
		Reel_2,
		Reel_3,
		Pin,
		Background,

		MAX
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CSpineWrapper[] _arrAnimator = new CSpineWrapper[(int)EComponentName.MAX];

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoPlayAnimation<Enum_AnimationName>( EComponentName eComponentName, Enum_AnimationName eAnimationName )
	{
		_arrAnimator[(int)eComponentName].DoPlayAnimation( eAnimationName, false );
	}

	public void DoPlayAnimation_Loop<Enum_AnimationName>( EComponentName eComponentName, Enum_AnimationName eAnimationName )
	{
		_arrAnimator[(int)eComponentName].DoPlayAnimation( eAnimationName, true );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();
		
		GetComponent(out _arrAnimator[(int)EComponentName.RouletteBoddy] );

		for (int i = 1; i < (int)EComponentName.MAX; i++)
			_arrAnimator[i] = GetGameObject( (EComponentName)i ).GetComponent<CSpineWrapper>();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
