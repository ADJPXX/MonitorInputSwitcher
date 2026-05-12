using System.Runtime.InteropServices;
using MonitorInputSwitcher.Native;

namespace MonitorInputSwitcher.Services;

public class MonitorService
{
    private readonly User32.MonitorEnumProc _callback;

    public MonitorService()
    {
        _callback = MonitorEnum;
    }

    public void ListarMonitores()
    {
        User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, _callback, IntPtr.Zero);
    }

    private bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
    {
        const string targetMonitor = "Alienware";
        
        const int displayPort = 15;
        
        const int hdmi1 = 18;
        
        var success1 = Dxva2.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out var monitorCount);
        
        if (!success1)
        {
            var result = Marshal.GetLastWin32Error();
            Console.WriteLine(result);
            return true;
        }

        var physicalMonitors = new PHYSICAL_MONITOR[monitorCount];
        
        var success2 = Dxva2.GetPhysicalMonitorsFromHMONITOR(hMonitor, monitorCount, physicalMonitors);

        if (!success2)
        {
            var result = Marshal.GetLastWin32Error();
            Console.WriteLine(result);
            return true;
        }

        foreach (var monitor in physicalMonitors)
        {
            uint currentValue;
            uint maxValue;
            uint vcpType;
            
            var result = Dxva2.GetVCPFeatureAndVCPFeatureReply(monitor.hPhysicalMonitor, 0x60, out vcpType, out currentValue, out maxValue);

            if (result)
            {
                var input = currentValue & 0xFF;

                if (!monitor.szPhysicalMonitorDescription.Contains(targetMonitor, StringComparison.OrdinalIgnoreCase))
                    continue;

                switch (input)
                {
                    case hdmi1:
                    {
                        var inputChanged = Dxva2.SetVCPFeature(monitor.hPhysicalMonitor, 0x60, displayPort);
                        
                        if (!inputChanged)
                        {
                            var erroWin = Marshal.GetLastWin32Error();
                            Console.WriteLine($"ERRO AO TROCAR INPUT: {erroWin}");
                        }
                        
                        break;
                    }

                    case displayPort:
                    {
                        var inputChanged = Dxva2.SetVCPFeature(monitor.hPhysicalMonitor, 0x60, hdmi1);
                        
                        if (!inputChanged)
                        {
                            var erroWin = Marshal.GetLastWin32Error();
                            Console.WriteLine($"ERRO AO TROCAR INPUT: {erroWin}");
                        }
                        
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("NÃO FOI POSSIVEL LER VCP");
            }
        }
        
        var monitorDestroyed = Dxva2.DestroyPhysicalMonitors(monitorCount, physicalMonitors);

        Console.WriteLine(monitorDestroyed ? "MONITORES DESTRUIDOS." : "FALHA AO DESTRUIR OS MONITORES.");

        return true;
    }
}