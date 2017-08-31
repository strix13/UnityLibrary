using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static List<MeshFilter> g_listMeshes = new List<MeshFilter>();

    private MeshFilter[] _arrMeshFilter;

    private void Awake()
    {
        _arrMeshFilter = GetComponentsInChildren<MeshFilter>();
    }

    void OnEnable()
    {
        if (_arrMeshFilter != null)
            g_listMeshes.AddRange(_arrMeshFilter);
    }

    void OnDisable()
    {
		if (_arrMeshFilter == null)
			return;

		for(int i = 0; i < g_listMeshes.Count; i++)
		{
			for(int j = 0; j < _arrMeshFilter.Length; j++)
			{
				if (g_listMeshes[i] == _arrMeshFilter[j])
				{
					g_listMeshes.RemoveAt(i);
					continue;
				}
			}
		}
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (var i = 0; i < g_listMeshes.Count; ++i)
        {
            var mf = g_listMeshes[i];
            if (mf == null) continue;

            var m = mf.sharedMesh;
            if (m == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = 0;
            sources.Add(s);
        }
    }
}
