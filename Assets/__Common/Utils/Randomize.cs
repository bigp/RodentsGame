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

	//////////////////////////////////////////////////////////// For Vector3:

	public static Vector3 VectorByAmount(float amount = 1) {
		return VectorByAmount(Vector3.zero, amount);
	}

	public static Vector3 VectorByAmount(Vector3 vec, float amount = 1) {
		float half = amount * 0.5f;
		vec.x = half + UnityEngine.Random.value * amount;
		vec.y = half + UnityEngine.Random.value * amount;
		vec.z = half + UnityEngine.Random.value * amount;
		return vec;
	}
}
