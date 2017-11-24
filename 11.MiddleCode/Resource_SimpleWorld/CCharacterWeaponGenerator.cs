using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : parkjonghwa                             
   Date        : 2017-05-16 오후 12:48:23
   Description : 
   Edit Log    : 
   ============================================ */
[ExecuteInEditMode]
public class CCharacterWeaponGenerator : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */
    private string[] _strPartsName ={ "Hand_Right_jnt", "Hand_Left_jnt" };

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

	protected override void OnEnableObject()
    {
  //      base.OnEnableObject();

		//if (GetComponent<CharacterController>() == null)
		//{
		//	CharacterController pCharacterController = gameObject.AddComponent<CharacterController>();
		//	pCharacterController.radius = 0.5f;
		//	pCharacterController.height = 3f;
		//	pCharacterController.center = new Vector3(0f, 1.4f, 0f);
		//}

		//if (GetComponent<CCharacterModelController>() == null)
		//{
		//	gameObject.AddComponent<CCharacterModelController>();
		//}

		//GeneratorParts(transform);
		////DestroyImmediate(this);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

	public void DoGeneratorParts(Transform pObject)
    {
        Transform[] pChild = pObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < pChild.Length; i++)
        {
			for (int j = 0; j < _strPartsName.Length; j++)
			{
				if (pChild[i].GetComponentInChildren<CCompoEquipmentHand>() != null)
					continue;

				if (pChild[i].name == "Hand_Right_jnt")
				{
					GameObject pObjectWeaponHand = new GameObject();
					pObjectWeaponHand.transform.parent = pChild[i].transform;
					pObjectWeaponHand.name = "WeaponHand";
					pObjectWeaponHand.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
					pObjectWeaponHand.transform.localPosition = Vector3.zero;
					CCompoEquipmentHand pWeaponHand = pObjectWeaponHand.AddComponent<CCompoEquipmentHand>();

					gameObject.GetComponent<CCharacterModelController>()._pEquipHand_Right = pWeaponHand;
				}
				else if(pChild[i].name == "Hand_Left_jnt")
				{
					while (pChild[i].childCount != 0)
						DestroyImmediate(pChild[i].GetChild(i).gameObject);

					GameObject pObjectShieldHand = new GameObject();
					pObjectShieldHand.transform.parent = pChild[i].transform;
					pObjectShieldHand.name = "ShieldHand";
					pObjectShieldHand.transform.localRotation = Quaternion.Euler(90f, -129f, 0f);
					pObjectShieldHand.transform.localPosition = new Vector3(0f, -0.2f, 0f);
					CCompoEquipmentHand pShieldHand = pObjectShieldHand.AddComponent<CCompoEquipmentHand>();

					gameObject.GetComponent<CCharacterModelController>()._pEquipHand_Left = pShieldHand;
				}
			}
		}
    }

}
