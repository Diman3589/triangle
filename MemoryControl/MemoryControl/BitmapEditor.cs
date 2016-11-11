﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MemoryControl
{
    class BitmapEditor : IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly BitmapData _bmpData;
        private readonly byte[] _rgbValues;
        private readonly int _size;

        public BitmapEditor(Bitmap bitmap)
        {
            _bitmap = bitmap;
            var rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            _bmpData = _bitmap.LockBits(rect, ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            _size = _bmpData.Stride*_bmpData.Height;
            _rgbValues = new byte[_size];
            Marshal.Copy(_bmpData.Scan0, _rgbValues, 0, _size);
        }


        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            if (x >= _bmpData.Width || y >= _bmpData.Height)
            {
                throw new ArgumentException("not valid coordinates");
            }

            var pixelCoord = y*_bmpData.Stride + x*3;

            _rgbValues[pixelCoord] = r;
            _rgbValues[pixelCoord + 1] = g;
            _rgbValues[pixelCoord + 2] = b;
        }

        public void Dispose()
        {
            Marshal.Copy(_rgbValues, 0, _bmpData.Scan0, _size);
            _bitmap.UnlockBits(_bmpData);
            _bitmap.Save("d:/tmp.bmp");
            _bitmap.Dispose();
        }

        public static void Main(string[] args)
        {
            var bitmap = (Bitmap) Image.FromFile("d:/sis.bmp");
            using (var bitmapEditor = new BitmapEditor(bitmap))
            {
                var timer = new Timer();
                using (timer.Start())
                {
                    for (var i = 0; i < 206; i++)
                    {
                        for (var j = 0; j < 137; j++)
                        {
                            var r = (byte) new Random().Next(0, 85);
                            var g = (byte) new Random().Next(85, 170);
                            var b = (byte) new Random().Next(170, 255);
                            bitmapEditor.SetPixel(i, j, r, g, b);
                        }
                    }
                }
                Console.WriteLine((double)timer.ElapsedMilliseconds / 1000);

                var timer1 = new Timer();
            var bitmap1 = (Bitmap) Image.FromFile("d:/sis.bmp");
                using (timer1.Start())
                {
                    for (var i = 0; i < 206; i++)
                    {
                        for (var j = 0; j < 137; j++)
                        {
                            var r = (byte) new Random().Next(85, 170);
                            var g = (byte) new Random().Next(0, 170);
                            var b = (byte) new Random().Next(170, 255);
                            var color = Color.FromArgb(r,g,b);
                            bitmap1.SetPixel(i, j, color);
                        }
                    }
                }
                Console.WriteLine((double)timer1.ElapsedMilliseconds / 1000);
                Console.ReadLine();
            }
        }
    }
}