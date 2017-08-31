using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCManagerInMiniTower : PCManagerGameBase<PCManagerInMiniTower>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private PCMiniTower_Airplane _pAirplane;
	private CManagerPooling<EMinigameTile, PCMiniTower_Tile> _pManagerPool_Tile;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	public void EventOnSupplyTile()
	{
		PCMiniTower_Tile pResourceTile = _pManagerPool_Tile.DoPop(EMinigameTile.Default_Tile);
		_pAirplane.DoSupplyTile(pResourceTile);
	}

	public void EventOnTouchDropTile()
	{
		_pAirplane.DoDropTile();
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventOnPopResource_Tile(EMinigameTile eTile, PCMiniTower_Tile pTile)
	{
		pTile.p_pTransCached.localScale = Vector2.one * 70f;
		pTile.p_pTransCached.localPosition = _pAirplane.p_pTransCached.localPosition;
		pTile.p_pTransCached.parent = _pAirplane.p_pTransCached;
	}

	/* protected - Override & Unity API         */

	protected override void OnGameStart( int iDifficultyLevel, bool bIsTest )
	{
		base.OnGameStart( iDifficultyLevel, bIsTest );

		EventOnSupplyTile();
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		_pAirplane = GetComponentInChildren<PCMiniTower_Airplane>(true);

		_pManagerPool_Tile = CManagerPooling<EMinigameTile, PCMiniTower_Tile>.instance;
		_pManagerPool_Tile.p_pObjectManager.transform.SetParent(p_pTransCached);
		_pManagerPool_Tile.p_pObjectManager.transform.DoResetTransform();
		_pManagerPool_Tile.p_EVENT_OnPopResource += EventOnPopResource_Tile;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
