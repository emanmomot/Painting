using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TexScaleTool : MonoBehaviour {

	public static TexScaleTool singleton;

	public GameObject toolGUI;
	public GameObject referenceObj;
	public GameObject horizScaleGUI;
	public GameObject vertScaleGUI;

	public Text xScaleText;
	public Text yScaleText;
	public InputField xScaleInput;
	public InputField yScaleInput;

	public float scaleChangeRate;

	public bool isOpen { get; private set; }

	private PaintableObject scaleObj;
	private Transform playerTransform;

	private bool scaleHoriz = true;
	private bool scaleVert = true;

	private LockMouse lockMouse;

	private Vector3 lastScale;

	void Awake() {
		singleton = this;
	}

	// Use this for initialization
	void Start () {
		playerTransform = PlayerInput.singleton.transform;
		lockMouse = playerTransform.GetComponentInChildren<LockMouse> ();
		referenceObj.SetActive (false);
		toolGUI.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("TexScaleTool")) {
			isOpen = !isOpen;
			if (isOpen) {
				OpenTool ();
			} else {
				CloseTool ();
			}
		}

		if (!isOpen) {
			return;
		}

		if (Input.GetButtonDown ("YButton")) {
			scaleVert = !scaleVert;
			vertScaleGUI.SetActive (scaleVert);
		}
		if (Input.GetButtonDown ("BButton")) {
			scaleHoriz = !scaleHoriz;
			horizScaleGUI.SetActive (scaleHoriz);
		}
		// paste last scale
		if (Input.GetButtonDown ("XButton")) {
			scaleObj.texScale = lastScale;
		}

		Vector3 scaleMod = Vector3.one;
		if (Input.GetButton ("IncBrushSize")) {
			scaleMod *= scaleChangeRate;
		} else if (Input.GetButton ("DecBrushSize")) {
			scaleMod *= -scaleChangeRate;
		} else {
			scaleMod *= 0;
		}

		if (!scaleHoriz) {
			scaleMod.x = 0;
		}
		if (!scaleVert) {
			scaleMod.y = 0;
		}

		Vector3 scale = scaleObj.texScale;
		scale += scaleMod;
		xScaleText.text = scale.x.ToString();
		yScaleText.text = scale.y.ToString();
		scaleObj.texScale = scale;

		// move ref obj around
		if (Input.GetAxis ("Paint") > 0) {
			referenceObj.transform.position += playerTransform.forward * .05f;
			referenceObj.transform.forward = playerTransform.forward;
		} else if (Input.GetAxis ("SelectColor") > 0) {
			referenceObj.transform.position += -playerTransform.forward * .05f;
			referenceObj.transform.forward = playerTransform.forward;
		}
	}

	public void SubmitXScale() {
		float newScale;
		if (float.TryParse (xScaleInput.text, out newScale)) {
			xScaleText.text = newScale.ToString ();
			scaleObj.texScale.x = newScale;
		}

		xScaleInput.text = "";
	}

	public void SubmitYScale() {
		float newScale;
		if (float.TryParse (yScaleInput.text, out newScale)) {
			yScaleText.text = newScale.ToString ();
			scaleObj.texScale.y = newScale;
		}
		yScaleInput.text = "";
	}

	private void OpenTool() {
		// cant open if were not on a paintable obj
		if (!TexturePainter.singleton.IsOnPaintableObject ()) {
			Debug.LogError ("Cannot open tex scale tool if you're not hovering on a paintable object");
			isOpen = false;
			return;
		}

		lockMouse.LockCursor (false);

		scaleObj = TexturePainter.singleton.GetPaintableObject ();

		toolGUI.SetActive (true);
		xScaleText.text = scaleObj.texScale.x.ToString();
		yScaleText.text = scaleObj.texScale.y.ToString();

		
		// move reference object in front of and to the right of the player
		referenceObj.SetActive(true);
		referenceObj.transform.forward = playerTransform.forward;
		referenceObj.transform.position = scaleObj.transform.position - playerTransform.forward * .5f + playerTransform.right * 2.0f;

		Debug.LogWarning ("Opened tex scale tool on " + scaleObj.gameObject + ". Starting tex scale: " + scaleObj.texScale.ToString("F4"));
	}

	private void CloseTool() {
		toolGUI.SetActive (false);
		referenceObj.SetActive (false);
		lockMouse.LockCursor (true);

		lastScale = scaleObj.texScale;

		scaleObj.SaveTexScale ();

		Debug.LogWarning ("Closing tex scale tool on " + scaleObj.gameObject + ". final tex scale: " + scaleObj.texScale.ToString("F4"));
	}

	void OnDestroy() {
		if (isOpen) {
			CloseTool ();
		}
	}
}
