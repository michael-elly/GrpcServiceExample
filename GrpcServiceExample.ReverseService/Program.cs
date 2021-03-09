using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcServiceExample.ReverseService {
	class Program {
		static void Main(string[] args) {
			ReverseServer server = new ReverseServer();
			server.Start();
			Console.WriteLine("Server is listening... Press q to stop...");			
			while (Console.ReadKey().Key != ConsoleKey.Q) {
				Console.WriteLine("\r\nPress q to stop...");
			}
			Console.WriteLine("\r\nExiting...");
			Task t = server.ShutDownAsync();			
			t.Wait();
			Console.WriteLine("Exited");
		}
	}
}
