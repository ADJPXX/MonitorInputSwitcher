using MonitorInputSwitcher.Services;

namespace MonitorInputSwitcher;

public static class Program
{
    public static void Main(string[] args)
    {
        var monitorService = new MonitorService();
        monitorService.ListarMonitores();

    }
}