using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public enum EObstacleType
{
	Ground,
	Wall_Top,
	Wall_Side,
	Wall_Bottom,
}

public class PCGameObstacle : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public EObstacleType p_eObstacleType {  get { return _eObstacleType; } }

	[SerializeField]
	private EObstacleType _eObstacleType = EObstacleType.Ground;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
