#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;

public class CSingletonDynamicBase<CLASS_SingletoneTarget> : CObjectBase
    where CLASS_SingletoneTarget : CSingletonDynamicBase<CLASS_SingletoneTarget>
{
    static public CLASS_SingletoneTarget instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CLASS_SingletoneTarget>();
				if(_instance == null)
				{
					GameObject pObjectDynamicGenerate = new GameObject( typeof( CLASS_SingletoneTarget ).Name );
					_instance = pObjectDynamicGenerate.AddComponent<CLASS_SingletoneTarget>();
				}

				if (_instance._bIsExcuteAwake == false)
                    _instance.OnAwake();
            }

            return _instance;
        }
    }

	static protected GameObject _pObjectManager; public GameObject p_pObjectManager { get { return _pObjectManager; } }
	static protected Transform _pTransManager;

	static private CLASS_SingletoneTarget _instance;
    static private bool _bIsQuitApplication = false;

	// ========================== [ Division ] ========================== //

	static public void DoReleaseSingleton()
	{
		if (_instance != null)
		{
			_instance.OnReleaseSingleton();
			_instance = null;
		}
	}
	
	static public void DoSetParents_ManagerObject( Transform pTransformParents )
	{
		//CManagerPooling<ENUM_Resource_Name, Class_Resource> pManagerCurrent = instance;
		//if (_pTransManager == null)
		//{
		//	_bIsInit = false; _bIsDestroy = false;
		//	pManagerCurrent.OnMakeSingleton();
		//}

		_pTransManager.SetParent( pTransformParents );
		_pTransManager.DoResetTransform();
	}

	// ========================== [ Division ] ========================== //

	virtual protected void OnReleaseSingleton() { }

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
        _instance = null;
        _bIsExcuteAwake = false;
    }

    private void OnApplicationQuit()
    {
        _bIsQuitApplication = true;
    }

    // ========================== [ Division ] ========================== //

    static public CLASS_SingletoneTarget EventMakeSingleton()
    {
        if (_bIsQuitApplication) return null;
        if (_instance != null) return instance;

        GameObject pObjectNewManager = new GameObject(typeof(CLASS_SingletoneTarget).ToString());
        return pObjectNewManager.AddComponent<CLASS_SingletoneTarget>();
    }
}
