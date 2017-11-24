using UnityEngine;
using System.Collections;

public class CSingletonBase<CLASS_SingletoneTarget> : CObjectBase
    where CLASS_SingletoneTarget : CSingletonBase<CLASS_SingletoneTarget>
{
    static public CLASS_SingletoneTarget instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CLASS_SingletoneTarget>();
                if (_instance != null && _instance._bIsExcuteAwake == false)
                    _instance.OnAwake();
            }

            return _instance;
        }
    }

    static private CLASS_SingletoneTarget _instance;
	static private bool _bIsQuitApplication = false;

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        if (_bIsExcuteAwake == false)
        {
            if (_instance == null)
                _instance = FindObjectOfType<CLASS_SingletoneTarget>();
        }

		base.OnAwake();
	}

	void OnDestroy()
    {
        _bIsExcuteAwake = false;
    }

	private void OnApplicationQuit()
	{
		_bIsQuitApplication = true;
	}

	// ========================== [ Division ] ========================== //

	static protected void EventMakeSingleton()
    {
		if (_bIsQuitApplication) return;
		if (_instance != null) return;

        GameObject pObjectNewManager = new GameObject(typeof(CLASS_SingletoneTarget).ToString());
        pObjectNewManager.AddComponent<CLASS_SingletoneTarget>();
    }
}
