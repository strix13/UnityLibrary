using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class PCManagerInMission : PCManagerGameBase<PCManagerInMission>
{
	/* const & readonly declaration             */

	const float const_fLeaveDelaySec = 0.5f;
	const float const_fAdjust_UnityPosToRuna = 0.01f;
	const float const_fFuelIncreaseSpeed_OnStation = 0.5f;

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	[SerializeField]
	private float _fFuelCurrent = 0;


	[SerializeField]
	private float fShakeTest = 0.2f;
	[SerializeField]
	private float fShakeTestTime = 0.5f;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private List<PCMission_Enemy> _listEnemyAlive = new List<PCMission_Enemy>();

	private CNGUITweenPositionExtend _pTweenPosBackground;
	private CShakeObject _pCamShake;
	private PCMission_Player _pPlayer;	public PCMission_Player p_pPlayer {  get { return _pPlayer; } }

	private int _iCombo;

	private int _iGenerateLine_Last = 0;
	private int _iRouletteTicket;

	private Camera _pCameraInGame;
	private PCMission_SpaceStation _pStation;
	private SInfoUser _pUserInfo;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoAddGold(int iGold)
	{
		_pUserInfo.iGold += iGold;
		PCManagerUIInMission.instance.DoSetTotal_Gold(_pUserInfo.iGold, iGold);
	}

	public void DoAddRouletteTicket()
	{
		_iRouletteTicket++;
		PCManagerUIInMission.instance.DoSetTotal_Roulette( _iRouletteTicket, 1 );
	}

	public void DoDecreaseRouletteTicket()
	{
		_iRouletteTicket--;
		PCManagerUIInMission.instance.DoSetTotal_Roulette( _iRouletteTicket, -1 );
	}


	public void DoShowSpaceStation()
	{
		PCManagerUIInMission.instance.DoShowUI_BossClear( DoPlayBGM );

		_pStation = CManagerPooling<EMissionObject, PCMission_SpaceStation>.instance.DoPop( EMissionObject.SpaceStation );
		_pStation.DoShowStation( ProcOnDockingStation );
		_pPlayer.DoSet_ShotingBullet( false );
	}

	public void DoPlayOrStop_DecreaseFuel(bool bPlay)
	{
		if(bPlay)
			StartCoroutine( "CoAutoDecreaseFuel" );
		else
			StopCoroutine( "CoAutoDecreaseFuel" );
	}

	public void DoPlayOrStop_CheckPlayerPos( bool bPlay )
	{
		if(bPlay)
			StartCoroutine( "CoCheckPlayerPos" );
		else
		{
			if(_pTweenPosBackground != null)
				_pTweenPosBackground.DoPlayOrStop_CheckTweenAmount( false );
			StopCoroutine( "CoCheckPlayerPos" );
		}
	}

	public void DoRegistEnemyList(PCMission_Enemy pEnemy)
	{
		_listEnemyAlive.Add(pEnemy);

		//Debug.Log( pEnemy.name + "Enmey Count : " + _listEnemyAlive.Count, pEnemy );
	}

	public void DoRemoveEnmeyList(PCMission_Enemy pEnemy)
	{
		_listEnemyAlive.Remove(pEnemy);
		
		//Debug.Log( pEnemy.name + "Enmey Count : " + _listEnemyAlive.Count, pEnemy );
	}

	public PCMission_Enemy DoGetClosestEnemy(Vector2 vecPos)
	{
		PCMission_Enemy pClosestEnemy = null;
		float fMinDistance = float.MaxValue;
		for (int i = 0; i < _listEnemyAlive.Count; i++)
		{
			Vector2 vecDirection = vecPos - (Vector2)_listEnemyAlive[i].p_pTransCached.position;
			float fDistance = Vector2.SqrMagnitude(vecDirection);
			if (fDistance < fMinDistance)
			{
				fMinDistance = fDistance;
				pClosestEnemy = _listEnemyAlive[i];
			}
		}

		return pClosestEnemy;
	}

	public void DoPlayBGM()
	{
		PCManagerFramework.p_pManagerSound.DoPlayBGM( ESoundGroup_BGM.Mission.GetRandomEnumInGroup<ESoundName>(), DoPlayBGM );
	}

	public void DoPlayBGM_Boss()
	{
		PCManagerFramework.p_pManagerSound.DoPlayBGM( ESoundGroup_BGM.MissionBoss.GetRandomEnumInGroup<ESoundName>(), DoPlayBGM_Boss );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void PManagerPool_Item_p_EVENT_OnMakeResource(EMissionItem arg1, PCMission_Item arg2)
	{
		arg2.p_eMissionItem = arg1;
		arg2.transform.DoResetTransform();
	}

	private void PManagerPool_Effect_p_EVENT_OnMakeResource(EMissionEffect arg1, PCEffect arg2)
	{
		arg2.transform.DoResetTransform();
	}

	private void PManagerPool_MissionObject_p_EVENT_OnMakeResource( EMissionObject arg1, PCMission_SpaceStation arg2 )
	{
		arg2.transform.DoResetTransform();
	}

	private void _pManagerPool_Enemey_p_EVENT_OnMakeResource(EMissionEnemy arg1, PCMission_Enemy arg2)
	{
		arg2.eMissionEnemy = arg1;
		if (PCManagerFramework.g_mapEnemy.ContainsKey(arg1))
			arg2.DoInitEnemy(PCManagerFramework.g_mapEnemy[arg1]);
		else
		{
			arg2.DoInitEnemy();
			Debug.LogWarning("아직 DB에 정보가 없습니다.. " + arg1 + " ID : " + (int)arg1);
		}
	}

	private void G_pManagerPool_Bullet_p_EVENT_OnMakeResource(EMissionBullet arg1, PCMission_Bullet arg2)
	{
		arg2.transform.localScale = Vector3.one;
	}

	/* protected - Override & Unity API         */

	protected override void OnGameStart(int iDifficultyLevel, bool bIsTest)
	{
		base.OnGameStart(iDifficultyLevel, bIsTest );

		_iGenerateLine_Last = 0;
		_iRouletteTicket = 1;
		_pUserInfo = PCManagerFramework.p_pInfoUser;
		_fFuelCurrent = SDataGame.GetFloat( EDataGameField.fMissionFuelOnStart );

		PCMission_Player[] arrPlayer = GetComponentsInChildren<PCMission_Player>(true);
		ECharacterName eSelectCharacter = PCManagerFramework.p_pInfoUser.eCharacterCurrent;
		for (int i = 0; i < arrPlayer.Length; i++)
		{
			if(arrPlayer[i].eCharacterName == eSelectCharacter)
			{
				_pPlayer = arrPlayer[i];
				break;
			}
		}

		if(_pPlayer == null)
		{
			Debug.Log( "기체가 없어 레이나 기체로 시작" );
			for (int i = 0; i < arrPlayer.Length; i++)
			{
				if (arrPlayer[i].eCharacterName == ECharacterName.reina)
				{
					_pPlayer = arrPlayer[i];
					break;
				}
			}
		}

		_pPlayer.DoInitPlayer( _pCameraInGame, bIsTest );
		DoPlayOrStop_CheckPlayerPos( true );
		DoPlayOrStop_DecreaseFuel( true );

		PCManagerLevel.instance.DoStartLevel( iDifficultyLevel );
		PCManagerUIInMission.instance.DoSetTotal_Roulette( _iRouletteTicket, 0 );
		PCManagerUIInMission.instance.DoSetTotal_Gold( _pUserInfo.iGold );
		PCManagerUIInMission.instance.DoSetTotal_Ticket( _pUserInfo.iTicket );

		Debug.Log( "Start Game"  + iDifficultyLevel );

		DoPlayBGM();
	}

	protected override void OnAddScore(int iAddScore)
	{
		base.OnAddScore(iAddScore);
		
		_iCombo++;
		if (_iCombo % 10 == 0)
			Debug.Log("Combo : " + _iCombo);

		PCManagerUIInMission.instance.DoSetTotal_Score(_iScoreTotal, iAddScore );
	}

	protected override void OnAddFeverGauge(float fFeverAdd)
	{
		base.OnAddFeverGauge( fFeverAdd );

		PCManagerUIInMission.instance.DoSetTotal_FeverGuage(_fFeverGauge, fFeverAdd );
	}

	protected override void OnFever()
	{
		base.OnFever();

		PCManagerUIInMission.instance.EventOnFever();
	}

	protected override void OnDecreaseHP(float fDecreaseHP, bool bShake )
	{
		base.OnDecreaseHP(fDecreaseHP, bShake );

		_iCombo = 0;
		_fFuelCurrent -= fDecreaseHP;

		if (_fFuelCurrent <= 0)
		{
			OnGameFinish(false);
		}
		else
		{
			if(bShake)
			{
				_pPlayer.DoDamagePlayer();
				_pCamShake.DoShakeObject( fShakeTest, fShakeTestTime );
			}
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		if (PCManagerFramework.instance == null)
		{
			Debug.LogWarning("인게임 미션UI 씬이 없으므로 로딩합니다.");
			PCManagerFramework.DoLoadScene(ESceneName.InGameUI_Mission, UnityEngine.SceneManagement.LoadSceneMode.Additive);
		}

		CManagerPooling<EMissionBullet, PCMission_Bullet> pManagerPool_Bullet = CManagerPooling<EMissionBullet, PCMission_Bullet>.instance;
		Transform pTransManagerPool = pManagerPool_Bullet.p_pObjectManager.transform;
		pTransManagerPool.SetParent(transform);
		pTransManagerPool.localScale = Vector3.one;

		CManagerPooling<EMissionEnemy, PCMission_Enemy> pManagerPool_Enemy = CManagerPooling<EMissionEnemy, PCMission_Enemy>.instance;
		pManagerPool_Enemy.p_pObjectManager.transform.SetParent(transform);
		pManagerPool_Enemy.p_pObjectManager.transform.DoResetTransform();
		pManagerPool_Enemy.p_EVENT_OnMakeResource += _pManagerPool_Enemey_p_EVENT_OnMakeResource;

		CManagerPooling<EMissionEffect, PCEffect> pManagerPool_Effect = CManagerPooling<EMissionEffect, PCEffect>.instance;
		pManagerPool_Effect.p_pObjectManager.transform.SetParent(transform);
		pManagerPool_Effect.p_pObjectManager.transform.DoResetTransform();
		pManagerPool_Effect.p_EVENT_OnMakeResource += PManagerPool_Effect_p_EVENT_OnMakeResource;
		PCMission_Enemy.g_pManagerPool_Effect = pManagerPool_Effect;

		CManagerPooling<EMissionItem, PCMission_Item> pManagerPool_Item = CManagerPooling<EMissionItem, PCMission_Item>.instance;
		pManagerPool_Item.p_pObjectManager.transform.SetParent(transform);
		pManagerPool_Item.p_pObjectManager.transform.DoResetTransform();
		pManagerPool_Item.p_EVENT_OnMakeResource += PManagerPool_Item_p_EVENT_OnMakeResource; ;
		PCMission_Enemy.g_pManagerPool_Item = pManagerPool_Item;

		CManagerPooling<EMissionObject, PCMission_SpaceStation> pManagerPool_MissionObject = CManagerPooling<EMissionObject, PCMission_SpaceStation>.instance;
		pManagerPool_MissionObject.p_pObjectManager.transform.SetParent( transform );
		pManagerPool_MissionObject.p_pObjectManager.transform.DoResetTransform();
		pManagerPool_MissionObject.p_EVENT_OnMakeResource += PManagerPool_MissionObject_p_EVENT_OnMakeResource;

		_pCameraInGame = GetComponentInChildren<Camera>();
		_pCamShake = GetComponentInChildren<CShakeObject>();
		_pPlayer = GetComponentInChildren<PCMission_Player>();

		_pTweenPosBackground = GetComponentInChildren<CNGUITweenPositionExtend>();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcOnDockingStation()
	{
		Debug.Log( "도킹, 룰렛 출현" + _iRouletteTicket );
		PCManagerFramework.DoNetworkDB_Update_Set<SInfoUser>( "iGold", _pUserInfo.iGold, null );
		StartCoroutine( "CoIncreaseFuel" );
		PCUIInPopup_MissionRoulette pUIPopupRoulette = PCManagerUIInMission.instance.DoShowHide_Popup( PCManagerUIInMission.EPopup.PCUIInPopup_MissionRoulette, true ) as PCUIInPopup_MissionRoulette;
		if (_iRouletteTicket > 0)
		{
			pUIPopupRoulette.DoRunRoulette( OnRouletteFinish );
			DoDecreaseRouletteTicket();
		}
		else
		{
			pUIPopupRoulette.DoHideRoulette();
			ProcOnLeaveStation();
		}

		//_pStation.DoLeaveStation();
	}

	private void ProcOnLeaveStation()
	{
		Debug.Log( "ProcOnLeaveStation" );
		StartCoroutine( CoLeaveStation() );
	}

	private void OnRouletteFinish()
	{
		PCUIInPopup_MissionRoulette pUIPopupRoulette = PCManagerUIInMission.instance.GetUIPopup<PCUIInPopup_MissionRoulette>();
		if (_iRouletteTicket > 0)
		{
			pUIPopupRoulette.DoRunRoulette( OnRouletteFinish );
			DoDecreaseRouletteTicket();
		}
		else
		{
			pUIPopupRoulette.DoHideRoulette();
			ProcOnLeaveStation();
		}			
	}

	private IEnumerator CoLeaveStation()
	{
		yield return new WaitForSeconds( const_fLeaveDelaySec );

		_pStation.DoLeaveStation();
	}

	private IEnumerator CoCheckPlayerPos()
	{
		_pTweenPosBackground.DoPlayOrStop_CheckTweenAmount( true );
		PCManagerLevel pManagerLevel = PCManagerLevel.instance;
		int iLevelRegenDistance = SDataGame.GetInt( EDataGameField.iLevelRegenDistance );
		while (true)
		{
			int iPlayerPosZ = Mathf.RoundToInt( _pTweenPosBackground .p_fTweenAmount);
			int iGeneateLine = iPlayerPosZ / iLevelRegenDistance;
			if (iGeneateLine > _iGenerateLine_Last)
			{
				_iGenerateLine_Last = iGeneateLine;
				pManagerLevel.DoSetDistance( Mathf.FloorToInt( iGeneateLine * iLevelRegenDistance * const_fAdjust_UnityPosToRuna ));
			}
			int iRuna = Mathf.RoundToInt( iPlayerPosZ * const_fAdjust_UnityPosToRuna );
			PCManagerUIInMission.instance.DoSetTotal_Runa( iRuna );

			yield return null;
		}
	}

	private IEnumerator CoAutoDecreaseFuel()
	{
		float fFuelDecreaseSpeed = SDataGame.GetFloat( EDataGameField.fMissionFuelDecreaseSpeed );
		while(_fFuelCurrent > 0f)
		{
			DoDecreasePlayerHP( fFuelDecreaseSpeed, false );
			
			yield return new WaitForSeconds( Time.deltaTime );
		}
		
		OnGameFinish( false );
	}

	private IEnumerator CoIncreaseFuel()
	{
		float fFuelIncreaseSpeed = const_fFuelIncreaseSpeed_OnStation;
		float fFuelMax = SDataGame.GetFloat( EDataGameField.fMissionFuelOnStart );

		while (_fFuelCurrent < fFuelMax)
		{
			DoDecreasePlayerHP( -fFuelIncreaseSpeed, false );

			yield return null;
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
