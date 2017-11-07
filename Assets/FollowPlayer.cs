using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	Vector3 d;
	public Transform player;

	// Use this for initialization
	void Start () {
		d = transform.position - player.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.rotation = Quaternion.identity;
		//transform.position = player.position - player.rotation * d;
	}
}
