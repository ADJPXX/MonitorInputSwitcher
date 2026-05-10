using System.Runtime.InteropServices;

namespace MonitorInputSwitcher.Native;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct PHYSICAL_MONITOR
{
    public IntPtr hPhysicalMonitor;
    
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szPhysicalMonitorDescription;
}

public struct RECT
{
    public int left;
    public int top;
    public int right;
    public int bottom;
}