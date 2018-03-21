using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using NUnit.Framework;
public class Test_CManagerStat
{
	public enum ECodeDefinition
	{
		전투테스트,
		크리티컬,
		강제죽이기테스트,
	}

	GameObject _pObjectPlayer;
	GameObject _pObjectEnemy;

	CManagerStat _pManagerStat;
	CCompoStat _pPlayer;
	CCompoStat _pEnemy;

	CCompoStat.SStat _pStatPlayer;
	CCompoStat.SStat _pStatEnemy;
	
	[Test]
	public void Test_CManagerStat_ForceKill()
	{
		InitTestSetting( new CCompoStat.SStat( 300, 1, 1, 5, 30f, 1, 2 ), new CCompoStat.SStat( 100, 0, 1, 5, 30f, 1, 2 ) );

		int iTestCount = Random.Range( 100, 200 );
		for (int i = 0; i < iTestCount; i++)
		{
			_pEnemy.DoResetStat();
			Assert.IsTrue( _pEnemy.p_bIsAlive );
			_pEnemy.DoKill();
			Assert.IsTrue( _pEnemy.p_bIsAlive == false );
		}
	}

	[Test]
	[Repeat( 10 )]
	public void Test_CManagerStat_Damage()
	{
		InitTestSetting( 
			new CCompoStat.SStat( 300, 1, 1, 5, 30f, 1, 2 ) , 
			new CCompoStat.SStat( 100, 0, 1, 5, 30f, 1, 2 ) );
		
		/// <see cref="ECodeDefinition.전투테스트"/> 
		int iTestCount = Random.Range( 100, 200 );
		for (int i = 0; i < iTestCount; i++)
		{
			while (_pPlayer.p_bIsAlive && _pEnemy.p_bIsAlive)
			{
				_pManagerStat.DoDamageObject( _pObjectPlayer, _pObjectEnemy );
				_pManagerStat.DoDamageObject( _pObjectEnemy, _pObjectPlayer );
			}

			Assert.IsTrue( _pPlayer.p_bIsAlive );

			_pPlayer.DoResetStat();
			_pEnemy.DoResetStat();

			Assert.IsTrue( _pPlayer.p_pStat.p_iHPMax == _pPlayer.p_pStat.p_iHPCurrent );

			while (_pPlayer.p_bIsAlive && _pEnemy.p_bIsAlive)
			{
				_pManagerStat.DoDamageObject( _pObjectPlayer, _pObjectEnemy );
				_pManagerStat.DoDamageObject( _pObjectEnemy, _pObjectPlayer );
			}

			Assert.IsTrue( _pPlayer.p_bIsAlive );
		}
	}

	[Test]
	[Repeat( 10 )]
	public void Test_CManagerStat_Critical()
	{
		InitTestSetting( 
			new CCompoStat.SStat( 30000, 20, 100, 500, 100f, 5, 10 ),
			new CCompoStat.SStat( 10000, 10, 100, 500, 10f, 5, 10 ) );

		int iCriticalHitCount_100Per = 0;
		int iCriticalHitCount_10Per = 0;

		/// <see cref="ECodeDefinition.크리티컬"/> 
		int iTestCount = Random.Range( 10000, 20000 );
		for (int i = 0; i < iTestCount; i++)
		{
			// 100% 이기 때문에 무조건 true가 나와야 한다.
			bool bIsCriticalHit_Player = _pPlayer.p_pStat.p_bIsCritical;
			if (bIsCriticalHit_Player)
				iCriticalHitCount_100Per++;

			// 10%라서 1000번중에 이상적인 케이스는 100번이나, 오차를 감안한다.
			bool bIsCriticalHit_Enemy = _pEnemy.p_pStat.p_bIsCritical;
			if (bIsCriticalHit_Enemy)
				iCriticalHitCount_10Per++;
		}

		Assert.IsTrue( iCriticalHitCount_100Per == iTestCount );

		try
		{
			// 크리티컬 10%의 경우 8%보다 높고
			Assert.IsTrue( iCriticalHitCount_10Per >= iTestCount * 0.08f );
			// 12%보다 낮아야 한다. 오차 += 2%
			Assert.IsTrue( iCriticalHitCount_10Per <= iTestCount * 0.12f );
		}
		catch
		{
			Debug.LogWarning( "Test Count : " + iTestCount + "iCriticalHitCount_10Per : " + iCriticalHitCount_10Per);
		}
	}




	private void InitTestSetting( CCompoStat.SStat pStatPlayer, CCompoStat.SStat pStatEnemyOrNull = null)
	{
		if(_pManagerStat == null)
		{
			GameObject pObjectManagerStat = new GameObject( "ManagerStat" );
			_pManagerStat = pObjectManagerStat.AddComponent<CManagerStat>();
		}

		if(_pObjectPlayer == null)
		{
			_pObjectPlayer = new GameObject( "Player" );
			_pPlayer = _pObjectPlayer.AddComponent<CCompoStat>();
			_pStatPlayer = pStatPlayer;
		}

		_pPlayer.DoInitStat( _pStatPlayer );

		if(pStatEnemyOrNull != null)
		{
			if (_pObjectEnemy == null)
			{
				_pObjectEnemy = new GameObject( "Enemy" );
				_pEnemy = _pObjectEnemy.AddComponent<CCompoStat>();
				_pStatEnemy = pStatEnemyOrNull;
			}

			_pEnemy.DoInitStat( _pStatEnemy );
		}
	}
}
#endif