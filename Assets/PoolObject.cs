using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

	// virtual means that derived classes of PoolObject can override this method
	public virtual void OnObjectReuse () {
		
	}

	protected void Destroy () {
		gameObject.SetActive (false);
	}
}
