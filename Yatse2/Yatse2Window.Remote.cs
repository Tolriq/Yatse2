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
using System.Globalization;
using System.Windows;
using System.Windows.Media.Animation;
using Setup;
using Yatse2.Classes;
using Yatse2.Libs;

namespace Yatse2
{
    public partial class Yatse2Window
    {

        private void InitRemote()
        {
            if (_remote != null)
            {
                _remote.Close();
                _remote.Dispose();
            }
            _remoteConnected = false;
            _remoteLibraryRefreshed = !_config.RefreshOnConnect;

            _remote = ApiHelper.Instance().GetRemoteByApi(null);
            if (_currentRemoteId == 0)
            {
                Logger.Instance().Log("Yatse2", "No current Remote");
                _remoteInfo = new Yatse2Remote { Id = 0, CacheFilled = 1 };
                return;
            }

            var remotes = _database.GetRemote(_currentRemoteId);
            if (remotes.Count < 1)
            {
                Logger.Instance().Log("Yatse2", "No remote found");
                return;
            }

            var remoteInfo = remotes[0];
            Logger.Instance().Log("Yatse2", "Init remote : " + remoteInfo.Id + " - " + remoteInfo.Name + " (" + remoteInfo.Api + " / " + remoteInfo.Version + ")");

            _remote = ApiHelper.Instance().GetRemoteByApi(remoteInfo.Api);
            if (_remote == null)
            {
                Logger.Instance().Log("Yatse2", "Error plugin not loaded for API : " + remoteInfo.Api);
                return;
            }

            _remotePlugin = ApiHelper.Instance().GetRemotePluginByApi(remoteInfo.Api);

            txb_Home_Movies.Visibility = (_remotePlugin.SupportedFunctions().MovieLibrary) ? Visibility.Visible : Visibility.Hidden;
            txb_Home_TvShows.Visibility = (_remotePlugin.SupportedFunctions().TvShowLibrary) ? Visibility.Visible : Visibility.Hidden;
            txb_Home_Artists.Visibility = (_remotePlugin.SupportedFunctions().AudioLibrary) ? Visibility.Visible : Visibility.Hidden;
            txb_Home_Albums.Visibility = (_remotePlugin.SupportedFunctions().AudioLibrary) ? Visibility.Visible : Visibility.Hidden;
            txb_Home_Genres.Visibility = (_remotePlugin.SupportedFunctions().AudioLibrary) ? Visibility.Visible : Visibility.Hidden;

            _failedRemoteCheck = false;

            _remote.Configure(remoteInfo.IP, remoteInfo.Port, remoteInfo.Login, remoteInfo.Password);
            _remoteInfo = remoteInfo;

            Helper.Instance.CurrentApi = _remoteInfo.Api;
            _yatse2Properties.Api = _remoteInfo.Api;

            ClearFiltersAndDataSource();

            if (_currentGrid.Name == "grd_Home")
            {
                _gridHistory.Clear();
            }
            else
            {
                _gridHistory.Clear();
                _gridHistory.Add("grd_Home");
            }

        }
        
        private void ClearFiltersAndDataSource()
        {
            _filterAudioArtist = "";
            _filterAudioGenre = "";
            _filterAudioAlbum = "";
            _filterMovie = "";
            _filterTvShow = "";

            btn_Movies_Filter.Background = GetSkinImageBrushSmall("Remote_Search");
            btn_AudioGenre_Filter.Background = GetSkinImageBrushSmall("Remote_Search");
            btn_AudioAlbums_Filter.Background = GetSkinImageBrushSmall("Remote_Search");
            btn_AudioArtists_Filter.Background = GetSkinImageBrushSmall("Remote_Search");
            btn_TvShows_Filter.Background = GetSkinImageBrushSmall("Remote_Search");


            _audioSongsDataSource.Clear();
            _audioGenresDataSource.Clear();
            _audioArtistsDataSource.Clear();
            _moviesDataSource.Clear();
            _tvShowsDataSource.Clear();
            _tvSeasonsDataSource.Clear();
            _tvEpisodesDataSource.Clear();

            _videoFavoritesFilter = false;
            btn_Home_Video_FilterFavorites.Background = GetSkinImageBrush("Filter_Favorites");
            /*_moviesCollectionView.Filter = null;
            _tvShowsCollectionView.Filter = null;*/

            _audioFavoritesFilter = false;
            btn_Home_Music_FilterFavorites.Background = GetSkinImageBrush("Filter_Favorites");
            /*_audioGenresCollectionView.Filter = null;
            _audioAlbumsCollectionView.Filter = null;
            _audioArtistsCollectionView.Filter = null;*/
        }


