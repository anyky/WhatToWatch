using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WhatToWatch.Handlers;
using WhatToWatch.Service;

namespace WhatToWatch.Repository
{
    public class AccountRepository
    {
        private readonly HttpClient httpClient;

        private string user = "";
        public bool LoggedIn => user != "";

        public string Username => user;

        public bool Admin
        {
            get;
            private set;
        }

        public AccountRepository(HttpClient http)
        {
            httpClient = http;
        }

        public string RegisterUser(string username, string password1, string password2, string email)
        {
            try
            {
                Task<string> task = Task.Run(() => RegisterUserTask(username, password1, password2, email));
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task<string> RegisterUserTask(string username, string password1, string password2, string email)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password1", password1),
                new KeyValuePair<string, string>("password2", password2),
                new KeyValuePair<string, string>("email", email)
            });

            Task<HttpResponseMessage> postResponse = httpClient.PostAsync(DataService.LanguagePrefix + "register/", formContent);

            HttpResponseMessage response = await postResponse;

            var responseString = await response.Content.ReadAsStringAsync();

            if (responseString == "OK")
            {
                return "OK";
            }
            else
            {
                try
                {
                    JObject jsonobject = (JObject)JsonConvert.DeserializeObject(responseString);
                    JObject json = jsonobject.GetValue("errors") as JObject;
                    string withHtml = "";
                    var val = json?.GetValue("username");
                    if (val != null)
                        withHtml += val;
                    val = json?.GetValue("password1");
                    if (val != null)
                        withHtml += val;
                    val = json?.GetValue("password2");
                    if (val != null)
                        withHtml += val;
                    val = json?.GetValue("email");
                    if (val != null)
                        withHtml += val;
                    withHtml = withHtml.Replace("&#39;", "'").Replace("</li>", "\n");
                    string result = ClearHtml(withHtml);
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(AppResources.InvalidServerAnswer);
                }
            }
        }

        public string LogInUser(string username, string password)
        {
            try
            {
                Task<string> task = Task.Run(() => LogInUserTask(username, password));
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task<string> LogInUserTask(string username, string password)
        {
            string responseString = "";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            Task<HttpResponseMessage> postResponse = httpClient.PostAsync(DataService.LanguagePrefix + "login/", formContent);

            HttpResponseMessage response = await postResponse;

            responseString = await response.Content.ReadAsStringAsync();

            if(responseString == "OK")
            {
                user = username;
                Admin = false;
                return "OK";
            }
            else if(responseString == "ADMIN")
            {
                user = username;
                Admin = true;
                return "ADMIN";
            }
            else
            {
                try
                {
                    JObject jsonobject = (JObject)JsonConvert.DeserializeObject(responseString);
                    string result = ClearHtml(jsonobject.GetValue("login_error").ToString());
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(AppResources.InvalidServerAnswer);
                }
            }
        }

        public void LogOutUser()
        {
            try
            {
                Task.Run(() => LogOutUserTask()).Wait();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw ex;
            }
        }

        private async Task LogOutUserTask()
        {
            Task<HttpResponseMessage> getResponse = httpClient.GetAsync(DataService.LanguagePrefix + "logout/");

            await getResponse;

            user = "";
        }
        
        private string ClearHtml(string html)
        {
            return Regex.Replace(html, "<[^>]+>", string.Empty);
        }
    }
}
