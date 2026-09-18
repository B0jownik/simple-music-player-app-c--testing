using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicPlayerApp
{
    /// <summary>
    /// Jeden utwor na liscie: nazwa pokazywana uzytkownikowi i sciezka do pliku.
    /// </summary>
    public class Track
    {
        public Track(string name, string path)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("nazwa utworu nie moze byc pusta", "name");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("sciezka do pliku nie moze byc pusta", "path");

            Name = name;
            Path = path;
        }

        public string Name { get; private set; }

        public string Path { get; private set; }
    }

    /// <summary>
    /// Lista odtwarzania. Trzyma utwory i pilnuje, zeby numer pozycji na liscie
    /// zawsze wskazywal na wlasciwy plik.
    ///
    /// Ta klasa celowo nie wie nic o WinForms ani o Windows Media Playerze -
    /// dzieki temu da sie ja przetestowac bez uruchamiania okna aplikacji.
    /// </summary>
    public class Playlist
    {
        private readonly List<Track> tracks = new List<Track>();

        public int Count
        {
            get { return tracks.Count; }
        }

        public ReadOnlyCollection<Track> Tracks
        {
            get { return tracks.AsReadOnly(); }
        }

        /// <summary>
        /// Doklada utwory na koniec listy. Nie kasuje tego, co juz na niej jest.
        /// </summary>
        public void AddRange(string[] names, string[] paths)
        {
            if (names == null)
                throw new ArgumentNullException("names");
            if (paths == null)
                throw new ArgumentNullException("paths");
            if (names.Length != paths.Length)
                throw new ArgumentException(
                    "liczba nazw (" + names.Length + ") musi sie zgadzac z liczba sciezek (" + paths.Length + ")",
                    "paths");

            for (int i = 0; i < names.Length; i++)
            {
                tracks.Add(new Track(names[i], paths[i]));
            }
        }

        /// <summary>
        /// Czy pod tym numerem cos jest. -1 (nic nie zaznaczone) tez tu wpada.
        /// </summary>
        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < tracks.Count;
        }

        public Track Get(int index)
        {
            if (!IsValidIndex(index))
                throw new ArgumentOutOfRangeException(
                    "index", index, "lista ma " + tracks.Count + " utworow");

            return tracks[index];
        }

        public string GetPath(int index)
        {
            return Get(index).Path;
        }

        public string GetName(int index)
        {
            return Get(index).Name;
        }

        public void Clear()
        {
            tracks.Clear();
        }
    }
}
