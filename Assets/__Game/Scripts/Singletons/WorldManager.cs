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
	public override void Start () {
		base.Start();

		RenderSettings.fog = true;

		worldBounds = BoundsUtils.GetBoundsOf(worldBuilding);
		NetManager.WhenConnected += OnConnected;
	}

	private void OnConnected() {
		trace("Connected! Send World Bounds to NodeJS Socket.IO...");
		NetManager.Socket.Emit("world-bounds", JsonUtility.ToJson(worldBounds));
	}

	// Update is called once per frame
	//void Update () {
		
	//}
}
