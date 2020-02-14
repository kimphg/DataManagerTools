using ReadFileJsonFirst.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Security;
using System.Threading;

namespace ReadFileJsonFirst
{
    public partial class Form1 : Form
    {
        DateTime timeStart;
        List<Thread> threads = new List<Thread>();
        Mutex mutex = new Mutex();
        Random random = new Random();
        Dictionary<Int32, Dictionary<Int32, Int32>> densityMap = new Dictionary<Int32, Dictionary<Int32, Int32>>();

        private string strSource;
        private int countProcessedFiles;
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 2000;
            
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
        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                strSource = txt_pathfd.Text;

                progress.Maximum = 100;
                progress.Value = 0;
                List<string> files = new List<string> ();
                DirectorySearchFile(strSource, files);

                progress.Maximum = files.Count;

                Thread t = new Thread(new ThreadStart(() => createThread(files)));
                t.Start();
                timer1.Start();
                timeStart = DateTime.UtcNow;
                countProcessedFiles = 0;

            } catch (Exception )
            {
                MessageBox.Show("Error ! Reset application");
            }
           
        }

        private void createThread(List<string> files )
        {
            int numThread = 16;
            // txt_numThread.Text = numThread.ToString();
            //tao ra bang 16 luong
            
         
           //progress.Maximum = files.Count;

            for (int i = 1; i <= numThread; i++)
            {
                Thread t = new Thread(new ThreadStart(() => setThreadToFile(i, files, numThread)));
                t.Start();
                threads.Add(t);

            }

            for (int i = 0; i < numThread; i++)
            {

                threads[i].Join();

            }

        }
        
        private void setThreadToFile(int hashcode, List<string> files, int numThread)
        {
            
            for (int i = hashcode; i <= files.Count; i += numThread)
            {
                
                handlingFile(files[i - 1]);
            }
        }
        private void handlingFile(string filename )
        {
            
            mutex.WaitOne();
            HandlingCoordinates handlingCoordinates = new HandlingCoordinates(filename);
            handlingCoordinates.checkCoordinate(densityMap);
            countProcessedFiles++;
            mutex.ReleaseMutex();
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
            if (countProcessedFiles < progress.Maximum)
            {
                progress.Value = countProcessedFiles;
                double timeSec = DateTime.UtcNow.Subtract(timeStart).TotalSeconds + 1;
                label3.Text = countProcessedFiles.ToString() + "/" + progress.Maximum.ToString() + 
                    " time:" + (timeSec.ToString("0.##")) + 
                    " Files per sec:" + (countProcessedFiles / timeSec).ToString("0.##");
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
                
                MessageBox.Show("Xong");
               

            }
        }

        private void saveDictionaryToFile()
        {
            JsonTools.writeFile( txt_fd_density.Text +"\\density.json", JsonConvert.SerializeObject(densityMap, Formatting.Indented));
            
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
