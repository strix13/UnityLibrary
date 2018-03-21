using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

// Build and update a localized navmesh from the sources marked by NavMeshSourceTag
[DefaultExecutionOrder(-102)]
public class CManagerNavMeshBuilder : CSingletonMonoBase<CManagerNavMeshBuilder>
{
    // The center of the build
    public Transform p_pTrasnformFollow;

	[SerializeField]
	private bool _bIsUpdate = false;

    // The size of the build bounds
    public Vector3 p_vecSize = new Vector3(80.0f, 20.0f, 80.0f);
    public Vector3 p_vecCenter = Vector3.zero;

    private NavMeshData _pNavMesh;
    private AsyncOperation _pOperation;
    private NavMeshDataInstance _pInstance;
    private List<NavMeshBuildSource> _listSources = new List<NavMeshBuildSource>();

    private System.Action _OnFinishBakeAsync;

    // ========================== [ Division ] ========================== //

    public void DoBakeNavmesh()
    {
        UpdateNavMesh(false);
    }

    public void DoBakeNavmesh_Async(System.Action OnFinishBake)
    {
        _OnFinishBakeAsync = OnFinishBake;
        StopAllCoroutines();
        StartCoroutine(CoBakeAsync());
    }

    // ========================== [ Division ] ========================== //

    private IEnumerator CoBakeAsync()
    {
		yield return null;

        UpdateNavMesh(true);
        
        yield return _pOperation;
		yield return null;

		if (_OnFinishBakeAsync != null)
	        _OnFinishBakeAsync();
    }

	private IEnumerator CoBakeAsyncLoop()
	{
		while(true)
		{
			UpdateNavMesh(true);

			yield return _pOperation;
		}
	}

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        // Construct and add navmesh
        _pNavMesh = new NavMeshData();
        _pInstance = NavMesh.AddNavMeshData(_pNavMesh);
        if (p_pTrasnformFollow == null)
            p_pTrasnformFollow = transform;


		if (_bIsUpdate)
			StartCoroutine(CoBakeAsyncLoop());
		else
			UpdateNavMesh(false);
	}

	protected override void OnDisableObject()
    {
        base.OnDisableObject();

        // Unload navmesh and clear handle
        _pInstance.Remove();
    }

    static Vector3 Quantize(Vector3 v, Vector3 quant)
    {
        float x = quant.x * Mathf.Floor(v.x / quant.x);
        float y = quant.y * Mathf.Floor(v.y / quant.y);
        float z = quant.z * Mathf.Floor(v.z / quant.z);
        return new Vector3(x, y, z);
    }

    // ========================== [ Division ] ========================== //

    void UpdateNavMesh(bool asyncUpdate = false)
    {
        NavMeshSourceTag.Collect(ref _listSources);
        var defaultBuildSettings = NavMesh.GetSettingsByID(0);
        var bounds = QuantizedBounds();

        if (asyncUpdate)
            _pOperation = NavMeshBuilder.UpdateNavMeshDataAsync(_pNavMesh, defaultBuildSettings, _listSources, bounds);
        else
            NavMeshBuilder.UpdateNavMeshData(_pNavMesh, defaultBuildSettings, _listSources, bounds);
    }

    Bounds QuantizedBounds()
    {
        // Quantize the bounds to update only when theres a 10% change in size
        var center = p_pTrasnformFollow ? p_pTrasnformFollow.position : transform.position;
        center += p_vecCenter;
        return new Bounds(Quantize(center, 0.1f * p_vecSize), p_vecSize);
    }

    // ========================== [ Division ] ========================== //

    void OnDrawGizmosSelected()
    {
        if (_pNavMesh)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_pNavMesh.sourceBounds.center, _pNavMesh.sourceBounds.size);
        }

        Gizmos.color = Color.yellow;
        var bounds = QuantizedBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.color = Color.green;
        var center = p_pTrasnformFollow ? p_pTrasnformFollow.position : transform.position;
        center += p_vecCenter;
        Gizmos.DrawWireCube(center, p_vecSize);
    }
}
