using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PCManagerSceneRoulette : PCManagerSceneBase<PCManagerSceneRoulette>
{
	public GameObject g_Reel1, g_Reel2, g_Reel3;
	public GameObject g_Light, g_Light1, g_Light2;
	CSpineWrapper Ani, Reel1, Reel2, Reel3;

	bool _bIsReady;

	// Use this for initialization
	void Start()
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Ani = this.gameObject.GetComponent<CSpineWrapper>();
		Reel1 = g_Reel1.GetComponent<CSpineWrapper>();
		Reel2 = g_Reel2.GetComponent<CSpineWrapper>();
		Reel3 = g_Reel3.GetComponent<CSpineWrapper>();
		StartCoroutine( Run() );
	}

	IEnumerator Run()
	{
		yield return new WaitForSeconds( 2f );
		Ani.DoPlayAnimation( "run", true );
		Reel1.DoPlayAnimation( "slotrun", true );
		Reel2.DoPlayAnimation( "slotrun", true );
		Reel3.DoPlayAnimation( "slotrun", true );
		_bIsReady = true;
	}

	public void Button()
	{
		if (_bIsReady)
		{
			StartCoroutine( Result() );
			_bIsReady = false;
		}
	}

	IEnumerator Result()
	{
		g_Light.SetActive( true );
		int RandomAni = Random.Range( 1, 5 );
		Reel1.DoPlayAnimation( string.Format("reina{0}", RandomAni), false );

		yield return new WaitForSeconds( 0.5f );
		g_Light1.SetActive( true );
		RandomAni = Random.Range( 1, 5 );
		Reel2.DoPlayAnimation( string.Format( "reina{0}", RandomAni ), false );

		yield return new WaitForSeconds( 0.5f );
		g_Light2.SetActive( true );
		RandomAni = Random.Range( 1, 5 );
		Reel3.DoPlayAnimation( string.Format( "reina{0}", RandomAni ), false );

		yield return new WaitForSeconds( 3f );
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.OutGame, 1f, Color.black );
	}
}
