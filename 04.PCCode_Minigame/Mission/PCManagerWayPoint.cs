using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public enum EWayPointName
{
	WayPoint_Rex,
	WayPoint_Jeff,
}

public class PCManagerWayPoint : CSingletonBase<PCManagerWayPoint>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<EMissionEnemy, PCCompo_WayPoint> _mapWayPoint = new Dictionary<EMissionEnemy, PCCompo_WayPoint>();

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public PCCompo_WayPoint GetWayPoint( EMissionEnemy eEnemyName)
	{
		if(_mapWayPoint.ContainsKey(eEnemyName) == false)
		{
			Debug.Log( eEnemyName + "가 WayPoint에 없습니다" );
			return null;
		}

		return _mapWayPoint[eEnemyName];
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

		PCCompo_WayPoint[] arrWayPoint = GetComponentsInChildren<PCCompo_WayPoint>();
		_mapWayPoint.DoAddItem( arrWayPoint );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
