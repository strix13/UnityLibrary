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
    void OnUpdate();
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
            for (int i = 0; i < _listObject.Count; i++)
                _listObject[i].OnUpdate();

#if UNITY_EDITOR
            name = string.Format("업데이트 매니져/{0}개 업데이트중", _listObject.Count - 1); // 한개는 자기 자신이기때문에 깎는다.
#endif

            yield return null;
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
