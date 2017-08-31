using UnityEngine;
using System.Collections;
using System;

public class LineGM : MonoBehaviour 
{
	public static LineGM         instance;

	public LineRenderer          lineRenderer; //라인렌더러
	public Solution              Sol;
//	int                          Sol_Dot_bool_Length;
	public int                   i_TouchCount;//점을 터치한 횟수


	public GameObject[]          gs_Dot_Line_Num;//터치한 오브젝트를 배열에 저장
	public GameObject[]          gs_Dot;//점을 찾아 배열에 저장
	public GameObject[]          gs_Stage;

	public GameObject            g_Solution_Object;

	public bool[]                bs_Dot_bool;//예외처리용
	public bool                  b_Success_bool;//완성후 예외 처리용

	int                          i_Stage_Num;

	void Awake(){	
		instance = this;	
		i_TouchCount  = 0;
		Array.Resize (ref bs_Dot_bool, 0);
		Array.Resize (ref gs_Dot_Line_Num, 0);
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetColors(Color.white, Color.white);
		lineRenderer.SetWidth(0.1f, 0.1f);
	}

	void Start(){

	}

	void Update()
	{
		touchClick();
		g_Solution_Object = GameObject.FindGameObjectWithTag ("Solution");
		Sol = g_Solution_Object.GetComponent<Solution> ();
		gs_Dot = GameObject.FindGameObjectsWithTag ("Dot");

		if (!Sol.Dots_bool) {
			Array.Resize (ref bs_Dot_bool, gs_Dot.Length);
		} else {
			Array.Resize (ref bs_Dot_bool, Sol.Line_sol+1);
		}
		//-----------------------------------------------------------------------라인렌더러 기본 설정

		if (i_Stage_Num == 0) {
			gs_Stage [0].SetActive (true);
			gs_Stage [1].SetActive (false);
			gs_Stage [2].SetActive (false);
		} else if (i_Stage_Num == 1) {
			gs_Stage [1].SetActive (true);
			gs_Stage [0].SetActive (false);
			gs_Stage [2].SetActive (false);
		} else if (i_Stage_Num == 2) 
		{
			gs_Stage [2].SetActive (true);
			gs_Stage [1].SetActive (false);
			gs_Stage [0].SetActive (false);
		}


	}

	void touchClick() {
		lineRenderer = GetComponent<LineRenderer>();
		if(!b_Success_bool)
			lineRenderer.SetVertexCount(i_TouchCount+1);
		
		//-----------------------------------------------------------------------터치 1회, 지속 터치, 터치 끝났을 때 명령
		if(Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch (0);

			if(touch.phase == TouchPhase.Began)
			{				

			}else if(touch.phase == TouchPhase.Moved){
				
				Vector2 touching = Camera.main.ScreenToWorldPoint (touch.position);

				if (i_TouchCount < Sol.Line_sol + 1 && !b_Success_bool) {
					lineRenderer.SetPosition (i_TouchCount, touching);
				}

				Ray touchray = Camera.main.ScreenPointToRay (Input.mousePosition); 
				RaycastHit hit;                                                                                                                                           
				Physics.Raycast (touchray, out hit); 

				if (hit.collider != null) 
				{
					if (!bs_Dot_bool [i_TouchCount])
					{
						bs_Dot_bool [i_TouchCount] = true;
						i_TouchCount += 1;
						Sol.Num += 1;
						Array.Resize (ref gs_Dot_Line_Num, i_TouchCount);
						gs_Dot_Line_Num [i_TouchCount - 1] = hit.collider.gameObject;

						if (i_TouchCount != 1) 
						{
							gs_Dot_Line_Num [i_TouchCount - 2].GetComponent<Quest> ().Count += 1;
							hit.collider.gameObject.GetComponent<Quest> ().Count += 1;
							gs_Dot_Line_Num [i_TouchCount - 2].GetComponent<BoxCollider> ().enabled = true;
						}

						Array.Resize (ref Sol.DotPositions, i_TouchCount);
						Sol.DotPositions [Sol.Num] = hit.collider.gameObject.transform.position;

						if (Sol.DotPositions.Length>1)
						{
							if (((Sol.DotPositions [Sol.Num - 1].x < Sol.DotPositions [Sol.Num].x) && (Sol.DotPositions [Sol.Num - 1].y < Sol.DotPositions [Sol.Num].y)) || ((Sol.DotPositions [Sol.Num - 1].x > Sol.DotPositions [Sol.Num].x) && (Sol.DotPositions [Sol.Num - 1].y > Sol.DotPositions [Sol.Num].y))) {
								//Debug.Log ("↗대각선");
								Sol.Diagonal++;
							} else if (((Sol.DotPositions [Sol.Num - 1].x > Sol.DotPositions [Sol.Num].x) && (Sol.DotPositions [Sol.Num - 1].y < Sol.DotPositions [Sol.Num].y)) || ((Sol.DotPositions [Sol.Num - 1].x < Sol.DotPositions [Sol.Num].x) && (Sol.DotPositions [Sol.Num - 1].y > Sol.DotPositions [Sol.Num].y))) {
								//Debug.Log ("↘대각선");
								Sol.Diagonal2++;
							} else if ((Sol.DotPositions [Sol.Num - 1].x == Sol.DotPositions [Sol.Num].x) && (Sol.DotPositions [Sol.Num - 1].y != Sol.DotPositions [Sol.Num].y)) {
								//Debug.Log ("세로");
								Sol.Vertical++;
							} else if ((Sol.DotPositions [Sol.Num - 1].x != Sol.DotPositions [Sol.Num].x) && (Sol.DotPositions [Sol.Num - 1].y == Sol.DotPositions [Sol.Num].y)) {
								//Debug.Log ("가로");
								Sol.Horizontal++;
							}
						}
							hit.collider.gameObject.GetComponent<BoxCollider> ().enabled = false;
						lineRenderer.SetPosition (i_TouchCount - 1, hit.collider.gameObject.transform.position);
					}
				}	
			}
			else if(touch.phase == TouchPhase.Ended)
			{
				for (int i = 0; i < gs_Dot.Length; i++) 
				{
					gs_Dot [i].GetComponent<BoxCollider> ().enabled = true;
					gs_Dot [i].GetComponent<Quest> ().Compare = false;
					gs_Dot [i].GetComponent<Quest> ().Count = 0;
				}
				Array.Resize (ref bs_Dot_bool, 0);
				Array.Resize (ref gs_Dot_Line_Num, 0);
				i_TouchCount = 0; 
				Sol.Num = -1;
				Array.Resize (ref Sol.DotPositions, 0);
				Sol.Sol_bool = false;
				Sol.Horizontal = 0;
				Sol.Vertical = 0;
				Sol.Diagonal = 0;
				Sol.Diagonal2 = 0;
				Sol.Count = 0;
				b_Success_bool = false;
			}
		}
	}


	public void Next()
	{
		if(i_Stage_Num !=2)
			i_Stage_Num++;
		g_Solution_Object = GameObject.FindGameObjectWithTag ("Solution");
		Sol.OK.SetActive (false);
		Debug.Log (i_Stage_Num);
	}

	public void Back()
	{
		if(i_Stage_Num != 0)
			i_Stage_Num--;
		Sol.OK.SetActive (false);
		Debug.Log (i_Stage_Num);
	}
}
