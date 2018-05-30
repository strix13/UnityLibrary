#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-28 오후 4:44:42
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CTweenColor : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Color pColor_Start = Color.white;
    public Color pColor_Dest = Color.black;

    /* protected & private - Field declaration         */

    Renderer _pRenderer;
    Graphic _pUIElement;

    Color _pColor_Backup;
    Material _pMaterial_Backup;

    public Color p_pColor
    {
        get
        {
            if (_pRenderer)
                return _pRenderer.material.color;

            if (_pUIElement)
                return _pUIElement.color;

            return Color.white;
        }

        private set
        {
            if (_pRenderer)
            {
                _pRenderer.material.color = value;
                return;
            }

            if (_pUIElement)
            {
                _pUIElement.color = value;
                return;
            }
        }
    }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pRenderer = GetComponent<Renderer>();
        _pUIElement = GetComponent<Graphic>();
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        pColor_Start = p_pColor;
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        pColor_Dest = p_pColor;
    }

    protected override void OnTween(float fProgress_0_1)
    {
        p_pColor = Color.Lerp(pColor_Start, pColor_Dest, fProgress_0_1);
    }

    public override void OnInitTween_EditorOnly()
    {
        OnAwake();

        _pColor_Backup = p_pColor;
        if (_pRenderer)
        {
            _pMaterial_Backup = _pRenderer.material;
            _pRenderer.material = new Material(_pMaterial_Backup);
            _pRenderer.sharedMaterials = new Material[] { _pMaterial_Backup };
            return;
        }
    }

    public override void OnReleaseTween_EditorOnly()
    {
        _pRenderer.material = _pMaterial_Backup;
        p_pColor = _pColor_Backup;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}