using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[RequireComponent(typeof(MeshCollider))]
public class PaintableObject : MonoBehaviour {

	const string c_texSavePath = "/save_data/textures/";
	const string c_paintTexUniform = "_PaintTex";
	const string c_cursorTexUniform = "_CursorTex";
	const float c_timeToDissapear = 15;

	public int canvasWidth = 512;
	public int canvasHeight = 512;
	public int depth = 24;

	public int matIndex = 0;

	public RenderTexture m_canvas;
	public RenderTexture m_baseTex;
	public RenderTexture m_cursorTex;
	private Transform m_brushContainer;

	[HideInInspector]
	public Vector3 texScale;

	private Material mat;
	public int guid;

	private bool hasInitializedCanvas;

	private float dissapearTimer;

	// Use this for initialization
	void Start () {
		texScale = TexScaleWriter.singleton.ReadValue (GetGUID());

		gameObject.layer = LayerMask.NameToLayer (TexturePainter.c_paintableLayer);

		m_canvas = new RenderTexture (canvasWidth, canvasHeight, depth, RenderTextureFormat.ARGB32);
		m_brushContainer = GameObject.Instantiate (TexturePainter.singleton.brushContainerPrefab, 
			TexturePainter.singleton.brushContainerParent, false).transform;
		m_brushContainer.gameObject.SetActive (false);

		m_baseTex = new RenderTexture (canvasWidth, canvasHeight, depth, RenderTextureFormat.ARGB32);
		m_cursorTex = new RenderTexture (canvasWidth, canvasHeight, depth, RenderTextureFormat.ARGB32);

		Renderer rend = GetComponent<Renderer> ();
		mat = rend.materials [matIndex];

		string paintTexProp = c_paintTexUniform;
		if (!mat.HasProperty (paintTexProp)) {
			paintTexProp = "_MainTex";
		}

		// get the default texture of the mat
		Texture matTex = mat.GetTexture (paintTexProp);
		// if none, check if we have default color
		if (matTex == null && mat.HasProperty ("_Color")) { 
			matTex = CreateSmallTexture (mat.GetColor ("_Color"));
		}

		// if we got a tex, blit to the base
		if (matTex != null) {
			Graphics.Blit (matTex, m_baseTex);
		}

		// read saved painting
		if (GUIManager.singleton.saveAndLoadTextures) {
			Texture2D texSave = ReadTexFromFile ();
			if (texSave != null) {
				Graphics.Blit (texSave, m_baseTex);
			}
		}

		mat.SetTexture (paintTexProp, m_canvas);
		mat.SetTexture (c_cursorTexUniform, m_cursorTex);
	}
	
	// Update is called once per frame
	void Update () {
		// we have to render the basetex to the canvas outside of start
		if (!hasInitializedCanvas) {
			TexturePainter.singleton.RenderCanvas (this, false);
			hasInitializedCanvas = true;
		}

		if (dissapearTimer > 0) {
			dissapearTimer -= Time.deltaTime;
			if (mat.HasProperty ("_PaintAlpha")) {
				mat.SetFloat ("_PaintAlpha", dissapearTimer / c_timeToDissapear);
			} else {
				Color c = mat.GetColor ("_Color");
				c.a = dissapearTimer / c_timeToDissapear;
				mat.SetColor ("_Color", c);
			}
		}
	}

	public RenderTexture GetCanvas() {
		return m_canvas;
	}

	public RenderTexture GetBaseTex() {
		return m_baseTex;
	}

	public void SetBaseTex(RenderTexture baseTex) {
		m_baseTex = baseTex;
	}

	public Transform GetBrushContainer() {
		return m_brushContainer;
	}

	public void SaveTexScale() {
		TexScaleWriter.singleton.UpdateValue (GetGUID(), texScale);
		TexScaleWriter.singleton.WriteMapToFile ();
	}

	private Texture2D CreateSmallTexture(Color color) {
		Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

		// set the pixel values
		texture.SetPixel(0, 0, color);
		texture.SetPixel(1, 0, color);
		texture.SetPixel(0, 1, color);
		texture.SetPixel(1, 1, color);

		// Apply all SetPixel calls
		texture.Apply();

		return texture;
	}

	public void ResetTransparency() {
		dissapearTimer = c_timeToDissapear;
	}

	// save tex on destroy
	void OnDestroy () {
		if (GUIManager.singleton.saveAndLoadTextures) {
			WriteTexToFile (m_baseTex);
		}
	}

	void WriteTexToFile(RenderTexture renderTexture) {
		// write contents of RT into a texture2d
		RenderTexture currentActiveRT = RenderTexture.active;
		RenderTexture.active = renderTexture;
		Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
		tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

		// write texture2d to file
		byte[] bytes = tex.EncodeToPNG();
		System.IO.File.WriteAllBytes (GetTexSavePath (), bytes);

		// clean up
		UnityEngine.Object.Destroy(tex);
		RenderTexture.active = currentActiveRT;
	}

	Texture2D ReadTexFromFile() {
		if (!System.IO.File.Exists (GetTexSavePath ())) {
			return null;
		}

		byte[] bytes;
		Texture2D tex = new Texture2D (1, 1);
		bytes = System.IO.File.ReadAllBytes (GetTexSavePath ());
		tex.LoadImage (bytes);
		return tex;
	}

	string GetTexSavePath() {
		return Application.dataPath + c_texSavePath + GetGUID() + ".png";
	}

	public void ClearTex() {
		Texture2D tex = CreateSmallTexture (new Color (0, 0, 0, 0));
		Graphics.Blit (tex, m_baseTex);
	}

	public int GetGUID() {
#if UNITY_EDITOR
		if(guid == 0) {
			PropertyInfo inspectorModeInfo =
				typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

			SerializedObject serializedObject = new SerializedObject(gameObject);
			inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

			SerializedProperty localIdProp =
				serializedObject.FindProperty("m_LocalIdentfierInFile");   //note the misspelling!

			guid = localIdProp.intValue;
		}
		return guid;
#else 
		throw new NotImplementedException("Cannot get guid when not running in editor. Serialize it before build!");
#endif
	}
}
