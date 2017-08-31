using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCManagerSceneCommon : CSingletonBase<PCManagerSceneCommon>
{
	public GameObject                 g_Exit;
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.Escape)) 
		{
			g_Exit.SetActive (true);
		}
	}

	public void Cancle()
	{
		g_Exit.SetActive (false);
	}

	public void Exit()
	{
		Application.Quit ();
	}
}
