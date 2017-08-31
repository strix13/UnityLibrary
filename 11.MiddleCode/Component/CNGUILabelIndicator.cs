using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class CNGUILabelIndicator : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EDirection
	{
		Up,
		Left,
		Right,
		Down
	}

	public class SInfoIndicator
	{
		public int iFontSize;
		public Color sColor;
		public Vector2 v2Direction;
		public float fDistance;
		public float fDuration;
		public float fFadeDelay;
		public float fFadeDuration;

		public System.Action OnDisable;
	}

	/* public - Variable declaration            */

	[Header("라벨 크기")] public int p_iFontSize;
	[Header("라벨 이동방향")] public EDirection p_eDirection;
	[Header("라벨 이동거리")] public float p_fDistance;
	[Header("라벨 이동속도")] public float p_fDuration;
	[Header("라벨 사라지는시간")] public float p_fFadeDelay;
	[Header("라벨 사라지는속도")] public float p_fFadeDuration;
	[Header( "라벨 색상" )] public Color p_sColorIndicator = Color.white;
	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CManagerPooling<ELabelIndicator, CLabelIndicator> _pManagerPool_LabelIndicator;
	private SInfoIndicator _sInfoIndicator = new SInfoIndicator();

	private UILabel _pUILabelInfo;
	private Transform _pTrans_Label;
	private Transform _pTrans_Manager;

	private int _iLastDepth;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoStartTween_Indicator(string strText, string strFormat = "{0}")
	{
		CLabelIndicator pResource = _pManagerPool_LabelIndicator.DoPop(ELabelIndicator.Label_Indicator);
		pResource.p_pTransCached.parent = _pTrans_Label;
		pResource.p_pTransCached.DoResetTransform();
		pResource.DoStartTween_Indicator( string.Format( strFormat, strText ), ++_iLastDepth, _sInfoIndicator.sColor, _sInfoIndicator );
	}

	public void DoStartTween_Indicator( Color pColor, string strText, string strFormat = "{0}" )
	{
		CLabelIndicator pResource = _pManagerPool_LabelIndicator.DoPop( ELabelIndicator.Label_Indicator );
		pResource.p_pTransCached.parent = _pTrans_Label;
		pResource.p_pTransCached.DoResetTransform();
		pResource.DoStartTween_Indicator( string.Format( strFormat, strText ), ++_iLastDepth, pColor, _sInfoIndicator );
	}


	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void OnPushResource_LabelIndicator()
	{
		_iLastDepth--;
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUILabelInfo = GetComponent<UILabel>();
		if (_pUILabelInfo == null)
		{
			Debug.LogWarning("라벨이 없습니다.");
			return;
		}

		_pManagerPool_LabelIndicator = CManagerPooling<ELabelIndicator, CLabelIndicator>.instance;
		_pTrans_Manager = _pManagerPool_LabelIndicator.p_pObjectManager.transform;
		_pTrans_Label = _pUILabelInfo.transform;
		_iLastDepth = _pUILabelInfo.depth;

		_sInfoIndicator.v2Direction = ProcGetEnumToDirection( p_eDirection );
		_sInfoIndicator.iFontSize = p_iFontSize;
		_sInfoIndicator.sColor = p_sColorIndicator;
		_sInfoIndicator.fDistance = p_fDistance;
		_sInfoIndicator.fDuration = p_fDuration;
		_sInfoIndicator.fFadeDelay = p_fFadeDelay;
		_sInfoIndicator.fFadeDuration = p_fFadeDuration;
		_sInfoIndicator.OnDisable = OnPushResource_LabelIndicator;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private Vector2 ProcGetEnumToDirection(EDirection eDirection)
	{
		switch (eDirection)
		{
			case EDirection.Up:
				return Vector2.up;
			case EDirection.Left:
				return Vector2.left;
			case EDirection.Right:
				return Vector2.right;
			case EDirection.Down:
				return Vector2.down;
			default:
				return Vector2.zero;
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
