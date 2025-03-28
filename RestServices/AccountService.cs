using Android.Animation;
using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using CliReserve.Activities;
using CliReserve.Models;
using CliReserve.RestAPI_Services.Dtos;
using CliReserve.RestServices.Dtos;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CliReserve.RestAPI_Services
{
    public interface IAccountAuth
    {
        Task<User> LoginAsync(LoginDto loginDto);
        Task<User> RegisterAsync(RegisterDto registerDto);
    }
    public class AccountService : IAccountAuth
    {
        private readonly string _authUri = "https://clireserve.azurewebsites.net/api/auth";

        public readonly Context Context;
        
        public AccountService(Context context)
        {
            Context = context;
        }
        //Stores Token Locally
        public void StoreToken(string loginToken)
        {
            ISharedPreferencesEditor editor = Context.ApplicationContext.GetSharedPreferences("auth_token", FileCreationMode.Private).Edit();

            editor.PutString("auth_token", loginToken);
            editor.Commit();
        }

        public static string GetToken(Context context)
        {
            return context.GetSharedPreferences("auth_token", FileCreationMode.Private).GetString("auth_token", "");
        }

        public async Task<User> LoginAsync(LoginDto loginDto)
        {
            Uri requestUri = new Uri($"{_authUri}/login");
            var content = new StringContent(
                JsonConvert.SerializeObject(loginDto),
                Encoding.UTF8,
                "application/json"
            );

            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.PostAsync(requestUri, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var auth_reponseDto = JsonConvert.DeserializeObject<TokenDto>(await response.Content.ReadAsStringAsync());

                        StoreToken(auth_reponseDto.token);
                        return auth_reponseDto.User;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorModel = JsonConvert.DeserializeObject<LoginErrorResponse>(await response.Content.ReadAsStringAsync());

                        foreach (var error in errorModel.Errors)
                        {
                            foreach (var message in error.Value)
                            {
                                Toast.MakeText(Context, message, ToastLength.Short).Show();
                            }
                        }
                    }
                    else
                        Toast.MakeText(Context, await response.Content.ReadAsStringAsync(), ToastLength.Short).Show();
                }
            }
            return null;
        }
        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            Uri requestUri = new Uri($"{_authUri}/register");
            var content = new StringContent(
                JsonConvert.SerializeObject(registerDto),
                Encoding.UTF8,
                "application/json"
            );

            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.PostAsync(requestUri, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var auth_reponseDto = JsonConvert.DeserializeObject<TokenDto>(await response.Content.ReadAsStringAsync());

                        StoreToken(auth_reponseDto.token);
                        return auth_reponseDto.User;
                    }
                    else if( response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        List<RegisterErrorResponse> errorDetails = JsonConvert.DeserializeObject<List<RegisterErrorResponse>>(await response.Content.ReadAsStringAsync());

                        foreach (var errorDetail in errorDetails)
                        {
                            Toast.MakeText(Context, errorDetail.description, ToastLength.Short).Show();
                        }
                    }
                }
            }
            return null;
        }
        public static async Task<UserDto> GetUser(string authToken)
        {
            Uri requestUri = new Uri($"https://clireserve.azurewebsites.net/api/auth");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.GetAsync(requestUri))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());
                }
                return null;
            }
        }
    }
}