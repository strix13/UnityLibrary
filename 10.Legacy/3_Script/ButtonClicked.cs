using UnityEngine;
using System.Collections;

public class ButtonClicked : MonoBehaviour {

    private GUITexture _thisObjBtn;
    public GameObject _target;
    public string _functionName = "Regame";

	// Use this for initialization
	void Start () {

        _thisObjBtn = gameObject.GetComponentInChildren<GUITexture>();

    }
	
	// Update is called once per frame
    void Update()
    {

        if( Input.GetMouseButtonDown(0))
        {
            if(_thisObjBtn.HitTest(Input.mousePosition))
            {

                if (_target != null)
                {
                    _target.SendMessage(_functionName, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
