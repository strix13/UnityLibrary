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

public class CCompoStat : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	[System.Serializable]
	public class SStat
	{
		// 인스펙터 세팅
		[Header( "스텟 세팅" )]
		[SerializeField]
		private float fHP = 2f;
		[SerializeField] [Header( "방어력은 퍼센트로 계산할 시 0 ~ 100으로 조정할 것" )]
		private float fArmor = 1f;
		[SerializeField]
		private float fDamage_Min = 1f;
		[SerializeField]
		private float fDamage_Max = 1f;
		[SerializeField]
		private float fCriticalPercent = 10f;
		[SerializeField] [Header( "크리티컬 데미지는 기본 데미지에서 곱셈" )]
		private float fCriticalDamage_Min = 1.5f;
		[SerializeField]
		private float fCriticalDamage_Max = 3f;

		[Header( "디버그용 프린트" )]
		[SerializeField]
		private float fHP_Max; public float p_fHPMax { get { return fHP_Max; } }
		[SerializeField]
		private float fHP_Current; public float p_fHPCurrent { get { return fHP_Current; } }
		[SerializeField]
		private float fArmor_Current; public float p_fArmorCurrent { get { return fArmor_Current; } }
		[SerializeField]
		private float fDamage_Min_Current; public float p_fDamage_Min_Current { get { return fDamage_Min_Current; } }
		[SerializeField]
		private float fDamage_Max_Current; public float p_fDamage_Max_Current { get { return fDamage_Max_Current; } }
		[SerializeField]
		private float fCriticalPercent_Current; public float p_fCriticalPercentCurrent { get { return fCriticalPercent_Current; } }

		public float p_fDamage { get { return Random.Range( fDamage_Min_Current, fDamage_Max_Current ); } }
		public bool p_bIsCritical { get { return Random.Range( 0f, 100f ) <= fCriticalPercent_Current; } }
		public float p_fCriticalDamage { get { return Random.Range( fCriticalDamage_Min, fCriticalDamage_Max ); } }

		private int iID;
		private GameObject pObjectOwner; public GameObject p_pObjectOwner { get { return pObjectOwner; } }

		public SStat( float fHP, float fArmor, float fDamage_Min, float fDamage_Max, float fCriticalPercent, float fCriticalDamage_Min, float fCriticalDamage_Max )
		{
			this.fHP = fHP;
			this.fArmor = fArmor;
			this.fDamage_Min = fDamage_Min;
			this.fDamage_Max = fDamage_Max;
			this.fCriticalPercent = fCriticalPercent;
			this.fCriticalDamage_Min = fCriticalDamage_Min;
			this.fCriticalDamage_Max = fCriticalDamage_Max;
		}

		public bool CheckIsAlive()
		{
			return fHP_Current > 0f;
		}

		public void DoInit( GameObject pObjectOwner )
		{
			this.pObjectOwner = pObjectOwner;
			iID = pObjectOwner.GetInstanceID();
		}

		public void DoReset()
		{
			fHP_Max = fHP;
			fHP_Current = fHP;
			fArmor_Current = fArmor;
			fDamage_Min_Current = fDamage_Min;
			fDamage_Max_Current = fDamage_Max;
			fCriticalPercent_Current = fCriticalPercent;
		}

		public void DoDamage( float fDamage, out float fHPDelta, out bool bIsDead )
		{
			fHP_Current -= fDamage;
			fHPDelta = fHP_Current / fHP;
			bIsDead = fHP_Current <= 0f;
		}
	}

	/* public - Field declaration            */

	public delegate void OnDamage( GameObject pObjectDamager, float fDamage, bool bIsCriticalAttack, float fHPDelta, bool bIsDead );
	public delegate void OnResetStat( SStat pStatTarget );

	public event OnResetStat p_Event_OnResetStat;
	public event OnDamage p_Event_OnDamage;

	[SerializeField]
	protected SStat _pStat; public SStat p_pStat { get { return _pStat; } }

	public bool p_bIsAlive {  get { return _pStat.CheckIsAlive(); } }

	/* protected - Field declaration         */

	protected CManagerStat _pManagerStat;

	/* private - Field declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitStat(SStat pStat)
	{
		_pStat = pStat;
		_pStat.DoInit(gameObject);
	}

	public void DoResetStat()
	{
		_pStat.DoReset();

		if(p_Event_OnResetStat != null)
			p_Event_OnResetStat( _pStat );
	}

	public void DoDamage(float fDamage, bool bIsCriticalAttack, GameObject pObjectDamager, out bool bIsDead)
	{
		float fHPDelta = 0f;
		bIsDead = !_pStat.CheckIsAlive();
		if (bIsDead == false)
			_pStat.DoDamage( fDamage, out fHPDelta, out bIsDead );

		if (p_Event_OnDamage != null)
			p_Event_OnDamage( pObjectDamager, fDamage, bIsCriticalAttack, fHPDelta, bIsDead );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pStat.DoInit(gameObject);

		_pManagerStat = CManagerStat.instance;
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if(_pManagerStat != null)
			_pManagerStat.DoRegistObjectStat( this );
	}

	protected override void OnDisableObject()
	{
		base.OnDisableObject();

		if (_pManagerStat != null)
			_pManagerStat.DoRemoveObjectStat( this );
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
