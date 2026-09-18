using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicPlayerApp.Tests
{
    [TestClass]
    public class TrackTests
    {
        [TestMethod]
        public void KonstruktorZapisujeNazweISciezke()
        {
            Track track = new Track("piosenka.mp3", @"C:\muzyka\piosenka.mp3");

            Assert.AreEqual("piosenka.mp3", track.Name);
            Assert.AreEqual(@"C:\muzyka\piosenka.mp3", track.Path);
        }

        [TestMethod]
        public void PustaNazwaJestOdrzucana()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new Track("", @"C:\muzyka\piosenka.mp3"));
        }

        [TestMethod]
        public void PustaSciezkaJestOdrzucana()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new Track("piosenka.mp3", ""));
        }

        [TestMethod]
        public void NullJestOdrzucany()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new Track(null, @"C:\muzyka\piosenka.mp3"));
        }
    }

    [TestClass]
    public class PlaylistDodawanieTests
    {
        [TestMethod]
        public void NowaListaJestPusta()
        {
            Playlist playlist = new Playlist();

            Assert.AreEqual(0, playlist.Count);
        }

        [TestMethod]
        public void AddRangeDodajeUtwory()
        {
            Playlist playlist = new Playlist();

            playlist.AddRange(
                new[] { "a.mp3", "b.mp3" },
                new[] { @"C:\a.mp3", @"C:\b.mp3" });

            Assert.AreEqual(2, playlist.Count);
            Assert.AreEqual("a.mp3", playlist.GetName(0));
            Assert.AreEqual(@"C:\b.mp3", playlist.GetPath(1));
        }

        /// <summary>
        /// Test regresyjny do bledu opisanego w PR: przed poprawka drugi wybor
        /// piosenek nadpisywal tablice sciezek, a listBox rosl dalej - przez co
        /// klikniecie w stary utwor wywalalo IndexOutOfRangeException.
        /// </summary>
        [TestMethod]
        public void DrugieAddRangeDokladaZamiastNadpisywac()
        {
            Playlist playlist = new Playlist();
            playlist.AddRange(
                new[] { "a.mp3", "b.mp3", "c.mp3" },
                new[] { @"C:\muzyka\a.mp3", @"C:\muzyka\b.mp3", @"C:\muzyka\c.mp3" });

            playlist.AddRange(
                new[] { "d.mp3", "e.mp3" },
                new[] { @"D:\inne\d.mp3", @"D:\inne\e.mp3" });

            Assert.AreEqual(5, playlist.Count, "obie partie maja byc na liscie");
            Assert.AreEqual(@"C:\muzyka\c.mp3", playlist.GetPath(2), "stary utwor ma dalej dzialac");
            Assert.AreEqual(@"D:\inne\e.mp3", playlist.GetPath(4), "nowy utwor tez");
        }

        [TestMethod]
        public void RozneDlugosciTablicSaOdrzucane()
        {
            Playlist playlist = new Playlist();

            Assert.ThrowsException<ArgumentException>(
                () => playlist.AddRange(new[] { "a.mp3", "b.mp3" }, new[] { @"C:\a.mp3" }));
        }

        [TestMethod]
        public void NullTablicaJestOdrzucana()
        {
            Playlist playlist = new Playlist();

            Assert.ThrowsException<ArgumentNullException>(
                () => playlist.AddRange(null, new[] { @"C:\a.mp3" }));
        }

        [TestMethod]
        public void PusteTabliceNicNieZmieniaja()
        {
            Playlist playlist = new Playlist();

            playlist.AddRange(new string[0], new string[0]);

            Assert.AreEqual(0, playlist.Count);
        }
    }

    [TestClass]
    public class PlaylistIndeksyTests
    {
        private Playlist ZListaTrzechUtworow()
        {
            Playlist playlist = new Playlist();
            playlist.AddRange(
                new[] { "a.mp3", "b.mp3", "c.mp3" },
                new[] { @"C:\a.mp3", @"C:\b.mp3", @"C:\c.mp3" });
            return playlist;
        }

        [TestMethod]
        public void GetPathZwracaSciezkeSpodIndeksu()
        {
            Playlist playlist = ZListaTrzechUtworow();

            Assert.AreEqual(@"C:\b.mp3", playlist.GetPath(1));
        }

        [TestMethod]
        public void IsValidIndexPrzyjmujeIstniejacePozycje()
        {
            Playlist playlist = ZListaTrzechUtworow();

            Assert.IsTrue(playlist.IsValidIndex(0));
            Assert.IsTrue(playlist.IsValidIndex(2));
        }

        /// <summary>
        /// listBox.SelectedIndex daje -1, gdy nic nie jest zaznaczone. To jest
        /// najczestsze zrodlo wysypki w tej aplikacji, wiec ma swoj test.
        /// </summary>
        [TestMethod]
        public void IsValidIndexOdrzucaMinusJeden()
        {
            Playlist playlist = ZListaTrzechUtworow();

            Assert.IsFalse(playlist.IsValidIndex(-1));
        }

        [TestMethod]
        public void IsValidIndexOdrzucaIndeksPozaZakresem()
        {
            Playlist playlist = ZListaTrzechUtworow();

            Assert.IsFalse(playlist.IsValidIndex(3));
        }

        [TestMethod]
        public void PustaListaNieMaZadnegoPoprawnegoIndeksu()
        {
            Playlist playlist = new Playlist();

            Assert.IsFalse(playlist.IsValidIndex(0));
        }

        [TestMethod]
        public void GetPozaZakresemRzucaWyjatek()
        {
            Playlist playlist = ZListaTrzechUtworow();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => playlist.Get(3));
        }
    }

    [TestClass]
    public class PlaylistClearTests
    {
        [TestMethod]
        public void ClearUsuwaWszystkieUtwory()
        {
            Playlist playlist = new Playlist();
            playlist.AddRange(new[] { "a.mp3" }, new[] { @"C:\a.mp3" });

            playlist.Clear();

            Assert.AreEqual(0, playlist.Count);
            Assert.IsFalse(playlist.IsValidIndex(0));
        }

        [TestMethod]
        public void PoClearMoznaDodawacOdNowa()
        {
            Playlist playlist = new Playlist();
            playlist.AddRange(new[] { "a.mp3" }, new[] { @"C:\a.mp3" });
            playlist.Clear();

            playlist.AddRange(new[] { "b.mp3" }, new[] { @"C:\b.mp3" });

            Assert.AreEqual(1, playlist.Count);
            Assert.AreEqual(@"C:\b.mp3", playlist.GetPath(0));
        }
    }
}
