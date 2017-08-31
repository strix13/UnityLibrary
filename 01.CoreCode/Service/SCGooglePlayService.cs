#if UNITY_ADS

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
/* ============================================ 
   Editor      : KJH                               
   Date        : 2017-02-14 오후 5:50:01
   Description : 
   Edit Log    : 
   ============================================ */
public static class SCGooglePlayService
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */
	[System.Serializable]
    public class SGooglePlayer
    {
        public string playerID;
        public string name;
    }

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    private static System.Action<bool> _onLogin;
	//private static GooglePlayGames _pGoogle = null;
    private static SGooglePlayer _sGooglePlayer;

	private static PlayGamesLocalUser _pUser = null;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */
    public static PlayGamesLocalUser DoInitAndLogin(System.Action<bool> onLogin = null)
	{
		ProcCheckGooglePlayService();

		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
	   .RequestEmail().RequestIdToken()
	   .Build();

		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();

		Social.localUser.Authenticate(ProcOnGoogleLogin);

		_pUser = (PlayGamesLocalUser)Social.localUser;
		_onLogin += onLogin;

		return _pUser;
	}

	public static PlayGamesLocalUser DoAddLoginEvent(this PlayGamesLocalUser pUser, System.Action<bool> onLogin)
	{
		_onLogin += onLogin;

		return pUser;
	}

	public static void DoLogout()
	{
		if (Social.localUser.authenticated == false) return;

		((PlayGamesPlatform)Social.Active).SignOut();
	}

    public static string GetGoogleNickName() { return _pUser.userName; }
    public static string GetGoogleUID() { return _pUser.id; }

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */
	private static void ProcOnGoogleLogin(bool bSuccess)
	{
		_onLogin(bSuccess);
    }
	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */
	private static void ProcCheckGooglePlayService()
	{
		using (AndroidJavaClass pClass = new AndroidJavaClass("com.Noonbaram.HitZombie.GooglePlayServiceExtends"))
		{
			using (AndroidJavaObject pContext = pClass.GetStatic<AndroidJavaObject>("pContext"))
			{
				string strResult = pContext.Call<string>("DoCheckGPGSUpdate");

				Debug.Log("GPGS Check Result : " + strResult);
			}
		}
	}
	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}
#endif