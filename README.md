# 환영합니다
- 이 프로젝트는 Unity C# 오픈소스 라이브러리 프로젝트입니다.

- 이 프로젝트는 게임을 보다 빠르고 편하게 만들기 위한 클래스와 함수를 제공합니다.

- 이 프로젝트는 유니티 내 어떤 프로젝트에도 종속되지 않습니다.

- 이 프로젝트는 프로그래머를 대상으로 설명합니다.

-  깃허브 링크 :
https://github.com/strix13/UnityLibrary

- 유니티 워킹 스탠다드 링크 :
https://docs.google.com/document/d/1X8nIU0anstawLqf_pTnylCFLxgAAMJSQILIOZ89hFTs


---
## 작업 스타일

- 저는 체계적인 스타일을 좋아합니다. 폴더부터 코드 네이밍, 코드 변수 및 함수 정의 위치, 유니티 오브젝트 네이밍 및 하이어라키까지 정하는 편입니다.
  - 이에 대해 항상 부가 설명을 하자면, 프로그래밍은 개발보다 유지보수 비용이 훨씬 크다고 생각합니다.
  - 또한, **담당자에게 물어보지 않고 스스로 예측 가능 하는 것을 최고로 생각** 하고 있습니다.
    - 부가 설명을 더 하자면, 현실의 방의 예를 듭니다. 아무리 어두운 방이라도, 우리는 항상 **문고리가 어느 위치쯤에 있는지, 전등 스위치가 문 주변의 벽에 있다는 점을 누가 알려주지 않아도 스스로 예측** 합니다.
  - 이런식으로 네이밍을 하면, 어느 클래스의 이름만 보아도 어느 계층의 어느 패턴을 상속받은 클래스라는 점, 어느 함수의 이름만 보아도 어느 위치에 있는 지를 **스스로 예측 할 수 있습니다.**

