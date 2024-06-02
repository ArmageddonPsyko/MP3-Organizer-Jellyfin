using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using TagLib;

namespace MP3_Organizer
{
    class TrackOrganizer
    {
        private List<string> _files = new List<string>();
        private List<string> _allowed = new List<string>();
        private TextInfo _textInfo;
        private string _startIn;
        private string _destination;

        #region Accessors

        public List<string> Files
        {
            get { return _files; }
            set { _files = value; }
        }

        public List<string> Allowed
        {
            get { return _allowed; }
            set { _allowed = value; }
        }

        public TextInfo TextInfo
        {
            get { return _textInfo; }
            set { _textInfo = value; }
        }

        public string StartIn
        {
            get { return _startIn; }
            set { _startIn = value; }
        }

        public string Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        #endregion

        public TrackOrganizer(string startIn, string destination, List<string> allowed)
        {
            StartIn = startIn;
            Destination = destination;
            Allowed = allowed;

            TextInfo = new CultureInfo("nl-NL", false).TextInfo;
        }

        public void Index()
        {
            DirectorySearch(StartIn);
        }

        public void Organize()
        {
            if (Files.Count == 0)
            {
                Console.WriteLine("Run Index() first!");
                return;
            }

            foreach (string file in Files)
            {
                // Update the metadata
                UpdateMetadata(file);

                // Create file path and name
                string destFilePath = CreateDirectoryPath(file);
                string destFileName = CreateFilePath(file);

                // Check if directory exists, otherwise create it
                if (!System.IO.Directory.Exists(Destination + destFilePath))
                {
                    System.IO.Directory.CreateDirectory(Destination + destFilePath);
                }

                // Create full file path
                string fullPath = Destination + destFilePath + destFileName;

                Console.WriteLine(fullPath);

                // Check if file exists, otherwise copy file to new location
                if (!System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Copy(file, fullPath);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (string file in Files)
            {
                builder.AppendLine(file);
            }

            return builder.ToString();
        }

        private void UpdateMetadata(string trackPath)
        {
            TagLib.File track = TagLib.File.Create(trackPath);

            bool hasChanges = false;

            // Update Performers
            if (track.Tag.Performers.Length > 0)
            {
                string performers = string.Join(",", track.Tag.Performers);
                if (performers.Contains(","))
                {
                    track.Tag.Performers = performers.Replace(",", ";").Split(';');
                    hasChanges = true;
                }
            }

            // Update Album
            if (!string.IsNullOrEmpty(track.Tag.Album))
            {
                string album = track.Tag.Album;
                if (album.Contains(","))
                {
                    track.Tag.Album = album.Replace(",", ";");
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                track.Save();
                Console.WriteLine($"Updated metadata: {trackPath}");
            }

            track.Dispose();
        }

        private string CreateDirectoryPath(string trackPath)
        {
            TagLib.File track = TagLib.File.Create(trackPath);

            if (track.Tag == null)
            {
                throw new Exception(string.Format("This track seems to be invalid and can't be read ({0})", trackPath));
            }

            if (track.Tag.Performers == null)
            {
                throw new Exception(string.Format("This track contains no artist information ({0})", trackPath));
            }

            if (track.Tag.Album == null)
            {
                throw new Exception(string.Format("This track contains no album information ({0})", trackPath));
            }

            string invalid = new string(System.IO.Path.GetInvalidPathChars());
            string performer = track.Tag.Performers[0]; // Use only the first artist

            string album = track.Tag.Album;

            track.Dispose();

            foreach (char c in invalid)
            {
                performer = performer.Replace(c.ToString(), "");
                album = album.Replace(c.ToString(), "");
            }

            performer = TextInfo.ToTitleCase(performer.ToLower()).Replace("/", "-").Replace("?", "").Replace(":", "");
            album = TextInfo.ToTitleCase(album.ToLower()).Replace("/", "-").Replace("?", "").Replace(":", "");

            StringBuilder builder = new StringBuilder();

            builder.AppendFormat(@"\{0}\{1}\", performer, album);

            return builder.ToString();
        }

        private string CreateFilePath(string trackPath)
        {
            TagLib.File track = TagLib.File.Create(trackPath);

            string extension = System.IO.Path.GetExtension(trackPath);
            string invalid = new string(System.IO.Path.GetInvalidFileNameChars());

            if (track.Tag.Title == null)
            {
                throw new Exception(string.Format("This track contains no title information ({0})", trackPath));
            }

            string title = track.Tag.Title;
            string number = track.Tag.Track.ToString("D2");

            track.Dispose();

            foreach (char c in invalid)
            {
                title = title.Replace(c.ToString(), "");
            }

            title = TextInfo.ToTitleCase(title.ToLower());

            StringBuilder builder = new StringBuilder();

            builder.AppendFormat(@"{2} - {0}{1}", title, extension, number);

            return builder.ToString();
        }

        private void DirectorySearch(string startIn)
        {
            try
            {
                foreach (string file in System.IO.Directory.GetFiles(startIn))
                {
                    string extension = System.IO.Path.GetExtension(file);

                    if (Allowed.Contains(extension))
                    {
                        Files.Add(file);
                    }
                }

                foreach (string directory in System.IO.Directory.GetDirectories(startIn))
                {
                    DirectorySearch(directory);
                }
            }
            catch (SystemException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
