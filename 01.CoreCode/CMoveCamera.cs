using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH                             
   Date        : 2017-02-07 오전 11:01:21
   Description : 스와이프 동작 카메라 움직임
   Edit Log    : 
   ============================================ */

public class CMoveCamera : CSingletonBase<CMoveCamera>
{
	/* const & readonly declaration             */
	public float PERSPCAM_ZOOM_MAX = 60f;
	public float PERSPCAM_ZOOM_MIN = 15f;
	public float ORTHOCAM_ZOOM_MAX = 10f;
	public float ORTHOCAM_ZOOM_MIN = 4f;

	/* enum & struct declaration                */

	/* public - Variable declaration            */
	public float p_fZoomSpeed = 1f;

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    private Camera[] _arrChildCamera;

    private Camera _pCamera = null;
	private Vector2 _v2LastCursorPos = Vector2.zero;

	private Vector3 _v3SmoothCamPos = Vector3.zero;
	private Vector3 _v3CamPos = Vector3.zero;

	private float _fZoomDir = 0f;
	private float _fZoomLimitForce = 256f;

	private bool _bIsMovedCamera = false; public bool p_bIsMovedCamera { get { return _bIsMovedCamera; } }
	private bool _bCanMoveCamera = false;

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

	protected override void OnAwake()
    {
        base.OnAwake();

		_pCamera = _pTransformCached.GetComponent<Camera>();
        _arrChildCamera = GetComponentsInChildren<Camera>();

		_v3SmoothCamPos = _pTransformCached.position;
		_v3CamPos = _pTransformCached.position;
	}

    protected override void OnUpdate()
    {
        base.OnUpdate();

		int iTouchCount = (Input.touchCount > 1) ? Input.touchCount : ((Input.GetMouseButton(0)) ? 1 : 0);
		switch (iTouchCount)
		{
			case 0:
				_bIsMovedCamera = false;

				// 리모트 어플 사용 시 작동안됨.
				if (_pCamera.orthographic)
					_pCamera.orthographicSize =
						Mathf.Clamp(_pCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * p_fZoomSpeed, ORTHOCAM_ZOOM_MIN, ORTHOCAM_ZOOM_MAX);
				else
					_pCamera.fieldOfView =
						Mathf.Clamp(_pCamera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * p_fZoomSpeed, PERSPCAM_ZOOM_MIN, PERSPCAM_ZOOM_MAX);
			break;

			case 1:
				if (Input.GetMouseButton(0) == false) break;

				Vector2 v2CursorPos = Input.mousePosition;
				if (Input.GetMouseButtonDown(0))
				{
					_v2LastCursorPos = v2CursorPos;

					_bCanMoveCamera = UICamera.hoveredObject == null;
					if(_bCanMoveCamera == false)
					{
						int iLayer = UICamera.hoveredObject.layer;
						_bCanMoveCamera = iLayer != LayerMask.NameToLayer("UI") && iLayer != LayerMask.NameToLayer("UI_3D");
					}
				}

				if (_bCanMoveCamera == false) break;

				float fDist = Vector3.Distance(v2CursorPos, _v2LastCursorPos);

				Vector3 v3Dir = (v2CursorPos - _v2LastCursorPos).normalized;
				v3Dir.z = v3Dir.y;
				v3Dir.y = 0;

				_v3CamPos = _pTransformCached.position - v3Dir * fDist * _pCamera.aspect * 0.1f;
				_v2LastCursorPos = v2CursorPos;

				break;

			case 2:
				Touch pTouchFirst = Input.touches[0], pTouchSecond = Input.touches[1];
				Vector2 pTouchFirstPos = pTouchFirst.position, pTouchSecondPos = pTouchSecond.position;

				fDist = Vector2.Distance(pTouchFirstPos, pTouchSecondPos);
				float fSmoothDist = Vector2.Distance((pTouchFirstPos - pTouchFirst.deltaPosition),
												     (pTouchSecondPos - pTouchSecond.deltaPosition));

				_fZoomDir = (fSmoothDist - fDist) / _fZoomLimitForce * p_fZoomSpeed;

				if (_pCamera.orthographic)
					_pCamera.orthographicSize =
						Mathf.Clamp(_pCamera.orthographicSize + _fZoomDir, ORTHOCAM_ZOOM_MIN, ORTHOCAM_ZOOM_MAX);
				else
					_pCamera.fieldOfView =
						Mathf.Clamp(_pCamera.fieldOfView + _fZoomDir, PERSPCAM_ZOOM_MIN, PERSPCAM_ZOOM_MAX);

				_bIsMovedCamera = true;
			break;
		}

		_v3SmoothCamPos = Vector3.Lerp(_v3SmoothCamPos, _v3CamPos, Time.deltaTime * 5f);
		_pTransformCached.position = _v3SmoothCamPos;

		if (_pCamera.orthographic)
        {
            float fOrthographicSize = _pCamera.orthographicSize;
            for (int i = 0; i < _arrChildCamera.Length; i++)
                _arrChildCamera[i].orthographicSize = fOrthographicSize;
        }
        else
        {
            float fFOV = _pCamera.fieldOfView;
            for (int i = 0; i < _arrChildCamera.Length; i++)
                _arrChildCamera[i].fieldOfView = fFOV;
        }



    }
    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}
