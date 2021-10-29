using FFImageLoading;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using ImageCircle.Forms.Plugin.Abstractions;
using SQLiteMapaTeste.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SQLiteMapaTeste.Service
{
    public class ImageService
    {

        public static byte[] ConvertImageToByte(string path, Assembly assembly)
        {
            var stream = GetImageFromStream(path, assembly);
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var buffer = memoryStream.ToArray();
            return buffer;
        }
        public static byte[] ConvertImageToByte(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var buffer = memoryStream.ToArray();
            return buffer;
        }
        public static Stream GetImageFromStream(string path, Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream(path);
            return stream;
        }
        
        public static Xamarin.Forms.View GetIcon(User user, double width, double height, float borderThickness = 5)
        {

            FFImageLoading.ImageService.Instance.LoadUrl("")
                .Retry(3, 200)
                .DownSample(75, 75);
            var a = new EmbeddedResourceImageSource("p1.png", App.assembly);

            TaskCompletionSource<Stream> imageTcs = new TaskCompletionSource<Stream>();
            imageTcs.TrySetResult(new MemoryStream(user.Buffer));

            var taskParams = FFImageLoading.ImageService.Instance.LoadStream(sct => imageTcs.Task);
            taskParams = taskParams.BitmapOptimizations(false);
            taskParams = taskParams.WithPriority(LoadingPriority.High);
            taskParams = taskParams.WithCache(FFImageLoading.Cache.CacheType.Disk);



            var img = new CachedImage
            {
                Aspect = Aspect.AspectFill,
                CacheDuration = new TimeSpan(0,5,0),
                Source = Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(user.Buffer)),
                Transformations = new List<ITransformation>() 
                {
                    new CircleTransformation(borderSize:40,"#68ce37"),
                },
            };

            AbsoluteLayout.SetLayoutBounds(img, new Rectangle(x: 0, y: 0, width: width, height: height));

            return (new StackLayout
            {
                WidthRequest = width,
                HeightRequest = height,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                AnchorX = 0.5,
                AnchorY = 1,
                Children =
                {
                    new AbsoluteLayout
                    {
                        Children =
                        {
                            img
                        }
                    }
                }
            });
        }
    }
}
