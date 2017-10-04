using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[RequireComponent(typeof(MeshCollider))]
public class PaintableObject : MonoBehaviour {

	public int canvasWidth = 512;
	public int canvasHeight = 512;
	public int depth = 24;

	private RenderTexture m_canvas;
	public RenderTexture m_baseTex;
	private Transform m_brushContainer;

	[HideInInspector]
	public Vector3 texScale;

	private int guid;

	// Use this for initialization
	void Start () {
		texScale = TexScaleWriter.singleton.ReadValue (GetGUID());

		gameObject.layer = LayerMask.NameToLayer (TexturePainter.c_paintableLayer);

		//m_canvas = new RenderTexture (canvasWidth, canvasHeight, 24);
		m_canvas = new RenderTexture (canvasWidth, canvasHeight, 24, RenderTextureFormat.ARGB32);
		m_brushContainer = GameObject.Instantiate (TexturePainter.singleton.brushContainerPrefab, 
			TexturePainter.singleton.brushContainerParent, false).transform;
		m_brushContainer.gameObject.SetActive (false);

		m_baseTex = new RenderTexture (canvasWidth, canvasHeight, 24, RenderTextureFormat.ARGB32);

		Renderer rend = GetComponent<Renderer> ();
		Texture matTex = rend.material.mainTexture;

		// if there is no base tex, create a white tex to start with
		if (matTex == null) {
			Texture2D whiteTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			whiteTex.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 1.0f));
			whiteTex.Apply();

			matTex = whiteTex;
		}

		// blit mat texture into m_baseTex
		Graphics.Blit (matTex, m_baseTex);

		TexturePainter.singleton.RenderCanvas (this, false);

		rend.material.mainTexture = m_canvas;
	}
	
	// Update is called once per frame
	void Update () {
		
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
