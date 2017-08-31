using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;

public class PCCharacterAnimation : CObjectBase, IComparable<PCCharacterAnimation>
{
	public ECharacterName _eCharacterName;
	private CSpineWrapper _pSpine;
	
	public int CompareTo( PCCharacterAnimation other )
	{
		if (_eCharacterName < other._eCharacterName)
			return -1;
		else if (_eCharacterName > other._eCharacterName)
			return 1;
		else
			return 0;
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		_pSpine = GetComponent<CSpineWrapper>();
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		StartCoroutine( RandomAni() );
	}

	IEnumerator RandomAni()
	{
		// Spine은 첫 프레임 시 Init이 덜되어 플레이가 안된다. 한 프레임 대기.
		yield return null;

		while (true)
		{
			_pSpine.DoPlayAnimation( ECharacterAnimationName.idle );

			yield return new WaitForSeconds( 5f );

			_pSpine.DoPlayAnimation( _eCharacterName.GetRandomEnumInGroup<ECharacterAnimationName>() );

			yield return new WaitForSeconds( 3f );
		}
	}
}
