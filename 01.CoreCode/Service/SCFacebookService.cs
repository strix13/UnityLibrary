#if Facebook
using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;

/* ============================================ 
   Editor      : KJH                               
   Date        : 2017-02-14 오후 5:50:01
   Description : 
   Edit Log    : 
   ============================================ */
public class SCFacebookService
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    private static List<string> _listFbPermissions = new List<string>() { "public_profile", "email", "user_friends" };

    private static System.Action<AccessToken> _onLogin = null;
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */
    public static void DoInitAndLogin(System.Action<AccessToken> onLogin = null)
    {
        if (FB.IsInitialized == false)
            FB.Init(OnInit_Facebook, OnHide_Unity);
        else
            FB.ActivateApp();

        AddEvent_OnLogin(onLogin);
    }

    public static void AddEvent_OnLogin(System.Action<AccessToken> onLogin = null)
    {
        _onLogin += onLogin;
    }

    private static void OnInit_Facebook()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            FB.LogInWithReadPermissions(_listFbPermissions, OnLogin_Facebook);
        }
        else
            Debug.LogWarning("페이스북 초기화 실패!");
    }

    private static void OnLogin_Facebook(ILoginResult iLoginResult)
    {
        if (FB.IsLoggedIn)
        {
            AccessToken pAccessToken = AccessToken.CurrentAccessToken;
            Debug.Log(pAccessToken.UserId);

            if (_onLogin != null) _onLogin(pAccessToken);
        }
        else
            Debug.LogWarning("페이스북 로그인 실패!");
    }

    private static void OnHide_Unity(bool bShow)
    {
        if (bShow)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }

    public static void DoLogOut()
    {
        FB.LogOut();
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}
#endif