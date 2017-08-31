using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame_PickNum_Tile : MonoBehaviour {

    public enum eTileType
    {
        Normal,
        Fever,
        Red
    }

    private eTileType m_eTileType;
    public int m_nIdx;
    public UILabel m_ulIdx;
    public UISprite m_usTile;

    public void SetType(int nIdx, eTileType tileType)
    {
        m_nIdx = nIdx;

        m_eTileType = tileType;
        switch (tileType)
        {
            case eTileType.Normal:
                m_ulIdx.text = (m_nIdx +1).ToString();
                m_usTile.spriteName = "answer";
                break;
            case eTileType.Fever:
                m_ulIdx.text = "FEVER";
                m_usTile.spriteName = "fever";
                break;
            case eTileType.Red:
                m_ulIdx.text = "RED";
                m_usTile.spriteName = "Wrong answer";
                break;
            default:
                break;
        }
    }

    public void BtnTouch()
    {
        this.gameObject.SetActive(false);
        MiniGame_PickNum.m_This.OnClickTile(m_eTileType, m_nIdx);
    }

}
