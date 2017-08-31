using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Lean.Touch;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class PCUIOutFrame_SelectCharacter : CUIFrameBase, IButton_OnClickListener<PCUIOutFrame_SelectCharacter.EUIButton>
{
	/* const & readonly declaration             */

	const int const_iCharacterCount = 4;
	const int const_iCharacterScrollSpeed = 10;

	readonly Vector3[] const_arrLookatPos = new Vector3[]
	{
		new Vector3( 0, 0, 1 ),     new Vector3( 1.602f, 0, 0.927f ), new Vector3( 1.602f, 0, -0.94f ),
		new Vector3( 0, 0, -0.94f ),new Vector3( -1.64f, 0, -0.94f ), new Vector3( -1.64f, 0, 0.932f )
	};

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_Select_Reina,
		Button_Select_Millia,
		Button_Select_Yuna,
		Button_Select_Zion,
		Button_Select_KKyung,

		Button_Left,
		Button_Right,

		Button_Team,
		Button_CharacterUpgrade
	}

	/* public - Variable declaration            */

	public List<EventDelegate> p_EVENT_OnSelectCharacter = new List<EventDelegate>();

	public GameObject g_CharacterParent;//-----------캐릭터 일러스트 부모오브젝트
	public GameObject g_LookAt;//--------------------타겟 오브젝트 유아이 최상위 오브젝트가 쳐다봄

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private List<PCCharacterAnimation> _listCharacters = new List<PCCharacterAnimation>();

	private ECharacterName _eCharacterSelected = ECharacterName.None;
	private int _iUIPosition = 0;

	private GameObject _pObjUI_Sprite_Selected;
	private Transform _pTransUI_Sprite_Selected;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	private void DoSelectCharacter(EUIButton eButtonName)
	{
		ECharacterName eCharacterNew = (ECharacterName)((int)eButtonName);
		if (_eCharacterSelected == eCharacterNew)
		{
			PCManagerFramework.p_pInfoUser.eCharacterCurrent = eCharacterNew;
			PCManagerFramework.DoNetworkDB_Update_Set<SInfoUser>( "eCharacterCurrent", (int)eCharacterNew, null );

			if (PCManagerFramework.p_pDataGame.bTutorial == false)
				PCManagerUIOutGame.instance.DoChangeFrame_FadeInout(PCManagerUIOutGame.EUIFrame.PCUIOutFrame_SelectCharacter, PCManagerUIOutGame.EUIFrame.PCUIOutFrame_MainMenu, 1f);

			EventDelegate.Execute(p_EVENT_OnSelectCharacter);
		}
		else
		{
			_eCharacterSelected = (ECharacterName)((int)eButtonName);
			_iUIPosition = (int)_eCharacterSelected;
			g_LookAt.transform.position = const_arrLookatPos[_iUIPosition];

			Transform pTransParent = GetGameObject(eButtonName.ToString_GarbageSafe()).transform;
			ProcCharacterSelected(pTransParent);
		}
	}

	public void IOnClick_Buttons( EUIButton eButtonName )
	{
		switch (eButtonName)
		{
			case EUIButton.Button_Left: LeftButton(); return;
			case EUIButton.Button_Right: RightButton(); return;
			case EUIButton.Button_Team: Debug.Log( "Button_Team" ); return;
			case EUIButton.Button_CharacterUpgrade: Debug.Log( "Button_CharacterUpgrade" ); return;
		}

		DoSelectCharacter(eButtonName);
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

		GetComponentsInChildren<PCCharacterAnimation>( _listCharacters );
		_listCharacters.Sort();

		_pObjUI_Sprite_Selected = GetGameObject("Sprite_Selected");
		_pObjUI_Sprite_Selected.SetActive(false);

		_pTransUI_Sprite_Selected = _pObjUI_Sprite_Selected.GetComponent<Transform>();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		g_CharacterParent.transform.LookAt( g_LookAt.transform.position );

		ProcCharacter_ShowOnlyOne( _iUIPosition );
		g_LookAt.transform.position = Vector3.MoveTowards( g_LookAt.transform.position, const_arrLookatPos[_iUIPosition], const_iCharacterScrollSpeed * Time.deltaTime );
	}

	protected override void OnShow( int iSortOrder )
	{
		base.OnShow( iSortOrder );

		LeanTouch.OnFingerSwipe += OnFingerSwipe;

		PCUIOutFrame_MainTop pUITop = PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainTop>();
		pUITop.DoSetIsShowMainMenu( false );
	}

	protected override void OnHide()
	{
		base.OnHide();

		LeanTouch.OnFingerSwipe -= OnFingerSwipe;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void OnFingerSwipe( LeanFinger finger )
	{
		var swipe = finger.SwipeScreenDelta;

		if (swipe.x < -Mathf.Abs( swipe.y ))//왼쪽
			RightButton();

		if (swipe.x > Mathf.Abs( swipe.y ))//오른쪽
			LeftButton();
	}

	private void LeftButton()
	{
		if (_iUIPosition != 0)
			_iUIPosition--;
		else
			_iUIPosition = const_iCharacterCount;
	}

	private void RightButton()
	{
		if (_iUIPosition != const_iCharacterCount)
			_iUIPosition++;
		else
			_iUIPosition = 0;
	}

	private void ProcCharacter_ShowOnlyOne( int iShowIndex )
	{
		_listCharacters[iShowIndex].gameObject.SetActive( true );
		for (int i = 0; i < _listCharacters.Count; i++)
		{
			if (i != iShowIndex)
				_listCharacters[i].gameObject.SetActive( false );
		}
	}

	private void ProcCharacterSelected(Transform pTransParent)
	{
		_pObjUI_Sprite_Selected.SetActive(false);
		_pObjUI_Sprite_Selected.SetActive(true);

		_pTransUI_Sprite_Selected.parent = pTransParent;
		_pTransUI_Sprite_Selected.localPosition = Vector3.zero;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