        private void AudioStarting()
        {
            if (grd_Dimming.Visibility == Visibility.Visible)
            {
                grd_Dimming_MouseDown(null, null);
            }
            if (grd_Diaporama.Visibility == Visibility.Visible)
            {
                grd_Diaporama_MouseDown(null, null);
            }
            if (_config.Currently)
            {
                Logger.Instance().Log("Yatse2", "Music starting : Switch to currently");
                ShowGrid(grd_Currently);
                _isScreenSaver = true;
            }
            else
            {
                if (_config.Dimming && !_config.DimmingOnlyVideo)
                {
                    Logger.Instance().Log("Yatse2", "Video starting : start screen saver : Dimming");
                    var stbDimmingShow = (Storyboard)TryFindResource("stb_ShowDimming");
                    if (stbDimmingShow != null)
                    {
                        stbDimmingShow.Begin(this);
                        _isScreenSaver = true;
                    }
                }
                else
                {
                    StartDiaporama();
                    _isScreenSaver = true;
                }

            }

        }

        private void VideoStarting()
        {
            if (grd_Dimming.Visibility == Visibility.Visible)
            {
                grd_Dimming_MouseDown(null, null);
            }
            if (grd_Diaporama.Visibility == Visibility.Visible)
            {
                grd_Diaporama_MouseDown(null, null);
            }
            if (_config.CurrentlyMovie)
            {
                Logger.Instance().Log("Yatse2", "Video starting : Switch to currently");
                ShowGrid(grd_Currently);
                _isScreenSaver = true;
            }
            else
            {
                if (_config.Dimming ) //&& _config.DimmingOnlyVideo)
                {
                    Logger.Instance().Log("Yatse2", "Video starting : start screen saver : Dimming");
                    var stbDimmingShow = (Storyboard)TryFindResource("stb_ShowDimming");
                    if (stbDimmingShow != null)
                    {
                        stbDimmingShow.Begin(this);
                        _isScreenSaver = true;
                    }
                }
                else
                {
                    StartDiaporama();
                    _isScreenSaver = true;
                }
            }
        }

