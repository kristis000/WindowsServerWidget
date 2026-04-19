using System.Net.Sockets;

namespace WindowsServerWidget;

internal sealed class TcpServerProbe
{
    public async Task<ProbeResult> ProbeAsync(AppSettings settings, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new TcpClient();
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(settings.ConnectTimeoutMs);

            await client.ConnectAsync(settings.Host, settings.Port, timeoutCts.Token);

            return new ProbeResult(ServerState.Online, $"TCP {settings.Host}:{settings.Port} accepted a connection.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new ProbeResult(ServerState.Offline, $"TCP {settings.Host}:{settings.Port} timed out.");
        }
        catch (Exception ex) when (ex is SocketException or InvalidOperationException)
        {
            return new ProbeResult(ServerState.Offline, ex.Message);
        }
    }
}
