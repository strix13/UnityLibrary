#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-17 오후 10:36:36
 *	기능 : 
 *	https://blogs.unity3d.com/kr/2015/12/23/1k-update-calls/
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public interface IUpdateAble
{
    void OnUpdate(ref bool bCheckUpdateCount);
}

public class CManagerUpdateObject : CSingletonDynamicMonoBase<CManagerUpdateObject>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected & private - Field declaration         */

    private List<IUpdateAble> _listObject = new List<IUpdateAble>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAddObject(IUpdateAble pObject)
    {
        _listObject.Add(pObject);
    }

    public void DoRemoveObject(IUpdateAble pObject)
    {
        _listObject.Remove(pObject);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        while(true)
        {
            int iUpdateObjectCount = 0;
            for (int i = 0; i < _listObject.Count; i++)
            {
                bool bCheckUpdate = false;
                _listObject[i].OnUpdate(ref bCheckUpdate);
                if(bCheckUpdate)
                    iUpdateObjectCount++;
            }

#if UNITY_EDITOR
            name = string.Format("업데이트 매니져/{0}개 업데이트중", iUpdateObjectCount);
#endif

            yield return null;
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
