using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace Media_Player_DIY
{
    public partial class Form1 : Form
    {
        MediaPlayer mpUser = new MediaPlayer();
        bool isMPplay = false;
        bool isMPopened = false;
        int isRepeat = 0;
        TimeSpan TrackPositionTime;
        Double TimeSound_double;
        List<string> paths = new List<string>();

        public Form1()
        {
            InitializeComponent();
            openFileDialog1.FileName = "";

            mpUser.MediaOpened += OpenedSound;
            mpUser.MediaEnded += NextTrackPlay;


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Выберите файл";
            //openFileDialog1.Filter = "Аудио файл | *.wav";
            openFileDialog1.ShowDialog();
            mpUser.Open(new Uri(openFileDialog1.FileName));
            isMPopened = true;
            TimeSound_double = (double)mpUser.NaturalDuration.TimeSpan.TotalSeconds;
            label1.Text += TimeSound_double.ToString();



        }

        private void TimelineTrackbar_ValueChanged(object sender, EventArgs e)
        {
            double lasttime = TimeSound_double;
            double SeekIndex = TimelineTrackbar.Value / 100f;

            if (TimeSound_double > 60 && TimeSound_double < 3600)
            {
                int minuts = (int)Math.Floor(lasttime * SeekIndex / 60); lasttime -= minuts * 60;
                int seconds = (int)Math.Floor(lasttime * SeekIndex) ;
                TrackPositionTime = new TimeSpan(0, minuts, seconds);
            }
            else if(TimeSound_double > 3600 && TimeSound_double < 86400)
            {
                int hours = (int)Math.Floor(lasttime * SeekIndex / 3600); lasttime -= hours*3600;
                int minuts = (int)Math.Floor(lasttime * SeekIndex / 60); lasttime -= minuts * 60;
                int seconds = (int)Math.Floor(lasttime * SeekIndex);
                TrackPositionTime = new TimeSpan(hours, minuts, seconds);
            }

            mpUser.Position = TrackPositionTime;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isMPopened && isMPplay == false)
            {
                isMPplay = true;
                mpUser.Play();
            }
            else if (isMPopened && isMPplay == true)
            {
                isMPplay = false;
                mpUser.Pause();  
            }
        }

        private void TrackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            mpUser.Volume = TrackBarVolume.Value / 100.0f;
        }

        private void RepeatButton_Click(object sender, EventArgs e)
        {
            if (isRepeat == 0)
            {
                isRepeat = 1;
                RepeatButton.Image = Media_Player_DIY.Properties.Resources.BRepeat_once;
            }
            else if (isRepeat == 1)
            {
                isRepeat = 2;
                RepeatButton.Image = Media_Player_DIY.Properties.Resources.BRepeat_all;
            }
            else if (isRepeat == 2)  
            {
                isRepeat = 0;
                RepeatButton.Image = Media_Player_DIY.Properties.Resources.BRepeat_off;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TrackBarVolume.Value = 50;
            mpUser.Volume = 0.5;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {

            foreach (string obj in (string[])e.Data.GetData(DataFormats.FileDrop))
                if(Directory.Exists(obj))
                    paths.AddRange(Directory.GetFiles(obj, "*.*", SearchOption.AllDirectories));
                else
                    paths.Add(obj);
            for (int i = 0; i < paths.Count; i++)
            {
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(paths[i]));
            }
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
           if(e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //listBox1.SelectedItem.ToString()
            string item_music = paths[listBox1.SelectedIndex];
            mpUser.Open(new Uri(item_music));
            isMPopened = true;
            mpUser.Play();
            isMPplay = true;
            //Thread.Sleep(100);
            //StreamTimeSound();
        }

        void StreamTimeSound()
        {
            if (mpUser.NaturalDuration.HasTimeSpan)
            {
                TimeSound_double = Math.Round(mpUser.NaturalDuration.TimeSpan.TotalSeconds, 1);
                label1.Text += "\n" + "Продолжительность трека: " + TimeSound_double.ToString();
            }
            
        }

        void OpenedSound(object sender, EventArgs e)
        {
            StreamTimeSound();
            isMPopened=true;
            NameTruckLabel.Text = Path.GetFileNameWithoutExtension(mpUser.Source.LocalPath);
            TimelineTrackbar.Enabled = true;
            //TimelineTrackbar_StreamTime();
        }

        void TimelineTrackbar_StreamTime()
        {
            while (true)
            {
                TimelineTrackbar.Value = (int)(mpUser.Position.TotalSeconds / TimeSound_double);
                Thread.Sleep(1000);
            }
        }

        void NextTrackPlay(object sender, EventArgs e)
        {
            if (isRepeat == 1)
            {
                string item_music = paths[listBox1.SelectedIndex];
                mpUser.Open(new Uri(item_music));
                mpUser.Play();
                if(!isMPplay) isMPplay = true;
            }
            else if (isRepeat == 2 && listBox1.Items.Count > 1)
            {
                if (listBox1.SelectedIndex != listBox1.Items.Count - 1)
                    listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                else
                    listBox1.SelectedIndex = 0;
            }
        }

        void DebugLine()
        {
            //label1
        }
    }
}
