using System;
using UnityEngine;
using System.Collections.Generic;

public class DotPool {

	private Transform m_poolParent;
	private GameObject m_prefab;
	private Queue<GameObject> m_unusedObjs;

	public DotPool() {
		m_poolParent = GameObject.Find("DotPool").transform;
		m_prefab = Resources.Load ("TexturePainter-Instances/BrushEntity") as GameObject;
		m_unusedObjs = new Queue<GameObject> ();
	}

	public DotPool (GameObject prefab) {
		m_poolParent = GameObject.Find("DotPool").transform;
		m_prefab = prefab;
		m_unusedObjs = new Queue<GameObject> ();
	}

	public GameObject GetDot() {
		if (m_unusedObjs.Count > 0) {
			GameObject ob = m_unusedObjs.Dequeue ();
			ob.SetActive (true);
			ob.transform.SetParent (null);
			ob.transform.localScale = m_prefab.transform.localScale;
			ob.transform.localRotation = m_prefab.transform.localRotation;
			ob.transform.localPosition = m_prefab.transform.localPosition;
			return ob;
		}

		return GameObject.Instantiate (m_prefab) as GameObject;
	}

	public void FreeDot(GameObject dot) {
		dot.SetActive (false);
		dot.transform.SetParent (m_poolParent);
		m_unusedObjs.Enqueue (dot);
	}
}

