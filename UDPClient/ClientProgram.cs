using System;
using System.Threading;

namespace MyUDP {
	using Packet;

	class ClientProgram {

		static Random randy = new Random();
		static ManualResetEvent _quitEvent = new ManualResetEvent(false);
		static MyUDPClient client;
		static string lastCommand;

		[STAThread] /// The main entry point for the application.
		static void Main() {
			Console.CancelKeyPress += (sender, eArgs) => {
				_quitEvent.Set();
				eArgs.Cancel = true;
				client.Close();
			};

			Log.trace("Which IP? (default: {0}):", MyDefaults.CLIENT_IP);
			string ip = Console.ReadLine();

			Log.trace("Which Port? (default: {0}):", MyDefaults.CLIENT_PORT);
			string portStr = Console.ReadLine();

			int port = -1;
			if(!Utils.stringIsOK(ip)) ip = MyDefaults.CLIENT_IP;
			if(!Utils.stringIsOK(portStr) || !int.TryParse(portStr, out port)) port = MyDefaults.CLIENT_PORT;

			Log.trace("STARTING CONNECTION TO: " + ip + ":"+ port);
			client = new MyUDPClient(ip, port);
			client.SetPacketType<UnityPacket>();

			client.OnPacketReceived += OnPacketReceived;
			client.OnPacketPreSend += OnPacketPreSend;

			Test();

			_quitEvent.WaitOne();
		}

		private static void OnPacketPreSend(MyUDPPacket obj) {
			Log.trace("---------------------");
		}

		private static void OnPacketReceived(MyUDPPacket packet) {
			UnityPacket unityPacket = (UnityPacket)packet;
			string preview = "{0} Commands: {1}";

			foreach(Command cmd in unityPacket.commands) {
				preview += "\n  ACK: " + cmd.ackID;
				preview += "\n  TIME: " + cmd.timeOffset;
				preview += "\n  TYPES: " + Convert.ToString((int)cmd.types, 2);

				XYZData xyzData = cmd.xyzData;

				Utils.ForEachFlags(cmd.types, (type) => {
					switch(type) {
						case EPacketTypes.ACK: preview += "\n    SERVER-ACK: " + xyzData.ackFromServer; break;
						case EPacketTypes.ACTION: preview += "\n    ACTION: " + xyzData.action; break;
						case EPacketTypes.JSON: preview += "\n    JSON: " + xyzData.jsonData; break;
						case EPacketTypes.POSITION: preview += "\n    POSITION: " + xyzData.rotation.Join(", "); break;
						case EPacketTypes.ROTATION: preview += "\n    ROTATION: " + xyzData.rotation.Join(", "); break;
					}
				});
			}

			Log.trace(preview, unityPacket.clientTimeFormatted, unityPacket.numOfCommands);
		}

		public static bool IsBadCommand(string[] strSplit, int minLength) {
			if (strSplit.Length < minLength) {
				Log.traceError("Missing arguments for '{0}'!", strSplit[0]);
				Test();
				return true;
			}

			return false;
		}

		public static void Test() {
			//Test
			Utils.GetTime();
			Utils.setTimeout((object state) => {
				if (lastCommand != "repeat") {
					lastCommand = Console.ReadLine();

					if (!Utils.stringIsOK(lastCommand)) {
						lastCommand = "Hello World!";
					}
				}

				UnityPacket pack = (UnityPacket) client.packet;
				pack.clientTime = Utils.GetTime();
				
				Command cmd = new Command();
				cmd.ackID = randy.Next(0, 10);
				cmd.timeOffset = randy.Next(10, 30);
				XYZData xyzData = cmd.xyzData;

				//If the command starts with a forward-slash, check if it's one of the known special commands!
				if (lastCommand.StartsWith("/")) {
					string lastCommandRest = lastCommand.Substring(1);
					string[] lastCommandSplit = lastCommandRest.Split(",");

					foreach(string cmdStr in lastCommandSplit) {
						string[] cmdSplit = cmdStr.Trim().Split(' ');

						try {
							switch (cmdSplit[0]) {
								case "pos":
									if (IsBadCommand(cmdSplit, 4)) return;
									
									xyzData.position[0] = double.Parse(cmdSplit[1]);
									xyzData.position[1] = double.Parse(cmdSplit[2]);
									xyzData.position[2] = double.Parse(cmdSplit[3]);
									cmd.types |= EPacketTypes.POSITION;

									break;

								case "rot":
									if (IsBadCommand(cmdSplit, 5)) return;
									
									xyzData.rotation[0] = double.Parse(cmdSplit[1]);
									xyzData.rotation[1] = double.Parse(cmdSplit[2]);
									xyzData.rotation[2] = double.Parse(cmdSplit[3]);
									xyzData.rotation[3] = double.Parse(cmdSplit[4]);
									cmd.types |= EPacketTypes.ROTATION;

									break;

								case "action":
									if (IsBadCommand(cmdSplit, 2)) return;

									xyzData.action = int.Parse(cmdSplit[1]);
									cmd.types |= EPacketTypes.ACTION;

									break;

								case "ack":
									if (IsBadCommand(cmdSplit, 2)) return;

									xyzData.ackFromServer = int.Parse(cmdSplit[1]);
									cmd.types |= EPacketTypes.ACK;

									break;

								default:
									xyzData.jsonData = cmdStr;
									cmd.types |= EPacketTypes.JSON;

									break;
							}
						} catch(Exception ex) {
							Log.traceError("ClientProgram error: " + ex.Message);
						}
					}

					
				} else {
					cmd.types |= EPacketTypes.JSON;
					xyzData.jsonData = lastCommand;
				}

				if(cmd.types>0) {
					pack.commands.Clear();
					pack.commands.Add(cmd);

					client.Send();
				}
				Test();
			}, 2000);
		}
	}

	
}
