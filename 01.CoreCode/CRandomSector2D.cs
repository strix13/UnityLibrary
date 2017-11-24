using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Description : 
   Edit Log    : 
   ============================================ */

public class CRandomSector2D : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum ESectorArroundCheckOption
	{
		None,
		//Side,
		//Up_And_Down,
		FourWay,
		EightWay,
	}


	public struct SSectorIndex
	{
		public int iX;
		public int iY;

		public SSectorIndex(int iX, int iY)
		{
			this.iX = iX;
			this.iY = iY;
		}

		public bool CheckIsEqual(ref SSectorIndex pTargetSectorIndex)
		{
			return (iX == pTargetSectorIndex.iX && iY == pTargetSectorIndex.iY);
		}
	}

	/* public - Variable declaration            */

	[Header( "섹터 구간 설정" )]
	[SerializeField]
	private Transform _pTrans_LeftDown = null;
	[SerializeField]
	private Transform _pTrans_RightUp = null;

	[Header( "섹터를 얼마나 나눌것인지" )]
	[SerializeField]
	private int _iSectorDivision_X = 2;
	[SerializeField]
	private int _iSectorDivision_Y = 2;

	[Header( "주변 섹터에 생성 금지 옵션" )]
	[SerializeField]
	private ESectorArroundCheckOption eCheckOption = ESectorArroundCheckOption.None;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<int, SSectorIndex> _mapUseSector = new Dictionary<int, SSectorIndex>();
	private HashSet<SSectorIndex> _setCheckSector = new HashSet<SSectorIndex>();

	private float _fSectorTotalPos_Right;
	private float _fSectorTotalPos_Down;

	private float _fSectorTotalPos_Left;
	private float _fSectorTotalPos_Up;

	private float _fSectorUnit_X;
	private float _fSectorUnit_Y;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public Vector2 GetRandomSector( )
	{
		return GetRandomPosition();
	}

	public Vector2 GetRandomSector(MonoBehaviour pTarget)
	{
		return ProcCheckEmptySector( pTarget.GetInstanceID());
	}

	public Vector2 GetRandomSector( int iObjectID )
	{
		return ProcCheckEmptySector( iObjectID);
	}

	public void DoRemoveUseSector( MonoBehaviour pTarget )
	{
		_mapUseSector.Remove( pTarget.GetInstanceID() );
	}

	public void DoRemoveUseSector(int iKey)
	{
		_mapUseSector.Remove(iKey);
	}

	public Vector2 GetRandomPosition()
	{
		float fRandX = Random.Range(_fSectorTotalPos_Left, _fSectorTotalPos_Right);
		float fRandY = Random.Range(_fSectorTotalPos_Down, _fSectorTotalPos_Up);

		return new Vector2(fRandX, fRandY);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();
		
		_fSectorTotalPos_Up = _pTrans_RightUp.position.y;
		_fSectorTotalPos_Down = _pTrans_LeftDown.position.y;

		_fSectorTotalPos_Left = _pTrans_LeftDown.position.x;
		_fSectorTotalPos_Right = _pTrans_RightUp.position.x;

		_fSectorUnit_X = Mathf.Abs(_fSectorTotalPos_Right - _fSectorTotalPos_Left) / _iSectorDivision_X;
		_fSectorUnit_Y = Mathf.Abs(_fSectorTotalPos_Up - _fSectorTotalPos_Down) / _iSectorDivision_Y;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		float fDrawPosX = _fSectorTotalPos_Left;
		float fDrawPos_StartY = _fSectorTotalPos_Up;
		float fDrawPos_DestY = _fSectorTotalPos_Down;

		for (int i = -1; i < _iSectorDivision_X; i++)
		{
			Gizmos.DrawLine( new Vector2( fDrawPosX, fDrawPos_StartY ), new Vector2( fDrawPosX, fDrawPos_DestY ));
			fDrawPosX += _fSectorUnit_X;
		}

		float fDrawPosY = _fSectorTotalPos_Down;
		float fDrawPos_StartX = _fSectorTotalPos_Left;
		float fDrawPos_DestX = _fSectorTotalPos_Right;

		for (int i = -1; i < _iSectorDivision_Y; i++)
		{
			Gizmos.DrawLine( new Vector2( fDrawPos_StartX, fDrawPosY ), new Vector2( fDrawPos_DestX, fDrawPosY ) );
			fDrawPosY += _fSectorUnit_Y;
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

	private Vector2 ProcCheckEmptySector(int iObjectID)
	{
		// 총길이(300) _arrPosition[(int)EVector2.X].x - 랜덤좌표(-115)

		int iCount = 0;
		while (iCount < 100)
		{
			Vector2 v2RandPos = GetRandomPosition();
			SSectorIndex sSectorIndex;
			sSectorIndex.iX = (int)(Mathf.Abs(_fSectorTotalPos_Right - v2RandPos.x) / _fSectorUnit_X);
			sSectorIndex.iY = (int)(Mathf.Abs(_fSectorTotalPos_Up - v2RandPos.y) / _fSectorUnit_Y);

			if (_mapUseSector.ContainsValue( sSectorIndex ) == false)
			{
				if (eCheckOption == ESectorArroundCheckOption.None || CheckArroundIs_AlreadyExistUnit( ref sSectorIndex ))
				{
					_mapUseSector.Add( iObjectID, sSectorIndex );
					return v2RandPos;
				}
			}

			iCount++;
		}

		if (iCount >= 100)
			Debug.Log( "ProcCheckEmptySector Loop Count > 100" );

		return Vector2.zero;
	}

	private bool CheckArroundIs_AlreadyExistUnit( ref SSectorIndex pSectorIndex)
	{
		_setCheckSector.Clear();

		if(eCheckOption == ESectorArroundCheckOption.FourWay)
		{
			for (int i = pSectorIndex.iX - 1; i < 3; i++)
			{
				SSectorIndex sCurrentCheckSector = new SSectorIndex( i, pSectorIndex.iY );
				if (pSectorIndex.CheckIsEqual( ref sCurrentCheckSector ) == false &&
					_mapUseSector.ContainsValue( new SSectorIndex( i, pSectorIndex.iY ) ))
					return false;
			}

			for (int i = pSectorIndex.iY - 1; i < 3; i++)
			{
				SSectorIndex sCurrentCheckSector = new SSectorIndex( pSectorIndex.iX, i );
				if (pSectorIndex.CheckIsEqual( ref sCurrentCheckSector ) == false &&
					_mapUseSector.ContainsValue( new SSectorIndex( pSectorIndex.iX, i ) ))
					return false;
			}
		}
		else if(eCheckOption == ESectorArroundCheckOption.EightWay)
		{
			for (int i = pSectorIndex.iX - 1; i < 3; i++)
			{
				for (int j = pSectorIndex.iY - 1; j < 3; j++)
				{
					SSectorIndex sCurrentCheckSector = new SSectorIndex( i, j );
					if (pSectorIndex.CheckIsEqual( ref sCurrentCheckSector) == false &&
						_mapUseSector.ContainsValue( new SSectorIndex( i, j ) ))
						return false;
				}
			}
		}

		return true;
	}
}
