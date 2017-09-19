using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */
[ExecuteInEditMode]
public class CCharacterModel_RideGenerator : CObjectBase
{
	/* const & readonly declaration             */

	const string const_strSpineTransformName = "Horse_Rig_Spine_01SHJnt";

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public Transform _pTransCharacter;

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

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (_pTransCharacter == null) return;

		Transform[] pChild = GetComponentsInChildren<Transform>();
		for (int i = 0; i < pChild.Length; i++)
		{
			if (pChild[i].name == const_strSpineTransformName)
			{
				_pTransCharacter.SetParent(pChild[i]);
				//_pTransCharacter.DoResetTransform();
				break;
			}
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
