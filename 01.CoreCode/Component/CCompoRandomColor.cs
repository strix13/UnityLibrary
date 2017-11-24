using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoRandomColor : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public Color _pColorRandom_Min = Color.green;
	public Color _pColorRandom_Max = Color.white;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

#if NGUI
	private UIWidget _pWidget;
#endif
	private MeshRenderer _pRenderer_Mesh;
	private SpriteRenderer _pRenderer_Sprite;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

#if NGUI
		_pWidget = GetComponentInChildren<UIWidget>( true );
#endif
		_pRenderer_Sprite = GetComponentInChildren<SpriteRenderer>(true);
		_pRenderer_Mesh = GetComponentInChildren<MeshRenderer>(true);
	}

	protected override void OnPlayEventMain()
	{
		base.OnPlayEventMain();

		float fRandomR = Random.Range( _pColorRandom_Min.r, _pColorRandom_Max.r );
		float fRandomG = Random.Range( _pColorRandom_Min.g, _pColorRandom_Max.g );
		float fRandomB = Random.Range( _pColorRandom_Min.b, _pColorRandom_Max.b );
		float fRandomA = Random.Range( _pColorRandom_Min.a, _pColorRandom_Max.a );

		Color pColorRandom = new Color( fRandomR, fRandomG, fRandomB, fRandomA );
		if (_pRenderer_Sprite != null)
			_pRenderer_Sprite.color = pColorRandom;

		if (_pRenderer_Mesh != null)
			_pRenderer_Mesh.material.color = pColorRandom;

#if NGUI
		if (_pWidget != null)
			_pWidget.color = pColorRandom;
#endif
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
