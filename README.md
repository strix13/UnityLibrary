# 환영합니다
- 이 프로젝트는 Unity C# 오픈소스 라이브러리 프로젝트입니다.

- 이 프로젝트는 게임을 보다 빠르고 편하게 만들기 위한 클래스와 함수를 제공합니다.

- 이 프로젝트는 유니티 내 어떤 프로젝트에도 종속되지 않습니다.

- 이 프로젝트는 프로그래머를 대상으로 설명합니다.

- 하단에 있는 구현 목록외의 사항도 있으나, 정리가 덜 되어 따로 작성하지 않았습니다.
  - 하단에 있는 구현 목록의 경우 비교적 최근에 리펙토링한 결과입니다.

-  깃허브 링크 :
https://github.com/strix13/UnityLibrary

## 루트 클래스

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjEx/MDAxNTI1NjY0ODU2NTc2.pl7EYZNbYzETHR2t9lPE6O2b6gp9eBCOSWRDpAQmvrEg.km7IoWpo9ucfK6lREhRdZ_gEbi_QIkF0QLRt_lxJZ_0g.JPEG.strix13/StrixLibrary_-_%EB%A3%A8%ED%8A%B8_%ED%81%B4%EB%9E%98%EC%8A%A4.jpg?type=w773)

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**

