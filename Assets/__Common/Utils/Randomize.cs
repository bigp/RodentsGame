using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize {


	public static T PickRandomChild<T>(GameObject parent) {
		return PickRandomChild(parent).GetComponent<T>();
	}

	public static GameObject PickRandomChild(GameObject parent) {
		int count = parent.transform.childCount;
		int id = Random.Range(0, count);

		return parent.transform.GetChild(id).gameObject;
	}
}
