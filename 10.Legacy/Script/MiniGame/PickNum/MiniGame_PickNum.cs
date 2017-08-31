using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame_PickNum : MonoBehaviour {

    enum eGameState
    {
        Lobby,
        UnActive,
        Play,
        GameOver,
    }

    private const float m_fTile_Width   = 100f;
    private const float m_fTile_Height  = 100f;
    private const float m_fTime_Tile    = 1.5f;     //타일 당 초
    private const float m_fBlankUp      = 100f;     //타일이 나타나지 않는 영역

    public static MiniGame_PickNum m_This;

    private eGameState m_eGameState = eGameState.UnActive;

    private int m_nScore = 0;
    public UILabel m_ulScore;

	public GameObject                g_Exit;
    public UISlider m_slider_Time;
    public GameObject m_goRoot_Tile;
    private const int m_nTotalNum = 10;
    private int m_nStageNum = 0;
    public MiniGame_PickNum_Stage[] m_stage = new MiniGame_PickNum_Stage[5];
    private MiniGame_PickNum_Tile[] m_tile = new MiniGame_PickNum_Tile[m_nTotalNum];

    private float m_fStartTime = 0f;
    private float m_fTime = 0f;
    private Queue<int> m_arrTile = new Queue<int>();
    private int m_nTileAct_Num = 0; //한판을 끝내기 위해 눌러야하는 타일 수

    public GameObject m_goGameOver;

	void Awake () {
        m_This = this;
        CreateTileObject();

        ResetStage();
        SetStage();
        UpdateUI();

	}

    void CreateTileObject()
    {

        //Object res = Resources.Load("Prefabs/MiniGame_PickNum_Tile");

        //for (int i = 0; i < m_nTotalNum; ++i)
        //{
        //    GameObject root = GameObject.Instantiate(res) as GameObject;
        //    root.transform.parent = m_goRoot_Tile.transform;
        //    root.transform.localPosition = new Vector3(0f, 0f, 0f);
        //    root.transform.localScale = Vector3.one;
        //    root.SetActive(false);
        //    MiniGame_PickNum_Tile tile = root.GetComponent<MiniGame_PickNum_Tile>();
        //    m_tile[i] = tile;
        //    //m_arrTile.Enqueue(tile);
        //}

    }

    void SetStage()
    {
        m_eGameState = eGameState.Play;

        int nTotalNum = Random.Range(1, 5);//Random.Range(1, 5)
        int nRedTile    = 0;
        bool bTermTile  = false;
        bool bFever     = false;

        if (nTotalNum < 7) 
		if (Random.Range(0, 100) < 20) 
			nRedTile = Random.Range(1, 3); 
        
        if (Random.Range(0, 100) < 40) bTermTile = true;   
        if (Random.Range(0, 100) < 0) bFever = true;

        m_fStartTime = (float)nTotalNum * m_fTime_Tile; 
        m_fTime = m_fStartTime;

        nTotalNum += nRedTile;
        m_nStageNum = nTotalNum - 1;

        int x = 0;
        int y = 0;

        switch (nTotalNum)
        {
            case 1: x = Random.Range(209, 626); y = Random.Range(0, 861); break;
            case 2: x = Random.Range(234, 598); y = Random.Range(0, 772); break;
            case 3: x = Random.Range(258, 587); y = Random.Range(-5, 732); break;
            case 4: x = Random.Range(276, 533); y = Random.Range(-1, 727); break;
            case 5: x = Random.Range(271, 528); y = Random.Range(15, 681); break;
            case 6: x = Random.Range(275, 506); y = Random.Range(15, 681); break;
            case 7: x = Random.Range(273, 477); y = Random.Range(58, 653); break;
        }

        m_stage[m_nStageNum].gameObject.transform.localPosition = new Vector3(x, y, 0f);

        ActiveTile(nTotalNum, nRedTile, bTermTile, bFever);
    }

    public void ActiveTile(int nTotalNum = 1, int nRedTile = 0, bool bTermTile = false, bool bFever = false)
    {
        Debug.LogFormat("m_nStageNum : {0}", m_nStageNum);
        m_stage[m_nStageNum].gameObject.SetActive(true);
        //1일때는 어떤 오브젝트

        int nTermIdx = 0;
        if (bTermTile == true)
        {
            nTermIdx = Random.Range(0, nTotalNum - nRedTile - 1);
        }

        //---------------------------------------------------------
        for (int i = 0; i < nTotalNum; ++i)
        {
            MiniGame_PickNum_Tile.eTileType tileType = MiniGame_PickNum_Tile.eTileType.Normal;
            int nTerm = 0;
            if (nTermIdx < i) nTerm = 1;

            if (bFever == true)
            {
                tileType = MiniGame_PickNum_Tile.eTileType.Fever;
            }
            if (nRedTile > 0 && i >= nTotalNum - nRedTile)
            {
                //빨간 타일은 걍 큐의 마지막
                tileType = MiniGame_PickNum_Tile.eTileType.Red;
            }

            m_stage[m_nStageNum].m_tile[i].gameObject.SetActive(true);
            m_stage[m_nStageNum].m_tile[i].SetType(i + nTerm, tileType);
            m_arrTile.Enqueue(i + nTerm);

            if (tileType == MiniGame_PickNum_Tile.eTileType.Normal 
                || tileType == MiniGame_PickNum_Tile.eTileType.Fever)
                ++m_nTileAct_Num;
        }

        SetTileLocation();
    }

    void SetTileLocation()
    {


    }

	void Update () {

		if (Input.GetKey (KeyCode.Escape)) 
		{
			g_Exit.SetActive (true);
		}

        if (m_eGameState == eGameState.Play)
        {
            m_fTime -= Time.deltaTime;
            m_slider_Time.value = (1f / m_fStartTime) * m_fTime;
            
            if (m_fTime <= 0f)
            {
                if (m_arrTile.Count > 0)
                {
                    //Debug.LogFormat("시간다 됨 게임오버");
                    m_eGameState = eGameState.GameOver;
                    UpdateUI();
                }
                
            }
            
        }
	}

    public void OnClickTile(MiniGame_PickNum_Tile.eTileType tileType, int nIdx)
    {

        switch (tileType)
        {
            case MiniGame_PickNum_Tile.eTileType.Normal:
                {
                    int nJudge = m_arrTile.Dequeue();

                    if (nJudge != nIdx)
                    {
                        //Debug.LogFormat("잘못누름 게임오버");
                        m_eGameState = eGameState.GameOver;
                        UpdateUI();
                    }
                    else
                    {
                        --m_nTileAct_Num;
                        m_nScore += 1;
                        //Debug.LogFormat("잘누름");
                    }
                }
                break;
            case MiniGame_PickNum_Tile.eTileType.Fever:
                {
                    m_nScore += 1;
                    //Debug.LogFormat("잘 누름 피버!");
                    --m_nTileAct_Num;
                    m_arrTile.Dequeue();
                }
                break;
            case MiniGame_PickNum_Tile.eTileType.Red:
                {
                    //Debug.LogFormat("빨간거 누름 게임오버");
                    m_eGameState = eGameState.GameOver;
                    UpdateUI();
                }
                break;
            default:
                break;
        }

        if (m_nTileAct_Num <=0)
        {	
			ResetStage();	
            SetStage();
        }
		UpdateUI ();
    }

    public void UpdateUI()
    {

        switch (m_eGameState)
        {
            case eGameState.Lobby:
                break;
            case eGameState.UnActive:
                break;
            case eGameState.Play:
                m_ulScore.text = m_nScore.ToString();
                break;
            case eGameState.GameOver:
                m_goGameOver.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ResetStage()
    {
        m_arrTile.Clear();
        m_nTileAct_Num = 0;
        for (int i = 0; i < m_stage[m_nStageNum].m_tile.Length; ++i)
        {
            m_stage[m_nStageNum].m_tile[i].gameObject.SetActive(false);
        }
        m_stage[m_nStageNum].gameObject.SetActive(false);

    }

    public void GameOver()
    {


    }
    public void Exit()
    {
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
    }

	public void Cancle(GameObject g_Exit)
	{
		g_Exit.SetActive (false);
	}
}
