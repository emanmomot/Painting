using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECF;

public class ECFTest : MonoBehaviour {

	ECF.ECF ecf;

	// Use this for initialization
	void Start () {
		CanvasContext ctx = new CanvasContext ();
		ecf = new ECF.ECF (Screen.width, Screen.height, ctx);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			ecf.mouseDownCallback (Input.mousePosition.x, Input.mousePosition.y);
		} else if (Input.GetMouseButton (0)) {
			ecf.mouseMoveCallback (Input.mousePosition.x, Input.mousePosition.y);
		}
		if (Input.GetMouseButtonUp (0)) {
			ecf.mouseUpCallback ();
		}
	}
}
