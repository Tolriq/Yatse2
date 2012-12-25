// ------------------------------------------------------------------------
//    YATSE 2 - A touch screen remote controller for XBMC (.NET 3.5)
//    Copyright (C) 2010  Tolriq (http://yatse.leetzone.org)
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// ------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml.Serialization;
using Setup;
using Yatse2.Libs;

namespace Yatse2.Classes
{
    public class Yatse2Config
    {
        public bool IsConfigured { get; set; }
        public bool Debug { get; set; }
        public bool SecondScreen { get; set; }
        public bool Topmost { get; set; }
        public bool KeepFocus { get; set; }
        public bool ForceResolution { get; set; }
        public bool Diaporama { get; set; }
        public bool Dimming { get; set; }
        public bool Currently { get; set; }
        public bool CurrentlyMovie { get; set; }
        public bool HideCursor { get; set; }
        public bool UseBanner { get; set; }
        public bool ShowOverlay { get; set; } // TODO : Use this
        public bool ShowEmptyMusicGenre { get; set; }
        public bool ManualRefresh { get; set; }
        public bool DisableAnimations { get; set; }
        public bool ShowEndTime { get; set; } // TODO : Use this
        public bool HideWatched { get; set; } // TODO : Use this
        public bool RefreshOnConnect { get; set; }
        public bool AnimatedMusicCover { get; set; }
        public bool DimmingOnlyVideo { get; set; }
        public bool Hack480 { get; set; }
        public bool DebugTrace { get; set; }
        public bool ForceOnCheckRemote { get; set; }
        public bool HideCompilationArtists { get; set; }
        public bool GenreToArtists { get; set; }
        public bool MusicFanartRotation { get; set; }
        public long ScreensaverTimer { get; set; }
        public long DimmingTimer { get; set; }
        public bool GoHomeOnEndPlayback { get; set; }
        public bool CheckForUpdate { get; set; }
        public bool DiaporamaSubdirs { get; set; }
        public bool DisableScreenPositioning { get; set; }
        public bool MouseMode { get; set; }
        public int MinDMBitsPerPel { get; set; }
        public int MinDMPelsWidth { get; set; }
        public int DiaporamaTimer { get; set; }
        public Devmode Resolution { get; set; }
        public string ImageDirectory { get; set; }
        public string Language { get; set; }
        public string Skin { get; set; }
        public string WeatherLoc { get; set; }
        public string WeatherUnit { get; set; }
        public string Homepage { get; set; }
        public long DefaultRemote { get; set; }
        public bool CropCacheImage { get; set; }
        public bool IgnoreSortTokens { get; set; }
        public string SortTokens { get; set; }
        public bool StartWithWindows { get; set; }
        public int DefaultPlayMode { get; set; }
        public int LongKeyPress { get; set; }
        public int DiaporamaMode { get; set; }
        public bool DisableResolutionDetection { get; set; }

        public Yatse2Config()
        {
            IsConfigured = false;
            Debug = true;
            SecondScreen = true;
            Topmost = true;
            Hack480 = false;
            KeepFocus = true;
            ForceResolution = false;
            Diaporama = false;
            Dimming = false;
            Currently = true;
            CurrentlyMovie = true;
            HideCursor = false;
            UseBanner = false;
            ShowOverlay = true;
            ShowEmptyMusicGenre = false;
            ManualRefresh = false;
            DisableAnimations = false;
            ShowEndTime = false;
            HideWatched = false;
            RefreshOnConnect = true;
            AnimatedMusicCover = true;
            DimmingOnlyVideo = true;
            DebugTrace = false;
            ForceOnCheckRemote = true;
            HideCompilationArtists = false;
            GenreToArtists = false;
            CheckForUpdate = true;
            Language = "English";
            Skin = "Default";
            WeatherLoc = "FRXX0076";
            WeatherUnit = "c";
            DefaultRemote = 0;
            Homepage = "Home";
            ScreensaverTimer = 120;
            GoHomeOnEndPlayback = false;
            MusicFanartRotation = true;
            DiaporamaSubdirs = true;
            MinDMBitsPerPel = 32;
            MinDMPelsWidth = 800;
            DiaporamaTimer = 10;
            DimmingTimer = 15;
            DisableScreenPositioning = false;
            MouseMode = false;
            CropCacheImage = true;
            SortTokens = "Le |La |Les |The |A |An |L'";
            IgnoreSortTokens = false;
            StartWithWindows = false;
            DefaultPlayMode = 0;
            LongKeyPress = 500;
            DiaporamaMode = 1;
            DisableResolutionDetection = false;

        }

