using System;
using System.Runtime.InteropServices;

using MpvHandle = System.IntPtr;

namespace AvaloniaApplication2.FFI.Raw;

internal static class WinFunctions
{
    // https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibrarya
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    // https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-freelibrary
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
    public static extern int FreeLibrary(IntPtr hModule);

    // https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-getprocaddress
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string lProcName);
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CommonCallBack(IntPtr data);

public static class Mpv
{
    //macos libmpv install by homebrew
    // private const string MpvDllPath = "/usr/local/Cellar/mpv/0.35.1/lib/libmpv.dylib";
    private const string MpvDllPath = "libmpv-2.dll";
    static Mpv()
    {
    }

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern MpvHandle mpv_create();

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_initialize(MpvHandle ctx);

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_set_option_string(MpvHandle ctx, string name, string data);

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void mpv_set_wakeup_callback(MpvHandle ctx, CommonCallBack cb, IntPtr data);

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr mpv_wait_event(MpvHandle ctx, double timeout);

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_render_context_create(out IntPtr ctx, IntPtr mpv, IntPtr paras);


    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void mpv_render_context_set_update_callback(IntPtr rednPtrctx,
        CommonCallBack callback,
        IntPtr data);

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_render_context_render(IntPtr ctx, IntPtr paras);

    [DllImport(MpvDllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_command_node(IntPtr mpv, IntPtr args, out MpvNode res);

}


public delegate void MpvRendeContextSetUpdateCallback(IntPtr ctx, CommonCallBack cb, IntPtr data);

public class DynamicMpv
{
    public static MpvRendeContextSetUpdateCallback RendeContextSetUpdateCallback { set; get; }

    static DynamicMpv()
    {
        var handle = WinFunctions.LoadLibrary("C:\\Users\\zhou\\Documents\\Code\\CSharp\\AvaloniaApplication2\\AvaloniaApplication2\\bin\\Debug\\net6.0\\libmpv-2.dll");
        var ptr = WinFunctions.GetProcAddress(handle, "mpv_render_context_set_update_callback");
        RendeContextSetUpdateCallback = (MpvRendeContextSetUpdateCallback)Marshal.GetDelegateForFunctionPointer(ptr, typeof(MpvRendeContextSetUpdateCallback));
    }
}

