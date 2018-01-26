using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-04-05 오후 1:24:58
   Description : 
   Edit Log    : 
   ============================================ */

public class CUICompoLocalize : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private System.Action<string> _EVENT_OnChangeText;

	private UnityEngine.UI.Text _pText_UGUI;
#if NGUI
	private UILabel _pText_NGUI;
#endif
#if TMPro
	private TMPro.TextMeshPro _pText_TMPro;
#endif

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoChangeText(string strText)
	{
		if(_EVENT_OnChangeText == null)
		{
			Debug.LogWarning( name + " _EVENT_OnChangeText == null", this );
			return;
		}
		_EVENT_OnChangeText(strText);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventChangeText_UGUI(string strText)
	{
		_pText_UGUI.text = strText;
	}

	private void EventChangeText_NGUI(string strText)
	{
#if NGUI
		_pText_NGUI.text = strText;
#endif
	}

	private void EventChangeText_TMPro(string strText)
	{
#if TMPro
		_pText_TMPro.text = strText;
#endif
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
    {
        base.OnAwake();

		_pText_UGUI = GetComponent<UnityEngine.UI.Text>();
		if (_pText_UGUI != null)
			_EVENT_OnChangeText = EventChangeText_UGUI;
#if NGUI
		_pText_NGUI = GetComponent<UILabel>();
		if (_pText_NGUI != null)
			_EVENT_OnChangeText = EventChangeText_NGUI;
#endif
#if TMPro
		_pText_TMPro = GetComponent<TMPro.TextMeshPro>();
		if (_pText_TMPro != null)
			_EVENT_OnChangeText = EventChangeText_TMPro;
#endif
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
