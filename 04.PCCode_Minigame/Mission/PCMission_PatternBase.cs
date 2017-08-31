using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 패턴 담당 클래스 - 패턴 기본 로직은 여기서..
   Version	   :
   ============================================ */

public partial class PCMission_PatternBase<Enum_Key, Class_Resource> : CObjectBase
	where Enum_Key : struct, System.IComparable, System.IConvertible
	where Class_Resource : Component
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public event System.Action<Enum_Key, Class_Resource> p_EVENT_OnGenerate;

	public Enum_Key p_eGenerateKey;

	[Header( "디버그용 모니터링" )]
	[SerializeField]
	private int _iGenerateRemainCount;
	[SerializeField]
	private bool _bIsGenerating = true;
	[SerializeField]
	private int _iPlayCountCurrent;

	[Header( "패턴 옵션" )]
	[SerializeField]
	public float _fDelaySec_Pattern = 0f;
	[SerializeField]
	public float _fDelaySec_Generate = 0.2f;
	[SerializeField]
	private EMissionPatternName _ePattern = EMissionPatternName.Line;
	[SerializeField]
	private int _iPlayCount_Pattern = 1;
	[SerializeField]
	private bool _bPlayOnEnable = false;
	[SerializeField]
	private bool _bIsLoop = false;
	[SerializeField]
	private float _fAngleMuzzle = 15f;
	[SerializeField]
	private float _fAngleRotate = 15f;
	[SerializeField]
	private Vector2 _vecPosGap = Vector2.zero;
	[SerializeField]
	private float _fTweenDuration = 2f;
	[SerializeField]
	private int _iMuzzleCount = 1;
	[SerializeField]
	private int _iGenerateCount_Max = 1;
	[SerializeField]
	private int _iGenerateCount_Min = 1;
	[SerializeField]
	private bool _bIsAimShot = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private System.Func<IEnumerator> _pPattern;
	private System.Action _OnFinishPattern;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetGenrating( bool bGenerating )
	{
		_bIsGenerating = bGenerating;
	}

	public void DoStopPattern(bool bGameObjectActiveOff = false)
	{
		_bIsGenerating = true;

		StopAllCoroutines();
		if (bGameObjectActiveOff)
			gameObject.SetActive(false);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	virtual protected void OnPatternPlay() { }
	virtual protected void OnPatternStop()
	{
		if(_ePattern == EMissionPatternName.Rotate_Model)
		{
			Debug.Log( name + _pTransformCached.localRotation, this );
			if (_pTransformCached.localRotation != Quaternion.identity)
			ProcRotateIdentify();
		}

		if (_OnFinishPattern != null)
		{
			_OnFinishPattern();
			_OnFinishPattern = null;
		}
	}

	virtual protected void OnGenerateSomthing( Enum_Key eKey, Class_Resource pResource ) { }
	virtual protected void ProcShotGenerate( Vector3 vecBulletPos, Quaternion rotBulletAngle )
	{
		Class_Resource pResource = CManagerPooling<Enum_Key, Class_Resource>.instance.DoPop( p_eGenerateKey );
		pResource.transform.position = vecBulletPos;
		pResource.transform.rotation = rotBulletAngle;
		pResource.transform.localScale = Vector3.one;

		if (p_EVENT_OnGenerate != null)
			p_EVENT_OnGenerate( p_eGenerateKey, pResource );
		OnGenerateSomthing( p_eGenerateKey, pResource );
	}


	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (_bPlayOnEnable)
			Invoke( "DoPlayPattern", 0.5f );
			//DoPlayPattern();
	}
#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if(Application.isEditor)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube( transform.position, Vector3.one * 0.5f );
			Gizmos.DrawLine( transform.position, transform.position + transform.up * 1f );
			UnityEditor.Handles.Label( transform.position, name );
		}
	}
