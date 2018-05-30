#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-11 오전 9:35:07
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CStreammingAssetGetter 
{
    Dictionary<string, WWW> _mapResourceCashing = new Dictionary<string, WWW>();
    StringBuilder _pStrBuilder = new StringBuilder();

    MonoBehaviour _pCoroutineExcuter;

    // ========================== [ Division ] ========================== //

    public CStreammingAssetGetter(MonoBehaviour pCoroutineExcuter)
    {
        _pCoroutineExcuter = pCoroutineExcuter;
    }

    public void GetResource(string strResourceName_With_Extension, System.Action<WWW> OnGetResource, bool bIsCashing)
    {
        if (bIsCashing)
        {
            WWW pFindResource;
            if (_mapResourceCashing.TryGetValue(strResourceName_With_Extension, out pFindResource) == false)
                OnGetResource(pFindResource);
        }

        _pCoroutineExcuter.StartCoroutine(CoGetStreammingAsset(strResourceName_With_Extension, OnGetResource, bIsCashing));
    }

    private IEnumerator CoGetStreammingAsset(string strResourceName_With_Extension, System.Action<WWW> OnGetResource, bool bIsCashing)
    {
        _pStrBuilder.Length = 0;
#if UNITY_EDITOR
        _pStrBuilder.Append("file://");
#endif
        _pStrBuilder.Append(strResourceName_With_Extension);

        WWW www = new WWW(_pStrBuilder.ToString());
        yield return www;

        OnGetResource(www);
        if (bIsCashing)
            _mapResourceCashing.Add(strResourceName_With_Extension, www);
    }
}
