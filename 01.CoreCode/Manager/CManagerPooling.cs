using UnityEngine;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-25 오전 12:03:32
   Description : 
   Edit Log    : 
   ============================================ */

public class CManagerPooling<ENUM_Resource_Name, Class_Resource> : CSingletonBase_Not_UnityComponent<CManagerPooling<ENUM_Resource_Name, Class_Resource>>
	where ENUM_Resource_Name : struct, System.IComparable, System.IConvertible
	where Class_Resource : Component
{
	/* public - Field declaration            */

	public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnMakeResource;
	public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnPopResource;
	public event System.Action<ENUM_Resource_Name, Class_Resource> p_EVENT_OnPushResource;

	/* private - Field declaration           */

	protected Dictionary<ENUM_Resource_Name, Class_Resource> _mapResourceOriginCopy = new Dictionary<ENUM_Resource_Name, Class_Resource>();

	private Dictionary<int, Class_Resource> _mapPoolingInstance = new Dictionary<int, Class_Resource>();
	private Dictionary<int, ENUM_Resource_Name> _mapPoolingResourceType = new Dictionary<int, ENUM_Resource_Name>();

	private Dictionary<ENUM_Resource_Name, Queue<Class_Resource>> _queuePoolingDisable = new Dictionary<ENUM_Resource_Name, Queue<Class_Resource>>();
	private Dictionary<ENUM_Resource_Name, int> _mapResourcePoolingCount = new Dictionary<ENUM_Resource_Name, int>();

	private Transform _pTransManagerObject;	public GameObject p_pObjectManager {  get { return _pTransManagerObject.gameObject; } }

	private UIPanel _pPanel;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

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
		else
			pFindResource = _queuePoolingDisable[eResourceName].Dequeue();

		if(pFindResource == null)
		{
			Debug.LogWarning( eResourceName + "를 못만들었다.." );
			return null;
		}

		if (pFindResource.transform.parent != _pTransManagerObject)
			pFindResource.transform.SetParent(_pTransManagerObject);

		pFindResource.gameObject.SetActive(bGameObjectActive);

		if(p_EVENT_OnPopResource != null)
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
	public void DoPopAll()
	{
		IEnumerator<KeyValuePair<int, Class_Resource>> pIter = _mapPoolingInstance.GetEnumerator();
		while(pIter.MoveNext())
		{
			ProcReturnResource(pIter.Current.Value);
		}
	}
	
	// ========================================================================== //
	
	/* protected - Override & Unity API         */

	protected override void OnMakeSingleton()
	{
		base.OnMakeSingleton();

		string strEnumName = typeof(ENUM_Resource_Name).Name;

		if (strEnumName[0] == 'E')
			strEnumName = strEnumName.Substring(1, strEnumName.Length - 1);

		GameObject pObjectManager = new GameObject(string.Format("ManagerPool_{0}", strEnumName));
		_pTransManagerObject = pObjectManager.transform;
		CCompoEventTrigger pTrigger = pObjectManager.AddComponent<CCompoEventTrigger>();
		pTrigger.p_eInputType = CCompoEventTrigger.EInputType.OnDestroy;
		EventDelegate.Add( pTrigger.p_OnEventAction, DoReleaseSingleton );

		GameObject[] arrResources = Resources.LoadAll<GameObject>(string.Format("{0}/", strEnumName));

		if(arrResources.Length == 0)
		{
			Debug.Log("Init Fail. Please Check Resources Path " + strEnumName);
			return;
		}

		for (int i = 0; i < arrResources.Length; i++)
		{
			bool bSuccess = true;
			ENUM_Resource_Name eResourceName = default(ENUM_Resource_Name);

			try
			{
				eResourceName = (ENUM_Resource_Name)System.Enum.Parse(typeof(ENUM_Resource_Name), arrResources[i].name);
			}
			catch
			{
				bSuccess = false;
				Debug.LogWarning(string.Format("{0} is not in Enum {1}", arrResources[i].name, strEnumName, this));
			}

			if (bSuccess)
			{
				GameObject pObjectCopy = Object.Instantiate(arrResources[i]);
				pObjectCopy.SetActive(false);
				Class_Resource pResource = pObjectCopy.GetComponent<Class_Resource>();
				if (pResource == null)
					pResource = pObjectCopy.AddComponent<Class_Resource>();

				if (_queuePoolingDisable.ContainsKey(eResourceName) == false)
				{
					_queuePoolingDisable.Add(eResourceName, new Queue<Class_Resource>());
					_mapResourcePoolingCount.Add(eResourceName, 0);
				}

				//if (p_EVENT_OnMakeResource != null)
				//	p_EVENT_OnMakeResource(eResourceName, pResource);

				_mapResourceOriginCopy.Add(eResourceName, pResource);
				ProcSetChild(eResourceName, pResource);
				pResource.name = string.Format( "{0}(Origin)", eResourceName );
				//ProcReturnResource(pResource);
			}
		}

		List<Class_Resource> listResources = _mapResourceOriginCopy.Values.ToList();
		if (listResources.Count == 0)
			return;

		for(int i = 0; i < listResources.Count; i++)
		{
			UIWidget pUIWidget = listResources[i].GetComponentInChildren<UIWidget>(true);
			if (pUIWidget != null)
			{
				_pPanel = p_pObjectManager.AddComponent<UIPanel>();
				p_pObjectManager.layer = pUIWidget.gameObject.layer;
				break;
			}
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void ProcSetChild(ENUM_Resource_Name eResourceName, Class_Resource pObjectMake)
	{
		Transform pTransMake = pObjectMake.transform;
		pTransMake.SetParent(_pTransManagerObject);
		pObjectMake.name = string.Format("{0}_{1}", eResourceName, ++_mapResourcePoolingCount[eResourceName]);

		int hInstanceID = pObjectMake.GetInstanceID();
		_mapPoolingInstance.Add(hInstanceID, pObjectMake);
		_mapPoolingResourceType.Add(hInstanceID, eResourceName);
	}

	private Class_Resource MakeResource(ENUM_Resource_Name eResourceName)
	{
		if(_mapResourceOriginCopy.ContainsKey(eResourceName) == false)
		{
			Debug.Log(string.Format("Dictionary has not exist Key {0} Check Resource Path {1}", eResourceName, typeof(ENUM_Resource_Name).Name));
			return null;
		}

		Class_Resource pObjectMake = Object.Instantiate(_mapResourceOriginCopy[eResourceName]);
		ProcSetChild(eResourceName, pObjectMake);

		if(p_EVENT_OnMakeResource != null)
			p_EVENT_OnMakeResource(eResourceName, pObjectMake);

		return pObjectMake;
	}

	private void ProcReturnResource(Class_Resource pResource)
	{
		int hInstanceID = pResource.GetInstanceID();
		if (_mapPoolingInstance.ContainsKey(hInstanceID) == false ||
			_mapPoolingResourceType.ContainsKey(hInstanceID) == false)
		{
			Debug.LogWarning(pResource.name + " Return fail!!");
			return;
		}

		pResource.gameObject.SetActive(false);
		ENUM_Resource_Name eResourceName = _mapPoolingResourceType[hInstanceID];
		if (_queuePoolingDisable[eResourceName].Contains(pResource))
			return;

		_queuePoolingDisable[eResourceName].Enqueue(pResource);

		if (p_EVENT_OnPushResource != null)
			p_EVENT_OnPushResource(eResourceName, pResource);
	}
}
