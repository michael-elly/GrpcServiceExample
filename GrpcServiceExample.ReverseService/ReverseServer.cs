using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using RevService.Generated;

namespace GrpcServiceExample.ReverseService {
    public class ReverseServer {
        private readonly Server _server;

        public ReverseServer() {
            _server = new Server() {
                Services = { RevService.Generated.RevService.BindService(new ReverseServiceImplementation()) },
                Ports = { new ServerPort("localhost", 11111, ServerCredentials.Insecure) }
            };
        }

        public void Start() { _server.Start(); }

        public async Task ShutDownAsync() { await _server.ShutdownAsync(); }
    }
}