        public bool Load(string configFile)
        {
            Logger.Instance().Log("Yatse2Config","Loading config : " + configFile);
            Yatse2Config config;
            try
            {
                var deserializer = new XmlSerializer(typeof(Yatse2Config));
                using (TextReader textReader = new StreamReader(configFile))
                {
                    config = (Yatse2Config) deserializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is InvalidOperationException )
                {
                    Logger.Instance().Log("Yatse2Config", "Error loading settings : " + ex.Message);
                    return false;
                }
                throw;
            }

            IsConfigured = config.IsConfigured;
            Debug = config.Debug;
            SecondScreen = config.SecondScreen;
            Topmost = config.Topmost;
            KeepFocus = config.KeepFocus;
            ForceResolution = config.ForceResolution;
            Diaporama = config.Diaporama;
            Dimming = config.Dimming;
            Currently = config.Currently;
            CurrentlyMovie = config.CurrentlyMovie;
            HideCursor = config.HideCursor;
            UseBanner = config.UseBanner;
            ShowOverlay = config.ShowOverlay;
            ShowEmptyMusicGenre = config.ShowEmptyMusicGenre;
            ManualRefresh = config.ManualRefresh;
            if (File.Exists(Helper.LangPath + config.Language + ".xaml"))
                Language = config.Language;
            if (Directory.Exists(Helper.SkinPath + config.Skin))
                Skin = config.Skin;
            Resolution = config.Resolution;
            ImageDirectory = config.ImageDirectory;
            DefaultRemote = config.DefaultRemote;
            WeatherLoc = config.WeatherLoc;
            WeatherUnit = config.WeatherUnit;
            DisableAnimations = config.DisableAnimations;
            ShowEndTime = config.ShowEndTime;
            HideWatched = config.HideWatched;
            RefreshOnConnect = config.RefreshOnConnect;
            AnimatedMusicCover = config.AnimatedMusicCover;
            DimmingOnlyVideo = config.DimmingOnlyVideo;
            Hack480 = config.Hack480;
            Homepage = config.Homepage;
            DebugTrace = config.DebugTrace;
            ForceOnCheckRemote = config.ForceOnCheckRemote;
            GenreToArtists = config.GenreToArtists;
            HideCompilationArtists = config.HideCompilationArtists;
            MusicFanartRotation = config.MusicFanartRotation;
            ScreensaverTimer = config.ScreensaverTimer;
            GoHomeOnEndPlayback = config.GoHomeOnEndPlayback;
            CheckForUpdate = config.CheckForUpdate;
            DiaporamaSubdirs = config.DiaporamaSubdirs;
            MinDMBitsPerPel = config.MinDMBitsPerPel;
            MinDMPelsWidth = config.MinDMPelsWidth;
            DisableScreenPositioning = config.DisableScreenPositioning;
            DiaporamaTimer = config.DiaporamaTimer;
            MouseMode = config.MouseMode;
            DimmingTimer = config.DimmingTimer;
            CropCacheImage = config.CropCacheImage;
            IgnoreSortTokens = config.IgnoreSortTokens;
            SortTokens = config.SortTokens;
            StartWithWindows = config.StartWithWindows;
            DefaultPlayMode = config.DefaultPlayMode;
            LongKeyPress = config.LongKeyPress;
            DiaporamaMode = config.DiaporamaMode;
            DisableResolutionDetection = config.DisableResolutionDetection;
            
            return true;
        }

        public void Save(string configFile)
        {
            Logger.Instance().Log("Yatse2Config", "Saving settings : " + configFile);
            try
            {
                var res = Resolution;
                res.DMFormName = "";
                Resolution = res;
                var serializer = new XmlSerializer(typeof(Yatse2Config));
                using (TextWriter textWriter = new StreamWriter(configFile))
                {
                    serializer.Serialize(textWriter, this);
                }
            }
            catch (IOException e)
            {
                Logger.Instance().Log("Yatse2Config", "Error saving settings : " + e.Message);
            }
            return;
        }
    }
}


