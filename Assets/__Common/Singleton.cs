using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : CoreScript where T : MonoBehaviour {
	public static T Instance;

	// Use this for initialization
	void Awake () {
		if(Instance!=null) {
			throw new System.Exception("Already instantiated " + this);
		}

		Instance = this as T;
	}
}
