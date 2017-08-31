using UnityEngine;
using System.Collections;

public class Effect_del : MonoBehaviour {

    public float _timerForDel;
    public float _timerForDelLim;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        _timerForDel += Time.deltaTime;
        if (_timerForDel > _timerForDelLim)
        {
            Destroy(gameObject);
        }

	
	}
}
