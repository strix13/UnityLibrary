using UnityEngine;
using System.Collections;
using System;

public enum ECharacterName : short
{
	None = - 1,
	reina,
	milia,
	Yuna,
	zion,
	kkyung,
	guide
}

public enum EMissionCategory : short
{
	reina,
	milia,
	Yuna,
	zion,
	kkyung,


}

public enum EMissionObject
{
	SpaceStation,
}

static public class PCEnumHelper
{
	static public EMissionCategory ConvertCharacterName( this ECharacterName eMission )
	{
		return (EMissionCategory)((int)eMission);
	}

	static public ECharacterName ConvertMissionToCharacterName(this EMissionCategory eMission)
	{
		return (ECharacterName)((int)eMission);
	}
}

public enum ECharacterAnimationName
{
	idle,
	angry,
	fighting,
	humming,
	happy,
	tear,

	fright,
	sorry,

	silence,
	smile,
	smile2,

	surprise,

	shame,
	sulk,
}

public enum ELevel
{
	Easy,
	Normal,
	Hard
}

public enum EDataGameField
{
	iMissionFuel_Min,
	iMissionFuel_Max,

	iLevelRegenDistance,
	fDifficultyAdjusted,
	fMissionFuelDecreaseSpeed,
	fMissionFuelOnStart,
}

[System.Serializable]
public class SDataMission : SDataBase<EMissionCategory>
{
	public EMissionCategory eCategory;

	public override EMissionCategory IDictionaryItem_GetKey()
	{
		return eCategory;
	}
}

[System.Serializable]
public class SDataMission_Character : IDictionaryItem<ECharacterName>
{
	public ECharacterName eCharcaterID;

	public int iBulletDamage;
	public float fBulletDelay;
	public float fBulletSpeed;


	public ECharacterName IDictionaryItem_GetKey()
	{
		return eCharcaterID;
	}
}

[System.Serializable]
public class SDataMission_Enemy : IDictionaryItem<EMissionEnemy>, IRandomItem
{
	public string strEnemyName;
	public int iHP;
	public float fSpeed;
	public int iScore;

	public int iGoldMin;
	public int iGoldMax;

	public float fFeverOnHit;
	public float fFeverOnDead;

	public float fPercent_Roulette;
	public float fSoundVolume;


	private bool _bEnumInit = false;
	private EMissionEnemy _eMissionEnemy;

	public EMissionEnemy IDictionaryItem_GetKey()
	{
		if(_bEnumInit == false)
		{
			_eMissionEnemy = strEnemyName.ConvertEnum<EMissionEnemy>();
			_bEnumInit = true;
		}

		return _eMissionEnemy;
	}

	public int IRandomItem_GetPercent()
	{
		return iScore;
	}
}

[System.Serializable]
public class SDataMission_Bullet : IDictionaryItem<EMissionBullet>
{
	public string strBulletName;
	public int iDamage;
	public float fSpeed;

	public EMissionBullet IDictionaryItem_GetKey()
	{
		return strBulletName.ConvertEnum<EMissionBullet>();
	}
}


[System.Serializable]
public class SGameData_Legacy : IDB_Insert
{
	public string ID;

	//------------------------------------------------------------캐릭터 생성씬 변수
	public ECharacterName eCharacterCurrent;
	//------------------------------------------------------------미션모드 변수
	public int iGold;
	public int iTicket;
	public int iExe;
	public int iStage;
	public int iSave_Stage;
	public int iOpenStage;
	public int iFuelNum;
	public int iPowerLevel;
	public ELevel eLevel;
	public bool bTutorial = true;

	public SGameData_Legacy( )
	{
	}

	public SGameData_Legacy(string strID)
	{
		ID = strID;
	}

	public StringPair[] IDB_Insert_GetField()
	{
		StringPair[] arrStrPair = new StringPair[1];
		arrStrPair[0] = new StringPair( "ID", "Test" );

		return arrStrPair;
	}
}

[System.Serializable]
public class SGameData : IDB_Insert
{
	public string ID;

	public ECharacterName eCharacterCurrent;
	public int iGold;
	public int iTicket;

	public SGameData()
	{
	}

	public SGameData( string strID )
	{
		ID = strID;
	}

	public StringPair[] IDB_Insert_GetField()
	{
		StringPair[] arrStrPair = new StringPair[1];
		arrStrPair[0] = new StringPair( "ID", "Test" );

		return arrStrPair;
	}
}

// =====================================================================

[System.Serializable]
public class SInfoUser
{
	public string ID;
	public ECharacterName eCharacterCurrent;
	public int iGold;
	public int iTicket;
	public int iLogInCount;
	public int iPlayCount;
}