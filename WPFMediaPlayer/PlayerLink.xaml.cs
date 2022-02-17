using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PixelMedia
{
    /// <summary>
    /// Interaction logic for PlayerLink.xaml
    /// </summary>
    public partial class PlayerLink : Window
    {
        public PlayerLink()
        {
            InitializeComponent();
        }


        private void Btn_Go_Click(object sender, RoutedEventArgs e)
        {
            Uri mul = new Uri(" ");

            IEnumerable<Uri> listrui = LoadPlaylist(mul);
            foreach (Uri ri in listrui)
            {
                string auri = ri.AbsoluteUri;
            }
        }


        IEnumerable<Uri> LoadPlaylist(Uri source)
        {
            using (var client = new WebClient())
            {
                var processedPlaylists = new HashSet<Uri>();
                var playlists = new Queue<Uri>();
                playlists.Enqueue(source);

                while (playlists.Count != 0)
                {
                    Uri playlistUri = playlists.Dequeue();
                    if (!processedPlaylists.Add(playlistUri)) continue;

                    string playlistContent = client.DownloadString(playlistUri);
                    string[] playlistLines = playlistContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in playlistLines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (line[0] == '#') continue;

                        Uri file;
                        if (!Uri.TryCreate(source, line, out file))
                        {
                            Console.WriteLine("Invalid line: '{0}'", line);
                            continue;
                        }
                        string extension = System.IO.Path.GetExtension(file.LocalPath);
                        if (extension.StartsWith(".m3u", StringComparison.OrdinalIgnoreCase))
                        {
                            playlists.Enqueue(file);
                        }
                        else
                        {
                            yield return file;
                        }
                    }
                }
            }
        }
    }
}