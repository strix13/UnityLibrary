using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-24 오후 8:59:21
   Description : 
   Edit Log    : 
   ============================================ */

public class SCManagerEffect<ENUM_EFFECT_NAME, CLASS_EFFECT> : CManagerPooling<ENUM_EFFECT_NAME, CLASS_EFFECT>
    where ENUM_EFFECT_NAME : System.IConvertible, System.IComparable
    where CLASS_EFFECT : CEffectBase<CLASS_EFFECT, ENUM_EFFECT_NAME>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    static public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Vector3 vecPos)
    {
        CLASS_EFFECT pEffect = instance.DoPop(eEffect);
        pEffect.DoPlayEffect(vecPos);

		//Debug.Log( eEffect, pEffect);

		return pEffect;
    }

	static public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Transform pTransParents, Vector3 vecPos)
	{
		CLASS_EFFECT pEffect = instance.DoPop( eEffect);
		pEffect.DoPlayEffect(eEffect, pTransParents, vecPos);

		return pEffect;
	}

	static public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Vector3 vecPos, Quaternion quatRot)
    {
        CLASS_EFFECT pEffect = instance.DoPop( eEffect);
        pEffect.DoPlayEffect(vecPos);
        pEffect.p_pTransCached.rotation = quatRot;

        return pEffect;
    }


	static public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Vector3 vecPos, Vector3 vecRot)
    {
        CLASS_EFFECT pEffect = instance.DoPop( eEffect);
        pEffect.DoPlayEffect(vecPos);
        pEffect.p_pTransCached.rotation = Quaternion.LookRotation(vecRot);

        return pEffect;
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */
       
    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */
    
    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
