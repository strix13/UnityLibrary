# 워킹 스탠다드

- 이 문서의 깃허브 링크 :
https://docs.google.com/document/d/1X8nIU0anstawLqf_pTnylCFLxgAAMJSQILIOZ89hFTs
---

제가 작업한 작업물들은 이 문서에 있는 워킹 스탠다드를 따라 작업하였습니다.
워킹 스탠다드는 제가 경험하면서 효율적인것은 따르고, 따르다가 비효율적인 것은 버리기도 하였습니다.

---
### 목차
1. 유니티 - 프로젝트 뷰
2. 유니티 - 하이어라키 뷰
3. 스크립팅
4. 스크립팅 - 프로젝트 레벨
5. 스크립팅 - 라이브러리 레벨

---
## 1. 유니티 - 프로젝트 뷰

이 항목은 Asset 폴더 내의 내부 폴더, 파일 네이밍 및 배치에 대한 규칙입니다.

1-1. 폴더 이름은 띄어쓰기 `_`를 제외한 괄호 등의 특수문자를 포함하지 않아야 합니다. ( 한글도 괜찮습니다. )

1-2. 리소스 관련 폴더 이름은 날짜 + 카테고리 순으로 명명해주시기 바랍니다.

	- 0530_주인공캐릭터
	- 0102_인벤토리UI

1-3. 동일한 리소스 관련 카테고리가 있는 경우, 뒤에 `_V (Version) + 2 ~` 를 붙여주시기 바랍니다.

	- 0103_인벤토리UI_V2
	- 0104_인벤토리UI_V3

1-4. 파일 이름은 되도록이면 띄어쓰기, `_`를 제외한 괄호 등의 특수문자를 포함하지 않아야 합니다.

1-5. 파일 이름은 되도록이면 기능보다 성격에 맞게 작성해주시기 바랍니다.

	- UI 버튼 이미지의 경우
		= X ) 게임플레이버튼.jpg
		= O ) 파란사각형버튼.jpg

		= X ) 인벤토리UI.jpg
		= O ) 검은색프레임.jpg

	- 애니메이션의 경우
		= X ) 공격애니메이션.anim
		= O ) 한손베기애니메이션.anim

		= X ) 캐릭터_스킬1.anim
		= O ) 점프후착지해서내려찍기.anim

	- 리소스는 항상 변경되기도 하고, 기존의 리소스를 다른 기능에 사용하기도 하기 때문입니다.


1-6. 스크립트는 한 폴더 내에 있어야 합니다.

1-7. 정렬 기준은 한 폴더 내에 파일이 5개 이상이며, 분류 가능한 경우 하위 카테고리 성격의 폴더를 만들어 주시기 바랍니다.

1-8. 에셋 스토어에서 구매한 에셋의 경우, 가급적이면 한 에셋 만 들어있는 폴더 내에 넣어주시기 바랍니다.

1-9. Resources 폴더 내에는 가급적이면 Resources 함수를 사용하는 오브젝트만 넣어주시기 바랍니다.

	- Resources에 들어가는 리소스는 빌드에 포함하여 빌드 용량이 많아집니다.

1-10. Asset 폴더의 최상단에는 되도록이면 파일이 없어야 합니다. 카테고리 성격의 폴더를 만들어 주시기 바랍니다.

1-11. 씬 파일의 경우 파일이 적으면 Asset폴더의 최상단에 배치해주시기 바랍니다.


---
## 2. 유니티 - 하이어라키 뷰

이 항목은 게임 내의 오브젝트 네이밍 및 부모자식 관계도에 대한 규칙입니다.

2-1. 오브젝트의 이름에 `한글 및 영어, 숫자`, "` `"(공백), "`_`" 등을 제외한 문자는 가급적이면 넣지 않습니다.

2-2. 게임 오브젝트 이름은 해당 오브젝트의 대표격인 컴포넌트의 이름으로 명명합니다.

2-3. 다른 클래스 참조는 되도록이면 하이어라키 구조를 이용합니다. 게임 오브젝트 역시 이에 맞게 배치합니다.

	- SomthingManager
		ㄴ SomthingImportant
		ㄴ SomthingNotImportant
		ㄴ SomthingNotImportant2

2-4. UI의 경우 게임 오브젝트 이름은 UI Element 성격을 접두어로 붙입니다.

	- UIButton의 경우
		Button_Somthing

	- UIImage의 경우
		Image_Somthing

2-5. 하이어라키 순서는 되도록이면 자주 작업하는 것이 위쪽으로 배치합니다.

2-6. 하이어라키 레벨에 분류 가능한 오브젝트가 많은 경우, 카테고리 성격의 빈 오브젝트를 만들어 분류합니다.

---
## 3. 스크립팅

스크립팅의 경우 프로젝트 레벨과 라이브러리 레벨로 나누어져 있습니다.

- 프로젝트 레벨은 프로젝트에 종속되는 코드를 뜻합니다.
- 라이브러리 레벨은 프로젝트에 종속되지 않는 코드를 뜻합니다.


---
## 4. 스크립팅 - 프로젝트 레벨
클래스 명명은 라이브러리의 경우 클래스 + 성격 + 기능( 명사 )로,
프로젝트의 경우 프로젝트 명 + 성격 + 기능 순으로 작명합니다.

라이브러리 : C (Class) + 성격 ( Manager(싱글톤), Component(단일 객체로서 기능을 발휘) ) ( 기능 + (er) )
```
= CManagerNetworkUDP	: C = 클래스 , Manager = 싱글톤 , Network UDP - UDP 기능 관련
= CCompoAddForce	: C = 클래스 , Compo = 단일 객체로서 기능을 발휘,
				  AddForce - AddForce 기능 관련
```

