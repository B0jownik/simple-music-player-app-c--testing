using System;
using System.Windows.Forms;

namespace MusicPlayerApp
{
    public partial class MusicPlayerApp : Form
    {
        public MusicPlayerApp()
        {
            InitializeComponent();
        }

        // Cala logika listy siedzi w klasie Playlist, zeby dalo sie ja przetestowac
        // bez uruchamiania okna. Formatka tylko pyta uzytkownika i pokazuje wynik.
        private readonly Playlist playlist = new Playlist();

        private void btnSelectSongs_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                playlist.AddRange(ofd.SafeFileNames, ofd.FileNames);
                PokazListe();
            }
        }

        private void listBoxSongs_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxSongs.SelectedIndex;

            // Przy czyszczeniu listy SelectedIndex robi sie -1 i tu trafiamy.
            if (!playlist.IsValidIndex(index))
            {
                return;
            }

            axWindowsMediaPlayerMusic.URL = playlist.GetPath(index);
        }

        private void PokazListe()
        {
            listBoxSongs.Items.Clear();

            foreach (Track track in playlist.Tracks)
            {
                listBoxSongs.Items.Add(track.Name);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
