using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

//using System.Windows.Forms;

static class Program {
	static MyServer server;
	static Thread mainThread;
	static ManualResetEvent _quitEvent = new ManualResetEvent(false);

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main() {
		//Application.EnableVisualStyles();
		//Application.SetCompatibleTextRenderingDefault(false);
		//Application.Run();
		Console.CancelKeyPress += (sender, eArgs) => {
			_quitEvent.Set();
			eArgs.Cancel = true;
		};

		__CreateServer();

		_quitEvent.WaitOne();
		//mainThread = new Thread(new ThreadStart(__CreateServer));
	}

	private static void __CreateServer() {
		server = new MyServer();

		//while(server.isRunning) { }
	}

	public static void trace(object a, params object[] args) {
		Console.WriteLine(a.ToString(), args);
	}
}