프로젝트 : 프로젝트 이니셜 코드 2글자 + 성격 ( Manager(싱글톤), Component(단일 객체로서 기능을 발휘) ) ( 기능 + (er) )
= 프로젝트 명이 SingVR의 경우	       : SV
		= 프로젝트 명이 ZiwooShooting의 경우  : ZS

 클래스가 abstract 혹은 상속을 통해서 재기능을 발휘해야 할 경우 접미어로 Base를 붙입니다.
```
	- CManagerFrameWorkBase
	- CSingletonMonoBase
```
변수 명명은 헝가리안을 기반으로 하지만, 유니티 C#에만 있는 경우 조금 변경된 스타일로 작성합니다.
```
	- 기본형의 경우 ( 유니티 C# 기준 )
		int : i, string : str, Vector : vec, bool : b

	- Class, Struct의 경우
		pSomthingClass, pSomthingStruct

	- 멤버 변수의 경우
		_iSomthingInt, _pSomthingStruct
```
자료구조나 디자인 패턴 등의 기존 소프트웨어 단어를 사용하면 그 단어를 포함합니다.
```
	- FSM을 사용한 클래스의 경우
		CFSM_PlayerStatus, CState_PlayerIdle

	- Singleton을 사용한 클래스의 경우
		CSingletonMonoBase

	- List, Stack 등의 경우
		= List<int> listSomthing
		= Stack<int> stackSomthing

	- Dictionary 의 경우 예외로 map을 사용합니다.
		Dictionary<int, int> mapSomthing
```

프로퍼티의 경우 p_ + 변수 명명법으로 작성합니다.

외부에 노출되는 변수는 되도록이면 프로퍼티로 작성합니다.

```

// public int iMinute;
public int p_iMinute { get; protected set; }

```


public Getter의 경우 함수보다 되도록이면 프로퍼티로 작성합니다.

```

// public int GetMinute();
public int p_iMinute { get; protected set; }

```

메소드 이름은 접두어가 있는경우 접두어부터 + 동사 + 명사 순으로 작명합니다.

	ㄱ. public 함수의 경우 접두어 Do
		DoPlayEffect, DoPlaySound 등

	ㄴ. non-public Getter, Setter 함수의 경우
		GetSomthing, SetSomthing

	ㄷ. 후크 혹은 이벤트 함수의 경우 접두어 On
		OnClickButton, OnPressButton

	ㄹ. 그 외 protected & private의 경우
		Process_Somthing


var는 반복문과 그에 관련된 작업에만 되도록 작성합니다.

상수는 const_ + 변수 명명법으로 작성합니다.

매직넘버를 쓰지 말고 상수를 선언합니다.

```

{
	// int iClockMaximum = 24;

	const int const_iClockMaximum;
	int iClockMaximum = const_iClockMaximum;
}

```


인터페이스의 명명법의 경우 I (Interface)를 붙이며, 변수는 p로 작성합니다.

인터페이스를 상속받는 함수의 경우 해당 인터페이스 타입명을 접두어로 붙입니다.

```

	interface IClockListener
	{
		void IClockListener_OnChangeClock(int iHour, int iMinute);
	}

```

Enum의 명명법의 경우 E (Enum)을 붙이며, 변수는 e로 작성합니다.

Enum Flag의 경우 형식 명에 Flag를 포함해야 합니다. 변수 역시 Flag를 접미어로 붙입니다.

```

	[System.Flags]
	enum EExportTypeFlags
	{
		CSV,
		TXT,
		DB
	}

	EExportTypeFlags _eExportTypeFlags;

```


Delegate의 명명법의 경우 On + 기능으로 작명합니다.

event 명명법의 경우 p_Event_ + 기능으로 작명합니다.

```

	delegate void OnChangeClock(int iHour, int iMinute);

	event OnChangeClock p_Event_OnChangeClock;

```


두개의 문자 이상인 경우 머리글자로 된 첫 번째 문자만 대문자로 표시합니다.

	ㄱ. iSessionCount


클래스, 변수, 메소드의 순서는 규약에 있는 포멧을 지킵니다.

메소드 전개 중, 메소드 내부에 체크가 필요할 시 체크부터 전개 후 리턴합니다. ( 중괄호를 최대한 줄입니다. )

```

void Process_Clock(int iHour, int iMinute)
{
	if(iHour > 24 || iMinute > 60)
		return;

	Process_Clock_Hour(iHour);
	Process_Clock_Minute(iMinute);
}

```


함수에서 Null을 리턴하는 경우 접미어로 _OrNull을 붙입니다.

	ㄱ. GetSomthing_OrNull()

---
## 5. 스크립팅 - 라이브러리 레벨


---
참고 목록
- 포프님 코딩 스탠다드 (한글 번역): http://lonpeach.com/2017/12/24/CSharp-Coding-Standard/#1-%ED%81%B4%EB%9E%98%EC%8A%A4-%EB%B0%8F-%EA%B5%AC%EC%A1%B0%EC%B2%B4%EC%97%90-%ED%8C%8C%EC%8A%A4%EC%B9%BC-%EC%BC%80%EC%9D%B4%EC%8A%A4-%EC%82%AC%EC%9A%A9
- 포프님 코딩 스탠다드 (원문)	 : https://docs.google.com/document/d/1ymFFTVpR4lFEkUgYNJPRJda_eLKXMB6Ok4gpNWOo4rc/edit

---
#### 라이브러리로 돌아가기
https://github.com/strix13/UnityLibrary
