using UnityEngine;
using System.Collections;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-04 오후 5:54:23
   Description : 
   Edit Log    : 
   ============================================ */

public class CSingletonBase_Not_UnityComponent<CLASS_SingletoneTarget>
    where CLASS_SingletoneTarget : CSingletonBase_Not_UnityComponent<CLASS_SingletoneTarget>, new()
{
    static private CLASS_SingletoneTarget _instance;

	// ========================== [ Division ] ========================== //

	static public CLASS_SingletoneTarget instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CLASS_SingletoneTarget();
				_instance.OnMakeSingleton();
			}

			return _instance;
		}
	}

	static public void DoReleaseSingleton()
	{
		_instance.OnReleaseSingleton();
		_instance = null;
	}

	// ========================== [ Division ] ========================== //

	virtual protected void OnMakeSingleton() { }
	virtual protected void OnReleaseSingleton() { }
}