using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public interface ILevelEnemy
{
	int ILevelGetScore();
}

public class PCManagerLevel : CSingletonBase<PCManagerLevel>
{
	/* const & readonly declaration             */

	const int const_iNoEnemyLineCount_BeforeBoss = 2;
	const int const_iDistanceUnit = 10;

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public bool p_bLock_RegenEnemy = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<int, List<PCCompo_PatternPlay>> _mapSpawner_Batch = new Dictionary<int, List<PCCompo_PatternPlay>>();
	private HashSet<int> _setNoEnemyLine = new HashSet<int>();
	private Dictionary<EMissionEnemy, List<PCCompo_PatternPlay>> _mapSpawnerEnemy = new Dictionary<EMissionEnemy, List<PCCompo_PatternPlay>>();

	[Header("디버그용 모니터링")] [SerializeField]
	public int _iDifficulty;
	private int _iDifficultyRemain;

	private float const_fDifficultyAdjustValue;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoStartLevel(int iDifficulty)
	{
		_iDifficulty = iDifficulty;
		const_fDifficultyAdjustValue = SDataGame.GetFloat( EDataGameField.fDifficultyAdjusted );
	}

	public void DoSetDistance(int iDistance)
	{
		if (p_bLock_RegenEnemy) return;
		if (_setNoEnemyLine.Contains( iDistance ))
		{
			PCManagerUIInMission.instance.DoShowUI_BossSpawn( PCManagerInMission.instance.DoPlayBGM_Boss );
			return;
		}
		
		//Debug.Log( "DoSetDistance" + iDistance );

		// 수동으로 배치한 레벨 디자인이 있을경우 그것을 실행
		if (_mapSpawner_Batch.ContainsKey( iDistance ))
		{
			bool bIsLock = false;
			for (int i = 0; i < _mapSpawner_Batch[iDistance].Count; i++)
			{
				if(bIsLock == false)
					bIsLock = _mapSpawner_Batch[iDistance][i].DoPlayPattern();
				else
					_mapSpawner_Batch[iDistance][i].DoPlayPattern();
			}

			p_bLock_RegenEnemy = bIsLock;
		}
		else // 아닐 경우 현재 레벨 난이도와 거리에 따라 랜덤 적 소환
		{
			int iEmptyLoopCount = 0;
			_iDifficultyRemain = (int)(_iDifficulty * iDistance * const_fDifficultyAdjustValue);
			//Debug.Log( _iDifficultyRemain );
			while(_iDifficultyRemain > 0)
			{
				SDataMission_Enemy pDataMissionEnemy = CManagerRandomTable<SDataMission_Enemy>.instance.GetRandomItem( _iDifficultyRemain );
				EMissionEnemy eEnemy = pDataMissionEnemy.IDictionaryItem_GetKey();
				if (_mapSpawnerEnemy.ContainsKey( eEnemy ) == false)
				{
					//Debug.LogWarning( "Manager Level에 " + eEnemy + " Spawn 정보가 없습니다" );
					if (iEmptyLoopCount++ > 10)
						break;
					else
						continue;
				}

				int iGeneateCount = 0;
				for (int i = 0; i < _mapSpawnerEnemy[eEnemy].Count; i++)
					iGeneateCount += _mapSpawnerEnemy[eEnemy][i].DoPlayPatternEnemy_RandomOne();

				_iDifficultyRemain -= iGeneateCount * pDataMissionEnemy.iScore;
				//Debug.Log( eEnemy + "Play iGeneateCount : " + iGeneateCount + " Pattern Reain : " + _iDifficultyRemain );
			}
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		PCCompo_PatternPlay[] arrSpanwer = GetComponentsInChildren<PCCompo_PatternPlay>(true );
		_mapSpawner_Batch.DoAddItem( arrSpanwer );
		_mapSpawnerEnemy.DoAddItem( arrSpanwer );

		_setNoEnemyLine.Add( 0 );
		for (int i = 0; i < arrSpanwer.Length; i++)
		{
			if(arrSpanwer[i].bIsBoss)
			{
				for(int j = 1; j < const_iNoEnemyLineCount_BeforeBoss; j++)
					_setNoEnemyLine.Add( arrSpanwer[i].iSpawnRuna - (j * const_iDistanceUnit) );
			}
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
