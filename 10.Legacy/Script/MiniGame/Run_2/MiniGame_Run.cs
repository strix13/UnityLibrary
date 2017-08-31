using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class MiniGame_Run : MonoBehaviour {

    public enum eGameState
    {
        Leady,  //게임시작전
        Start,  //게임 시작 카운트 3초
        Play,   //게임 중
        END,
        Roulette,
    }
    enum eScore
    {
        Perfect,
        Good,
        Clear,
        NotBad, 
        Fail,   //버튼을 잘못 눌렀을 경우 (타일이 없어진다)
        Early,  //너무 빨리 눌렀을 경우   (타일이 안없어진다)
        Miss,   //너무 늦었을 경우        (타일이 없어진다)
    }

    enum eRouletteType
    {
        Reverse,//거꾸로
        AddTime,//시간연장
        Fever,  //질주
        Speed,  //속도증가
    }

    public static MiniGame_Run m_This;

    public eGameState m_eGameState = eGameState.Leady;

    public GameObject[] m_goBackground = new GameObject[2];

    public SkeletonAnimation m_animPlayer;
    public SkeletonAnimation m_animEnemy;
    public SkeletonAnimation m_animScore;
    public SkeletonAnimation[] m_animTitle = new SkeletonAnimation[2];

    public GameObject   m_goSliderTime;
    public UISlider     m_sliderTime;
    public UISlider     m_sliderFever;
    private float       m_fTime = 3f*60f;
    private float       m_fFever = 0f;
    public bool         m_bFever = false;
    public GameObject   m_goFever;
    private GameObject[] m_goFeverTile = new GameObject[12];

    public Transform    m_trGoalPoint;

    private const int                   m_nTotalNum = 12;  //총 타일 개수
    public Transform                    m_trTileRoot;
    private Queue<MiniGame_Run_Tile>    m_arrTile = new Queue<MiniGame_Run_Tile>(); //반복적인 생성을 위한 배열
    private MiniGame_Run_Tile[]         m_tile = new MiniGame_Run_Tile[m_nTotalNum];
    private int                         m_nTileCount= 0; //맨 앞에있는 타일 

    public UISprite         m_usJudgment;

    public GameObject       m_goScore;
    public UILabel          m_ulScore;
    public int              m_nScore = 0;

    public float            m_fGameSpeed       = 1f;
    public float            m_fDiffSpeed       = 0f; //난이도에 따라 빨라지는 스피드
    public float            m_fCreateSpeed     = 2f; //타일 생성주기 ( 최저 0.3f )

    private Queue<float>    m_arrMap = new Queue<float>(); //난이도맵
    private float           m_fDiffTimer = 0;

    private int             m_nCombo = 0;
    public GameObject       m_goCombo;
    public UILabel          m_ulCombo;

    public GameObject       m_goGameOver;
    public GameObject       m_goPause;

    public GameObject       m_goRoulette;
    public SkeletonAnimation m_animRoulette;

    public AudioSource[] m_audio_Btn = new AudioSource[2];

	float                f_speed;

	void Awake () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
        m_This = this.gameObject.GetComponent<MiniGame_Run>();

        m_eGameState = eGameState.Leady;

        CreateTileObject();

        Map();

        ComboEffect();

        StartCoroutine(StartCount());

		f_speed = 2;
	}

    void Map()
    {
        //최대 2초
        //최소 0.3초
        m_arrMap.Clear();
        //---------------
        m_arrMap.Enqueue(10f); //몇초동안
        m_arrMap.Enqueue(2f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로

        m_arrMap.Enqueue(2f); //몇초동안
        m_arrMap.Enqueue(0.3f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로
        //---------------
        m_arrMap.Enqueue(10f); //몇초동안
        m_arrMap.Enqueue(2f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로

        m_arrMap.Enqueue(2f); //몇초동안
        m_arrMap.Enqueue(0.3f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로
        //---------------
        m_arrMap.Enqueue(10f); //몇초동안
        m_arrMap.Enqueue(2f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로

        m_arrMap.Enqueue(2f); //몇초동안
        m_arrMap.Enqueue(0.3f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로
        //---------------
        m_arrMap.Enqueue(10f); //몇초동안
        m_arrMap.Enqueue(2f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로

        m_arrMap.Enqueue(2f); //몇초동안
        m_arrMap.Enqueue(0.3f); //몇초의 생성주기로

        m_arrMap.Enqueue(5f); //몇초동안
        m_arrMap.Enqueue(1f); //몇초의 생성주기로
    }

    IEnumerator DifficultyTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_fDiffTimer);
            m_fDiffTimer = m_arrMap.Dequeue(); //몇초동안
            m_fCreateSpeed = m_arrMap.Dequeue();//이 생성 주기로

            if (m_eGameState == eGameState.END)
            {
                break;
            }
        }
    }

    void CreateTileObject()
    {
        Object res = Resources.Load("Prefabs/MiniGame_Run_Tile");

        for (int i = 0; i < m_nTotalNum; ++i)
        {
            GameObject root = GameObject.Instantiate(res) as GameObject;
            root.transform.parent = m_trTileRoot.transform;
            root.transform.localPosition = new Vector3(0f, 0f, 0f);
            root.transform.localScale = Vector3.one;
            root.SetActive(false);
            MiniGame_Run_Tile tile = root.GetComponent<MiniGame_Run_Tile>();
            m_tile[i] = tile;
            tile.ResetTile();
            m_arrTile.Enqueue(tile);
        }

        Object res_fever = Resources.Load("Prefabs/MiniGame_Run_Fever");

        for (int i = 0; i < 12; ++i)
        {
            GameObject root = GameObject.Instantiate(res_fever) as GameObject;
            root.transform.parent = m_goFever.transform;
            root.transform.localPosition = new Vector3(-235f + (43f * i), -283f, 0f);
            root.transform.localScale = Vector3.one;
            root.SetActive(false);
            m_goFeverTile[i] = root;

            
        }
        UpdateFeverGauge();
    }

    private void UpdateFeverGauge()
    {
        int tileNum = (int)(m_fFever / (100f / 12f));
        for (int i = 0; i < tileNum; ++i)
        {
            m_goFeverTile[i].SetActive(true);
        }
    }

    IEnumerator StartCount()
    {
        UpdateUI();
        yield return new WaitForSeconds(1.5f);
        m_eGameState = eGameState.Start;
        UpdateUI();
        yield return new WaitForSeconds(2.2f);
        m_eGameState = eGameState.Play;
        UpdateUI();
        StartCoroutine(Timer());
        StartCoroutine(ActiveTile());
        StartCoroutine(DifficultyTimer());
    }

    IEnumerator Timer() {
        while(true){
            yield return new WaitForSeconds(1f);

            if (m_eGameState == eGameState.Play || m_eGameState == eGameState.Roulette)
            {
                continue;
            }

            //Debug.LogFormat("Timer()");
            --m_fTime;
            if (m_fTime <= 0)
            {
                m_eGameState = eGameState.END;

                for (int i = 0; i < m_nTotalNum; ++i )
                {
                    m_tile[i].ResetTile();
                }
                UpdateUI();
                break;
            }
            m_sliderTime.value = (1f / (60f * 3f)) * m_fTime;
        }
    }

    void UpdateUI()
    {

        switch (m_eGameState)
        {
            case eGameState.Leady:
                m_goGameOver.SetActive(false);
                m_usJudgment.gameObject.SetActive(false);
                m_goScore.gameObject.SetActive(false);
                m_goSliderTime.gameObject.SetActive(false);
                m_trGoalPoint.gameObject.SetActive(false);
                m_goRoulette.SetActive(false);

                m_animScore.loop = false;
                m_animScore.AnimationName = "readyopen";
                break;
            case eGameState.Start:
                m_goGameOver.SetActive(false);
                m_usJudgment.gameObject.SetActive(false);
                m_goScore.gameObject.SetActive(false);
                m_goSliderTime.gameObject.SetActive(false);
                m_trGoalPoint.gameObject.SetActive(false);
                m_goRoulette.SetActive(false);

                m_animScore.loop = false;
                m_animScore.AnimationName = "start";
                break;
            case eGameState.Play:
                m_goGameOver.SetActive(false);
                m_usJudgment.gameObject.SetActive(false);
                m_goScore.gameObject.SetActive(true);
                m_goSliderTime.gameObject.SetActive(true);
                m_trGoalPoint.gameObject.SetActive(true);
                m_goRoulette.SetActive(false);

                m_animScore.loop = false;
                m_animScore.AnimationName = "startclose";
                m_animPlayer.loop = true;
                m_animPlayer.AnimationName = "run";
                m_animEnemy.loop = true;
                m_animEnemy.AnimationName = "run normal";
                break;
            case eGameState.END:
                m_goGameOver.SetActive(true);
                m_usJudgment.gameObject.SetActive(false);
                m_goScore.gameObject.SetActive(false);
                m_goSliderTime.gameObject.SetActive(false);
                m_trGoalPoint.gameObject.SetActive(false);
                m_goRoulette.SetActive(false);

                m_animScore.gameObject.SetActive(false);
                m_animPlayer.gameObject.SetActive(false);
                m_animEnemy.gameObject.SetActive(false);
                break;
            case eGameState.Roulette:
                StartCoroutine(Update_Roulette());
                //StartCoroutine(Update_Roulette());
                //m_goRoulette.SetActive(true);
                break;
            default:
                break;
        }

    }

    IEnumerator ActiveTile()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_fCreateSpeed);

            if (m_eGameState == eGameState.END || m_eGameState == eGameState.Roulette)
            {
                continue;
            }
            
            MiniGame_Run_Tile tile = m_arrTile.Dequeue();

            int nRan = Random.Range(0, 100);

            if (0 <= nRan && nRan < 45)
            {
                tile.m_eTileType = MiniGame_Run_Tile.eTileType.Tile_A;
            }
            else
            if (45 <= nRan && nRan < 90)
            {
                tile.m_eTileType = MiniGame_Run_Tile.eTileType.Tile_B;
            }
            else
            if (90 <= nRan && nRan < 100)
            {
                tile.m_eTileType = MiniGame_Run_Tile.eTileType.Tile_R;
            }

            //tile.m_eTileType = MiniGame_Run_Tile.eTileType.Tile_R;

            tile.SetImgTile();
            tile.ActiveTile();

        }
    }

	void Update () {
		
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			m_goPause.SetActive (true);
		}

        if (m_eGameState == eGameState.Play)
        {
			m_goBackground[0].transform.Translate(Vector3.left * Time.deltaTime * f_speed * (m_fGameSpeed + m_fDiffSpeed));
			m_goBackground[1].transform.Translate(Vector3.left * Time.deltaTime * f_speed * (m_fGameSpeed + m_fDiffSpeed));

            if (m_goBackground[0].transform.localPosition.x <= -1300.0f) {
                if (Random.Range(0, 100) <= 70) m_animTitle[0].gameObject.SetActive(false); else m_animTitle[0].gameObject.SetActive(true);
                m_goBackground[0].transform.localPosition = new Vector3(1270, 0, 0f);
            }
            if (m_goBackground[1].transform.localPosition.x <= -1300.0f) {
                if (Random.Range(0, 100) <= 70) m_animTitle[1].gameObject.SetActive(false); else m_animTitle[1].gameObject.SetActive(true);
                m_goBackground[1].transform.localPosition = new Vector3(1270, 0, 0f);
            }
        }

	}

    public void ReturnArrayTile(MiniGame_Run_Tile tile)
    {
        m_arrTile.Enqueue(tile);
    }

    public void Miss()
    {
        ApplyScore(eScore.Miss);                    //점수 및 피버타임 스코어 적용
        StartCoroutine(ScoreEffect(eScore.Miss));   //이펙트
        ComboEffect();
        m_tile[m_nTileCount].ResetTile();           //위치값 및 상태 리셋
        ReturnArrayTile(m_tile[m_nTileCount]);      //본 배열로 복귀
        IncreaseCount();                            //맨 앞 타일 카운트 변경
    }

    public void Btn_A() { Btn_Judgment(MiniGame_Run_Tile.eTileType.Tile_A); m_audio_Btn[0].Play(); }

    public void Btn_B() { Btn_Judgment(MiniGame_Run_Tile.eTileType.Tile_B); m_audio_Btn[1].Play(); }

    private void Btn_Judgment(MiniGame_Run_Tile.eTileType btnType)
    {
        //내가 누른 버튼을 받아와서 점수 판정
        float dist = Vector3.Distance(m_tile[m_nTileCount].transform.localPosition, m_trGoalPoint.transform.localPosition);
        eScore score = Judgment(dist);

        if (score == eScore.Early)
        {
            //단순히 너무 빨리 누른거라면 패널티는 주지만 버튼을 없애지는 않는다.
            StartCoroutine(ScoreEffect(score));        //이펙트
            ApplyScore(score);                         //점수 및 피버타임 스코어 적용
            return;
        }

        if (m_tile[m_nTileCount].m_eTileType == MiniGame_Run_Tile.eTileType.Tile_R)
        {
            m_eGameState = eGameState.Roulette;
            UpdateUI();
        }

        if ((m_tile[m_nTileCount].m_eTileType != btnType)
            && (m_bFever == false)
            && (m_tile[m_nTileCount].m_eTileType != MiniGame_Run_Tile.eTileType.Tile_R))
        {
            score = eScore.Fail;
        }

        ApplyScore(score);                         //점수 및 피버타임 스코어 적용
        StartCoroutine(ScoreEffect(score));        //이펙트
        ComboEffect();
        m_tile[m_nTileCount].ResetTile();          //위치값 및 상태 리셋
        ReturnArrayTile(m_tile[m_nTileCount]);     //본 배열로 복귀
        IncreaseCount();                           //맨 앞 타일 카운트 변경
    }

    private eScore Judgment(float dist)
    {
        //거리에 따른 점수 판정
        eScore score = eScore.Fail;
        if (dist > 90)
        {
            score = eScore.Early;
        }
        else if ((60 < dist) && (dist <= 90))
        {
            score = eScore.NotBad;
        }
        else if ((40 < dist) && (dist <= 60))
        {
            score = eScore.Clear;
        }
        else if ((20 < dist) && (dist <= 40))
        {
            score = eScore.Good;
        }
        else if ((0 < dist) && (dist <= 20))
        {
            score = eScore.Perfect;
        }

        return score;
    }

    IEnumerator ScoreEffect(eScore score)
    {
        //점수에 따른 이펙트
        m_usJudgment.gameObject.SetActive(true);

        switch (score)
        {
            case eScore.Perfect: m_usJudgment.spriteName = "perfect"; break;
            case eScore.Good: m_usJudgment.spriteName = "good"; break;
            case eScore.Clear: m_usJudgment.spriteName = "clear"; break;
            case eScore.NotBad: m_usJudgment.spriteName = "not bad"; break;
            case eScore.Fail: m_usJudgment.spriteName = "filed"; break;
            case eScore.Early: m_usJudgment.spriteName = "filed"; break;
            case eScore.Miss: m_usJudgment.spriteName = "miss"; break;
        }
        yield return new WaitForSeconds(1f);
        m_usJudgment.gameObject.SetActive(false);
    }

    private void ComboEffect()
    {
        if (m_nCombo < 1)
        {
            m_goCombo.SetActive(false);
        }
        else if (m_nCombo >= 1)
        {
            m_goCombo.SetActive(true);
            m_ulCombo.text = m_nCombo.ToString();
        }
    }

    private void ApplyScore(eScore score)
    {

        switch (score)
        {
            case eScore.Perfect:
            {
                m_fFever += 5;
                m_nScore += 50;
                m_fTime += 2f;
                ++m_nCombo;
                break;
            }
            case eScore.Good:
            {
                m_fFever += 4;
                m_nScore += 25;
                m_fTime += 2f;
                ++m_nCombo;
                break;
            }
            case eScore.Clear:
            {
                m_fFever += 3;
                m_nScore += 15;
                m_fTime += 2f;
                ++m_nCombo;
                break;
            }
            case eScore.NotBad:
            {
                m_fFever += 2;
                m_nScore += 5;
                m_fTime += 2f;
                ++m_nCombo;
                break;
            }
            case eScore.Fail:
            {
                m_nCombo = 0;
                m_fFever += -2;
                m_fTime += -20;
                StartCoroutine(PenaltyTime());
                break;
            }
            case eScore.Miss:
            {
                m_nCombo = 0;
                m_fFever += -2;
                m_fTime += -20;
                StartCoroutine(PenaltyTime());
                break;
            }
            case eScore.Early:
            {
                m_nCombo = 0;
                m_fFever += -2;
                m_fTime += -20;
                StartCoroutine(PenaltyTime());
                break;
            }
        }

        m_sliderTime.value = (1f / (60f * 3f)) * m_fTime;

        UpdateFeverGauge();
        m_ulScore.text = m_nScore.ToString();

        if (m_bFever == true)
        {
            m_fFever = 0; //0으로 안만들면 피버타임에도 피버게이지가 오른다.
        }

        m_sliderFever.value = (1f / 100f) * m_fFever;
        
        if (m_fFever >= 100f)
        {
            m_fFever = 0;
            m_bFever = true;
            StartCoroutine(FeverTime());
        }
        
    }

    IEnumerator PenaltyTime()
    {
		f_speed = 0.5f;
        yield return new WaitForSeconds(0.8f);
		f_speed = 2f;
    }

    IEnumerator FeverTime()
    {
        m_bFever = true;
        m_fGameSpeed = 1.5f;
        for (int i = 0; i < m_nTotalNum; ++i)
        {
            m_tile[i].SetImgTile();
        }

        yield return new WaitForSeconds(10f);

        m_fFever = 0;
        m_sliderFever.value = (1f / 100f) * m_fFever;
        m_fGameSpeed = 1f;

        m_bFever = false;
        for (int i = 0; i < m_nTotalNum; ++i)
        {
            m_tile[i].SetImgTile();
        }
    }

    private void IncreaseCount()
    {
        ++m_nTileCount;
        if (m_nTileCount >= m_nTotalNum)
        {
            m_nTileCount = 0;
        }
    }

    public void Btn_Pause()
    {
        m_goPause.SetActive(true);
        Time.timeScale = 0f;

    }

    public void Btn_Resume()
    {
        m_goPause.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Btn_Exit()
    {
        Time.timeScale = 1f;
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
    }

    IEnumerator Update_Roulette()
    {
        m_animPlayer.timeScale = 0f;
        m_animScore.timeScale = 0f;
        m_animTitle[0].timeScale = 0f;
        m_animTitle[1].timeScale = 0f;

        m_animRoulette.loop = false;
        m_animRoulette.AnimationName = "run";

        m_goRoulette.SetActive(true);

        yield return new WaitForSeconds(1.07f);

        eRouletteType rouletteType = (eRouletteType)Random.Range(0, 4);
        m_animRoulette.loop = false;

        switch (rouletteType)
        {
            case eRouletteType.Fever: 
                {
				    int RandomAni = Random.Range (0, 3);
			    	if (RandomAni == 0) {
			    		m_animRoulette.AnimationName = "gallop1";
			     	} else if (RandomAni == 1) {
			    		m_animRoulette.AnimationName = "gallop2";
			     	} else if (RandomAni == 2) {
			    		m_animRoulette.AnimationName = "gallop3";
			    	}
				yield return new WaitForSeconds (7f);
                    break;
                }
            case eRouletteType.Reverse:
                {
			      	int RandomAni = Random.Range (0, 3);
			    	if (RandomAni == 0) {
					    m_animRoulette.AnimationName = "Opposition1";
			    	    } else if (RandomAni == 1) {
			     		m_animRoulette.AnimationName = "Opposition2";
			         	} else if (RandomAni == 2) {
				    	m_animRoulette.AnimationName = "Opposition3";
				        }
				yield return new WaitForSeconds (6f);
                    break;
                }
            case eRouletteType.Speed:
                {
		   		    int RandomAni = Random.Range (0, 3);
		     		if (RandomAni == 0) {
			    		m_animRoulette.AnimationName = "speedup1";
		     		} else if (RandomAni == 1) {
			    		m_animRoulette.AnimationName = "speedup2";
		     		} else if (RandomAni == 2) {
			    		m_animRoulette.AnimationName = "speedup3";
			     	}
				yield return new WaitForSeconds (10f);
                    break;
                }
            case eRouletteType.AddTime:
                {
			    	int RandomAni = Random.Range (0, 3);
		    		if (RandomAni == 0) {
		    			m_animRoulette.AnimationName = "Timeextension1";
		    		} else if (RandomAni == 1) {
			    		m_animRoulette.AnimationName = "Timeextension2";
			     	} else if (RandomAni == 2) {
				    	m_animRoulette.AnimationName = "Timeextension3";
		       	 	}
				yield return new WaitForSeconds (7f);
                    break;
                }
            default:
                break;
        }

        m_eGameState = eGameState.Play;

        m_animPlayer.timeScale = 1f;
        m_animScore.timeScale = 1f;
        m_animTitle[0].timeScale = 1f;
        m_animTitle[1].timeScale = 1f;

        m_goRoulette.SetActive(false);

    }

    public void GameResume()
    {
        m_animPlayer.timeScale = 1f;
        m_animScore.timeScale = 1f;
    }
}
