using Grpc.Core;
using GrpcService;

namespace GrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        // Unary
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        // Server streaming
        public override async Task SayHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < 5; i++)
                {
                    await responseStream.WriteAsync(new HelloReply
                    {
                        Message = "Hello " + request.Name + " " + i
                    });
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        // Client streaming
        public override async Task<CounterResponse> AccumulateCount(IAsyncStreamReader<CounterRequest> requestStream, ServerCallContext context)
        {
            int count = 0;
            await foreach (var message in requestStream.ReadAllAsync())
            {
                count += message.Count;
            }
            return new CounterResponse { Count = count };
        }

        // Bi-directional streaming
        public override async Task Echo(IAsyncStreamReader<EchoeRequest> requestStream, IServerStreamWriter<EchoResponse> responseStream, ServerCallContext context)
        {
            string combinedMessage = "";
            // Read requests in a background task.
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    combinedMessage += " " + message.Message;
                }
            });

            // Send responses until the client signals that it is complete.
            while (!readTask.IsCompleted)
            {
                await responseStream.WriteAsync(new EchoResponse { Message = combinedMessage });
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }

        /*public override Task<ExampleResponse> UnaryCall(ExampleRequest request, ServerCallContext context)
        {
            var response = new ExampleResponse();
            return Task.FromResult(response);
        }

        public override async Task StreamingFromServer(ExampleRequest request, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < 5; i++)
                {
                    await responseStream.WriteAsync(new ExampleResponse());
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        public override async Task<ExampleResponse> StreamingFromClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                // ...
            }
            return new ExampleResponse();
        }

        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            // Read requests in a background task.
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    // Process request.
                }
            });

            // Send responses until the client signals that it is complete.
            while (!readTask.IsCompleted)
            {
                await responseStream.WriteAsync(new ExampleResponse());
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }*/
    }
}
