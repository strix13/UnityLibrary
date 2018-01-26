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

public class CManagerStat : CSingletonBase<CManagerStat>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EDamageCalculateOption
	{
		PlusMinus,
		Percentage,
	}

	/* public - Field declaration            */

	public delegate float OnCalculateDamage( CCompoStat.SStat pStatAttacker, CCompoStat.SStat pStatVictim, float fDamageCalculated );
	public delegate void OnDeadObject( GameObject pObjectDead, int iObjectID );

	public event OnCalculateDamage p_Event_OnCalculateDamage;
	public event OnDeadObject p_Event_OnDead;

	[SerializeField]
	private EDamageCalculateOption _eDamageCalculateOption = EDamageCalculateOption.PlusMinus;
	[SerializeField]
	private int _iDamageMin = 0;

	private HashSet<GameObject> _setUnTouchable = new HashSet<GameObject>();

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Dictionary<int, CCompoStat> _mapObjectStat = new Dictionary<int, CCompoStat>();

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public bool DoCheckIs_ContainStat(GameObject pObjectTarget)
	{
		return _mapObjectStat.ContainsKey( pObjectTarget.GetInstanceID() );
	}

	public void DoSet_UnTouchableObject( GameObject pObjectUnTouchable, bool bIsUnTouchable )
	{
		int iID_UnTouchable = pObjectUnTouchable.GetInstanceID();
		if (_mapObjectStat.ContainsKey( iID_UnTouchable ) == false)
		{
			Debug.Log( "[Error] ManagerStat - Not Contain Stat" + pObjectUnTouchable.name, pObjectUnTouchable );
			return;
		}

		if(bIsUnTouchable)
		{
			if (_setUnTouchable.Contains( pObjectUnTouchable ) == false)
				_setUnTouchable.Add( pObjectUnTouchable );
		}
		else
		{
			if (_setUnTouchable.Contains( pObjectUnTouchable ))
				_setUnTouchable.Remove( pObjectUnTouchable );
		}
	}

	public void DoKillObject( GameObject pObjectVictim )
	{
		int iID_Victim = pObjectVictim.GetInstanceID();
		if(_mapObjectStat.ContainsKey( iID_Victim ) == false)
		{
			Debug.Log( "[Error] ManagerStat - Not Contain Stat" + pObjectVictim.name, pObjectVictim );
			return;
		}

		CCompoStat pStat_Victim = _mapObjectStat[iID_Victim];
		pStat_Victim.DoKill();
	}

	public void DoDamageObject( GameObject pObjectDamager, GameObject pObjectVictim )
	{
		bool bIsDead;
		DoDamageObject( pObjectDamager, pObjectVictim, out bIsDead );
	}

	public void DoDamageObject( GameObject pObjectDamager, GameObject pObjectVictim, out bool bIsDead)
	{
		if (pObjectDamager == null)
		{
			bIsDead = false;
			return;
		}

		bIsDead = false;
		int iID_Damager = pObjectDamager.GetInstanceID();
		int iID_Victim = pObjectVictim.GetInstanceID();

		if (_mapObjectStat.ContainsKey( iID_Damager ) == false ||
			_mapObjectStat.ContainsKey( iID_Victim ) == false)
		{
			//Debug.LogWarning( "[Error] ManagerStat - Not Contain Stat" );
			return;
		}

		CCompoStat pStat_Victim = _mapObjectStat[iID_Victim];
		if (pStat_Victim.p_bIsAlive == false || _setUnTouchable.Contains( pObjectVictim ))
		{
			pStat_Victim.DoDamage( 0, false, pObjectDamager );
			return;
		}

		CCompoStat pStat_Damager = _mapObjectStat[iID_Damager];
		bool bIsCriticalAttack = pStat_Damager.p_pStat.p_bIsCritical;

		float iDamage = CalculateDamage( pStat_Damager.p_pStat, pStat_Victim.p_pStat, bIsCriticalAttack );


		float fHP = pStat_Victim.p_pStat.p_iHPCurrent;
		pStat_Victim.DoDamage( iDamage, bIsCriticalAttack, pStat_Damager.gameObject, out bIsDead );

		if (bIsDead && p_Event_OnDead != null)
			p_Event_OnDead( pObjectVictim, iID_Victim );
	}

	public void DoRegistObjectStat( CCompoStat pStat, bool bIsReset = true )
	{
		int iObjectID = pStat.gameObject.GetInstanceID();
		if (_mapObjectStat.ContainsKey( iObjectID ) == false)
			_mapObjectStat.Add( iObjectID, pStat );

		if(bIsReset)
			_mapObjectStat[iObjectID].DoResetStat();
	}

	public void DoRemoveObjectStat( CCompoStat pStat )
	{
		int iObjectID = pStat.gameObject.GetInstanceID();
		if (_mapObjectStat.ContainsKey( iObjectID ))
			_mapObjectStat.Remove( iObjectID );
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

	public float CalculateDamage(CCompoStat.SStat pStatAttacker, CCompoStat.SStat pStatVictim, bool bIsCritical )
	{
		float iDamageCalculated = pStatAttacker.p_iDamageCurrent;
		switch (_eDamageCalculateOption)
		{
			case EDamageCalculateOption.Percentage:
				//iDamageCalculated *= (pStatVictim.p_iArmorCurrent * 0.01f);
				break;

			case EDamageCalculateOption.PlusMinus:
				iDamageCalculated -= pStatVictim.p_iArmorCurrent;
				break;
		}

		if (bIsCritical)
			iDamageCalculated *= pStatAttacker.p_iCriticalDamage;

		if (p_Event_OnCalculateDamage != null)
			p_Event_OnCalculateDamage( pStatAttacker, pStatVictim, iDamageCalculated );

		if (iDamageCalculated < _iDamageMin)
			iDamageCalculated = _iDamageMin;

		return iDamageCalculated;
	}

	#endregion Private
}
