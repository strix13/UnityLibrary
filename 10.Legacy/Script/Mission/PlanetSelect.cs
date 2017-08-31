using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Lean.Touch
{
	public class PlanetSelect : MonoBehaviour
	{
		public static PlanetSelect   instance;
		bool[]                       bs_ButtonActive;//-------------버튼 상태 받는 불값 

		int                          i_Solution=0;//----------------버튼이벤트 활성화 코드
		public int                   i_UIPosition=0;//--------------i_Solution과 같으면 버튼이벤트 활성화

		public GameObject            g_LookAt;//--------------------타겟 오브젝트 유아이 최상위 오브젝트가 쳐다봄
		public GameObject[]          gs_CharacterImage;//-----------캐릭터 이미지 오브젝트

		void Awake()
		{
			instance = this;
			Array.Resize(ref bs_ButtonActive,6);//------------------배열 초기화

		}

		void Update()
		{
			transform.LookAt (g_LookAt.transform.position);

			for (int i = 0; i < 6; i++) 
			{
				if (!bs_ButtonActive [i]) 
				{
					Color rgb = gs_CharacterImage [i].GetComponent<UISprite> ().color;
					rgb.r = 0.5f;
					rgb.g = 0.5f;
					rgb.b = 0.5f;
					gs_CharacterImage [i].GetComponent<UISprite> ().color = rgb;
				}else {
					Color rgb = gs_CharacterImage [i].GetComponent<UISprite> ().color;
					rgb.r = 1f;
					rgb.g = 1f;
					rgb.b = 1f;
					gs_CharacterImage [i].GetComponent<UISprite> ().color = rgb;
				}
			}

			if (i_UIPosition == 0) {
				g_LookAt.transform.position = Vector3.MoveTowards (g_LookAt.transform.position, new Vector3(0,0.79f,1), 10*Time.deltaTime);
				bs_ButtonActive [0] = true;
				bs_ButtonActive [1] = false;
				bs_ButtonActive [2] = false;
				bs_ButtonActive [3] = false;
				bs_ButtonActive [4] = false;
				bs_ButtonActive [5] = false;
				gs_CharacterImage [0].GetComponent<UISprite> ().depth = 6;
				gs_CharacterImage [1].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [2].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [3].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [4].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [5].GetComponent<UISprite> ().depth = 3;
			} else if (i_UIPosition == 1) {
				g_LookAt.transform.position = Vector3.MoveTowards (g_LookAt.transform.position, new Vector3(1.602f,0.79f,0.927f),10*Time.deltaTime);
				bs_ButtonActive [0] = false;
				bs_ButtonActive [1] = false;
				bs_ButtonActive [2] = true;
				bs_ButtonActive [3] = false;
				bs_ButtonActive [4] = false;
				bs_ButtonActive [5] = false;
				gs_CharacterImage [0].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [1].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [2].GetComponent<UISprite> ().depth = 6;
				gs_CharacterImage [3].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [4].GetComponent<UISprite> ().depth = 3;
				gs_CharacterImage [5].GetComponent<UISprite> ().depth = 4;
			} else if (i_UIPosition == 2) {
				g_LookAt.transform.position = Vector3.MoveTowards (g_LookAt.transform.position, new Vector3(1.602f,0.79f,-0.94f),10*Time.deltaTime);
				bs_ButtonActive [0] = false;
				bs_ButtonActive [1] = false;
				bs_ButtonActive [2] = false;
				bs_ButtonActive [3] = true;
				bs_ButtonActive [4] = false;
				bs_ButtonActive [5] = false;
				gs_CharacterImage [0].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [1].GetComponent<UISprite> ().depth = 3;
				gs_CharacterImage [2].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [3].GetComponent<UISprite> ().depth = 6;
				gs_CharacterImage [4].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [5].GetComponent<UISprite> ().depth = 5;
			} else if (i_UIPosition == 3) {
				g_LookAt.transform.position = Vector3.MoveTowards (g_LookAt.transform.position, new Vector3(0,0.79f,-0.94f),10*Time.deltaTime);
				bs_ButtonActive [0] = false;
				bs_ButtonActive [1] = false;
				bs_ButtonActive [2] = false;
				bs_ButtonActive [3] = false;
				bs_ButtonActive [4] = false;
				bs_ButtonActive [5] = true;
				gs_CharacterImage [0].GetComponent<UISprite> ().depth = 3;
				gs_CharacterImage [1].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [2].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [3].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [4].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [5].GetComponent<UISprite> ().depth = 6;
			} else if (i_UIPosition == 4) {
				g_LookAt.transform.position = Vector3.MoveTowards (g_LookAt.transform.position, new Vector3(-1.64f,0.79f,-0.94f),10*Time.deltaTime);
				bs_ButtonActive [0] = false;
				bs_ButtonActive [1] = false;
				bs_ButtonActive [2] = false;
				bs_ButtonActive [3] = false;
				bs_ButtonActive [4] = true;
				bs_ButtonActive [5] = false;
				gs_CharacterImage [0].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [1].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [2].GetComponent<UISprite> ().depth = 3;
				gs_CharacterImage [3].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [4].GetComponent<UISprite> ().depth = 6;
				gs_CharacterImage [5].GetComponent<UISprite> ().depth = 5;
			} else if (i_UIPosition == 5) {
				g_LookAt.transform.position = Vector3.MoveTowards (g_LookAt.transform.position, new Vector3(-1.64f,0.79f,0.932f),10*Time.deltaTime);
				bs_ButtonActive [0] = false;
				bs_ButtonActive [1] = true;
				bs_ButtonActive [2] = false;
				bs_ButtonActive [3] = false;
				bs_ButtonActive [4] = false;
				bs_ButtonActive [5] = false;
				gs_CharacterImage [0].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [1].GetComponent<UISprite> ().depth = 6;
				gs_CharacterImage [2].GetComponent<UISprite> ().depth = 4;
				gs_CharacterImage [3].GetComponent<UISprite> ().depth = 3;
				gs_CharacterImage [4].GetComponent<UISprite> ().depth = 5;
				gs_CharacterImage [5].GetComponent<UISprite> ().depth = 4;

			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSwipe += OnFingerSwipe;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSwipe -= OnFingerSwipe;
		}

		public void OnFingerSwipe(LeanFinger finger)
		{
			var swipe = finger.SwipeScreenDelta;

			if (swipe.x < -Mathf.Abs(swipe.y))//왼쪽
			{
				if (i_UIPosition != 5) {
					i_Solution++;
					i_UIPosition++;
				} else {
					i_Solution = 0;
					i_UIPosition = 0;
				}
			}

			if (swipe.x > Mathf.Abs(swipe.y))//오른쪽
			{
				if (i_UIPosition != 0) {
					i_Solution--;
					i_UIPosition--;
				} else {
					i_Solution = 5;
					i_UIPosition = 5;
				}
			}

			if (swipe.y < -Mathf.Abs(swipe.x))//아래
			{
			}

			if (swipe.y > Mathf.Abs(swipe.x))//위
			{
			}
		}

		public void CreateButton(GameObject Character)
		{
			int i_ButtonNum = (int)Character.GetComponent<CharacterButton> ().eCharacterName;

			if (i_Solution == i_ButtonNum) 
			{
				Debug.Log (i_ButtonNum);
				PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
			}
		}

		public void LeftButton()
		{
			if (i_UIPosition != 0) {
				i_Solution--;
				i_UIPosition--;
			} else {
				i_Solution=5;
				i_UIPosition = 5;
			}
		}

		public void RightButton()
		{
			if (i_UIPosition != 5) {
				i_Solution++;
				i_UIPosition++;
			} else {
				i_Solution=0;
				i_UIPosition = 0;
			}
		}
	}
}