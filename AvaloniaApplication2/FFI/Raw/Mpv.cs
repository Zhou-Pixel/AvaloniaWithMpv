using System;
using System.Runtime.InteropServices;

using MpvHandle = System.IntPtr;

namespace AvaloniaApplication2.FFI.Raw;


[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CommonCallBack(IntPtr data);

public static class Mpv
{
    //macos libmpv install by homebrew
    // private const string MpvDllPath = "/usr/local/Cellar/mpv/0.35.1/lib/libmpv.dylib";
    private const string MpvDllPath = "libmpv-2.dll";

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