#region Header
/* ============================================ 
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UGUIInventory_Test : CUGUIInventoryBase<UGUIInventory_Test.SInfoInventoryTest>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public class SInfoInventoryTest : CInventoryDataBase
	{
		public SInfoInventoryTest(int iSlotID, string strImageName, IInventoryListener IListener)
		{
			DoInit(iSlotID, strImageName, IListener);
		}
	}

	#region Field
	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */


	#endregion Field

	#region Public
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected
	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		List<SInfoInventoryTest> listInventoryData = new List<SInfoInventoryTest>();
		listInventoryData.Add(new SInfoInventoryTest(1, "Uranos", this));
		listInventoryData.Add(new SInfoInventoryTest(3, "Airplane_1", this));

		DoInitInventory(listInventoryData);
	}

	#endregion Protected

	// ========================================================================== //

	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
