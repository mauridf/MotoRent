public class MinioHealthCheckService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient();
            var response = client.GetAsync("http://localhost:9001").Result;

            if (!response.IsSuccessStatusCode)
            {
                // Tentar reiniciar ou alertar
                Console.WriteLine("MinIO não está respondendo!");
            }
        }
        catch
        {
            Console.WriteLine("Falha ao verificar MinIO");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}