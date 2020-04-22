using AISTools.Model;
using AISTools.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AISTools
{
    public partial class frmMain : Form
    {
        DateTime timeBegin;
        System.Timers.Timer timerNow = new System.Timers.Timer();
        System.Timers.Timer timerAutoSave = new System.Timers.Timer();
        Dictionary<string, ShipJourney> dictShipJourney = new Dictionary<string, ShipJourney>();
        int dem = 0;
        ShipModel shipmodel = new ShipModel();
        ShipJourneyModel journeymodel = new ShipJourneyModel();
        Dictionary<string, ShipJourney> dictShipJourneyFirst = new Dictionary<string, ShipJourney>();
        Dictionary<string, ShipJourney> dictShipJourneySecond = new Dictionary<string, ShipJourney>();
        bool modeChooseDictSaveFileFirst;//lua chon dung dict 1 hay dict 2 de luu du lieu
        bool modeChooseDictSaveFileSecond;//lua chon dung dict 1 hay dict 2 de luu du lieu

        public frmMain()
        {
            InitializeComponent();
            setState();
            modeChooseDictSaveFileFirst = true;
            modeChooseDictSaveFileSecond = false;
        }
        private void setState()
        {
            GlobalVar.hashMMSI = shipmodel.getAll();
            //set style of form
            btn_start.TabStop = false;
            btn_start.FlatStyle = FlatStyle.Flat;
            btn_start.FlatAppearance.BorderSize = 0;
            btn_stop.TabStop = false;
            btn_stop.FlatStyle = FlatStyle.Flat;
            btn_stop.FlatAppearance.BorderSize = 0;
            //timer
            timerTask.Interval = GlobalVar.TIME_REQUEST; // set time request to server
        }
        
        private void btn_start_Click(object sender, EventArgs e)
        {
            timeBegin = DateTime.Now;
            txt_status.Text = "True";
            dem++;
            Task task = new Task(sendRequestAsync);
            task.Start();

            timerNow.Interval = GlobalVar.TIME_UPDATE;
            timerNow.Elapsed += timerNow_Tick;
            timerNow.Start();

            timerAutoSave.Interval = GlobalVar.TIME_SAVE;//set time auto save
            timerAutoSave.Elapsed += timerAutoSave_Tick;
            timerAutoSave.Start();

            timerTask.Start();
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            timerNow.Stop();
            timerTask.Stop();
        }

        bool saveAvaiable = true;

        //timer tick save file
        private void timerAutoSave_Tick(object sender, EventArgs e)
        {
            
            /*
                if (dictAvaiable && saveAvaiable)
                {

                    saveAvaiable = false;

                    foreach (var item in dictShipJourney.Values.ToList())
                    {
                        //ShipModel model = new ShipModel();
                        //model.insert(item.Mmsi, item.Vsnm,Convert.ToInt32(item.Type), "B");
                        //foreach(var coor in item.ListCoor)
                        //{
                        //    ShipJourneyModel modelSj = new ShipJourneyModel();
                        //    modelSj.insert(item.Mmsi, coor.lat, coor.lng, "", "", coor.time);

                        //}

                        //check mmsi
                        if (!GlobalVar.hashMMSI.Contains(item.Mmsi))
                        {
                            model.insert(item.Mmsi, item.Vsnm,Convert.ToInt32(item.Type), "B");
                            GlobalVar.hashMMSI.Add(item.Mmsi);
                        }

                        string temp = "Ship " + item.Mmsi +" is saving..." ;
                        SetText("\n" + temp + " \n");
                    }



                    saveAvaiable = true;
                }
            */
            if (dictAvaiable && saveAvaiable)
            {
                saveAvaiable = false;
                if (modeChooseDictSaveFileFirst) // luu file dictJourneyFirst
                {
                    modeChooseDictSaveFileFirst = false;
                    modeChooseDictSaveFileSecond = true;
                    SetText("\n" + "Mode 1" + " \n");
                    savetoDatabase(dictShipJourneyFirst,"mode 1");

                }
                // luu file dictJourneySecond
                else
                {
                    modeChooseDictSaveFileFirst = true;
                    modeChooseDictSaveFileSecond =false;
                    SetText("\n" + "Mode 2" + " \n");
                    savetoDatabase(dictShipJourneySecond,"mode 2");
                }
                saveAvaiable = true;
            }
        }
        private void savetoDatabase(Dictionary<string,ShipJourney> dict,string mode)
        {

            SetText("\n" + "Saving database ... \n");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            journeymodel.Insert(dict);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            SetText("\n" + "Success !  "+ String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds,ts.Milliseconds / 10) +"\n");
            dict.Clear();
        }
   
        private void timerNow_Tick(object sender, EventArgs e)
        {
            updateTime();
        }
        private void updateTime()
        {
            ThreadHelper.SetText(this, txt_timer, (DateTime.Now.Subtract(timeBegin)).ToString(@"dd\.hh\:mm\:ss"));
        }
        delegate void SetTextCallback(string text);
        private void timerTask_Tick(object sender, EventArgs e)
        {
            dem++;
            Task task = new Task(sendRequestAsync);
            task.Start();
        }
        DateTime currentTime;
        //request get data from server
        private void sendRequestAsync()
        {
            RequestShip request = new RequestShip(GlobalVar.urlAll);
            currentTime = DateTime.Now;
            ThreadHelper.SetText(this, txt_dem, dem.ToString());
            if (request.getType() != "null")
            {
                ThreadHelper.SetText(this, txt_total, request.getTotal().ToString());
                ThreadHelper.SetText(this, txt_type, request.getType().ToString());
                dictAvaiable = false;
                DataTable data = request.getDataTableShip();
                ThreadHelper.SetData(this, dataGridView1, data);
                Task.Run(() => configDataToDict(data));
            }

            if(request.errorOutput.Length>0) SetText( request.errorOutput);
            
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox1.Text = text +  richTextBox1.Text;
            }
        }
        bool dictAvaiable = false;
        //handling data for dict
        private void configDataToDict(DataTable data)
        {
            int k = 0;
            //if (dem == 3)
            //    k = dem;
            //neu la true se chon dictShipJourneyFirst de lu
            if(modeChooseDictSaveFileFirst == true)
            {
                foreach (DataRow dr in data.Rows)
                {
                    saveMMSItoDB(dr);
                    if (checkShip(dr["mmsi"].ToString(),dictShipJourneyFirst))
                    {
                        //add new coor into ship of dict
                        ShipJourney sj = new ShipJourney();
                        sj = dictShipJourneyFirst[dr["mmsi"].ToString()];
                        sj.ListCoor.Add(new Coordinate(
                            float.Parse(dr["lat"].ToString()), 
                            float.Parse(dr["lng"].ToString()), 
                            currentTime.ToString("MM/dd/yyyy h:mm tt"),
                            float.Parse(dr["sog"].ToString()),
                            float.Parse(dr["cog"].ToString()),
                            Convert.ToInt32(dr["lost"].ToString()),
                            Convert.ToInt32(dr["min"].ToString())));
                        dictShipJourneyFirst[dr["mmsi"].ToString()] = sj;
                    }
                    else
                    {
                        ShipJourney sj = new ShipJourney(dr["mmsi"].ToString(), dr["vsnm"].ToString(), dr["type"].ToString(), dr["class"].ToString());
                        sj.addCoordinate(new Coordinate(
                            float.Parse(dr["lat"].ToString()),
                            float.Parse(dr["lng"].ToString()),
                            currentTime.ToString("MM/dd/yyyy h:mm tt"),
                            float.Parse(dr["sog"].ToString()),
                            float.Parse(dr["cog"].ToString()),
                            Convert.ToInt32(dr["lost"].ToString()),
                            Convert.ToInt32(dr["min"].ToString())));
                        //add new ship to dict
                        dictShipJourneyFirst.Add(dr["mmsi"].ToString(), sj);
                    }

                }
                SetText("\n" + "Using mode 1" + " \n");
            }
            //neu la true se chon dictShipJourneySecond de luu
            else
            {
                if (modeChooseDictSaveFileSecond)
                {
                    foreach (DataRow dr in data.Rows)
                    {
                        saveMMSItoDB(dr);
                        if (checkShip(dr["mmsi"].ToString(), dictShipJourneySecond))
                        {
                            //add new coor into ship of dict
                            ShipJourney sj = new ShipJourney();
                            sj = dictShipJourneySecond[dr["mmsi"].ToString()];
                            sj.ListCoor.Add(new Coordinate(
                            float.Parse(dr["lat"].ToString()),
                            float.Parse(dr["lng"].ToString()),
                            currentTime.ToString("MM/dd/yyyy h:mm tt"),
                            float.Parse(dr["sog"].ToString()),
                            float.Parse(dr["cog"].ToString()),
                            Convert.ToInt32(dr["lost"].ToString()),
                            Convert.ToInt32(dr["min"].ToString())));
                            dictShipJourneySecond[dr["mmsi"].ToString()] = sj;
                        }
                        else
                        {
                            ShipJourney sj = new ShipJourney(dr["mmsi"].ToString(), dr["vsnm"].ToString(), dr["type"].ToString(), dr["class"].ToString());
                            sj.addCoordinate(new Coordinate(
                            float.Parse(dr["lat"].ToString()),
                            float.Parse(dr["lng"].ToString()),
                            currentTime.ToString("MM/dd/yyyy h:mm tt"),
                            float.Parse(dr["sog"].ToString()),
                            float.Parse(dr["cog"].ToString()),
                            Convert.ToInt32(dr["lost"].ToString()),
                            Convert.ToInt32(dr["min"].ToString())));
                            //add new ship to dict
                            dictShipJourneySecond.Add(dr["mmsi"].ToString(), sj);

                        }
                    }
                    SetText("\n" + "Using mode 2" + " \n");
                }
            }
            dictAvaiable = true;
        }

        private Boolean checkShip(string mmsi, Dictionary<string, ShipJourney> dict)
        {

            if (dict.ContainsKey(mmsi)) return true;
            return false;
        }

        private void saveMMSItoDB(DataRow dr)
        {
            if (GlobalVar.hashMMSI.Contains(dr["mmsi"].ToString()) == false)
            {
                shipmodel.Insert(dr["mmsi"].ToString(), dr["vsnm"].ToString(), Convert.ToInt32(dr["type"].ToString()), dr["class"].ToString());
                GlobalVar.hashMMSI.Add(dr["mmsi"].ToString());
            }
        }
        //onclose
        protected override void OnClosed(EventArgs e)
        {
            if (dictAvaiable)
            {
                timerNow.Stop();
                timerTask.Stop();
                //DialogResult dialogResult = MessageBox.Show("Save All", "Notice", MessageBoxButtons.YesNo);
                //if (dialogResult == DialogResult.Yes)
                //{
                //    SaveFileDialog dlg = new SaveFileDialog();
                //    //   dlg.SelectedPath = Properties.Settings.Default.StoreFolder;
                //   // dlg.CheckFileExists = true;
                //    dlg.InitialDirectory = @"D:\";
                //    dlg.RestoreDirectory = true;
                //    dlg.DefaultExt = "json";
                //    dlg.Filter = "json file (*.json)|*.json";
                //  //  dlg.FilterIndex = 2;
                //    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //    {
                //        //if (File.Exists(dlg.FileName))
                //        //{
                //        //    Thread thread = new Thread(new ThreadStart(() => SaveFile.toSaveAll(dictShipJourney, dlg.FileName)));
                //        //    thread.Start();
                //        //}
                //        //else
                //        //{
                //        //    Thread thread = new Thread(new ThreadStart(() => SaveFile.toSaveAll(dictShipJourney, dlg.FileName)));
                //        //    thread.Start();
                //        //}
                //        //if (File.Exists(dlg.FileName))
                //        //{
                //        //    DialogResult dr = MessageBox.Show("File already exists! \nDo you like override it? \n" + dlg.FileName, "Save As", MessageBoxButtons.YesNo);
                //        //    //if choose override
                //        //    if (dialogResult == DialogResult.Yes)
                //        //    {
                //        //        Thread thread = new Thread(new ThreadStart(() => SaveFile.toSaveAll(dictShipJourney, dlg.FileName,1)));
                //        //        thread.Start();
                //        //    }
                //        //    //if choose replace
                //        //    else if (dialogResult == DialogResult.No)
                //        //    {
                //        //        Thread thread = new Thread(new ThreadStart(() => SaveFile.toSaveAll(dictShipJourney, dlg.FileName)));
                //        //        thread.Start();
                //        //    }
                //        //}
                //        //else
                //        //{
                //        //    Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity)));
                //        //    thread.Start();

                //        //}
                //        Thread thread = new Thread(new ThreadStart(() => SaveFile.toSaveAll(dictShipJourney, dlg.FileName)));
                //        thread.Start();

                //    }
                //}
                //else if (dialogResult == DialogResult.No)
                //{
                //    //do something else
                //}
                base.OnClosed(e);
            }

          
        }
        //click save bouy from dict
        /*
        private void save_bouy_Click(object sender, EventArgs e)
        {
            if (dictAvaiable)
            {
                if (GlobalVar.pathSaveFileBouy == "")
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.InitialDirectory = @"D:\";
                    dlg.RestoreDirectory = true;
                    dlg.DefaultExt = "json";
                    dlg.Filter = "json file (*.json)|*.json";
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GlobalVar.pathSaveFileBouy = dlg.FileName;
                        Thread thread = new Thread(new ThreadStart(() => SaveFile.toBouy(dictShipJourney, GlobalVar.pathSaveFileBouy)));
                        thread.Start();
                    }

                }
                else
                {
                    Thread thread = new Thread(new ThreadStart(() => SaveFile.toBouy(dictShipJourney, GlobalVar.pathSaveFileBouy)));
                    thread.Start();
                }
                MessageBox.Show("Save file bouys success !", "Notice ");
            }

        }
        //save density of ship in the East Sea
        private void save_Ship_Click(object sender, EventArgs e)
        {
            //if (dictAvaiable)
            //{
            //    if (GlobalVar.pathSaveFileDensity == "")
            //    {
            //        SaveFileDialog dlg = new SaveFileDialog();
            //        dlg.InitialDirectory = @"D:\";
            //        dlg.RestoreDirectory = true;
            //        dlg.DefaultExt = "json";
            //        dlg.Filter = "json file (*.json)|*.json";

            //        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //        {
            //            GlobalVar.pathSaveFileDensity = dlg.FileName;
            //            Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity)));
            //            thread.Start();
            //        }

            //    }
            //    else
            //    {
            //        Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity)));
            //        thread.Start();
            //    }
            //    MessageBox.Show("Save file density success !", "Notice ");
            //}
            if (dictAvaiable)
            {
                bool isSaved = false;
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.InitialDirectory = @"D:\";
                dlg.RestoreDirectory = true;
                dlg.DefaultExt = "json";
                dlg.Filter = "json file (*.json)|*.json";
                dlg.OverwritePrompt = false;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    GlobalVar.pathSaveFileDensity = dlg.FileName;
                    //check exist of file in computer
                    if (File.Exists(dlg.FileName))
                    {
                        DialogResult dialogResult = MessageBox.Show("File already exists! \nDo you like override it? \n" + dlg.FileName, "Save As", MessageBoxButtons.YesNoCancel);
                        //if choose override
                        if (dialogResult == DialogResult.Yes)
                        {
                            isSaved = true;
                            Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity, 1)));
                            thread.Start();
                        }
                        //if choose replace
                        else if (dialogResult == DialogResult.No)
                        {
                            isSaved = true;
                            Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity)));
                            thread.Start();
                        }
                    }
                    else
                    {
                        isSaved = true;
                        Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity)));
                        thread.Start();

                    }
                }
                if (isSaved) MessageBox.Show("Save file bouys success !", "Notice ");
            }
        }
        //save info ship has shipwrecked
        private void save_shipWreck_Click(object sender, EventArgs e)
        {
            if (dictAvaiable)
            {
                if (GlobalVar.pathSaveFileWreck == "")
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.InitialDirectory = @"D:\";
                    dlg.RestoreDirectory = true;
                    dlg.DefaultExt = "json";
                    dlg.Filter = "json file (*.json)|*.json";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GlobalVar.pathSaveFileWreck = dlg.FileName;
                        Thread thread = new Thread(new ThreadStart(() => SaveFile.toShipWreck(dictShipJourney, GlobalVar.pathSaveFileWreck)));
                        thread.Start();
                    }

                }
                else
                {
                    Thread thread = new Thread(new ThreadStart(() => SaveFile.toShipWreck(dictShipJourney, GlobalVar.pathSaveFileWreck)));
                    thread.Start();
                }
                MessageBox.Show("Save file ship wreck success !", "Notice ");
            }
        }

        //save info the leading mark
        private void save_leadingMark_Click(object sender, EventArgs e)
        {
            if (dictAvaiable)
            {
                if (GlobalVar.pathSaveFileLeadingMark == "")
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.InitialDirectory = @"D:\";
                    dlg.RestoreDirectory = true;
                    dlg.DefaultExt = "json";
                    dlg.Filter = "json file (*.json)|*.json";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GlobalVar.pathSaveFileLeadingMark = dlg.FileName;
                        Thread thread = new Thread(new ThreadStart(() => SaveFile.toLeadingMark(dictShipJourney, GlobalVar.pathSaveFileLeadingMark)));
                        thread.Start();
                    }

                }
                else
                {
                    Thread thread = new Thread(new ThreadStart(() => SaveFile.toLeadingMark(dictShipJourney, GlobalVar.pathSaveFileLeadingMark)));
                    thread.Start();
                }
                MessageBox.Show("Save file leading mark success !", "Notice ");

            }
        }
        */
    }
}
