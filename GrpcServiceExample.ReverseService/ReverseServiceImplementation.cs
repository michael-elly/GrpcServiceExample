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
			//System.Threading.Thread.Sleep(5000);
			return Task.FromResult(response);
		}

		public override async Task ReverseSt(Data request, IServerStreamWriter<Data> responseStream, ServerCallContext context) {
			//return base.ReverseSt(request, responseStream, context);
			string s = request.Str;
			for (int i = s.Length-1; i >= 0; i--) {
				Data d = new Data() { Str = s.Substring(i, 1) };
				await responseStream.WriteAsync(d);
				//await responseStream.WriteAsync(new Data() { Str = new string("2") });				
				Console.WriteLine($" --> [{context.Host}|{context.Method}|{context.Peer}] ReverseSt: {request.Str} --> {d.Str}");
			}
		}
	}
}
