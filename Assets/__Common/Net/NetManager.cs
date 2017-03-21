using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using ExtensionMethods;

using MyUDP;
using MyUDP.Packet;


//using BestHTTP;
//using BestHTTP.SocketIO;


public class NetManager : Singleton<NetManager> {

	//private SocketManager _manager;
	//private Socket _socket;

	//public static SocketManager Manager { get { return Instance._manager; } }
	//public static Socket Socket { get { return Instance._socket; } }
	public static Action WhenConnected;
	public static MyUDPServer server;
	public static MyUDPClient client;

	public float counterInterval = 1.0f;
	private float _counter;

	// Use this for initialization
	void Start () {
		//_manager = new SocketManager(new Uri("http://localhost:9999/socket.io/"));
		//_socket = _manager.Socket;
		//_socket.On("connect", OnConnected);
		//_socket.On("echo", OnEcho);

		trace("Socket started.");

		client = new MyUDPClient();

		_counter = counterInterval;
	}

	void Update() {
		if(PlayerManager.Instance==null) return;
		_counter -= Time.deltaTime;
		if(_counter>0) return;
		_counter += counterInterval;

		Transform trans = PlayerManager.Instance.transform;

		var cmd = new Command();
		cmd.xyzData = new XYZData();
		SetPosition(ref cmd, ref cmd.xyzData, trans);
		SetRotation(ref cmd, ref cmd.xyzData, trans);
		SetAction(ref cmd, ref cmd.xyzData, RandomNum());
		SetJSON(ref cmd, "Hello! " + Time.frameCount);

		if (client.packet.commands.Count==0) {
			client.packet.commands.Add(cmd);
		} else {
			client.packet.commands[0] = cmd;
		}

		client.Send();
	}

	private static int RandomNum() {
		return UnityEngine.Random.Range((int)1, 10);
	}

	public static void SetPosition(ref Command cmd, ref XYZData xyzData, Transform trans) {
		cmd.types |= EPacketTypes.POSITION;
		cmd.xyzData.position = new double[3];
		cmd.xyzData.position[0] = trans.localPosition.x;
		cmd.xyzData.position[1] = trans.localPosition.y;
		cmd.xyzData.position[2] = trans.localPosition.z;
	}

	public static void SetRotation(ref Command cmd, ref XYZData xyzData, Transform trans) {
		cmd.types |= EPacketTypes.ROTATION;

		xyzData.rotation = new double[4];
		xyzData.rotation[0] = trans.localRotation.x;
		xyzData.rotation[1] = trans.localRotation.y;
		xyzData.rotation[2] = trans.localRotation.z;
		xyzData.rotation[3] = trans.localRotation.w;
	}

	public static void SetAction(ref Command cmd, ref XYZData xyzData, int actionID) {
		cmd.types |= EPacketTypes.ACTION;
		xyzData.action = actionID;
	}

	public static void SetJSON(ref Command cmd, string data) {
		cmd.types |= EPacketTypes.JSON;
		cmd.jsonData = data;
	}

	void OnApplicationQuit() {
		//if(_socket==null) return;
		//trace("_socket.Disconnect...");
		//_socket.Disconnect();
		client.Close();
	}

	//void OnConnected(Socket socket, Packet packet, params object[] args) {
	//	trace("Socket Connected!");

	//	if(WhenConnected != null) {
	//		WhenConnected();
	//	}

	//	//_socket.Emit("echo", "Testing");
	//}

	//void OnEcho(Socket socket, Packet packet, params object[] args) {
	//	trace("Echo received! " + args[0]);
	//}
}
