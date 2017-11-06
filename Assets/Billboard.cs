using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void LateUpdate () {
		if (Camera.main != null) {
			transform.LookAt (Camera.main.transform.position, Camera.main.transform.up);
			//transform.Rotate (transform.up, 180);
		}
	}
}
