using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Contrast
{// Добавил код сюда 
    public struct RGB_Color
    {
    public RGB_Color(int Red, int Green, int Blue)
            {
                R = Red;
                G = Green;
                B = Blue;
            }
    public int R, G, B;
    }

    public class Filter
    {
    private Bitmap UPicture = null;
    private BitmapData BmpData = null;
    private unsafe byte* Begin = (byte*)IntPtr.Zero;
    private int BytesPerPix = 0;

    public Filter(Bitmap MainBitmap)
    {
        if (MainBitmap != null)
        {
            UPicture = (Bitmap)MainBitmap.Clone();
            switch (UPicture.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    {
                        BytesPerPix = 3;
                        break;
                    }
                case PixelFormat.Format32bppArgb:
                    {
                        BytesPerPix = 4;
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Формат пикселей не соответствует стандарту");
                    }
            } 
            BmpData = UPicture.LockBits(new Rectangle(0, 0, UPicture.Width, UPicture.Height),
                      ImageLockMode.ReadWrite, UPicture.PixelFormat);
            unsafe
            {
                Begin = (byte*)BmpData.Scan0;
            }
        }
        else
            throw new ArgumentException("Неверный параметр #1");
    }

    public Bitmap Picture
    {
        get { return UPicture; }
    }
    public int Height
    {
        get { return UPicture.Height; }
    }
    public int Width
    {
        get { return UPicture.Width; }
    }
    public int BytesPerPixel
    {
        get { return BytesPerPix; }
    }
    public IntPtr Safe_IMG_Scan0
    {
        get { return BmpData.Scan0; }
    }
    public unsafe byte* Unsafe_IMG_Scan0
    {
        get { return Begin; }
    }
    public int AllPixelsBytes
    {
        get { return UPicture.Width * UPicture.Height * BytesPerPix; }
    }
    public void UnLock()
    {
        UPicture.UnlockBits(BmpData);
    }
    public unsafe RGB_Color GetPixel(int X, int Y)
    {
        RGB_Color Pixel = new RGB_Color();
        int IDX = (Y * UPicture.Width + X) * BytesPerPix; //Вычисляем позицию пикселя
        Pixel.B = *(Begin + (IDX + 0)); //B
        Pixel.G = *(Begin + (IDX + 1)); //G
        Pixel.R = *(Begin + (IDX + 2)); //R
        return Pixel;
    }
    public unsafe void SetPixel(RGB_Color CL,int X,int Y)
    {
        int IDX = (Y * UPicture.Width + X) * BytesPerPix; //Вычисляем позицию пикселя
        *(Begin + (IDX + 0)) = Convert.ToByte(CL.B); //B
        *(Begin + (IDX + 1)) = Convert.ToByte(CL.G); //G
        *(Begin + (IDX + 2)) = Convert.ToByte(CL.R); //R
    }

    public class ChangeContrast
    {
        public unsafe static void ApplyContrast(double contrast, Bitmap bmp)
        {
            
            byte[] contrast_lookup = new byte[256];
            double newValue = 0;
            double c = (100.0 + contrast) / 100.0;

            c *= c;

            for (int i = 0; i < 256; i++)
            {
                newValue = (double)i;
                newValue /= 255.0;
                newValue -= 0.5;
                newValue *= c;
                newValue += 0.5;
                newValue *= 255;

                if (newValue < 0)
                    newValue = 0;
                if (newValue > 255)
                    newValue = 255;
                contrast_lookup[i] = (byte)newValue;
            }

            var bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int PixelSize = 4;

            for (int y = 0; y < bitmapdata.Height; y++)
            {
                byte* destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
                for (int x = 0; x < bitmapdata.Width; x++)
                {
                    destPixels[x * PixelSize] = contrast_lookup[destPixels[x * PixelSize]]; // B
                    destPixels[x * PixelSize + 1] = contrast_lookup[destPixels[x * PixelSize + 1]]; // G
                    destPixels[x * PixelSize + 2] = contrast_lookup[destPixels[x * PixelSize + 2]]; // R
                    //destPixels[x * PixelSize + 3] = contrast_lookup[destPixels[x * PixelSize + 3]]; //A
                }
            }
            bmp.UnlockBits(bitmapdata);
           
        }

        public unsafe static Bitmap ProcessImage(Filter Main, int Value)
        {
            int RedVal, GreenVal, BlueVal;

            double Pixel;
            double Contrast = (100.0 + Value) / 100.0; //Вычисляем общее значение контраста

            Contrast = Contrast * Contrast;

            for (int I = 0; I < Main.AllPixelsBytes; I += Main.BytesPerPixel)
            {
                BlueVal = *(Main.Unsafe_IMG_Scan0 + (I + 0)); //B
                GreenVal = *(Main.Unsafe_IMG_Scan0 + (I + 1)); //G 
                RedVal = *(Main.Unsafe_IMG_Scan0 + (I + 2)); //R

                Pixel = RedVal / 255.0;
                Pixel = Pixel - 0.5;
                Pixel = Pixel * Contrast;
                Pixel = Pixel + 0.5;
                Pixel = Pixel * 255;
                if (Pixel < 0) Pixel = 0;
                if (Pixel > 255) Pixel = 255;
                *(Main.Unsafe_IMG_Scan0 + (I + 2)) = Convert.ToByte(Pixel);

                Pixel = GreenVal / 255.0;
                Pixel = Pixel - 0.5;
                Pixel = Pixel * Contrast;
                Pixel = Pixel + 0.5;
                Pixel = Pixel * 255;
                if (Pixel < 0) Pixel = 0;
                if (Pixel > 255) Pixel = 255;
                *(Main.Unsafe_IMG_Scan0 + (I + 1)) = Convert.ToByte(Pixel);

                Pixel = BlueVal / 255.0;
                Pixel = Pixel - 0.5;
                Pixel = Pixel * Contrast;
                Pixel = Pixel + 0.5;
                Pixel = Pixel * 255;
                if (Pixel < 0) Pixel = 0;
                if (Pixel > 255) Pixel = 255;
                *(Main.Unsafe_IMG_Scan0 + (I + 0)) = Convert.ToByte(Pixel);
            }
            Main.UnLock();
            return Main.Picture;
        }
    }
    }
}
