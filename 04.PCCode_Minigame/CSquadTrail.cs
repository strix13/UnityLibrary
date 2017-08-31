using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CSquadTrail : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public int iPositionCapacity = 100;
	public List<Transform> _listSquadObject = null;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private List<Vector3> _listOldPos = new List<Vector3>( );

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
	
	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		StartCoroutine( CoUpdateSquad() );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoUpdateSquad()
	{
		while(true)
		{
			Vector3 vecPos = _pTransformCached.position;
			if (_listOldPos.Count == 0 ||
			   (_listOldPos.Count != 0 && _listOldPos[_listOldPos.Count - 1] != vecPos))
			{
				if (_listOldPos.Count >= iPositionCapacity)
					_listOldPos.RemoveAt( 0 );

				_listOldPos.Add( vecPos );
				for (int i = 0; i < _listSquadObject.Count; i++)
				{
					int iIndex = _listOldPos.Count - Mathf.RoundToInt(((float)_listOldPos.Count / (i + 2)));
					if (iIndex > _listOldPos.Count - 1)
						iIndex = _listOldPos.Count - 1;

					_listSquadObject[i].position = _listOldPos[iIndex];
				}
			}

			yield return null;
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
