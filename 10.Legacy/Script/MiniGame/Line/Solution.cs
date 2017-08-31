using UnityEngine;
using System.Collections;
using System;

public class Solution : MonoBehaviour {
	public static Solution           instance;

	public GameObject[]              DotObject;

	public int                       Count;//각 점마다 연결되어야 하는 개수가 일치한 오브젝트의 개수
	public int                       Count_sol;//Count와 Count_sol이 일치하면 정답조건 중 1개 성립
	public int                       Line_sol;// 그림에 총 선이 몇개여야 하는지
	public int                       Horizontal,Vertical,Diagonal,Diagonal2;//--------Diagonal은 / Diagonal2는 \
	public int                       Sol_Horizontal,Sol_Vertical,Sol_Diagonal,Sol_Diagonal2;//가로선,세로선,대각선의 개수를 비교하여 정답판별
	public int                       Num;//배열에 저장한 좌표의 번호

	public Vector3[]                 DotPositions;//LineGM에서 터치한 오브젝트의 좌표를 저장하는 배열

	public bool                      Sol_bool;// 예외처리용 
	public bool                      Dots_bool;//한 점에 두번이상 터치해야할 경우 true

	public GameObject                OK;

	void Awake(){
		instance = this;
		Num = -1;
		Horizontal = 0;
		Vertical = 0;
		Diagonal = 0;
		Diagonal2 = 0;
		Count = 0;
	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		DotObject = GameObject.FindGameObjectsWithTag ("Dot");

		if (Line_sol == LineGM.instance.i_TouchCount - 1)
		{	
			LineGM.instance.b_Success_bool = true;

			if (Count_sol == Count && !Sol_bool && Horizontal == Sol_Horizontal && Vertical == Sol_Vertical && Diagonal == Sol_Diagonal && Diagonal2 == Sol_Diagonal2) {
				Sol_bool = true;
				if (Sol_bool) {
					for (int i = 0; i < LineGM.instance.gs_Dot.Length; i++) {
						LineGM.instance.gs_Dot [i].GetComponent<BoxCollider> ().enabled = false;
					}
				}
				LineGM.instance.lineRenderer.SetVertexCount (LineGM.instance.i_TouchCount);

				for (int i = 0; i < LineGM.instance.i_TouchCount; i++) 
				{
					if (LineGM.instance.i_TouchCount != Line_sol + 1)
						LineGM.instance.lineRenderer.SetPosition (i, LineGM.instance.gs_Dot_Line_Num [i].transform.position);
				}
		
				OK.SetActive (true);
				Debug.Log ("정답");
			}
		} 
	}
}
