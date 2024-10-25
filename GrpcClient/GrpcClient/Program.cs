// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;

var channel = GrpcChannel.ForAddress("https://localhost:7258");

// Unary call

var client = new Greeter.GreeterClient(channel);

try
{
    var response = client.SayHelloAsync(new HelloRequest { Name = "World" }, deadline: DateTime.UtcNow.AddSeconds(5));

    var headers = await response.ResponseHeadersAsync;
    var myValueH = headers.GetValue("my-trailer-name");
    var response1 = await response.ResponseAsync;

    var trailers = response.GetTrailers();
    var myValueT = trailers.GetValue("my-trailer-name");

    Console.WriteLine("Greeting: " + response1.Message);
}

catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
{
    Console.WriteLine("Greeting timeout.");
}

// Server streaming

/*var client = new Greeter.GreeterClient(channel);
using var call = client.SayHellos(new HelloRequest { Name = "World" });

await foreach (var response in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine("Greeting: " + response.Message);
}*/


// Client streaming

/*var client = new Greeter.GreeterClient(channel);
using var call = client.AccumulateCount();

for (var i = 0; i < 5; i++)
{
    await call.RequestStream.WriteAsync(new CounterRequest { Count = i });
}
await call.RequestStream.CompleteAsync();

var response = await call;
Console.WriteLine($"Count: {response.Count}");*/

// Bi-directional streaming

/*var client = new Greeter.GreeterClient(channel);
using var call = client.Echo();

Console.WriteLine("Starting background task to receive messages");
var readTask = Task.Run(async () =>
{
    await foreach (var response in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(response.Message);
        // Echo messages sent to the service
    }
});

Console.WriteLine("Starting to send messages");
Console.WriteLine("Type a message to echo then press enter.");
while (true)
{
    var result = Console.ReadLine();
    if (string.IsNullOrEmpty(result))
    {
        break;
    }

    await call.RequestStream.WriteAsync(new EchoeRequest { Message = result });
}

Console.WriteLine("Disconnecting");
await call.RequestStream.CompleteAsync();
await readTask;*/
