using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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


            //var options = new FilePickerOpenOptions()
            //{
            //    Title = "打开文件",
            //    AllowMultiple = false,
            //    FileTypeFilter = new List<FilePickerFileType>()
            //    {
            //        new FilePickerFileType("照片")
            //        {
            //            Patterns = new List<string>()
            //            {
            //                "*.png",
            //                "*.mp4"
            //            }
            //        }
            //    }
                
            //};
            //var result = await this.StorageProvider.OpenFilePickerAsync(options);

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