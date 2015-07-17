using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using System.IO;

namespace SduPackage.Functions
{
    public class IdentifyCheckCode
    {
        static byte[] _origeByte;
        static int[,] _binaryByte;
        static int[][,] _imageMutrix;

        public static string Identify(WriteableBitmap _CheckCodeImage)
        {
            string result = string.Empty;
            _origeByte = _CheckCodeImage.PixelBuffer.ToArray();
            GrayByPixels(_origeByte);
            CutByPixels(_origeByte);
            for (int i = 0; i < 4; i++)
            {
                result += GetImageText(_imageMutrix[i]);
            }
            return result;
        }

        #region 图像处理方法
        /// <summary>
        /// 处理验证码，将构成数字的像素变成黑色，其余的变成白色
        /// </summary>
        /// <param name="_byte"></param>
        public static void GrayByPixels(byte[] _byte)
        {
            for (int i = 0; i < 1200; i++)
            {
                int index = (4 * i);
                byte _b = _byte[index];
                byte _g = _byte[index + 1];
                byte _r = _byte[index + 2];
                //通过PS的分析，在这种验证码中，构成数字的像素的rgb中总含有255
                if ((_b == 255) || (_g == 255) || (_r == 255))
                {
                    _byte[index] = 0;
                    _byte[index + 1] = 0;
                    _byte[index + 2] = 0;
                }
                else
                {
                    _byte[index] = 255;
                    _byte[index + 1] = 255;
                    _byte[index + 2] = 255;
                }
            }
        }

        /// <summary>
        /// 根据最上边和最左边，以及数字的固定长宽，画出边界
        /// </summary>
        /// <param name="_byte"></param>
        /// <returns></returns>
        public static void CutByPixels(byte[] _byte)
        {
            BinaryImage(_origeByte);
            int[] left = new int[4];
            left[0] = 4; left[1] = 17; left[2] = 30; left[3] = 43;
            int[] top = new int[4];

            //逐行扫描找出最上方的点
            int count = 0;
            for (int row = 0; row < 20; row++)
            {
                for (int col = 0; col < 60; col++)
                {
                    if (_binaryByte[row, col] == 1)
                    {
                        if (col >= 4 && col <= 9)
                        {
                            if (top[0] == 0)
                            {
                                top[0] = row;
                                count++;
                            }
                            if (count == 4)
                            {
                                col = 60;
                                row = 20;
                            }
                        }
                        if (col >= 17 && col <= 22)
                        {
                            if (top[1] == 0)
                            {
                                top[1] = row;
                                count++;
                            }
                            if (count == 4)
                            {
                                col = 60;
                                row = 20;
                            }
                        }
                        if (col >= 30 && col <= 35)
                        {
                            if (top[2] == 0)
                            {
                                top[2] = row;
                                count++;
                            }
                            if (count == 4)
                            {
                                col = 60;
                                row = 20;
                            }
                        }
                        if (col >= 43 && col <= 48)
                        {
                            if (top[3] == 0)
                            {
                                top[3] = row;
                                count++;
                            }
                            if (count == 4)
                            {
                                col = 60;
                                row = 20;
                            }
                        }
                    }
                }
            }
            _imageMutrix = new int[4][,];
            for (int i = 0; i < 4; i++)
            {
                _imageMutrix[i] = new int[11, 7];
            }
            for (int i = 0; i < 4; i++)
            {
                //Debug.WriteLine("imageMutrix[" + i + "]:");
                for (int row = 0; row < 11; row++)
                {
                    string line = string.Empty;
                    for (int col = 0; col < 6; col++)
                    {
                        _imageMutrix[i][row, col] = _binaryByte[top[i] + row, left[i] + col];
                        line += (_imageMutrix[i][row, col] + " ");
                    }
                    //Debug.WriteLine(line);

                }
            }
        }

        /// <summary>
        /// 将图片变为01矩阵
        /// </summary>
        /// <param name="_LineByte"></param>
        public static void BinaryImage(byte[] _Byte)
        {
            _binaryByte = new int[20, 60];
            for (int row = 0; row < 20; row++)
            {
                string line = "";
                for (int col = 0; col < 60; col++)
                {
                    if (_Byte[row * 60 * 4 + col * 4] == 255)
                        _binaryByte[row, col] = 0;
                    else
                        _binaryByte[row, col] = 1;
                    line += (_binaryByte[row, col] + " ");
                }
                //Debug.WriteLine(line);
            }
        }

        /// <summary>
        /// 由分割后的图片，根据特征值找出每一个图片对应的数字
        /// </summary>
        /// <param name="_imageByte"></param>
        /// <returns></returns>
        public static string GetImageText(int[,] _imageByte)
        {
            string result = string.Empty;
            int[] EveryValue = new int[10];
            for (int i = 0; i < 10; i++)
            {
                int value = 0;
                for (int row = 0; row < 11; row++)
                {
                    for (int col = 0; col < 6; col++)
                    {
                        if (_imageByte[row, col] == 1)
                        {
                            if (numbers[i][row, col] == 1)
                            {
                                value++;
                            }
                        }
                    }
                }
                EveryValue[i] = value;
            }
            result += MaxValue(EveryValue)[0];
            return result;
        }
        #endregion

        #region 图像处理
        private async Task<WriteableBitmap> byteToWritebleBitmap(byte[] _byte, int _weight, int _height)
        {
            WriteableBitmap _wb = new WriteableBitmap(_weight, _height);
            int _area = _weight * _height * 4;
            using (Stream stream = _wb.PixelBuffer.AsStream())
            {
                if (stream.CanWrite)
                {
                    byte[] pixelArray = new byte[_area];
                    for (int i = 0; i < _area; i++)
                    {
                        pixelArray[i] = _byte[i];
                    }
                    await stream.WriteAsync(pixelArray, 0, pixelArray.Length);
                    stream.Flush();
                }
            }
            return _wb;
        }

