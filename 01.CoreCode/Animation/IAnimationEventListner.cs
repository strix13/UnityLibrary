using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-21 오후 8:49:28
   Description : 
   
	Animation 에셋에 Event의 Function Name을 EventAnimation 로 할당한뒤
	매개변수로 Int값을 EAnimationEvent에 맞게 넘겨야 한다.

   Edit Log    : 
   ============================================ */

public enum EAnimationEvent
{
	None = 0,
	AnimationStart = 1,

	AttackStart = 2,
	AttackFinish = 3,

	AnimationFinish = 4,
}

public interface IAnimationEventListner
{
	void EventAnimation(EAnimationEvent eMessage);
}
