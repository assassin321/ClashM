using Microsoft.Extensions.Hosting;

namespace ClashM.Elevated;

/// <summary>
/// 当以 Windows 服务运行时，由这个类接管生命周期
/// </summary>
public class ElevatedServiceLifetime : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly string[] _originalArgs;

    public ElevatedServiceLifetime(IHostApplicationLifetime lifetime, string[] originalArgs)
    {
        _lifetime = lifetime;
        _originalArgs = originalArgs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 去掉 --service 参数后，调用你原有的 Main 逻辑
        var args = _originalArgs.Where(a => a != "--service").ToArray();
        await Program.Main(args);
    }
}