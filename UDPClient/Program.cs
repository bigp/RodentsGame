using System;
using System.Threading;

namespace MyUDP {
	using Packet;

	class ProgramClient {

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

			Test();

			_quitEvent.WaitOne();
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

				MyPacket pack = client.packet;
				pack.clientTime = Utils.GetTime();
				
				Command cmd = new Command();
				cmd.ackID = randy.Next(0, 10);
				cmd.timeOffset = randy.Next(10, 30);
				cmd.xyzData = new XYZData();

				//If the command starts with a forward-slash, check if it's one of the known special commands!
				if (lastCommand.StartsWith("/")) {
					string lastCommandRest = lastCommand.Substring(1);
					string[] lastCommandSplit = lastCommandRest.Split(",");

					foreach(string cmdStr in lastCommandSplit) {
						string[] cmdSplit = cmdStr.Trim().Split(' ');

						Log.trace("COMMAND: " + cmdStr);

						switch (cmdSplit[0]) {
							case "pos":
								if (IsBadCommand(cmdSplit, 4)) return;

								cmd.types |= EPacketTypes.POSITION;
								cmd.xyzData.position = new double[3];
								cmd.xyzData.position[0] = double.Parse(cmdSplit[1]);
								cmd.xyzData.position[1] = double.Parse(cmdSplit[2]);
								cmd.xyzData.position[2] = double.Parse(cmdSplit[3]);
								break;

							case "rot":
								if (IsBadCommand(cmdSplit, 5)) return;

								cmd.types |= EPacketTypes.ROTATION;
								cmd.xyzData.rotation = new double[4];
								cmd.xyzData.rotation[0] = double.Parse(cmdSplit[1]);
								cmd.xyzData.rotation[1] = double.Parse(cmdSplit[2]);
								cmd.xyzData.rotation[2] = double.Parse(cmdSplit[3]);
								cmd.xyzData.rotation[3] = double.Parse(cmdSplit[4]);
								break;

							case "action":
								if (IsBadCommand(cmdSplit, 2)) return;

								cmd.types |= EPacketTypes.ACTION;
								cmd.xyzData.action = int.Parse(cmdSplit[1]);
								break;

							case "ack":
								if (IsBadCommand(cmdSplit, 2)) return;

								cmd.types |= EPacketTypes.ACK;
								cmd.xyzData.ackFromServer = int.Parse(cmdSplit[1]);
								break;

							default:
								cmd.types |= EPacketTypes.JSON;
								cmd.jsonData = lastCommandRest;
								break;
						}
					}

					
				} else {
					cmd.types |= EPacketTypes.JSON;
					cmd.jsonData = lastCommand;
				}

				Log.trace("cmd.types: " + cmd.types); //

				pack.commands.Clear();
				pack.commands.Add(cmd);
				pack.numOfCommands = pack.commands.Count;

				client.Send();

				Test();
			}, 2000);
		}
	}

	
}
