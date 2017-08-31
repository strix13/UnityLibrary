using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK.Protocol;

public class PCManagerScenePrologue : PCManagerSceneBase<PCManagerScenePrologue>
{
	public enum eResLogin
	{
		Success, //로그인 성공
		None, //없는 아이디
	}
	public enum eResCreateAccount
	{
		Success, //아이디 생성 성공
		Overlap, //아이디 중복
	}

	public enum eUIState
	{
		Prolog,     //세계관 설명
		TalkBox,    //대화창
	}

	public eUIState m_eUIState; //디폴트 값은 인스펙터에서

	private Vector3 m_vecDest = new Vector3( 0f, 0f, 0f );
	private int m_nPage = 0;
	private int m_nPage_Total = 5;

	public GameObject m_goProlog;
	public Transform m_trProlog;
	private float m_fPrologSpeed = 0.2f;

	private bool m_bLogin = false;//로그인 여부

	void Awake()
	{
		UpdateUI();
	}

	public void UpdateUI()
	{
		switch (m_eUIState)
		{
			case eUIState.Prolog:
				m_trProlog.localPosition = new Vector3( 0f, -640f, 0f );
				m_goProlog.SetActive( true );
				break;
			case eUIState.TalkBox:
				TalkBox.Show( true, TalkBox.eUIState.Solo, this.transform );

				Queue<string> arrContent = new Queue<string>();
				arrContent.Enqueue( "대화창이 정상적으로 작동하느지 테스트대화창이 정상적으로 작동하느지 테스트" );
				arrContent.Enqueue( "대화창이 정상적으로 작동하느지 테스트" );
				arrContent.Enqueue( "대화창이 정상적으로 작동하느지 테스트" );

				Queue<string> arrAnim = new Queue<string>();
				arrAnim.Enqueue( "idle" );
				arrAnim.Enqueue( "idle" );
				arrAnim.Enqueue( "smile" );
				TalkBox.m_This.SetTalk( arrContent, arrAnim );

				m_goProlog.SetActive( false );
				break;
		}
	}

	IEnumerator Pop_TalkBox()
	{
		yield return new WaitForSeconds( 1f );
		m_eUIState = eUIState.TalkBox;
		UpdateUI();
	}

	public void SelectChar_Accept()
	{
		Destroy( this.gameObject );
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
	}

	public void Prolog_Skip()
	{
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
	}

	public void Update()
	{
		if (m_eUIState == eUIState.Prolog)
		{
			m_trProlog.Translate( Vector3.up * m_fPrologSpeed * Time.deltaTime );
		}
	}
}