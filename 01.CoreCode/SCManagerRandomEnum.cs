#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCManagerRandomEnum<Enum_Group, Enum_Item> : CSingletonBase_Not_UnityComponent<SCManagerRandomEnum<Enum_Group, Enum_Item>>
	where Enum_Group : System.IComparable, System.IConvertible
	where Enum_Item : System.IComparable, System.IConvertible
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	static public CDictionary_ForEnumKey<Enum_Group, List<Enum_Item>> _mapTable = new CDictionary_ForEnumKey<Enum_Group, List<Enum_Item>>();

	/* protected - Field declaration         */

	/* private - Field declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	static public void DoAddGroup( Enum_Group eGroup, Enum_Item eItem )
	{
		if (_mapTable.ContainsKey( eGroup ) == false)
			_mapTable.Add( eGroup, new List<Enum_Item>() );

		_mapTable[eGroup].Add( eItem );
	}

	static public Enum_Item GetRandomEnum( Enum_Group eGroup )
	{
		if (_mapTable.ContainsKey_PrintOnError( eGroup.GetHashCode() ) == false)
			return default(Enum_Item);

		List<Enum_Item> listItem = _mapTable[eGroup];
		if (listItem.Count == 0)
			return default( Enum_Item );

		int iRandomIndex = Random.Range( 0, listItem.Count );
		return listItem[iRandomIndex];
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
