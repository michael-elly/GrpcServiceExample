using System;

namespace GrpcServiceExample.ReverseService {
	class Program {
		static void Main(string[] args) {
			ReverseServer server = new ReverseServer();
			server.Start();
			Console.WriteLine("Server is listening... press any key to stop...");
			Console.ReadKey();
		}
	}
}
