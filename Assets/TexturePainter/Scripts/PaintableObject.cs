using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class PaintableObject : MonoBehaviour {

	public int canvasWidth = 256;
	public int canvasHeight = 256;
	public int depth = 24;

	private RenderTexture m_canvas;
	public RenderTexture m_baseTex;
	private Transform m_brushContainer;

	public Vector3 texScale = Vector3.one;

	// Use this for initialization
	void Start () {
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

		texScale = new Vector3 (1F / transform.localScale.x, 1F / transform.localScale.y, 1F / transform.localScale.z);
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
}
