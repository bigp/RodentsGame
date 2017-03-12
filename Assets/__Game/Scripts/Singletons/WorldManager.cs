using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class WorldManager : Singleton<WorldManager> {
	public GameObject worldBuilding;
	public GameObject worldEnvironment;

	private JSONBounds worldBounds;
	//private Bounds world

	// Use this for initialization
	void Start () {
		worldBounds = BoundsUtils.GetBoundsOf(worldBuilding);
		NetManager.WhenConnected += OnConnected;
	}

	private void OnConnected() {
		trace("Connected!!!!!!!!!");
		NetManager.Socket.Emit("world-bounds", JsonUtility.ToJson(worldBounds));
	}

	// Update is called once per frame
	void Update () {
		
	}
}
