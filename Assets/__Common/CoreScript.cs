using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreScript : MonoBehaviour {

	[HideInInspector] public Rigidbody rb;
	[HideInInspector] public Collider col;

	// Use this for initialization
	public virtual void Start() {
		rb = this.GetComponent<Rigidbody>();
		col = this.GetComponent<Collider>();

		Init();
		AddEvents();
	}

	public virtual void Init() {}
	public virtual void AddEvents() {}

	public static void trace(object any) {
		Debug.Log(any);
	}
}

public class Map<T, K> : Dictionary<T, K> { }
public class Array<T> : List<T> {
	public int Length { get { return this.Count; } }
	public void Push(T item) { Add(item); }
	public T Pop() {
		int id = Count - 1;
		var item = id < 0 ? this[id] : default(T);
		RemoveAt(id);
		return item;
	}
}