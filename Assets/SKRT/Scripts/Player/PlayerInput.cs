using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

	public float brushChangeRate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			TexturePainter.singleton.Paint ();
		}

		if (Input.GetKey (KeyCode.Q)) {
			GUIManager.singleton.ReduceBrushSize (brushChangeRate);
		}

		if (Input.GetKey (KeyCode.E)) {
			GUIManager.singleton.IncreaseBrushSize (brushChangeRate);
		}

	}
}
