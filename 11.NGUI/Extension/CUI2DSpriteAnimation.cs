using UnityEngine;
using System.Collections;

public class CUI2DSpriteAnimation : CObjectBase
{
    public event System.Action p_EVENT_OnEndAnim;

    private System.Action _OnMatchFrameIndex;
    private System.Action _OnFinishAnim;

    private int _iFrameIndex_Event;
    private BoxCollider _pBoxCollider;


    public bool PlayOnAwake = false;

    /// <summary>
    /// Index of the current frame in the sprite animation.
    /// </summary>

    public int frameIndex = 0;

    /// <summary>
    /// How many frames there are in the animation per second.
    /// </summary>

    [SerializeField]
    protected int framerate = 20;

    /// <summary>
    /// Should this animation be affected by time scale?
    /// </summary>

    public bool ignoreTimeScale = true;

    /// <summary>
    /// Should this animation be looped?
    /// </summary>

    public bool loop = true;

    /// <summary>
    /// Actual sprites used for the animation.
    /// </summary>

    public UnityEngine.Sprite[] frames;

    UnityEngine.SpriteRenderer mUnitySprite;
    UI2DSprite mNguiSprite;
    float mUpdate = 0f;

    /// <summary>
    /// Returns is the animation is still playing or not
    /// </summary>

    public bool isPlaying { get { return enabled; } }

    /// <summary>
    /// Animation framerate.
    /// </summary>

    public int framesPerSecond { get { return framerate; } set { framerate = value; } }

    /// <summary>
    /// Continue playing the animation. If the animation has reached the end, it will restart from beginning
    /// </summary>

    public void Play()
    {
        _iFrameIndex_Event = -1;
        if (frames != null && frames.Length > 0)
        {
            if (!enabled && !loop)
            {
                int newIndex = framerate > 0 ? frameIndex + 1 : frameIndex - 1;
                if (newIndex < 0 || newIndex >= frames.Length)
                    frameIndex = framerate < 0 ? frames.Length - 1 : 0;
            }

            enabled = true;
            UpdateSprite();
        }

        if (_pBoxCollider == null)
            _pBoxCollider = GetComponent<BoxCollider>();
    }

    public void Play(System.Action OnFinishAnim)
    {
        _OnFinishAnim = OnFinishAnim;
        Play();
    }


    public void Play(int iFrameIndex, System.Action OnMatchFrameIndex)
    {
        Play();
        _iFrameIndex_Event = iFrameIndex;
        _OnMatchFrameIndex = OnMatchFrameIndex;
    }

    /// <summary>
    /// Pause the animation.
    /// </summary>

    public void Pause() { enabled = false; }

    /// <summary>
    /// Reset the animation to the beginning.
    /// </summary>

    public void ResetToBeginning()
    {
        frameIndex = framerate < 0 ? frames.Length - 1 : 0;
        UpdateSprite();
    }

    /// <summary>
    /// Start playing the animation right away.
    /// </summary>

    void Start() { if (PlayOnAwake) Play(); }

    /// <summary>
    /// Advance the animation as necessary.
    /// </summary>

    void Update()
    {
        if (frames == null || frames.Length == 0)
        {
            enabled = false;
        }
        else if (framerate != 0)
        {
            float time = ignoreTimeScale ? RealTime.time : Time.time;

            if (mUpdate < time)
            {
                mUpdate = time;
                int newIndex = framerate > 0 ? frameIndex + 1 : frameIndex - 1;

                if (newIndex == _iFrameIndex_Event)
                {
                    _OnMatchFrameIndex();
                    _OnMatchFrameIndex = null;
                    _iFrameIndex_Event = -1;
                }

                if (!loop && (newIndex < 0 || newIndex >= frames.Length))
                {
                    enabled = false;
                    if (p_EVENT_OnEndAnim != null)
                        p_EVENT_OnEndAnim();

                    if (_OnFinishAnim != null)
                    {
                        _OnFinishAnim();
                        _OnFinishAnim = null;
                    }

                    return;
                }

                frameIndex = NGUIMath.RepeatIndex(newIndex, frames.Length);
                UpdateSprite();
            }
        }
    }

    /// <summary>
    /// Immediately update the visible sprite.
    /// </summary>

    void UpdateSprite()
    {
        if (mUnitySprite == null && mNguiSprite == null)
        {
            mUnitySprite = GetComponent<UnityEngine.SpriteRenderer>();
            mNguiSprite = GetComponent<UI2DSprite>();

            if (mUnitySprite == null && mNguiSprite == null)
            {
                enabled = false;
                return;
            }
        }

        float time = ignoreTimeScale ? RealTime.time : Time.time;
        if (framerate != 0) mUpdate = time + Mathf.Abs(1f / framerate);

        if (mUnitySprite != null)
        {
            mUnitySprite.sprite = frames[frameIndex];

            if (_pBoxCollider != null)
            {
                Vector3 vecSize = mUnitySprite.bounds.size;
                Vector3 vecLocalScale = transform.lossyScale;
                vecSize.x /= vecLocalScale.x;
                vecSize.y /= vecLocalScale.x;
                vecSize.z /= vecLocalScale.y;
                _pBoxCollider.size = new Vector3(vecSize.x, vecSize.z, vecSize.y);
                _pBoxCollider.center = new Vector3(0f, vecSize.z / 2f);
            }

        }
        else if (mNguiSprite != null)
        {
            mNguiSprite.nextSprite = frames[frameIndex];
        }
    }
}
