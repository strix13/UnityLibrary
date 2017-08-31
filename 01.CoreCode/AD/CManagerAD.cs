#if UNITY_ADS

using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class CManagerAD : CSingletonBase<CManagerAD>
{
    [SerializeField]
    private string strGameID = "";
    private ShowOptions _pShowOption = new ShowOptions();

    // ========================== [ Division ] ========================== //

    public void DoStartAD(System.Action<ShowResult> OnEndAD)
    {
        StopAllCoroutines();
        StartCoroutine(ProcStart(OnEndAD));
    }

    // ========================== [ Division ] ========================== //

    private IEnumerator ProcStart(System.Action<ShowResult> OnEndAD)
    {
        if (Advertisement.isSupported)
            Advertisement.Initialize(strGameID);

        while(Advertisement.isInitialized == false || Advertisement.IsReady() == false)
        {
            yield return new WaitForSeconds(0.5f);
        }

        _pShowOption.resultCallback = OnEndAD;
        Advertisement.Show(null, _pShowOption);
    }
}
#endif