using UnityEngine;
using System.Collections;

namespace Lean.Touch
{
	public class LeanSwipeDirection4 : MonoBehaviour
	{
		public static LeanSwipeDirection4  instance;
		public Animator              Ani_Player;
		bool                         b_Jump1,b_Jump2;
		bool                         b_Sliding;
		bool                         b_Invincible;
		public bool                  b_Boost;
		public bool                  b_Obstacle_SpeedUp;
		public UILabel               test;
		public GameObject            g_World;

		void Awake()
		{
			instance = this;
			b_Jump1 = false;
			b_Jump2 = false;
		}

		void Update()
		{
			if (b_Sliding) {
				Ani_Player.SetBool ("Slided", true);
				GetComponent<BoxCollider2D> ().offset = new Vector2 (-43, -26);
				GetComponent<BoxCollider2D> ().size = new Vector2 (133, 51);
			} else {
		
					Ani_Player.SetBool ("Slided", false);
					GetComponent<BoxCollider2D> ().offset = new Vector2 (-43, 5);
					GetComponent<BoxCollider2D> ().size = new Vector2 (116, 117);

			}
			
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSwipe += OnFingerSwipe;
		}
	
		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSwipe -= OnFingerSwipe;
		}
	
		public void OnFingerSwipe(LeanFinger finger)
		{
				var swipe = finger.SwipeScreenDelta;
			
				if (swipe.x < -Mathf.Abs(swipe.y))//왼쪽
				{
				Debug.Log (swipe.x);
				}
			
				if (swipe.x > Mathf.Abs(swipe.y))//오른쪽
				{
					
				}
			
				if (swipe.y < -Mathf.Abs(swipe.x))//아래
				{
				    b_Sliding = true;
					Ani_Player.SetBool ("Slided", true);
				    StartCoroutine (Swipe_test ());
				}
			
				if (swipe.y > Mathf.Abs(swipe.x))//위
				{
					if(b_Jump2)
					{		
						GetComponent<Rigidbody2D>().velocity = (transform.up*3);
						b_Jump2 = false;
					}
					if(b_Jump1)
					{						
						GetComponent<Rigidbody2D>().velocity = (transform.up*3);
						b_Jump1 = false;
						b_Jump2 = true;
					}
				}
		}

		public void Jump_Button()
		{
			if(b_Jump2)
			{		
				GetComponent<Rigidbody2D>().velocity = (transform.up*3);
				b_Jump2 = false;
			}

			if(b_Jump1)
			{						
				GetComponent<Rigidbody2D>().velocity = (transform.up*3);
				b_Jump1 = false;
				b_Jump2 = true;
			}	
		}

		public void Slide_OnPress()
		{
			b_Sliding = true;
		}

		public void Slide_OnRelease()
		{
			b_Sliding = false;
		}

		void OnCollisionEnter2D(UnityEngine.Collision2D Coll)
		{
			if(Coll.gameObject.CompareTag("Ground"))
			{	
//				b_Sliding = false;
				b_Jump1 = true;
				b_Jump2 = false;
			}
		}

		void OnTriggerEnter2D(Collider2D Coll)
		{
			if (Coll.gameObject.GetComponent<Item> ()) 
			{
				if (Coll.gameObject.GetComponent<Item> ().b_Boost){
					StartCoroutine (Boost ());
				}else if (Coll.gameObject.GetComponent<Item> ().b_TimeUp){
					RunGM_Jump.instance.f_Time += 0.3f;
				}else if (Coll.gameObject.GetComponent<Item> ().b_ObstacleSpeed){

				}
			}

			if(!b_Invincible)
			{
		     	if (Coll.GetComponent<Planet> ()) 
	     		{
		    		iTween.ShakePosition(g_World, iTween.Hash("x", 0.02f, "y", 0.02f, "time", 1f)); 
			    	StartCoroutine (Testtext ());
				    test.text = "소행성에 부딪혔다";
				    Debug.Log ("소행성에 부딪혔다");
				    if (RunGM_Jump.instance.i_count > 5)
				      RunGM_Jump.instance.i_count = 5;

					StartCoroutine (InvincibleTime ());
			    }

     			if (Coll.gameObject.GetComponent<Obstacle> ())
				{
     				if (Coll.gameObject.GetComponent<Obstacle> ().b_Trampoline) 
					{
				    	GetComponent<Rigidbody2D> ().velocity = (transform.up * 5.5f);
			    	}
					if (!Coll.gameObject.GetComponent<Obstacle> ().b_Up && !Coll.gameObject.GetComponent<Obstacle> ().b_Trampoline) 
					{
				    	iTween.ShakePosition (g_World, iTween.Hash ("x", 0.02f, "y", 0.02f, "time", 1f)); 
				    	StartCoroutine (Testtext ());
				    	test.text = "아래 부딪혔다";
				     	Debug.Log ("아래 부딪혔다");
				    	RunGM_Jump.instance.i_count = 5;

						StartCoroutine (InvincibleTime ());
					} else if (Coll.gameObject.GetComponent<Obstacle> ().b_Up && !Coll.gameObject.GetComponent<Obstacle> ().b_Trampoline) {
				    	iTween.ShakePosition (g_World, iTween.Hash ("x", 0.02f, "y", 0.02f, "time", 1f)); 
				    	StartCoroutine (Testtext ());
				    	test.text = "위에 부딪혔다";
				    	Debug.Log ("위에 부딪혔다");
				    	RunGM_Jump.instance.i_count = 5;

						StartCoroutine (InvincibleTime ());
				}
			}
		}
	}

		IEnumerator Swipe_test()
		{
			yield return new WaitForSeconds (0.7f);
			b_Sliding = false;		
		}

		IEnumerator Testtext()
		{
			yield return new WaitForSeconds (1f);
			test.text = " ";
		}

		IEnumerator Boost()
		{
			yield return new WaitForSeconds (0f);
			b_Boost = true;
			RunGM_Jump.instance.i_count = 20;
			GetComponent<Rigidbody2D> ().simulated = false;
			transform.localPosition = new Vector2 (-300, - 38);
			yield return new WaitForSeconds (5f);
			StartCoroutine (InvincibleTime ());
			b_Boost = false;
			GetComponent<Rigidbody2D> ().simulated = true;
		}

		IEnumerator InvincibleTime()
		{
			yield return new WaitForSeconds (0f);
			b_Invincible = true;
			yield return new WaitForSeconds (3f);
			b_Invincible = false;
		}

		IEnumerator Obstacle_Speed()
		{
			yield return new WaitForSeconds (0f);
			b_Obstacle_SpeedUp = true;
			yield return new WaitForSeconds (5f);
			b_Obstacle_SpeedUp = false;
		}
	}
}