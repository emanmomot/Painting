using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

	public static PlayerInput singleton;

	public float brushChangeRate;
	public float minBrushSize;
	public float maxBrushSize;

	public bool isTriggerHeld { get; private set; }
	private float brushSizeRange;

	private float brushSize;

	private bool isJoystick;
	private bool joystickInit;

	void Awake() {
		singleton = this;
	}

	// Use this for initialization
	void Start () {
		brushSizeRange = maxBrushSize - minBrushSize;

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
		if (isTriggerHeld) {
			TexturePainter.singleton.SetBrushSize (brushSize * paintAxis);
		}

		if (Input.GetButton ("IncBrushSize") && brushSize < maxBrushSize) {
			brushSize += brushChangeRate;
		} else if (Input.GetButton ("DecBrushSize") && brushSize > minBrushSize) {
			brushSize -= brushChangeRate;
		}

		TexturePainter.singleton.UpdateTP ();

		if (Input.GetButtonDown ("EyeDrop") && TexturePainter.singleton.IsOnPaintableObject()) {
			TexturePainter.singleton.EyeDrop ();
		}

		// return to regular cursor size after finalizing the stroke
		if (!isTriggerHeld) {
			TexturePainter.singleton.SetBrushSize (brushSize);
		}



		/*if (!isTriggerHeld && paintAxis > 0 && TexturePainter.singleton.IsOnPaintableObject()) {
			isTriggerHeld = true;
			TexturePainter.singleton.SetBrushSize (brushSize * paintAxis);
			TexturePainter.singleton.StartPaint ();
		}

		if (isTriggerHeld) {
			if (paintAxis > 0) {
				TexturePainter.singleton.SetBrushSize (brushSize * paintAxis);
			} else {
				isTriggerHeld = false;
				if (TexturePainter.singleton.IsPainting ()) {
					TexturePainter.singleton.EndPaint ();
				}
			}
		} else {
			TexturePainter.singleton.SetBrushSize (brushSize);
		}*/

	}
}
