using System;
using System.Text;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using RevService.Generated;

namespace client {
	class Program {
		static void Main(string[] args) {
			//Console.WriteLine("Hello World!");
			Console.WriteLine("Enter q to quit... "); 
			Console.WriteLine("enter 'u <string>' to reverse a string using a unary gRPC call. ");
			Console.WriteLine("enter 's <string>' to reverse a string using a server streaming gRPC call. ");

			while (true) {
				Console.Write("> ");
				string s = Console.ReadLine();
				if (s == "q") break;
				if (s.StartsWith("u ") && s.Length > 2) {
					var reverseString = Reverse(s.Substring(2)).GetAwaiter().GetResult();
					Console.WriteLine(reverseString);
				} else if (s.StartsWith("s ") && s.Length > 2) {
					SearchForContact(s.Substring(2)).GetAwaiter().GetResult();
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
				Console.WriteLine("Printing out stream of found contacts");

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
