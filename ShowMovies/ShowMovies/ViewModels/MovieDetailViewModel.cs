using ShowMovies.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace ShowMovies.ViewModels
{
    [QueryProperty(nameof(MovieId), nameof(MovieId))]
    public class MovieDetailViewModel : BaseViewModel
    {
        public MovieDetailViewModel()
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadMoreReviewsCommand = new Command(async () => await LoadMoreReviews());
        }

        private async Task LoadMoreReviews()
        {
            // lazy load. call API to load the next page when there's less than 5 item in the collection view. return nothing if the last page is passed
            try
            {
                page += 1;
                if (page > lastPage)
                    return;

                var result = await DataStore.GetMovieReviewsAsync(MovieId, page);
                foreach (var item in result.results)
                {
                    Reviews.Add(item);
                }
                Debug.WriteLine($"finished loading page {page} out of {lastPage}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task ExecuteLoadItemsCommand()
        {
            // first load when the page appearing or if refreshed
            IsBusy = true;

            try
            {
                movie = await DataStore.GetMovie(MovieId);
                Debug.WriteLine("Movie item:" + movie.title);
                string YoutubeUrl = await DataStore.GetTrailerUrl(MovieId);
                // change the message located before the trailer, and show the trailer
                if (!string.IsNullOrEmpty(YoutubeUrl))
                {
                    ShowTrailer = true;
                    TrailerMessage = "YouTube Trailer:";
                    GetVideoContent(YoutubeUrl);
                }
                DisplayDetail(movie);

                // set the page and last page to stop the lazy load later
                Reviews.Clear();
                page = 1;
                var userReviews = await DataStore.GetMovieReviewsAsync(MovieId, page);
                lastPage = userReviews.total_pages;
                foreach (var item in userReviews.results)
                {
                    Reviews.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void GetVideoContent(string youtubeUrl)
        {
            // the xamarin community toolkit (xct) having a bug and can't play youtube video (source: github thread)
            // using YoutubeExplode to convert the YouTube link into video stream, and have the xct MediaElement play it
            YoutubeClient youtube = new();
            StreamManifest streamManifest = await youtube.Videos.Streams.GetManifestAsync(youtubeUrl);
            IVideoStreamInfo streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
            if (streamInfo != null)
            {
                VideoUrl = streamInfo.Url;
            }
        }

        private void DisplayDetail(Movie movie)
        {
            // copy movie data to property. binding only works on property with SetProperty or Observable
            ImageUrl = "https://image.tmdb.org/t/p/w400" + movie.poster_path;
            Vote = movie.vote_average.ToString();
            MovieTitle = movie.title;
            Tagline = movie.tagline;
            MovieGenre = string.Empty;
            if (movie.genres.Length > 0)
            {
                foreach (var item in movie.genres)
                {
                    MovieGenre = MovieGenre + " | " + item.name;
                }
            }
            
            ReleasedDate = "Release Date: " + movie.release_date;
            Overview = movie.overview;
            OriginalTitle = "Original Title: " + movie.original_title;
            Language = "Language: " + movie.original_language;
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreReviewsCommand { get; }
        private int page;
        private int lastPage;

        private int movieId;

        public int MovieId
        {
            get { return movieId; }
            set { movieId = value; }
        }

        public Movie movie { get; set; }
        public ObservableCollection<UserReviews> Reviews { get; } = new ObservableCollection<UserReviews>();

        private string trailerMessage = "There's no trailer available.";

        public string TrailerMessage
        {
            get { return trailerMessage; }
            set { SetProperty(ref trailerMessage, value); }
        }

        private string imageUrl;

        public string ImageUrl
        {
            get { return imageUrl; }
            set { SetProperty(ref imageUrl, value); }
        }

        private bool showTrailer = false;

        public bool ShowTrailer
        {
            get { return showTrailer; }
            set { SetProperty(ref showTrailer, value); }
        }

        private string videoUrl;

        public string VideoUrl
        {
            get { return videoUrl; }
            set { SetProperty(ref videoUrl, value); }
        }

        private string vote;

        public string Vote
        {
            get { return vote; }
            set { SetProperty(ref vote, value); }
        }

        private string movieTitle;

        public string MovieTitle
        {
            get { return movieTitle; }
            set { SetProperty(ref movieTitle, value); }
        }

        private string tagline;

        public string Tagline
        {
            get { return tagline; }
            set { SetProperty(ref tagline, value); }
        }

        private string movieGenre;

        public string MovieGenre
        {
            get { return movieGenre; }
            set { SetProperty(ref movieGenre, value); }
        }

        private string releasedDate;

        public string ReleasedDate
        {
            get { return releasedDate; }
            set { SetProperty(ref releasedDate, value); }
        }

        private string overview;

        public string Overview
        {
            get { return overview; }
            set { SetProperty(ref overview, value); }
        }

        private string originalTitle;

        public string OriginalTitle
        {
            get { return originalTitle; }
            set { SetProperty(ref originalTitle, value); }
        }

        private string language;

        public string Language
        {
            get { return language; }
            set { SetProperty(ref language, value); }
        }
    }
}