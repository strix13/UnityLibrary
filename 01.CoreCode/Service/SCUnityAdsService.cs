using UnityEngine;
using System.Collections.Generic;

#if UNITY_ADS
using UnityEngine.Advertisements;
#else
public enum ShowResult
{
    Finished, Skipped, Failed
}
public class ShowOptions
{
}
#endif

#pragma warning disable 0414

/* ============================================ 
   Editor      : KJH                               
   Date        : 2017-02-14 오후 5:50:01
   Description : 
   Edit Log    : 
   ============================================ */

public class SCUnityAdsService
{
	/* const & readonly declaration             */
	private const string const_strAppID_Android = "1395478";
	private const string const_strAppID_IOS = "1395479";

	private const string const_strVideoID = "rewardedVideo";

	/* enum & struct declaration                */

	/* public - Variable declaration            */
	//public static List<EventDelegate> p_EVENT_OnResultUnityAds_Rewarded = new List<EventDelegate>()
	private static System.Action<ShowResult> _OnFinishUAds_Rewarded = null;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	private static ShowOptions _pUnityAdsOptions = new ShowOptions();

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */
	public static void DoInit()
    {
#if UNITY_ADS
		Advertisement.Initialize(const_strAppID_Android, false);

		_pUnityAdsOptions.resultCallback = OnResult_UnityAds;
#endif
	}

    // UnityEngine.Advertisements
    public static void DoShow_RewardedVideo(System.Action<ShowResult> onFinishUAds_Rewarded)
	{
#if UNITY_ADS
		if (Advertisement.IsReady() == false) return;

        _OnFinishUAds_Rewarded = onFinishUAds_Rewarded;
        Advertisement.Show(const_strVideoID, _pUnityAdsOptions);
#endif
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */
    private static void OnResult_UnityAds(ShowResult eResult)
	{
		switch (eResult)
		{
			case ShowResult.Finished:
				if (_OnFinishUAds_Rewarded != null)
                {
                    _OnFinishUAds_Rewarded(eResult);
                    _OnFinishUAds_Rewarded = null;
                }
			break;

			case ShowResult.Skipped: break;
			case ShowResult.Failed: break;
		}
	}

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}