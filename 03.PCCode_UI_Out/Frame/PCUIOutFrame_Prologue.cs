using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Version	   :
   ============================================ */

public class PCUIOutFrame_Prologue : CUIFrameBase, IButton_OnClickListener<PCUIOutFrame_Prologue.EUIButton>
{
	/* const & readonly declaration             */

	private const float const_fScrollSpeed = 0.15f;
	private const float const_fScrollLimit = 1600f;

	/* enum & struct declaration                */

	public enum EUIButton
	{
		Button_Prologue,
		Button_SkipPrologue
	}

	private enum EUISprite
	{
		Sprite_Prologue,

	}

	private enum EPhasePrologue
	{
		None,
		Update,
		Dragging,
	}

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Transform _pTransUI_Prologue;
	private UIButton _pUIButton_Prologue;
	private Coroutine _pCoProcUpdatePrologue;

	private EPhasePrologue _ePhasePrologue;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/
	
	public void IOnClick_Buttons(EUIButton eButton)
	{
		switch (eButton)
		{
			case EUIButton.Button_SkipPrologue:
				PCManagerFramework.DoLoadScene_FadeInOut(ESceneName.OutGame, 1f, Color.black);
				break;
		}
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void OnDrag_Button(GameObject pObj, Vector2 v2DirectionDelta)
	{
		_ePhasePrologue = EPhasePrologue.Dragging;

		v2DirectionDelta.x = 0;
		ProcUpdatePosition_Prologue(v2DirectionDelta, true);
	}

	private void OnDragOver_Button(GameObject pObj = null)
	{
		_ePhasePrologue = EPhasePrologue.Update;

		if (_pCoProcUpdatePrologue != null)
		{
			StopCoroutine(_pCoProcUpdatePrologue);
			_pCoProcUpdatePrologue = null;
		}

		_pCoProcUpdatePrologue = StartCoroutine(CoProcUpdatePrologue());
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pTransUI_Prologue = GetGameObject(EUISprite.Sprite_Prologue).transform;
		_pUIButton_Prologue = GetUIButton(EUIButton.Button_Prologue);

		GameObject pGameObject_Cached = _pUIButton_Prologue.gameObject;
		UIEventListener pUIEventListener = UIEventListener.Get(pGameObject_Cached);
		pUIEventListener.onDrag += OnDrag_Button;
		pUIEventListener.onDragOut += OnDragOver_Button;
	}

	protected override void OnShow(int iSortOrder)
	{
		base.OnShow(iSortOrder);

		OnDragOver_Button();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoProcUpdatePrologue()
	{
		while (_ePhasePrologue == EPhasePrologue.Update)
		{
			if (_pTransUI_Prologue.localPosition.y > const_fScrollLimit)
			{
				_ePhasePrologue = EPhasePrologue.None;
				IOnClick_Buttons(EUIButton.Button_SkipPrologue);
				break;
			}

			ProcUpdatePosition_Prologue(Vector3.up, false);
			yield return null;
		}
	}

	private void ProcUpdatePosition_Prologue(Vector3 v3Direction, bool bDrag)
	{
		if (bDrag)
		{
			float fScreenSize = Mathf.Max(Screen.width, Screen.height);
			Vector3 v3DeltaPercent = v3Direction / fScreenSize;

			_pTransUI_Prologue.position += v3DeltaPercent;
		}
		else
			_pTransUI_Prologue.position += v3Direction * Time.deltaTime * const_fScrollSpeed;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
