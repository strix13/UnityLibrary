using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public enum ESoundName
{
	BGM_Title_LenskoCetus,

	Boss,
	CK_Blaster_Shot_226,
	CoinSound,

	getgold,

	smallenemyboom,
	bossboom,

	BGM_idle1,
	BGM_idle2,
	BGM_idle3,
	BGM_idle4,
	BGM_idle5,
	BGM_idle6,

	BGM_boss1,
	BGM_boss2,
}

public enum ESoundGroup_BGM
{
	OutGame,
	Mission,
	MissionBoss,
}

public enum EEffectName
{

}

public enum ESceneName
{
	Common,
	OutGame,
	Mission,
	Rush,
	Start,
	Title,
	InGame_Mission,
	InGameUI_Mission
}

public class PCManagerFramework : CManagerFrameWorkBase<PCManagerFramework, ESoundName, EEffectName, ESceneName, PCEffect, PCSoundPlayer>
{
	/* const & readonly declaration             */

	const int const_iIDLength = 40;
	const int const_iTryLimitCount = 10;

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	static public System.Action p_EVENT_OnDBLoadFinish;

	static public Dictionary<ECharacterName, SDataMission_Character> g_mapMissionCharacterInfo = new Dictionary<ECharacterName, SDataMission_Character>();
	static public Dictionary<EMissionEnemy, SDataMission_Enemy> g_mapEnemy = new Dictionary<EMissionEnemy, SDataMission_Enemy>();
	static public Dictionary<EMissionBullet, SDataMission_Bullet> g_mapBullet = new Dictionary<EMissionBullet, SDataMission_Bullet>();
	static public SInfoUser p_pInfoUser;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	static private SGameData_Legacy _pDataGame = new SGameData_Legacy(); static public SGameData_Legacy p_pDataGame { get { return _pDataGame; } }
	static private int _iTryCount_CreateID = 0;

	private int _iDBCount_Request;
	private int _iDBCount_Finish;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoLoadScene_FadeInOut( ESceneName eSceneName )
	{
		DoLoadScene_FadeInOut( eSceneName, 1f, Color.black );
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

		_pManagerScene.p_EVENT_OnSceneLoaded += ProcOnSceneLoaded;

		ProcResource();
	}

	// 구글 플레이 연결 전까진 ID를 로컬파일에서 담당
	protected override void OnLoadFinish_INI_PlayerSetting( bool bSuccess, SINI_UserSetting sUserSetting )
	{
		base.OnLoadFinish_INI_PlayerSetting( bSuccess, sUserSetting );
		
		// 로컬 파일에 없어서 새로 접속하는 경우
		if (bSuccess == false || sUserSetting.ID == null || sUserSetting.ID == "")
		{
			// 네트워크에 ID 생성 요청
			sUserSetting.ID = SCManagerLogIn.CalculateHashPassword( const_iIDLength );
			_strID = sUserSetting.ID;

			p_pManagerJsonINI.DoWriteJson( EINI_JSON_FileName.UserSetting, sUserSetting );
			Debug.Log( "로컬파일에 없어 생성 요청 ID : " + _strID );
		}
		else
		{
			_strID = sUserSetting.ID;
			Debug.Log( "로컬 파일 저장된 ID : " + _strID );
		}

		DoNetworkDB_GetOrInsert_Single<SInfoUser>( OnDBLoad_UserData, new StringPair( "ID", _sSetting_User.ID ) ); _iDBCount_Request++;
	}

