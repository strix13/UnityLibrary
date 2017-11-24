#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 컴포넌트 방식으로 구성요소로 사용하는 Animator Wrapping 클래스
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;

public class CAnimatorController : CObjectBase
{
	private System.Action _OnFinishAnimation;
	private Animator _pAnimator;

	private float _fCurrentAnimation_NomalizeTime;	public float p_fCurrentAnimation_NomalizeTime {  get { return _fCurrentAnimation_NomalizeTime; } }
	private string _strCurrentAnimName;
    private bool _bIsLoop;

    // ========================== [ Division ] ========================== //

    /// <summary>
    /// 사용하기 전에 반드시 실행 // Animator를 Getcomponent 합니다 (없을경우 자식 에서 찾음)
    /// </summary>
    public void DoInitAnimator()
    {
        _pAnimator = GetComponent<Animator>();
        if (_pAnimator == null)
            _pAnimator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 애니메이션이 실행 중인지 체크합니다.
    /// </summary>
    /// <param name="eAnimName">체크할 애니메이션 이름의 Enum</param>
    /// <returns></returns>
    public bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
    {
        bool bFindAnimation = false;
        for(int i = 0; i < _pAnimator.layerCount; i++)
        {
            AnimatorStateInfo sCurrentState = _pAnimator.GetCurrentAnimatorStateInfo(i);
            if(sCurrentState.IsName(eAnimName.ToString()))
            {
                bFindAnimation = true;
                break;
            }
        }
        return bFindAnimation;
    }

    /// <summary>
    /// 애니메이션이 실행 중인지 체크합니다.
    /// </summary>
    /// <param name="eAnimName">체크할 애니메이션 이름의 Enum</param>
    /// <returns></returns>
    public bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName, int iAnimLayer)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
    {
        AnimatorStateInfo sCurrentState = _pAnimator.GetCurrentAnimatorStateInfo(iAnimLayer);
        return sCurrentState.IsName(eAnimName.ToString());
    }

    /// <summary>
    /// 애니메이션을 실행합니다.
    /// </summary>
    /// <param name="eAnimName">플레이 할 애니메이션 이름의 Enum</param>
    /// <param name="OnFinishAnimation">애니메이션이 종료될 때 호출할 함수</param>
    public void DoPlayAnimation<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName, System.Action OnFinishAnimation = null)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
    {
        _OnFinishAnimation = OnFinishAnimation;
        _strCurrentAnimName = eAnimName.ToString();
		ProcPlayAnim(false);
    }
    
    /// <summary>
    /// 애니메이션을 반복으로 실행합니다.
    /// </summary>
    /// <param name="eAnimName">반복 실행할 애니메이션 이름의 Enum</param>
    public void DoPlayAnimation_Loop<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable
    {
        _strCurrentAnimName = eAnimName.ToString();
		ProcPlayAnim(true);
    }

    /// <summary>
    /// 애니메이션 Parameter를 직접 세팅합니다.
    /// </summary>
    /// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
    public void DoSetParam_trigger<ENUM_ANIMATION_PARAMETER>(ENUM_ANIMATION_PARAMETER eParam)
         where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
    {
        _pAnimator.SetTrigger(eParam.ToString());
    }

    /// <summary>
    /// 애니메이션 Parameter를 직접 세팅합니다.
    /// </summary>
    /// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
    public void DoSetParam_float<ENUM_ANIMATION_PARAMETER>(ENUM_ANIMATION_PARAMETER eParam, float fParameter)
        where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
    {
        _pAnimator.SetFloat(eParam.ToString(), fParameter);
    }

	/// <summary>
	/// 애니메이션 Parameter를 직접 세팅합니다.
	/// </summary>
	/// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
	public void DoSetParam_Int<ENUM_ANIMATION_PARAMETER>(ENUM_ANIMATION_PARAMETER eParam, int iParameter)
		where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
	{
		_pAnimator.SetInteger(eParam.ToString(), iParameter);
	}

	/// <summary>
	/// 애니메이션 Parameter를 직접 세팅합니다.
	/// </summary>
	/// <param name="eParam">애니메이션 Parameter 이름의 Enum</param>
	public void DoSetParam_bool<ENUM_ANIMATION_PARAMETER>(ENUM_ANIMATION_PARAMETER eParam, bool bParam)
        where ENUM_ANIMATION_PARAMETER : System.IConvertible, System.IComparable
    {
        _pAnimator.SetBool(eParam.ToString(), bParam);
    }

    public void DoSetAnimationSpeed(float fSpeed)
    {
        _pAnimator.speed = fSpeed;
    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();
        
        if(_pAnimator == null)
            DoInitAnimator();
    }

    // ========================== [ Division ] ========================== //

    private void ProcPlayAnim(bool bIsLoop)
    {
        _pAnimator.Play(_strCurrentAnimName, 0, 0f);
        //_pAnimator.Update(0);
        _bIsLoop = bIsLoop;
		_fCurrentAnimation_NomalizeTime = 0f;

        StopAllCoroutines();
        StartCoroutine(CoUpdateAnimation());
    }
    
    private IEnumerator CoUpdateAnimation()
    {
		while (true)
        {
			yield return null;

            bool bFindAnimation = false;
            AnimatorStateInfo sCurrentState = _pAnimator.GetCurrentAnimatorStateInfo(0);
            for (int i = 0; i < _pAnimator.layerCount; i++)
            {
				if (_pAnimator.GetCurrentAnimatorStateInfo(i).IsName(_strCurrentAnimName))
                {
                    sCurrentState = _pAnimator.GetCurrentAnimatorStateInfo(i);
                    bFindAnimation = true;
                    break;
                }
            }

            if (bFindAnimation == false)
            {
                if(_OnFinishAnimation != null)
                {
                    System.Action OnFinishAnimation = _OnFinishAnimation;
                    _OnFinishAnimation = null;
                    yield return null;

                    OnFinishAnimation();
                }
                break;
            }

            _fCurrentAnimation_NomalizeTime = sCurrentState.normalizedTime;
            // 플레이가 끝났을 때
            if (_fCurrentAnimation_NomalizeTime >= 0.99f || sCurrentState.IsName(_strCurrentAnimName) == false)
            {
                //yield return SCManagerYield.GetWaitForSecond(sCurrentState.length * (1 - sCurrentState.normalizedTime));

                if (_bIsLoop)
                    DoPlayAnimation_Loop(_strCurrentAnimName);
                else if (_OnFinishAnimation != null)
                {
                    // OnFinishAnimation에 DoPlayAnimation(ENUM_ANIM_NAME eAnimName, System.Action OnFinishAnimation)을 실행하면, 
                    // _OnFinishAnimation을 세팅한다. 근데 그 함수가 끝나고 바로 null처리를 하기 때문에,
                    // 결과적으로 세팅을 해도 _OnFinishAnimation을은 null이 된다. 따라서, 임시 객체에 일단 저장 후 미리 null처리 후 호출한다.
                    System.Action OnFinishAnimation = _OnFinishAnimation;
                    _OnFinishAnimation = null;
					yield return null;

                    OnFinishAnimation();
                }

                break;
            }

            yield return null;
        }


        yield break;
    }
}
