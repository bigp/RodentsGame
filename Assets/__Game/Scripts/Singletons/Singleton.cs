using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : CoreScript where T : MonoBehaviour {
	public static T Instance;

	// Use this for initialization
	void Awake () {
		Instance = this as T;
		trace("Instance: " + Instance);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
