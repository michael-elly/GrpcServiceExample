using System;
using System.Text;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using RevService.Generated;
using System.Diagnostics;

namespace client {
	class Program {
		static void Main(string[] args) {
			//Console.WriteLine("Hello World!");
			Console.WriteLine("Enter q to quit... "); 
			Console.WriteLine("enter 'u <string>' to reverse a string using a unary gRPC call. ");
			Console.WriteLine("enter 's <string>' to reverse a string using a server streaming gRPC call. ");
			Console.WriteLine("enter 'U <string>' to reverse a many strings in parallel using a unary gRPC call. "); 
			Console.WriteLine("enter 'S <string>' to reverse a many strings in parallel string using a server streaming gRPC call. ");
			Stopwatch sw = new Stopwatch();
			
			while (true) {
				Console.Write("> ");
				string s = Console.ReadLine();
				if (s == "q") break;
				if (s.StartsWith("u ") && s.Length > 2) {
					sw.Restart();
					var reverseString = Reverse(s.Substring(2)).GetAwaiter().GetResult();
					Console.WriteLine(reverseString);
					Console.WriteLine($"  Completed in {sw.Elapsed.TotalMilliseconds:N0} ms.");
				} else if (s.StartsWith("s ") && s.Length > 2) {
					sw.Restart(); 
					SearchForContact(s.Substring(2)).GetAwaiter().GetResult();
					Console.WriteLine($"  Completed in {sw.Elapsed.TotalMilliseconds:N0} ms.");
				} else if (s.StartsWith("U ") && s.Length > 2) {
					sw.Restart(); 
					int n = 10; // parallel client threads
					Task<string>[] tasks = new Task<string>[n]; 
					for (int i=0; i<n; i++) {
						tasks[i] = Reverse(s.Substring(2) + $"{i}");
					}
					for (int i = n - 1; i >= 0; i--) {
						Console.WriteLine($"Task[{i}] Result: {tasks[i].GetAwaiter().GetResult()}");
					}
					Console.WriteLine($"  Completed in {sw.Elapsed.TotalMilliseconds:N0} ms.");
				} else if (s.StartsWith("S ") && s.Length > 2) {
					sw.Restart(); 
					int n = 10; // parallel client threads
					Task[] tasks = new Task[n];
					for (int i = 0; i < n; i++) {
						tasks[i] = SearchForContact(s.Substring(2) + $"{i}");
					}
					for (int i = 0; i < n; i++) {
						tasks[i].GetAwaiter().GetResult();
					}
					Console.WriteLine($"  Completed in {sw.Elapsed.TotalMilliseconds:N0} ms.");
				} else {
					Console.WriteLine("invalid input... ");
				}
			}
		}

		static async Task<string> Reverse(string input) {
			Channel channel = new Channel("localhost", 11111, ChannelCredentials.Insecure);
			RevService.Generated.RevService.RevServiceClient client = new RevService.Generated.RevService.RevServiceClient(channel);
			var data = new RevService.Generated.Data() { Str = input };
			var res = await client.ReverseAsync(data);
			return res.Str;
		}

		internal static async Task SearchForContact(string input) {
			// Prepare request object
			Channel channel = new Channel("localhost", 11111, ChannelCredentials.Insecure);
			RevService.Generated.RevService.RevServiceClient client = new RevService.Generated.RevService.RevServiceClient(channel);
			var data = new RevService.Generated.Data() { Str = input };

			// Prepare stream read
			using (var search = client.ReverseSt(data)) {
				Console.WriteLine("Printing stream segments as they are being sent by the server...");

				try {
					// Waiting for stream elements
					// This loop will go on until server closes the stream
					while (await search.ResponseStream.MoveNext()) {
						var currentContact = search.ResponseStream.Current;
						Console.WriteLine(currentContact.Str);
					}
				} catch (RpcException rpcException) {
					Console.WriteLine("There was an error communicating with gRPC server");
					Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
				} catch (Exception ex) {
					Console.WriteLine(ex);
				}
			}
		}

	}
}
