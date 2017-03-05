using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using BestHTTP;
using BestHTTP.SocketIO;


public class NetworkingManager : Singleton<NetworkingManager> {

	private SocketManager _manager;
	private Socket _socket;

	void Awake() {
		_manager = new SocketManager(new Uri("http://localhost:9999"));
		_socket = _manager.Socket;
	}

	// Use this for initialization
	void Start () {
		_socket.Emit("event", "Test");
	}

	// Update is called once per frame
	void Update () {
		
	}
}
