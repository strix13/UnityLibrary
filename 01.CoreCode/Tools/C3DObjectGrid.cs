using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-04-09 오후 9:54:07
   Description : 
   Edit Log    : 
   ============================================ */

[ExecuteInEditMode]
public class C3DObjectGrid : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum ECircleOption
	{
		None,
		Rotate_Circle,
		Rotate_Circle_Inverse_Y,
		Rotate_Circle_Inverse_Z,
	}

	/* public - Variable declaration            */

	public Vector3 _vecLocalPosOffset = Vector3.zero;

	public ECircleOption _eCircleOption = ECircleOption.None;
	public Vector3 _vecRotate_OnCircle;
	public Vector3 _vecPos_OnCircle;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (Application.isPlaying)
		{
			enabled = false;
			//Debug.LogWarning("이 컴포넌트는 Editor 전용이기 때문에 실행시 자동으로 컴포넌트를 삭제합니다." + name, this);
			//DestroyObject(this);
		}

		if (transform.childCount == 0)
		{
			Debug.LogWarning( "자식이 없어서 정렬을 못합니다. 부모 오브젝트에게 붙여주세요" );
			return;
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (transform.childCount == 0) return;

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform pTransformChild = transform.GetChild( i );
			pTransformChild.localPosition = _vecLocalPosOffset * i;

			if (_eCircleOption != ECircleOption.None)
			{
				pTransformChild.localRotation = Quaternion.Euler( _vecRotate_OnCircle * i );
				Vector3 vecCurrentLocalPos = pTransformChild.localPosition;

				vecCurrentLocalPos += pTransformChild.forward * _vecPos_OnCircle.z;
				vecCurrentLocalPos += pTransformChild.up * _vecPos_OnCircle.y;
				vecCurrentLocalPos += pTransformChild.right * _vecPos_OnCircle.x;
				pTransformChild.localPosition = vecCurrentLocalPos;

				if (_eCircleOption == ECircleOption.Rotate_Circle_Inverse_Y)
				{
					Vector3 vecDirection = _pTransformCached.position - pTransformChild.position;
					pTransformChild.up = vecDirection.normalized;
				}
				else if (_eCircleOption == ECircleOption.Rotate_Circle_Inverse_Z)
				{
					Vector3 vecDirection = _pTransformCached.position - pTransformChild.position;
					pTransformChild.forward = vecDirection.normalized;
				}
			}
		}
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
