using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CircleRoulette : MonoBehaviour {
	SkeletonAnimation            Ani;

	// Use this for initialization
	void Start () {
		Ani = this.gameObject.GetComponent<SkeletonAnimation> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public IEnumerator Anistart()
	{
		yield return new WaitForSeconds (2f);
		Ani.loop = true;
		Ani.AnimationName = "run";
		yield return new WaitForSeconds (1.88f);
		int RandomNum = Random.Range (0, 4);
		int RandomAni = Random.Range (0, 3);
		if (RandomNum == 0) {
			Ani.loop = false;
			if(RandomAni==0)
		    	Ani.AnimationName = "gold1";
			else if (RandomAni == 2)
				Ani.AnimationName = "gold2";
			else if (RandomAni == 3)
				Ani.AnimationName = "gold3";
			yield return new WaitForSeconds (8.5f);
		} else if (RandomNum == 1) {
			Ani.loop = false;
			if(RandomAni==0)
				Ani.AnimationName = "mission1";
			else if (RandomAni == 2)
				Ani.AnimationName = "mission2";
			else if (RandomAni == 3)
				Ani.AnimationName = "mission3";
			yield return new WaitForSeconds (6.5f);
		} else if (RandomNum == 2) {
			Ani.loop = false;
			if(RandomAni==0)
				Ani.AnimationName = "training1";
			else if (RandomAni == 2)
				Ani.AnimationName = "training2";
			else if (RandomAni == 3)
				Ani.AnimationName = "training3";
			yield return new WaitForSeconds (7.5f);
		} else if (RandomNum == 3) {
			Ani.loop = false;
			if(RandomAni==0)
				Ani.AnimationName = "rush1";
			else if (RandomAni == 2)
				Ani.AnimationName = "rush2";
			else if (RandomAni == 3)
				Ani.AnimationName = "rush3";
			yield return new WaitForSeconds (6f);
		}

		PCUIOutFrame_MainMenu pUIFrame = PCManagerUIOutGame.instance.GetUIFrame<PCUIOutFrame_MainMenu>();
		pUIFrame.g_Tab_bar.SetActive (false);
		this.gameObject.SetActive (false);
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.Rush, 1f, Color.black );
	}
}
