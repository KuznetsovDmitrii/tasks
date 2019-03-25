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

        //Билатериальная фильтрация
        static ColorFloatImage Bilateral(ColorFloatImage image, double sigma_d, double sigma_r)
        {
            ColorFloatImage New_Image = new ColorFloatImage(image.Width, image.Height);
            int Significant_Radius = Convert.ToInt32(Math.Ceiling(sigma_d));

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

                    ColorFloatPixel Weight = new ColorFloatPixel();

                    //double Weight = 0.0;
                    

                    for (int j = Y_P_Significant; j >= Y_M_Significant; j--)
                    {
                        int new_y = IsInRangeY(y, j - y, image.Height);
                        for (int i = X_M_Sigbificant; i <= X_P_Sigbificant; i++)
                        {
                            int new_x = IsInRangeX(x, i - x, image.Width);

                            ColorFloatPixel Old_Pixel = image[x, y];
                            ColorFloatPixel New_Pixel_1 = image[new_x, new_y];
                             
                            int Arg_Exp = (i - x) * (i - x) + (j - y) * (j - y);
                            double Arg_Exp_r = (float)Math.Sqrt((float)(Math.Pow(Old_Pixel.r - New_Pixel_1.r, 2)));
                            double Arg_Exp_b = (float)Math.Sqrt((float)(Math.Pow(Old_Pixel.b - New_Pixel_1.b, 2)));
                            double Arg_Exp_g = (float)Math.Sqrt((float)(Math.Pow(Old_Pixel.g - New_Pixel_1.g, 2)));

                            //double Arg_Exp_r = (float)(Math.Pow(Old_Pixel.r - New_Pixel_1.r, 2));
                            //double Arg_Exp_b = (float)(Math.Pow(Old_Pixel.b - New_Pixel_1.b, 2));
                            //double Arg_Exp_g = (float)(Math.Pow(Old_Pixel.g - New_Pixel_1.g, 2));

                            double square_sigma_d = 2 * sigma_d * sigma_d;
                            double square_sigma_r = 2 * sigma_r * sigma_r;

                            /*double Weight_In = (float)(Math.Exp((float)(((-1) * Arg_Exp) / (square_sigma_d)) - 
                                (float)(((float)Math.Pow(Old_Pixel.r - New_Pixel_1.r, 2) +
                                (float)Math.Pow(Old_Pixel.b - New_Pixel_1.b, 2) + 
                                (float)Math.Pow(Old_Pixel.g - New_Pixel_1.g, 2)) / (square_sigma_r))));*/

                            double Weight_r = (float)(Math.Exp((float)(((-1) * Arg_Exp) / (square_sigma_d)) - Arg_Exp_r / (square_sigma_r)));
                            double Weight_b = (float)(Math.Exp((float)(((-1) * Arg_Exp) / (square_sigma_d)) - Arg_Exp_b / (square_sigma_r)));
                            double Weight_g = (float)(Math.Exp((float)(((-1) * Arg_Exp) / (square_sigma_d)) - Arg_Exp_g / (square_sigma_d)));

                            pi[Pixel_i] = Old_Pixel;
                            pi[Pixel_i].r = (float)(Weight_r * New_Pixel_1.r);
                            pi[Pixel_i].b = (float)(Weight_b * New_Pixel_1.b);
                            pi[Pixel_i].g = (float)(Weight_g * New_Pixel_1.g);
                            Pixel_i++;

                            Weight.r += (float)Weight_r;
                            Weight.b += (float)Weight_b;
                            Weight.g += (float)Weight_g;

                            /*pi[Pixel_i] = Old_Pixel;
                            pi[Pixel_i].r = (float)(Weight_In * Old_Pixel.r);
                            pi[Pixel_i].b = (float)(Weight_In * Old_Pixel.b);
                            pi[Pixel_i].g = (float)(Weight_In * Old_Pixel.g);
                            Pixel_i++;

                            Weight = (float)Weight_In;*/
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

                    New_Pixel.r /= Weight.r;
                    New_Pixel.b /= Weight.b;
                    New_Pixel.g /= Weight.g;

                    //New_Pixel.r /= (float)Weight;
                    //New_Pixel.b /= (float)Weight;
                    //New_Pixel.g /= (float)Weight;

                    New_Image[x, y] = New_Pixel;
                }
            return New_Image;
        }

        static void Main(string[] args)
        {
            if ((args.Length == 5) && ((args[0] == "bilateral") || (args[0] == "Bilateral")))
            {
                string Str_Sigma_d = Convert.ToString(args[1]);
                string Str_Sigma_r = Convert.ToString(args[2]);
                double sigma_d, sigma_r;
                sigma_d = double.Parse(Str_Sigma_d, CultureInfo.InvariantCulture);
                sigma_r = double.Parse(Str_Sigma_r, CultureInfo.InvariantCulture);

                string InputFileName = args[3], OutputFileName = args[4];
                if (!File.Exists(InputFileName))
                    return;

                ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                ColorFloatImage New_Image;

                New_Image = Bilateral(image, sigma_d, sigma_r);


                ImageIO.ImageToFile(New_Image, OutputFileName);
            }
        }
    }
}