- ``깨진 창문``을 주의하기 위하여 클린코드의 ``나중은 오지 않는다``(르블랑의 법칙)와, ``왔을 때보다 좀 더 깔끔하게``보이 스카웃 법칙을 따르도록 노력하는 편입니다.
 - 이에 관련된 SlideShare [링크](https://www.slideshare.net/jrogue/ss-38012889)


### 폴더 규칙
![](https://postfiles.pstatic.net/MjAxODA1MDdfMjEx/MDAxNTI1NjgwODA1NzIy.JJjPfUfZsb1h4pMe43rVI-XsYHAQEx_nH_JjkYERjpgg.__21JNDQ_dOTeevTJvh-lr6Zo2KspJDr1eeShZYE3Nog.JPEG.strix13/bandicam_2018-05-07_17-12-50-429.jpg?type=w773)
![](https://postfiles.pstatic.net/MjAxODA1MDdfMjg4/MDAxNTI1NjgwODA1NzIy.uMpNjBJQaIbqjvwyUUwcbhV1TleJJJHqe7GgGSjnAKcg.kK584c7PcZVdka3dr3G-eh1s2_g2JqpOrqjsL8DRkQcg.JPEG.strix13/bandicam_2018-05-07_17-13-02-930.jpg?type=w773)

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

ㄴ 제가 작성하고 사용하는 Visual Studio - Unity Item Template 입니다.

---
# 라이브러리 기능 목록

## 루트 클래스

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjEx/MDAxNTI1NjY0ODU2NTc2.pl7EYZNbYzETHR2t9lPE6O2b6gp9eBCOSWRDpAQmvrEg.km7IoWpo9ucfK6lREhRdZ_gEbi_QIkF0QLRt_lxJZ_0g.JPEG.strix13/StrixLibrary_-_%EB%A3%A8%ED%8A%B8_%ED%81%B4%EB%9E%98%EC%8A%A4.jpg?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.



[![Video Label](http://img.youtube.com/vi/xuhKn5H6ck4/0.jpg)](https://www.youtube.com/watch?v=xuhKn5H6ck4=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다. ( 영상은 GetComponent Attribute를 통해 컴포넌트를 얻은 것과 Update Object 를 사용한 예시입니다.)



- 기존 Unity의 **MonoBehaviour의 기능을 확장** 시킨 루트 클래스입니다. ( MonoBehaviour 대용 )
- **루트 클래스의 주요 기능**
  - AwakeCoroutine, EnableCoroutine 기능
  - 외부에서 Awake 호출 ( 이미 Awake를 실행한 경우 한번 더 실행유무도 지원 )
  - GetComponentAttribute 지원 ( Awake, Inspector 등에서 할당하지 않고 Attribute로 한줄 작성 )
  - 자체적인 Update를 통해 Update를 사용하는 Object의 실시간 개수 파악과 퍼포먼스 개선

- [작성한 코드 및 좀 더 알아 보기 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/ObjectBase)

---
## UI 클래스 관계도

![](
https://blogfiles.pstatic.net/MjAxODA1MDVfNSAg/MDAxNTI1NDk2MjEzMzk4.YBGR9hSvFoGqEg5lUCeF346bvZ3x9EEgLQSfjWcyFPsg.DdcKOk5Ml-eeOorUOfqwJcJnscZGxqmvC_Ol40H9eZ4g.PNG.strix13/StrixLibrary_-_UI_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.png)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




- 구조는 크게 Manager, Panel로 이루어져 있으며 Manager는 Panel을 관리하는 싱글톤 패턴입니다.
- NGUI 버젼, UGUI 버젼을 구분하였으며 모두 같은 클래스와 인터페이스를 상속받게 하였습니다.
-
- **Manager 주요 기능**
  - Singleton - 언제 어디서든 호출 가능
  - Panel의 Show, Hide 함수 지원
      - Sorting Layer 관리
      - Show, Hide Animation Coroutine 지원
  -
  - Panel FadeIn / Out 효과 지원
  - 해당 Panel Class Get 지원

---
- Panel의 경우 NGUI로는 NGUIPanel, UGUI로는 Canvas 단위로 된 하나의 Window 단위 입니다.
-
- **Panel 주요 기능**
  - 하위 **UI Element의 이벤트 ( 버튼, DropItem, Input 입력 ) 등을 쉽게 override하여 쉽게 사용할 수 있도록 지원**
  - **UI Element를 쉽게 Get** 할 수 있도록 지원
  - OnShow / OnHide의 이벤트가 있으며, **Show / Hide 시 코루틴이 동작합니다. ( 애니메이션을 기다리기 위함, 코루틴 끝날 때 콜백 지원 )**

---
- [작성한 UI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/02.UI)
- [작성한 UGUI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/03.UGUI)
- [작성한 NGUI 베이스 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/04.NGUI)

---
## 로컬 라이징 시스템

![](
https://postfiles.pstatic.net/MjAxODA1MDdfMTUg/MDAxNTI1NjYxNjYzOTMw.uTGH7T0d2IFz7o5BHMhKqZndkZY1_XN_N9HF4FBF9VMg.hGGB_GJmzsuWrqreBhTW40fd806yUgnS4LFqL6gUlmkg.JPEG.strix13/StrixLibrary_-_%EB%A1%9C%EC%BB%AC%EB%9D%BC%EC%9D%B4%EC%A7%95_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




[![Video Label](http://img.youtube.com/vi/mLQMwqKgh4I/0.jpg)](https://www.youtube.com/watch?v=mLQMwqKgh4I=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.




- 로컬라이징의 경우 Key, Value를 가진 임의의 Text파일을 파싱한 데이터를 기반으로 동작합니다.

- 외부에서 Instance 요청 시 자동으로 생성되서 파일을 파싱 후, 자동으로 로컬라이징이 될 수 있게끔 작업하였습니다.

- 작성한 Text, Sprite 컴포넌트 외에 이벤트를 받고 싶으면 ILocalizeListner를 구현하고, Manager에 등록하면 됩니다.

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Localize)

---
## 사운드 시스템 클래스 관계도

![](
https://postfiles.pstatic.net/MjAxODA1MDZfODIg/MDAxNTI1NTg2NjI4NDg5.2-piThykc2EWXJVPdEQUx0FlQ9PoSANx5ZLP1S8-KWwg.vvcEPNS5G5_jlnjJqcXHSgF2I94o_bMPaWWPa4537BEg.JPEG.strix13/StrixLibrary_-_%EC%82%AC%EC%9A%B4%EB%93%9C_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




[![Video Label](http://img.youtube.com/vi/TN145PFwvkI/0.jpg)](https://www.youtube.com/watch?v=TN145PFwvkI=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.




- 사운드 시스템의 경우, SoundManager가 SoundSlot 클래스를 관리하며, 스크립트 혹은 Sound Player를 통해 사운드를 재생합니다.
-
- **사운드 매니져 주요 기능**
  - Singleton - 언제 어디서든지 호출 가능
  - SoundSlot을 통해 AudioClip 재생, SoundSlot은 풀링
  - 현재 설정된 BGM Volume, Game Effect Volume, 몇개의 SoundSlot이 플레이 중인지 지원 [ Editor Only ]
  - Sound의 Mute, FadeIn, Out 지원
  - 그룹을 설정하여 랜덤 Sound 플레이 기능

---
- **사운드 슬롯 주요 기능**
  - AudioSource 래핑한 클래스
  - Local Volume, AudioClip이 끝날 때 Event 통지
  - 현재 재생중인지, 총 몇초 중 몇초 플레이 중인지 지원 [ Editor Only ]
---
- **사운드 플레이어 주요 기능**
  - 인스펙터에 세팅하여 유니티 주요 이벤트 ( Awake, Enable, Disable 등 )시에 사운드 재생
  - Key(string) - AudioClip 로 세팅하여 한 플레이어에 임의의 AudioClip 실행 가능
  - 어떤 사운드 슬롯을 통해 실행되는지 확인 가능 [ Editor Only ]
  - 현재 재생중인지 지원 [ Editor Only ]

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Sound)

---
## 이펙트 시스템 클래스 관계도

![](
https://postfiles.pstatic.net/MjAxODA1MDZfMzUg/MDAxNTI1NjEwMDU3MjQz.az-Y7SxCGDV-jlb4YCi06dZMTi5DQpIbyeBIzaLJwJkg.kfdc4L_DxkRORmGBFbGdlu6vHzu1MqnzrZKj3CL_kZIg.JPEG.strix13/StrixLibrary_-_%EC%9D%B4%ED%8E%99%ED%8A%B8_%EC%8B%9C%EC%8A%A4%ED%85%9C_%ED%81%B4%EB%9E%98%EC%8A%A4_%EA%B4%80%EA%B3%84%EB%8F%84.jpg?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




[![Video Label](http://img.youtube.com/vi/OQ8UpqBUIJQ/0.jpg)](https://www.youtube.com/watch?v=OQ8UpqBUIJQ=0s)

### ㄴ 이미지를 클릭하시면 유튜브에서 비디오를 시청하실 수 있습니다.




- 이펙트 시스템은 사운드 시스템과 비슷한 설계입니다. 매니져와, 리소스 래퍼와, 플레이어로 이루어져 있습니다.

- Manager 주요 기능
  - Effect 풀링
  - 현재 몇개의 Effect가 플레이 중이고, 몇개를 생성했는지 지원
---
- CEffect 주요 기능
  - NGUI, Spine, Particle System을 지원.
  - 이펙트가 끝날 때 이벤트 통보를 지원.

- [작성한 코드 및 좀 더 알아 보기 링크 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Effect)

---
## 유니티 마크다운 뷰어

![](https://camo.githubusercontent.com/6651c1a8b118bb47b030aa998be60db73d057f8d/68747470733a2f2f706f737466696c65732e707374617469632e6e65742f4d6a41784f4441314d4456664d6a4d342f4d4441784e5449314e5445354e4459354e4441312e775252445f66685450675239423470716b71496151574b4e4d586b543435757a575f526753323272494c30672e45756153622d417733516552434b4e73795f4d6643357566475561456f6448674d666435335550766f724d672e4a5045472e737472697831332f62616e646963616d5f323031382d30352d30355f32302d31392d32302d3732392e6a70673f747970653d77373733)

#### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.


- 유니티 내에서 마크다운 문서를 읽기 위해 만들었습니다.
- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Editor/MarkdownViewer)

---
## 유니티 로그 래핑 클래스

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjcg/MDAxNTI1Njc5NTI2ODE1.x-IjrZGPCJ9jE_2Vbq-Z8V7RM47fA8ixYiP2mGsnbHQg.b41kzPpZpwXmpUiz0q69tnt3LehiBJ2zik8RpPxTCQQg.JPEG.strix13/bandicam_2018-05-07_16-52-00-886.jpg?type=w773)
![](
https://postfiles.pstatic.net/MjAxODA1MDdfMTc4/MDAxNTI1Njc5NDU5MzMz.4WXQCeIXmSfGTzxJ7HsOJr8adwepP0JlfNOIrT7vR70g.iXFhfhPCRYxKJVHB7oOmKfpU14Aei4eNv-GbfphEZp0g.PNG.strix13/%EB%A1%9C%EA%B7%B8.png?type=w773)

### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.




- 로그 작성자, 로그 레벨에 따라 로그를 출력하지 않는 필터링 기능을 지원합니다.

- **로그 발생 시 txt, csv, DB로 동시 출력할 수 있는 기능** 을 지원합니다.
  - 파일에 로그 발생 시간과 콜스택도 기록합니다.

- 차후 Conditional을 도입할 예정입니다.

- [작성한 코드 링크](https://github.com/KorStrix/Unity_DebugWrapper)

---
## 랜덤 관련 클래스
### 랜덤 매니져

- 랜덤 매니져는 IRandomItem 을 구현한 클래스에 대해 랜덤으로 리턴해주는 관리자입니다.

- **IRandomItem의 경우 랜덤확률을 구현하게끔 되어있으며, 랜덤확률에 따라 클래스를 리턴** 합니다.

- **랜덤모드로 Peek, Delete 모드를 지원** 합니다.
  - Peek의 경우 Random Item List에서 **아이템을 뽑을 때, List에 지장이 없습니다.**
  - Delete의 경우 List에서 **아이템을 뽑을 때 해당 아이템은List에서 제거됩니다.**

### 랜덤 2D 섹터

- 섹터의 경우, 인스펙터에서 몇 바이 몇 섹터인지, 섹터 당 사이즈가 몇인지를 설정합니다.

- 그 외 주변 섹터 체크옵션으로 주위 4타일(위 아래, 양 옆), 주위 8타일(4타일에서 대각선 4타일 추가)를 지원합니다.

- 섹터에게 랜덤 포지션을 요청 시, **직전의 섹터가 같은 섹터가 아니고, 주변 섹터도 아닌 섹터 내의 랜덤 위치만 리턴**합니다.

- **이펙트, 몬스터 스폰 등에 쓰입니다.** ( 스폰 시 주변에 골고루 나오게 하기 위한 로직 )

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/tree/master/01.CoreCodeV2/Random)

---
## 한글 파서

![](
https://postfiles.pstatic.net/MjAxODA1MDdfNjAg/MDAxNTI1NjgzNDYwNTkx.7LVx1ndhvjxdw2992EqJKUM_u6ZZSuYqaGKxwlNv-Cwg.ieXZO9IUNCEUPh_ut5VM-TEf1GaUJknekDBuKTyQUxIg.JPEG.strix13/bandicam_2018-05-07_17-57-24-601.jpg?type=w773)

#### ㄴ 이미지를 클릭하시면 확대하실 수 있습니다.



- VR, 모바일 등에서 커스터마이징 된 키보드로 한글 혹은 영어를 입력할 수 있기 위해 제작하였습니다.

- 한글Char와 한글String으로 이루어져 있습니다.

- 한글Char의 경우 한글의 초, 중, 종성 및 모음 등을 파싱합니다.

- [참고한 코드 링크 1](http://plog2012.blogspot.kr/2012/11/c.html)

- [참고한 코드 링크 2](http://ehclub.co.kr/2484?category=658554)

- [작성한 코드 링크](https://github.com/strix13/UnityLibrary/blob/master/SHangul.cs)

---
## 그 외

![](https://postfiles.pstatic.net/MjAxODA1MDdfMjM4/MDAxNTI1NjgyMzE3Nzgw.yGbOqrWKQ8zXPQuGo4GGgAF3CFvL0wNJdtdMWfKFQEcg.1zO8gO6AaNemOFtekPgnoqSigNlRyWJ0jOE4tuYiqaYg.JPEG.strix13/bandicam_2018-05-07_17-38-28-987.jpg?type=w773)

- 최근에 작성한 코드부터는 Test 코드를 작성하기 시작했습니다.

### 읽은 책 목록
- **C# 및 유니티 관련**
  - 절대강좌 유니티5, 뇌를 자극하는 C# 5.0 프로그래밍, Effective C#, C# 코딩의 기술 실전편

- **자료구조 및 알고리즘**
  - 누워서 읽는 알고리즘, 윤성우의 열혈 자료구조, 뇌를 자극하는 알고리즘

- **코드 스타일**
  - 클린 코드, 리펙토링, 좋은 코딩 나쁜 코딩

- **아키텍트**
  - HeadFirst DesignPattern, 소프트웨어 설계 테크닉, 프로그래밍 패턴

- **그 외**
  - 객체지향의 사실과 오해, 게임TCP/IP 윈도우 소켓 프로그래밍, 뇌를 자극하는 윈도우즈 시스템 프로그래밍, TDD 실천법과 도구, 익스트림 프로그래밍, 소프트웨어 장인, 실용주의 프로그래머


---
읽어주셔서 감사합니다.

### 개발자 연락처

- 카카오톡 ( 유니티 개발자 모임 ) : https://open.kakao.com/o/gOi17az

- 디스코드 ( 유니티 개발자 모임 ) : https://discord.gg/9EGrJap
