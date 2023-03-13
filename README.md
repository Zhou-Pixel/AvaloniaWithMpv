# a way to use mpv and opengl in Avalonia

## some problems:
- test on Windows11 and Macos but it only works on Windows maybe it is a bug from Avalonia
- use a bad way (create a static var) to make delegate live long enough when the delegate is invoked by C library



## info
> modify AvaloniaApplication2.FFI.Raw.Mpv.MpvDllPath correctly before you run this program