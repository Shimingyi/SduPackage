using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SduPackage.Functions
{
    public class IdentifyCheckCode
    {
        public static string Identify(BitmapImage _CheckCodeImage)
        {
            string result = string.Empty;
            
            return result;
        }
        
        public async Task<string> openImage(BitmapImage _CheckCodeImage)
        {
            string result = string.Empty;
            //将图片类型转化为byte[]类型
            Windows.Storage.Streams.RandomAccessStreamReference rasr = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(_CheckCodeImage.UriSource);
            var streamWithContent = await rasr.OpenReadAsync();
            byte[] buffer = new byte[streamWithContent.Size];
            await streamWithContent.ReadAsync(buffer.AsBuffer(),(uint)streamWithContent.Size, Windows.Storage.Streams.InputStreamOptions.None);
            
            
            
            return result;
        }

    }
}
