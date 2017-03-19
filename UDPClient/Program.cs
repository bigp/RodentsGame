using System;
using System.Threading;

namespace MyUDP {
	using Packet;

	class ProgramClient {

		static Random randy = new Random();
		static ManualResetEvent _quitEvent = new ManualResetEvent(false);
		static MyUDPClient client;

		static int attempts = 5;

		[STAThread] /// The main entry point for the application.
		static void Main() {
			Log.trace("Started Client!");

			Console.CancelKeyPress += (sender, eArgs) => {
				_quitEvent.Set();
				eArgs.Cancel = true;
			};

			client = new MyUDPClient("192.168.2.127");

			Test();

			_quitEvent.WaitOne();
		}

		public static void Test() {
			if(attempts<=0) return;

			Utils.setTimeout((object state) => {
				Command cmd = new Command();
				cmd.ackID = randy.Next(0, 10);
				cmd.timeOffset = randy.Next(10, 30);
				cmd.types = EPacketTypes.JSON;
				cmd.jsonData = "{'x': 1}";

				client.packet.clientTime = Utils.GetTime();
				client.packet.numOfCommands = 1;
				client.packet.commands.Clear();
				client.packet.commands.Add(cmd);
				client.Send();

				Test();

				attempts--;
			}, 2000);
		}
	}

	
}
