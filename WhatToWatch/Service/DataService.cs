using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using WhatToWatch.Model;
using WhatToWatch.Repository;

namespace WhatToWatch.Service
{
    public class DataService
    {
        private static DataService dataService;
        private readonly MovieRepository movieRepository;
        private readonly AccountRepository accountRepository;

        private readonly HttpClient httpClient;
        private const string baseUrl = "http://what22watch.herokuapp.com/";
        private readonly Dictionary<string, string> languages = new Dictionary<string, string>
        {
            { "English", "en/" },
            { "Українська", "uk/" }
        };
        private static string languagePrefix = "";
        public static string LanguagePrefix
        {
            get { return languagePrefix; }
            set { languagePrefix = dataService.languages[value]; }
        }

        public int GradedCount => movieRepository.GradedCount;

        public int RequireGraded => movieRepository.RequireGraded;

        public Movie TheMovie => movieRepository.TheMovie;

        public List<Movie> Movies => movieRepository.Movies;

        public List<Criteria> Criterias => movieRepository.Criterias;

        public bool LoggedIn => accountRepository.LoggedIn;

        public string Username => accountRepository.Username;

        public bool Admin => accountRepository.Admin;

        public string BaseUrl => baseUrl;

        public bool NeedQuiz => movieRepository.NeedQuiz;

        protected DataService()
        {
            httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            movieRepository = new MovieRepository(httpClient);
            accountRepository = new AccountRepository(httpClient);
        }

        public static DataService Instance()
        {
            CheckInternetConnectivity();
            return dataService ?? (dataService = new DataService());
        }

        private static void CheckInternetConnectivity()
        {
            if (!CrossConnectivity.Current.IsConnected)
                throw new Exception(AppResources.NoInternet);
        }

        public async Task<byte[]> DownloadImageBytes(string url)
        {
            return await httpClient.GetByteArrayAsync(url);
        }

        // Movie data

        public List<Movie> GetNewMovies(int count = 8)
        {
            return movieRepository.GetNewMovies(count);
        }

        public List<Movie> GetMoviesBySearch(string search)
        {
            return movieRepository.GetMoviesBySearch(search);
        }

        public Movie GetMovieById(int id)
        {
            return movieRepository.GetMovieById(id, LoggedIn);
        }

        public Movie GetQuizMovie()
        {
            return movieRepository.GetQuizMovie(LoggedIn);
        }

        public Movie GetRandomMovie()
        {
            return movieRepository.GetRandomMovie(LoggedIn);
        }

        // favorites

        public List<Movie> GetFavoriteMovies()
        {
            return movieRepository.GetFavoriteMovies();
        }

        public bool GetMovieFavorite(int movieId)
        {
            return movieRepository.GetMovieFavorite(movieId);
        }

        public void ChangeFavorite(int movieId, bool stateToSet)
        {
            movieRepository.ChangeFavorite(movieId, stateToSet);
        }

        // marks
        
        public List<Mark> GetMainMovieMarks()
        {
            return movieRepository.GetMainMovieMarks();
        }

        public List<Mark> GetEmotMovieMarks()
        {
            return movieRepository.GetEmotMovieMarks();
        }

        public void ChangeMark(int markId, int markValue)
        {
            movieRepository.ChangeMark(markId, markValue);
        }

        // comments

        public void AddComment(string text)
        {
            movieRepository.AddComment(Username, text);
        }

        public void DeleteComment(int commentId)
        {
            movieRepository.DeleteComment(commentId);
        }

        public List<Commentary> GetComments()
        {
            return movieRepository.GetComments();
        }

        public void AddComplaint(int commentId)
        {
            movieRepository.AddComplaint(commentId);
        }

        // recommendations
        public void StartRecommendations()
        {
            movieRepository.StartRecommendations();
        }

        public void GetRecommendations(int[] criterias)
        {
            movieRepository.GetRecommendations(criterias);
        }

        // Acount data

        public string RegisterUser(string username, string password1, string password2, string email)
        {
            return accountRepository.RegisterUser(username, password1, password2, email);
        }

        public string LogInUser(string username, string password)
        {
            return accountRepository.LogInUser(username, password);
        }

        public void LogOutUser()
        {
            accountRepository.LogOutUser();
        }
    }
}