#endif
	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoPlayPattern( bool bIsLoop )
	{
		if (bIsLoop)
		{
			while (true)
			{
				if(gameObject.activeSelf)
				{
					OnPatternPlay();
					StartCoroutine( CoLookAtTarget() );
					yield return StartCoroutine( _pPattern() );
					yield return new WaitForSeconds( _fDelaySec_Pattern );
					OnPatternStop();
				}
			}
		}
		else
		{
			_iPlayCountCurrent = _iPlayCount_Pattern;
			while (_iPlayCountCurrent-- > 0)
			{
				if (gameObject.activeSelf)
				{
					OnPatternPlay();
					StartCoroutine( CoLookAtTarget() );
					yield return StartCoroutine( _pPattern() );
					yield return new WaitForSeconds( _fDelaySec_Pattern );
					OnPatternStop();
				}
			}
		}
	}

	// ========================================================================== //

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private IEnumerator CoGenerateSomthing()
	{
		while (_bIsLoop || _iGenerateRemainCount > 0)
		{
			if (_bIsGenerating)
			{
				_iGenerateRemainCount--;
				ProcShotGenerate(_pTransformCached.position, _pTransformCached.rotation);
			}

			yield return new WaitForSeconds( _fDelaySec_Generate );
		}
	}

	private IEnumerator CoGenerateSomthing( Vector3 vecPosGap, Vector3 vecAngleGap)
	{
		while (_bIsLoop || _iGenerateRemainCount > 0)
		{
			if (_bIsGenerating)
			{
				_iGenerateRemainCount--;
				ProcShotGenerate(_pTransformCached.position + vecPosGap, Quaternion.Euler(_pTransformCached.rotation.eulerAngles + vecAngleGap));
			}

			yield return new WaitForSeconds( _fDelaySec_Generate );
		}
	}

	private void ProcSetPatternIsFinish()
	{
		//Debug.Log("ProcSetPatternIsFinish");
		StopCoroutine(_pPattern());
	}

	private CNGUITweenRotationSpin GetTweenRotate()
	{
		CNGUITweenRotationSpin pTweenRotate = GetComponent<CNGUITweenRotationSpin>();
		if (pTweenRotate == null)
			pTweenRotate = gameObject.AddComponent<CNGUITweenRotationSpin>();

		pTweenRotate.enabled = true;
		pTweenRotate.ResetToBeginning();

		return pTweenRotate;
	}

	private CNGUITweenRotationSpin GetTweenRotate_EnemyModel()
	{
		Debug.Log( "GetTweenRotate_EnemyModel" );
		PCMission_Enemy pOwnerEnemy = GetComponentInParent<PCMission_Enemy>();
		if(pOwnerEnemy == null)
		{
			Debug.Log("부모에 " + typeof(PCMission_Enemy) + " 컴포넌트가 없습니다");
			return null;
		}

		CNGUITweenRotationSpin pTweenRotate = pOwnerEnemy.p_pTransModel.GetComponent<CNGUITweenRotationSpin>();
		if (pTweenRotate == null)
			pTweenRotate = pOwnerEnemy.p_pTransModel.gameObject.AddComponent<CNGUITweenRotationSpin>();

		pTweenRotate.enabled = true;
		pTweenRotate.ResetToFactor();

		return pTweenRotate;
	}

	private void ProcRotateIdentify()
	{
		CNGUITweenRotationSpin pTweenRotate = GetTweenRotate_EnemyModel();

		Vector3 vecCurrentRotation = pTweenRotate.transform.rotation.eulerAngles;
		pTweenRotate.from = vecCurrentRotation;

		if(vecCurrentRotation.z > 180)
			pTweenRotate.to = new Vector3(0f, 0, 359.9f);
		else
			pTweenRotate.to = new Vector3(0f, 0, 0f);

		pTweenRotate.duration = _fTweenDuration;
		pTweenRotate.PlayForward();
	}

	private IEnumerator CoLookAtTarget()
	{
		if (_bIsAimShot == false)
			yield break;

		PCMission_Player pPlayer = PCManagerInMission.instance.p_pPlayer;
		if(pPlayer != null)
		{
			Transform pTransformTarget = pPlayer.transform;
			while (true)
			{
				_pTransformCached.up = pTransformTarget.position - _pTransformCached.position;
				yield return null;
			}
		}
	}
}
