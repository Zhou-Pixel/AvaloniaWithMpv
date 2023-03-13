using System;
using System.Runtime.InteropServices;

namespace AvaloniaApplication2.FFI.Raw;

[StructLayout(LayoutKind.Sequential)]
public struct MpvRenderParam
{
       public MpvRenderParamType type;
       public IntPtr data;
}