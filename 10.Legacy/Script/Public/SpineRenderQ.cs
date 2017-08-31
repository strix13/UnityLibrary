using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineRenderQ : MonoBehaviour {
	GameObject g_SpineResource;
	public int        i_Depth;

	void Start () {
		g_SpineResource = this.gameObject;
		g_SpineResource.layer = LayerMask.NameToLayer("UI");
//		g_SpineResource.transform.localScale = new Vector3(41, 41, 41);
		SetUISpine(g_SpineResource, i_Depth); //ngui에 스파인 띄우기
	}
	public void SetUISpine(GameObject spineObj, int _rendQ)
	{
		if (spineObj.GetComponent<SkeletonAnimator>() == null)
			return;

		StartCoroutine(SetUISpineDo(spineObj, _rendQ));
	}
	IEnumerator SetUISpineDo(GameObject spineObj, int _rendQ)
	{
		//스파인 오브젝트를 생성하고 바로 실행한 경우 한프레임 대기하여 스파인 오브젝트 데이터 로드가 다 되도록 한다.
		yield return new WaitForEndOfFrame();

		SkeletonAnimator spineObjskel = spineObj.GetComponent<SkeletonAnimator>();

		//렌더큐 수정가능한 매터리얼 적용을 위해 새 AtlasAsset생성
		AtlasAsset atlasAsset = ScriptableObject.CreateInstance<AtlasAsset>();
		atlasAsset.atlasFile = spineObjskel.skeletonDataAsset.atlasAssets[0].atlasFile; // 기존 스파인 아틀라스 파일을 가져와서 설정

		//스파인 쉐이더로 새 매터리얼 생성
		Material atlasMaterial = new Material(Shader.Find("Spine/Skeleton"));
		atlasMaterial.mainTexture = spineObjskel.GetComponent<MeshRenderer>().materials[0].mainTexture;//기존 스파인 텍스쳐를 가져와서 설정
		atlasMaterial.name = "SpineUI_Mat";
		atlasMaterial.renderQueue = _rendQ; // 지정된 렌더큐로 새 매터리얼의 렌더큐를 설정한다.

		atlasAsset.materials = new[] { atlasMaterial };

		//새 AtlasAsset을 적용하기 위해 기존 SkeletonDataAsset을 복제하여 생성
		SkeletonDataAsset skeletonDataAsset = Instantiate(spineObj.GetComponent<SkeletonAnimator>().skeletonDataAsset)as SkeletonDataAsset;
		skeletonDataAsset.atlasAssets[0] = atlasAsset; //복제 생성한 SkeletonDataAsset에 새 AtlasAsset을 지정

		spineObjskel.skeletonDataAsset = skeletonDataAsset; // 스파인 오브젝트에 새 SkeletonDataAsset을 적용

		//새로 적용한 SkeletonDataAsset로 스파인  Reload
		if (spineObjskel.skeletonDataAsset != null)
		{
			if (spineObjskel.skeletonDataAsset.atlasAssets[0] != null)
			{
				//spineObjskel.skeletonDataAsset.atlasAssets[0].Reset();
			}
			//spineObjskel.skeletonDataAsset.Reset();
		}
		//spineObjskel.skeletonDataAsset.Reset();
	}
}
