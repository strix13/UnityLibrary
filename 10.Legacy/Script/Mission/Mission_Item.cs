using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Item : MonoBehaviour {
	public bool                          b_Coin,b_Roulette,b_fuel;
	public bool                          b_Move=false;
	public bool                          b_Down = false;
	public GameObject                    g_Boomb;
	GameObject                           g_Effect_Parent;
	// Use this for initialization
	void Start () {
		g_Effect_Parent = GameObject.FindGameObjectWithTag ("MainCamera");
		if (b_Coin) {
			transform.localScale = new Vector3 (50, 50, 50);
		} else if (b_Roulette) {
			transform.localScale = new Vector3 (100, 100, 100);
		} else if (b_fuel) {
			transform.localScale = new Vector3 (0.7f, 0.7f, 0.7f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!MissionRoulette.instance.b_Roulette) 
		{
			if (!b_Move)
				transform.Translate (Vector2.down * 0.3f * Time.deltaTime);
			if (b_Down)
				transform.Translate (Vector2.down * Time.deltaTime);
		}
	}

	public IEnumerator Death()
	{
		yield return new WaitForSeconds (0f);
		GameObject Boom = Instantiate (g_Boomb, g_Effect_Parent.transform)as GameObject;
		Boom.transform.localPosition = transform.localPosition;
		Boom.transform.localScale = new Vector2 (82,82);
		Destroy (gameObject);
	}
}
