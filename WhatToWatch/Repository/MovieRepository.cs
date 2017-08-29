using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WhatToWatch.Model;
using WhatToWatch.Service;

namespace WhatToWatch.Repository
{
    public class MovieRepository
    {
        private List<Movie> movies;
        private Movie theMovie;
        private readonly HttpClient httpClient;
        private List<Criteria> criterias;

        public Movie TheMovie => theMovie;
        public List<Movie> Movies => movies;
        public List<Criteria> Criterias => criterias;

        public int RandomMovie { get; set; }
        public int GradedCount { get; set; }
        public int RequireGraded { get; set; }

        public bool NeedQuiz  { get; private set; }
        
        private string ClearHtml(string html)
        {
            return Regex.Replace(html, "<[^>]+>", string.Empty);
        }

        public MovieRepository(HttpClient http)
        {
            httpClient = http;
            movies = new List<Movie>();
            criterias = null;
        }

        public List<Movie> GetFavoriteMovies()
        {
            try
            {
                Task.Run(() => LoadFavoriteMovies()).Wait();

                return movies;
            }
            catch(Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task LoadFavoriteMovies()
        {
            movies = new List<Movie>();

            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "api2/favorites/");
            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                var items = JsonConvert.DeserializeObject<JToken>(responseString);
                foreach (var item in items)
                {
                    Movie movie = new Movie();
                    movie.MovieId = (int)item["id"];
                    movie.Name = (string)item["name"];
                    movie.PosterURL = (string)item["poster"];
                    movie.InFavorite = true;
                    movies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        public async Task<byte[]> DownloadImageBytes(string url)
        {
            return await httpClient.GetByteArrayAsync(url);
        }

        public void ChangeFavorite(int movieId, bool stateToSet)
        {
            try
            {
                Task.Run(() => SendMovieFavorite(movieId, stateToSet)).Wait();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task SendMovieFavorite(int movieId, bool stateToSet)
        {
            Task<HttpResponseMessage> getResponse = httpClient.PostAsync(DataService.LanguagePrefix + "api/favorite/" + (stateToSet ? "add" : "delete") + "/" + movieId + "/", null);

            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(responseString);
                if (json.GetValue("myresponse").ToString() == "OK")
                {
                    if (theMovie != null && theMovie.MovieId == movieId)
                        theMovie.InFavorite = stateToSet;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        internal bool GetMovieFavorite(int movieId)
        {
            return movies.Find(i => i.MovieId == movieId).InFavorite;
        }
        
        public List<Movie> GetNewMovies(int count)
        {
            try
            {
                Task.Run(() => LoadNewMovies(count)).Wait();

                return movies;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task LoadNewMovies(int count)
        {
            movies = new List<Movie>();

            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "api2");

            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                List<JObject> jsonobject = JsonConvert.DeserializeObject<List<JObject>>(responseString);

                foreach (var item in jsonobject)
                {
                    Movie movie = new Movie();
                    movie.MovieId = (int)item["id"];
                    movie.Name = (string)item["name"];
                    movie.PosterURL = (string)item["poster"];
                    movie.InFavorite = true;
                    movies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        } 

        public Movie GetMovieById(int id, bool loggedIn)
        {
            try
            {
                Task.Run(() => LoadMovieById(id, loggedIn)).Wait();
                return theMovie;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task LoadMovieById(int id, bool loggedIn)
        {
            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "api2/films/get/" + id);

            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(responseString);
                Task.Run(() => UnpackMovieJson(json, loggedIn)).Wait();
            }
            catch(Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        public Movie GetRandomMovie(bool loggedIn)
        {
            return GetMovieById(RandomMovie, loggedIn);
        }

        public Movie GetQuizMovie(bool loggedIn)
        {
            try
            {
                Task.Run(() => LoadQuizMovie(loggedIn)).Wait();

                return theMovie;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task LoadQuizMovie(bool loggedIn)
        {
            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "api2/recommend/quiz/");
            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            NeedQuiz = responseString != "[\"End Quiz\"]";
            if (NeedQuiz)
            {
                try
                {
                    JObject json = JsonConvert.DeserializeObject<JObject>(responseString);

                    Task.Run(() => UnpackMovieJson(json, loggedIn)).Wait();

                    RandomMovie = (int)json["random_film"];
                    GradedCount = (int)json["grade_number"];
                    RequireGraded = (int)json["required_graded"];
                }
                catch (Exception ex)
                {
                    throw new Exception(AppResources.InvalidServerAnswer);
                }
            }
            else
                theMovie = null;
        }
        
        private async Task UnpackMovieJson(JObject json, bool loggedIn)
        {
            Movie movie = new Movie();
            movie.MovieId = (int)json["id"];
            movie.Name = (string)json["name"];
            movie.Actors = (string)json["actors"];
            movie.Directors = (string)json["directors"];
            movie.Description = (string)json["description"];
            movie.Duration = (int)json["duration"];
            movie.Year = (int)json["year"];
            movie.PosterURL = (string)json["poster"];
            movie.Genres = (string)json["genres"];

            foreach (var item in json["mark_types"])
            {
                int markid = (int)item["id"];
                movie.AddNewMark(markid, (int)item["type"], (string)item["name"]);
                int usermark;
                float avaragemark;
                if (int.TryParse(item["user_mark"].ToString(), out usermark))
                    movie.AddMarkValue(markid, usermark);
                if(float.TryParse(item["avarage_mark"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out avaragemark))
                    movie.AddMarkAverage(markid, avaragemark);
            }

            foreach (var item in json["comments"])
            {
                movie.AddComment((int)item["id"], (string)item["user"], (string)item["date"], (string)item["text"]);
            }
            
            movie.CanComment = ((int)json["is_able_to_comment"] == 1);

            if (loggedIn)
            {
                Task<HttpResponseMessage> postResponse = httpClient.PostAsync(DataService.LanguagePrefix + "api/favorite/check/" + movie.MovieId + "/", null);
                HttpResponseMessage response = await postResponse;
                var byteArray = await response.Content.ReadAsByteArrayAsync();

                string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                json = JsonConvert.DeserializeObject<JObject>(responseString);

                movie.InFavorite = json["myresponse"].ToString() == "OK";

                foreach (var item in movie.GetComments())
                {
                    postResponse = httpClient.PostAsync(DataService.LanguagePrefix + "api/films/complain_check/" + item.CommentId + "/", null);
                    response = await postResponse;
                    byteArray = await response.Content.ReadAsByteArrayAsync();

                    responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                    json = JsonConvert.DeserializeObject<JObject>(responseString);

                    movie.HasComplaint(item.CommentId, json["myresponse"].ToString() == "OK");
                }
            }

            theMovie = movie;
        }

        public List<Movie> GetMoviesBySearch(string search)
        {
            try
            {
                Task.Run(() => LoadMoviesBySearch(search)).Wait();

                return movies;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task LoadMoviesBySearch(string search)
        {
            movies = new List<Movie>();

            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "api2/search/?query=" + search);

            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                var items = JsonConvert.DeserializeObject<JToken>(responseString);
                foreach (var item in items)
                {
                    Movie movie = new Movie();
                    movie.MovieId = (int)item["id"];
                    movie.Name = (string)item["name"];
                    movie.PosterURL = (string)item["poster"];
                    movie.InFavorite = true;
                    movies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        public void StartRecommendations()
        {
            try
            {
                Task.Run(() => StartRecommendationsTask()).Wait();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task StartRecommendationsTask()
        {
            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "api2/recommend/");
            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            NeedQuiz = (responseString == "\"Need Quiz\"");
            if (!NeedQuiz)
            {
                try
                {
                    var json = JsonConvert.DeserializeObject<JObject[]>(responseString);
                    criterias = new List<Criteria>();
                    foreach (JObject markType in json)
                    {
                        criterias.Add(new Criteria
                        {
                            CriteriaId = (int)markType["id"],
                            CriteriaName = (string)markType["name"],
                            CriteriaType = (int)markType["type"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(AppResources.InvalidServerAnswer);
                }
            }
        }

        public List<Movie> GetRecommendations(int[] criterias)
        {
            try
            {
                Task.Run(() => GetRecommendationTask(criterias)).Wait();

                return movies;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task GetRecommendationTask(int[] criterias)
        {
            FormUrlEncodedContent formContent;
            if (criterias.Length > 0)
                formContent = new FormUrlEncodedContent(criterias.Select(i => new KeyValuePair<string, string>("genres", i.ToString())));
            else
                formContent = new FormUrlEncodedContent(new [] { new KeyValuePair<string, string>("genres", null) });

            Task <HttpResponseMessage> getResponse = httpClient.PostAsync(DataService.LanguagePrefix + "api2/recommend/", formContent);
            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                List<JObject> jsonobject = JsonConvert.DeserializeObject<List<JObject>>(responseString);

                movies = new List<Movie>();
                foreach (var item in jsonobject)
                {
                    Movie movie = new Movie();
                    movie.MovieId = (int)item["id"];
                    movie.Name = (string)item["name"];
                    movie.PosterURL = (string)item["poster"];
                    movie.InFavorite = true;
                    movies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        // marks
        
        public List<Mark> GetMainMovieMarks()
        {
            return theMovie.GetMainMarks();
        }

        public List<Mark> GetEmotMovieMarks()
        {
            return theMovie.GetEmotMarks();
        }
        
        public void ChangeMark(int markId, int markValue)
        {
            try
            {
                Task.Run(() => SendMovieMark(markId, markValue)).Wait();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task SendMovieMark(int markId, int markValue)
        {
            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "api/films/like/" + theMovie.MovieId + "?likes=" + markValue + "&mark_type=" + markId);
            HttpResponseMessage response = await getResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(responseString);
                if (json["myresponse"].ToString() == "OK")
                {
                    theMovie.AddMarkValue(markId, markValue);
                    theMovie.AddMarkAverage(markId, (float)json["avarage_mark"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        // comments

        public List<Commentary> GetComments()
        {
            return theMovie.GetComments();
        }

        public void AddComment(string user, string text)
        {
            try
            {
                Task.Run(() => SendMovieComment(user, text)).Wait();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task SendMovieComment(string user, string text)
        {
            var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("text", text) });

            Task<HttpResponseMessage> postResponse = httpClient.PostAsync(DataService.LanguagePrefix + "api2/films/comment/" + theMovie.MovieId + "/", formContent);

            HttpResponseMessage response = await postResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(responseString);
                int commentsId;
                if (int.TryParse(json["id"].ToString(), out commentsId))
                    theMovie.AddComment(commentsId, (string)json["user"], (string)json["date"], (string)json["text"], true);
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        public void AddComplaint(int commentId)
        {
            try
            {
                Task.Run(() => SendCommentComplaint(commentId)).Wait();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task SendCommentComplaint(int commentId)
        {
            Task<HttpResponseMessage> postResponse = httpClient.PostAsync(DataService.LanguagePrefix + "api/films/complain_comment/" + commentId + "/", null);
            HttpResponseMessage response = await postResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(responseString);
                if (json["myresponse"].ToString() == "OK")
                    theMovie.AddComplaint(commentId);
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }

        public void DeleteComment(int commentId)
        {
            try
            {
                Task.Run(() => SendCommentDelete(commentId)).Wait();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task SendCommentDelete(int commentId)
        {
            Task<HttpResponseMessage> postResponse = httpClient.PostAsync(DataService.LanguagePrefix + "api/films/delete_comment/" + commentId + "/", null);
            HttpResponseMessage response = await postResponse;

            var byteArray = await response.Content.ReadAsByteArrayAsync();
            string responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(responseString);
                if (json["myresponse"].ToString() == "OK")
                    theMovie.DeleteComment(commentId);
            }
            catch (Exception ex)
            {
                throw new Exception(AppResources.InvalidServerAnswer);
            }
        }
    }
}