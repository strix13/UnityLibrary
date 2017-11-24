using UnityEngine;
using System.Collections.Generic;

public class CRippleEffectPlayer : CObjectBase
{
	// 셰이더와 배열 총량을 맞춰주어야 한다.
	// 셰이더에 프로퍼티로 빼고 싶었으나, 셰이더에선 Array가 Dynamic일 수 없다.
	// 셰이더에 Define으로 정의해놓았다.
	const int const_iEffectCapacity = 20;

	public AnimationCurve waveform = new AnimationCurve(
		new Keyframe(0.00f, 0.50f, 0, 0),
		new Keyframe(0.05f, 1.00f, 0, 0),
		new Keyframe(0.15f, 0.10f, 0, 0),
		new Keyframe(0.25f, 0.80f, 0, 0),
		new Keyframe(0.35f, 0.30f, 0, 0),
		new Keyframe(0.45f, 0.60f, 0, 0),
		new Keyframe(0.55f, 0.40f, 0, 0),
		new Keyframe(0.65f, 0.55f, 0, 0),
		new Keyframe(0.75f, 0.46f, 0, 0),
		new Keyframe(0.85f, 0.52f, 0, 0),
		new Keyframe(0.99f, 0.50f, 0, 0)
	);

	[Range(0.01f, 1.0f)]
	public float refractionStrength = 0.5f;

	public Color reflectionColor = Color.gray;

	[Range(0.01f, 1.0f)]
	public float reflectionStrength = 0.7f;

	[Range(1.0f, 3.0f)]
	public float waveSpeed = 1.25f;

	[SerializeField]
	private float _fDuration = 3f;

	[SerializeField]
	Shader shader;

	public class SInfoRippleEffect
	{
		private Vector2 position;
		private float time;
		private float _fDuration;

		public void DoReset(Vector2 vecPos, float fDuration)
		{
			time = 0;
			position = vecPos;
			_fDuration = fDuration;
		}

		public bool Update()
		{
			time += Time.deltaTime;

			return time > _fDuration;
		}

		public Vector4 MakeShaderParameter(float fAspect)
		{
			return new Vector4(position.x * fAspect, position.y, time, 0);
		}
	}

	private LinkedList<SInfoRippleEffect> _listDropInfo = new LinkedList<SInfoRippleEffect>();
	private List<Vector4> _listShaderParameter = new List<Vector4>();
	private Queue<SInfoRippleEffect> _queueOnDisable = new Queue<SInfoRippleEffect>();

	private Texture2D _pTextureGrad;
	private Material _pMaterial;
	private Camera _pCamera;

	private float _fCameraAspect;

	public void DoPlayRippleEffect(Vector3 vecPos)
	{
		if(_queueOnDisable.Count != 0)
		{
			SInfoRippleEffect pDropInfoDisable = _queueOnDisable.Dequeue();
			pDropInfoDisable.DoReset(vecPos, _fDuration);
			_listDropInfo.AddLast(pDropInfoDisable);
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		_pCamera = GetComponentInParent<Camera>();
		_fCameraAspect = _pCamera.aspect;

		_pTextureGrad = new Texture2D(32, 1, TextureFormat.Alpha8, false);
		_pTextureGrad.wrapMode = TextureWrapMode.Clamp;
		_pTextureGrad.filterMode = FilterMode.Bilinear;
		for (var i = 0; i < _pTextureGrad.width; i++)
		{
			var x = 1.0f / _pTextureGrad.width * i;
			var a = waveform.Evaluate(x);
			_pTextureGrad.SetPixel(i, 0, new Color(a, a, a, a));
		}
		_pTextureGrad.Apply();

		_pMaterial = new Material(shader);
		_pMaterial.hideFlags = HideFlags.DontSave;
		_pMaterial.SetTexture("_GradTex", _pTextureGrad);

		for (int i = 0; i < const_iEffectCapacity; i++)
			_queueOnDisable.Enqueue(new SInfoRippleEffect());

		UpdateShaderParameters();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (Input.GetMouseButtonDown(0))
		{
			Vector3 vecPos = _pCamera.ScreenToWorldPoint(Input.mousePosition);
			Vector3 vecPos2 = _pCamera.WorldToViewportPoint(vecPos);
			DoPlayRippleEffect(vecPos2);
		}

		var pNodeCurrent = _listDropInfo.First;
		while(pNodeCurrent != null)
		{
			SInfoRippleEffect pInfoEffect = pNodeCurrent.Value;
			if (pInfoEffect.Update())
			{
				_listDropInfo.Remove(pInfoEffect);
				_queueOnDisable.Enqueue(pInfoEffect);
			}

			pNodeCurrent = pNodeCurrent.Next;
		}

		UpdateShaderParameters();
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, _pMaterial);
	}

	void UpdateShaderParameters()
	{
		if(_listDropInfo.Count != 0)
		{
			List<Vector4> listParameter = new List<Vector4>();
			var pNodeCurrent = _listDropInfo.First;
			int iCount = 0;
			while (pNodeCurrent != null)
			{
				listParameter.Add(pNodeCurrent.Value.MakeShaderParameter(_fCameraAspect));
				pNodeCurrent = pNodeCurrent.Next;
				iCount++;
			}

			for(int i = iCount; i < const_iEffectCapacity; i++)
				listParameter.Add(Vector4.zero);
	
			_pMaterial.SetVectorArray("_DropInfo", listParameter );

			//_pMaterial.SetVector("_Drop1", _listDropInfo.Last.Value.MakeShaderParameter(_fCameraAspect));
			//_pMaterial.SetVectorArray("_DropInfo", new Vector4[] { _listDropInfo.Last.Value.MakeShaderParameter(_fCameraAspect) });
		}
		_pMaterial.SetColor("_Reflection", reflectionColor);
		_pMaterial.SetVector("_Params1", new Vector4(_fCameraAspect, 1, 1 / waveSpeed, 0));
		_pMaterial.SetVector("_Params2", new Vector4(1, 1 / _fCameraAspect, refractionStrength, reflectionStrength));
	}

}