        private static void WriteByte(byte[] _byte)
        {
            for (int i = 0; i < 20; i++)
            {
                string _line = string.Empty;
                for (int j = 0; j < 240; j++)
                {
                    _line += (_byte[i * 60 + j].ToString() + " ");
                }
                Debug.WriteLine(_line);
            }
        }

        private static void WriteCustomByte(byte[] _byte, int _weight, int _height)
        {
            for (int i = 0; i < _height; i++)
            {
                string line = string.Empty;
                for (int j = 0; j < _height; j++)
                {
                    line += (_byte[i * _weight + j].ToString() + " ");
                }
                Debug.WriteLine(line);
            }
        }

        private static void WriteCustomInt(int[] _byte, int _weight, int _height)
        {
            for (int i = 0; i < _height; i++)
            {
                string line = string.Empty;
                for (int j = 0; j < _height; j++)
                {
                    line += (_byte[i * _weight + j].ToString() + " ");
                }
                Debug.WriteLine(line);
            }
        }

        private static int[] MaxValue(int[] _number)
        {
            int[] result = new int[2];
            int MaxValue = 0;
            int index = 0;
            for (int i = 0; i < _number.Length; i++)
            {
                if (_number[i] > MaxValue)
                {
                    MaxValue = _number[i];
                    index = i;
                }
            }
            result[0] = index;
            result[1] = MaxValue;
            return result;
        }
        #endregion

        #region 特征样本
        public static int[,] _0 ={
		{0,0,1,1,1,0,0},
		{0,1,1,0,1,1,0},
		{0,1,0,0,0,1,0},
		{1,1,0,0,0,1,1},
		{1,1,0,0,0,1,1},
		{1,1,0,0,0,1,1},
		{1,1,0,0,0,1,1},
		{1,1,0,0,0,1,1},
		{0,1,0,0,0,1,0},
		{0,1,1,0,1,1,0},
		{0,0,1,1,1,0,0}
	    };
        public static int[,] _1 =
		{   
		{1,1,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,1,0,0},
		{1,1,1,1,1,1}
		};
        public static int[,] _2 ={
		{0,1,1,1,0,0},
		{1,1,1,1,1,0},
		{0,0,0,1,1,1},
		{0,0,0,1,1,1},
		{0,0,0,1,1,1},
		{0,0,0,1,1,0},
		{0,0,1,1,0,0},
		{0,0,1,0,0,0},
		{0,1,0,0,0,0},
		{1,1,1,1,1,1},
		{1,1,1,1,1,1}
	    };
        public static int[,] _3 ={
		{0,1,1,1,1,0},
		{1,1,1,1,1,0},
		{1,0,0,1,1,1},
		{0,0,0,1,1,0},
		{0,0,1,1,1,0},
		{0,1,1,1,1,0},
		{0,0,0,1,1,1},
		{0,0,0,0,1,1},
		{0,0,0,0,1,1},
		{1,1,0,0,1,0},
		{1,1,1,1,0,0}
	    };
        public static int[,] _4 ={
		{0,0,0,0,1,1},
		{0,0,0,1,1,0},
		{0,0,1,1,1,0},
		{0,0,1,1,1,1},
		{0,1,0,1,1,0},
		{0,1,0,1,1,0},
		{1,0,0,1,1,0},
		{1,1,1,1,1,1},
		{1,1,1,1,1,1},
		{0,0,0,1,1,0},
		{0,0,0,1,1,0}
	    };
        public static int[,] _5 ={
		{0,1,1,1,1,1},
		{0,1,1,1,1,1},
		{0,1,0,0,0,0},
		{1,1,1,1,0,0},
		{1,1,1,1,1,0},
		{0,0,0,1,1,1},
		{0,0,0,0,1,1},
		{0,0,0,0,0,1},
		{0,0,0,0,0,1},
		{1,1,0,0,1,0},
		{1,1,1,1,1,0}
	    };
        public static int[,] _6 =
		{
		{0,0,0,1,1,1},
		{0,0,1,1,1,0},
		{0,1,1,0,0,0},
		{1,1,1,0,0,0},
		{1,1,1,1,1,0},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{0,1,1,1,1,0}
		};
        public static int[,] _7 ={
		{1,1,1,1,1,1},
		{1,1,1,1,1,1},
		{0,0,0,0,0,1},
		{0,0,0,0,1,0},
		{0,0,0,0,1,0},
		{0,0,0,0,1,0},
		{0,0,0,1,0,0},
		{0,0,0,1,0,0},
		{0,0,1,1,0,0},
		{0,0,1,0,0,0},
		{0,0,1,0,0,0}
	    };
        public static int[,] _8 ={
		{0,1,1,1,1,0},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{1,1,1,0,1,1},
		{1,1,1,1,1,0},
		{0,1,1,1,1,0},
		{0,0,0,1,1,1},
		{1,1,0,0,1,1},
		{1,0,0,0,1,1},
		{1,1,0,1,1,1},
		{0,1,1,1,1,0}
	    };
        public static int[,] _9 ={
		{0,1,1,1,1,0},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{1,1,0,0,1,1},
		{0,1,1,1,1,1},
		{0,0,0,0,1,1},
		{0,0,0,1,1,0},
		{0,0,1,1,0,0},
		{1,1,0,0,0,0}
	    };
        public static int[][,] numbers = new int[10][,] { _0, _1, _2, _3, _4, _5, _6, _7, _8, _9 };
        #endregion
    }
}