        private void UpdateCurrently(Plugin.ApiCurrently nowPlaying )
        {
            _isPlaying = true;
            switch (nowPlaying.MediaType)
            {
                case "Audio":
                    Logger.Instance().Log("Yatse2", "New Music Media : " + nowPlaying.FileName);
                    _yatse2Properties.Currently.IsMusic = true;
                    _yatse2Properties.Currently.MusicAlbum = nowPlaying.Album;
                    _yatse2Properties.Currently.MusicSong = nowPlaying.Title;
                    _yatse2Properties.Currently.MusicArtist = nowPlaying.Artist;
                    _yatse2Properties.Currently.Fanart = _config.MusicFanartRotation ? GetRandomImagePath(Helper.CachePath + @"Music\Fanarts") : GetMusicFanartPath(nowPlaying.FanartURL);

                    _yatse2Properties.Currently.Thumb = GetMusicThumbPath(nowPlaying.ThumbURL); // TODO : Change to converter
                    _yatse2Properties.Currently.MusicYear = nowPlaying.Year.ToString(CultureInfo.InvariantCulture);
                    _yatse2Properties.Currently.MusicTrack = nowPlaying.Track.ToString(CultureInfo.InvariantCulture);
                    _yatse2Properties.Currently.MusicGenre = nowPlaying.Genre;

                    var songinfo = _database.GetAudioSongFromFile(_remoteInfo.Id, nowPlaying.FileName);
                    if (songinfo.Count > 0)
                    {
                        var artistinfo = _database.GetAudioArtistFromName(_remoteInfo.Id, songinfo[0].Artist);
                        _yatse2Properties.Currently.MusicBiography = artistinfo.Count > 0 ? artistinfo[0].Biography : "No information";
                        if (!_config.MusicFanartRotation)
                            _yatse2Properties.Currently.Fanart = artistinfo.Count > 0 ? GetMusicFanartPath(artistinfo[0].Fanart) : "";
                    }
                    AudioStarting();

                    break;
                case "TvShow":
                    Logger.Instance().Log("Yatse2", "New TvShow Media : " + nowPlaying.FileName);
                    _yatse2Properties.Currently.IsTv = true;
                    _yatse2Properties.Currently.Thumb = GetVideoThumbPath(nowPlaying.ThumbURL); // TODO : Change to converter
                    var epinfo = _database.GetTvEpisodeFromFile(_remoteInfo.Id, nowPlaying.FileName);
                    if (epinfo.Count > 0)
                    {
                        _yatse2Properties.Currently.TvShow = epinfo[0].ShowTitle + " - " + epinfo[0].Title;
                        _yatse2Properties.Currently.TvTitle = epinfo[0].Title;
                        _yatse2Properties.Currently.TvAired = epinfo[0].Date;
                        _yatse2Properties.Currently.TvPlot = epinfo[0].Plot;
                        _yatse2Properties.Currently.TvEpisode = GetLocalizedString(77) + " " + epinfo[0].Season + " " + GetLocalizedString(78) + " " + epinfo[0].Episode;
                        _yatse2Properties.Currently.TvNote = epinfo[0].Rating;
                        var showinfo = _database.GetTvShowFromName(_remoteInfo.Id, epinfo[0].ShowTitle);
                        _yatse2Properties.Currently.Fanart = GetVideoFanartPath(showinfo.Count > 0 ? showinfo[0].Fanart : nowPlaying.FanartURL); // TODO : Change to Covnerter
                        _yatse2Properties.Currently.TvStudio = Helper.SkinPath + _yatse2Properties.Skin + @"\Studios\" + epinfo[0].Studio + ".png";
                        _yatse2Properties.Currently.TvDirector = epinfo[0].Director;
                        _yatse2Properties.Currently.TvYear = epinfo[0].Date.Length > 3 ? epinfo[0].Date.Substring(0, 4) : epinfo[0].Date;
                        _yatse2Properties.Currently.TvVotes = "";
                    }
                    else
                    {
                        _yatse2Properties.Currently.TvShow = nowPlaying.ShowTitle + " - S" + nowPlaying.SeasonNumber + " E" + nowPlaying.EpisodeNumber;
                        _yatse2Properties.Currently.TvTitle = nowPlaying.Title;
                        _yatse2Properties.Currently.TvAired = nowPlaying.FirstAired.ToShortDateString();
                        _yatse2Properties.Currently.TvPlot = nowPlaying.Plot;
                        _yatse2Properties.Currently.TvEpisode = GetLocalizedString(77) + " " + nowPlaying.SeasonNumber + " " + GetLocalizedString(78) + " " + nowPlaying.EpisodeNumber;
                        _yatse2Properties.Currently.TvNote = nowPlaying.Rating;
                        _yatse2Properties.Currently.Fanart = GetVideoFanartPath(nowPlaying.FanartURL); // TODO : Change to Converter
                        _yatse2Properties.Currently.TvStudio = Helper.SkinPath + _yatse2Properties.Skin + @"\Studios\" + nowPlaying.Studio + ".png";
                        _yatse2Properties.Currently.TvDirector = nowPlaying.Director;
                        _yatse2Properties.Currently.TvYear = nowPlaying.Year.ToString(CultureInfo.InvariantCulture);
                        _yatse2Properties.Currently.TvVotes = "";
                    }
                    VideoStarting();
                    break;
                case "Movie":
                    _yatse2Properties.Currently.Thumb = GetVideoThumbPath(nowPlaying.ThumbURL); // TODO : Change to converter

                    Logger.Instance().Log("Yatse2", "New Movie Media : " + nowPlaying.FileName);
                    var movieinfo = _database.GetMovieFromFile(_remoteInfo.Id, nowPlaying.FileName);
                    _yatse2Properties.Currently.IsMovie = true;
                    if (movieinfo.Count > 0)
                    {
                        _yatse2Properties.Currently.MovieTitle = movieinfo[0].Title;
                        _yatse2Properties.Currently.Fanart = GetVideoFanartPath(movieinfo[0].Fanart); // TODO : Change to Covnerter
                        _yatse2Properties.Currently.MovieYear = movieinfo[0].Year.ToString(CultureInfo.InvariantCulture);
                        _yatse2Properties.Currently.MoviePlot = movieinfo[0].Plot;
                        _yatse2Properties.Currently.MovieDirector = movieinfo[0].Director;
                        _yatse2Properties.Currently.MovieNote = movieinfo[0].Rating;
                        _yatse2Properties.Currently.MovieVotes = movieinfo[0].Votes + " " + GetLocalizedString(82);
                        _yatse2Properties.Currently.MovieStudio = Helper.SkinPath + _yatse2Properties.Skin + @"\Studios\" + movieinfo[0].Studio + ".png";
                    }
                    else
                    {
                        _yatse2Properties.Currently.MovieTitle = nowPlaying.Title;
                        _yatse2Properties.Currently.Fanart = GetVideoFanartPath(nowPlaying.FanartURL); // TODO : Change to Converter
                        _yatse2Properties.Currently.MovieYear = nowPlaying.Year.ToString(CultureInfo.InvariantCulture);
                        _yatse2Properties.Currently.MoviePlot = nowPlaying.Plot;
                        _yatse2Properties.Currently.MovieDirector = nowPlaying.Director;
                        _yatse2Properties.Currently.MovieNote = nowPlaying.Rating;
                        _yatse2Properties.Currently.MovieStudio = Helper.SkinPath + _yatse2Properties.Skin + @"\Studios\" + nowPlaying.Studio + ".png";
                    }

                    VideoStarting();
                    break;
                case "Unknown":
                    _yatse2Properties.Currently.IsUnknown = true;
                    _yatse2Properties.Currently.Thumb = GetVideoThumbPath(nowPlaying.ThumbURL); // TODO : Change to converter
                    _yatse2Properties.Currently.UnknownFile = nowPlaying.FileName;
                    _yatse2Properties.Currently.Fanart = GetVideoFanartPath(nowPlaying.FanartURL); // TODO : Change to Covnerter

                    VideoStarting();
                    break;
                default:
                    break;
            }
        }

