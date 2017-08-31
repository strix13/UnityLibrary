using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-24 오후 8:59:21
   Description : 
   Edit Log    : 
   ============================================ */

public class SCManagerEffect<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER> : SCManagerPoolingBase<SCManagerEffect<ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_EFFECT, CLASS_SOUNDPLAYER>, ENUM_EFFECT_NAME, CLASS_EFFECT>
    where ENUM_SOUND_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where ENUM_EFFECT_NAME : System.IFormattable, System.IConvertible, System.IComparable
    where CLASS_EFFECT : CEffectBase<CLASS_EFFECT, ENUM_EFFECT_NAME, ENUM_SOUND_NAME, CLASS_SOUNDPLAYER>
    where CLASS_SOUNDPLAYER : CSoundPlayerBase<ENUM_SOUND_NAME>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    //public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Transform pTransParents)
    //{
    //    CLASS_EFFECT pEffect = GetResource_Disable(eEffect);
    //    if (pEffect != null)
    //        pEffect.DoPlayEffect(pTransParents);

    //    return pEffect;
    //}

    public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Vector3 vecPos)
    {
        CLASS_EFFECT pEffect = GetResource_Disable(eEffect);
        if (pEffect != null)
            pEffect.DoPlayEffect(vecPos);

        return pEffect;
    }

	public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Transform pTransParents, Vector3 vecPos)
	{
		CLASS_EFFECT pEffect = GetResource_Disable(eEffect);
		if (pEffect != null)
			pEffect.DoPlayEffect(eEffect, pTransParents, vecPos);

		return pEffect;
	}

	public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Vector3 vecPos, Quaternion quatRot)
    {
        CLASS_EFFECT pEffect = GetResource_Disable(eEffect);
        if (pEffect != null)
        {
            pEffect.DoPlayEffect(vecPos);
            pEffect.p_pTransCached.rotation = quatRot;
        }

        return pEffect;
    }


    public CLASS_EFFECT DoPlayEffect(ENUM_EFFECT_NAME eEffect, Vector3 vecPos, Vector3 vecRot)
    {
        CLASS_EFFECT pEffect = GetResource_Disable(eEffect);
        if (pEffect != null)
        {
            pEffect.DoPlayEffect(vecPos);
            pEffect.p_pTransCached.rotation = Quaternion.LookRotation(vecRot);
        }

        return pEffect;
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */
       
    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */
    
    protected override void OnMakeResource(ENUM_EFFECT_NAME eResourceName, ref CLASS_EFFECT pMakeResource)
    {
        pMakeResource.p_eEffectName = eResourceName;

        pMakeResource.EventInitEffect(this);
    }

    protected override void OnGetResource_Disable(ref CLASS_EFFECT pFindResource)
    {
        pFindResource.p_bIsPooling = true;
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
