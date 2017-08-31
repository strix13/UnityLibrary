using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIPanel))]
public class CUIManagerFadeInOut : CSingletonBase<CUIManagerFadeInOut>
{
    [SerializeField]
    private Texture _pTexture = null;
    private UITexture _pUITexture;
    private TweenAlpha _pTweenAlpha;

    // ========================== [ Division ] ========================== //

    public void DoStartFadeIn(float fDuration, EventDelegate.Callback OnEndFade)
    {
        _pUITexture.enabled = true;
        _pTweenAlpha.ResetToBeginning();
        _pTweenAlpha.from = 0f;
        _pTweenAlpha.to = 1f;
        _pTweenAlpha.duration = fDuration;
        _pTweenAlpha.PlayForward();
        EventDelegate.Add(_pTweenAlpha.onFinished, OnEndFade, true);
    }

    public void DoStartFadeOut(float fDuration, EventDelegate.Callback OnEndFade)
    {
        _pUITexture.enabled = true;
        _pTweenAlpha.ResetToBeginning();
        _pTweenAlpha.from = 1f;
        _pTweenAlpha.to = 0f;
        _pTweenAlpha.duration = fDuration;
        _pTweenAlpha.PlayForward();
        EventDelegate.Add(_pTweenAlpha.onFinished, OnEndFade, true);
    }

    public void DoStartFadeInOut()
    {

    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        _pUITexture = GetComponentInChildren<UITexture>();
        _pTweenAlpha = GetComponentInChildren<TweenAlpha>();

        if (_pTexture == null)
        {
            Texture2D pTexture = new Texture2D(1, 1);
            pTexture.SetPixel(0, 0, Color.black);
            pTexture.Apply();

            _pUITexture.mainTexture = pTexture;
            _pUITexture.color = Color.black;
        }
        else
            _pUITexture.mainTexture = _pTexture;
    }

    void Reset()
    {
        if(Application.isPlaying == false)
        {
            GameObject pObjectChild = new GameObject();
            pObjectChild.transform.parent = transform;
            _pUITexture = pObjectChild.AddComponent<UITexture>();
            _pTweenAlpha = pObjectChild.AddComponent<TweenAlpha>();

            UIRoot pRoot = FindObjectOfType<UIRoot>();
            _pUITexture.width = pRoot.manualWidth;
            _pUITexture.height = pRoot.manualHeight;

            EventDelegate.Add(_pTweenAlpha.onFinished, OnEndTween);
            GetComponent<UIPanel>().depth = 100;
            _pTweenAlpha.enabled = false;
        }
    }

    // ========================== [ Division ] ========================== //

    private void OnEndTween()
    {
        _pUITexture.enabled = false;
    }
}
