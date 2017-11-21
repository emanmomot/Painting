using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkScript : MonoBehaviour {

	public CharacterController player;

	public float amplitude;
	public float speed;

	private int ind;
	private int numLinks;
	private Vector3 offset;

	private float amt;

	public void InitLink(int ind, int numLinks) {
		this.ind = ind;
		this.numLinks = numLinks;
		
		// no even numlinks
		if (numLinks % 2 == 0) {
			numLinks++;
		}

		float center = numLinks / 2.0f;
		//amt = 
	}

	// Use this for initialization
	void Start () {
		offset = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
		//float t = (ind / (float)numLinks + speed * Time.time) * 2 * Mathf.PI;
		//transform.localPosition = offset + new Vector3 (0, amplitude * Mathf.Sin (t), 0);
	}
}