	protected override void OnLoadFinish_INI_Developer( bool bSuccess )
	{
		base.OnLoadFinish_INI_Developer( bSuccess );

		//	if (Application.isEditor)
		//		_strID = _sSetting_Developer.strTestID;

		//	//DoNetworkDB_Insert( null, new SDataGame( _strID ) );
		//	//DoNetworkDB_Get_Single<SDataGame>( OnDBLoadFinihs );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcOnSceneLoaded(UnityEngine.SceneManagement.Scene pScene, UnityEngine.SceneManagement.LoadSceneMode eLoadSceneMode)
	{
		Debug.Log(pScene.name + " 씬 로딩완료.");
	}

	private void OnDBLoad_UserData( bool bResult, SInfoUser Data )
	{
		if(bResult)
		{
			p_pInfoUser = Data;
			DoNetworkDB_Get_Array<SDataGame>( OnDBLoad_DataGame ); _iDBCount_Request++;
			DoNetworkDB_Get_Array<SDataMission>( OnDBLoad_DataMission ); _iDBCount_Request++;
			DoNetworkDB_Get_Array<SDataMission_Character>( OnDBLoad_DataMission_Character ); _iDBCount_Request++;
			DoNetworkDB_Get_Array<SDataMission_Enemy>( OnDBLoad_DataMission_Enemy ); _iDBCount_Request++;
			DoNetworkDB_Get_Array<SDataMission_Bullet>( OnDBLoad_DataMission_Bullet ); _iDBCount_Request++;
		}
		else
		{
			if (_iTryCount_CreateID++ > const_iTryLimitCount)
			{
				Debug.LogWarning( "아이디를 " + _iTryCount_CreateID + " 번 요청했으나 실패" );
				return;
			}


			_sSetting_User.ID = SCManagerLogIn.CalculateHashPassword( const_iIDLength );
			_strID = _sSetting_User.ID;

			Debug.Log( "아이디 생성 요청" + _strID );
			p_pManagerJsonINI.DoWriteJson( EINI_JSON_FileName.UserSetting, _sSetting_User );
			DoNetworkDB_GetOrInsert_Single<SInfoUser>( OnDBLoad_UserData, new StringPair("ID", _sSetting_User.ID )); _iDBCount_Request++;
		}

		CheckIsFinishDBLoad();
		Debug.Log( "OnDBLoad_UserData - Count : " + Data );
	}

	private void OnDBLoad_DataGame( bool bResult, SDataGame[] arrData )
	{
		SDataGame.SetData( arrData );

		CheckIsFinishDBLoad();
		Debug.Log( "OnDBLoad_DataGame - Count : " + arrData.Length + " Progress : " + _iDBCount_Finish + " / " + _iDBCount_Request );
	}

	private void OnDBLoad_DataMission( bool bResult, SDataMission[] arrData )
	{
		SDataMission.SetData( arrData );

		CheckIsFinishDBLoad();
		Debug.Log( "OnDBLoad_DataMission - Count : " + arrData.Length + " Progress : " + _iDBCount_Finish + " / " + _iDBCount_Request );
	}

	private void OnDBLoad_DataMission_Character( bool bResult, SDataMission_Character[] arrData )
	{
		g_mapMissionCharacterInfo.DoAddItem( arrData );

		CheckIsFinishDBLoad();
		Debug.Log( "OnDBLoad_DataMission_Character - Count : " + arrData.Length + " Progress : " + _iDBCount_Finish + " / " + _iDBCount_Request );
	}

	private void OnDBLoad_DataMission_Enemy( bool bResult, SDataMission_Enemy[] arrData )
	{
		g_mapEnemy.DoAddItem( arrData );
		CManagerRandomTable<SDataMission_Enemy>.instance.DoAddRandomItem_Range( g_mapEnemy.Values.ToList() );

		CheckIsFinishDBLoad();
		Debug.Log( "OnDBLoad_DataMission_Enemy - Count : " + arrData.Length + " Progress : " + _iDBCount_Finish + " / " + _iDBCount_Request );
	}

	private void OnDBLoad_DataMission_Bullet( bool bResult, SDataMission_Bullet[] arrData )
	{
		g_mapBullet.DoAddItem( arrData );

		CheckIsFinishDBLoad();
		Debug.Log( "OnDBLoad_DataMission_Bullet - Count : " + arrData.Length + " Progress : " + _iDBCount_Finish + " / " + _iDBCount_Request );
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private void CheckIsFinishDBLoad()
	{
		if (++_iDBCount_Finish == _iDBCount_Request)
		{
			bool bIsFirstLogIn = p_pInfoUser.iLogInCount == 0;
			if (bIsFirstLogIn == false)
				DoNetworkDB_UpdateAdd<SInfoUser>( "iLogInCount", 1, null );

			_pDataGame.bTutorial = bIsFirstLogIn;

			if (p_EVENT_OnDBLoadFinish != null)
				p_EVENT_OnDBLoadFinish();
		}
	}

	private void ProcResource()
	{
		ECharacterName.reina.AddEnumGroup( ECharacterAnimationName.angry );
		ECharacterName.reina.AddEnumGroup( ECharacterAnimationName.fighting );
		ECharacterName.reina.AddEnumGroup( ECharacterAnimationName.humming );
		ECharacterName.reina.AddEnumGroup( ECharacterAnimationName.happy );
		ECharacterName.reina.AddEnumGroup( ECharacterAnimationName.tear );

		ECharacterName.milia.AddEnumGroup( ECharacterAnimationName.angry );
		ECharacterName.milia.AddEnumGroup( ECharacterAnimationName.fright );
		ECharacterName.milia.AddEnumGroup( ECharacterAnimationName.sorry );
		ECharacterName.milia.AddEnumGroup( ECharacterAnimationName.happy );

		ECharacterName.kkyung.AddEnumGroup( ECharacterAnimationName.angry );
		ECharacterName.kkyung.AddEnumGroup( ECharacterAnimationName.tear );
		ECharacterName.kkyung.AddEnumGroup( ECharacterAnimationName.happy );

		ECharacterName.zion.AddEnumGroup( ECharacterAnimationName.angry );
		ECharacterName.zion.AddEnumGroup( ECharacterAnimationName.silence );
		ECharacterName.zion.AddEnumGroup( ECharacterAnimationName.smile );
		ECharacterName.zion.AddEnumGroup( ECharacterAnimationName.smile2 );
		ECharacterName.zion.AddEnumGroup( ECharacterAnimationName.surprise );

		ECharacterName.Yuna.AddEnumGroup( ECharacterAnimationName.angry );
		ECharacterName.Yuna.AddEnumGroup( ECharacterAnimationName.shame );
		ECharacterName.Yuna.AddEnumGroup( ECharacterAnimationName.sulk );

		ESoundGroup_BGM.Mission.AddEnumGroup( ESoundName.BGM_idle1 );
		ESoundGroup_BGM.Mission.AddEnumGroup( ESoundName.BGM_idle2 );
		ESoundGroup_BGM.Mission.AddEnumGroup( ESoundName.BGM_idle3 );
		ESoundGroup_BGM.Mission.AddEnumGroup( ESoundName.BGM_idle4 );
		ESoundGroup_BGM.Mission.AddEnumGroup( ESoundName.BGM_idle5 );
		ESoundGroup_BGM.Mission.AddEnumGroup( ESoundName.BGM_idle6 );

		ESoundGroup_BGM.MissionBoss.AddEnumGroup( ESoundName.BGM_boss1 );
		ESoundGroup_BGM.MissionBoss.AddEnumGroup( ESoundName.BGM_boss2 );

		ESoundGroup_BGM.OutGame.AddEnumGroup( ESoundName.BGM_Title_LenskoCetus );
	}

}
