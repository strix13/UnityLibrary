using UnityEngine;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-25 오전 12:03:32
   Description : 
   Edit Log    : 
   ============================================ */

public class CManagerPooling<ENUM_Resource_Name, Class_Resource> : CSingletonBase_Not_UnityComponent<CManagerPooling<ENUM_Resource_Name, Class_Resource>>
	where ENUM_Resource_Name : System.IComparable, System.IConvertible
	where Class_Resource : Component
{
	/* public - Field declaration            */

	public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnMakeResource;
	public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnPopResource;
	public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnPushResource;

	/* protected - Field declaration            */

	protected Dictionary<ENUM_Resource_Name, Class_Resource> _mapResourceOriginCopy = new Dictionary<ENUM_Resource_Name, Class_Resource>();

	protected GameObject _pObjectManager; public GameObject p_pObjectManager { get { return _pObjectManager; } }
	protected Transform _pTransManager;

	/* private - Field declaration           */

	static private bool _bIsInit = false;
	static private bool _bIsDestroy = false;

	private Dictionary<int, Class_Resource> _mapPoolingInstance = new Dictionary<int, Class_Resource>();
	private Dictionary<int, ENUM_Resource_Name> _mapPoolingResourceType = new Dictionary<int, ENUM_Resource_Name>();

	private Dictionary<ENUM_Resource_Name, Queue<Class_Resource>> _queuePoolingDisable = new Dictionary<ENUM_Resource_Name, Queue<Class_Resource>>();
	private Dictionary<ENUM_Resource_Name, int> _mapResourcePoolingCount = new Dictionary<ENUM_Resource_Name, int>();

	private int _iPopCount = 0;	public int p_iPopCount {  get { return _iPopCount; } }
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	static public void DoUnLockGenerate_ManagerObject()
	{
		_bIsDestroy = false;
	}

	static public void DoSetParents_ManagerObject(Transform pTransformParents)
	{
		CManagerPooling<ENUM_Resource_Name, Class_Resource> pManagerCurrent = instance;
		pManagerCurrent._pTransManager.SetParent( pTransformParents );
		pManagerCurrent._pTransManager.DoResetTransform();

		if(pManagerCurrent._queuePoolingDisable.Count != 0)
		{
			var listOrigin = pManagerCurrent._mapResourceOriginCopy.Values.ToList();
			for (int i = 0; i < listOrigin.Count; i++)
				listOrigin[i].transform.DoResetTransform();
		}
	}

	static public void DoInitPoolingObject(List<GameObject> listPoolingObject)
	{
		ProcInitManagerPooling( listPoolingObject );
	}

	static public void DoClearPoolingObject()
	{
		_bIsInit = false;

		if(instance._pObjectManager != null)
		{
			GameObject.DestroyObject( instance._pObjectManager );
			instance._pObjectManager = null;
		}

		instance._queuePoolingDisable.Clear();
		instance._mapResourcePoolingCount.Clear();
		instance._mapResourceOriginCopy.Clear();
	}

	/// <summary>
	/// 현재 사용하지 않는 오브젝트를 얻습니다. 주의) 사용후 반드시 Return 바람
	/// </summary>
	/// <param name="eResourceName">Enum형태의 리소스 이름</param>
	/// <returns></returns>
	public Class_Resource DoPop(ENUM_Resource_Name eResourceName, bool bGameObjectActive = true)
	{
		Class_Resource pFindResource = null;

		if (_queuePoolingDisable.ContainsKey(eResourceName) == false)
		{
			_mapResourcePoolingCount.Add(eResourceName, 0);
			_queuePoolingDisable.Add(eResourceName, new Queue<Class_Resource>());
		}

		if (_queuePoolingDisable[eResourceName].Count == 0)
			pFindResource = MakeResource(eResourceName);

		int iLoopCount = 0;
		while(pFindResource == null && iLoopCount++ < _queuePoolingDisable[eResourceName].Count)
		{
			pFindResource = _queuePoolingDisable[eResourceName].Dequeue();
		}

		if (pFindResource == null)
		{
			pFindResource = MakeResource( eResourceName );
			if(pFindResource == null)
			{
				Debug.Log( eResourceName + "가 Pop에 실패했습니다..", _pTransManager );
				return null;
			}
		}

		if (pFindResource.transform.parent != _pTransManager)
			pFindResource.transform.SetParent(_pTransManager);

		pFindResource.gameObject.SetActive(bGameObjectActive);
		_iPopCount++;

		if (p_EVENT_OnPopResource != null)
			p_EVENT_OnPopResource(eResourceName, pFindResource);

		return pFindResource;
	}

	/// <summary>
	/// 사용했던 오브젝트를 반환합니다. 자동으로 GameObject가 Disable 됩니다.
	/// </summary>
	/// <param name="pResource">사용한 리소스</param>
	public void DoPush(Class_Resource pResource)
	{
		ProcReturnResource(pResource);
	}

	/// <summary>
	/// Enum형태의 리소스 이름의 List에 있는 오브젝트만 풀링을 위해 오브젝트를 새로 생성합니다. 
	/// </summary>
	/// <param name="listRequestPooling">풀링할 Enum 형태의 리소스 리스트</param>
	public void DoStartPooling(List<ENUM_Resource_Name> listRequestPooling, int iPoolingCount)
	{
		for (int i = 0; i < listRequestPooling.Count; i++)
		{
			ENUM_Resource_Name eResourceName = listRequestPooling[i];
			if(_queuePoolingDisable.ContainsKey(eResourceName) == false)
			{
				_queuePoolingDisable.Add(eResourceName, new Queue<Class_Resource>());
				_mapResourcePoolingCount.Add(eResourceName, 0);
			}

			for (int j = 0; j < iPoolingCount; j++)
			{
				Class_Resource pResource = MakeResource(eResourceName);
				ProcReturnResource(pResource);
			}
		}
	}

	/// <summary>
	/// 사용한 모든 오브젝트를 강제로 리턴시킵니다.
	/// </summary>
	public void DoPushAll()
	{
		IEnumerator<KeyValuePair<int, Class_Resource>> pIter = _mapPoolingInstance.GetEnumerator();
		while(pIter.MoveNext())
		{
			ProcReturnResource(pIter.Current.Value);
		}
	}

	static public void EventUpdateTransform()
	{
		instance._pTransManager = instance._pObjectManager.transform;
	}

	// ========================================================================== //

	/* protected - Override & Unity API         */

	protected override void OnMakeSingleton()
	{
		base.OnMakeSingleton();

		if (_bIsInit || _bIsDestroy) return;

		// Enum 과 같은 이름의 폴더를 확인
		string strEnumName = typeof(ENUM_Resource_Name).Name;
		if (strEnumName[0] == 'E')
			strEnumName = strEnumName.Substring(1, strEnumName.Length - 1);

		GameObject[] arrResources = Resources.LoadAll<GameObject>(string.Format("{0}/", strEnumName));
		ProcInitManagerPooling( arrResources.ToList() );
	}

	protected override void OnReleaseSingleton()
	{
		base.OnReleaseSingleton();

		DoClearPoolingObject();
		_bIsDestroy = true;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	static private void ProcInitManagerPooling( List<GameObject> listObject)
	{
		_bIsInit = true;

		System.Type pType = typeof( ENUM_Resource_Name );
		string strEnumName = pType.Name;
		if (listObject.Count == 0)
		{
			Debug.Log( "Init Fail. Please Check Resources Path " + strEnumName );
			return;
		}
		
		for (int i = 0; i < listObject.Count; i++)
		{
			ENUM_Resource_Name eResourceName = default( ENUM_Resource_Name );

			try
			{
				eResourceName = (ENUM_Resource_Name)System.Enum.Parse( pType, listObject[i].name );
			}
			catch
			{
				eResourceName = (ENUM_Resource_Name)((object)listObject[i].name);
				
				if(pType.IsEnum)
					Debug.Log( string.Format( "{0} is not in Enum {1}", listObject[i].name, strEnumName ));
			}

			GameObject pObjectCopy = Object.Instantiate( listObject[i].gameObject );
			pObjectCopy.SetActive( false );
			Class_Resource pResource = pObjectCopy.GetComponent<Class_Resource>();
			if (pResource == null)
				pResource = pObjectCopy.AddComponent<Class_Resource>();

			if (instance._queuePoolingDisable.ContainsKey( eResourceName ) == false)
			{
				instance._queuePoolingDisable.Add( eResourceName, new Queue<Class_Resource>() );
				instance._mapResourcePoolingCount.Add( eResourceName, 0 );
			}

			//if (p_EVENT_OnMakeResource != null)
			//	p_EVENT_OnMakeResource(eResourceName, pResource);

			instance._mapResourceOriginCopy.Add( eResourceName, pResource );
			instance.ProcSetChild( eResourceName, pResource );
			pResource.name = string.Format( "{0}(Origin)", eResourceName );
		}

		if(instance._pObjectManager == null)
		{
			instance._pObjectManager = new GameObject( string.Format( "ManagerPool_{0}", strEnumName ) );
			instance._pTransManager = instance._pObjectManager.transform;
			CCompoEventTrigger pTrigger = instance._pObjectManager.AddComponent<CCompoEventTrigger>();
			pTrigger.p_eInputType_Main = CCompoEventTrigger.EInputType.OnDestroy;
			pTrigger.DoAddEvent_Main( DoReleaseSingleton );
		}
	}

	private void ProcSetChild(ENUM_Resource_Name eResourceName, Class_Resource pObjectMake)
	{
		Transform pTransMake = pObjectMake.transform;
		pTransMake.SetParent(_pTransManager);
		pTransMake.DoResetTransform();
		pObjectMake.name = string.Format("{0}_{1}", eResourceName, ++_mapResourcePoolingCount[eResourceName]);

		int hInstanceID = pObjectMake.GetInstanceID();
		_mapPoolingInstance.Add(hInstanceID, pObjectMake);
		_mapPoolingResourceType.Add(hInstanceID, eResourceName);
	}

	private Class_Resource MakeResource(ENUM_Resource_Name eResourceName)
	{
		if(_mapResourceOriginCopy.ContainsKey(eResourceName) == false)
		{
			Debug.LogWarning(string.Format("Dictionary has not exist Key {0} Check Resource Path {1}", eResourceName, typeof(ENUM_Resource_Name).Name));
			return null;
		}

		Class_Resource pObjectMake = null;

		try
		{
			pObjectMake = Object.Instantiate( _mapResourceOriginCopy[eResourceName] );
			ProcSetChild( eResourceName, pObjectMake );

		}
		catch
		{
			pObjectMake = Object.Instantiate( _mapResourceOriginCopy[eResourceName] );
			ProcSetChild( eResourceName, pObjectMake );
		}

		if (p_EVENT_OnMakeResource != null)
			p_EVENT_OnMakeResource(eResourceName, pObjectMake);

		return pObjectMake;
	}
	
	private void ProcReturnResource(Class_Resource pResource)
	{
		int hInstanceID = pResource.GetInstanceID();
		if (_mapPoolingInstance.ContainsKey(hInstanceID) == false ||
			_mapPoolingResourceType.ContainsKey(hInstanceID) == false)
		{
			//Debug.LogWarning(pResource.name + " Return fail!!");
			return;
		}

		if(pResource.gameObject.activeSelf)
			pResource.gameObject.SetActive(false);

		ENUM_Resource_Name eResourceName = _mapPoolingResourceType[hInstanceID];
		if (_queuePoolingDisable.ContainsKey( eResourceName) == false || 
			_queuePoolingDisable[eResourceName].Contains(pResource))
			return;
		
		_queuePoolingDisable[eResourceName].Enqueue(pResource);

		if (p_EVENT_OnPushResource != null)
			p_EVENT_OnPushResource(eResourceName, pResource);
	}
}
