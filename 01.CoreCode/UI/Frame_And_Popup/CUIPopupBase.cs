using UnityEngine;
using System.Collections;
using System;

public class CUIPopupBase : CUIFrameBase
{
	/*
	private enum EPopupAnimType
	{
		Instant,
		TweenAlpha
	}

	[SerializeField] private EPopupAnimType _eAnimType = EPopupAnimType.Instant;

	private Component _pCompoCached = null;
	private Type _pTypeCached = null;

	protected override void OnAwake()
	{
		base.OnAwake();

		AddTweenComponent(_eAnimType);

		switch (_eAnimType)
		{
			case EPopupAnimType.TweenAlpha:
				GetTweenCurrent<TweenAlpha>().from = 0;
				break;
		}
	}

	protected override void OnHide()
	{
		base.OnHide();

		switch (_eAnimType)
		{
			case EPopupAnimType.TweenAlpha:
				GetTweenCurrent<TweenAlpha>().from = 0;
				GetTweenCurrent<TweenAlpha>().to = 1;
				GetTweenCurrent<TweenAlpha>().ResetToBeginning();

				break;
		}
	}

	private void AddTweenComponent<ETween>(ETween eTween)
	{
		string strType = eTween.ToString();
		_pTypeCached = Type.GetType(strType);

		if (_pTypeCached == null || _pTypeCached.BaseType == null || _pTypeCached.BaseType != typeof(UITweener)) return;

		_pCompoCached = _pGameObjectCached.AddComponent(_pTypeCached);
	}

	public Component GetTweenCurrent<Component>()
	{
		return (Component)Convert.ChangeType(_pCompoCached, _pTypeCached);
	}
	*/
}
