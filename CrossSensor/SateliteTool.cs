using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace SateliteTool
{
    class SateliteImage
    {
        public int numfiles = 0;
        public string SceneID;
        public string SensorType;
        public string[] FileName;
        public int Height;
        public int Width;
        public double ccr;
        public DateTime AcqDate;
        public int Hgt, Wdt;

        private double[] Ml, Al; //Radiance
        private double[] Mp, Ap; //Reflectance
        private double[] K1, K2;
        private double SE, SA;
        private double L, U, R, D;
        private int intersect_offset_row;
        private int intersect_offset_col;
        private int union_offset_row;
        private int union_offset_col;
        private string path;
        private double[] Esun;
        private double[] ESd = new double[]
        {
            #region Earth Sun Distance Constant Values (366 Day Of Year matrix)
            0.983310, 0.983300, 0.983300, 0.983300, 0.983300, 0.983320, 0.983330,
            0.983350, 0.983380, 0.983410, 0.983450, 0.983490, 0.983540, 0.983590,
            0.983650, 0.983710, 0.983780, 0.983850, 0.983930, 0.984010, 0.984100,
            0.984190, 0.984280, 0.984390, 0.984490, 0.984600, 0.984720, 0.984840,
            0.984960, 0.985090, 0.985230, 0.985360, 0.985510, 0.985650, 0.985800,
            0.985960, 0.986120, 0.986280, 0.986450, 0.986620, 0.986800, 0.986980,
            0.987170, 0.987350, 0.987550, 0.987740, 0.987940, 0.988140, 0.988350,
            0.988560, 0.988770, 0.988990, 0.989210, 0.989440, 0.989660, 0.989890,
            0.990120, 0.990360, 0.990600, 0.990840, 0.991080, 0.991330, 0.991580,
            0.991830, 0.992080, 0.992340, 0.992600, 0.992860, 0.993120, 0.993390,
            0.993650, 0.993920, 0.994190, 0.994460, 0.994740, 0.995010, 0.995290,
            0.995560, 0.995840, 0.996120, 0.996400, 0.996690, 0.996970, 0.997250,
            0.997540, 0.997820, 0.998110, 0.998400, 0.998680, 0.998970, 0.999260,
            0.999540, 0.999830, 1.000120, 1.000410, 1.000690, 1.000980, 1.001270,
            1.001550, 1.001840, 1.002120, 1.002400, 1.002690, 1.002970, 1.003250,
            1.003530, 1.003810, 1.004090, 1.004370, 1.004640, 1.004920, 1.005190,
            1.005460, 1.005730, 1.006000, 1.006260, 1.006530, 1.006790, 1.007050,
            1.007310, 1.007560, 1.007810, 1.008060, 1.008310, 1.008560, 1.008800,
            1.009040, 1.009280, 1.009520, 1.009750, 1.009980, 1.010200, 1.010430,
            1.010650, 1.010870, 1.011080, 1.011290, 1.011500, 1.011700, 1.011910,
            1.012100, 1.012300, 1.012490, 1.012670, 1.012860, 1.013040, 1.013210,
            1.013380, 1.013550, 1.013710, 1.013870, 1.014030, 1.014180, 1.014330,
            1.014470, 1.014610, 1.014750, 1.014880, 1.015000, 1.015130, 1.015240,
            1.015360, 1.015470, 1.015570, 1.015670, 1.015770, 1.015860, 1.015950,
            1.016030, 1.016100, 1.016180, 1.016250, 1.016310, 1.016370, 1.016420,
            1.016470, 1.016520, 1.016560, 1.016590, 1.016620, 1.016650, 1.016670,
            1.016680, 1.016700, 1.016700, 1.016700, 1.016700, 1.016690, 1.016680,
            1.016660, 1.016640, 1.016610, 1.016580, 1.016550, 1.016500, 1.016460,
            1.016410, 1.016350, 1.016290, 1.016230, 1.016160, 1.016090, 1.016010,
            1.015920, 1.015840, 1.015750, 1.015650, 1.015550, 1.015440, 1.015330,
            1.015220, 1.015100, 1.014970, 1.014850, 1.014710, 1.014580, 1.014440,
            1.014290, 1.014140, 1.013990, 1.013830, 1.013670, 1.013510, 1.013340,
            1.013170, 1.012990, 1.012810, 1.012630, 1.012440, 1.012250, 1.012050,
            1.011860, 1.011650, 1.011450, 1.011240, 1.011030, 1.010810, 1.010600,
            1.010370, 1.010150, 1.009920, 1.009690, 1.009460, 1.009220, 1.008980,
            1.008740, 1.008500, 1.008250, 1.008000, 1.007750, 1.007500, 1.007240,
            1.006980, 1.006720, 1.006460, 1.006200, 1.005930, 1.005660, 1.005390,
            1.005120, 1.004850, 1.004570, 1.004300, 1.004020, 1.003740, 1.003460,
            1.003180, 1.002900, 1.002620, 1.002340, 1.002050, 1.001770, 1.001480,
            1.001190, 1.000910, 1.000620, 1.000330, 1.000050, 0.999760, 0.999470,
            0.999180, 0.998900, 0.998610, 0.998320, 0.998040, 0.997750, 0.997470,
            0.997180, 0.996900, 0.996620, 0.996340, 0.996050, 0.995770, 0.995500,
            0.995220, 0.994940, 0.994670, 0.994400, 0.994120, 0.993850, 0.993590,
            0.993320, 0.993060, 0.992790, 0.992530, 0.992280, 0.992020, 0.991770,
            0.991520, 0.991270, 0.991020, 0.990780, 0.990540, 0.990300, 0.990070,
            0.989830, 0.989610, 0.989380, 0.989160, 0.988940, 0.988720, 0.988510,
            0.988300, 0.988090, 0.987890, 0.987690, 0.987500, 0.987310, 0.987120,
            0.986940, 0.986760, 0.986580, 0.986410, 0.986240, 0.986080, 0.985920,
            0.985770, 0.985620, 0.985470, 0.985330, 0.985190, 0.985060, 0.984930,
            0.984810, 0.984690, 0.984570, 0.984460, 0.984360, 0.984260, 0.984160,
            0.984070, 0.983990, 0.983910, 0.983830, 0.983760, 0.983700, 0.983630,
            0.983580, 0.983530, 0.983480, 0.983440, 0.983400, 0.983370, 0.983350,
            0.983330, 0.983310
            #endregion
        };

        public void LoadInfo(string fn)
        {
            path = Path.GetDirectoryName(fn);
            string ext = Path.GetExtension(fn);
            numfiles++;
            if (fn.Contains("LC8"))
            {
                SensorType = "Landsat 8 OLI";
                FileName = new string[12];
                K1 = new double[2];
                K2 = new double[2];
                Ml = new double[11];
                Al = new double[11];
                Mp = new double[9];
                Ap = new double[9];
            }
            else if (fn.Contains("LE7"))
            {
                SensorType = "Landsat 7 ETM+";
                FileName = new string[9];
                K1 = new double[1];
                K2 = new double[1];
                Ml = new double[9];
                Al = new double[9];
                Esun = new double[] { 1970, 1842, 1547, 1044, 225.7, 0, 0, 82.06, 1369 };
            }
            else if (fn.Contains("LT5"))
            {
                SensorType = "Landsat 5 TM";
                FileName = new string[7];
                K1 = new double[1];
                K2 = new double[1];
                Ml = new double[7];
                Al = new double[7];
                Esun = new double[7] { 1970, 1842, 1547, 1044, 225.7, 0, 82.06 };

            }
            else if (ext == ".rpt")
            {
                SensorType = "SPOT 5";
                FileName = new string[4];
                Ml = new double[4];
                Al = new double[4];
                Esun = new double[] { 1843, 1568, 1052, 233 };
            }
            else
            {
                MessageBox.Show("Unsupported Landsat");
                return;
            }
            string[] MTL = File.ReadAllLines(fn);
            string[] spliter = new string[] { "\t", " ", "\"" };
            bool GainPos = false;
            int Zone = 0;
            if (ext == ".rpt")
            {
                foreach (string strs in MTL)
                {
                    string[] str;
                    #region SPOT 5 Image
                    for (int i = 0; i < 4; i++)
                    {
                        if (strs.Contains("Imagery File( Band " + (i + 1).ToString()))
                        {
                            str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                            FileName[i] = str[str.Length - 1];
                        }
                    }
                    if (strs.Contains("Image Size"))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        Height = Convert.ToInt32(str[4]);
                        Width = Convert.ToInt32(str[7]);
                    }
                    if (strs.Contains("UL"))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        U = Convert.ToDouble(str[3]);
                        L = Convert.ToDouble(str[4]);
                    }
                    if (strs.Contains("LR"))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        D = Convert.ToDouble(str[3]);
                        R = Convert.ToDouble(str[4]);
                    }
                    if (GainPos)
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < 4; i++)
                        {
                            Ml[i] = 1 / Convert.ToDouble(str[i]);
                            Al[i] = 0;
                        }
                        GainPos = false;
                    }
                    if (strs.Contains("PHYSICAL GAIN "))
                    {
                        GainPos = true;
                    }
                    if (strs.Contains("FILE ID: "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        SceneID = str[str.Length - 1];
                    }
                    if (strs.Contains("GMT"))
                    {
                        str = strs.Split(new string[] { "\t", " ", "\"", "/", ":", "." }, StringSplitOptions.RemoveEmptyEntries);
                        AcqDate = new DateTime(Convert.ToInt32(str[1]), Convert.ToInt32(str[2]), Convert.ToInt32(str[3]), Convert.ToInt32(str[4]) + 8, Convert.ToInt32(str[5]), Convert.ToInt32(str[6]));
                    }
                    if (strs.Contains("Elevation"))
                    {
                        str = strs.Split(new string[] { "\t", " ", "\"", "/", "," }, StringSplitOptions.RemoveEmptyEntries);
                        SA = Convert.ToDouble(str[2]);
                        SE = Convert.ToDouble(str[5]);
                    }
                    #endregion
                }
            }
            else
            {
                foreach (string strs in MTL)
                {
                    string[] str;
                    #region Landsat All
                    if (strs.Contains("LANDSAT_SCENE_ID "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        SceneID = str[str.Length - 1];
                    }
                    if (strs.Contains("DATE_ACQUIRED "))
                    {
                        str = strs.Split(new string[] { "\t", " ", "\"", "-" }, StringSplitOptions.RemoveEmptyEntries);
                        AcqDate = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[3]), Convert.ToInt32(str[4]));
                    }
                    if (strs.Contains("CORNER_UL_PROJECTION_X_PRODUCT "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        L = Convert.ToDouble(str[str.Length - 1]);
                    }
                    if (strs.Contains("CORNER_UL_PROJECTION_Y_PRODUCT "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        U = Convert.ToDouble(str[str.Length - 1]);
                    }
                    if (strs.Contains("CORNER_LR_PROJECTION_X_PRODUCT "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        R = Convert.ToDouble(str[str.Length - 1]);
                    }
                    if (strs.Contains("CORNER_LR_PROJECTION_Y_PRODUCT "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        D = Convert.ToDouble(str[str.Length - 1]);
                    }
                    if (strs.Contains("REFLECTIVE_LINES "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        Height = Hgt = Convert.ToInt32(str[str.Length - 1]);
                    }
                    if (strs.Contains("REFLECTIVE_SAMPLES "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        Width = Wdt = Convert.ToInt32(str[str.Length - 1]);
                    }
                    if (strs.Contains("CLOUD_COVER "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        ccr = Convert.ToDouble(str[str.Length - 1]);
                    }
                    if (strs.Contains("SUN_AZIMUTH "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        SA = Convert.ToDouble(str[str.Length - 1]);
                    }
                    if (strs.Contains("SUN_ELEVATION "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        SE = Convert.ToDouble(str[str.Length - 1]);
                    }
                    if (strs.Contains("UTM_ZONE "))
                    {
                        str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                        Zone = Convert.ToInt32(str[str.Length - 1]);
                    }
                    #endregion
                    if (SensorType == "Landsat 8 OLI")
                    {
                        #region Landsat 8
                        for (int n = 0; n < 11; n++)
                        {
                            if (strs.Contains("FILE_NAME_BAND_" + (n + 1).ToString() + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                FileName[n] = str[str.Length - 1];
                            }
                            if (strs.Contains("RADIANCE_MULT_BAND_" + (n + 1).ToString() + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Ml[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                            if (strs.Contains("RADIANCE_ADD_BAND_" + (n + 1).ToString() + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Al[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                            if (strs.Contains("REFLECTANCE_MULT_BAND_" + (n + 1).ToString() + " ") && n < 9)
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Mp[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                            if (strs.Contains("REFLECTANCE_ADD_BAND_" + (n + 1).ToString() + " ") && n < 9)
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Ap[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                            if (strs.Contains("K1_CONSTANT_BAND_" + (n + 10).ToString() + " ") && n < 2)
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                K1[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                            if (strs.Contains("K2_CONSTANT_BAND_" + (n + 10).ToString() + " ") && n < 2)
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                K2[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                        }
                        if (strs.Contains("FILE_NAME_BAND_QUALITY "))
                        {
                            str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                            FileName[11] = str[str.Length - 1];
                        }
                        #endregion
                    }
                    else if (SensorType == "Landsat 7 ETM+")
                    {
                        #region Landsat 7 ETM+
                        string[] tails = new string[9] { "1", "2", "3", "4", "5", "6_VCID_1", "6_VCID_2", "7", "8" };
                        for (int n = 0; n < 9; n++)
                        {
                            if (strs.Contains("FILE_NAME_BAND_" + tails[n] + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                FileName[n] = str[str.Length - 1];
                            }
                            if (strs.Contains("RADIANCE_MULT_BAND_" + tails[n] + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Ml[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                            if (strs.Contains("RADIANCE_ADD_BAND_" + tails[n] + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Al[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                        }
                        K1[0] = 666.09;
                        K2[0] = 1282.71;
                        #endregion
                    }
                    else if (SensorType == "Landsat 5 TM")
                    {
                        #region Landsat 5 TM
                        for (int n = 0; n < 7; n++)
                        {
                            if (strs.Contains("FILE_NAME_BAND_" + (n + 1).ToString() + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                FileName[n] = str[str.Length - 1];
                            }
                            if (strs.Contains("RADIANCE_MULT_BAND_" + (n + 1).ToString() + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Ml[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                            if (strs.Contains("RADIANCE_ADD_BAND_" + (n + 1).ToString() + " "))
                            {
                                str = strs.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                                Al[n] = Convert.ToDouble(str[str.Length - 1]);
                            }
                        }
                        K1[0] = 607.76;
                        K2[0] = 1260.56;
                        #endregion
                    }
                }
            }
            Hgt = Height;
            Wdt = Width;
        }
        public byte[] ReadBand(int band)
        {
            byte[] img = new byte[Hgt * Wdt];
            byte[] buffer = File.ReadAllBytes(path + "\\" + SceneID + "\\"+ FileName[band]);
            int TIFFTag = 8;
            if (SensorType.Contains("SPOT"))
                TIFFTag = 0;

            Parallel.For(intersect_offset_row, Hgt + intersect_offset_row, i =>
            {
                Buffer.BlockCopy(buffer, TIFFTag + (i * Width + intersect_offset_col), img, (i - intersect_offset_row) * Wdt, Wdt);
            });
            return img;
        }
        public UInt16[] ReadBand16(int band)
        {
            UInt16[] img = new UInt16[Hgt * Wdt];
            byte[] buffer = File.ReadAllBytes(path + "\\" + SceneID+ "\\" + FileName[band]);
            int TIFFTag = 8;
            if (Hgt > Height || Wdt > Width)
            {
                Parallel.For(0, Height, i =>
                {
                    Buffer.BlockCopy(buffer, TIFFTag + 2 * (i * Width), img, 2 * ((i + intersect_offset_row) * Wdt + intersect_offset_col), 2 * Width);
                });
            }
            else
            {
                Parallel.For(intersect_offset_row, Hgt + intersect_offset_row, i =>
                {
                    Buffer.BlockCopy(buffer, TIFFTag + 2 * (i * Width + intersect_offset_col), img, 2 * (i - intersect_offset_row) * Wdt, Wdt * 2);
                });
            }
            return img;
        }
        public double[] ReadBand_Reflectance(int band)
        {
            double[] TOA;
            if (SensorType == "Landsat 8 OLI")
            {
                UInt16[] img = ReadBand16(band);
                TOA = new double[img.Length];
                double[] ro = new double[65536];
                Parallel.For(1, 65536, i =>
                {
                    ro[i] = i * Mp[band] + Ap[band];
                });
                Parallel.For(0, TOA.Length, i =>
                {
                    TOA[i] = ro[img[i]];
                });
            }
            else
            {
                byte[] img = ReadBand(band);
                TOA = new double[img.Length];
                double[] ro = new double[256];
                double[] rad = new double[256];
                double d = ESd[AcqDate.DayOfYear];
                Parallel.For(1, 256, i =>
                {
                    rad[i] = i * Ml[band] + Al[band];
                    ro[i] = Math.PI * rad[i] * d * d / (Esun[band] * Math.Sin(SE / 180.0 * Math.PI));
                });
                Parallel.For(0, TOA.Length, i =>
                {
                    TOA[i] = ro[img[i]];
                });
            }
            return TOA;
        }
        public Bitmap ToTrueColor()
        {
            Bitmap bmp;
            double[] b, g, r;
            if (SensorType == "Landsat 8 OLI")
            {
                b = ReadBand_Reflectance(1);
                g = ReadBand_Reflectance(2);
                r = ReadBand_Reflectance(3);
            }
            else if (SensorType.Contains("SPOT"))
            {
                return null;
            }
            else
            {
                b = ReadBand_Reflectance(0);
                g = ReadBand_Reflectance(1);
                r = ReadBand_Reflectance(2);
            }
            bmp = BitmapTool.Array2Bitmap(b, g, r, Hgt, Wdt);
            return bmp;
        }
        
        public static double[] MAD_processing(double[,] L7_matrix, double[,] L8_matrix, int lengtharray)
        {
            double[,] meanL7 = new double[3, 1];
            double[,] meanL8 = new double[3, 1];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    meanL7[i, 0] += L7_matrix[i, j];
                    meanL8[i, 0] += L8_matrix[i, j];
                }
                meanL7[i, 0] = meanL7[i, 0] / lengtharray;
                meanL8[i, 0] = meanL8[i, 0] / lengtharray;
            }
            /*
            Matrix<double> mat7 = Matrix<double>.Build.DenseOfArray(MatrixmeanL7);
            Matrix<double> mat8 = Matrix<double>.Build.DenseOfArray(MatrixmeanL8);

            Matrix<double> A = mat7.Transpose() * mat8;
            Matrix<double> V = A.Evd().EigenVectors;
            Matrix<double> D = A.Evd().D;
            
            double[,] AAAA = A.ToArray();
            */

            double[,] MmL7 = new double[3, lengtharray];
            double[,] MmL8 = new double[3, lengtharray];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    MmL7[i, j] = L7_matrix[i, j] - meanL7[i, 0];
                    MmL8[i, j] = L8_matrix[i, j] - meanL8[i, 0];
                }
            }

            Matrix<double> XX = Matrix<double>.Build.DenseOfArray(MmL7);
            Matrix<double> YY = Matrix<double>.Build.DenseOfArray(MmL8);
            /*
            Matrix<double> X = Matrix<double>.Build.DenseOfArray(L7_matrix);
            Matrix<double> Y = Matrix<double>.Build.DenseOfArray(L8_matrix);
            Matrix<double> MeanX = Matrix<double>.Build.DenseOfArray(meanL7);
            Matrix<double> MeanY = Matrix<double>.Build.DenseOfArray(meanL8);

            Matrix<double> XX = X - MeanX;
            Matrix<double> YY = Y - MeanY;
            */
            Matrix<double> Sxx = XX * XX.Transpose();
            Matrix<double> Syy = YY * YY.Transpose();

            //Matrix<double> XT = XX.Transpose();
            //Matrix<double> YT = YY.Transpose();

            Matrix<double> Sxy = XX * YY.Transpose();
            Matrix<double> Syx = YY * XX.Transpose();

            Matrix<double> iSxx = Sxx.Inverse();
            Matrix<double> iSyy = Syy.Inverse();

            Matrix<double> A = iSxx * Sxy * iSyy * Syx;
            Matrix<double> B = iSyy * Syx * iSxx * Sxy;

            Matrix<double> VA = A.Evd().EigenVectors;
            Matrix<double> VB = B.Evd().EigenVectors;

            Matrix<double> iVA = VA.Inverse();
            Matrix<double> iVB = VB.Inverse();

            //Matrix<double> U = iVA * XX;
            //Matrix<double> V = iVB * YY;
            //Matrix<double> MAD = U - V;

            double[,] iiVA = iVA.ToArray();
            double[,] iiVB = iVB.ToArray();
            double[,] iiXX = XX.ToArray();
            double[,] iiYY = YY.ToArray();
            double[,] U = new double[3, lengtharray];
            double[,] V = new double[3, lengtharray];
            double[,] MADresult = new double[3, lengtharray];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    U[0, j] = (iiVA[0, 0] * iiXX[0, j]) + (iiVA[0, 1] * iiXX[1, j]) + (iiVA[0, 2] * iiXX[2, j]);
                    U[1, j] = (iiVA[1, 0] * iiXX[0, j]) + (iiVA[1, 1] * iiXX[1, j]) + (iiVA[1, 2] * iiXX[2, j]);
                    U[2, j] = (iiVA[2, 0] * iiXX[0, j]) + (iiVA[2, 1] * iiXX[1, j]) + (iiVA[2, 2] * iiXX[2, j]);

                    V[0, j] = (iiVB[0, 0] * iiYY[0, j]) + (iiVB[0, 1] * iiYY[1, j]) + (iiVB[0, 2] * iiYY[2, j]);
                    V[1, j] = (iiVB[1, 0] * iiYY[0, j]) + (iiVB[1, 1] * iiYY[1, j]) + (iiVB[1, 2] * iiYY[2, j]);
                    V[2, j] = (iiVB[2, 0] * iiYY[0, j]) + (iiVB[2, 1] * iiYY[1, j]) + (iiVB[2, 2] * iiYY[2, j]);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    MADresult[i, j] = U[i,j] - V[i,j];
                }
            }

            // Standard deviation
            double[,] MADmean = new double[3, 1];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    MADmean[i, 0] += MADresult[i, j];
                }
                MADmean[i, 0] = MADmean[i, 0] / lengtharray;
            }
            double[,] MADmm = new double[3, lengtharray];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    MADmm[i, j] = Math.Pow(MADresult[i, j] - MADmean[i, 0],2);
                }
            }

            double[,] MADsum = new double[3, 1];
            double[,] MADstd = new double[3, 1];
            double[,] STD = new double[3, 1];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    MADsum[i, 0] += MADmm[i, j];
                }
                MADsum[i, 0] = MADsum[i, 0] / lengtharray;
                MADstd[i, 0] = Math.Sqrt(MADsum[i,0]);
            }

            double[] NMAD = new double[lengtharray];
            double[,] NMADsquare = new double[3, lengtharray];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < lengtharray; j++)
                {
                    NMADsquare[i, j] = Math.Pow(MADresult[i,j],2);
                    NMAD[j] += NMADsquare[i, j] / Math.Pow(MADstd[i, 0], 2);
                }
            }
            
            return NMAD;
        }
        public Bitmap ToFalseColor()
        {
            Bitmap bmp;
            double[] b, g, r;
            if (SensorType == "Landsat 8 OLI")
            {
                b = ReadBand_Reflectance(2);
                g = ReadBand_Reflectance(3);
                r = ReadBand_Reflectance(4);
            }
            else if (SensorType.Contains("SPOT"))
            {
                b = ReadBand_Reflectance(0);
                g = ReadBand_Reflectance(1);
                r = ReadBand_Reflectance(2);
            }
            else
            {
                b = ReadBand_Reflectance(1);
                g = ReadBand_Reflectance(2);
                r = ReadBand_Reflectance(3);
            }
            bmp = BitmapTool.Array2Bitmap(b, g, r, Hgt, Wdt);
            return bmp;
        }
        public static void Register_Intersect(List<SateliteImage> Image)
        {
            double L = double.MinValue;
            double U = double.MaxValue;
            double R = double.MaxValue;
            double D = double.MinValue;
            foreach (SateliteImage img in Image)
            {
                if (img.L > L)
                    L = img.L;
                if (img.U < U)
                    U = img.U;
                if (img.R < R)
                    R = img.R;
                if (img.D > D)
                    D = img.D;
            }
            foreach (SateliteImage img in Image)
            {
                img.intersect_offset_row = (int)Math.Round((img.U - U) / (float)(img.U - img.D) * img.Height);
                img.intersect_offset_col = (int)Math.Round((L - img.L) / (float)(img.R - img.L) * img.Width);
                img.Hgt = (int)Math.Round(img.Height * (U - D) / (float)(img.U - img.D));
                img.Wdt = (int)Math.Round(img.Width * (R - L) / (float)(img.R - img.L));
                if (img.Hgt <= 0 || img.Wdt <= 0)
                {
                    MessageBox.Show("No Intersect!");
                    break;
                }
            }
        }
        public static Bitmap UnionTrueColor(List<SateliteImage> Image)
        {
            #region Registration
            double L = double.MaxValue;
            double U = double.MinValue;
            double R = double.MinValue;
            double D = double.MaxValue;
            foreach (SateliteImage img in Image)
            {
                if (img.L < L)
                    L = img.L;
                if (img.U > U)
                    U = img.U;
                if (img.R > R)
                    R = img.R;
                if (img.D < D)
                    D = img.D;
            }

            int H = 0, W = 0; //Total Range
            foreach (SateliteImage img in Image)
            {
                img.union_offset_row = (int)Math.Round((U - img.U) / (float)(img.U - img.D) * img.Height);
                img.union_offset_col = (int)Math.Round((img.L - L) / (float)(img.R - img.L) * img.Width);
                H = (int)Math.Round(img.Height * (U - D) / (float)(img.U - img.D));
                W = (int)Math.Round(img.Width * (R - L) / (float)(img.R - img.L));
            }
            double[][] data = new double[3][];
            Parallel.For(0, 3, b =>
            {
                data[b] = new double[H * W];
            });
            #endregion

            foreach (SateliteImage img in Image)
            {
                double[] b, g, r;
                if (img.SensorType.Contains("Landsat 8"))
                {
                    b = img.ReadBand_Reflectance(1);
                    g = img.ReadBand_Reflectance(2);
                    r = img.ReadBand_Reflectance(3);
                }
                else if(img.SensorType.Contains("SPOT"))
                {
                    return null;
                }
                else
                {
                    b = img.ReadBand_Reflectance(0);
                    g = img.ReadBand_Reflectance(1);
                    r = img.ReadBand_Reflectance(2);
                }
                Parallel.For(0, img.Height, i =>
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        if (b[i * img.Width + j] == 0 || g[i * img.Width + j] == 0 || r[i * img.Width + j] == 0)
                            continue;
                        data[0][(i + img.union_offset_row) * W + j + img.union_offset_col] = b[i * img.Width + j];
                        data[1][(i + img.union_offset_row) * W + j + img.union_offset_col] = g[i * img.Width + j];
                        data[2][(i + img.union_offset_row) * W + j + img.union_offset_col] = r[i * img.Width + j];
                    }
                });
            }

            Bitmap bmp = BitmapTool.Array2Bitmap(data[0], data[1], data[2], H, W);
            return bmp;
        }
        public static Bitmap UnionFalseColor(List<SateliteImage> Image)
        {
            #region Registration
            double L = double.MaxValue;
            double U = double.MinValue;
            double R = double.MinValue;
            double D = double.MaxValue;
            foreach (SateliteImage img in Image)
            {
                if (img.L < L)
                    L = img.L;
                if (img.U > U)
                    U = img.U;
                if (img.R > R)
                    R = img.R;
                if (img.D < D)
                    D = img.D;
            }

            int H = 0, W = 0; //Total Range
            foreach (SateliteImage img in Image)
            {
                img.union_offset_row = (int)Math.Round((U - img.U) / (float)(img.U - img.D) * img.Height);
                img.union_offset_col = (int)Math.Round((img.L - L) / (float)(img.R - img.L) * img.Width);
                H = (int)Math.Round(img.Height * (U - D) / (float)(img.U - img.D));
                W = (int)Math.Round(img.Width * (R - L) / (float)(img.R - img.L));
            }
            double[][] data = new double[3][];
            Parallel.For(0, 3, b =>
            {
                data[b] = new double[H * W];
            });
            #endregion

            foreach (SateliteImage img in Image)
            {
                double[] b, g, r;
                if (img.SensorType.Contains("Landsat 8"))
                {
                    b = img.ReadBand_Reflectance(2);
                    g = img.ReadBand_Reflectance(3);
                    r = img.ReadBand_Reflectance(4);
                }
                else if(img.SensorType.Contains("SPOT"))
                {
                    b = img.ReadBand_Reflectance(0);
                    g = img.ReadBand_Reflectance(1);
                    r = img.ReadBand_Reflectance(2);
                }
                else
                {
                    b = img.ReadBand_Reflectance(1);
                    g = img.ReadBand_Reflectance(2);
                    r = img.ReadBand_Reflectance(3);
                }
                Parallel.For(0, img.Height, i =>
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        if (b[i * img.Width + j] == 0 || g[i * img.Width + j] == 0 || r[i * img.Width + j] == 0)
                            continue;
                        data[0][(i + img.union_offset_row) * W + j + img.union_offset_col] = b[i * img.Width + j];
                        data[1][(i + img.union_offset_row) * W + j + img.union_offset_col] = g[i * img.Width + j];
                        data[2][(i + img.union_offset_row) * W + j + img.union_offset_col] = r[i * img.Width + j];
                    }
                });
            }

            Bitmap bmp = BitmapTool.Array2Bitmap(data[0], data[1], data[2], H, W);
            return bmp;
        }
    }
    class BitmapTool
    {
        public static Bitmap Array2Bitmap(UInt16[] b, UInt16[] g, UInt16[] r, int Height, int Width)
        {
            PixelFormat format = PixelFormat.Format24bppRgb;
            Bitmap myBitmap = new Bitmap(Width, Height, format);
            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);

            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int j = 0; j < bmpData.Height; j++)
                {
                    for (int i = 0; i < bmpData.Width; i++)
                    {
                        *(imgPtr + 3 * i + 0) = Convert.ToByte((int)(b[j * Width + i] / 256.0));
                        *(imgPtr + 3 * i + 1) = Convert.ToByte((int)(g[j * Width + i] / 256.0));
                        *(imgPtr + 3 * i + 2) = Convert.ToByte((int)(r[j * Width + i] / 256.0));
                    }
                    imgPtr += bmpData.Stride;
                }
            }

            myBitmap.UnlockBits(bmpData);
            return myBitmap;
        }
        public static Bitmap Array2Bitmap(byte[] b, byte[] g, byte[] r, int Height, int Width)
        {
            PixelFormat format = PixelFormat.Format24bppRgb;
            Bitmap myBitmap = new Bitmap(Width, Height, format);
            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);

            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int j = 0; j < bmpData.Height; j++)
                {
                    for (int i = 0; i < bmpData.Width; i++)
                    {
                        *(imgPtr + 3 * i + 0) = b[j * Width + i];
                        *(imgPtr + 3 * i + 1) = g[j * Width + i];
                        *(imgPtr + 3 * i + 2) = r[j * Width + i];
                    }
                    imgPtr += bmpData.Stride;
                }
            }

            myBitmap.UnlockBits(bmpData);
            return myBitmap;
        }
        public static Bitmap Array2Bitmap(double[] vb, double[] vg, double[] vr, int Height, int Width)
        {
            byte[] b = new byte[Height * Width];
            byte[] g = new byte[Height * Width];
            byte[] r = new byte[Height * Width];
            double max_ref = 0.3;
            double min_ref = 0;
            Parallel.For(0, Height * Width, i =>
            {
                if (vb[i] > max_ref)
                    vb[i] = max_ref;
                if (vb[i] < min_ref)
                    vb[i] = min_ref;
                if (vg[i] > max_ref)
                    vg[i] = max_ref;
                if (vg[i] < min_ref)
                    vg[i] = min_ref;
                if (vr[i] > max_ref)
                    vr[i] = max_ref;
                if (vr[i] < min_ref)
                    vr[i] = min_ref;
                b[i] = Convert.ToByte((vb[i] - min_ref) / (max_ref - min_ref) * 255);
                g[i] = Convert.ToByte((vg[i] - min_ref) / (max_ref - min_ref) * 255);
                r[i] = Convert.ToByte((vr[i] - min_ref) / (max_ref - min_ref) * 255);
            });

            PixelFormat format = PixelFormat.Format24bppRgb;
            Bitmap myBitmap = new Bitmap(Width, Height, format);
            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);

            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int j = 0; j < bmpData.Height; j++)
                {
                    for (int i = 0; i < bmpData.Width; i++)
                    {
                        *(imgPtr + 3 * i + 0) = b[j * Width + i];
                        *(imgPtr + 3 * i + 1) = g[j * Width + i];
                        *(imgPtr + 3 * i + 2) = r[j * Width + i];
                    }
                    imgPtr += bmpData.Stride;
                }
            }

            myBitmap.UnlockBits(bmpData);
            return myBitmap;
        }
        // LANDSAT 8
        public static Bitmap Array2Bitmap(UInt16[] Intensity, int Height, int Width)
        {
            PixelFormat format = PixelFormat.Format8bppIndexed;
            ///PixelFormat format = PixelFormat.Format16bppGrayScale;
            ColorPalette tempPalette;


            Bitmap myBitmap = new Bitmap(Width, Height, format);
            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);

            using (Bitmap tempBmp = new Bitmap(1, 1, format))
            {
                tempPalette = tempBmp.Palette;
            }


            for (int i = 0; i < 255; i++)
            {
                tempPalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            myBitmap.Palette = tempPalette;

            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int j = 0; j < bmpData.Height; j++)
                {
                    for (int i = 0; i < bmpData.Width; i++)
                    {
                        *(imgPtr + i) = Convert.ToByte((int)(Intensity[j * Width + i] / 256.0));
                    }
                    imgPtr += bmpData.Stride;
                }
            }

            myBitmap.UnlockBits(bmpData);
            return myBitmap;
        }
        
        // LANDSAT 7
        public static Bitmap Array2Bitmap(byte[] Intensity, int Height, int Width)
        {
            PixelFormat format = PixelFormat.Format8bppIndexed;
            ColorPalette tempPalette;

            Bitmap myBitmap = new Bitmap(Width, Height, format);
            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);

            using (Bitmap tempBmp = new Bitmap(1, 1, format))
            {
                tempPalette = tempBmp.Palette;
            }


            for (int i = 0; i < 255; i++)
            {
                tempPalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            myBitmap.Palette = tempPalette;

            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int j = 0; j < bmpData.Height; j++)
                {
                    for (int i = 0; i < bmpData.Width; i++)
                    {
                        *(imgPtr + i) = Intensity[j * Width + i];
                    }
                    imgPtr += bmpData.Stride;
                }
            }

            myBitmap.UnlockBits(bmpData);
            return myBitmap;
        }
        
        public static Bitmap Array2Bitmap(double[] value, int Height, int Width)
        {
            PixelFormat format = PixelFormat.Format8bppIndexed;
            ColorPalette tempPalette;
            byte[] Intensity = new byte[Height * Width];
            double max = value.Max();
            double min = value.Min();
            Parallel.For(0, Height * Width, i =>
            {
                Intensity[i] = Convert.ToByte((value[i] - min) / (max - min) * 255);
            });

            Bitmap myBitmap = new Bitmap(Width, Height, format);
            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);

            using (Bitmap tempBmp = new Bitmap(1, 1, format))
            {
                tempPalette = tempBmp.Palette;
            }


            for (int i = 0; i < 255; i++)
            {
                tempPalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            myBitmap.Palette = tempPalette;

            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int j = 0; j < bmpData.Height; j++)
                {
                    for (int i = 0; i < bmpData.Width; i++)
                    {
                        *(imgPtr + i) = Intensity[j * Width + i];
                    }
                    imgPtr += bmpData.Stride;
                }
            }

            myBitmap.UnlockBits(bmpData);
            return myBitmap;
        }
        public static byte[][] Bitmap2Array(Bitmap myBitmap)
        {
            int LayerNumber = 3;

            byte[][] imgData = new byte[LayerNumber][];
            for (int i = 0; i < LayerNumber; i++)
                imgData[i] = new byte[myBitmap.Height * myBitmap.Width];

            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;
            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int x = 0; x < bmpData.Height; x++)
                {
                    for (int y = 0; y < bmpData.Width; y++)
                    {
                        for (int k = 0; k < LayerNumber; k++)
                        {
                            imgData[k][x * bmpData.Width + y] = *(imgPtr + k);
                        }
                        imgPtr += LayerNumber;
                    }
                    imgPtr += byteOfSkip;
                }
            }
            myBitmap.UnlockBits(bmpData);
            return imgData;
        }
        //轉換Bitmap為Array
        public static byte[,,] Bitmap2ArrayModel2(Bitmap myBitmap)
        {
            byte[,,] imgData = new byte[myBitmap.Width, myBitmap.Height, 3];
            int LayerNumber;
            PixelFormat Format = new PixelFormat();
            ColorPalette tempPalette;

            //判斷24位元彩色影像(R,G,B)
            if (myBitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                LayerNumber = 3;
                Format = PixelFormat.Format24bppRgb;
                imgData = new byte[myBitmap.Width, myBitmap.Height, LayerNumber];

                BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadWrite, Format);
                int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;

                unsafe
                {
                    byte* imgPtr = (byte*)(bmpData.Scan0);
                    for (int j = 0; j < bmpData.Height; j++)
                    {
                        for (int i = 0; i < bmpData.Width; i++)
                        {
                            for (int k = 0; k < LayerNumber; k++)
                            {
                                imgData[i, j, k] = *(imgPtr + k);
                            }
                            imgPtr += LayerNumber;
                        }
                        imgPtr += byteOfSkip;
                    }
                }
                myBitmap.UnlockBits(bmpData);
            }

            //判斷8位元灰階影像
            if (myBitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                LayerNumber = 1;
                Format = PixelFormat.Format8bppIndexed;
                tempPalette = myBitmap.Palette;
                byte[] newDN = new byte[256];
                Color c;
                for (int i = 0; i < 256; i++)
                {
                    c = tempPalette.Entries[i];
                    newDN[i] = Convert.ToByte(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
                }
                imgData = new byte[myBitmap.Width, myBitmap.Height, LayerNumber];

                BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadWrite, Format);
                int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;

                unsafe
                {
                    byte tmpDN;
                    byte* imgPtr = (byte*)(bmpData.Scan0);
                    for (int j = 0; j < bmpData.Height; j++)
                    {
                        for (int i = 0; i < bmpData.Width; i++)
                        {
                            for (int k = 0; k < LayerNumber; k++)
                            {
                                tmpDN = *(imgPtr + k);
                                imgData[i, j, k] = newDN[tmpDN];
                            }
                            imgPtr += LayerNumber;
                        }
                        imgPtr += byteOfSkip;
                    }
                }
                myBitmap.UnlockBits(bmpData);
            }

            return imgData;
        }
        //轉換Array為Bitmap
        public static Bitmap Array2BitmapModel2(byte[,,] imgData, PixelFormat format)
        {
            int Width = imgData.GetLength(0);
            int Height = imgData.GetLength(1);
            Bitmap myBitmap = new Bitmap(Width, Height, format);
            BitmapData bmpData = myBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, format);
            ColorPalette tempPalette;
            int LayerNumber = 0;

            //判斷24位元彩色影像(R,G,B)
            if (format == PixelFormat.Format24bppRgb)
            {
                LayerNumber = 3;
            }

            //判斷8位元灰階影像
            if (format == PixelFormat.Format8bppIndexed)
            {
                LayerNumber = 1;
                using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                {
                    tempPalette = tempBmp.Palette;
                }
                for (int i = 0; i < 256; i++)
                {
                    tempPalette.Entries[i] = Color.FromArgb(i, i, i);
                }
                myBitmap.Palette = tempPalette;
            }

            int byteOfSkip = bmpData.Stride - bmpData.Width * LayerNumber;
            unsafe
            {
                byte* imgPtr = (byte*)(bmpData.Scan0);
                for (int j = 0; j < bmpData.Height; j++)
                {
                    for (int i = 0; i < bmpData.Width; i++)
                    {
                        for (int k = 0; k < LayerNumber; k++)
                        {
                            *(imgPtr + k) = imgData[i, j, k];
                        }
                        imgPtr += LayerNumber;
                    }
                    imgPtr += byteOfSkip;
                }
            }

            myBitmap.UnlockBits(bmpData);
            return myBitmap;
        }
        internal static Bitmap Array2Bitmap(uint[,] img2)
        {
            throw new NotImplementedException();
        }
    }
    class DatumTrans
    {
        const double a = 6378137;
        const double b = 6356752.314245;
        public static Point latlon2TWD97(double phi, double lamda)
        {
            const double m0 = 0.9999;
            const double lon0 = 121 / 180.0 * Math.PI; //Taiwan
            const double dx = 250000;
            const double dy = 0;

            double lat = phi / 180.0 * Math.PI;
            double lon = lamda / 180.0 * Math.PI;

            double e1 = Math.Sqrt(1 - (b * b) / (a * a));
            double e2 = Math.Pow(e1, 2) / (1 - Math.Pow(e1, 2));
            double N = a / Math.Sqrt(1 - Math.Pow(e1 * Math.Sin(lat), 2));
            double G = a * ((1 - Math.Pow(e1, 2) / 4.0 - 3 * Math.Pow(e1, 4) / 64.0 - 5 * Math.Pow(e1, 6) / 256.0) * lat
                - (3 * Math.Pow(e1, 2) / 8.0 + 3 * Math.Pow(e1, 4) / 32.0 + 45 * Math.Pow(e1, 6) / 1024.0) * Math.Sin(2 * lat)
                + (15 * Math.Pow(e1, 4) / 256.0 + 45 * Math.Pow(e1, 6) / 1024.0) * Math.Sin(4 * lat)
                - (35 * Math.Pow(e1, 6) / 3072) * Math.Sin(6 * lat));
            double T = Math.Pow(Math.Tan(lat), 2);
            double C = Math.Pow(e2 * Math.Cos(lat), 2);
            double A = (lon - lon0) * Math.Cos(lat);

            double x = dx + m0 * N * (A + (1 - T + C) * Math.Pow(A, 3) / 6 + (5 - 18 * T + Math.Pow(T, 2) + 72 * C - 58 * Math.Pow(e2, 2)) * Math.Pow(A, 5) / 120);
            double y = dy + m0 * (G + N * Math.Tan(lat) * (Math.Pow(A, 2) / 2 + (5 - T + 9 * C + 4 * C * C) * Math.Pow(A, 4) / 24 + (61 - 58 * T + T * T + 600 * C - 300 * e2 * e2) * Math.Pow(A, 6) / 720));

            Point TWD97 = new Point();
            TWD97.X = (int)(Math.Round(x, 0));
            TWD97.Y = (int)(Math.Round(y, 0));
            return TWD97;
        }
        public static Point latlon2UTM(double phi, double lamda, int Zone)
        {
            const double m0 = 0.9996;
            const double dx = 500000;
            const double dy = 0;

            double lat = phi / 180.0 * Math.PI;
            double lon = lamda / 180.0 * Math.PI;
            double lon0 = (-180 + Zone * 6 - 3) / 180.0 * Math.PI;

            double e1 = Math.Sqrt(1 - (b * b) / (a * a));
            double e2 = Math.Pow(e1, 2) / (1 - Math.Pow(e1, 2));
            double N = a / Math.Sqrt(1 - Math.Pow(e1 * Math.Sin(lat), 2));
            double G = a * ((1 - Math.Pow(e1, 2) / 4.0 - 3 * Math.Pow(e1, 4) / 64.0 - 5 * Math.Pow(e1, 6) / 256.0) * lat
                - (3 * Math.Pow(e1, 2) / 8.0 + 3 * Math.Pow(e1, 4) / 32.0 + 45 * Math.Pow(e1, 6) / 1024.0) * Math.Sin(2 * lat)
                + (15 * Math.Pow(e1, 4) / 256.0 + 45 * Math.Pow(e1, 6) / 1024.0) * Math.Sin(4 * lat)
                - (35 * Math.Pow(e1, 6) / 3072) * Math.Sin(6 * lat));
            double T = Math.Pow(Math.Tan(lat), 2);
            double C = Math.Pow(e2 * Math.Cos(lat), 2);
            double A = (lon - lon0) * Math.Cos(lat);

            double x = dx + m0 * N * (A + (1 - T + C) * Math.Pow(A, 3) / 6 + (5 - 18 * T + Math.Pow(T, 2) + 72 * C - 58 * Math.Pow(e2, 2)) * Math.Pow(A, 5) / 120);
            double y = dy + m0 * (G + N * Math.Tan(lat) * (Math.Pow(A, 2) / 2 + (5 - T + 9 * C + 4 * C * C) * Math.Pow(A, 4) / 24 + (61 - 58 * T + T * T + 600 * C - 300 * e2 * e2) * Math.Pow(A, 6) / 720));

            Point UTM = new Point();
            UTM.X = (int)(100 * Math.Round(x / 100.0, 0));
            UTM.Y = (int)(100 * Math.Round(y / 100.0, 0));
            return UTM;
        }
        public static double[] TWD972latlon(double x, double y)
        {
            const double x0 = 250000;
            const double y0 = 0;
            const double m0 = 0.9999;

            double lon0 = 121 / 180.0 * Math.PI; //Taiwan
            double e0 = Math.Sqrt(1 - (b * b) / (a * a));
            double mu = (y - y0) / (a * m0 * (1 - Math.Pow(e0, 2) / 4 - 3 * Math.Pow(e0, 4) / 64 - 5 * Math.Pow(e0, 6) / 256));
            double e1 = (1 - Math.Sqrt(1 - Math.Pow(e0, 2))) / (1 + Math.Sqrt(1 - Math.Pow(e0, 2)));
            double phi1 = mu + (3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32) * Math.Sin(2 * mu)
                + (21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32) * Math.Sin(4 * mu)
                + (151 * Math.Pow(e1, 3) / 96) * Math.Sin(6 * mu)
                + (1097 * Math.Pow(e1, 4) / 512) * Math.Sin(8 * mu);
            double ep = Math.Sqrt(Math.Pow(e0, 2) / (1 - Math.Pow(e0, 2)));
            double N1 = a / Math.Sqrt(1 - Math.Pow(e0 * Math.Sin(phi1), 2));
            double T1 = Math.Pow(Math.Tan(phi1), 2);
            double C1 = Math.Pow(ep * Math.Cos(phi1), 2);
            double M1 = a * (1 - Math.Pow(e0, 2)) / Math.Pow((1 - Math.Pow(e0 * Math.Sin(phi1), 2)), 1.5);
            double D = (x - x0) / (N1 * m0);

            double lat = (phi1 - (N1 * Math.Tan(phi1) / M1) * (D * D / 2 - (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * ep * ep) * Math.Pow(D, 4) / 24
                + (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * ep * ep - 3 * C1 * C1) * Math.Pow(D, 6) / 720)) / Math.PI * 180;
            double lon = (lon0 + (D - (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6 + (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * ep * ep + 24 * T1 * T1) * Math.Pow(D, 5) / 120) / Math.Cos(phi1)) / Math.PI * 180;
            double[] TWD97 = new double[] { lat, lon };
            return TWD97;
        }
        public static double[] UTM2latlon(double x, double y, int Zone)
        {
            const double x0 = 500000;
            const double y0 = 0;
            const double m0 = 0.9996;

            double lon0 = (-180 + Zone * 6 - 3) / 180.0 * Math.PI;
            double e0 = Math.Sqrt(1 - (b * b) / (a * a));
            double mu = (y - y0) / (a * m0 * (1 - Math.Pow(e0, 2) / 4 - 3 * Math.Pow(e0, 4) / 64 - 5 * Math.Pow(e0, 6) / 256));
            double e1 = (1 - Math.Sqrt(1 - Math.Pow(e0, 2))) / (1 + Math.Sqrt(1 - Math.Pow(e0, 2)));
            double phi1 = mu + (3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32) * Math.Sin(2 * mu)
                + (21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32) * Math.Sin(4 * mu)
                + (151 * Math.Pow(e1, 3) / 96) * Math.Sin(6 * mu)
                + (1097 * Math.Pow(e1, 4) / 512) * Math.Sin(8 * mu);
            double ep = Math.Sqrt(Math.Pow(e0, 2) / (1 - Math.Pow(e0, 2)));
            double N1 = a / Math.Sqrt(1 - Math.Pow(e0 * Math.Sin(phi1), 2));
            double T1 = Math.Pow(Math.Tan(phi1), 2);
            double C1 = Math.Pow(ep * Math.Cos(phi1), 2);
            double M1 = a * (1 - Math.Pow(e0, 2)) / Math.Pow((1 - Math.Pow(e0 * Math.Sin(phi1), 2)), 1.5);
            double D = (x - x0) / (N1 * m0);

            double lat = (phi1 - (N1 * Math.Tan(phi1) / M1) * (D * D / 2 - (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * ep * ep) * Math.Pow(D, 4) / 24
                + (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * ep * ep - 3 * C1 * C1) * Math.Pow(D, 6) / 720)) / Math.PI * 180;
            double lon = (lon0 + (D - (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6 + (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * ep * ep + 24 * T1 * T1) * Math.Pow(D, 5) / 120) / Math.Cos(phi1)) / Math.PI * 180;
            double[] UTM = new double[] { lat, lon };
            return UTM;
        }
    }
    class CanonicalCorrelationAnaysis
    {
        public static double[] CCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
        public static double[] IRCCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
        public static double[] KCCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
        public static double[] IRKCCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
        public static double[] GCCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
        public static double[] IRGCCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
        public static double[] KGCCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
        public static double[] IRKGCCA(byte inputByte)
        {

            double[] Double = new double[1000];
            return Double;
        }
    }
    
}