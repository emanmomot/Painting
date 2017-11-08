using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class makeHamBod : ProcBase {

	public bool generateLinks;
	private bool linksgenned;

	public GameObject linkPrefab;
	public GameObject handModel;
	public GameObject head;
	public GameObject tail;
	public int numLinks;
	public int numSegmentsPerLink;
	public int numRadialSegments;
	public float radius;

	public List<Transform> links;

	private List<Vector3> ps;
	private List<Vector3> ts;
	private List<Vector3> ms;

	private Mesh mesh;

    private bool enableBod;

	// Use this for initialization
	public override void Init () {
		ps = new List<Vector3> ();
		ts = new List<Vector3> ();
		ms = new List<Vector3> ();

		if (generateLinks) {
			GenLinks ();
            enableBod = true;
		} else
        {
            enableBod = true;
        }
	}

	void GenLinks() {
		// build links
		links = new List<Transform>();
		links.Add (head.transform);

		Vector3 dif = tail.transform.position - head.transform.position;
		float length = dif.magnitude;
		float linkSize = length / (numLinks+1);
		Vector3 dir = dif.normalized;
		Vector3 pos = head.transform.position;
		GameObject prevLink = head;

		for (int i = 0; i < numLinks; i++) {
			pos += dir * linkSize;
			GameObject newLink = (GameObject)GameObject.Instantiate (linkPrefab, pos, Quaternion.identity);
			//SpringJoint joint = newLink.GetComponent<SpringJoint> ();
			//joint.connectedBody = prevLink.GetComponent<Rigidbody> ();
			prevLink = newLink;

			//newLink.transform.SetParent (head.transform);

			Rigidbody body = newLink.GetComponent<Rigidbody> ();
			body.constraints = RigidbodyConstraints.FreezeAll;

			newLink.GetComponent<LinkScript> ().InitLink (i, numLinks);
	

			links.Add (newLink.transform);
		}

		tail.GetComponent<SpringJoint> ().connectedBody = prevLink.GetComponent<Rigidbody> ();
		links.Add (tail.transform);
	}

	// Update is called once per frame
	void LateUpdate () {
		if (!generateLinks && linksgenned || !enableBod) {
			return;
		}
        //if (linksgenned)
        //    return;
		linksgenned = true;

		ms.Clear ();
		ps.Clear ();
		ts.Clear ();

		// construct splines
		for (int i = 0; i < links.Count; i++) {
			ms.Add (finiteDiffSlope (i));
		}

		ms [0] = -head.transform.forward;
			
		Vector3 xPrev, pPrev, d, x, p;
		for (int i = 0; i < links.Count-1; i++) {

			d = links [i+1].position - links [i].position;
			float l = d.magnitude / numSegmentsPerLink;
			d.Normalize ();

			xPrev = links [i].position;
			pPrev = links [i].position;

			for (int j = 0; j < numSegmentsPerLink; j++) {
				x = xPrev + d * l;
				p = evalSpline3D (x, ms, i);

				//Debug.DrawLine (p, pPrev, Color.red, 0, false);

				ps.Add (p);
				ts.Add ((p - pPrev).normalized);

				//Debug.DrawLine (p, p + .2f * ts [ts.Count - 1], Color.blue, 0, false);

				xPrev = x;
				pPrev = p;
			}
		}

		if (mesh == null) {
			mesh = BuildMesh ();
			SetMesh (mesh);
		} else {
			UpdateMesh ();
		}
	}

    public void EnableBod()
    {
        enableBod = true;
    }

    public void DisableBod()
    {
        enableBod = false;
    }

	//Build the mesh:
	public override Mesh BuildMesh() {
		MeshBuilder meshBuilder = new MeshBuilder ();

		Vector3 axis;
		Quaternion rot;
		for (int i = 0; i < ps.Count; i++) {
			axis = Vector3.Cross (Vector3.up, ts[i]);
			float angle = Vector3.Angle (Vector3.up, ts [i]);
			rot = Quaternion.AngleAxis (angle, axis);
			BuildRing (meshBuilder, numRadialSegments, ps [i] - transform.position, radius, i / ((float)ps.Count), i > 0, rot);
		}

		return meshBuilder.CreateMesh ();
	}

	public void UpdateMesh() {
		BeginMeshEdit (mesh);

		Vector3 axis;
		Quaternion rot;
		for (int i = 0; i < ps.Count; i++) {
			axis = Vector3.Cross (Vector3.up, ts[i]);
			float angle = Vector3.Angle (Vector3.up, ts [i]);
			rot = Quaternion.AngleAxis (angle, axis);
			UpdateRing (mesh, numRadialSegments, ps [i] - transform.position, radius, i, rot);
		}

		EndMeshEdit (mesh);
		mesh.RecalculateBounds ();
	}

	public Vector3 evalSpline3D(Vector3 pos, List<Vector3> ms, int k) {
		return new Vector3 (evalSpline1D (pos [0], ms, k, 0), 
							evalSpline1D (pos [1], ms, k, 1), 
							evalSpline1D (pos [2], ms, k, 2));
	}

	/// <param name="coord">0 for x, 1 for y, 2 for z</param>
	public float evalSpline1D(float x, List<Vector3> ms, int k, int coord) {
		float xk = links [k].position [coord];
		float xk1 = links [k + 1].position [coord];
		float interval = xk1 - xk;

		if (interval == 0) {
			return xk;
		}

		float t = (x - xk) / interval;

		float h00 = (1 + 2 * t) * (1 - t) * (1 - t);
		float h10 = t * (1 - t) * (1 - t);
		float h01 = t * t * (3 - 2 * t);
		float h11 = t * t * (t - 1);

		float px = h00 * links [k].position [coord] + h10  * ms [k] [coord]
		           + h01 * links [k + 1].position [coord] + h11  * ms [k + 1] [coord];
		return px;
	}

	// is tk+1 - t necessarily 1??
	public Vector3 finiteDiffSlope(int index) {
		Vector3 m;
		if (index == 0) {
			m = links [1].position - links [0].position;
		} else if (index == links.Count - 1) {
			m = links [links.Count - 1].position - links [links.Count - 2].position;
		} else {
			m = .5f * ((links [index + 1].position - links [index].position)
				+ (links [index].position - links [index - 1].position));
		}
		return m;
	}
}
