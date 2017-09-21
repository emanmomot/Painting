using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableObject : MonoBehaviour {

	public int canvasWidth = 256;
	public int canvasHeight = 256;
	public int depth = 24;

	private RenderTexture m_canvas;
	private Texture2D m_baseTex;
	private Transform m_brushContainer;

	// Use this for initialization
	void Start () {
		gameObject.layer = LayerMask.NameToLayer (TexturePainter.c_paintableLayer);

		m_canvas = new RenderTexture (canvasWidth, canvasHeight, 24);
		m_brushContainer = GameObject.Instantiate (TexturePainter.singleton.brushContainerPrefab, 
			TexturePainter.singleton.brushContainerParent, false).transform;
		m_brushContainer.gameObject.SetActive (false);

		Renderer rend = GetComponent<Renderer> ();
		m_baseTex = rend.material.mainTexture as Texture2D;

		TexturePainter.singleton.RenderCanvas (this, false);

		rend.material.mainTexture = m_canvas;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public RenderTexture GetCanvas() {
		return m_canvas;
	}

	public Texture2D GetBaseTex() {
		return m_baseTex;
	}

	public void SetBaseTex(Texture2D baseTex) {
		m_baseTex = baseTex;
	}

	public Transform GetBrushContainer() {
		return m_brushContainer;
	}
}
