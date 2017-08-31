using UnityEngine;
using System.Collections;

public class Roulette_Circle : MonoBehaviour {
	float             f_Event_Percent=0;            //------------------연출 확률
	float             f_Quaternion_value=0f;        //------------------룰렛 스톱각도
	float             f_Quaternion_Speed=0f;        //------------------룰렛 회전 속도
	float             f_Quaternion_retardation=0f;  //------------------감속수치

	int               i_Quaternion_Count=0;         //------------------룰렛 구간
	int               i_MiniGame=0;

	bool              b_Start_Stop=false;           //------------------룰렛 회전 속도 서서히 줄어들음
	public bool       b_Roulette = false;           //-----------true면 골드룰렛, false면 일반룰렛

	public GameObject g_Roulette_Button=null;
	public GameObject g_RouletteScene;
	public GameObject g_ChipRouletteScene;


	void Awake()
	{
		if (g_Roulette_Button != null) 
		{
			g_Roulette_Button.GetComponent<BoxCollider> ().enabled = true;
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (f_Quaternion_value > 0 && f_Quaternion_value <= 144) {//미션
			i_Quaternion_Count = 1;
		} else if (f_Quaternion_value > 144 && f_Quaternion_value <= 252) {//러쉬
			i_Quaternion_Count = 2;
		} else if (f_Quaternion_value > 252 && f_Quaternion_value <= 288) {//꽝
			i_Quaternion_Count = 3;
		} else if (f_Quaternion_value > 288 && f_Quaternion_value <= 342) {//미니게임
			i_Quaternion_Count = 4;
		} else if (f_Quaternion_value > 342 && f_Quaternion_value <= 360) {//칩
			i_Quaternion_Count = 5;
		}

		f_Quaternion_Speed -= f_Quaternion_retardation;//-------------------------회전
		transform.rotation = Quaternion.Euler(0,0,f_Quaternion_Speed);
	
		if (b_Start_Stop)
		{
			if (transform.eulerAngles.z >= f_Quaternion_value - 3 && transform.eulerAngles.z <= f_Quaternion_value + 3) 
			{	
				b_Start_Stop = false;	
				f_Quaternion_Speed = f_Quaternion_value;//--정해진 각도로 정지
				f_Quaternion_retardation = 0;

				if (i_Quaternion_Count == 1 || i_Quaternion_Count == 3 || i_Quaternion_Count == 5 || f_Event_Percent<=50 && i_Quaternion_Count == 2 || f_Event_Percent<=50 && i_Quaternion_Count == 4)
				{
					StartCoroutine ("SceneRotate");
				} else if(i_Quaternion_Count == 2 || i_Quaternion_Count == 4)
				{
					StartCoroutine ("RouletteEvent");
				}
			}
		}
	}

	public void Roulette_On()//--------------------------------------------------------------------------------------룰렛 돌리기
	{
		g_Roulette_Button.GetComponent<BoxCollider> ().enabled = false;
		f_Event_Percent = Random.Range (0,100+1);//----------연출 확률 //★확률★
		f_Quaternion_value = Random.Range (0,360+1);//★확률★
		f_Quaternion_retardation=10;
		StartCoroutine ("RouletteStop");
	}

	IEnumerator RouletteStop()//--------------------------------------------------------------------------------------룰렛 감속
	{
		yield return new WaitForSeconds (3f);
		f_Quaternion_retardation = 8;
		yield return new WaitForSeconds (0.5f);
		f_Quaternion_retardation = 6;
		yield return new WaitForSeconds (0.5f);
		f_Quaternion_retardation = 4;
		yield return new WaitForSeconds (0.5f);
		f_Quaternion_retardation = 2;
		yield return new WaitForSeconds (0.5f);
		f_Quaternion_retardation = 1;
		b_Start_Stop = true;
	}

	IEnumerator RouletteEvent()//--------------------------------------------------------------------------------------룰렛 연출
	{
		yield return new WaitForSeconds (0f);
		f_Quaternion_retardation = 0.1f;
		yield return new WaitForSeconds (1.5f);
		f_Quaternion_retardation = -0.1f;
		yield return new WaitForSeconds (1.5f);
		if (transform.eulerAngles.z >= f_Quaternion_value - 3 && transform.eulerAngles.z <= f_Quaternion_value + 3) 
		{
			b_Start_Stop = false;	
			f_Quaternion_Speed = f_Quaternion_value;//--정해진 각도로 정지
			f_Quaternion_retardation = 0;
			StartCoroutine ("SceneRotate");
		}
	}

	IEnumerator SceneRotate()//---------------------------------------------------------------------------------------룰렛씬 OFF
	{
		yield return new WaitForSeconds (5f);
		g_Roulette_Button.GetComponent<BoxCollider> ().enabled = true;
		transform.rotation = Quaternion.Euler (0, 0, 0);

		if (i_Quaternion_Count == 1) {//---------------------------------------------------골드
			Debug.Log("z");
			g_RouletteScene.SetActive (false);
//			g_ChipRouletteScene.SetActive (true);
		} else if (i_Quaternion_Count == 2) {//--------------------------------------------훈련
			Debug.Log("x");
			i_MiniGame = Random.Range (1, 5 + 1);
			if (i_MiniGame == 1) 
			{
				
			} else if (i_MiniGame == 2) {
				
			} else if (i_MiniGame == 3) {
				
			} else if (i_MiniGame == 4) {

			} else if (i_MiniGame == 5) {

			}
			g_RouletteScene.SetActive (false);
		} else if (i_Quaternion_Count == 3) {//--------------------------------------------참가권
			Debug.Log("c");
			g_RouletteScene.SetActive (false);
		} else if (i_Quaternion_Count == 4) {//--------------------------------------------미션
			Debug.Log("v");
			g_RouletteScene.SetActive (false);
		} else if (i_Quaternion_Count == 5) {//--------------------------------------------러쉬
			Debug.Log("b");
			g_RouletteScene.SetActive (false);
		} 
	}
}
