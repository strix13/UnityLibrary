using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class TalkBox : MonoBehaviour {

    public enum eUIState
    {
        Left,
        Right,
        Solo,
    }

    public static TalkBox     m_This;

    public UISprite     m_usTalkBox;        //대화창 이미지
    public UILabel      m_ulTalkBox;        //대화창 내용

    public UISprite     m_usNameBoxLeft;    //왼쪽 이름
    public UILabel      m_ulNameBoxLeft;    //왼쪽 이름
    public UISprite     m_usNameBoxRight;   //왼쪽 이름
    public UILabel      m_ulNameBoxRight;   //오른쪽 이름

    private string      m_strTotal = "";    //대화내용전문
    private int         m_nTalkNum = 0;     //대화내용중에 어디까지 나왔는지
    private bool        m_bTalk = false;     //말이 나오고있는 중인지
    private eUIState    m_eUIState = eUIState.Left;

    public SkeletonAnimation m_animChar;

    private Queue<string> m_arrContent;
    private Queue<string> m_arrAnim;

    public static void Show(bool bShow, eUIState eState = eUIState.Left, Transform transform = null)
    {

        if (!bShow)
        {
            if (m_This == null)
            {
                return;
            }
            Destroy(m_This.gameObject);
        }
        else
        {
            Object res = Resources.Load("Prefabs/TalkBox");

            GameObject root = GameObject.Instantiate(res) as GameObject;
            root.transform.parent = transform;
            root.transform.localPosition = new Vector3(0f, 0f, 0f);
            root.transform.localScale = Vector3.one;

            m_This = root.GetComponent<TalkBox>();

            m_This.m_eUIState = eState;
            m_This.UpdateUI();
        }
    }

    public void SetTalk(Queue<string> arrContent, Queue<string> arrAnim)
    {
        m_This.m_arrContent = arrContent;
        m_arrAnim = arrAnim;

        m_animChar.loop = true;
        m_animChar.AnimationName = m_arrAnim.Dequeue();

        m_This.m_strTotal = m_This.m_arrContent.Dequeue();
        m_bTalk = true;

    }

    void UpdateUI()
    {
        if (m_bTalk == false) return;

        if (m_nTalkNum < m_strTotal.Length - 1)
        {
            m_bTalk = true;
            m_ulTalkBox.text = "";

            switch (m_eUIState)
            {
                case eUIState.Left:
                    m_usNameBoxLeft.gameObject.SetActive(true);
                    m_usNameBoxRight.gameObject.SetActive(false);
                    break;
                case eUIState.Right:
                    m_usNameBoxLeft.gameObject.SetActive(false);
                    m_usNameBoxRight.gameObject.SetActive(true);
                    break;
                case eUIState.Solo:
                    m_usNameBoxLeft.gameObject.SetActive(true);
                    m_usNameBoxRight.gameObject.SetActive(false);
                    break;
            }
        }
        
    }
	
	void Update () {

        if (m_bTalk == true)
        {
            if (m_nTalkNum <= m_strTotal.Length - 1)
            {
                m_ulTalkBox.text += m_strTotal.Substring(m_nTalkNum, 1);
                ++m_nTalkNum;
                //Debug.LogFormat("strContent : {0}", strContent);
            }
            else
            {
                m_bTalk = false;
            }
        }


	}

    public void TouchTalkBox()
    {
        if (m_arrContent.Count > 0)
        {
            m_bTalk = true;
            m_nTalkNum = 0;
            m_ulTalkBox.text = "";
            m_This.m_strTotal = m_arrContent.Dequeue();
            m_animChar.AnimationName = m_arrAnim.Dequeue();
        }
        else
        {

            Show(false);

            if (PCManagerScenePrologue.instance != null)
                PCManagerScenePrologue.instance.UpdateUI();
        }

        if (m_eUIState == eUIState.Left) m_eUIState = eUIState.Right; else if (m_eUIState == eUIState.Right) m_eUIState = eUIState.Left;

        UpdateUI();
    }

    public void Skip()
    {
        m_bTalk = true;
        m_nTalkNum = 0;
        PCManagerScenePrologue.instance.UpdateUI();
        Show(false);
    }
}
