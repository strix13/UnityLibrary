using UnityEngine;
using System.Collections;

public class CFollowObject : CObjectBase
{
    public enum EFollowPos
    {
        All,
        X,
        XY,
        XZ,
        Y,
        YZ,
        Z
    }

    public enum EFollowMode
    {
        ControlOutside,
        FixedUpdate,
        Update,   
    }

	[Header("쫓아가기 옵션")]
    [SerializeField]
    private EFollowPos _eFollowPos = EFollowPos.All;
    [SerializeField]
    private Transform _pTransTarget = null;
    [SerializeField]
    private EFollowMode _bUseFixedUpdate = EFollowMode.Update;
	[SerializeField]
	private bool _bIsSmoothFollow = false;
	[SerializeField] [Range(0, 1f)]
	private float _fSmoothFollowDelta = 0.1f;

	[Header("흔들기 옵션")]
	[SerializeField]
    private float _fShakeMinusDelta = 0.1f;

	[Header( "디버그용" )]
	private bool _bIsFollow = false;

	private Vector3 _vecAwakePos;
	private Vector3 _vecOriginPos;
    private Vector3 _vecTargetOffset;
    private float _fRemainShakePow;

    private bool _bFollowX;
    private bool _bFollowY;
    private bool _bFollowZ;

    // ========================== [ Division ] ========================== //

	public void DoSetPos_OnAwake()
	{
		transform.position = _vecAwakePos;
	}

    public void DoShakeObject(float fShakePow)
    {
        if(_fRemainShakePow <= 0f)
        {
            //Debug.Log("Shake Start CurrentPos : "  + _pTransformCashed.position + " Offset : " + _vecTargetOffset);
            _vecOriginPos = _pTransformCached.position;
        }

        _fRemainShakePow = fShakePow;
    }

    public void DoInitTarget(Transform pTarget)
    {
        _pTransTarget = pTarget;
		DoResetFollowOffset();
	}

	public void DoResetFollowOffset()
	{
		if (_pTransTarget == null) return;

		_vecTargetOffset = _pTransTarget.position - _pTransformCached.position;

		_bFollowX = _eFollowPos == EFollowPos.All || _eFollowPos == EFollowPos.X || _eFollowPos == EFollowPos.XY || _eFollowPos == EFollowPos.XZ;
		_bFollowY = _eFollowPos == EFollowPos.All || _eFollowPos == EFollowPos.Y || _eFollowPos == EFollowPos.XY || _eFollowPos == EFollowPos.YZ;
		_bFollowZ = _eFollowPos == EFollowPos.All || _eFollowPos == EFollowPos.Z || _eFollowPos == EFollowPos.XZ || _eFollowPos == EFollowPos.YZ;
	}

	public void DoSetFollow(bool bFollow)
	{
		_bIsFollow = bFollow;
	}

    public void DoUpdateFollow()
    {
		if (_bIsFollow == false) return;
		if (_pTransTarget == null) return;

        Vector3 vecFollowPos = _pTransformCached.position;
        Vector3 vecTargetPos = _pTransTarget.position;

		if(_bIsSmoothFollow)
			ProcFollow_Smooth( ref vecFollowPos, vecTargetPos );
		else
			ProcFollow_Normal( ref vecFollowPos, vecTargetPos );

		vecFollowPos = ProcShake( vecFollowPos );
		_pTransformCached.position = vecFollowPos;
    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

		_vecAwakePos = transform.position;
		if (_pTransTarget != null)
		{
			DoInitTarget( _pTransTarget );
			DoSetFollow( true );
		}
	}

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (_bUseFixedUpdate == EFollowMode.Update)
            DoUpdateFollow();
    }

    private void FixedUpdate()
    {
        if (_bUseFixedUpdate == EFollowMode.FixedUpdate)
            DoUpdateFollow();
    }

	// ========================== [ Division ] ========================== //

	private void ProcFollow_Smooth( ref Vector3 vecFollowPos, Vector3 vecTargetPos )
	{
		Vector3 vecDestPos = vecFollowPos;
		ProcFollow_Normal( ref vecDestPos, vecTargetPos );
		vecFollowPos = Vector3.Lerp( _pTransformCached.position, vecDestPos, _fSmoothFollowDelta );
	}

	private void ProcFollow_Normal( ref Vector3 vecFollowPos, Vector3 vecTargetPos )
	{
		if (_eFollowPos != EFollowPos.All)
		{
			if (_bFollowX)
				vecFollowPos.x = vecTargetPos.x - _vecTargetOffset.x;

			if (_bFollowY)
				vecFollowPos.y = vecTargetPos.y - _vecTargetOffset.y;

			if (_bFollowZ)
				vecFollowPos.z = vecTargetPos.z - _vecTargetOffset.z;
		}
		else
			vecFollowPos = vecTargetPos - _vecTargetOffset;
	}

	private Vector3 ProcShake(Vector3 vecFollowPos )
	{
		if (_fRemainShakePow > 0f)
		{
			_fRemainShakePow -= _fShakeMinusDelta;
			if (_fRemainShakePow <= 0f)
			{
				if (_bFollowX == false)
					vecFollowPos.x = _vecOriginPos.x;

				if (_bFollowY == false)
					vecFollowPos.y = _vecOriginPos.y;

				if (_bFollowZ == false)
					vecFollowPos.z = _vecOriginPos.z;
			}
			else
			{
				Vector3 vecShakePos = PrimitiveHelper.RandomRange( vecFollowPos.AddFloat( -_fRemainShakePow ), vecFollowPos.AddFloat( _fRemainShakePow ) );

				if (_bFollowX) vecShakePos.x = vecFollowPos.x;
				if (_bFollowY) vecShakePos.y = vecFollowPos.y;
				if (_bFollowZ) vecShakePos.z = vecFollowPos.z;

				vecFollowPos = vecShakePos;
			}
		}

		return vecFollowPos;
	}
}
