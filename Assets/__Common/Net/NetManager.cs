using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using BestHTTP;
using BestHTTP.SocketIO;


public class NetManager : Singleton<NetManager> {

	private SocketManager _manager;
	private Socket _socket;

	public static SocketManager Manager { get { return Instance._manager; } }
	public static Socket Socket { get { return Instance._socket; } }
	public static Action WhenConnected;

	// Use this for initialization
	void Start () {
		_manager = new SocketManager(new Uri("http://localhost:9999/socket.io/"));
		_socket = _manager.Socket;
		_socket.On("connect", OnConnected);
		_socket.On("echo", OnEcho);

		trace("Socket started.");
	}

	void OnApplicationQuit() {
		if(_socket==null) return;
		trace("_socket.Disconnect...");
		_socket.Disconnect();
	}

	void OnConnected(Socket socket, Packet packet, params object[] args) {
		trace("Socket Connected!");

		if(WhenConnected != null) {
			WhenConnected();
		}

		_socket.Emit("echo", "Testing");
	}

	void OnEcho(Socket socket, Packet packet, params object[] args) {
		trace("Echo received! " + args[0]);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
