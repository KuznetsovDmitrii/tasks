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
        static void FlipImage(GrayscaleFloatImage image)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width / 2; x++)
                {
                    float p = image[x, y];
                    image[x, y] = image[image.Width - 1 - x, y];
                    image[image.Width - 1 - x, y] = p;
                }
        }


        //Горизонтальное отражение изображения
        static void MirrorImageX(ColorFloatImage image)
        {
            ColorFloatImage tmpimage = new ColorFloatImage(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width / 2; x++)
                {
                    //float p = image[x, y];
                    //float b = image[x, y].b;
                    //float g = image[x, y].g;
                    //float r = image[x, y].r;
                    tmpimage[x,y] = image[x, y];
                    image[x, y] = image[image.Width - 1 - x, y];
                    image[image.Width - 1 - x, y] = tmpimage[x,y];
                   
                }
        }

       //Вертикальное отражение изображения
        static void MirrorImageY(ColorFloatImage image)
        {
            ColorFloatImage tmpimage = new ColorFloatImage(image.Width, image.Height);
            for (int y = 0; y < image.Height / 2; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    tmpimage[x, y] = image[x, y];
                    image[x, y] = image[x,image.Height -1 - y];
                    image[x, image.Height - 1 - y] = tmpimage[x, y];

                }
        }

        //Поворот изображения по часовой стрелке на 90 градусов или 270 против часовой
        static ColorFloatImage RotateImageCW90(ColorFloatImage image)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Height, image.Width);
            for (int y = 0; y < image.Width; y++)
                for (int x = 0; x < image.Height; x++)
                {
                    int new_x, new_y;
                    new_x = y;
                    new_y = image.Height - 1 - x;
                    New_Image[x, y] = image[new_x, new_y];
                }
            return New_Image;
        }

        //Поворот изображения против часовой стрелке на 90 градусов или 270 по часовой
        static ColorFloatImage RotateImageCCW90(ColorFloatImage image)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Height, image.Width);
            for (int y = 0; y < image.Width; y++)
                for (int x = 0; x < image.Height; x++)
                {
                    int new_x, new_y;
                    new_x = image.Width - 1 - y;
                    new_y = x;
                    New_Image[x, y] = image[new_x, new_y];
                }
            return New_Image;
        }

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

        //Фильтр Собеля по производной по Х
        static ColorFloatImage SobelX (ColorFloatImage image)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {

                    int x1 = IsInRangeX(x, -1, image.Width);
                    int x2 = IsInRangeX(x, 0, image.Width);
                    int x3 = IsInRangeX(x, 1, image.Width);
                    int x7 = IsInRangeX(x, -1, image.Width);
                    int x8 = IsInRangeX(x, 0, image.Width);
                    int x9 = IsInRangeX(x, 1, image.Width);

                    int y1 = IsInRangeY(y, -1, image.Height);
                    int y2 = IsInRangeY(y, -1, image.Height);
                    int y3 = IsInRangeY(y, -1, image.Height);
                    int y7 = IsInRangeY(y, 1, image.Height);
                    int y8 = IsInRangeY(y, 1, image.Height);
                    int y9 = IsInRangeY(y, 1, image.Height);

                    ColorFloatPixel p = image[x, y];

                    p.r = (-1) * image[x1, y1].r - 2 * image[x2, y2].r - image[x3, y3].r + image[x7, y7].r + 2 * image[x8, y8].r + image[x9, y9].r;
                    p.b = (-1) * image[x1, y1].b - 2 * image[x2, y2].b - image[x3, y3].b + image[x7, y7].b + 2 * image[x8, y8].b + image[x9, y9].b;
                    p.g = (-1) * image[x1, y1].g - 2 * image[x2, y2].g - image[x3, y3].g + image[x7, y7].g + 2 * image[x8, y8].g + image[x9, y9].g;

                    p.r += 128;
                    p.b += 128;
                    p.g += 128;

                    New_Image[x, y] = p;
                    //New_Image[x, y] = image[x+1, y+1] + image[x + 1, y];
                }


            return New_Image;
        }

        //Фильтр Собеля по производной по Y
        static ColorFloatImage SobelY(ColorFloatImage image)
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

                    ColorFloatPixel p = image[x, y];

                    p.r = (-1) * image[x1, y1].r - 2 * image[x4, y4].r - image[x7, y7].r + image[x3, y3].r + 2 * image[x6, y6].r + image[x9, y9].r;
                    p.b = (-1) * image[x1, y1].b - 2 * image[x4, y4].b - image[x7, y7].b + image[x3, y3].b + 2 * image[x6, y6].b + image[x9, y9].b;
                    p.g = (-1) * image[x1, y1].g - 2 * image[x4, y4].g - image[x7, y7].g + image[x3, y3].g + 2 * image[x6, y6].g + image[x9, y9].g;

                    p.r += 128;
                    p.b += 128;
                    p.g += 128;

                    New_Image[x, y] = p;
                    //New_Image[x, y] = image[x+1, y+1] + image[x + 1, y];
                }


            return New_Image;
        }

        //Фильтр Собеля по производной по XY
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

                    //p.r += 128;
                    //p.b += 128;
                    //p.g += 128;

                    New_Image[x, y] = p;
                    //New_Image[x, y] = image[x+1, y+1] + image[x + 1, y];
                }


            return New_Image;
        }

        //Сортировка пузырьком для цветов пикселя
        static void BubbleSortBlue(ColorFloatPixel[] Array)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                for (int j = 0; j < Array.Length - i - 1; j++)
                {
                    if (Array[j].b > Array[j + 1].b)
                    {
                        float temp = Array[j].b;
                        Array[j].b = Array[j + 1].b;
                        Array[j + 1].b = temp;
                    }
                }
            }
        }

        static void BubbleSortGreen(ColorFloatPixel[] Array)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                for (int j = 0; j < Array.Length - i - 1; j++)
                {
                    if (Array[j].g > Array[j + 1].g)
                    {
                        float temp = Array[j].g;
                        Array[j].g = Array[j + 1].g;
                        Array[j + 1].g = temp;
                    }
                }
            }
        }

        static void BubbleSortRed(ColorFloatPixel[] Array)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                for (int j = 0; j < Array.Length - i - 1; j++)
                {
                    if (Array[j].r > Array[j + 1].r)
                    {
                        float temp = Array[j].r;
                        Array[j].r = Array[j + 1].r;
                        Array[j + 1].r = temp;
                    }
                }
            }
        }

        //Медианная фильтрация cо стороной квадрата 2 * rad + 1
        static ColorFloatImage MedianFilter(ColorFloatImage image, int rad)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);

            for (int y = 0 ; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    //Вычисляем значение каждого пикселя в окне
                    ColorFloatPixel[] pi = new ColorFloatPixel[(2 * rad + 1) * (2 * rad + 1)];
                    int Pixel_i = 0;
                    for (int j = rad; j >= (-1) * rad; j--)
                    {
                        int new_y = IsInRangeY(y, j, image.Height);
                        for (int i = (-1) * rad; i <= rad; i++)
                        {
                            int new_x = IsInRangeX(x, i, image.Width);

                            pi[Pixel_i] = image[new_x, new_y];
                            pi[Pixel_i].r = image[new_x, new_y].r;
                            pi[Pixel_i].g = image[new_x, new_y].g;
                            pi[Pixel_i].b = image[new_x, new_y].b;

                            Pixel_i++;
                        }
                    }
                    //Применяем фильтр к исходному изображению
                    BubbleSortRed(pi);
                    BubbleSortGreen(pi);
                    BubbleSortBlue(pi);
                    int number_pixel = Convert.ToInt32(Math.Round(((Math.Pow((2 * rad + 1), 2) - 1) / 2)) + 1);
                    ColorFloatPixel New_Pixel = image[x,y];
                    New_Pixel.r = pi[number_pixel].r;
                    New_Pixel.g = pi[number_pixel].g;
                    New_Pixel.b = pi[number_pixel].b;
                    New_Image[x, y] = New_Pixel;
                }
            return New_Image;
        }

        //Свёртка с фильтром Гаусса с произвольным выбором параметра — радиуса σ с гамма-коррекцией
        static ColorFloatImage GaussGamma(ColorFloatImage image, double sigma, double gamma)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma * 3));

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    //Фильтр Гаусса с окном радиусом 3 * sigma
                    //var val = 0; var wsum = 0;

                    ColorFloatPixel New_Pixel = image[x, y];
                    ColorFloatPixel[] pi = new ColorFloatPixel[(2 * Significant_Radius + 1) * (2 * Significant_Radius + 1)];
                    int Pixel_i = 0;

                    int X_M_Sigbificant = x - Significant_Radius;
                    int Y_M_Significant = y - Significant_Radius;
                    int X_P_Sigbificant = x + Significant_Radius;
                    int Y_P_Significant = y + Significant_Radius;

                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j - y, image.Height);
                        for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                        {

                            //int new_y = IsInRangeY(j, 0, image.Height);
                            int new_x = IsInRangeX(x, i - x, image.Width);

                            int Arg_Exp = (i - x) * (i - x) + (j - y) * (j - y);
                            double Weight = Math.Exp(( (-1) * Arg_Exp) / (2 * sigma * sigma)) / (Math.PI * 2 * sigma * sigma);

                            //Гамма преобразование
                            ColorFloatPixel Old_Pixel = image[new_x, new_y];

                            float New_Red_1 = (float)(Math.Pow(Old_Pixel.r / 255, 1 / gamma) * 255);
                            float New_Green_1 = (float)(Math.Pow(Old_Pixel.g / 255, 1 / gamma) * 255);
                            float New_Blue_1 = (float)(Math.Pow(Old_Pixel.b / 255, 1 / gamma) * 255);

                            Old_Pixel.r = New_Red_1;
                            Old_Pixel.g = New_Green_1;
                            Old_Pixel.b = New_Blue_1;

                            pi[Pixel_i] = Old_Pixel;
                            pi[Pixel_i].r = (float)(Weight * Old_Pixel.r);
                            pi[Pixel_i].b = (float)(Weight * Old_Pixel.b);
                            pi[Pixel_i].g = (float)(Weight * Old_Pixel.g);
                            Pixel_i++;

                            //New_Pixel.r += (float)(Weight * image[new_x, new_y].r);
                            //New_Pixel.g += (float)(Weight * image[new_x, new_y].g);
                            //New_Pixel.b += (float)(Weight * image[new_x, new_y].b);
                            //sum ++;

                        }
                    }
                    //Применение фильтра Гаусса к одному пикселю
                    New_Pixel.r = pi[0].r;
                    New_Pixel.b = pi[0].b;
                    New_Pixel.g = pi[0].g;
                    for (int i = 1; i < Pixel_i; i++)
                    {
                        New_Pixel.r += pi[i].r;
                        New_Pixel.b += pi[i].b;
                        New_Pixel.g += pi[i].g;
                    }

                    //Обратная Гамма коррекция для полученного пикселя
                    float New_Red = (float)(Math.Pow(New_Pixel.r / 255,  gamma) * 255);
                    float New_Green = (float)(Math.Pow(New_Pixel.g / 255,  gamma) * 255);
                    float New_Blue = (float)(Math.Pow(New_Pixel.b / 255,  gamma) * 255);

                    New_Pixel.r = New_Red;
                    New_Pixel.g = New_Green;
                    New_Pixel.b = New_Blue;

                    New_Image[x, y] = New_Pixel;
                }
            return New_Image;
        }

        //Вычисление модуля градиента как корень из суммы квадратов свёрток с первой производной фильтра Гаусса по горизонтали и вертикали
       /* static ColorFloatImage GradientGaussNew(ColorFloatImage image, double sigma)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma * 3));

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    //Фильтр Гаусса с окном радиусом 3 * sigma по разным направлениям

                    ColorFloatPixel New_Pixel_X_1 = image[x, y];
                    ColorFloatPixel New_Pixel_Y_1 = image[x, y];
                    ColorFloatPixel New_Pixel_X_2 = image[x, y];
                    ColorFloatPixel New_Pixel_Y_2 = image[x, y];
                    ColorFloatPixel[] Pixels_Y_1 = new ColorFloatPixel[(2 * Significant_Radius + 1)];
                    ColorFloatPixel[] Pixels_X_1 = new ColorFloatPixel[(2 * Significant_Radius + 1)];
                    ColorFloatPixel[] Pixels_Y_2 = new ColorFloatPixel[(2 * Significant_Radius + 1)];
                    ColorFloatPixel[] Pixels_X_2 = new ColorFloatPixel[(2 * Significant_Radius + 1)];

                    int Pixels_X_1_i = 0, Pixels_Y_1_i = 0, Pixels_X_2_i = 0, Pixels_Y_2_i = 0;

                    int X_M_Sigbificant = x - Significant_Radius;
                    int Y_M_Significant = y - Significant_Radius;
                    int X_P_Sigbificant = x + Significant_Radius;
                    int Y_P_Significant = y + Significant_Radius;

                    
                    //Производная по Y
                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j - y, image.Height);
                        int Arg_Exp = (j - y) * (j - y);
                        double Weight = (Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) * (-2) * (j - y)) / (Math.Sqrt(Math.PI * 2) * sigma* sigma * sigma * 2);
                        
                        Pixels_Y_1[Pixels_Y_1_i] = image[x, new_y];
                        Pixels_Y_1[Pixels_Y_1_i].r = (float)(Weight * image[x, new_y].r);
                        Pixels_Y_1[Pixels_Y_1_i].b = (float)(Weight * image[x, new_y].b);
                        Pixels_Y_1[Pixels_Y_1_i].g = (float)(Weight * image[x, new_y].g);
                        Pixels_Y_1_i++;
                    }

                    for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                    {
                        int new_x = IsInRangeX(x, i - x, image.Width);
                        int Arg_Exp = (i - x) * (i - x);
                        double Weight = Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) / (Math.Sqrt(Math.PI * 2) * sigma);

                        Pixels_X_1[Pixels_X_1_i] = image[new_x, y];
                        Pixels_X_1[Pixels_X_1_i].r = (float)(Weight * image[new_x, y].r);
                        Pixels_X_1[Pixels_X_1_i].b = (float)(Weight * image[new_x, y].b);
                        Pixels_X_1[Pixels_X_1_i].g = (float)(Weight * image[new_x, y].g);
                        Pixels_X_1_i++;
                    }

                    New_Pixel_X_1.r = Pixels_X_1[0].r;
                    New_Pixel_X_1.b = Pixels_X_1[0].b;
                    New_Pixel_X_1.g = Pixels_X_1[0].g;

                    for (int i = 1; i < Pixels_X_1_i; i++)
                    {
                        New_Pixel_X_1.r += Pixels_X_1[i].r;
                        New_Pixel_X_1.b += Pixels_X_1[i].b;
                        New_Pixel_X_1.g += Pixels_X_1[i].g;
                    }

                    New_Pixel_Y_1.r = Pixels_Y_1[0].r;
                    New_Pixel_Y_1.b = Pixels_Y_1[0].b;
                    New_Pixel_Y_1.g = Pixels_Y_1[0].g;

                    for (int j = 1; j < Pixels_Y_1_i; j++)
                    {
                        New_Pixel_Y_1.r += Pixels_Y_1[j].r;
                        New_Pixel_Y_1.b += Pixels_Y_1[j].b;
                        New_Pixel_Y_1.g += Pixels_Y_1[j].g;
                    }

                    ColorFloatPixel New_Pixel = image[x, y];
                    ColorFloatPixel New_Pixel_1 = image[x, y];
                    ColorFloatPixel New_Pixel_2 = image[x, y];

                    New_Pixel_1.r = New_Pixel_X_1.r + New_Pixel_Y_1.r;
                    New_Pixel_1.b = New_Pixel_X_1.b + New_Pixel_Y_1.b;
                    New_Pixel_1.g = New_Pixel_X_1.g + New_Pixel_Y_1.g;


                    /*New_Pixel_1.r = (float)(Math.Sqrt(New_Pixel_Y_1.r * New_Pixel_Y_1.r + New_Pixel_X_1.r * New_Pixel_X_1.r));
                    New_Pixel_1.b = (float)(Math.Sqrt(New_Pixel_Y_1.b * New_Pixel_Y_1.b + New_Pixel_X_1.b * New_Pixel_X_1.b));
                    New_Pixel_1.g = (float)(Math.Sqrt(New_Pixel_Y_1.g * New_Pixel_Y_1.g + New_Pixel_X_1.g * New_Pixel_X_1.g));


                    //Производная по Х
                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j - y, image.Height);
                        int Arg_Exp = (j - y) * (j - y);
                        double Weight = Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) / (Math.Sqrt(Math.PI * 2) * sigma);

                        Pixels_Y_2[Pixels_Y_2_i] = image[x, new_y];
                        Pixels_Y_2[Pixels_Y_2_i].r = (float)(Weight * image[x, new_y].r);
                        Pixels_Y_2[Pixels_Y_2_i].b = (float)(Weight * image[x, new_y].b);
                        Pixels_Y_2[Pixels_Y_2_i].g = (float)(Weight * image[x, new_y].g);
                        Pixels_Y_2_i++;
                    }

                    for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                    {
                        int new_x = IsInRangeX(x, i - x, image.Width);
                        int Arg_Exp = (i - x) * (i - x);
                        double Weight = (Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) * (-2) * (i - x)) / (Math.Sqrt(Math.PI * 2) * sigma * sigma * sigma * 2);

                        Pixels_X_2[Pixels_X_2_i] = image[new_x, y];
                        Pixels_X_2[Pixels_X_2_i].r = (float)(Weight * image[new_x, y].r);
                        Pixels_X_2[Pixels_X_2_i].b = (float)(Weight * image[new_x, y].b);
                        Pixels_X_2[Pixels_X_2_i].g = (float)(Weight * image[new_x, y].g);
                        Pixels_X_2_i++;
                    }

                    New_Pixel_X_2.r = Pixels_X_2[0].r;
                    New_Pixel_X_2.b = Pixels_X_2[0].b;
                    New_Pixel_X_2.g = Pixels_X_2[0].g;

                    for (int i = 1; i < Pixels_X_2_i; i++)
                    {
                        New_Pixel_X_2.r += Pixels_X_2[i].r;
                        New_Pixel_X_2.b += Pixels_X_2[i].b;
                        New_Pixel_X_2.g += Pixels_X_2[i].g;
                    }

                    New_Pixel_Y_2.r = Pixels_Y_2[0].r;
                    New_Pixel_Y_2.b = Pixels_Y_2[0].b;
                    New_Pixel_Y_2.g = Pixels_Y_2[0].g;

                    for (int j = 1; j < Pixels_Y_2_i; j++)
                    {
                        New_Pixel_Y_2.r += Pixels_Y_2[j].r;
                        New_Pixel_Y_2.b += Pixels_Y_2[j].b;
                        New_Pixel_Y_2.g += Pixels_Y_2[j].g;
                    }

                    /*New_Pixel_2.r = (float)(Math.Sqrt(New_Pixel_Y_2.r * New_Pixel_Y_2.r + New_Pixel_X_2.r * New_Pixel_X_2.r));
                    New_Pixel_2.b = (float)(Math.Sqrt(New_Pixel_Y_2.b * New_Pixel_Y_2.b + New_Pixel_X_2.b * New_Pixel_X_2.b));
                    New_Pixel_2.g = (float)(Math.Sqrt(New_Pixel_Y_2.g * New_Pixel_Y_2.g + New_Pixel_X_2.g * New_Pixel_X_2.g));

                    New_Pixel_2.r = New_Pixel_X_2.r + New_Pixel_Y_2.r;
                    New_Pixel_2.b = New_Pixel_X_2.b + New_Pixel_Y_2.b;
                    New_Pixel_2.g = New_Pixel_X_2.g + New_Pixel_Y_2.g;

                    New_Pixel.r = (float)(Math.Sqrt(New_Pixel_2.r * New_Pixel_2.r + New_Pixel_1.r * New_Pixel_1.r));
                    New_Pixel.b = (float)(Math.Sqrt(New_Pixel_2.b * New_Pixel_2.b + New_Pixel_1.b * New_Pixel_1.b));
                    New_Pixel.g = (float)(Math.Sqrt(New_Pixel_2.g * New_Pixel_2.g + New_Pixel_1.g * New_Pixel_1.g));

                    New_Image[x, y] = New_Pixel;
                }

            return New_Image;
        }
    */

        //Вычисление модуля градиента как корень из суммы квадратов свёрток с первой производной фильтра Гаусса по горизонтали и вертикали
        static ColorFloatImage GradientGauss(ColorFloatImage image, double sigma)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma * 3));

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
                        double Weight = (Math.Exp(((-1) * Arg_Exp) / (2 * sigma * sigma)) * (-2) * (j - y))/ (Math.Sqrt(Math.PI * 2) * sigma * sigma * sigma * 2);

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

                    New_Pixel_1.r = (float)(Math.Sqrt(New_Pixel_Y.r * New_Pixel_Y.r + New_Pixel_X.r * New_Pixel_X.r));
                    New_Pixel_1.b = (float)(Math.Sqrt(New_Pixel_Y.b * New_Pixel_Y.b + New_Pixel_X.b * New_Pixel_X.b));
                    New_Pixel_1.g = (float)(Math.Sqrt(New_Pixel_Y.g * New_Pixel_Y.g + New_Pixel_X.g * New_Pixel_X.g));

                   
                    New_Image[x, y] = New_Pixel_1;
                }

            return New_Image;
        }

        //Поворот изображения на любой угол по часовой
        static ColorFloatImage RotateAnyAngelCW(ColorFloatImage image, int angel)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            if (image.Width >= image.Height)
                New_Image = new ColorFloatImage(image.Width, image.Width);
            else
                New_Image = new ColorFloatImage(image.Height, image.Height);


            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    ColorFloatPixel New_Pixel = image[x, y];
                    New_Pixel.r = 0;
                    New_Pixel.b = 0;
                    New_Pixel.g = 0;
                    New_Image[x, y] = New_Pixel;
                }

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    int new_x = Convert.ToInt32(Math.Ceiling(x * Math.Cos(angel) - y * Math.Sin(angel)));
                    int new_y = Convert.ToInt32(Math.Ceiling(x * Math.Sin(angel) + y * Math.Cos(angel)));

                    ColorFloatPixel New_Pixel = image[x, y];
                    New_Image[new_x, new_y] = New_Pixel;
                }

            int New_Height = New_Image.Height;
            int New_Width = New_Image.Width;

            for (int y = 0; y < New_Height; y++)
                for (int x = 0; x < New_Width; x++)
                {
                    int x1 = IsInRangeX(x, -1, New_Width);
                    int x2 = IsInRangeX(x, 1, New_Width);
                    int x3 = IsInRangeX(x, -1, New_Width);
                    int x4 = IsInRangeX(x, 1, New_Width);

                    int y1 = IsInRangeY(y, -1, New_Height);
                    int y2 = IsInRangeY(y, -1, New_Height);
                    int y3 = IsInRangeY(y, 1, New_Height);
                    int y4 = IsInRangeY(y, 1, New_Height);

                    int y5 = IsInRangeY(y, -1, New_Height);
                    int y6 = IsInRangeY(y, 1, New_Height);

                    ColorFloatPixel Pixel_1 = image[x1, y1];
                    ColorFloatPixel Pixel_2 = image[x2, y2];
                    ColorFloatPixel Pixel_3 = image[x3, y3];
                    ColorFloatPixel Pixel_4 = image[x4, y4];
                    ColorFloatPixel Pixel_5 = image[x, y5];
                    ColorFloatPixel Pixel_6 = image[x, y6];

                    ColorFloatPixel New_Pixel = image[x, y];

                    float Red_X_1 = (x4 - x) / (x4 - x3) * Pixel_3.r + (x - x3) / (x4 - x3) * Pixel_4.r;
                    float Red_X_2 = (x4 - x) / (x4 - x3) * Pixel_1.r + (x - x3) / (x4 - x3) * Pixel_2.r;

                    //float Red = 

                    New_Image[x, y] = New_Pixel;
                }

            return New_Image;
        }

        //Билатериальная фильтрация
        static ColorFloatImage Bilateral(ColorFloatImage image, double sigma_d, double sigma_r)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma_d * 3));

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    //Билатериальная фильтрация с окном радиусом 3 * sigma_d

                    ColorFloatPixel New_Pixel = image[x, y];
                    ColorFloatPixel[] pi = new ColorFloatPixel[(2 * Significant_Radius + 1) * (2 * Significant_Radius + 1)];
                    int Pixel_i = 0;

                    int X_M_Sigbificant = x - Significant_Radius;
                    int Y_M_Significant = y - Significant_Radius;
                    int X_P_Sigbificant = x + Significant_Radius;
                    int Y_P_Significant = y + Significant_Radius;

                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j, image.Height);
                        for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                        {

                            //int new_y = IsInRangeY(j, 0, image.Height);
                            int new_x = IsInRangeX(x, i, image.Width);

                            ColorFloatPixel Old_Pixel = image[x,y];
                            ColorFloatPixel New_Pixel = image[x, y];

                            int Arg_Exp = (i - x) * (i - x) + (j - y) * (j - y);
                            double Weight_r = Math.Exp(((-1) * Arg_Exp) / (2 * sigma_d * sigma_d) - ());

                            pi[Pixel_i] = Old_Pixel;
                            pi[Pixel_i].r = (float)(Weight_r * Old_Pixel.r);
                            pi[Pixel_i].b = (float)(Weight * Old_Pixel.b);
                            pi[Pixel_i].g = (float)(Weight * Old_Pixel.g);
                            Pixel_i++;

                            //New_Pixel.r += (float)(Weight * image[new_x, new_y].r);
                            //New_Pixel.g += (float)(Weight * image[new_x, new_y].g);
                            //New_Pixel.b += (float)(Weight * image[new_x, new_y].b);
                            //sum ++;

                        }
                    }
                    //Применение фильтра Гаусса к одному пикселю
                    New_Pixel.r = pi[0].r;
                    New_Pixel.b = pi[0].b;
                    New_Pixel.g = pi[0].g;
                    for (int i = 1; i < Pixel_i; i++)
                    {
                        New_Pixel.r += pi[i].r;
                        New_Pixel.b += pi[i].b;
                        New_Pixel.g += pi[i].g;
                    }

                    //Обратная Гамма коррекция для полученного пикселя
                    float New_Red = (float)(Math.Pow(New_Pixel.r / 255, gamma) * 255);
                    float New_Green = (float)(Math.Pow(New_Pixel.g / 255, gamma) * 255);
                    float New_Blue = (float)(Math.Pow(New_Pixel.b / 255, gamma) * 255);

                    New_Pixel.r = New_Red;
                    New_Pixel.g = New_Green;
                    New_Pixel.b = New_Blue;

                    New_Image[x, y] = New_Pixel;
                }
            return New_Image;
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
                return;
            if ((args.Length == 4) && (args[0] == "mirror"))
            {
                if (args[1] == "x")
                {
                    string InputFileName = args[2], OutputFileName = args[3];
                    if (!File.Exists(InputFileName))
                        return;

                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);

                    MirrorImageX(image);

                    ImageIO.ImageToFile(image, OutputFileName);
                }
                else if (args[1] == "y")
                {
                    string InputFileName = args[2], OutputFileName = args[3];
                    if (!File.Exists(InputFileName))
                        return;

                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);

                    MirrorImageY(image);

                    ImageIO.ImageToFile(image, OutputFileName);
                }
                else
                {
                    return;
                }
            }
            if ((args.Length == 5) && (args[0] == "rotate"))
            {
                if (args[1] == "cw")
                {
                    if (args[2] == "90")
                    {
                        string InputFileName = args[3], OutputFileName = args[4];
                        if (!File.Exists(InputFileName))
                            return;

                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ColorFloatImage New_Image;

                        New_Image = RotateImageCW90(image);

                        ImageIO.ImageToFile(New_Image, OutputFileName);
                    }
                    if (args[2] == "180")
                    {
                        string InputFileName = args[3], OutputFileName = args[4];
                        if (!File.Exists(InputFileName))
                            return;

                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ColorFloatImage New_Image, New_Image2;

                        New_Image = RotateImageCW90(image);
                        New_Image2 = RotateImageCW90(New_Image);

                        ImageIO.ImageToFile(New_Image2, OutputFileName);
                    }
                    if (args[2] == "270")
                    {
                        string InputFileName = args[3], OutputFileName = args[4];
                        if (!File.Exists(InputFileName))
                            return;

                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ColorFloatImage New_Image;

                        New_Image = RotateImageCCW90(image);

                        ImageIO.ImageToFile(New_Image, OutputFileName);
                    }
                }
                else if (args[1] == "ccw")
                {
                    if (args[2] == "270")
                    {
                        string InputFileName = args[3], OutputFileName = args[4];
                        if (!File.Exists(InputFileName))
                            return;

                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ColorFloatImage New_Image;

                        New_Image = RotateImageCW90(image);

                        ImageIO.ImageToFile(New_Image, OutputFileName);
                    }
                    if (args[2] == "90")
                    {
                        string InputFileName = args[3], OutputFileName = args[4];
                        if (!File.Exists(InputFileName))
                            return;

                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ColorFloatImage New_Image;

                        New_Image = RotateImageCCW90(image);

                        ImageIO.ImageToFile(New_Image, OutputFileName);
                    }
                    if (args[2] == "180")
                    {
                        string InputFileName = args[3], OutputFileName = args[4];
                        if (!File.Exists(InputFileName))
                            return;

                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ColorFloatImage New_Image, New_Image2;

                        New_Image = RotateImageCCW90(image);
                        New_Image2 = RotateImageCCW90(New_Image);

                        ImageIO.ImageToFile(New_Image2, OutputFileName);
                    }
                }
                else
                {
                    return;
                }
            }
            if ((args.Length == 4) && ((args[0] == "sobel") || (args[0] == "Sobel")))
            {
                if (args[1] == "x")
                {
                    string InputFileName = args[2], OutputFileName = args[3];
                    if (!File.Exists(InputFileName))
                        return;

                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                    ColorFloatImage New_Image;

                    New_Image = SobelX(image);


                    ImageIO.ImageToFile(New_Image, OutputFileName);
                }
                else if (args[1] == "y")
                {
                    string InputFileName = args[2], OutputFileName = args[3];
                    if (!File.Exists(InputFileName))
                        return;

                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                    ColorFloatImage New_Image;

                    New_Image = SobelY(image);


                    ImageIO.ImageToFile(New_Image, OutputFileName);
                }
                else if ((args[1] == "xy") || (args[1] == "yx"))
                {
                    string InputFileName = args[2], OutputFileName = args[3];
                    if (!File.Exists(InputFileName))
                        return;

                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                    ColorFloatImage New_Image;

                    New_Image = SobelXY(image);


                    ImageIO.ImageToFile(New_Image, OutputFileName);
                }
                else
                    return;
            }
            if ((args.Length == 4) && ((args[0] == "median") || (args[0] == "Median")))
            {
                int rad = Convert.ToInt32(args[1]);
                string InputFileName = args[2], OutputFileName = args[3];
                if (!File.Exists(InputFileName))
                    return;

                ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                ColorFloatImage New_Image;

                New_Image = MedianFilter(image, rad);
                
                ImageIO.ImageToFile(New_Image, OutputFileName);
            }
            if ((args.Length == 5) && ((args[0] == "gauss") || (args[0] == "Gauss")))
            {
                string Str_Sigma = Convert.ToString(args[1]);
                string Str_Gamma = Convert.ToString(args[2]);
                double gamma = 2.2, lenght_g, sigma = 1.0, lenght_s;
                if (Double.TryParse(Str_Gamma, out lenght_g))
                    gamma = lenght_g;
                gamma = double.Parse(Str_Gamma, CultureInfo.InvariantCulture);
                if(Double.TryParse(Str_Sigma, out lenght_s))
                    sigma = lenght_s;
                sigma = double.Parse(Str_Sigma, CultureInfo.InvariantCulture);
                string InputFileName = args[3], OutputFileName = args[4];
                if (!File.Exists(InputFileName))
                    return;

                ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                ColorFloatImage New_Image;

                New_Image = GaussGamma(image, sigma, gamma);


                ImageIO.ImageToFile(New_Image, OutputFileName);
            }
            if ((args.Length == 4) && ((args[0] == "gradient") || (args[0] == "Gradient")))
            {
                string Str_Sigma = Convert.ToString(args[1]);
                double sigma = 1.0, lenght_s;
                if (Double.TryParse(Str_Sigma, out lenght_s))
                    sigma = lenght_s;
                sigma = double.Parse(Str_Sigma, CultureInfo.InvariantCulture);
                string InputFileName = args[2], OutputFileName = args[3];
                if (!File.Exists(InputFileName))
                    return;

                ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                ColorFloatImage New_Image;

                New_Image = GradientGauss(image, sigma);


                ImageIO.ImageToFile(New_Image, OutputFileName);
            }
            /*string InputFileName = args[0], OutputFileName = args[1];
            if (!File.Exists(InputFileName))
                return;

            GrayscaleFloatImage image = ImageIO.FileToGrayscaleFloatImage(InputFileName);

            FlipImage(image);

            ImageIO.ImageToFile(image, OutputFileName);*/


        }
    }
}
