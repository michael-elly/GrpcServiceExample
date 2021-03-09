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
			Console.WriteLine("Enter q to quit...");
			while (true) {
				Console.Write("> ");
				string s = Console.ReadLine();
				if (s == "q") break;
				var reverseString = Reverse(s).GetAwaiter().GetResult();
				Console.WriteLine(reverseString);				
			}
		}

		static async Task<string> Reverse(string input) {
			Channel channel = new Channel("localhost", 11111, ChannelCredentials.Insecure);
			RevService.Generated.RevService.RevServiceClient client = new RevService.Generated.RevService.RevServiceClient(channel);
			var data = new RevService.Generated.Data() { Str = input };
			var res = await client.ReverseAsync(data);
			return res.Str;
		}
	}
}
