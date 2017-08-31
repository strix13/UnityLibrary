using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEvent : MonoBehaviour
{
	public float f_Time;

	void Start () {
		Destroy (this.gameObject, f_Time);
	}
}
