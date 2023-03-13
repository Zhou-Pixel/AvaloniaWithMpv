using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication2.FFI.Raw;

namespace AvaloniaApplication2.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            //var handle = Mpv.mpv_create();
            //int ret = Mpv.mpv_set_option_string(handle, "terminal", "yes");
            //ret = Mpv.mpv_initialize(handle);
            //Console.WriteLine();

            
            var fileDialog = new OpenFileDialog();
            fileDialog.Title = "选择文件";
            var ret = await fileDialog.ShowAsync(this);
            if (ret == null || ret.Length != 1)
            {
                Console.WriteLine(value: "just choose a video");
                return;
            }
            this.GlControl.CommandNode(args: new List<string>()
            {
                "loadfile",
                ret[0]
            });
            foreach (var i in ret)
            {
                Console.WriteLine(i);
            }
        }
    }
}