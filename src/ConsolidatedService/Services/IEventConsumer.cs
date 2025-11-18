namespace FluxoCaixa.Consolidated.Services
{
    public interface IEventConsumer
    {
        void Start(Func<string, Task> handler);
        void Stop();
    }
}
