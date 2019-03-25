using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace ImageReadCS
{
    class Program
    {
        
        //Проверка на выход за границу изображения по X
        static int IsInRangeX(int Value_X, int Step, int Width)
        {
            int New_Value_X = 0;
            if (Value_X + Step >= Width)
                New_Value_X = Value_X;
            else if (Value_X + Step <= 0)
                New_Value_X = Value_X;
            else
                New_Value_X = Value_X + Step;

            return New_Value_X;
        }

        //Проверка на выход за границу изображения по Y
        static int IsInRangeY(int Value_Y, int Step, int Height)
        {
            int New_Value_Y = 0;
            if (Value_Y + Step >= Height)
                New_Value_Y = Value_Y;
            else if (Value_Y + Step <= 0)
                New_Value_Y = Value_Y;
            else
                New_Value_Y = Value_Y + Step;

            return New_Value_Y;
        }

        //Поиск среднего цвета для изображения
        static double AvarageColor(ColorFloatImage Image)
        {
            double My_Color = 0.0;
            for (int y = 0; y < Image.Height; y++)
                for (int x = 0; x < Image.Width; x++)
                {
                    My_Color += (0.3 * Image[x, y].r + 0.11 * Image[x, y].b + 0.59 * Image[x, y].g);
                }
            return My_Color / (Image.Height * Image.Width);
        }

        //Поиск дисперсии для изображения 
        static double Dispercia(ColorFloatImage Image, double Avarage)
        {
            double dispercia = 0.0;
            for (int y = 0; y < Image.Height; y++)
                for (int x = 0; x < Image.Width; x++)
                {
                    /* double difference = (0.3 * Image[x, y].r + 0.11 * Image[x, y].b + 0.59 * Image[x, y].g) - Avarage;
                     dispercia += Math.Pow(difference,2);*/
                    dispercia += ((Image[x, y].r - Avarage) * (Image[x, y].r - Avarage)) + ((Image[x, y].g - Avarage) * (Image[x, y].g - Avarage)) +
                        ((Image[x, y].b - Avarage) * (Image[x, y].b - Avarage));
                }

            dispercia = dispercia / (Image.Height * Image.Width);
            return dispercia;
        }

        //Поиск ковариации по X или по Y
        static double Covariance(ColorFloatImage Image, double Avarage)
        {
            double covariance = 0.0;
            for (int y = 0; y < Image.Height; y++)
                for (int x = 0; x < Image.Width; x++)
                {
                    covariance += ((Image[x, y].r - Avarage) * (Image[x, y].r - Avarage)) + ((Image[x, y].g - Avarage) * (Image[x, y].g - Avarage)) +
                        ((Image[x, y].b - Avarage) * (Image[x, y].b - Avarage));
                }

            covariance = covariance / (Image.Height * Image.Height);
            return covariance;
        }

        //Поиск ковариации по X и Y
        static double CovarianceXY(ColorFloatImage Image_1, ColorFloatImage Image_2, double Avarage_1, double Avarage_2)
        {
            double covariance = 0.0;
            for (int y = 0; y < Image_1.Height; y++)
                for (int x = 0; x < Image_1.Width; x++)
                {
                    covariance += ((Image_1[x, y].r - Avarage_1) * (Image_2[x, y].r - Avarage_2)) + ((Image_1[x, y].g - Avarage_1) * (Image_2[x, y].g - Avarage_2)) +
                        ((Image_1[x, y].b - Avarage_1) * (Image_2[x, y].b - Avarage_2));
                }

            covariance = covariance / (Image_1.Height * Image_1.Width);
            return covariance;
        }

        //Перевод изображения в серый цвет
        static GrayscaleFloatImage InGrayscale(ColorFloatImage image)
        {
            GrayscaleFloatImage New_Image = new GrayscaleFloatImage(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
                for(int x = 0; x < image.Width; x++)
                {
                    double Gray_Color = 0.299 * image[x, y].r + 0.587 * image[x, y].g + 0.114 * image[x, y].b;
                    New_Image[x, y] = (float)Gray_Color;
                }

            return New_Image;
        }

        //Фильтр Собеля
        static ColorFloatImage SobelXY(ColorFloatImage image)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {

                    int x1 = IsInRangeX(x, -1, image.Width);
                    int x4 = IsInRangeX(x, -1, image.Width);
                    int x7 = IsInRangeX(x, -1, image.Width);
                    int x3 = IsInRangeX(x, 1, image.Width);
                    int x6 = IsInRangeX(x, 1, image.Width);
                    int x9 = IsInRangeX(x, 1, image.Width);

                    int y1 = IsInRangeY(y, -1, image.Height);
                    int y4 = IsInRangeY(y, 0, image.Height);
                    int y7 = IsInRangeY(y, 1, image.Height);
                    int y3 = IsInRangeY(y, -1, image.Height);
                    int y6 = IsInRangeY(y, 0, image.Height);
                    int y9 = IsInRangeY(y, 1, image.Height);

                    ColorFloatPixel py = image[x, y];
                    ColorFloatPixel px = image[x, y];
                    ColorFloatPixel p = image[x, y];

                    py.r = (-1) * image[x1, y1].r - 2 * image[x4, y4].r - image[x7, y7].r + image[x3, y3].r + 2 * image[x6, y6].r + image[x9, y9].r;
                    py.b = (-1) * image[x1, y1].b - 2 * image[x4, y4].b - image[x7, y7].b + image[x3, y3].b + 2 * image[x6, y6].b + image[x9, y9].b;
                    py.g = (-1) * image[x1, y1].g - 2 * image[x4, y4].g - image[x7, y7].g + image[x3, y3].g + 2 * image[x6, y6].g + image[x9, y9].g;


                    x1 = IsInRangeX(x, -1, image.Width);
                    int x2 = IsInRangeX(x, 0, image.Width);
                    x3 = IsInRangeX(x, 1, image.Width);
                    x7 = IsInRangeX(x, -1, image.Width);
                    int x8 = IsInRangeX(x, 0, image.Width);
                    x9 = IsInRangeX(x, 1, image.Width);

                    y1 = IsInRangeY(y, -1, image.Height);
                    int y2 = IsInRangeY(y, -1, image.Height);
                    y3 = IsInRangeY(y, -1, image.Height);
                    y7 = IsInRangeY(y, 1, image.Height);
                    int y8 = IsInRangeY(y, 1, image.Height);
                    y9 = IsInRangeY(y, 1, image.Height);


                    px.r = (-1) * image[x1, y1].r - 2 * image[x2, y2].r - image[x3, y3].r + image[x7, y7].r + 2 * image[x8, y8].r + image[x9, y9].r;
                    px.b = (-1) * image[x1, y1].b - 2 * image[x2, y2].b - image[x3, y3].b + image[x7, y7].b + 2 * image[x8, y8].b + image[x9, y9].b;
                    px.g = (-1) * image[x1, y1].g - 2 * image[x2, y2].g - image[x3, y3].g + image[x7, y7].g + 2 * image[x8, y8].g + image[x9, y9].g;

                    p.r = (float)Math.Sqrt(Math.Pow(px.r, 2) + Math.Pow(py.r, 2));
                    p.b = (float)Math.Sqrt(Math.Pow(px.b, 2) + Math.Pow(py.b, 2));
                    p.g = (float)Math.Sqrt(Math.Pow(px.g, 2) + Math.Pow(py.g, 2));

                    p.r -= 128;
                    p.b -= 128;
                    p.g -= 128;

                    New_Image[x, y] = p;
                    //New_Image[x, y] = image[x+1, y+1] + image[x + 1, y];
                }


            return New_Image;
        }

        //Подсчёт метрики MSE
        static double MSE(ColorFloatImage Image_1, ColorFloatImage Image_2)
        {
            double mse = 0.0;
            int Height = 0, Width = 0;
            if (Image_1.Height >= Image_2.Height)
                Height = Image_1.Height;
            else
                Height = Image_2.Height;
            if (Image_1.Width >= Image_2.Width)
                Width = Image_1.Width;
            else
                Width = Image_2.Width;

            ColorFloatImage New_Image_1 = new ColorFloatImage(Width, Height);
            ColorFloatImage New_Image_2 = new ColorFloatImage(Width, Height);

            for (int y = 0; y < Height; y ++)
                for (int x = 0; x < Width; x++)
                {
                    if ((x <= Image_1.Width) && (y <= Image_1.Height))
                        New_Image_1[x, y] = Image_1[x, y];
                    if ((x <= Image_2.Width) && (y <= Image_2.Height))
                        New_Image_2[x, y] = Image_2[x, y];
                }

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    /*double Brightness_1 = ((New_Image_1[x, y].r + New_Image_1[x, y].b + New_Image_1[x, y].g) / (3));
                    double Brightness_2 = ((New_Image_2[x, y].r + New_Image_2[x, y].b + New_Image_2[x, y].g) / (3));
                    double Argument = Math.Pow(Brightness_1 - Brightness_2, 2);*/
                    /*double Bright_1 = 0.3 * New_Image_1[x, y].r + 0.59 * New_Image_1[x, y].g + 0.11 * New_Image_1[x, y].b;
                    double Bright_2 = 0.3 * New_Image_2[x, y].r + 0.59 * New_Image_2[x, y].g + 0.11 * New_Image_2[x, y].b;
                    double Argument = Math.Pow(Bright_1 - Bright_2, 2);*/
                    double Red_Difference = 0.0, Green_Difference = 0.0, Blue_Difference = 0.0;
                    Red_Difference = New_Image_1[x, y].r - New_Image_2[x, y].r;
                    Blue_Difference = New_Image_1[x, y].b - New_Image_2[x, y].b;
                    Green_Difference = New_Image_1[x, y].g - New_Image_2[x, y].g;

                    double Argument = Math.Pow((Red_Difference + Blue_Difference + Green_Difference) / (3), 2);

                    mse += Argument;
                }

            mse = mse / (Height * Width);

            return mse;
        }

        //Подсчёт метрики PSNR
        static double PSNR(ColorFloatImage Image_1, ColorFloatImage Image_2)
        {
            double psnr = 0.0;
            double mse = MSE(Image_1, Image_2);

            psnr = 10 * Math.Log10(255 * 255 / mse);

            return psnr;
        }

        //Подсчет метрики SSIM
        static double SSIM(ColorFloatImage Image_1, ColorFloatImage Image_2)
        {
            double ssim = 0.0;

            double Mat_X = AvarageColor(Image_1);
            double Mat_Y = AvarageColor(Image_2);
            double Dis_X = Dispercia(Image_1, Mat_X);
            double Dis_Y = Dispercia(Image_2, Mat_Y);
            double Cov_X = Covariance(Image_1, Mat_X);
            double Cov_Y = Covariance(Image_2, Mat_Y);
            double Cov_XY = CovarianceXY(Image_1, Image_2, Mat_X, Mat_Y);
            double C_1 = Math.Pow(0.01 * 255,2);
            double C_2 = Math.Pow(0.03 * 255,2);
            double C_3 = C_2 / 2;

            ssim = (2 * Mat_X * Mat_Y + C_1) * (2 * Cov_XY + C_2) / ((Mat_X * Mat_X + Mat_Y * Mat_Y + C_1) * (Dis_X + Dis_Y + C_2));

           // ssim = ((2 * Mat_X * Mat_Y + C_1) / (Math.Pow(Mat_X, 2) + Math.Pow(Mat_Y, 2) + C_1)) * ((2 * Cov_X * Cov_Y + C_2) / (Dis_X + Dis_Y + C_2)) *
            //    ((Cov_XY + C_3) / (Cov_X * Cov_Y + C_3));

             return ssim;
        }

        //Посчет метрики MSSIM
        static double MSSIM(ColorFloatImage Image_1, ColorFloatImage Image_2)
        {
            double mssim = 0.0;
            ColorFloatImage New_Image_1 = new ColorFloatImage(8, 8);
            ColorFloatImage New_Image_2 = new ColorFloatImage(8, 8);
            int count = 0;
            int Blocks_X = 0, Blocks_Y = 0;
            Blocks_X = Image_1.Width / 8;
            Blocks_Y = Image_1.Height / 8;

            for (int j = 0; j < Blocks_Y; j++)
                for (int i = 0; i < Blocks_X; i++)
                {
                    for (int x = 0; x < 8; x++)
                        for (int y = 0; y < 8; y++)
                        {
                            New_Image_1[x, y] = Image_1[i * 8 + x, j * 8 + y];
                            New_Image_2[x, y] = Image_2[i * 8 + x, j * 8 + y];
                        }
                    mssim += SSIM(New_Image_1, New_Image_2);
                }
            mssim = mssim / (Blocks_X * Blocks_Y);
            return mssim;
        }

       /* static GrayscaleFloatImage New_Canny(ColorFloatImage image, double sigma, double threshhold_high, double threshhold_low)
        {
            GrayscaleFloatImage New_Image = new GrayscaleFloatImage(image.Width, image.Height);
            GrayscaleFloatImage Gray_Image = new GrayscaleFloatImage(image.Width, image.Height);

            double[,] teta = new double[image.Width, image.Height];
            float intensive = 0;

            Gray_Image = InGrayscale(image);
            
            GrayscaleFloatImage Gray_Grad = GradientGaussForCanny(Gray_Image, sigma, ref teta, ref intensive);

            threshhold_high = threshhold_high * intensive;
            threshhold_low = threshhold_low * intensive;

            GrayscaleFloatImage Max_Image = new GrayscaleFloatImage(image.Width, image.Height);
            GrayscaleFloatImage Min_Image = new GrayscaleFloatImage(image.Width, image.Height);

            for (int j = 0; j < image.Height; j++)
                for (int i = 0; i < image.Width; i++)
                {
                    double angel = teta[i, j] * (180 / Math.PI);
                    if (Gray_Grad[i,j] >= threshhold_low)
                    {
                        if ((angel >= 0 && angel < 22.5) || (angel > 157.5 && angel <= 180))
                        {
                            if ((Gray_Grad[IsInRangeX(i,0,image.Width),IsInRangeY(j,0,image.Height)] > Gray_Grad[IsInRangeX(i, -1, image.Width), IsInRangeY(j, 0, image.Height)]) &&
                                    (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] > Gray_Grad[IsInRangeX(i, 1, image.Width), IsInRangeY(j, 0, image.Height)]))
                            {
                                if (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] >= threshhold_high)
                                    Max_Image[i, j] = Gray_Grad[i, j];
                                else
                                    Min_Image[i, j] = Gray_Grad[i, j];
                            }
                        }
                        else if (angel >= 22.5 && angel < 67.5)
                        {
                            if ((Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] > Gray_Grad[IsInRangeX(i, -1, image.Width), IsInRangeY(j, 1, image.Height)]) &&
                                   (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] > Gray_Grad[IsInRangeX(i, 1, image.Width), IsInRangeY(j, -1, image.Height)]))
                            {
                                if (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] >= threshhold_high)
                                    Max_Image[i, j] = Gray_Grad[i, j];
                                else
                                    Min_Image[i, j] = Gray_Grad[i, j];
                            }
                        }
                        else if (angel > 112.5 && angel <= 157.5)
                        {
                            if ((Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] > Gray_Grad[IsInRangeX(i, 1, image.Width), IsInRangeY(j, 1, image.Height)]) &&
                                  (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] > Gray_Grad[IsInRangeX(i, -1, image.Width), IsInRangeY(j, -1, image.Height)]))
                            {
                                if (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] >= threshhold_high)
                                    Max_Image[i, j] = Gray_Grad[i, j];
                                else
                                    Min_Image[i, j] = Gray_Grad[i, j];
                            }
                        }
                        else
                        {
                            if ((Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] > Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, -1, image.Height)]) &&
                                  (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] > Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 1, image.Height)]))
                            {
                                if (Gray_Grad[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 0, image.Height)] >= threshhold_high)
                                    Max_Image[i, j] = Gray_Grad[i, j];
                                else
                                    Min_Image[i, j] = Gray_Grad[i, j];
                            }
                        }
                    }
                }

            for (int j = 0; j < image.Height; j++)
                for (int i = 0; i < image.Width; i++)
                {
                    if (Min_Image[i,j] != 0)
                    {
                        if (Max_Image[IsInRangeX(i, -1, image.Width), IsInRangeY(j, -1, image.Height)] != 0 ||
                            Max_Image[IsInRangeX(i, 0, image.Width), IsInRangeY(j, -1, image.Height)] != 0 ||
                            Max_Image[IsInRangeX(i, 1, image.Width), IsInRangeY(j, -1, image.Height)] != 0 ||
                            Max_Image[IsInRangeX(i, -1, image.Width), IsInRangeY(j, 0, image.Height)] != 0 ||
                            Max_Image[IsInRangeX(i, 1, image.Width), IsInRangeY(j, 0, image.Height)] != 0 ||
                            Max_Image[IsInRangeX(i, -1, image.Width), IsInRangeY(j, 1, image.Height)] != 0 ||
                            Max_Image[IsInRangeX(i, 0, image.Width), IsInRangeY(j, 1, image.Height)] != 0 ||
                            Max_Image[IsInRangeX(i, 1, image.Width), IsInRangeY(j, 1, image.Height)] != 0)
                            Max_Image[i, j] = Min_Image[i, j];

                        if (Max_Image[i, j] != 0)
                            Max_Image[i, j] = 255;
                    }
                }

            return Max_Image;
        }*/

        //Алгоритм детектирования контуров Канни
        static GrayscaleFloatImage Canny(ColorFloatImage image, double sigma, double threshhold_high, double threshhold_low)
        {
            GrayscaleFloatImage New_Image = new GrayscaleFloatImage(image.Width, image.Height);
            GrayscaleFloatImage Gray_Image = new GrayscaleFloatImage(image.Width, image.Height);
            GrayscaleFloatImage TMP_Gray_Image = new GrayscaleFloatImage(image.Width, image.Height);
            ColorFloatImage Image_Gradient = new ColorFloatImage(image.Width, image.Height);
            GrayscaleFloatImage Gray = new GrayscaleFloatImage(image.Width, image.Height);

            double[,] teta = new double[image.Width, image.Height];
            float max_intensive = 0, new_max_intensive = 0;

            //Gray = ToGray(image);
           // TMP_Gray_Image = ToGray(image, ref max_intensive);
            Image_Gradient = GradientGaussForCanny(image, sigma, ref teta, ref max_intensive);
            //Gray_Image = GradientGaussForCanny(Gray, sigma, ref teta, ref max_intensive);
            //Image_Gradient = SobelXY(image);
            Gray_Image = ToGray(Image_Gradient, ref new_max_intensive);
            //Gray_Image = InGrayscale(Image_Gradient);


            threshhold_high = threshhold_high * max_intensive;
            threshhold_low = threshhold_low * max_intensive;

            GrayscaleFloatImage max = new GrayscaleFloatImage(image.Width, image.Height);
            GrayscaleFloatImage min = new GrayscaleFloatImage(image.Width, image.Height);

            //Подавление не максимумов
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    double angle = teta[x, y] * (180 / Math.PI);
                    if (Gray_Image[x, y] >= threshhold_low) {
                        if ((angle >= 0 && angle < 22.5) || (angle > 157.5 && angle <= 180)) //угол 0 градусов
                        {
                            if ((Gray_Image[x, y] > Gray_Image[IsInRangeX(x, -1, image.Width), y]) && (Gray_Image[x, y] > Gray_Image[IsInRangeX(x, 1, image.Width), y]) &&
                                (Gray_Image[x, y] > Gray_Image[IsInRangeX(x, -2, image.Width), y]) && (Gray_Image[x, y] > Gray_Image[IsInRangeX(x, 2, image.Width), y]))
                                if (Gray_Image[x, y] >= threshhold_high)
                                    max[x, y] = Gray_Image[x, y];
                                else
                                    min[x, y] = Gray_Image[x, y];

                        }
                        else if (angle >= 22.5 && angle < 67.5){ // угол 45 градусов
                            if ((Gray_Image[x, y] > Gray_Image[IsInRangeX(x, 1, image.Width), IsInRangeY(y, 1, image.Height)]) && 
                                    (Gray_Image[x, y] > Gray_Image[IsInRangeX(x, -1, image.Width), IsInRangeY(y, -1, image.Height)]) &&
                                    (Gray_Image[x, y] > Gray_Image[IsInRangeX(x, 2, image.Width), IsInRangeY(y, 2, image.Height)]) &&
                                    (Gray_Image[x, y] > Gray_Image[IsInRangeX(x, -2, image.Width), IsInRangeY(y, -2, image.Height)]))
                                if (Gray_Image[x, y] >= threshhold_high)
                                    max[x, y] = Gray_Image[x, y];
                                else
                                    min[x, y] = Gray_Image[x, y];
                        }
                        else if (angle >= 67.5 && angle <= 112.5) //угол 90 градусов
                        {
                            if ((Gray_Image[x, y] > Gray_Image[x, IsInRangeY(y, -1, image.Height)]) && 
                                    (Gray_Image[x, y] > Gray_Image[x, IsInRangeY(y, 1, image.Height)]) &&
                                    (Gray_Image[x, y] > Gray_Image[x, IsInRangeY(y, -2, image.Height)]) && 
                                    (Gray_Image[x, y] > Gray_Image[x, IsInRangeY(y, 2, image.Height)]))
                                if (Gray_Image[x, y] >= threshhold_high)
                                    max[x, y] = Gray_Image[x, y];
                                else
                                    min[x, y] = Gray_Image[x, y];

                        }
                        else if (angle > 112.5 && angle <= 157.5)
                        {
                            if ((Gray_Image[x, y] > Gray_Image[IsInRangeX(x, -1, image.Width), IsInRangeY(y, 1, image.Height)]) &&
                                (Gray_Image[x, y] > Gray_Image[IsInRangeY(x, 1, image.Width), IsInRangeY(y, -1, image.Height)]) &&
                                 (Gray_Image[x, y] > Gray_Image[IsInRangeX(x, -2, image.Width), IsInRangeY(y, 2, image.Height)]) &&
                                (Gray_Image[x, y] > Gray_Image[IsInRangeY(x, 2, image.Width), IsInRangeY(y, -2, image.Height)]))
                                if (Gray_Image[x, y] >= threshhold_high)
                                    max[x, y] = Gray_Image[x, y];
                                else
                                    min[x, y] = Gray_Image[x, y]; 
                        }
                  }
                }

            //Проверка минимумов
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x <image.Width; x++)
                {
                    if (min[x, y] != 0)
                        if (max[IsInRangeX(x, -1, image.Width), IsInRangeY(y, -1, image.Height)] != 0 || max[IsInRangeX(x, 0, image.Width), IsInRangeY(y, -1, image.Height)] != 0 ||
                            max[IsInRangeX(x, 1, image.Width), IsInRangeY(y, -1, image.Height)] != 0 || max[IsInRangeX(x, -1, image.Width), IsInRangeY(y, 0, image.Height)] != 0 ||
                            max[IsInRangeX(x, 1, image.Width), IsInRangeY(y, 0, image.Height)] != 0 || max[IsInRangeX(x, -1, image.Width), IsInRangeY(y, 1, image.Height)] != 0 ||
                            max[IsInRangeX(x, 0, image.Width), IsInRangeY(y, 1, image.Height)] != 0 || max[IsInRangeX(x, 1, image.Width), IsInRangeY(y, 1, image.Height)] != 0)
                            max[x, y] = min[x, y];

                    if (max[x, y] != 0)
                        max[x, y] = 255.0f;
                }


            return max;
        }

        //Перевод изображения в серый цвет  и поиск интенсивности
        static GrayscaleFloatImage ToGray(ColorFloatImage image, ref float Max_intensive)
        {
            GrayscaleFloatImage New_Image = new GrayscaleFloatImage(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x <image.Width; x++)
                {
                    New_Image[x, y] = (float)(0.3 * image[x, y].r + 0.59 * image[x, y].g + 0.11 * image[x, y].b);
                    if (Max_intensive < New_Image[x, y])
                        Max_intensive = New_Image[x, y];
                }

            return New_Image;
        }

        //Поиск градиента изображения, угла и интесивности 
        static ColorFloatImage GradientGaussForCanny(ColorFloatImage image, double sigma, ref double[,] Teta, ref float Max_intensive)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma * 3));
            Max_intensive = 0;

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    //Фильтр Гаусса с окном радиусом 3 * sigma по разным направлениям

                    ColorFloatPixel New_Pixel_X = image[x, y];
                    ColorFloatPixel New_Pixel_Y = image[x, y];
                    ColorFloatPixel[] Pixels_Y = new ColorFloatPixel[(2 * Significant_Radius + 1)];
                    ColorFloatPixel[] Pixels_X = new ColorFloatPixel[(2 * Significant_Radius + 1)];

                    int Pixels_X_i = 0, Pixels_Y_i = 0;

                    int X_M_Sigbificant = x - Significant_Radius;
                    int Y_M_Significant = y - Significant_Radius;
                    int X_P_Sigbificant = x + Significant_Radius;
                    int Y_P_Significant = y + Significant_Radius;

                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j - y, image.Height);
                        int Arg_Exp = (j - y) * (j - y);
                        double Weight = (Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) * (-2) * (j - y)) / (Math.Sqrt(Math.PI * 2) * sigma * sigma * sigma * 2);

                        Pixels_Y[Pixels_Y_i] = image[x, new_y];
                        Pixels_Y[Pixels_Y_i].r = (float)(Weight * image[x, new_y].r);
                        Pixels_Y[Pixels_Y_i].b = (float)(Weight * image[x, new_y].b);
                        Pixels_Y[Pixels_Y_i].g = (float)(Weight * image[x, new_y].g);
                        Pixels_Y_i++;
                    }

                    for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                    {
                        int new_x = IsInRangeX(x, i - x, image.Width);
                        int Arg_Exp = (i - x) * (i - x);
                        double Weight = (Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) * (-2) * (i - x)) / (Math.Sqrt(Math.PI * 2) * sigma * sigma * sigma * 2);

                        Pixels_X[Pixels_X_i] = image[new_x, y];
                        Pixels_X[Pixels_X_i].r = (float)(Weight * image[new_x, y].r);
                        Pixels_X[Pixels_X_i].b = (float)(Weight * image[new_x, y].b);
                        Pixels_X[Pixels_X_i].g = (float)(Weight * image[new_x, y].g);
                        Pixels_X_i++;
                    }

                    New_Pixel_X.r = Pixels_X[0].r;
                    New_Pixel_X.b = Pixels_X[0].b;
                    New_Pixel_X.g = Pixels_X[0].g;

                    for (int i = 1; i < Pixels_X_i; i++)
                    {
                        New_Pixel_X.r += Pixels_X[i].r;
                        New_Pixel_X.b += Pixels_X[i].b;
                        New_Pixel_X.g += Pixels_X[i].g;
                    }

                    New_Pixel_Y.r = Pixels_Y[0].r;
                    New_Pixel_Y.b = Pixels_Y[0].b;
                    New_Pixel_Y.g = Pixels_Y[0].g;

                    for (int j = 1; j < Pixels_Y_i; j++)
                    {
                        New_Pixel_Y.r += Pixels_Y[j].r;
                        New_Pixel_Y.b += Pixels_Y[j].b;
                        New_Pixel_Y.g += Pixels_Y[j].g;
                    }

                    ColorFloatPixel New_Pixel = image[x, y];
                    ColorFloatPixel New_Pixel_1 = image[x, y];
                    ColorFloatPixel New_Pixel_2 = image[x, y];

                    New_Pixel_1.r = (float)(Math.Sqrt(New_Pixel_Y.r * New_Pixel_Y.r + New_Pixel_X.r * New_Pixel_X.r) * 3);
                    New_Pixel_1.b = (float)(Math.Sqrt(New_Pixel_Y.b * New_Pixel_Y.b + New_Pixel_X.b * New_Pixel_X.b) * 3);
                    New_Pixel_1.g = (float)(Math.Sqrt(New_Pixel_Y.g * New_Pixel_Y.g + New_Pixel_X.g * New_Pixel_X.g) * 3);

                    
                    New_Image[x, y] = New_Pixel_1;


                    //Вычисление угла направления градиента
                    double New_Teta = (Math.Abs(Math.Atan2(New_Pixel_Y.r, New_Pixel_X.r)) + Math.Abs(Math.Atan2(New_Pixel_Y.g, New_Pixel_X.g)) + 
                        Math.Abs(Math.Atan2(New_Pixel_Y.b, New_Pixel_X.b))) / 3;
                    Teta[x, y] = New_Teta;

                    //  float New_Intensive = (float)(0.3 * (New_Pixel_X.r + New_Pixel_Y.r) + 0.59 * 
                    //    (New_Pixel_X.g + New_Pixel_Y.g) + 0.11 * (New_Pixel_X.b + New_Pixel_Y.b));
                    float New_Intensive = (float)((New_Image[x, y].r + New_Image[x, y].b +  New_Image[x, y].g));
                   // float New_Intensive = (float)New_Image[x, y].r;
                    //float New_Intensive = (float)(0.299 * image[x, y].r + 0.587 * image[x, y].g + 0.114 * image[x, y].b);
                    if (New_Intensive >= Max_intensive)
                        Max_intensive = New_Intensive;

                }

            return New_Image;
        }

        //Поиск градиента изображения, угла и интесивности для черно-белого изображения
        static GrayscaleFloatImage GradientGaussForCanny(GrayscaleFloatImage image, double sigma, ref double[,] Teta, ref float Max_intensive)
        {
            GrayscaleFloatImage New_Image = new GrayscaleFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma * 3));
            Max_intensive = 0;

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    //Фильтр Гаусса с окном радиусом 3 * sigma по разным направлениям

                  //  GrayscaleFloatImage New_Pixel_X = image[x, y];
                    //GrayscaleFloatImage New_Pixel_Y = image[x, y];
                    float Pixels_Y = 0, Pixels_X = 0; 

                    int Pixels_X_i = 0, Pixels_Y_i = 0;

                    int X_M_Sigbificant = x - Significant_Radius;
                    int Y_M_Significant = y - Significant_Radius;
                    int X_P_Sigbificant = x + Significant_Radius;
                    int Y_P_Significant = y + Significant_Radius;

                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j - y, image.Height);
                        int Arg_Exp = (j - y) * (j - y);
                        double Weight = (Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) * (-2) * (j - y)) / (Math.Sqrt(Math.PI * 2) * sigma * sigma * sigma * 2);

                        Pixels_Y += (float)(image[x, new_y] * Weight);
                        Pixels_Y_i++;
                    }

                    for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                    {
                        int new_x = IsInRangeX(x, i - x, image.Width);
                        int Arg_Exp = (i - x) * (i - x);
                        double Weight = (Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) * (-2) * (i - x)) / (Math.Sqrt(Math.PI * 2) * sigma * sigma * sigma * 2);

                        Pixels_X = (float)(image[new_x, y] * Weight);
                        Pixels_X_i++;
                    }
                   // Pixels_X /= Pixels_X_i;
                    //Pixels_Y /= Pixels_Y_i;

                    New_Image[x, y] = (float)Math.Sqrt(Pixels_X * Pixels_X + Pixels_Y * Pixels_Y);


                    //Вычисление угла направления градиента
                    double New_Teta = (Math.Atan2(Pixels_Y, Pixels_X));
                    Teta[x, y] = New_Teta;


                    float New_Intensive = New_Image[x, y];
                    //float New_Intensive = (float)(0.3 * New_Image[x, y].r + 0.11 * New_Image[x, y].b + 0.59 * New_Image[x, y].g);
                    //float New_Intensive = image[x, y];
                    if (New_Intensive >= Max_intensive)
                        Max_intensive = New_Intensive;

                }

            return New_Image;
        }

        //ФИльтр Габора
        static ColorFloatImage New_Gabor(ColorFloatImage image, double sigma, double gamma, int theta, double lambda, int psi)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma * 3));

            double theta_rad = theta * Math.PI / 180;
            double psi_rad = psi * Math.PI / 180;

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    ColorFloatPixel New_Pixel = image[x,y];
                    ColorFloatPixel[] Pixels = new ColorFloatPixel[(2 * Significant_Radius + 1) * (2 * Significant_Radius + 1)];

                    int Pixels_i = 0;

                    for (int j = Significant_Radius; j >= -Significant_Radius; j--)
                        for (int i = -Significant_Radius; i <= Significant_Radius; i++)
                        {
                            int new_y = IsInRangeY(y, j, image.Height);
                            int new_x = IsInRangeX(x, i, image.Width);

                             double x_theta = ( i) * (float)Math.Cos(theta_rad) + (-j) * (float)Math.Sin(theta_rad);
                             double y_theta = (-1) * (i) * (float)Math.Sin(theta_rad) + (-j) * (float)Math.Cos(theta_rad);

                           //double x_theta = (i - 1/2) * (float)Math.Cos(theta_rad) + (j + 1/2) * (float)Math.Sin(theta_rad);
                           //double y_theta = (-1) * (i - 1/2) * (float)Math.Sin(theta_rad) + (j + 1/2) * (float)Math.Cos(theta_rad);

                            double gabor = (float)(Math.Exp((-0.5F) * ((float)Math.Pow(x_theta, 2) / (float)Math.Pow(sigma, 2) + (float)Math.Pow(y_theta, 2) / (float)Math.Pow(sigma / gamma, 2))) *
                                (float)(Math.Cos(2 * Math.PI * x_theta / lambda + psi_rad)));

                            Pixels[Pixels_i] = image[new_x, new_y];
                            Pixels[Pixels_i].r = (float)(image[new_x, new_y].r * gabor);
                            Pixels[Pixels_i].g = (float)(image[new_x, new_y].g * gabor);
                            Pixels[Pixels_i].b = (float)(image[new_x, new_y].b * gabor);
                            Pixels_i++;
                        }
                    New_Pixel.r = Pixels[0].r;
                    New_Pixel.b = Pixels[0].b;
                    New_Pixel.g = Pixels[0].g;

                    for (int i = 1; i < Pixels_i; i++)
                    {
                        New_Pixel.r += Pixels[i].r;
                        New_Pixel.b += Pixels[i].b;
                        New_Pixel.g += Pixels[i].g;
                    }

                    New_Pixel.r += 128;
                    New_Pixel.b += 128;
                    New_Pixel.g += 128;

                    New_Image[x, y] = New_Pixel;
                }

            return New_Image;
        }


        //Фильтр Габора
        /*static ColorFloatImage Gabor(ColorFloatImage image, double sigma, double gamma, int theta, double lambda, int psi)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma * 3));

            double theta_rad = theta * Math.PI / 180;
            double psi_rad = psi * Math.PI / 180;


            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    //Фильтр Габора с окном радиусом 3 * sigma

                    ColorFloatPixel New_Pixel = image[x, y];
                    ColorFloatPixel[] Pixels = new ColorFloatPixel[(2 * Significant_Radius + 1) * (2 * Significant_Radius + 1)];

                    int Pixels_i = 0;

                    int X_M_Sigbificant = x - Significant_Radius;
                    int Y_M_Significant = y - Significant_Radius;
                    int X_P_Sigbificant = x + Significant_Radius;
                    int Y_P_Significant = y + Significant_Radius;
                   

                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j - y, image.Height);
                        for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                        {
                            int new_x = IsInRangeX(x, i - x, image.Width);

                            double x_theta = (i - x) * Math.Cos(theta_rad) + (j - y)  * Math.Sin(theta_rad);
                            double y_theta = (-1) * (i - x) * Math.Sin(theta_rad) + (j - y) * Math.Cos(theta_rad);

                            //double x_theta = (i - (int)((2 * Significant_Radius + 1) / 2)) * Math.Cos(theta_rad) + (j - (int)((2 * Significant_Radius + 1) / 2)) * Math.Sin(theta_rad);
                            //double y_theta = (j - (int)((2 * Significant_Radius + 1) / 2)) * Math.Cos(theta_rad) - (i - (int)((2 * Significant_Radius + 1) / 2)) * Math.Sin(theta_rad);

                            //double gabor = Math.Exp((-1) * ((x_theta * x_theta + gamma * gamma * y_theta * y_theta) / (2 * sigma * sigma))) * 
                            //  Math.Cos(2 * Math.PI * x_theta / lambda + psi_rad);

                            double gabor = Math.Exp((-1 / 2) * (Math.Pow(x_theta, 2) / Math.Pow(sigma, 2) + Math.Pow(y_theta, 2) / Math.Pow(sigma / gamma, 2))) *
                                (float)(Math.Cos(2 * Math.PI * x_theta / lambda + psi_rad));

                            /*Pixels[Pixels_i] = image[x, y];
                            Pixels[Pixels_i].r = (float)(image[x, y].r * gabor);
                            Pixels[Pixels_i].g = (float)(image[x, y].g * gabor);
                            Pixels[Pixels_i].b = (float)(image[x, y].b * gabor);
                            Pixels_i++;

                            Pixels[Pixels_i] = image[new_x, new_y];
                            Pixels[Pixels_i].r = (float)(image[new_x, new_y].r * gabor);
                            Pixels[Pixels_i].g = (float)(image[new_x, new_y].g * gabor);
                            Pixels[Pixels_i].b = (float)(image[new_x, new_y].b * gabor);
                            Pixels_i++;
                        }
                    }

                    New_Pixel.r = Pixels[0].r;
                    New_Pixel.b = Pixels[0].b;
                    New_Pixel.g = Pixels[0].g;

                    for (int i = 1; i < Pixels_i; i++)
                    {
                        New_Pixel.r += Pixels[i].r;
                        New_Pixel.b += Pixels[i].b;
                        New_Pixel.g += Pixels[i].g;
                    }

                    //New_Pixel.r += 128;
                    //New_Pixel.b += 128;
                    //New_Pixel.g += 128;

                    //New_Pixel.r /= (2 * Significant_Radius + 1) * (2 * Significant_Radius + 1);
                    //New_Pixel.b /= (2 * Significant_Radius + 1) * (2 * Significant_Radius + 1);
                    //New_Pixel.g /= (2 * Significant_Radius + 1) * (2 * Significant_Radius + 1);

                    New_Image[x, y] = New_Pixel;

                }

            return New_Image;
        }*/

        static void Main(string[] args)
        {
            if ((args.Length == 3) && ((args[0] == "mse") || (args[0] == "MSE") || (args[0] == "Mse"))) {
                string InputFileName_1 = args[1], InputFileName_2 = args[2];
                if (!File.Exists(InputFileName_1))
                    return;
                if (!File.Exists(InputFileName_2))
                    return;

                ColorFloatImage Image_1 = ImageIO.FileToColorFloatImage(InputFileName_1);
                ColorFloatImage Image_2 = ImageIO.FileToColorFloatImage(InputFileName_2);

                double answer = MSE(Image_1, Image_2);

                Console.Write("MSE metric =  ");
                Console.WriteLine("{0}", answer);
                
            }
            if ((args.Length == 3) && ((args[0] == "Psnr") || (args[0] == "PSNR") || (args[0] == "psnr")))
            {
                string InputFileName_1 = args[1], InputFileName_2 = args[2];
                if (!File.Exists(InputFileName_1))
                    return;
                if (!File.Exists(InputFileName_2))
                    return;

                ColorFloatImage Image_1 = ImageIO.FileToColorFloatImage(InputFileName_1);
                ColorFloatImage Image_2 = ImageIO.FileToColorFloatImage(InputFileName_2);

                double answer = PSNR(Image_1, Image_2);

                Console.Write("PSNR metric =  ");
                Console.WriteLine("{0}", answer);
            }
            if ((args.Length == 3) && ((args[0] == "Ssim") || (args[0] == "SSIM") || (args[0] == "ssim")))
            {
                string InputFileName_1 = args[1], InputFileName_2 = args[2];
                if (!File.Exists(InputFileName_1))
                    return;
                if (!File.Exists(InputFileName_2))
                    return;

                ColorFloatImage Image_1 = ImageIO.FileToColorFloatImage(InputFileName_1);
                ColorFloatImage Image_2 = ImageIO.FileToColorFloatImage(InputFileName_2);

                double answer = SSIM(Image_1, Image_2);

                Console.Write("SSIM metric =  ");
                Console.WriteLine("{0}", answer);
            }
            if ((args.Length == 3) && ((args[0] == "Mssim") || (args[0] == "MSSIM") || (args[0] == "mssim")))
            {
                string InputFileName_1 = args[1], InputFileName_2 = args[2];
                if (!File.Exists(InputFileName_1))
                    return;
                if (!File.Exists(InputFileName_2))
                    return;

                ColorFloatImage Image_1 = ImageIO.FileToColorFloatImage(InputFileName_1);
                ColorFloatImage Image_2 = ImageIO.FileToColorFloatImage(InputFileName_2);

                double answer = MSSIM(Image_1, Image_2);

                Console.Write("MSSIM metric =  ");
                Console.WriteLine("{0}",answer);
            }
            if ((args.Length == 6) && ((args[0] == "Canny") || (args[0] == "CANNY") || (args[0] == "canny")))
            {
                string InputFileName = args[4], OutputFileName= args[5];
                if (!File.Exists(InputFileName))
                    return;

                double sigma = double.Parse(args[1], CultureInfo.InvariantCulture);
                double thr_high = double.Parse(args[2], CultureInfo.InvariantCulture);
                double thr_low = double.Parse(args[3], CultureInfo.InvariantCulture);

                ColorFloatImage Image = ImageIO.FileToColorFloatImage(InputFileName);
                GrayscaleFloatImage New_Image;

                New_Image = Canny(Image, sigma, thr_high, thr_low);

                ImageIO.ImageToFile(New_Image, OutputFileName);
            }
            if ((args.Length == 8) && ((args[0] == "Gabor") || (args[0] == "GABOR") || (args[0] == "gabor")))
            {
                string InputFileName = args[6], OutputFileName = args[7];
                if (!File.Exists(InputFileName))
                    return;

                double sigma = double.Parse(args[1], CultureInfo.InvariantCulture);
                double gamma = double.Parse(args[2], CultureInfo.InvariantCulture);
                int theta = int.Parse(args[3], CultureInfo.InvariantCulture);
                double lambda = double.Parse(args[4], CultureInfo.InvariantCulture);
                int psi = int.Parse(args[5], CultureInfo.InvariantCulture);

                ColorFloatImage Image = ImageIO.FileToColorFloatImage(InputFileName);
                ColorFloatImage New_Image, New_Image_1;
               
                //New_Image = SobelXY(Image);
                New_Image_1 = New_Gabor(Image, sigma, gamma, theta, lambda, psi);

                GrayscaleFloatImage Gray = new GrayscaleFloatImage(New_Image_1.Width, New_Image_1.Height);
                //double max_intensive = 0;
                Gray = InGrayscale(New_Image_1);

                ImageIO.ImageToFile(Gray, OutputFileName);
            }
        }
    }
}
