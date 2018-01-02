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

public interface IStat_Buffer
{
	int GetStat_AddHP();
	int GetStat_AddArmor();
	int GetStat_AddDamage();
	int GetStat_CriticalPercent();
	int GetStat_CriticalDamage();
}

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
		private int iHP = 2;
		[SerializeField] [Header( "방어력은 퍼센트로 계산할 시 0 ~ 100으로 조정할 것" )]
		private int iArmor = 1;
		[SerializeField]
		private int iDamage_Min = 1;
		[SerializeField]
		private int iDamage_Max = 1;
		[SerializeField]
		private float fCriticalPercent = 10f;
		[SerializeField] [Header( "크리티컬 데미지는 기본 데미지에서 곱셈" )]
		private int iCriticalDamage_Min = 1;
		[SerializeField]
		private int iCriticalDamage_Max = 3;

		[Header( "디버그용 프린트" )]
		[SerializeField]
		private float iHP_Max; public float p_iHPMax { get { return iHP_Max; } }
		[SerializeField]
		private float iHP_Current; public float p_iHPCurrent { get { return iHP_Current; } }
		[SerializeField]
		private float iArmor_Current; public float p_iArmorCurrent { get { return iArmor_Current * _fArmor_Weight; } }
		[SerializeField]
		private float iDamage_Min_Current; public float p_iDamage_Min_Current { get { return iDamage_Min_Current; } }
		[SerializeField]
		private float iDamage_Max_Current; public float p_iDamage_Max_Current { get { return iDamage_Max_Current; } }
		[SerializeField]
		private float fCriticalPercent_Current; public float p_fCriticalPercentCurrent { get { return fCriticalPercent_Current; } }
		[SerializeField]
		private float iCriticalDamage_Min_Current; public float p_iCriticalDamage_Min_Current { get { return iCriticalDamage_Min_Current; } }
		[SerializeField]
		private float iCriticalDamage_Max_Current; public float p_iCriticalDamage_Max_Current { get { return iCriticalDamage_Max_Current; } }

		public float p_iDamageCurrent { get { return Random.Range( iDamage_Min_Current, iDamage_Max_Current + 1 ); } }
		public bool p_bIsCritical { get { return Random.Range( 0f, 100f ) <= fCriticalPercent_Current; } }
		public float p_iCriticalDamage { get { return Random.Range( iCriticalDamage_Min_Current, iCriticalDamage_Max_Current ); } }


		private float _fArmor_Weight = 1f;

		private List<IStat_Buffer> _listStatBuffer = new List<IStat_Buffer>();

		public void AddStatBuffer( IStat_Buffer pStatBuffer)
		{
			_listStatBuffer.Add( pStatBuffer );
		}

		public void RemoveStatBuffer( IStat_Buffer pStatBuffer )
		{
			_listStatBuffer.Remove( pStatBuffer );
		}

		public void DoAddCurrentStat_Armor(int fAddArmor)
		{
			iArmor_Current += fAddArmor;
		}

		public void DoSetStatWeight_Armor( float fWeight )
		{
			_fArmor_Weight = fWeight;
		}


		private int iID;
		private GameObject pObjectOwner; public GameObject p_pObjectOwner { get { return pObjectOwner; } }

		public SStat( int iHP, int iArmor, int iDamage_Min, int iDamage_Max, float fCriticalPercent, int iCriticalDamage_Min, int iCriticalDamage_Max )
		{
			this.iHP = iHP;
			this.iArmor = iArmor;
			this.iDamage_Min = iDamage_Min;
			this.iDamage_Max = iDamage_Max;
			this.fCriticalPercent = fCriticalPercent;
			this.iCriticalDamage_Min = iCriticalDamage_Min;
			this.iCriticalDamage_Max = iCriticalDamage_Max;
		}

		public bool CheckIsAlive()
		{
			return iHP_Current > 0f;
		}

		public void DoInit( GameObject pObjectOwner )
		{
			this.pObjectOwner = pObjectOwner;
			iID = pObjectOwner.GetInstanceID();
			DoReset();
		}
		
		public void DoIncrease_HP(float fPercent_0_100)
		{
			iHP_Current += iHP_Max * fPercent_0_100;
			if (iHP_Current > iHP_Max)
				iHP_Current = iHP_Max;
		}

		public void DoReset()
		{
			int iHP_Result = iHP;
			int iArmor_Result = iArmor;
			int iDamage_Min_Result = iDamage_Min;
			int iDamage_Max_Result = iDamage_Max;
			float fCriticalPercent_Result = fCriticalPercent;
			int iCriticalDamage_Min_Result = iCriticalDamage_Min;
			int iCriticalDamage_Max_Result = iCriticalDamage_Max;

			if(_listStatBuffer != null)
			{
				for (int i = 0; i < _listStatBuffer.Count; i++)
				{
					iHP_Result += _listStatBuffer[i].GetStat_AddHP();
					iArmor_Result += _listStatBuffer[i].GetStat_AddArmor();
					iDamage_Min_Result += _listStatBuffer[i].GetStat_AddDamage();
					iDamage_Max_Result += _listStatBuffer[i].GetStat_AddDamage();
					fCriticalPercent_Result += _listStatBuffer[i].GetStat_CriticalPercent();
					iCriticalDamage_Min_Result += _listStatBuffer[i].GetStat_CriticalDamage();
					iCriticalDamage_Max_Result += _listStatBuffer[i].GetStat_CriticalDamage();
				}
			}

			iHP_Max = iHP_Result;
			iHP_Current = iHP_Result;
			iArmor_Current = iArmor_Result;
			iDamage_Min_Current = iDamage_Min_Result;
			iDamage_Max_Current = iDamage_Max_Result;
			fCriticalPercent_Current = fCriticalPercent_Result;
			iCriticalDamage_Min_Current = iCriticalDamage_Min_Result;
			iCriticalDamage_Max_Current = iCriticalDamage_Max_Result;

		}

		public void DoKill()
		{
			iHP_Current = 0;
		}

		public void DoDamage( float iDamage, out float fHPDelta, out bool bIsDead )
		{
			iHP_Current -= iDamage;
			fHPDelta = GetHPDelta_0_1();
			bIsDead = iHP_Current < 1;
		}

		public float GetHPDelta_0_1()
		{
			return iHP_Current / iHP_Max; ;
		}
	}

	/* public - Field declaration            */

	public delegate void OnChangeStat( SStat pStatTarget );
	public delegate void OnDamage( GameObject pObjectDamager, float fDamage, bool bIsCriticalAttack, float fHPDelta, bool bIsDead );
	public delegate void OnDamage_OnDead( GameObject pObjectDamager );
	public delegate void OnResetStat( SStat pStatTarget );

	public event OnChangeStat p_Event_OnChangeStat;
	public event OnResetStat p_Event_OnResetStat;
	public event OnDamage p_Event_OnDamage;
	public event OnDamage_OnDead p_Event_OnDamage_OnDead;

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

	public void DoIncreaseStat_HP(float fPercent_0_100 )
	{
		_pStat.DoIncrease_HP( fPercent_0_100  );
		if(p_Event_OnChangeStat != null)
			p_Event_OnChangeStat( _pStat );
	}

	public void DoResetStat()
	{
		_pStat.DoReset();

		if(p_Event_OnResetStat != null)
			p_Event_OnResetStat( _pStat );
	}

	public void DoKill()
	{


		if (p_Event_OnDamage_OnDead != null)
			p_Event_OnDamage_OnDead( null );
	}

	public void DoDamage( float iDamage, bool bIsCriticalAttack, GameObject pObjectDamager )
	{
		bool bIsDead;
		DoDamage( iDamage, bIsCriticalAttack, pObjectDamager, out bIsDead );
	}

	public void DoDamage( float iDamage, bool bIsCriticalAttack, GameObject pObjectDamager, out bool bIsDead)
	{
		float fHPDelta = 0f;
		bIsDead = !_pStat.CheckIsAlive();
		if (bIsDead)
		{
			if (p_Event_OnDamage_OnDead != null)
				p_Event_OnDamage_OnDead( pObjectDamager );
		}
		else
		{
			_pStat.DoDamage( iDamage, out fHPDelta, out bIsDead );
			if (p_Event_OnDamage != null)
				p_Event_OnDamage( pObjectDamager, iDamage, bIsCriticalAttack, fHPDelta, bIsDead );
		}
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
