using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCCompo_WayPoint : CObjectBase, IDictionaryItem<EMissionEnemy>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public string strEnemyName;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<string, Transform> _mapWayPoint = new Dictionary<string, Transform>();
	private List<Transform> _listWayPoint = new List<Transform>();

	private int _iLastRandomIndex = -1;
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public Vector3 GetWayPointPos(string strWayPointName)
	{
		if (_mapWayPoint.ContainsKey(strWayPointName) == false)
		{
			Debug.Log( "웨이포인트" + name + " 에 " + strWayPointName + " 가 들어있지 않습니다.", this );
			return Vector3.zero;
		}
		else
			return _mapWayPoint[strWayPointName].position;
	}

	public Vector3 GetRandomPos()
	{
		if(_listWayPoint.Count <= 1)
			return Vector3.zero;

		int iRandomIndex = Random.Range( 0, _listWayPoint.Count );
		while(_iLastRandomIndex == iRandomIndex)
		{
			iRandomIndex = Random.Range( 0, _listWayPoint.Count );
		}
		_iLastRandomIndex = iRandomIndex;
		return _listWayPoint[iRandomIndex].position;
	}

	public void GetRandomPos(out Vector3 vecRandomPos, out string strWayPointName)
	{
		if (_listWayPoint.Count <= 1)
		{
			vecRandomPos = Vector3.zero;
			strWayPointName = "Fail";
			return;
		}

		int iRandomIndex = Random.Range( 0, _listWayPoint.Count );
		while (_iLastRandomIndex == iRandomIndex)
		{
			iRandomIndex = Random.Range( 0, _listWayPoint.Count );
		}

		_iLastRandomIndex = iRandomIndex;
		vecRandomPos = _listWayPoint[iRandomIndex].position;
		strWayPointName = _listWayPoint[iRandomIndex].name;
	}

	public string GetRandomWayPointName()
	{
		if (_listWayPoint.Count <= 1)
			return "Fail";

		int iRandomIndex = Random.Range( 0, _listWayPoint.Count );
		while (_iLastRandomIndex == iRandomIndex)
		{
			iRandomIndex = Random.Range( 0, _listWayPoint.Count );
		}

		_iLastRandomIndex = iRandomIndex;
		return _listWayPoint[iRandomIndex].name;
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

		GetComponentsInChildren( _listWayPoint );
		_listWayPoint.Remove( transform );

		if (_listWayPoint.Count <= 1)
			Debug.LogWarning( "WayPoint 카운트가 1이하입니다", this );

		for(int i = 0; i < _listWayPoint.Count; i++)
			_mapWayPoint.Add(_listWayPoint[i].name, _listWayPoint[i]);
	}

	public EMissionEnemy IDictionaryItem_GetKey()
	{
		return strEnemyName.ConvertEnum<EMissionEnemy>();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
