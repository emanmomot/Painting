/// <summary>
/// CodeArtist.mx 2015
/// This is the main class of the project, its in charge of raycasting to a model and place brush prefabs infront of the canvas camera.
/// If you are interested in saving the painted texture you can use the method at the end and should save it to a file.
/// </summary>


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Painter_BrushMode { PAINT, DECAL };

public class TexturePainter : MonoBehaviour {

	public const string c_paintableLayer = "Paintable";

	public static TexturePainter singleton;
	public static LayerMask paintableLayerMask;

	public Transform brushContainerParent;
	public GameObject brushContainerPrefab;

	public GameObject brushCursor;//, brushContainer; //The cursor that overlaps the model and our container for the brushes painted
	public Camera sceneCamera, canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
	public Sprite cursorPaint, cursorDecal; // Cursor for the differen functions 
	//public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
	public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)

	PaintableObject lastPaintableObject;

	Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)
	float brushSize = 1.0f; //The size of our brush
	Color brushColor; //The selected color
	int brushCounter = 0, MAX_BRUSH_COUNT = 10000; //To avoid having millions of brushes
	bool saving = false; //Flag to check if we are saving the texture

	public DotPool dotPool { get; private set; }

	Vector2 brushPos;
	SmoothBrush smoothBrush;

	const float BRUSH_STEP = .01f;
	const float CP_TIME = .15f;
	const float PAINT_RANGE = 30F;

	private float cpTimer;

	void Awake () {
		singleton = this;
		paintableLayerMask = 1 << LayerMask.NameToLayer (c_paintableLayer);
	}

	void Start () {
		dotPool = new DotPool ();
	}
	
	public void UpdateTP () {
		brushColor = ColorSelector.GetColor ();	//Updates our painted color with the selected color

		UpdateBrushCursor ();

		if (smoothBrush != null) {
			smoothBrush.UpdateCursorPos (brushPos, brushSize);
			cpTimer += Time.deltaTime;
			if (cpTimer > CP_TIME) {
				smoothBrush.AddCP (brushPos, brushSize);
				cpTimer = 0;
			}
			smoothBrush.UpdateStroke ();

		}

		if (lastPaintableObject != null) {
			if (smoothBrush != null && !PlayerInput.singleton.isTriggerHeld) {
				EndPaint ();
			} else {
				RenderCanvas (lastPaintableObject, true);
			}
		}
			
	}

	public bool IsOnPaintableObject() {
		return lastPaintableObject != null;
	}

	public PaintableObject GetPaintableObject() {
		return lastPaintableObject;
	}

	public bool IsPainting() {
		return smoothBrush != null;
	}
		
	public void StartPaint(Vector2 pos) {
		smoothBrush = new SmoothBrush (pos, brushSize, BRUSH_STEP, lastPaintableObject);
	}

	public void EndPaint() {
		smoothBrush.AddCP (brushPos, brushSize);
		smoothBrush.UpdateStroke ();
		smoothBrush = null;

		RenderCanvas (lastPaintableObject, false);
		SaveTexture (lastPaintableObject);
	}

	public void EyeDrop() {
		// disable cursor
		RenderCanvas (lastPaintableObject, false);

		RenderTexture canvas = lastPaintableObject.GetCanvas ();
		Texture2D eyeDropTex = new Texture2D (1, 1);

		Vector2 pixelPos = new Vector2 ();
		pixelPos.x = (brushPos.x + canvasCam.orthographicSize) * canvas.width;
		pixelPos.y = (brushPos.y + canvasCam.orthographicSize) * canvas.height;

		Rect pixelRect = new Rect (pixelPos, Vector2.one);

		RenderTexture.active = canvas;
		eyeDropTex.ReadPixels (pixelRect, 0, 0);
		RenderTexture.active = null;

		ColorSelector.SetColor (eyeDropTex.GetPixel (0, 0));
	}

	public GameObject MakeDot(PaintableObject paintableObject, Vector2 pos, float size) {
		GameObject brushObj;

		brushObj = dotPool.GetDot (); //Paint a brush
		SpriteRenderer rend = brushObj.GetComponent<SpriteRenderer>();
		rend.color = brushColor; //Set the brush color
		rend.sortingOrder = paintableObject.GetBrushContainer().childCount;
	
		//brushColor.a = brushSize * 2.0f; // Brushes have alpha to have a merging effect when painted over.
		brushObj.transform.SetParent(paintableObject.GetBrushContainer(),false);
		//brushObj.transform.parent = paintableObject.GetBrushContainer(); //Add the brush to our container to be wiped later
		brushObj.transform.localPosition = new Vector3(pos.x, pos.y, 0); //The position of the brush (in the UVMap)

		brushObj.transform.localScale = new Vector3 (brushObj.transform.localScale.x * paintableObject.texScale.x, 
														brushObj.transform.localScale.y * paintableObject.texScale.y, 
														brushObj.transform.localScale.z);
		brushObj.transform.localScale *= size;

		return brushObj;
	}

	//To update at realtime the painting cursor on the mesh
	void UpdateBrushCursor () {
		
		Vector3 uvWorldPosition = Vector3.zero;
		PaintableObject paintableObject = null;

		if (!saving && HitTestUVPosition (ref uvWorldPosition, ref paintableObject)) {

			Vector2 newBrushPos = new Vector2 (uvWorldPosition.x, uvWorldPosition.y);

			// clear cursor out of last canvas
			if (lastPaintableObject != null && lastPaintableObject != paintableObject) {
				if (smoothBrush != null) {
					EndPaint ();
					lastPaintableObject = paintableObject;
					StartPaint (newBrushPos);
				} else {
					RenderCanvas (lastPaintableObject, false);
					lastPaintableObject = paintableObject;
				}
			} else {
				lastPaintableObject = paintableObject;
			}

			brushCursor.SetActive (true);
			brushCursor.GetComponent<SpriteRenderer> ().color = brushColor;

			brushCursor.transform.localScale = paintableObject.texScale * brushSize * .15f;
			brushCursor.transform.localPosition = uvWorldPosition;

			brushPos = newBrushPos;

			if (PlayerInput.singleton.isTriggerHeld && smoothBrush == null) {
				StartPaint (brushPos);
			}

		} else if (lastPaintableObject != null) {
			if (smoothBrush != null) {
				EndPaint ();
			} else {
				RenderCanvas (lastPaintableObject, false);
			}

			lastPaintableObject = null;
		}		
	}

	//Returns the position on the texuremap according to a hit in the mesh collider
	bool HitTestUVPosition(ref Vector3 uvWorldPosition, ref PaintableObject paintableObject) {
		
		RaycastHit hit;
		//RaycastHit borderHit;

		Vector3 cursorPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0.0f);
		//Vector3 borderOffset = new Vector3 (5, 0, 0);

		Ray cursorRay = sceneCamera.ScreenPointToRay (cursorPos);
		//Ray borderRay = sceneCamera.ScreenPointToRay (cursorPos + borderOffset);

		if (Physics.Raycast (cursorRay, out hit, PAINT_RANGE, paintableLayerMask)) {

			// cursor is overlapping multiple objects
			//if (hit.collider != borderHit.collider) {
			//	return false;
			//}
			
			paintableObject = hit.collider.GetComponent<PaintableObject> ();
			if (paintableObject == null) {
				return false;
			}

			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider == null || meshCollider.sharedMesh == null) {
				return false;			
			}
				
			Vector2 pixelUV = new Vector2 (hit.textureCoord2.x, hit.textureCoord2.y);
			//Vector2 borderUV = new Vector2 (borderHit.textureCoord.x, borderHit.textureCoord.y);

			//texScale = (borderUV - pixelUV).magnitude;
			//Debug.Log (texScale);

			uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
			uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;//To center the UV on Y
			uvWorldPosition.z = 0.0f;

			return true;
		} else {		
			return false;
		}
		
	}

	public void RenderCanvas(PaintableObject paintableObject, bool withCursor) {
		brushCursor.SetActive (withCursor);

		Transform brushContainer = paintableObject.GetBrushContainer ();
		brushContainer.gameObject.SetActive (true);

		baseMaterial.mainTexture = paintableObject.GetBaseTex ();

		canvasCam.targetTexture = paintableObject.GetCanvas ();
		canvasCam.Render ();
		brushContainer.gameObject.SetActive (false);
	}

	public void SaveTexture (PaintableObject paintableObject) {

		brushCursor.SetActive (false);

		baseMaterial.mainTexture = paintableObject.GetCanvas();

		canvasCam.targetTexture = paintableObject.GetBaseTex ();
		canvasCam.Render ();

		baseMaterial.mainTexture = paintableObject.GetBaseTex ();

		Transform brushContainer = paintableObject.GetBrushContainer ();

		while (brushContainer.childCount > 0) {
			dotPool.FreeDot (brushContainer.GetChild (0).gameObject);
		}

		ShowCursor ();

	}
	//Show again the user cursor (To avoid saving it to the texture)
	void ShowCursor () {	
		saving = false;
	}

	////////////////// PUBLIC METHODS //////////////////

	public void SetBrushMode(Painter_BrushMode brushMode) { //Sets if we are painting or placing decals
		mode = brushMode;
	}

	public void SetBrushSize(float newBrushSize){ //Sets the size of the cursor brush or decal
		brushSize = newBrushSize;
		brushCursor.transform.localScale = Vector3.one * brushSize;
	}

	////////////////// OPTIONAL METHODS //////////////////

	#if !UNITY_WEBPLAYER 
		IEnumerator SaveTextureToFile(Texture2D savedTexture){		
			brushCounter=0;
			string fullPath=System.IO.Directory.GetCurrentDirectory()+"\\UserCanvas\\";
			System.DateTime date = System.DateTime.Now;
			string fileName = "CanvasTexture.png";
			if (!System.IO.Directory.Exists(fullPath))		
				System.IO.Directory.CreateDirectory(fullPath);
			var bytes = savedTexture.EncodeToPNG();
			System.IO.File.WriteAllBytes(fullPath+fileName, bytes);
			Debug.Log ("<color=orange>Saved Successfully!</color>"+fullPath+fileName);
			yield return null;
		}
	#endif
}
