using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class MiniGame_Run_Tile : MonoBehaviour {

    public enum eTileType
    {
        Tile_A,
        Tile_B,
        Tile_R
    }

    private bool m_bActive = false;
    public eTileType m_eTileType = eTileType.Tile_A;

    public UISprite m_usTile;
    public SkeletonAnimation m_animTile_R;

    void Awake()
    {
    }

    public void SetImgTile()
    {
        if (MiniGame_Run.m_This.m_bFever == false)
        {
            switch (m_eTileType)
            {
                case eTileType.Tile_A:
                    m_usTile.spriteName = "button A";
                    m_usTile.gameObject.SetActive(true);
                    m_animTile_R.gameObject.SetActive(false);
                    break;
                case eTileType.Tile_B:
                    m_usTile.spriteName = "button B";
                    m_usTile.gameObject.SetActive(true);
                    m_animTile_R.gameObject.SetActive(false);
                    break;
                case eTileType.Tile_R:
                    m_usTile.gameObject.SetActive(false);
                    m_animTile_R.gameObject.SetActive(true);
                    m_animTile_R.loop = false;
                    m_animTile_R.AnimationName = "idle";
                    break;
            }
        }
        else
        {
            switch (m_eTileType)
            {
                case eTileType.Tile_A:
                    m_usTile.spriteName = "fever button A";
                    m_usTile.gameObject.SetActive(true);
                    m_animTile_R.gameObject.SetActive(false);
                    break;
                case eTileType.Tile_B:
                    m_usTile.spriteName = "fever button B";
                    m_usTile.gameObject.SetActive(true);
                    m_animTile_R.gameObject.SetActive(false);
                    break;
                case eTileType.Tile_R:
                    m_usTile.gameObject.SetActive(false);
                    m_animTile_R.gameObject.SetActive(true);
                    m_animTile_R.loop = false;
                    m_animTile_R.AnimationName = "idle";
                    break;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if ((m_bActive == false)||(MiniGame_Run.m_This.m_eGameState != MiniGame_Run.eGameState.Play)) return;
        this.transform.Translate(Vector3.left * Time.deltaTime * 1 * (MiniGame_Run.m_This.m_fGameSpeed + MiniGame_Run.m_This.m_fDiffSpeed));

        if (this.transform.localPosition.x <= -430f)
        {
            Miss();
        }
        
		
	}

    public void ActiveTile()
    {
        m_bActive = true;
        this.gameObject.SetActive(m_bActive);

    }

    public void Miss()
    {
        MiniGame_Run.m_This.Miss();          //점수처리
    }

    public void ResetTile()
    {
        m_bActive = false;
        this.gameObject.SetActive(m_bActive);
        this.transform.localPosition = new Vector3(720f, 200f, 0f);
    }

}
