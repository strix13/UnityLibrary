#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	작성자 : Strix
 *	
 *	기능 : https://blogs.unity3d.com/kr/2015/12/23/1k-update-calls/
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class CObjectBase : MonoBehaviour
{
	protected bool _bIsExcuteAwake = false;
    private Coroutine _pCoroutineOnEnable;

	public Vector3 p_vecPos
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	// ========================== [ Division ] ========================== //

	void Awake()
    {
        if(_bIsExcuteAwake == false)
            OnAwake();
    }

    void OnEnable()
    {
        CManagerUpdateObject.instance.DoAddObject(this);

        OnEnableObject();
        if (_pCoroutineOnEnable != null)
            StopCoroutine(_pCoroutineOnEnable);
        _pCoroutineOnEnable = StartCoroutine(OnEnableObjectCoroutine());
    }

    void OnDisable()
    {
        CManagerUpdateObject.instance.DoRemoveObject(this);
        OnDisableObject();
    }

    void Start() { OnStart(); }

    // ========================== [ Division ] ========================== //

    virtual protected void OnAwake()
    {
		if (_bIsExcuteAwake == false)
        {
            SCManagerGetComponent.DoUpdateGetComponentAttribute(this);
			_bIsExcuteAwake = true;
        }
	}

	virtual protected void OnStart() { }
    virtual protected void OnEnableObject() {}
    virtual protected IEnumerator OnEnableObjectCoroutine() { yield break; }
    virtual public void OnUpdate() { }
    virtual protected void OnDisableObject() { }
	
	// ========================== [ Division ] ========================== //
	
	public void EventOnAwake()
	{
		if (_bIsExcuteAwake == false)
			OnAwake();
	}

	Dictionary<System.Action, Coroutine> _mapCoroutinePlaying = new Dictionary<System.Action, Coroutine>();
	protected void EventDelayExcuteCallBack(System.Action OnAfterDelayAction, float fDelaySec)
	{
        if (this != null && gameObject.activeInHierarchy)
        {
            if (_mapCoroutinePlaying.ContainsKey(OnAfterDelayAction))
                StopCoroutine(_mapCoroutinePlaying[OnAfterDelayAction]);

            Coroutine pCoroutine = StartCoroutine(CoDelayAction(OnAfterDelayAction, fDelaySec));
            if (_mapCoroutinePlaying.ContainsKey(OnAfterDelayAction) == false)
                _mapCoroutinePlaying.Add(OnAfterDelayAction, pCoroutine);
        }

    }

	protected IEnumerator CoDelayAction( System.Action OnAfterDelayAction, float fDelaySec )
	{
		yield return SCManagerYield.GetWaitForSecond( fDelaySec );

		OnAfterDelayAction();
		_mapCoroutinePlaying.Remove( OnAfterDelayAction );
	}
}
