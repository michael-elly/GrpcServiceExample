using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using RevService.Generated;

namespace GrpcServiceExample.ReverseService {
	public class ReverseServiceImplementation : RevService.Generated.RevService.RevServiceBase {
		public override Task<Data> Reverse(Data request, ServerCallContext context) {
			var response = new Data() { Str = new string(request.Str.Reverse().ToArray()) };
			Console.WriteLine($" --> [{context.Host}|{context.Method}|{context.Peer}] Reverse: {request.Str} --> {response.Str}");
			System.Threading.Thread.Sleep(500); // simulate long processing
			//System.Threading.Thread.Sleep(5000);
			return Task.FromResult(response);
		}

		public override async Task ReverseSt(Data request, IServerStreamWriter<Data> responseStream, ServerCallContext context) {
			//return base.ReverseSt(request, responseStream, context);
			string s = request.Str;
			if (s.Length < 2) s.PadRight(2); // make sure we have at least 2 chars
			if (s.Length % 2 == 1) s += " "; // make sure we have even # of chars
			for (int i = s.Length-2; i >= 0; i-=2) {
				Data d = new Data() { Str = s.Substring(i, 2) };
				await responseStream.WriteAsync(d);
				//await responseStream.WriteAsync(new Data() { Str = new string("2") });				
				Console.WriteLine($" --> [{context.Host}|{context.Method}|{context.Peer}] ReverseSt: {request.Str} --> {d.Str}");
				System.Threading.Thread.Sleep(500); // simulate long processing
			}
		}
	}
}
