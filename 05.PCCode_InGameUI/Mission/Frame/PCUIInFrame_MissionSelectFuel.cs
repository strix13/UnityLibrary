using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Edit Log    : 
   ============================================ */

public class PCUIGameFrame_MissionSelectFuel : CUIFrameBase, IButton_OnClickListener<PCUIGameFrame_MissionSelectFuel.EUIButton>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_fuel_1,
		Button_fuel_2,
		Button_fuel_3,
		Button_fuel_4,
		Button_fuel_5,

		Button_fuel_Start,
	}

	public class SFuelButton
	{
		public UILabel pLabel_AddFuel;
		public TweenAlpha pTweenAlpha;
		public int iRandomFuel;

		public void DoInitRandomFuel( int iFuel)
		{
			iRandomFuel = iFuel;
			pLabel_AddFuel.text = string.Format( "+ {0} 연료", iRandomFuel );
		}

		public void DoStartTween()
		{
			pTweenAlpha.PlayForward();
		}
	}

	/* public - Variable declaration            */

	[SerializeField]
	private GameObject _pObjectFuel = null;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<UIButton, SFuelButton> _mapFuelButton = new Dictionary<UIButton, SFuelButton>();
	private List<UIButton> _listAddFuelButton = new List<UIButton>();
	private HashSet<int> _setRandomFuel = new HashSet<int>();

	// ========================================================================== //

	/* public - [Do] Function
	 * 외부 객체가 호출                         */

	public void IOnClick_Buttons( EUIButton eButtonName )
	{
		switch (eButtonName)
		{
			case EUIButton.Button_fuel_1:
			case EUIButton.Button_fuel_2:
			case EUIButton.Button_fuel_3:
			case EUIButton.Button_fuel_4:
			case EUIButton.Button_fuel_5:

				StartCoroutine( CoAddFuelShow((int)eButtonName ) );
				break;


			case EUIButton.Button_fuel_Start:
				break;
		}

		Debug.Log( eButtonName );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pObjectFuel.GetComponentsInChildren<UIButton>( _listAddFuelButton );
		_listAddFuelButton.Sort_ObjectSibilingIndex();

		for (int i = 0; i < _listAddFuelButton.Count; i++)
		{
			SFuelButton sFuelButton = new SFuelButton();
			sFuelButton.pTweenAlpha = _listAddFuelButton[i].GetComponentInChildren<TweenAlpha>();
			sFuelButton.pLabel_AddFuel = _listAddFuelButton[i].GetComponentInChildren<UILabel>();
			_mapFuelButton.Add( _listAddFuelButton[i], sFuelButton );
		}
	}

	protected override void OnShow( int iSortOrder )
	{
		base.OnShow( iSortOrder );

		_setRandomFuel.Clear();
		int iRandomFuel_Min = SDataGame.GetInt( EDataGameField.iMissionFuel_Min );
		int iRandomFuel_Max = SDataGame.GetInt( EDataGameField.iMissionFuel_Max );

		if(iRandomFuel_Min == -1 || iRandomFuel_Max == -1)
		{
			Debug.LogWarning( "RandomFuelMin : " + iRandomFuel_Min + "iRandomFuel_Max : " + iRandomFuel_Max );
			return;
		}
		iRandomFuel_Max += 1;

		for (int i = 0; i < _listAddFuelButton.Count; i++)
		{
			int iRandomFuel;
			while (true)
			{
				iRandomFuel = Random.Range( iRandomFuel_Min, iRandomFuel_Max );
				if (_setRandomFuel.Contains( iRandomFuel ) == false)
				{
					_setRandomFuel.Add( iRandomFuel );

					SFuelButton sFuelButton = _mapFuelButton[_listAddFuelButton[i]];
					sFuelButton.DoInitRandomFuel( iRandomFuel );
					break;
				}
			}
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private IEnumerator CoAddFuelShow(int iIndex)
	{
		_mapFuelButton[_listAddFuelButton[iIndex]].DoStartTween();
		//PCManagerInMission.instance.DoAddFuel( _mapFuelButton[_listAddFuelButton[iIndex]].iRandomFuel );

		yield return new WaitForSeconds( 1.5f );

		for (int i = 0; i < _listAddFuelButton.Count; i++)
			_mapFuelButton[_listAddFuelButton[i]].DoStartTween();

		yield return new WaitForSeconds( 1.5f );

		//PCUIGameFrame_MissionGame pUIGame = PCManagerGameMission.instance.GetUIFrame<PCUIGameFrame_MissionGame>();
		//pUIGame.DoSetPlayer( PCManagerFramework.p_pInfoUser.eCharacterCurrent );
		//DoHide();
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