        private void ResfreshCurrently()
        {
            if (_remoteInfo == null || !_remoteConnected)
                return;
            
            var nowPlaying = _remote.Player.NowPlaying(true);
            if (nowPlaying.IsNewMedia && (nowPlaying.IsPlaying || nowPlaying.IsPaused) && !String.IsNullOrEmpty(nowPlaying.FileName))
            {
                UpdateCurrently(nowPlaying);
            }
            if ((nowPlaying.IsPlaying || nowPlaying.IsPaused))
            {
                btn_Header_Remotes.Background = GetSkinImageBrush("Menu_Remote_Connected_Playing");
                _yatse2Properties.Currently.Progress = nowPlaying.Progress;
                _yatse2Properties.Currently.Volume = nowPlaying.Volume;
                _yatse2Properties.Currently.Time = nowPlaying.Time.ToString();
                _yatse2Properties.Currently.Duration = nowPlaying.Duration.ToString();
                _yatse2Properties.Currently.IsPlaying = nowPlaying.IsPlaying;
                _yatse2Properties.Currently.IsPaused = nowPlaying.IsPaused;
                if (nowPlaying.IsPlaying)
                {
                    if (_config.MusicFanartRotation)
                        if (nowPlaying.MediaType == "Audio" && _timerHeader == 1)
                        {
                            _yatse2Properties.Currently.Fanart = GetRandomImagePath(Helper.CachePath + @"Music\Fanarts");
                        }
                }
            }
            else
            {
                if (_isPlaying)
                {
                    _isPlaying = false;
                    if (_remote.IsConnected())
                        btn_Header_Remotes.Background = GetSkinImageBrush("Menu_Remote_Connected");
                    else
                        btn_Header_Remotes.Background = GetSkinImageBrush("Menu_Remote_Disconnected");
                    if (_config.GoHomeOnEndPlayback)
                        ShowHome();
                    else
                    {
                        if (_currentGrid == grd_Currently)
                        {
                            Logger.Instance().Log("Debug", "Nothing playing go Back");
                            GoBack();
                        }
                    }
                }
                _yatse2Properties.Currently.IsNothing = true;
                if ((_config.Currently || _config.CurrentlyMovie) && ((grd_Dimming.Visibility != Visibility.Visible) && (grd_Diaporama.Visibility != Visibility.Visible)))
                    _isScreenSaver = false;
            }
        }

