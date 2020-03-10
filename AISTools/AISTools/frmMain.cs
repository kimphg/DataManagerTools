using AISTools.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        Dictionary<string, ShipJourney> dictShipJourney = new Dictionary<string, ShipJourney>();
        int dem = 0;
        public frmMain()
        {
            InitializeComponent();
            setState();
        }
        private void setState()
        {
            //set style of form
            btn_start.TabStop = false;
            btn_start.FlatStyle = FlatStyle.Flat;
            btn_start.FlatAppearance.BorderSize = 0;
            btn_stop.TabStop = false;
            btn_stop.FlatStyle = FlatStyle.Flat;
            btn_stop.FlatAppearance.BorderSize = 0;
            //timer
            timerTask.Interval = 1000 * 60 * 10; // set time request to server
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            timeBegin = DateTime.Now;
            txt_status.Text = "True";
            dem++;
            Task task = new Task(sendRequestAsync);
            task.Start();

            timerNow.Interval = 1000;
            timerNow.Elapsed += timerNow_Tick;
            timerNow.Start();
            timerTask.Start();
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            timerNow.Stop();
            timerTask.Stop();
        }
        private void timerNow_Tick(object sender, EventArgs e)
        {
            updateTime();
        }
        private void updateTime()
        {
            ThreadHelper.SetText(this, txt_timer, (DateTime.Now.Subtract(timeBegin)).ToString(@"dd\.hh\:mm\:ss"));
        }

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
            RequestShip request = new RequestShip(GlobalVar.url);
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

        }
        bool dictAvaiable = false;
        //handling data for dict
        private void configDataToDict(DataTable data)
        {
            int k = 0;
            //if (dem == 3)
            //    k = dem;

            foreach (DataRow dr in data.Rows)
            {
                if (checkShip(dr["mmsi"].ToString()))
                {
                    //add new coor into ship of dict
                    ShipJourney sj = new ShipJourney();
                    sj = dictShipJourney[dr["mmsi"].ToString()];
                    sj.ListCoor.Add(new Coordinate(float.Parse(dr["lat"].ToString()), float.Parse(dr["lng"].ToString()), currentTime.ToString("MM/dd/yyyy h:mm tt")));
                    dictShipJourney[dr["mmsi"].ToString()] = sj;


                }
                else
                {
                    ShipJourney sj = new ShipJourney(dr["mmsi"].ToString(), dr["vsnm"].ToString(), dr["type"].ToString());
                    sj.addCoordinate(new Coordinate(float.Parse(dr["lat"].ToString()), float.Parse(dr["lng"].ToString()), currentTime.ToString("MM/dd/yyyy h:mm tt")));
                    //add new ship to dict
                    dictShipJourney.Add(dr["mmsi"].ToString(), sj);

                }
            }
            dictAvaiable = true;
            //MessageBox.Show("ok");
        }
        //check mmsi of ship before it add to dict
        private Boolean checkShip(string mmsi)
        {
            if (dictShipJourney.ContainsKey(mmsi)) return true;
            return false;
        }
        //onclose
        protected override void OnClosed(EventArgs e)
        {
            if (dictAvaiable)
            {
                timerNow.Stop();
                timerTask.Stop();
                DialogResult dialogResult = MessageBox.Show("Save All", "Notice", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    //   dlg.SelectedPath = Properties.Settings.Default.StoreFolder;
                   // dlg.CheckFileExists = true;
                    dlg.InitialDirectory = @"D:\";
                    dlg.RestoreDirectory = true;
                    dlg.DefaultExt = "json";
                    dlg.Filter = "json file (*.json)|*.json";
                  //  dlg.FilterIndex = 2;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Thread thread = new Thread(new ThreadStart(() => SaveFile.toSaveAll(dictShipJourney, dlg.FileName)));
                        thread.Start();
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }

            base.OnClosed(e);
        }
        //click save bouy from dict
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
            if (dictAvaiable)
            {
                if (GlobalVar.pathSaveFileDensity == "")
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.InitialDirectory = @"D:\";
                    dlg.RestoreDirectory = true;
                    dlg.DefaultExt = "json";
                    dlg.Filter = "json file (*.json)|*.json";

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GlobalVar.pathSaveFileDensity = dlg.FileName;
                        Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity)));
                        thread.Start();
                    }

                }
                else
                {
                    Thread thread = new Thread(new ThreadStart(() => SaveFile.toDensity(dictShipJourney, GlobalVar.pathSaveFileDensity)));
                    thread.Start();
                }
                MessageBox.Show("Save file density success !", "Notice ");
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
    }
}
