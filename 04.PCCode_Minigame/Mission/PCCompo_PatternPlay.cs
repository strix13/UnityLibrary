using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCCompo_PatternPlay : CObjectBase, IDictionaryItem<string>, IDictionaryItem<int>, IDictionaryItem<EMissionEnemy>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public EMissionEnemy eMissionEnemy;
	public int iSpawnRuna = 0;
	public bool bIsBoss = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private List<PCMission_PatternBullet> _listPatternBullet = new List<PCMission_PatternBullet>();
	private List<PCMission_PatternEnemy> _listPatternEnemy = new List<PCMission_PatternEnemy>();

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public int DoPlayPatternEnemy_RandomOne()
	{
		if (_listPatternEnemy.Count == 0)
			Debug.LogWarning( name + "_listPatternEnemy .Count == 0", this );

		int iRandomIndex = Random.Range( 0, _listPatternEnemy.Count );
		return _listPatternEnemy[iRandomIndex].DoPlayPattern();
	}

	public bool DoPlayPattern()
	{
		gameObject.SetActive( true );

		for (int i = 0; i < _listPatternBullet.Count; i++)
			_listPatternBullet[i].DoPlayPattern();

		for (int i = 0; i < _listPatternEnemy.Count; i++)
			_listPatternEnemy[i].DoPlayPattern();

		return bIsBoss;
	}

	public void DoPlayPattern( System.Action OnFinishPattern )
	{
		gameObject.SetActive( true );

		for (int i = 0; i < _listPatternBullet.Count; i++)
			_listPatternBullet[i].DoPlayPattern( OnFinishPattern );

		for (int i = 0; i < _listPatternEnemy.Count; i++)
			_listPatternEnemy[i].DoPlayPattern( OnFinishPattern );
	}
	
	public void DoStopPattern( bool bGameObjectActiveOff = false )
	{
		if (bGameObjectActiveOff)
			gameObject.SetActive( false );

		for (int i = 0; i < _listPatternBullet.Count; i++)
			_listPatternBullet[i].DoStopPattern( bGameObjectActiveOff );

		for (int i = 0; i < _listPatternEnemy.Count; i++)
			_listPatternEnemy[i].DoStopPattern( bGameObjectActiveOff );
	}

	public void DoSetGenrating( bool bGenerating )
	{
		gameObject.SetActive( true );

		for (int i = 0; i < _listPatternBullet.Count; i++)
			_listPatternBullet[i].DoSetGenrating( bGenerating );

		for (int i = 0; i < _listPatternEnemy.Count; i++)
			_listPatternEnemy[i].DoSetGenrating( bGenerating );

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

		GetComponentsInChildren( true, _listPatternBullet );
		GetComponentsInChildren( true, _listPatternEnemy );
	}

	public string IDictionaryItem_GetKey()
	{
		return name;
	}

	int IDictionaryItem<int>.IDictionaryItem_GetKey()
	{
		return iSpawnRuna;
	}

	EMissionEnemy IDictionaryItem<EMissionEnemy>.IDictionaryItem_GetKey()
	{
		return eMissionEnemy;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
