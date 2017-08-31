using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-30 오후 1:33:47
// Description : 
// Edit Log    : 
// ============================================ 

public class SCSceneLoader<ENUM_Scene_Name>
    where ENUM_Scene_Name : System.IFormattable, System.IConvertible, System.IComparable
{
    //protected struct SSceneGroupInfo
    //{
    //    public ENUM_Scene_Name eBaseScene;
    //    public List<ENUM_Scene_Name> listAdditiveScene;

    //    public SSceneGroupInfo(ENUM_Scene_Name eBaseScene, List<ENUM_Scene_Name> listAdditiveScene)
    //    {
    //        this.eBaseScene = eBaseScene; this.listAdditiveScene = listAdditiveScene;
    //    }
    //}

    // ===================================== //
    // public - Variable declaration         //
    // ===================================== //

    public event UnityEngine.Events.UnityAction<Scene, LoadSceneMode> p_EVENT_OnSceneLoaded { add { SceneManager.sceneLoaded += value; } remove { SceneManager.sceneLoaded -= value; } }

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    private AsyncOperation _pCurrentAsyncOP;     public AsyncOperation p_pAsyncOP { get { return _pCurrentAsyncOP; } }
    private EventDelegate.Callback _OnLoadCompleteAll;

    private int _iLoadSceneCountCurrent;
    private int _iLoadSceneCount;
    private bool _bCheckLoadSceneListComplete;       public bool p_bCheckLoadSceneListComplete {  get { return _bCheckLoadSceneListComplete; } }
    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //

    public void DoLoadSceneAsync(List<ENUM_Scene_Name> listScene, EventDelegate.Callback OnLoadCompleteAll)
    {
        _bCheckLoadSceneListComplete = true;
        _OnLoadCompleteAll = OnLoadCompleteAll;
        _iLoadSceneCountCurrent = 0;
        _iLoadSceneCount = listScene.Count;

        ProcAsyncLoad(listScene[0].ToString(), LoadSceneMode.Single);
        for(int i = 1; i < listScene.Count; i++)
            ProcAsyncLoad(listScene[i].ToString(), LoadSceneMode.Additive);
    }

    public void DoLoadSceneAsync(ENUM_Scene_Name eScene, LoadSceneMode eLoadSceneMode)
    {
        ProcAsyncLoad(eScene.ToString(), eLoadSceneMode);
    }

	public void DoLoadSceneAsync_FadeInOut( ENUM_Scene_Name eScene, float fFadeDuration, Color pColor )
	{
		AutoFade.LoadLevel( eScene.ToString(), fFadeDuration / 2f, fFadeDuration / 2f, pColor );
	}

	// ===================================== //
	// public - [Event] Function             //
	// 프랜드 객체가 요청                    //
	// ===================================== //

	public void EventCheckIsLoadComplete()
    {
        if (_bCheckLoadSceneListComplete)
        {
            if (++_iLoadSceneCountCurrent == _iLoadSceneCount)
            {
                _bCheckLoadSceneListComplete = false;
                _OnLoadCompleteAll();
            }
        }
    }

    // ========================================================================== //

    // ===================================== //
    // protected - [abstract & virtual]      //
    // ===================================== //

    // ===================================== //
    // protected - [Event] Function          //
    // 자식 객체가 요청                      //
    // ===================================== //

    // ===================================== //
    // protected - Override & Unity API      //
    // ===================================== //

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    private void ProcAsyncLoad(string strSceneName, LoadSceneMode eLoadSceneMode)
    {
        _pCurrentAsyncOP = SceneManager.LoadSceneAsync(strSceneName, eLoadSceneMode);
    }

    // ===================================== //
    // private - Other[Find, Calculate] Func //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

}
