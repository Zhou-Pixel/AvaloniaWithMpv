using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.Threading;
using AvaloniaApplication2.FFI.Raw;

namespace AvaloniaApplication2.Controls;

using Avalonia.OpenGL.Controls;

public class OpenGlControl : OpenGlControlBase
{
    private bool _first = true;
    private IntPtr _mpvHanle = Mpv.mpv_create();
    private IntPtr _mpvRenderCtx;
    private static CommonCallBack _wakupCallBack;// a simple way to make the delegete live long enough when the delegate is passed to C library
    private static CommonCallBack _updateCallBack;// live long enough

    public OpenGlControl()
    {
        _wakupCallBack = this.wake_up;
        _updateCallBack = this.updateGl;
        var retcode = Mpv.mpv_set_option_string(_mpvHanle, "terminal", "yes");
        //retcode = Mpv.mpv_set_option_string(_mpvHanle, "msg-level", "all=v");
        int code = Mpv.mpv_initialize(_mpvHanle);
        if (code != 0)
        {
            throw new Exception("init");
        }
        Mpv.mpv_set_wakeup_callback(_mpvHanle, _wakupCallBack, IntPtr.Zero);
    }



    public void CommandNode(List<string> args)
    {   
        //Mpv.mpv_command_node()
        MpvNode node = new MpvNode();
        node.format = MpvFormat.MPV_FORMAT_NODE_ARRAY;


        MpvNodeList list = new MpvNodeList();


        list.num = args.Count;
        var nodeList = new MpvNode[list.num];
        for (int i = 0; i < nodeList.Length; i++)
        {
            nodeList[i].format = MpvFormat.MPV_FORMAT_STRING;
            nodeList[i].data.str = Marshal.StringToHGlobalAnsi(args[i]);//todo: free mem
        }
        list.nodeValues = Marshal.UnsafeAddrOfPinnedArrayElement(nodeList, 0);

        IntPtr listPtr = Marshal.AllocHGlobal(Marshal.SizeOf(list));
        Marshal.StructureToPtr(list, listPtr, true);
        node.data.nodeList = listPtr;


        MpvNode res = new MpvNode();
        unsafe
        {
            IntPtr nodePtr = new IntPtr((void*)&node);
            int code = Mpv.mpv_command_node(_mpvHanle, nodePtr, out res);
            Console.WriteLine();
        }
    }

    private void wake_up(IntPtr data)
    {
        Task.Run(() => //avoid block here 
        {
            while (true)
            {
                Console.WriteLine("start");
                var ptr = Mpv.mpv_wait_event(_mpvHanle, 0);
                Console.WriteLine("end");
                var evt = Marshal.PtrToStructure<MpvEvent>(ptr);
                if (evt.event_id == MpvEventId.MPV_EVENT_NONE)
                {
                    break;
                }
            }
        });

    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        Console.WriteLine($" this control {this.Bounds.Width} {this.Bounds.Height}");
        MpvOpenglFbo fbo = new MpvOpenglFbo()
        {
            fbo = fb,
            w = (int)this.Bounds.Width,
            h = (int)this.Bounds.Height,
            internal_format = 0
        };
        unsafe
        {
            var flip = 1;
            IntPtr fboPtr = Marshal.AllocHGlobal(Marshal.SizeOf(fbo));
            Marshal.StructureToPtr(fbo, fboPtr, true);

            var _params = new MpvRenderParam[]
            {
                new MpvRenderParam()
                {
                    type = MpvRenderParamType.MPV_RENDER_PARAM_OPENGL_FBO,
                    data = fboPtr,
                },
                new MpvRenderParam()
                {
                    type = MpvRenderParamType.MPV_RENDER_PARAM_FLIP_Y,
                    data = new IntPtr(&flip)
                },
                new MpvRenderParam()
                {
                    type = MpvRenderParamType.MPV_RENDER_PARAM_INVALID,
                    data = IntPtr.Zero
                }
            };
            Marshal.FreeHGlobal(fboPtr);
            Mpv.mpv_render_context_render(_mpvRenderCtx, Marshal.UnsafeAddrOfPinnedArrayElement(_params, 0));
        }
        // Console.WriteLine("request update from render func");
        // this.RequestNextFrameRendering();//need this to update

    }


    private async void updateGl(IntPtr data)
    {
        // Console.WriteLine("request update");
        await Dispatcher.UIThread.InvokeAsync(this.RequestNextFrameRendering);
    }
    
    protected override void OnOpenGlInit(GlInterface gl)
    {
        
        if (!_first)
        {
            Console.WriteLine("init twice");
            return;
        }
        else
        {
            _first = false;
        }
        MpvOpenglInitParams para = new()
        {
            get_pro_address = (ctx, name) =>
            {
                Console.WriteLine($"get proc name ==> {name}");
                return gl.GetProcAddress(name);
            }
        };
        IntPtr ptrs = Marshal.StringToHGlobalAnsi("opengl");
        IntPtr paramsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(para));
        Marshal.StructureToPtr(para, paramsPtr, true);
        unsafe
        {

            var _params = new MpvRenderParam[]
        {
            new MpvRenderParam()
            {
                type = MpvRenderParamType.MPV_RENDER_PARAM_API_TYPE,
                data = ptrs
            },
            new MpvRenderParam()
            {
                type = MpvRenderParamType.MPV_RENDER_PARAM_OPENGL_INIT_PARAMS,
                data = paramsPtr,
            },
            new MpvRenderParam()
            {
                type = MpvRenderParamType.MPV_RENDER_PARAM_INVALID,
                data = IntPtr.Zero
            }
        };
        var code = Mpv.mpv_render_context_create(out _mpvRenderCtx, _mpvHanle, Marshal.UnsafeAddrOfPinnedArrayElement(_params, 0));
        }

        //DynamicMpv.RendeContextSetUpdateCallback(_mpvRenderCtx, _updateCallBack, IntPtr.Zero);
        Mpv.mpv_render_context_set_update_callback(_mpvRenderCtx, _updateCallBack, IntPtr.Zero);
        Marshal.FreeHGlobal(paramsPtr);

        Marshal.FreeHGlobal(ptrs);
    }

}