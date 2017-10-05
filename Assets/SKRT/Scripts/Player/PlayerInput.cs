using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

	public static PlayerInput singleton;

	public float brushChangeRate;
	public float minBrushSize;
	public float maxBrushSize;

	public bool isTriggerHeld { get; private set; }

	private float brushSize;

	private bool isJoystick;
	private bool joystickInit;

	void Awake() {
		singleton = this;
	}

	// Use this for initialization
	void Start () {
		if (Input.GetJoystickNames ().Length > 0) {
			isJoystick = true;
		}

		brushSize = (minBrushSize + maxBrushSize) / 2.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		float paintAxis = Input.GetAxis ("Paint");

		if (isJoystick) {
			// on mac joystick axis is init to 0 instead of -1
			if (!joystickInit) {
				if (paintAxis == 0) {
					paintAxis = -1;
				} else {
					joystickInit = true;
				}
			}

			paintAxis = (paintAxis + 1) / 2.0f;
		}

		isTriggerHeld = paintAxis > 0;

		if (TexScaleTool.singleton.isOpen) {
			isTriggerHeld = false;
		}

		if (isTriggerHeld) {
			TexturePainter.singleton.SetBrushSize (brushSize * paintAxis);
		}

		// if the tool is open we shouldn't change brush size
		if (!TexScaleTool.singleton.isOpen) {
			if (Input.GetButton ("IncBrushSize") && brushSize < maxBrushSize) {
				brushSize += brushChangeRate;
			} else if (Input.GetButton ("DecBrushSize") && brushSize > minBrushSize) {
				brushSize -= brushChangeRate;
			}
		}

		TexturePainter.singleton.UpdateTP ();

		if (Input.GetButtonDown ("EyeDrop") && TexturePainter.singleton.IsOnPaintableObject()) {
			TexturePainter.singleton.EyeDrop ();
		}

		// return to regular cursor size after finalizing the stroke
		if (!isTriggerHeld) {
			TexturePainter.singleton.SetBrushSize (brushSize);
		}

	}
}
