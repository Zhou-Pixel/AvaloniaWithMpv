using System;
using System.Runtime.InteropServices;

namespace AvaloniaApplication2.FFI.Raw;

[StructLayout(LayoutKind.Explicit)]
public struct MpvNodeData
{
    [FieldOffset(0)] public IntPtr str;  /** valid if format==MPV_FORMAT_STRING */
    [FieldOffset(0)] public int flag;  /** valid if format==MPV_FORMAT_FLAG   */
    [FieldOffset(0)] public ulong intData;  /** valid if format==MPV_FORMAT_INT64  */
    [FieldOffset(0)] public double doubleData; /** valid if format==MPV_FORMAT_DOUBLE */
    [FieldOffset(0)] public IntPtr nodeList; // valid if format==MPV_FORMAT_NODE_ARRAY or if format==MPV_FORMAT_NODE_MAP
    [FieldOffset(0)] public IntPtr byteArray; // valid if format==MPV_FORMAT_BYTE_ARRAY
}