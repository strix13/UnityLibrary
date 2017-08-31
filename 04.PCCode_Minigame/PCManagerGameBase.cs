using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCManagerGameBase<DERIVED_CLASS> : CSingletonBase<DERIVED_CLASS>
	where DERIVED_CLASS : PCManagerGameBase<DERIVED_CLASS>
{
	/* const & readonly declaration             */

	private const int const_iMaxFeverGauge = 100;

	/* enum & struct declaration                */

	public enum EUIPopupCommon
	{
		OnGameStart,
		OnGameFinish_Clear,
		OnGameFinish_Fail,
		OnAddScore,
		OnAddFeaverGauge,
		OnFeaver
	}

	public enum EGameState
	{
		Tutorial,
		Wait,
		Start,
		Running,
		Dead,
		Clear,
		Fail
	}

	/* public - Variable declaration            */

	public event System.Action p_EVENT_OnGameStart;
	public event System.Action<bool> p_EVENT_OnGameFinish;
	public event System.Action<float> p_EVENT_OnPlayerTakeDamage;
	public event System.Action p_EVENT_OnFever;

	/* protected - Variable declaration         */

	protected CFSM<EGameState> _pFSMGameState = new CFSM<EGameState>();

	protected ECharacterName _ePlayCharacter = ECharacterName.None;
	protected int _iDifficultyLevel;
	protected int _iScoreTotal; public int p_iScoreTotal { get { return _iScoreTotal; } }
	protected float _fFeverGauge;

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoStartTutorial()
	{
		_iDifficultyLevel = 1;
	}

	public void DoStartGame(int iDifficultyLevel)
	{
		_iDifficultyLevel = iDifficultyLevel;

		OnGameStart( iDifficultyLevel, false );
	}

	public void DoStartGame_Test()
	{
		OnGameStart( 1, true);
	}

	public void DoFinishGame(bool bIsClear)
	{
		OnGameFinish( bIsClear );
	}

	public void DoAddScore(int iScore)
	{
		OnAddScore( iScore );
	}

	public void DoAddFeverGuage(float fFeverAdd)
	{
		OnAddFeverGauge(fFeverAdd );
	}

	public void DoDecreasePlayerHP(float fDecreaseHP, bool bShake)
	{
		OnDecreaseHP( fDecreaseHP, bShake );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/
	   
	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	//abstract protected void OnRegistCommonPopup();

	virtual protected void OnChangeGameState( EGameState eGameState ) { }

	virtual protected void OnGameStart( int iDifficultyLevel, bool bIsTest )
	{
		if (p_EVENT_OnGameStart != null)
			p_EVENT_OnGameStart();
	}

	virtual protected void OnGameFinish(bool bIsClear)
	{
		if (p_EVENT_OnGameFinish != null)
			p_EVENT_OnGameFinish(bIsClear);
	}

	virtual protected void OnAddScore(int iAddScore)
	{
		_iScoreTotal += iAddScore;
	}

	virtual protected void OnAddFeverGauge( float fFeverAdd )
	{
		_fFeverGauge += fFeverAdd;

		if (_fFeverGauge >= const_iMaxFeverGauge)
			OnFever();
	}

	virtual protected void OnFever()
	{
		_fFeverGauge = 0;

		//if (p_EVENT_OnFever != null)
		//	p_EVENT_OnFever();
	}

	virtual protected void OnDecreaseHP(float fDecreaseHP, bool bShake )
	{
		if (p_EVENT_OnPlayerTakeDamage != null)
			p_EVENT_OnPlayerTakeDamage(fDecreaseHP);
	}

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pFSMGameState.p_EVENT_OnChangeState += OnChangeGameState;

		if (PCManagerFramework.instance == null)
		{
			PCManagerFramework.DoLoadScene( ESceneName.Common, UnityEngine.SceneManagement.LoadSceneMode.Additive );
			PCManagerFramework.p_EVENT_OnDBLoadFinish += DoStartGame_Test;
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */
	   
	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