        private void UpdateRemote()
        {
            if (_remote == null)
                return;
            if (!_remoteConnected && _remote.IsConnected() && !_failedRemoteCheck)
            {
                Logger.Instance().Log("Yatse2", "Remote connected : " + _remoteInfo.Id + " - " + _remoteInfo.Name + " (" + _remoteInfo.Api + " / " + _remoteInfo.Version + ")");
                var check = _remote.CheckRemote(_remoteInfo.OS, _remoteInfo.Version, _remoteInfo.Additional, _config.ForceOnCheckRemote);
                if (!check)
                {
                    Logger.Instance().Log("Yatse2", "Remote " + _remoteInfo.Id + " - " + _remoteInfo.Name + " : Failed check, some params have changed" + " (" + _remoteInfo.Api + " / " + _remoteInfo.Version + ")");
                    ShowOkDialog(GetLocalizedString(105));
                    _failedRemoteCheck = true;
                    if (_currentGrid != grd_Remotes)
                        ShowGrid(grd_Remotes, false);
                    return;
                }
                if (_currentGrid == grd_Remotes)
                {
                    ShowGrid(grd_Home);
                }
                ShowPopup(GetLocalizedString(96) + " " + _remoteInfo.Name);
                btn_Header_Remotes.Background = GetSkinImageBrush("Menu_Remote_Connected");
                _remoteConnected = true;

                if (!_remoteLibraryRefreshed)
                {
                    RefreshLibrary();
                }
                else
                {
                    if (_remoteInfo.CacheFilled == 0 && _remote.File.AsyncDownloadFinished())
                    {
                        RefreshThumbsFanarts();
                        ShowPopup(GetLocalizedString(101));
                    }
                }
            }

            if (!_remoteConnected && !_showRemoteSelect && _config.IsConfigured && _config.DefaultRemote != 0)
            {
                if (_timer > 20)
                {
                    _showRemoteSelect = true;
                    ShowGrid(grd_Remotes);
                }
            }

            if (_remoteInfo != null)
            {
                if (_remote.File.AsyncDownloadFinished() && _remoteInfo.CacheFilled == 0 && _remote.IsConnected())
                {
                    Logger.Instance().Log("Yatse2", "Image cache filling completed");
                    _remoteInfo.CacheFilled = 1;
                    _yatse2Properties.IsSyncing = false;
                    _database.UpdateRemote(_remoteInfo);
                    ShowPopup(GetLocalizedString(102));
                    ShowHome();
                    _audioSongsDataSource.Clear();
                    _audioGenresDataSource.Clear();
                    _audioArtistsDataSource.Clear();
                    _moviesDataSource.Clear();
                    _tvShowsDataSource.Clear();
                    _tvSeasonsDataSource.Clear();
                    _tvEpisodesDataSource.Clear();
                }
            }

            if (_remoteConnected && !_remote.IsConnected())
            {
                if (_remoteInfo != null)
                {
                    Logger.Instance().Log("Yatse2",
                                          "Remote disconnected : " + _remoteInfo.Id + " - " + _remoteInfo.Name + " (" +
                                          _remoteInfo.Api + " / " + _remoteInfo.Version + ")");
                    ShowPopup(GetLocalizedString(97) + " " + _remoteInfo.Name);
                    _remote.File.StopAsync();
                    _yatse2Properties.IsSyncing = false;
                }
                btn_Header_Remotes.Background = GetSkinImageBrush("Menu_Remote_Disconnected");
                _remoteConnected = false;
                _isPlaying = false;
                ShowHome();
                _yatse2Properties.Currently.IsNothing = true;
            }
            else
            {
                ResfreshCurrently();
            }

        }

        private void RefreshRemotes()
        {
            var remotes = _database.SelectAllRemote();
            lst_Remotes.Items.Clear();

            foreach (var remote in remotes)
            {
                remote.IsDefault = _config.DefaultRemote == remote.Id ? 1 : 0;
                remote.IsSelected = _currentRemoteId == remote.Id ? 1 : 0;
                var index = lst_Remotes.Items.Add(remote);

                if (_currentRemoteId == remote.Id)
                {
                    lst_Remotes.SelectedIndex = index;
                }

            }

            if (remotes.Count > 0)
            {
                if (lst_Remotes.SelectedIndex == -1)
                    lst_Remotes.SelectedIndex = 0;
                brd_Remotes_Actions.Visibility = Visibility.Visible;
                brd_Remotes_NoActions.Visibility = Visibility.Hidden;
            }
            else
            {
                brd_Remotes_Actions.Visibility = Visibility.Hidden;
                brd_Remotes_NoActions.Visibility = Visibility.Visible;
            }
        }
    }
}