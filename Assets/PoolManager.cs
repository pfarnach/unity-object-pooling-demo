using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

	Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>> ();

	// Singleton pattern
	static PoolManager _instance;

	// Singleton getter -- will be used like: PoolManager.instance.CreatePool (prefab, 3);
	public static PoolManager instance {
		get {
			if (_instance == null) {
				// Instantiate itself if it doesn't already exist in the scene
				_instance = FindObjectOfType<PoolManager> ();
			}
				
			return _instance;
		}
	}

	public void CreatePool(GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID ();

		// For organizing game objects in hierarchy
		GameObject poolHolder = new GameObject (prefab.name + " Pool");
		poolHolder.transform.parent = transform;

		// Make pool for poolKey (prefab) if it doesn't exist already
		if (!poolDictionary.ContainsKey (poolKey)) {
			poolDictionary.Add (poolKey, new Queue<ObjectInstance>());

			for (var i = 0; i < poolSize; i++) {
				ObjectInstance newObject = new ObjectInstance(Instantiate (prefab) as GameObject);
				newObject.SetParent (poolHolder.transform);  // to organize all the instances in a given pool
				poolDictionary [poolKey].Enqueue (newObject);
			}
		}
	}

	public void ReuseObject (GameObject prefab, Vector3 pos, Quaternion rot) {
		int poolKey = prefab.GetInstanceID ();

		// If it's in dictionary's, de-queue it, reset its position, rotation, and set to active
		if (poolDictionary.ContainsKey (poolKey)) {
			// Get first object out
			ObjectInstance objectToReuse = poolDictionary [poolKey].Dequeue ();

			// Put it back in the queue to be reused later
			poolDictionary [poolKey].Enqueue (objectToReuse);

			// Have class method handle the resetting
			objectToReuse.Reuse (pos, rot);
		}
	}

	// We do this for the purpose of resetting the instance back to its original self
	// The below Reuse method handles basic pos/rot resetting, but if the gameObject inherited from PoolObject, it will also have a OnObjectReuse method for more specific resetting
	// Note that the OnObjectReuse method can be overriden (like it is in TestObject.cs)
	private class ObjectInstance {

		GameObject gameObject;
		Transform transform;

		bool hasPoolObjectComponent;
		PoolObject poolObjectScript;

		// Constructor
		public ObjectInstance(GameObject objectInstance) {
			gameObject = objectInstance;
			transform = gameObject.transform;
			gameObject.SetActive (false);

			// Checking to see if gameObject we're going to reset has a PoolObject script (and by extension a OnObjectReuse method)
			if (gameObject.GetComponent<PoolObject>()) {
				hasPoolObjectComponent = true;
				poolObjectScript = gameObject.GetComponent<PoolObject>();
			}
		}

		public void Reuse (Vector3 pos, Quaternion rot) {
			// Reset pos, rot and isActive
			gameObject.SetActive (true);
			gameObject.transform.position = pos;
			gameObject.transform.rotation = rot;

			if (hasPoolObjectComponent) {
				poolObjectScript.OnObjectReuse ();
			}
		}

		public void SetParent(Transform parent) {
			transform.parent = parent;
		}
	}
}
