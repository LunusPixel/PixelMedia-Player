using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using PixelMedia;

namespace WPFMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        private const string TIME_FORMAT = @"hh\:mm\:ss";
        private bool _isplaying = true;
        private DispatcherTimer _timerVideoTime;
        private bool userIsDraggingSlider = false;
        private bool mediaPlayerIsPlaying = false;


        public MainWindow()
        {
            InitializeComponent();
            ChangeIsPlaying(false);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            DataContext = this;
        }

        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = @"All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV"
                //"Video files/m3u (*.mpg; *.mpeg; *.avi; *.mp4; *.m3u;)| *.mpg; *.mpeg; *.avi; *.mp4 *.m3u;"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Source = new Uri(openFileDialog.FileName);
            }
        }


        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            SubWindow subWindow = new SubWindow();
            subWindow.Show();
        }

        private void Player_OnClick(object sendder, RoutedEventArgs e)
        {
            PlayerLink PlayerLink = new PlayerLink();
            PlayerLink.Show();
        }

        private void OnMediaEnded()
        {
            mediaPlayer.Stop();
            ChangeIsPlaying(false);
        }

        private void ChangeIsPlaying(bool isPlaying)
        {
            _isplaying = isPlaying;

            if (_isplaying)
            {
                //btnPlayPause.Content = "Play";
                //btnPlayPause.ToolTip = "Media Playing";
                mediaPlayer.Play();
            }
            else
            {
                //btnPlayPause.Content = "Paused";
                //btnPlayPause.Content = "Media Paused";
                mediaPlayer.Pause();
            }
        }

        private void MediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            OnMediaEnded();
        }

        private void MediaElement_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OnMediaEnded();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Close();
        }

        private void PlayPauseClicked(object sender, RoutedEventArgs e)
        {
            ChangeIsPlaying(!_isplaying);
        }

        private void MyMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            ChangeIsPlaying(true);

            _timerVideoTime = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timerVideoTime.Tick += new EventHandler(timer_Tick);
            _timerVideoTime.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mediaPlayer.Source != null) && (mediaPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mediaPlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mediaPlayer != null) && (mediaPlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Play();
            mediaPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }
    }
}