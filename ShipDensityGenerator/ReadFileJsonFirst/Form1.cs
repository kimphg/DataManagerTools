using ReadFileJsonFirst.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ReadFileJsonFirst
{
    public partial class Form1 : Form
    {

        DateTime timeStart;
        List<Thread> threads = new List<Thread>();
        Mutex mutex = new Mutex();
        Random random = new Random();
        //Dictionary<Int32, Dictionary<Int32, Int32>> densityMap = new Dictionary<Int32, Dictionary<Int32, Int32>>();
        public static string connectionString = @"Data Source=WIN-CS49MK82IQN\SQLEXPRESS;Initial Catalog=seamap;Integrated Security=True";
        //private string strSource;
        private int countShips;
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 2000;
            HandlingCoordinates.nameFileSource = "density.txt";
          //  test();
        }
       
        //mở chọn folder lấy file tàu
        private void btn_sf_Click(object sender, EventArgs e)
        {

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = @"D:\";

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txt_pathfd.Text = fbd.SelectedPath;
                }
            }



        }

        //mở chọn folder để lưu dữ liệu mật độ
        private void btn_openfd_density_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = @"D:\";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txt_fd_density.Text = fbd.SelectedPath;
                }
            }
        }

        //click xử lý dữ liệu 
        /*
         Quy trình chọn file mật độ -> chọn file lưu -> kiểm tra từng toạ độ 1
             */
        public List<string> getShipList()
        {
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($"SELECT MMSI FROM SHIP_LIST", connectionString))
            {
                adapter.Fill(table);
            };
            List<string> list = table.AsEnumerable()
                                   .Select(r => r.Field<string>("MMSI"))
                                   .ToList();
            return list;
        }
        private DataTable getShipData(string mmsi)
        {
            try
            {
                var table = new DataTable();
                using (var adapter = new SqlDataAdapter($" select [LAT],[LNG],[SOG],[COG],[TIME] from SHIPJOURNEY where SOG > 1 and MMSI like " + mmsi, connectionString))
                {
                    
                    adapter.Fill(table);
                };
                return table;
            }
            catch (Exception e)
            {
                //retry
                try
                {
                    var table = new DataTable();
                    using (var adapter = new SqlDataAdapter($" select [LAT],[LNG],[SOG],[COG],[TIME] from SHIPJOURNEY where SOG > 1 and MMSI like " + mmsi, connectionString))
                    {
                        adapter.Fill(table);
                    };
                    return table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
            }
        }
        public List<string> listShips;
        private void btn_start_Click(object sender, EventArgs e)
        {
            listShips = getShipList();
            //DataTable data = getAll();
            
            try
            {
                progress.Value = 0;
                progress.Maximum = listShips.Count;
                countShips = 0;
                Thread t = new Thread(new ThreadStart(() => createThread(listShips)));
                t.Start();
                timer1.Start();
                timeStart = DateTime.UtcNow;
                

            } catch (Exception )
            {
                MessageBox.Show("Error ! Reset application");
            }

        }

        private void createThread(List<string> ships )
        {
            int numThread = 16;
            // txt_numThread.Text = numThread.ToString();
            //tao ra bang 16 luong
            
         
           //progress.Maximum = files.Count;

            for (int i = 1; i <= numThread; i++)
            {
                Thread t = new Thread(new ThreadStart(() => setThreadToFile(i, ships, numThread)));
                t.Start();
                threads.Add(t);

            }

            for (int i = 0; i < numThread; i++)
            {

                threads[i].Join();

            }

        }
        
        private void setThreadToFile(int hashcode, List<string> ships , int numThread)
        {
            
            for (int i = hashcode; i < ships.Count; i += numThread)//ships.Count
            {
                //fillShipList(i.ToString());
                processShip(ships[i]);
                //processShip(i.ToString());
                countShips++;
            }
        }
        bool checkShipExist(String mmsi)
        {
            var table = new DataTable();
            using (var adapter = new SqlDataAdapter($" select COUNT(1) from SHIPJOURNEY where MMSI like " + mmsi, connectionString))
            {

                adapter.Fill(table);
            };
            return table.Rows.Count>0;
        }
        void fillShipList(string mmsi)
        {
            if (checkShipExist(mmsi) == false) return;
            bool shipExist = false;
            foreach (String ship in listShips)
            {
                if (Int32.Parse(ship) == Int32.Parse(mmsi))
                {
                    shipExist = true;
                    break;
                }
            }
            if (shipExist == false)
            {
                try
                {
                    string query = "INSERT INTO SHIP_LIST (MMSI,VSNM,TYPE,CLASS,LAT,LON,SOG,COG,TIME)";
                    query += " VALUES (@mmsi,@vsnm,@type,@class,@lat,@lon,@sog,@cog,@time)";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@mmsi", mmsi);
                            command.Parameters.AddWithValue("@vsnm", "xxxx");
                            command.Parameters.AddWithValue("@type", "0");
                            command.Parameters.AddWithValue("@class", "0");
                            command.Parameters.AddWithValue("@lat", "0");
                            command.Parameters.AddWithValue("@lon", "0");
                            command.Parameters.AddWithValue("@sog", "0");
                            command.Parameters.AddWithValue("@cog", "0");//(long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                            command.Parameters.AddWithValue("@time", (long)DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds);
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    String ex = e.ToString();
                    
                }
            }
        }
        private void processShip(string mmsi)
        {
            DataTable data =  getShipData(mmsi);
            if (data == null) return;
            
            int numOfRows = data.Rows.Count;
            if (numOfRows < 2) return;
            for (int i = 0;i< numOfRows - 1;i++)
            {
                DataRow row1 = data.Rows[i];
                DataRow row2 = data.Rows[i+1];
                double lat1 = (double)row1[0];
                double lat2 = (double)row2[0];
                double lon1 = (double)row1[1];
                double lon2 = (double)row2[1];
                double cog1 = (double)row1[3];
                double cog2 = (double)row2[3];
                //lat,long,sog,cog,time
                //double lat1 = (double)row1.ItemArray[0];

                double distance = GeoOperations.DistanceTo(lat1,lon1,lat2,lon2);
                double turnAngle = Math.Abs(cog1 - cog2);
                if (turnAngle > 180) turnAngle -= 180;
                if (cog1 == 0 && cog2 == 0) continue;
                    //HandlingCoordinates.checkCoordinate(lat1,lon1);
                mutex.WaitOne();
                
                {
                    if (distance < 4 && distance > 0.1 && turnAngle < 15)
                    {

                        HandlingCoordinates.AddLine(lat1, lon1, lat2, lon2);

                    }
                    else if (distance < 2 && distance > 0.1 && turnAngle < 30)
                    {

                        HandlingCoordinates.AddLine(lat1, lon1, lat2, lon2);

                    }
                    else HandlingCoordinates.AddPoint(lat1, lon1);
                }
                mutex.ReleaseMutex();



            }
            
            
        }
        private void DirectorySearchFile(string sDir, List<string> files)
        {
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirectorySearchFile(d, files);
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        private void progress_Click(object sender, EventArgs e)
        {
           
        }
        private void updateProgress()
        {
            if (countShips < progress.Maximum)
            {
                progress.Value = countShips;
                
                
            }
            double timeSec = DateTime.UtcNow.Subtract(timeStart).TotalSeconds + 1;
            label3.Text = countShips.ToString() + "/" + progress.Maximum.ToString() +
                    " Time:" + (timeSec.ToString("0.##")) +
                    " Ships per sec:" + (countShips / timeSec).ToString("0.##");
        }
        private void stop()
        {
            progress.Value = progress.Maximum;
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            bool taskDone = true;
            foreach (Thread th in threads)
            {
                if (th.IsAlive) taskDone = false;
            }
            updateProgress();
            
            if (taskDone)
            {
                timer1.Stop();
                stop();
                optimizeData();
                
                
                MessageBox.Show("Xong");
               

            }
        }

        private void optimizeData()
        {

            /*foreach (var plat in HandlingCoordinates.densityMap)
            {
                int lat = plat.Key;
                foreach (var plon in plat.Value)
                {
                    int lon = plon.Key;

                    //int value = plon.Value;
                    float sumVal = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int dist = i * i + j * j;
                            float k = 0;
                            if (dist >= 3) k = 0.0f;
                            else if(dist >= 2) k = 0.3f;
                            else if(dist >= 1) k = 0.8f;
                            else k = 1;
                            sumVal += k*getValueAt(lat + i, lon + j);
                            
                        }
                    }
                    sumVal /= (float)(1+4*0.8+4*0.3);
                    HandlingCoordinates.densityMap[lat][lon] = (int)(sumVal);

                }
            }*/
            //8 104 22 111/ 14000 8000
            //int imgW = 8000, imgH = 14000;
            //Bitmap bitmapData = new Bitmap(imgW, imgH, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            //BitmapData data = bitmapData.LockBits(new Rectangle(0, 0, bitmapData.Width, bitmapData.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            //byte[] bytes = new byte[data.Height * data.Stride];
            //Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            //for (int i = 0; i < imgW; i ++)
            //    for (int j = 0; j < imgH; j++)
            //    {
            //        int lon = 104000 + i;
            //        int lat = 8000 + j;
            //        int val = (int)(Math.Sqrt(getValueAt(lat, lon))*10);
            //        if (val<2) continue;
            //        if (val > 255) val = 255;
            //        bytes[j * data.Stride + i] = (byte)val;
            //        //Color
            //        //.SetPixel(i, j, val);
            //    }
            //Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            //bitmapData.UnlockBits(data);
           
            //imgW = 8000; imgH = 14000;
            //for (int lat1000 = 8; lat1000 < 22; lat1000++)
            //{
            //    for (int lon1000 = 103; lon1000 < 111; lon1000++)
            //    {
            //        Bitmap bitmapData = new Bitmap(imgW, imgH);
            //        for (int i = 0; i < imgW; i++)
            //            for (int j = 0; j < imgH; j++)
            //            {
            //                int lon = lon1000 * 1000 + i;
            //                int lat = lat1000 * 1000 + j;
            //                double val = getValueAt(lat, lon);
            //                if (val < 2) continue;
            //                int color = (int)(Math.Sqrt(val) * 50);
            //                if (color > 255) color = 255;
            //                bitmapData.SetPixel(i, imgH - j - 1, Color.FromArgb(color, 0, 0, color));
            //            }
            //        bitmapData.Save(dialog.FileName + "_" + lat1000.ToString() + "_" + lon1000.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    }
            //}
            return ;
            
        }

        private int getValueAt(int v1, int v2)
        {
            Dictionary<int, int> a;
            HandlingCoordinates.densityMap.TryGetValue(v1,out a);
            if (a == null) return 0;
            int res=0;
            a.TryGetValue(v2, out res);
            return res;

        }

        private void saveDictionaryToFile()
        {
            string data = JsonConvert.SerializeObject(HandlingCoordinates.densityMap, Formatting.Indented);
            JsonTools.writeFile( txt_fd_density.Text +"D:\\Phuong\\density.json",data );
            
        }

        private void txt_fd_density_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //saveDictionaryToFile();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Png Image|*.png|Bitmap Image|*.bmp|JPeg Image 30|*.jpg|JPeg Image 50|*.jpg";
            if (dialog.ShowDialog() != DialogResult.OK) return;

            int imgW = 1000, imgH = 1000;
            for (int lat1000 = 8; lat1000 < 22; lat1000++)
            {
                for (int lon1000 = 103; lon1000 < 111; lon1000++)
                {
                    Bitmap bitmapData = new Bitmap(imgW, imgH);
                    for (int i = 0; i < imgW; i++)
                        for (int j = 0; j < imgH; j++)
                        {
                            int lon = lon1000 * 1000 + i;
                            int lat = lat1000 * 1000 + j;
                            double val = getValueAt(lat, lon);
                            //if (val < 2) continue;
                            int color = (int)(Math.Sqrt(val) * 50);
                            if (color > 255) color = 255;
                            bitmapData.SetPixel(i, imgH - j - 1, Color.FromArgb(color, 0, 0, 255));
                        }
                    ImageCodecInfo pngCodec;
                    switch (dialog.FilterIndex)
                    {
                        case 1:
                            pngCodec = ImageCodecInfo.GetImageEncoders().Where(codec => codec.FormatID.Equals(ImageFormat.Png.Guid)).FirstOrDefault();
                            if (pngCodec != null)
                            {
                                EncoderParameters parameters = new EncoderParameters();
                                parameters.Param[0] = new EncoderParameter(Encoder.ColorDepth, 8);
                                //myImage.Save(myStream, pngCodec, parameters);
                                bitmapData.Save(dialog.FileName + "_" + lat1000.ToString() + "_" + lon1000.ToString() + ".png", pngCodec, parameters);
                            }
                            break;
                        case 2:
                            pngCodec = ImageCodecInfo.GetImageEncoders().Where(codec => codec.FormatID.Equals(ImageFormat.Bmp.Guid)).FirstOrDefault();
                            if (pngCodec != null)
                            {
                                EncoderParameters parameters = new EncoderParameters(1);
                                parameters.Param[0] = new EncoderParameter(Encoder.ColorDepth, 8);
                                //myImage.Save(myStream, pngCodec, parameters);
                                bitmapData.Save(dialog.FileName + "_" + lat1000.ToString() + "_" + lon1000.ToString() + ".bmp", pngCodec, parameters);
                            }
                            break;
                        case 3:
                            pngCodec = ImageCodecInfo.GetImageEncoders().Where(codec => codec.FormatID.Equals(ImageFormat.Jpeg)).FirstOrDefault();
                            if (pngCodec != null)
                            {
                                EncoderParameters parameters = new EncoderParameters(1);
                                Encoder myEncoder = Encoder.Quality;
                                parameters.Param[0] = new EncoderParameter(myEncoder,30L);
                                //myImage.Save(myStream, pngCodec, parameters);
                                bitmapData.Save(dialog.FileName + "_" + lat1000.ToString() + "_" + lon1000.ToString() + ".jpg", pngCodec, parameters);
                            }
                            break;
                        case 4:
                            pngCodec = ImageCodecInfo.GetImageEncoders().Where(codec => codec.FormatID.Equals(ImageFormat.Jpeg)).FirstOrDefault();
                            if (pngCodec != null)
                            {
                                EncoderParameters parameters = new EncoderParameters(1);
                                Encoder myEncoder = Encoder.Quality;
                                parameters.Param[0] = new EncoderParameter(myEncoder, 50L);
                                //myImage.Save(myStream, pngCodec, parameters);
                                bitmapData.Save(dialog.FileName + "_" + lat1000.ToString() + "_" + lon1000.ToString() + ".jpg", pngCodec, parameters);
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }
}
