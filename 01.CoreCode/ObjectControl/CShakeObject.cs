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
	//[SerializeField]
	//private bool _bMachineShaking = false;

	private Vector3 _vecOriginPos;
    private float _fRemainShakePow;
    private bool _bBackToOriginPos;

    Coroutine _pCoroutine;

	// ========================== [ Division ] ========================== //

	public void DoSetShakeOnMobileShake(bool bEnable)
	{
		//_bMachineShaking = bEnable;
	}

	public void DoShakeObject(bool bBackToOriginPos)
    {
        if (_bBackToOriginPos)
            transform.localPosition = _vecOriginPos;

        _bBackToOriginPos = bBackToOriginPos;
        _fRemainShakePow = _fDefaultShakePow;

        if (_pCoroutine != null)
            StopCoroutine(_pCoroutine);
        _pCoroutine = StartCoroutine(CoStartShake());

		//if (_bMachineShaking)
		//	.instance.DoShakeMobile();
	}

	public void DoShakeObject(float fShakePow, bool bReverseOrigin)
    {
        if (_bBackToOriginPos)
            transform.localPosition = _vecOriginPos;

        _bBackToOriginPos = bReverseOrigin;
        _fRemainShakePow = fShakePow;

        if (_pCoroutine != null)
            StopCoroutine(_pCoroutine);
        _pCoroutine = StartCoroutine(CoStartShake());
    }

    // ========================== [ Division ] ========================== //

    private IEnumerator CoStartShake()
    {
        _vecOriginPos = transform.localPosition;
        while (_fRemainShakePow > 0f)
        {
            Vector3 vecOriginPos = transform.localPosition;
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

            transform.localPosition = vecShakePos;
            _fRemainShakePow -= _fShakeMinusDelta;

            yield return null;
        }

        if (_bBackToOriginPos)
            transform.localPosition = _vecOriginPos;

        yield break;
    }
}
