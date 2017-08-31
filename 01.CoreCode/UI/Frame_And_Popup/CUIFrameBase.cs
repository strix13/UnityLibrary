using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIPanel))]
abstract public class CUIFrameBase : CUIObjectBase
{
	[SerializeField]
	private List<UIPanel> _listPanelFixedDepth = new List<UIPanel>();
    [SerializeField]
	private bool _bAlwaysShow = false; public bool p_bAlwaysShow {  get { return _bAlwaysShow; } }
    [SerializeField]
	private bool _bFixedSortOrder = false; public bool p_bFixedSortOrder {  get { return _bFixedSortOrder; } }

	protected UIPanel _pUIPanel;

	private List<UIPanel> _listPanel = new List<UIPanel>();

    private bool _bShowCurrent;         public bool p_bShowCurrent { get { return _bShowCurrent; } }
    private bool _bInit = false;

	public List<EventDelegate> p_EVENT_OnShow = new List<EventDelegate>();

	// ========================== [ Division ] ========================== //

	virtual protected void OnShow(int iSortOrder) { }
    virtual protected void OnHide() { }

    // ========================== [ Division ] ========================== //

	public void DoStartFadeInOutPanel_Delayed(bool bShow, float fDelay = 0, float fFadeTime = 1f)
	{
		_pUIPanel.alpha = bShow ? 0 : 1;

		if (bShow) DoShow();

		StartCoroutine(CoProcFadeInOutPanel(bShow, fDelay, fFadeTime));
	}

	private IEnumerator CoProcFadeInOutPanel(bool bShow, float fDelay, float fFadeTime)
	{
		yield return new WaitForSeconds(fDelay);

		int iFadeLimit = bShow ? 1 : 0;
		int iFadeDir = bShow ? 1 : -1;

		bool bLoop = true;
		while (bLoop)
		{
			_pUIPanel.alpha += iFadeDir * Time.unscaledDeltaTime * fFadeTime;

			float fAlpha = _pUIPanel.alpha;
			if (bShow && fAlpha >= iFadeLimit)
				bLoop = false;
			else if (bShow == false && fAlpha <= iFadeLimit)
				bLoop = false;

			yield return null;
		}

		if (bShow == false) DoHide();
	}

	public void DoShowHide(bool bShow)
	{
		if (bShow)
			DoShow();
		else
			DoHide();
	}

    public void DoShow()
    {
        _bShowCurrent = true;
        _pGameObjectCached.SetActive(true);

        OnShow(_pUIPanel.depth);

		if (_pUIPanel.alpha < 1)
			_pUIPanel.alpha = 1;

		EventDelegate.Execute(p_EVENT_OnShow);
	}

    public void EventShow(int iSortOrder)
    {
		EventUIFrame_SetOrder( iSortOrder );

		DoShow();
	}

	public void DoHide()
    {
        _bShowCurrent = false;
        _pGameObjectCached.SetActive(false);

        OnHide();
    }

	public void EventUIFrame_SetOrder(int iSortOrder)
	{
		if (_bFixedSortOrder == false)
		{
			_pUIPanel.depth = iSortOrder;
			_pUIPanel.sortingOrder = Mathf.FloorToInt(iSortOrder * 0.1f);

			int iChildPanelDepth = iSortOrder + 1;
			for (int i = 0; i < _listPanel.Count; i++)
			{
				if(_listPanelFixedDepth.Contains(_listPanel[i]) == false)
					_listPanel[i].depth = iChildPanelDepth;
			}
		}
	}

    public void EventUIFrame_OnAwake() { if(_bInit == false) OnAwake(); }
    
	public void EventRegist_UIPanel(UIPanel pUIPanel)
	{
		_listPanel.Add( pUIPanel );
	}

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

		_pUIPanel = GetComponent<UIPanel>();
		if (_pUIPanel == null)
			_pUIPanel = p_pGameObjectCached.AddComponent<UIPanel>();

		GetComponentsInChildren<UIPanel>( _listPanel );
		_listPanel.Remove( _pUIPanel );

		_bInit = true;

		if (_bAlwaysShow)
			DoShow();
    }
}
