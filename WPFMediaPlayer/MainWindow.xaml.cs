using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WPFMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        private const string TIME_FORMAT = @"hh\:mm\:ss";
        private bool _isplaying = false;
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
                Filter = @"Video files (*.mpg; *.mpeg; *.avi; *.mp4; *.m3u8; *.m3u;)| *.mpg; *.mpeg; *.avi; *.mp4 *.m3u *.m3u8 *.mkv"
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
            //btnPlayPause.IsEnabled = true;

            // Create a timer that will update the counters and the time slider
            _timerVideoTime = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            //_timerVideoTime.Tick += new EventHandler(Timer_Tick);
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
            //lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
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