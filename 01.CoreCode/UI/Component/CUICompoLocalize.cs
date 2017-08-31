using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-04-05 오후 1:24:58
   Description : 
   Edit Log    : 
   ============================================ */

[ExecuteInEditMode]
public class CUICompoLocalize : CUIObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */
	public string p_strText { set { p_pUILabel.text = value; } }
    public UILabel p_pUILabel { get { return _pUILabel; } }

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	[SerializeField]
    private string _strLangKey; public string p_strLangKey { set { _strLangKey = value; } get { return _strLangKey; } }
    [SerializeField]
    private string _strPrintFormat = null;

    private UILabel _pUILabel;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoChangeLocaleLabel(params string[] arrParams)
    {
        if (_strPrintFormat != null)
            _pUILabel.text = string.Format(_strPrintFormat, arrParams);
        else
        {
            _pUILabel.text = "";
            for (int i = 0; i < arrParams.Length; i++)
                _pUILabel.text += arrParams[i];
        }
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    public void EventSetLocalePrintFormat(string strPrintFormat)
    {
        _strPrintFormat = strPrintFormat;
    }

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	private void Reset()
	{
		if (Application.isEditor)
			_strLangKey = name;
	}

	protected override void OnAwake()
    {
        base.OnAwake();

        _pUILabel = GetComponent<UILabel>();
	}

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
