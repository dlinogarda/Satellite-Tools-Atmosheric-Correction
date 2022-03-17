using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SateliteTool;
using System.Drawing.Imaging;
using System.IO;
using MathNet.Numerics.LinearAlgebra;


namespace CrossSensor
{
    public partial class CrossSensorGUI : Form
    {
        // Variables declarations
        public int MetaDms;
        public byte[,] L7;
        public UInt16[,] L8;

        public string[] MetaBatch;
        public string[] FolderNames;
        public string[] StringL7name;
        public string[] StringL8name;

        private int GV3ColumnIndex;
        private int GV3RowsIndex;
        private int listBox1Selected;
        public CrossSensorGUI()
        {
            InitializeComponent();
        }
        
        List<SateliteImage> L7Barrack = new List<SateliteImage>();
        List<SateliteImage> L8Barrack = new List<SateliteImage>();
        //
        //SateliteImage[] Pair = new SateliteImage[10];
        //List<SateliteImage> another = new List<SateliteImage>();
        //Pair.RemoveAt(9);
        //Pair.toa
        //another.Add(Data);
        private void button1_Click(object sender, EventArgs e)
        {
            List<SateliteImage> Pair = new List<SateliteImage>();
            // Reading metadata
            openFileDialog1.Title = "Select your Metadata files";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Text|*.txt|All|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MetaBatch = (openFileDialog1.FileNames);
                MetaDms = MetaBatch.Length;
                FolderNames = new string[MetaDms];
                // Show browser location
                string DIR = Path.GetDirectoryName(openFileDialog1.FileName);
                textBox1.Text = "File Location : " + DIR;
                for (int i=0; i<MetaDms;i++)
                {
                    SateliteImage Data = new SateliteImage();
                    //GetFolderName
                    string fnn = MetaBatch[i].Replace(@"_MTL.txt", "");
                    string[] Fold = fnn.Split('\\');
                    FolderNames[i] = Fold[Fold.Length - 1];
                    //Getting image barrack
                    Data.LoadInfo(MetaBatch[i]);
                    Pair.Add(Data);
                }
            }
            SateliteImage.Register_Intersect(Pair);
            // File name for table
            {
                int m = 0;
                int n = 0;
                StringL7name = new string[Pair.Count/2];
                StringL8name = new string[Pair.Count/2];
                for (int i = 0; i < Pair.Count; i++)
                    {
                    if (Pair[i].SceneID.Contains("LE7"))
                        {
                            L7Barrack.Add(Pair[i]);
                            StringL7name[m] = FolderNames[i];
                            m++;
                        }
                    else if (Pair[i].SceneID.Contains("LC8"))
                        {
                            L8Barrack.Add(Pair[i]);
                            StringL8name[n] = FolderNames[i];
                            n++;
                        }
                    }
            }
            // Show to the dataGridView1
            for (int i=0; i<StringL7name.Length; i++)
            {
                DataGridViewRowCollection row = dataGridView1.Rows;
                dataGridView1.Rows.Add(StringL7name[i], StringL8name[i]);
            }
        }
        private void dataGridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1Selected = listBox1.SelectedIndex;
            GV3ColumnIndex = dataGridView1.CurrentCell.ColumnIndex;
            GV3RowsIndex = dataGridView1.CurrentCell.RowIndex;
            byte[] ll7 = new byte[L7Barrack[0].Hgt * L7Barrack[0].Wdt];
            byte[] ll7b1 = new byte[L7Barrack[0].Hgt * L7Barrack[0].Wdt];
            byte[] ll7b2 = new byte[L7Barrack[0].Hgt * L7Barrack[0].Wdt];
            byte[] ll7b3 = new byte[L7Barrack[0].Hgt * L7Barrack[0].Wdt];
            UInt16[] ll8 = new UInt16[L8Barrack[0].Hgt * L8Barrack[0].Wdt];
            UInt16[] ll8b1 = new UInt16[L8Barrack[0].Hgt * L8Barrack[0].Wdt];
            UInt16[] ll8b2 = new UInt16[L8Barrack[0].Hgt * L8Barrack[0].Wdt];
            UInt16[] ll8b3 = new UInt16[L8Barrack[0].Hgt * L8Barrack[0].Wdt];
            if (listBox1Selected != -1) //|| listBox1Selected != L7Barrack.Count
            {
                if (listBox1Selected > 0 || listBox1Selected < 6)
                {
                    if (GV3ColumnIndex == 0)
                    {
                        ll7 = L7Barrack[GV3RowsIndex].ReadBand(listBox1Selected);
                        pictureBox1.Image = BitmapTool.Array2Bitmap(ll7, L7Barrack[0].Hgt, L7Barrack[0].Wdt);
                    }
                    else if (GV3ColumnIndex == 1)
                    {
                        ll8 = L8Barrack[GV3RowsIndex].ReadBand16(listBox1Selected + 1);
                        pictureBox1.Image = BitmapTool.Array2Bitmap(ll8, L8Barrack[0].Hgt, L8Barrack[0].Wdt);
                    }
                    groupBox2.Text = "Original Gray Image";
                }
                if (listBox1Selected == 6) //|| listBox1Selected != L7Barrack.Count
                {
                    if (GV3ColumnIndex == 0)
                    {
                        ll7b1 = L7Barrack[GV3RowsIndex].ReadBand(0);
                        ll7b2 = L7Barrack[GV3RowsIndex].ReadBand(1);
                        ll7b3 = L7Barrack[GV3RowsIndex].ReadBand(2);
                        pictureBox1.Image = BitmapTool.Array2Bitmap(ll7b1, ll7b2, ll7b3, L7Barrack[0].Hgt, L7Barrack[0].Wdt);
                    }
                    else if (GV3ColumnIndex == 1)
                    {
                        ll8b1 = L8Barrack[GV3RowsIndex].ReadBand16(1);
                        ll8b2 = L8Barrack[GV3RowsIndex].ReadBand16(2);
                        ll8b3 = L8Barrack[GV3RowsIndex].ReadBand16(3);
                        pictureBox1.Image = BitmapTool.Array2Bitmap(ll8b1, ll8b2, ll8b3, L8Barrack[0].Hgt, L7Barrack[0].Wdt);
                    }
                    groupBox2.Text = "Original Color Image";
                }
            }
                
        }
        private void dataGridView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void openMetaDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<SateliteImage> Pair = new List<SateliteImage>();
            // Reading metadata
            openFileDialog1.Title = "Select your Metadata files";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Text|*.txt|All|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MetaBatch = (openFileDialog1.FileNames);
                MetaDms = MetaBatch.Length;
                FolderNames = new string[MetaDms];
                // Show browser location
                string DIR = Path.GetDirectoryName(openFileDialog1.FileName);
                textBox1.Text = "File Location : " + DIR;
                for (int i = 0; i < MetaDms; i++)
                {
                    SateliteImage Data = new SateliteImage();
                    //GetFolderName
                    string fnn = MetaBatch[i].Replace(@"_MTL.txt", "");
                    string[] Fold = fnn.Split('\\');
                    FolderNames[i] = Fold[Fold.Length - 1];
                    //Getting image barrack
                    Data.LoadInfo(MetaBatch[i]);
                    Pair.Add(Data);
                }
            }
            SateliteImage.Register_Intersect(Pair);
            // File name for table
            {
                int m = 0;
                int n = 0;
                StringL7name = new string[Pair.Count / 2];
                StringL8name = new string[Pair.Count / 2];
                for (int i = 0; i < Pair.Count; i++)
                {
                    if (Pair[i].SceneID.Contains("LE7"))
                    {
                        L7Barrack.Add(Pair[i]);
                        StringL7name[m] = FolderNames[i];
                        m++;
                    }
                    else if (Pair[i].SceneID.Contains("LC8"))
                    {
                        L8Barrack.Add(Pair[i]);
                        StringL8name[n] = FolderNames[i];
                        n++;
                    }
                }
            }
            // Show to the dataGridView1
            for (int i = 0; i < StringL7name.Length; i++)
            {
                DataGridViewRowCollection row = dataGridView1.Rows;
                dataGridView1.Rows.Add(StringL7name[i], StringL8name[i]);
            }
        }
    }
}
