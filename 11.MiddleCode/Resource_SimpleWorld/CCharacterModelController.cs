using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 7:26:49
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(CAnimatorController))]
public class CCharacterModelController : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */
	
    public enum EAnimatorParam
    {
        Speed_f,		// For Move
        Grounded,		// For Fall
		
		WeaponType_int,
		MeleeType_int,
		Crouch_b,		// For Block
		Shoot_b,
    }

	public enum EAnimatorParam_MeleeType
	{
		None = -1,
		Stab = 0,
		OneHeaded = 1,
		TwoHanded = 2,
	}

	public enum EWeaponType
	{
		None = 0,

		Auto = 2,
		Auto2 = 3,
		Shotgun = 4,
		Rifle = 5,
		Riple2 = 6,
		SubMachineGun = 7,
		RPG = 8,
		MiniGun = 9,
		Bow = 11,
		Melee = 12,
	}

	[System.Flags]
	public enum EFlagCharacterModelOption
	{
		Normal = 1,
		MoveStop_OnAttack = 2,
		ColliderCheck_OnAttack = 4,
	}

	/* public - Field declaration            */

	public event System.Action<List<Collider>> p_EVENT_OnAttackFinish;

	public CCompoEquipmentHand _pEquipHand_Right;
	public CCompoEquipmentHand _pEquipHand_Left;

	public float p_fSpeed = 2f;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private List<Collider> _listEnemy = new List<Collider>();
    private CAnimatorController _pControl_Animator;
	private Collider _pCollider;	public Collider p_pCollider { get { return _pCollider; } }

	private EWeaponType _eWeaponType = EWeaponType.None;
	private EFlagCharacterModelOption _eCharcaterModelOption = EFlagCharacterModelOption.Normal;

	private float _fModelOffsetY;	public float p_fModelOffsetY {  get { return _fModelOffsetY; } }
	private bool _bMoveLock;
	private bool _bIsPlaying_Attack;
	private bool _bIsPlaying_Block;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoMoveCharacter(bool bInput, Vector3 vecMoveDirection, float fDelta)
    {
		if (vecMoveDirection == Vector3.zero)
		{
			_pControl_Animator.DoSetParam_float(EAnimatorParam.Speed_f, 0f);
			return;
		}
	
		_pTransformCached.rotation = Quaternion.LookRotation(vecMoveDirection);
		_pControl_Animator.DoSetParam_float(EAnimatorParam.Speed_f, fDelta);

		if (_bMoveLock || _bIsPlaying_Block) return;

		if (fDelta >= 0.5f)    // 뛰기 모션은 0.5부터
			_pTransformCached.Translate(Vector3.forward * p_fSpeed * 2f * Time.deltaTime, Space.Self);
		else if (fDelta >= 0.25f)   // 걷기 모션이 0.25부터 움직임
			_pTransformCached.Translate(Vector3.forward * p_fSpeed * Time.deltaTime, Space.Self);
	}

	public void DoLookCharacter(Vector3 vecDirection)
	{
		_pTransformCached.rotation = Quaternion.LookRotation(vecDirection.normalized);
	}

	public void DoPlayAttack_CurrentWeaponType()
	{
		if (_eWeaponType == EWeaponType.Melee)
			DoPlayAttack_Melee(EAnimatorParam_MeleeType.OneHeaded);
		else 
			DoPlayAttack_Shoot();
	}

	public void DoPlayAttack_Shoot()
	{
		if (_eWeaponType == EWeaponType.None || _eWeaponType == EWeaponType.Melee)
			return;

		if ((_eCharcaterModelOption & EFlagCharacterModelOption.MoveStop_OnAttack) == EFlagCharacterModelOption.MoveStop_OnAttack)
			_pControl_Animator.DoSetParam_float(EAnimatorParam.Speed_f, 0f);

		_pControl_Animator.DoSetParam_bool(EAnimatorParam.Shoot_b, true);
		_bIsPlaying_Attack = true;

		Debug.Log("Shoot!");
	}

	public void DoPlayAttack_Melee(EAnimatorParam_MeleeType eMeleeType)
    {
		if (_eWeaponType != EWeaponType.Melee) return;
		if ((_eCharcaterModelOption & EFlagCharacterModelOption.MoveStop_OnAttack) == EFlagCharacterModelOption.MoveStop_OnAttack)
			_pControl_Animator.DoSetParam_float(EAnimatorParam.Speed_f, 0f);

		_pControl_Animator.DoSetParam_Int(EAnimatorParam.MeleeType_int, (int)eMeleeType);
		_bIsPlaying_Attack = true;
	}

	public void DoPlay_Block(bool bBlock)
	{
		if (_bIsPlaying_Attack && bBlock && _bIsPlaying_Block) return;

		_bIsPlaying_Block = bBlock;
		_pControl_Animator.DoSetParam_bool(EAnimatorParam.Crouch_b, bBlock);
	}

	public void DoPlay_BlockSwitch()
	{
		if (_bIsPlaying_Attack) return;

		_bIsPlaying_Block = !_bIsPlaying_Block;
		_pControl_Animator.DoSetParam_bool(EAnimatorParam.Crouch_b, _bIsPlaying_Block);
	}

	public void DoSetWeapon(EWeaponType eWeaponType, Transform pTransWeaponModel)
	{
		_eWeaponType = eWeaponType;
		_pControl_Animator.DoSetParam_Int(EAnimatorParam.WeaponType_int, (int)eWeaponType);

		try
		{
			_pEquipHand_Right.DoSetEquipment(pTransWeaponModel);
		}
		catch
		{
			Debug.Log("Error", this);
		}
	}

	public void DoSetShield(Transform pTransShieldModel)
	{
		_pEquipHand_Left.DoSetEquipment(pTransShieldModel);
	}

	public void DoSetCharacterModel(EFlagCharacterModelOption eAnimationOption)
	{
		_eCharcaterModelOption = eAnimationOption;
	}

	public void DoSetMoveLock(bool bIsMoveLock)
	{
		_bMoveLock = bIsMoveLock;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */
	   
	public void EventAnimation(EAnimationEvent eMessage)
	{
		switch (eMessage)
		{
			case EAnimationEvent.AnimationStart:
				_bIsPlaying_Attack = true;
				break;

			case EAnimationEvent.AnimationFinish:
				_bIsPlaying_Attack = false;
				_pControl_Animator.DoSetParam_Int(EAnimatorParam.MeleeType_int, (int)EAnimatorParam_MeleeType.None);
				break;

			case EAnimationEvent.AttackStart:

				if((_eCharcaterModelOption & EFlagCharacterModelOption.ColliderCheck_OnAttack) == EFlagCharacterModelOption.ColliderCheck_OnAttack)
					_pEquipHand_Right.DoSetColliderOn(_pCollider);

				break;

			case EAnimationEvent.AttackFinish:
				HashSet<Collider> setEnemy = _pEquipHand_Right.DoSetColliderOff();
				setEnemy.Remove(_pCollider);
				setEnemy.ToList(_listEnemy);
				p_EVENT_OnAttackFinish(_listEnemy);
				break;
		}
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
    {
        base.OnAwake();

		_pCollider = GetComponent<Collider>();
        _pControl_Animator = GetComponent<CAnimatorController>();
		_pControl_Animator.DoInitAnimator();

		_pControl_Animator.DoSetParam_Int(EAnimatorParam.MeleeType_int, (int)EAnimatorParam_MeleeType.None);
		
		Renderer pRenderer = GetComponentInChildren<Renderer>();
		if (pRenderer != null)
			_fModelOffsetY = pRenderer.bounds.max.y;

		CCharacterWeaponGenerator pGenerator = gameObject.AddComponent<CCharacterWeaponGenerator>();
		pGenerator.DoGeneratorParts(transform);
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	private void OnIsFalling(bool bIsGrounded)
    {
        _pControl_Animator.DoSetParam_bool(EAnimatorParam.Grounded, bIsGrounded);
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
