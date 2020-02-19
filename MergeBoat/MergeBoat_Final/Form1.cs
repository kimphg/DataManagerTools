using MergeBoat_Final.Object;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MergeBoat_Final
{
    public partial class Form1 : Form
    {
        string sourcePath = "";
        List<String> folders = new List<string>();
        List<Thread> threads = new List<Thread>();
        Dictionary<string, Boat> ListBoat = new Dictionary<string, Boat>();
        Mutex mutex = new Mutex();
        Random rand = new Random();
        public Form1()
        {
            InitializeComponent();
            timer_thread.Interval = 1000;
           
        }
        private void DirectorySearchFolder(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    folders.Add(d);
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
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
      //  CommonOpenFileDialog ofd = new CommonOpenFileDialog();
        private void btn_sf_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txt_pathfd.Text = fbd.SelectedPath;
                    sourcePath = fbd.SelectedPath;
                    getInfo();
                    MessageBox.Show("Thành công !");
                }
            }

            //ofd.InitialDirectory = Application.ExecutablePath;
            //ofd.IsFolderPicker = true;
            //ofd.RestoreDirectory = false;
            //if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            //{
            //    try
            //    {
            //        txt_pathfd.Text = ofd.FileName;
            //        sourcePath = ofd.FileName;

            //        getInfo();
            //        MessageBox.Show("Thành công !");
            //    }
            //    catch (SecurityException ex)
            //    {
            //        MessageBox.Show(@"Security error.\n\nError message: {ex.Message}\n\n" +
            //        @"Details:\n\n{ex.StackTrace}");
            //    }
            //}
        }
        public void getInfo()
        {
            try
            {
                folders.Clear();
                DirectorySearchFolder(sourcePath);
                if (folders.Count == 0)
                {
                    folders.Add(sourcePath);
                }
                txt_numFoder.Text = folders.Count.ToString();
                //List<string> a = new List<string>();
                //DirectorySearchFile(sourcePath, a);
                //txt_numFile.Text = a.Count.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void btn_startMerge_Click(object sender, EventArgs e)
        {
            try
            {
                //su ly khi tao luong
                //tao luong moi de xu ly
                progressMeger.Maximum = Convert.ToInt32(txt_numFoder.Text);
                progressMeger.Value = 0;
                string temp = folders.ElementAt(0);
                Thread t = new Thread(new ThreadStart(() => createThread(temp)));
                t.Start();
                folders.RemoveAt(0);
                timer_thread.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Chọn file");
            }

            

        }

        private void createThread(string path_folder)
        {
            dem++;
            int numThread = 16;
           // txt_numThread.Text = numThread.ToString();
            //tao ra bang 16 luong
            List<string> files = new List<string>();
            DirectorySearchFile(path_folder, files);
            //progressBar1.Maximum = files.Count;
      
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
        int dem = 0;
        private void setThreadToFile(int hashcode, List<string> files, int numThread)
        {
            
            for (int i = hashcode; i <= files.Count; i += numThread)
            {
                handlingFile(files[i - 1]);
            }
        }
        //xu ly file
        private void handlingFile(string filename)
        {
            
            Boat boat = new Boat();
            boat = JsonToolBoat.readFileJson(filename);
            mutex.WaitOne();
            if (checkMmsi(boat.Mmsi))
            {
                if(checkDate(boat))
                    addCoordinateToBoat(boat);
            }
            else 
            {
                ListBoat.Add(boat.Mmsi, boat);
            }
            mutex.ReleaseMutex();
        }
        //cap nhat lai data khi thread chay
        private void updateProgress()
        {
            txt_numFile.Text = dem.ToString();
            if (dem < progressMeger.Maximum) progressMeger.Value = dem;
            else progressMeger.Value = progressMeger.Maximum; 
        }
        private void timer_thread_Tick(object sender, EventArgs e)
        {
            bool taskDone = true;
            foreach (Thread th in threads)
            {
                if (th.IsAlive) taskDone = false;
            }
            if (folders.Count > 0 && taskDone)
            {
                
                createThread(folders.ElementAt(0));
                folders.RemoveAt(0);
            }
            if (folders.Count == 0 && taskDone)
            {
                timer_thread.Stop();
                MessageBox.Show("Xong","Thông báo ");
              
            }

            updateProgress();
        }
        //them toa do cho tau da co san
        private void addCoordinateToBoat(Boat boat)
        {
            Boat boatInDistionary = new Boat();
            boatInDistionary = ListBoat[boat.Mmsi];
            boatInDistionary.ListCoor.Add(boat.ListCoor[0]);
            ListBoat[boat.Mmsi] = boatInDistionary;
        }
        //kiểm tra thời gian
        private bool checkDate(Boat boat)
        {
            Boat boatInDistionary = new Boat();
            boatInDistionary = ListBoat[boat.Mmsi];
            foreach(Coordinate item in boatInDistionary.ListCoor)
                if (item.time == boat.ListCoor[0].time) return false;
            return true;
        }
        //kiem tra mmsi
        private bool checkMmsi(String mmsi)
        {
            if (ListBoat.ContainsKey(mmsi)) return true;
            return false;
        }
        int numprogressSaveFile = 0;
        private void btn_save_Click(object sender, EventArgs e)
        {
            if (txt_fd_sf.Text != "")
            {
                try
                {
                   // timer_thread.Stop();
                    progressSaveFile.Maximum = ListBoat.Count;
                    progressSaveFile.Value = 0;
                    string pathSaveFile = txt_fd_sf.Text;
                    Thread thread = new Thread(new ThreadStart(() => writeFile(ListBoat, pathSaveFile)));
                    thread.Start();
                    timer1.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error");
                }
            }
        }
        private void writeFile(Dictionary<string,Boat> list,string pathSaveFile)
        {
            
            foreach (var item in list)
            {
                mutex.WaitOne();
                JsonToolBoat.writeFileJson(pathSaveFile+ "\\" + item.Key + ".json", item.Value);
                numprogressSaveFile++;
                mutex.ReleaseMutex();
            }
           
            MessageBox.Show("Hoàn Thành");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            updateProgressSaveFile();
        }
        private void updateProgressSaveFile()
        {
           
            progressSaveFile.Value = numprogressSaveFile;
        }
        protected override void OnClosed(EventArgs e)
        {
            
            base.OnClosed(e);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btn_openfd_sf_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txt_fd_sf.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
