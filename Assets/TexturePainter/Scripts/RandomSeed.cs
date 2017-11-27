using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSeed : MonoBehaviour {

	public Renderer rend;

	// Use this for initialization
	void Start () {
		if (rend == null) {
			rend = GetComponent<Renderer> ();
		}

		rend.material.SetFloat ("_Seed", Random.value * 50.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
