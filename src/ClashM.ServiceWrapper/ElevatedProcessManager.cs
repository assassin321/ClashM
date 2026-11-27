using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ElevatedProcessManager : BackgroundService
{
    private readonly ILogger<ElevatedProcessManager> _logger;
    private Process? _elevatedProcess;

    public ElevatedProcessManager(ILogger<ElevatedProcessManager> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var elevatedExe = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ClashM.Elevated.exe");

        if (!File.Exists(elevatedExe))
        {
            _logger.LogError("找不到 ClashM.Elevated.exe");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = elevatedExe,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = "--token YOUR_TOKEN --client-sid YOUR_SID --owner 0 --owner-start 0"
                };

                _elevatedProcess = Process.Start(psi);
                if (_elevatedProcess == null)
                {
                    _logger.LogError("无法启动 Elevated 进程");
                    return;
                }

                _logger.LogInformation("Elevated 进程已启动，PID: {Pid}", _elevatedProcess.Id);
                await _elevatedProcess.WaitForExitAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elevated 进程异常");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }

    public override void Dispose()
    {
        _elevatedProcess?.Kill(entireProcessTree: true);
        _elevatedProcess?.Dispose();
        base.Dispose();
    }
}