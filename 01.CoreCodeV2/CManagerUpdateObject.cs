#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-17 오후 10:36:36
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class CManagerUpdateObject : CSingletonDynamicBase<CManagerUpdateObject>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    private List<CObjectBase> _listObject = new List<CObjectBase>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAddObject(CObjectBase pObject)
    {
        _listObject.Add(pObject);
    }

    public void DoRemoveObject(CObjectBase pObject)
    {
        _listObject.Remove(pObject);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void Update()
    {
        for (int i = 0; i < _listObject.Count; i++)
            _listObject[i].OnUpdate();
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private
    
    #endregion Private
}
