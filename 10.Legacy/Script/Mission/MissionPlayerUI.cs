using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class MissionPlayerUI : MonoBehaviour {
	public static MissionPlayerUI instance;

	SkeletonAnimation skeletonAnimation;


	void Awake()
	{
		instance = this;
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void AnimationMethod(int i)
	{
		if (i == 0) {
			skeletonAnimation.state.AddAnimation (0, "roulette_stand_by", true, 0f);
		} else if (i == 1) {
			skeletonAnimation.state.AddAnimation (0, "roulette_cheer", true, 0f);
		} else if (i == 2) {
			skeletonAnimation.state.AddAnimation (0, "roulette_disappointment", true, 0f);
		} else if (i == 3) {
			skeletonAnimation.state.AddAnimation (0, "roulette_happy", true, 0f);
		}
	}
}
