using UnityEngine;
using System.Collections;

public class CShakeObject : CObjectBase
{
    public enum EShakePos
    {
        All,
        X,
        XY,
        XZ,
        Y,
        YZ,
        Z
    }

    [SerializeField]
    private EShakePos _eShakePosType = EShakePos.All;
    [SerializeField]
    private float _fDefaultShakePow = 1f;
    [SerializeField]
    private float _fShakeMinusDelta = 0.1f;
	[SerializeField]
	private bool _bMachineShaking = false;

	private Vector3 _vecOriginPos;
    private float _fRemainShakePow;
    private bool _bBackToOriginPos;

	// ========================== [ Division ] ========================== //

	public void DoSetShakeOnMobileShake(bool bEnable)
	{
		_bMachineShaking = bEnable;
	}

	public void DoShakeObject(float fShakePow, float fTime)
	{
		_vecOriginPos = _pTransformCached.localPosition;
		Vector3 vecShakePos = PrimitiveHelper.RandomRange( _vecOriginPos.AddFloat( -fShakePow ), _vecOriginPos.AddFloat( fShakePow ) );

		iTween.ShakePosition( _pGameObjectCached, vecShakePos, fTime );
		//if(_bMachineShaking)
		//	PCManagerFramework.instance.DoShakeMobile();
	}

	public void DoShakeObject(bool bBackToOriginPos)
    {
        if (_bBackToOriginPos)
            _pTransformCached.localPosition = _vecOriginPos;

        _bBackToOriginPos = bBackToOriginPos;
        _fRemainShakePow = _fDefaultShakePow;
        StopAllCoroutines();
        StartCoroutine(CoStartShake());
	}

	public void DoShakeObject(float fShakePow, bool bReverseOrigin)
    {
        if (_bBackToOriginPos)
            _pTransformCached.localPosition = _vecOriginPos;

        _bBackToOriginPos = bReverseOrigin;
        _fRemainShakePow = fShakePow;
        StopAllCoroutines();
        StartCoroutine(CoStartShake());
    }

    // ========================== [ Division ] ========================== //
    
    private IEnumerator CoStartShake()
    {
        _vecOriginPos = _pTransformCached.localPosition;
        while (_fRemainShakePow > 0f)
        {
            Vector3 vecOriginPos = _pTransformCached.localPosition;
            Vector3 vecShakePos = PrimitiveHelper.RandomRange(vecOriginPos.AddFloat(-_fRemainShakePow), vecOriginPos.AddFloat(_fRemainShakePow));
            if(_eShakePosType != EShakePos.All)
            {
                if (_eShakePosType == EShakePos.Y || _eShakePosType == EShakePos.YZ || _eShakePosType == EShakePos.Z)
                    vecShakePos.x = vecOriginPos.x;

                if (_eShakePosType == EShakePos.X || _eShakePosType == EShakePos.XZ|| _eShakePosType == EShakePos.Z)
                    vecShakePos.y = vecOriginPos.y;

                if (_eShakePosType == EShakePos.X || _eShakePosType == EShakePos.XY || _eShakePosType == EShakePos.Y)
                    vecShakePos.z = vecOriginPos.z;
            }

            _pTransformCached.localPosition = vecShakePos;
            _fRemainShakePow -= _fShakeMinusDelta;

            yield return null;
        }

        if (_bBackToOriginPos)
            _pTransformCached.localPosition = _vecOriginPos;

        yield break;
    }
}