[![Video Label](http://img.youtube.com/vi/xuhKn5H6ck4/0.jpg)](https://www.youtube.com/watch?v=xuhKn5H6ck4=0s)

**ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.**

- 기존 Unity의 **MonoBehaviour의 기능을 확장** 시킨 루트 클래스입니다.

- **이 클래스를 상속받은 뒤 Update문을 작성하면, 매니져 클래스 한곳의 코루틴에서 Update를 루프로 돌며, 몇 개가 업데이트 중인지 하이어라키 뷰에서 실시간으로 체크할 수 있습니다.**
 - MonoBehaviour가 아닌 클래스도 IUpdateAble을 구현하여 업데이트 매니져에 등록하면 Update처럼 사용할 수 있습니다.

- 이 클래스는 **GetComponentAttribute를 지원**합니다.
  - GetComponentAttribute는 GetComponent, GetComponentInParents, GetComponentInChildren이 있습니다.
  - GetComponentInChildren은 자식 중 첫번째를 찾기, 이름으로 찾기, **복수형 자료형**을 지원하며, **Dictionary도 지원**합니다.
  - Dictionary의 경우 **string을 키값으로 두면 오브젝트의 이름, Enum으로 두면 오브젝트의 이름을 Enum으로 파싱에 성공한 것들만** 저장합니다.
  **- 만약 하나도 못찾을 시 경고 로그를 출력합니다.**

- 그 외 UnityEvent 함수를 가상함수로 지원합니다.


- [링크 - 유니티 블로그 - 10000번의 Update() 호출](https://blogs.unity3d.com/kr/2015/12/23/1k-update-calls/)
- [링크 - Unity3D 자동 GetComponent](https://openlevel.postype.com/post/683269)
- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/ObjectBase)

---
## UI 클래스 관계도

![](
https://blogfiles.pstatic.net/MjAxODA1MDVfNSAg/MDAxNTI1NDk2MjEzMzk4.YBGR9hSvFoGqEg5lUCeF346bvZ3x9EEgLQSfjWcyFPsg.DdcKOk5Ml-eeOorUOfqwJcJnscZGxqmvC_Ol40H9eZ4g.PNG.strix13/StrixLibrary_-_UI_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.png)

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**

- 구조는 크게 Manager, Panel로 이루어져 있으며 Manager는 Panel을 관리하는 싱글톤 패턴입니다.

- Manager의 경우 Panel을 Show / Hide 시키며, Panel의 레이어를 관리합니다.

- Panel을 Show / Hide 시 FadeIn / Out 효과도 지원합니다.


- Panel의 경우 NGUI로는 NGUIPanel, UGUI로는 Canvas 단위로 된 하나의 Window 단위 입니다.

- Panel의 경우 하위 **UI Element의 이벤트 ( 버튼, DropItem, Input 입력 ) 등을 쉽게 override하여 쉽게 사용할 수 있도록 지원**합니다.

- Panel의 경우 그 외 **UI Element를 쉽게 Get**할 수 있도록 지원합니다.

- Panel의 경우 OnShow / OnHide의 이벤트가 있으며, **Show / Hide 시 코루틴이 동작합니다. ( 애니메이션을 기다리기 위함, 코루틴 끝날 때 콜백 지원 )**


- [UI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/02.UI)
- [UGUI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/03.UGUI)
- [NGUI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/04.NGUI)

---
## 로컬 라이징 시스템

![](
https://postfiles.pstatic.net/MjAxODA1MDdfMTUg/MDAxNTI1NjYxNjYzOTMw.uTGH7T0d2IFz7o5BHMhKqZndkZY1_XN_N9HF4FBF9VMg.hGGB_GJmzsuWrqreBhTW40fd806yUgnS4LFqL6gUlmkg.JPEG.strix13/StrixLibrary_-_%EB%A1%9C%EC%BB%AC%EB%9D%BC%EC%9D%B4%EC%A7%95_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**

[![Video Label](http://img.youtube.com/vi/mLQMwqKgh4I/0.jpg)](https://www.youtube.com/watch?v=mLQMwqKgh4I=0s)

**ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.**

- 로컬라이징의 경우 Key, Value를 가진 임의의 Text파일을 파싱한 데이터를 기반으로 동작합니다.

- 외부에서 Instance 요청 시 자동으로 생성되서 파일을 파싱 후, 자동으로 로컬라이징이 될 수 있게끔 작업하였습니다.

- 작성한 Text, Sprite 컴포넌트 외에 이벤트를 받고 싶으면 ILocalizeListner를 구현하고, Manager에 등록하면 됩니다.

- [코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Localize)

---
## 사운드 시스템 클래스 관계도

![](
https://postfiles.pstatic.net/MjAxODA1MDZfODIg/MDAxNTI1NTg2NjI4NDg5.2-piThykc2EWXJVPdEQUx0FlQ9PoSANx5ZLP1S8-KWwg.vvcEPNS5G5_jlnjJqcXHSgF2I94o_bMPaWWPa4537BEg.JPEG.strix13/StrixLibrary_-_%EC%82%AC%EC%9A%B4%EB%93%9C_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**

[![Video Label](http://img.youtube.com/vi/TN145PFwvkI/0.jpg)](https://www.youtube.com/watch?v=TN145PFwvkI=0s)

**ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.**

- 사운드 시스템의 경우, SoundManager가 SoundSlot 클래스를 관리하는 형태입니다.

- **Instnace 요청 시 없을 경우 Instance를 생성하여 Initialize하여 플레이 할 수 있게끔 적용**하였습니다.

- SoundManager는 외부에서 사운드 플레이 요청 시 AudioClip을 얻고, **SoundSlot을 풀링**하여 Slot에게 플레이 합니다.

- **MainVolume, 개개별 사운드 Volume, Effect Volume, BGM Volume을 지원합니다.**


- SoundSlot의 경우 AudioClip과 AudioSource를 관리하는 클래스입니다.

- **사운드가 끝날 때 이벤트 통보를 지원**합니다.

- **FadeIn / Out** 효과를 지원합니다.


- **SoundPlayer의 경우 임의의 게임오브젝트에 추가하여 쉽게 사운드를 플레이할 수 있는 클래스**입니다.

- 인스펙터에 세팅한 값을 토대로 SoundManager에게 요청하는 형태이기 때문에, **SoundManager에서 관리하는 옵션값(MainVolume, 개개별 사운드 Volume, Effect Volume 등) 및 SoundSlot 풀링을 지원**합니다.

- 그 외로 그룹 내 랜덤 사운드 플레이, 사운드가 끝났을 때 이벤트 통보를 지원합니다.

- 사운드 플레이 조건은 EventTrigger 클래스의 트리거 발동 조건 내에 가능합니다.
 - 트리거 발동 조건 : 유니티 이벤트(OnAwake ~ OnDisable), 물리 이벤트(Triger / Collision Enter), UI 이벤트(Click, Press) 등

- [코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Sound)

---
## 이펙트 시스템 클래스 관계도

![](
https://postfiles.pstatic.net/MjAxODA1MDZfMzUg/MDAxNTI1NjEwMDU3MjQz.az-Y7SxCGDV-jlb4YCi06dZMTi5DQpIbyeBIzaLJwJkg.kfdc4L_DxkRORmGBFbGdlu6vHzu1MqnzrZKj3CL_kZIg.JPEG.strix13/StrixLibrary_-_%EC%9D%B4%ED%8E%99%ED%8A%B8_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**

[![Video Label](http://img.youtube.com/vi/OQ8UpqBUIJQ/0.jpg)](https://www.youtube.com/watch?v=OQ8UpqBUIJQ=0s)

**ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.**

- 이펙트 시스템은 사운드 시스템과 비슷한 설계입니다.

- **Instnace 요청 시 없을 경우 Instance를 생성하여 Initialize하여 플레이 할 수 있게끔 적용**하였습니다.


- EffectManager는 CEffect라는 클래스를 관리합니다.

- EffectManager에 이펙트를 요청시, 원하는 위치에 이펙트를 실행하며, **풀링**합니다.


- CEffect의 경우 NGUI, Spine, Particle System을 래핑하였습니다.

- CEffect는 이펙트가 끝날 때 이벤트 통보를 지원합니다.

- EffectPlayer의 경우 SoundPlayer와 비슷한 기능이므로 생략합니다.

- [코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Effect)

---
## 유니티 마크다운 뷰어

![](https://camo.githubusercontent.com/6651c1a8b118bb47b030aa998be60db73d057f8d/68747470733a2f2f706f737466696c65732e707374617469632e6e65742f4d6a41784f4441314d4456664d6a4d342f4d4441784e5449314e5445354e4459354e4441312e775252445f66685450675239423470716b71496151574b4e4d586b543435757a575f526753323272494c30672e45756153622d417733516552434b4e73795f4d6643357566475561456f6448674d666435335550766f724d672e4a5045472e737472697831332f62616e646963616d5f323031382d30352d30355f32302d31392d32302d3732392e6a70673f747970653d77373733)

- [코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Editor/MarkdownViewer)

---
### 개발자 연락처

카카오톡 (유니티 개발자 모임 ) : https://open.kakao.com/o/gOi17az
