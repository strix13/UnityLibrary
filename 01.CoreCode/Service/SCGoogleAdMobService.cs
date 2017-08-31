#if ADMOB
using admob;
using UnityEngine;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH                               
   Date        : 2017-02-14 오후 5:50:01
   Description : 
   Edit Log    : 
   ============================================ */
public class SCGoogleAdMobService
{
	/* const & readonly declaration             */
	private const string const_strAdmobID_FullScreen = "ca-app-pub-6637495624082233/3169188905";
	private const string const_strAdmobID_Rewarded = "ca-app-pub-6637495624082233/7738989308";
	private const string const_strAdmobID_Banner = "";

	private const string const_strTesterKJH = "113CFA29CF6B560C";
	private const string const_strTesterLYJ = "3B87009C27908D6";

	/* enum & struct declaration                */

	/* public - Variable declaration            */
	public static List<EventDelegate> p_EVENT_OnFinish_FullScreenAds = new List<EventDelegate>();
	public static List<EventDelegate> p_EVENT_OnFinish_RewardedVideo = new List<EventDelegate>();

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	private static Admob _pAdmob = null;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */
	public static void DoInit()
    {
		if (_pAdmob == null) _pAdmob = Admob.Instance();

		_pAdmob.initAdmob(const_strAdmobID_Banner, const_strAdmobID_FullScreen);

		DoRequestAdmob_FullScreen();
		//DoRequestAdMob_RewardedVideo();

		_pAdmob.interstitialEventHandler += EventAdFullScreen;
		//_pAdmob.rewardedVideoEventHandler += EventAdRewarded;
	}

	public static void DoShowAd_FullScreen()
	{
		if (_pAdmob.isInterstitialReady() == false) { DoRequestAdmob_FullScreen(); Debug.LogWarning("전면 광고 재생 실패"); return; }

		_pAdmob.showInterstitial();
	}

	public static void DoShowAd_Rewarded()
	{
		if (_pAdmob.isRewardedVideoReady() == false) { DoRequestAdMob_RewardedVideo(); Debug.LogWarning("보상형 광고 재생 실패"); return; }

		_pAdmob.showRewardedVideo();
	}

	public static void DoRequestAdmob_FullScreen()
	{
		_pAdmob.loadInterstitial();
		_pAdmob.setTesting(true);
	}

	public static void DoRequestAdMob_RewardedVideo()
	{
		_pAdmob.loadRewardedVideo(const_strAdmobID_Rewarded);
		_pAdmob.setTesting(true);
	}
	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */
	private static void EventAdFullScreen(string strEventName, string strMsg)
	{
		DoRequestAdmob_FullScreen();

		Debug.LogWarning("전면 광고를 다봤다. " + strEventName + " - " + strMsg);
	}

	private static void EventAdRewarded(string strEventName, string strMsg)
	{
		DoRequestAdMob_RewardedVideo();

		if (strEventName.Equals("onRewarded") == false) return;

 		Debug.LogWarning("보상형 광고를 다봤다. " + strEventName + " - " + strMsg);

		//EventDelegate.Execute(p_EVENT_OnFinish_RewardedVideo);
	}

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}
#endif