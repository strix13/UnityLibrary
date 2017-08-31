using UnityEngine;
using System.Collections;

public class CUIPopupYESNOBase : CUIPopupBase
{
    public event EventDelegate.Callback p_EVENT_OnYES;
    public event EventDelegate.Callback p_EVENT_OnNo;

    // ========================== [ Division ] ========================== //    

    virtual protected void OnYes() {  }
    virtual protected void OnNo() {  }

    public void OnBtn_YES()
    {
        if(p_EVENT_OnYES != null)
            p_EVENT_OnYES();
        OnYes();
        DoHide();
    }

    public void OnBtn_No()
    {
        if(p_EVENT_OnNo != null)
            p_EVENT_OnNo();
        OnNo();
        DoHide();
    }

    // ========================== [ Division ] ========================== //

    protected override void OnHide()
    {
        base.OnHide();

        p_EVENT_OnYES = null;    p_EVENT_OnNo = null;
    }
}
