using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCManagerSceneTitle : PCManagerSceneBase<PCManagerSceneTitle>
{
	public void NextScene()
	{
		PCManagerFramework.DoLoadScene_FadeInOut( ESceneName.Start, 1f, Color.black );

		Debug.LogWarning("Scene Move " + Time.time);
	}

	private void OnFinishLoad_Common()
	{
		PCManagerFramework.p_pManagerSound.DoPlayBGM( ESoundName.BGM_Title_LenskoCetus, null );
	}
}
