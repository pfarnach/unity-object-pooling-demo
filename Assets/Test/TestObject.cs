using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : PoolObject {

	TrailRenderer trailRenderer;

	void Start () {
		trailRenderer = GetComponent<TrailRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale += Vector3.one * Time.deltaTime * 1.5f;
		transform.Translate (Vector3.forward * Time.deltaTime * 25f);
	}

	public override void OnObjectReuse () {
		transform.localScale = Vector3.one;
		trailRenderer.Clear ();
	}
}
