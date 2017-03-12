using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsUtils {
	
	public static JSONBounds GetBoundsOf(GameObject go) {
		MeshFilter[] meshes = go.GetComponentsInChildren<MeshFilter>();

		Bounds b = new Bounds();
		bool first = true;
		foreach (MeshFilter meshFilter in meshes) {
			Mesh mesh = meshFilter.mesh;
			Bounds bounds = mesh.bounds;

			if (first) {
				b = bounds;
				first = false;
			} else {
				b.Encapsulate(bounds);
			}
		}

		return new JSONBounds(b.min, b.max);
	}
}

[Serializable]
public struct JSONBounds {
	public Vector3 min;
	public Vector3 max;

	public JSONBounds(Vector3 min, Vector3 max) {
		this.min = min;
		this.max = max;
	}
}
