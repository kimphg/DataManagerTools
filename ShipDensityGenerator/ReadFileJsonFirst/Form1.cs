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
            int numThread = 1;
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
            
            for (int i = hashcode; i < ships.Count; i += numThread)
            {

                processShip(ships[i]);
                countShips++;
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

                //HandlingCoordinates.checkCoordinate(lat1,lon1);
                mutex.WaitOne();
                if (distance < 4 && distance > 0.1 && turnAngle < 15)
                {

                    HandlingCoordinates.AddLine(lat1, lon1, lat2, lon2);

                }
                else if (distance < 2 && distance > 0.1 && turnAngle < 30)
                {

                    HandlingCoordinates.AddLine(lat1, lon1, lat2, lon2);

                }
                else HandlingCoordinates.AddPoint(lat1, lon1);
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
                double timeSec = DateTime.UtcNow.Subtract(timeStart).TotalSeconds + 1;
                label3.Text = countShips.ToString() + "/" + progress.Maximum.ToString() + 
                    " Time:" + (timeSec.ToString("0.##")) + 
                    " Ships per sec:" + (countShips / timeSec).ToString("0.##");
            }
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
                List<int> removeLat = new List<int>();
                
                foreach (var plat in HandlingCoordinates.densityMap)
                {
                    List<int> removeLon = new List<int>();
                    foreach (var plon in plat.Value)
                    {
                        if (plon.Value < 2)
                        {
                            //HandlingCoordinates.densityMap[plat.Key].Remove(plon.Key);
                            
                            removeLon.Add(plon.Key);
                        }

                    }
                    for (int i = 0; i < removeLon.Count(); i++)
                    {
                        HandlingCoordinates.densityMap[plat.Key].Remove(removeLon[i]);
                    }
                    if(HandlingCoordinates.densityMap[plat.Key].Count()==0) removeLat.Add(plat.Key);

                }
                for (int i = 0; i < removeLat.Count(); i++)
                {
                    HandlingCoordinates.densityMap.Remove(removeLat[i]);
                }
                
                    MessageBox.Show("Xong");
               

            }
        }

        private void saveDictionaryToFile()
        {
            JsonTools.writeFile( txt_fd_density.Text +"D:\\Phuong\\density.json", JsonConvert.SerializeObject(HandlingCoordinates.densityMap, Formatting.Indented));
            
        }

        private void txt_fd_density_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveDictionaryToFile();
        }
    }
}
