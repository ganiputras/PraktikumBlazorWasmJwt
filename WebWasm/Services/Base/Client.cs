namespace WebWasm.Services.Base;

public partial class Client : IClient
{
   public HttpClient HttpClient { get; } = new();
}