using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastAndForms
{
    public partial class frmMain : Form
    {
        enum Cars
        {
            car1,
            car2
        }
        public frmMain()
        {
            InitializeComponent();
        }
        BackgroundWorker bgwcar1 = new BackgroundWorker();
        BackgroundWorker bgwcar2 = new BackgroundWorker();
        private void frmMain_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize += this.Size;
            
        }
        void racestart()
        {
            if (bgwcar1.IsBusy||bgwcar2.IsBusy)
            {
                MessageBox.Show("Gara in corso");
                return;
            }
            bgwcar1.WorkerReportsProgress = true;
            bgwcar2.WorkerReportsProgress = true;
            bgwcar1.DoWork += RndRace;
            bgwcar2.DoWork += RndRace;
            bgwcar1.ProgressChanged += RaceView;
            bgwcar2.ProgressChanged += RaceView;
            bgwcar1.RunWorkerCompleted += RaceEnd;
            bgwcar2.RunWorkerCompleted += RaceEnd;
            bgwcar1.WorkerSupportsCancellation = true;
            bgwcar2.WorkerSupportsCancellation = true;
            bgwcar1.RunWorkerAsync(pctCar1);
            bgwcar2.RunWorkerAsync(pctCar2);
        }
        void RndRace(object sender, DoWorkEventArgs e)
        {
            var rnd = new Random();
            
            var currbgw = sender as BackgroundWorker;
            var currpct = e.Argument as PictureBox;
            int i;
            for (i = 0; i < this.Size.Width-currpct.Width/2; i++)
            {
                if (currbgw.CancellationPending)
                {
                    e.Result = (false, i,"");
                    return;
                }
                i += rnd.Next(1, 20);
                currbgw.ReportProgress(i, e.Argument);
                Thread.Sleep(100);
            }
            e.Result = (true, i,currpct.Tag as string);
        }
        void RaceView(object sender, ProgressChangedEventArgs e)
        {
            var imgbx = e.UserState as PictureBox;
            imgbx.Location = new Point(e.ProgressPercentage, imgbx.Location.Y);
        }
        void RaceEnd(object sender, RunWorkerCompletedEventArgs e)
        {
            var (completed, res,carname) = (ValueTuple<bool, int, string>)e.Result;
            if (!completed)
            {
                Console.WriteLine(carname + " cancellato");
                return;
            }
            lblWhoHasWon.Text = carname + " ha vinto";
            Console.WriteLine(carname + " completato");
            if (bgwcar1.IsBusy)
            { bgwcar1.CancelAsync(); }
            if (bgwcar2.IsBusy)
            { bgwcar2.CancelAsync(); }
        }

        private void btnStartRace_Click(object sender, EventArgs e)
        {
            racestart();
        }
    }
}
