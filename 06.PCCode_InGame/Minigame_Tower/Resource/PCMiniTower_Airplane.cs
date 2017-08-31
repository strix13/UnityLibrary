using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCMiniTower_Airplane : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCMiniTower_Tile _pSupplyTile;
	private TweenPosition _pTweenPosition;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoSupplyTile(PCMiniTower_Tile pResourceTile)
	{
		_pSupplyTile = pResourceTile;
		_pSupplyTile.DoInit();

		_pTweenPosition.enabled = true;

		print("타일 보급됨");
	}

	public void DoDropTile()
	{
		if (_pSupplyTile == null)
		{
			Debug.LogWarning("장착된 타일이 없습니다.");
			return;
		}

		_pSupplyTile.DoDrop();
		_pSupplyTile = null;

		print("타일 드랍됨");
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

		_pTweenPosition = GetComponent<TweenPosition>();
		_pTweenPosition.enabled = false;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
