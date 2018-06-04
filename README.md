# 환영합니다
- 이 프로젝트는 Unity C# 오픈소스 라이브러리 프로젝트입니다.

- 이 프로젝트는 게임을 보다 빠르고 편하게 만들기 위한 클래스와 함수를 제공합니다.

- 이 프로젝트는 유니티 내 어떤 프로젝트에도 종속되지 않습니다.

- 이 프로젝트는 프로그래머를 대상으로 설명합니다.

- 하단에 있는 구현 목록외의 사항도 있으나, 정리가 덜 되어 따로 작성하지 않았습니다.
  - 하단에 있는 구현 목록의 경우 비교적 최근에 리펙토링한 결과입니다.

-  깃허브 링크 :
https://github.com/strix13/UnityLibrary

- 코딩 스탠다드 링크 :
https://docs.google.com/document/d/1X8nIU0anstawLqf_pTnylCFLxgAAMJSQILIOZ89hFTs

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

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**


- [코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Editor/MarkdownViewer)

---
## 유니티 로그 래핑 클래스

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjcg/MDAxNTI1Njc5NTI2ODE1.x-IjrZGPCJ9jE_2Vbq-Z8V7RM47fA8ixYiP2mGsnbHQg.b41kzPpZpwXmpUiz0q69tnt3LehiBJ2zik8RpPxTCQQg.JPEG.strix13/bandicam_2018-05-07_16-52-00-886.jpg?type=w773)
![](
https://postfiles.pstatic.net/MjAxODA1MDdfMTc4/MDAxNTI1Njc5NDU5MzMz.4WXQCeIXmSfGTzxJ7HsOJr8adwepP0JlfNOIrT7vR70g.iXFhfhPCRYxKJVHB7oOmKfpU14Aei4eNv-GbfphEZp0g.PNG.strix13/%EB%A1%9C%EA%B7%B8.png?type=w773)

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**

- 로그 작성자, 로그 레벨에 따라 로그를 출력하지 않는 필터링 기능을 지원합니다.

- 로그 발생 시 txt, csv 파일로 출력할 수 있는 기능을 지원합니다.
  - 파일에 로그 발생 시간과 콜스택도 기록합니다.

- [코드 링크](https://github.com/KorStrix/Unity_DebugWrapper)

---
## 랜덤 관련 클래스
### 랜덤 매니져

- 랜덤 매니져는 IRandomItem 을 구현한 클래스에 대해 랜덤으로 리턴해주는 관리자입니다.

- **IRandomItem의 경우 랜덤확률을 구현하게끔 되어있으며, 랜덤확률에 따라 클래스를 리턴**합니다.

- **랜덤모드로 Peek, Delete 모드를 지원**합니다.
  - Peek의 경우 Random Item List에서 **아이템을 뽑을 때, List에 지장이 없습니다.**
  - Delete의 경우 List에서 **아이템을 뽑을 때 해당 아이템은List에서 제거됩니다.**

### 랜덤 2D 섹터

- 섹터의 경우, 인스펙터에서 몇 바이 몇 섹터인지, 섹터 당 사이즈가 몇인지를 설정합니다.

- 그 외 주변 섹터 체크옵션으로 주위 4타일(위 아래, 양 옆), 주위 8타일(4타일에서 대각선 4타일 추가)를 지원합니다.

- 섹터에게 랜덤 포지션을 요청 시, **직전의 섹터가 같은 섹터가 아니고, 주변 섹터도 아닌 섹터 내의 랜덤 위치만 리턴**합니다.

- **이펙트, 몬스터 스폰 등에 쓰입니다.** ( 스폰 시 주변에 골고루 나오게 하기 위한 로직 )

- [코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Random)

---
## 한글 파서

![](
https://postfiles.pstatic.net/MjAxODA1MDdfNjAg/MDAxNTI1NjgzNDYwNTkx.7LVx1ndhvjxdw2992EqJKUM_u6ZZSuYqaGKxwlNv-Cwg.ieXZO9IUNCEUPh_ut5VM-TEf1GaUJknekDBuKTyQUxIg.JPEG.strix13/bandicam_2018-05-07_17-57-24-601.jpg?type=w773)

**ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.**

- VR, 모바일 등에서 커스터마이징 된 키보드로 한글 혹은 영어를 입력할 수 있기 위해 제작하였습니다.

- 한글Char와 한글String으로 이루어져 있습니다.

- 한글Char의 경우 한글의 초, 중, 종성 및 모음 등을 파싱합니다.

- [참고한 코드 링크 1](http://plog2012.blogspot.kr/2012/11/c.html)

- [참고한 코드 링크 2](http://ehclub.co.kr/2484?category=658554)

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/blob/master/SHangul.cs)

---
## 작업 스타일

- 저는 체계적인 스타일을 좋아합니다. 폴더부터 코드 네이밍, 코드 변수 및 함수 정의 위치까지 다 정하는 편입니다.
  - 이에 대해 항상 부가 설명을 하자면, 프로그래밍은 개발보다 유지보수 비용이 훨씬 크다고 생각합니다.
  - 또한, **담당자에게 물어보지 않고 스스로 예측 가능 하는 것을 최고로 생각**하고 있습니다.
    - 부가 설명을 더 하자면, 현실의 방의 예를 듭니다. 아무리 어두운 방이라도, 우리는 항상 **문고리가 어느 위치쯤에 있는지, 전등 스위치가 문 주변의 벽에 있다는 점을 누가 알려주지 않아도 스스로 예측**합니다.
  - 이런식으로 네이밍을 하면, 어느 클래스의 이름만 보아도 어느 계층의 어느 패턴을 상속받은 클래스라는 점, 어느 함수의 이름만 보아도 어디쯤의 위치에 있는 지를 **담당자에게 물어보지 않고 예측 할 수 있습니다.**

- 코드를 작성하기 전에 간단한 설계부터 하는 편입니다.

- 저의 모토는 ``올바른 방향으로 고민하여, 실천할 수록 실력이 늘 수밖에 없다.``입니다.
  - 고민이 올바른 방향으로 가는지 체크하기 위해 항상 관련 개발 서적을 읽습니다.

- 또한 ``깨진 창문``을 주의하기 위하여 클린코드의 ``나중은 오지 않는다``(르블랑의 법칙)와, ``왔을 때보다 좀 더 깔끔하게``보이 스카웃 법칙을 따르도록 노력하는 편입니다.
 - 이에 관련된 SlideShare [링크](https://www.slideshare.net/jrogue/ss-38012889)

### 폴더 규칙
![](https://postfiles.pstatic.net/MjAxODA1MDdfMjEx/MDAxNTI1NjgwODA1NzIy.JJjPfUfZsb1h4pMe43rVI-XsYHAQEx_nH_JjkYERjpgg.__21JNDQ_dOTeevTJvh-lr6Zo2KspJDr1eeShZYE3Nog.JPEG.strix13/bandicam_2018-05-07_17-12-50-429.jpg?type=w773)
![](https://postfiles.pstatic.net/MjAxODA1MDdfMjg4/MDAxNTI1NjgwODA1NzIy.uMpNjBJQaIbqjvwyUUwcbhV1TleJJJHqe7GgGSjnAKcg.kK584c7PcZVdka3dr3G-eh1s2_g2JqpOrqjsL8DRkQcg.JPEG.strix13/bandicam_2018-05-07_17-13-02-930.jpg?type=w773)
![](https://postfiles.pstatic.net/MjAxODA1MDdfMjA4/MDAxNTI1NjgwODA1NzIw.1IL3CGeKGoIplF66PTuw4nw43xC7UVjy_D8ax7z6ZZEg.FhyyV8wpxCIKHGupwNiGsnOz2K3uN_61m6D0RPpLmz8g.JPEG.strix13/bandicam_2018-05-07_17-13-13-657.jpg?type=w773)

- 저의 오픈 소스인 다른 유니티 프로젝트를 보시면, 폴더 규칙이 있는것을 보실 수 있습니다.

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjcx/MDAxNTI1NjgwOTE3ODc2.tcsBDP5EHuki3zJ6lAQwXkP-s1UsQ3DpZGbowZvwE_8g.qzWFvQFdCb2E15HGyyu_R6OGBEza84LUBE-D1Zt3gZMg.JPEG.strix13/bandicam_2018-05-07_17-14-40-530.jpg?type=w773)
![](https://postfiles.pstatic.net/MjAxODA1MDdfMTAw/MDAxNTI1NjgwOTE3ODc4.68ZbbOjtquIIK6L9hCaVBW-yzaCJMCJIiwhGtmfjMH8g.K2tB4W2q3Ffo1GdtwSVeGhKyB-3saIgRwLMYEQzqEQQg.JPEG.strix13/bandicam_2018-05-07_17-14-50-448.jpg?type=w773)

- 최상위 폴더는 항상 숫자를 붙이는 편입니다.
  - Asset을 활용할 경우 최상위 폴더에 없으면 동작 안하는 경우가 있습니다.
  - 이 때문에, 폴더의 위치가 변경될 수 있어서, 위치를 고정하기 위해 숫자를 붙입니다.

- 그 하위 폴더는 숫자를 붙이지 않습니다.
  - 항상 폴더를 생성하지 않고, 하위 항목이 5개 이상일 경우 폴더를 생성합니다.
  - 코드 폴더나, 프리팹, 리소스 등의 특정 폴더의 경우 예외입니다.

### 코드

![](https://postfiles.pstatic.net/MjAxODA1MDdfNjAg/MDAxNTI1NjgyMTk1OTIw.KWb2jh-HlZxIpGvqURDhOToGnoJTmN0PCbjZLzgQUm0g.4hdmcELocTlAEAKNhnFRIvQpRDbemTH05Nik6nNYlYMg.JPEG.strix13/bandicam_2018-05-07_17-36-23-518.jpg?type=w773)

ㄴ 저의 Visual Studio - Unity Item Template 입니다.

- 제가 작성한 다른 코드를 보시면 주석으로 여기저기 무슨 구간이라 표시되있는 것이 있습니다. 이것은 Visual Studio의 Item Templates를 이용하였습니다.
 - Item Template의 형식도 계속 업데이트 됨에 따라 형식이 제각각입니다.

- Template에 작성자와 Library 링크를 적은 이유는 코드에 책임감을 부여하기 위해서입니다.

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjM4/MDAxNTI1NjgyMzE3Nzgw.yGbOqrWKQ8zXPQuGo4GGgAF3CFvL0wNJdtdMWfKFQEcg.1zO8gO6AaNemOFtekPgnoqSigNlRyWJ0jOE4tuYiqaYg.JPEG.strix13/bandicam_2018-05-07_17-38-28-987.jpg?type=w773)

- 최근에 작성한 코드부터는 Test 코드를 작성하기 시작했습니다.

### 읽은 책 목록
- C# 및 유니티 관련
  - 절대강좌 유니티5, 뇌를 자극하는 C# 5.0 프로그래밍, Effective C#, C# 코딩의 기술 실전편

- 자료구조 및 알고리즘
  - 누워서 읽는 알고리즘, 윤성우의 열혈 자료구조, 뇌를 자극하는 알고리즘

- 코드 스타일
  - 클린 코드, 리펙토링, 좋은 코딩 나쁜 코딩

- 아키텍트
  - HeadFirst DesignPattern, 소프트웨어 설계 테크닉, 프로그래밍 패턴

- 그 외
  - 객체지향의 사실과 오해, 게임TCP/IP 윈도우 소켓 프로그래밍, 뇌를 자극하는 윈도우즈 시스템 프로그래밍, TDD 실천법과 도구, 익스트림 프로그래밍, 소프트웨어 장인, 실용주의 프로그래머

---
읽어주셔서 감사합니다.

### 개발자 연락처

- 카카오톡 ( 유니티 개발자 모임 ) : https://open.kakao.com/o/gOi17az

- 디스코드 ( 유니티 개발자 모임 ) : https://discord.gg/9EGrJap
