using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CliReserve.RestAPI_Services.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using CliReserve.Models;
using Java.Util.Concurrent;
using Android.Graphics;
using System.Net.Mime;

namespace CliReserve.RestAPI_Services
{
    public class ClirServices
    {
        private static readonly string _authUri = "https://clireserve.azurewebsites.net/api/clir";
        public static async Task<List<Clir>> GetClirsAsync(string authToken)
        {
            Uri requestUri = new Uri(_authUri);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.GetAsync(requestUri))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return JsonConvert.DeserializeObject<List<Clir>>(await response.Content.ReadAsStringAsync());
                }
                return new List<Clir>();
            }
        }
        public static async Task<List<SeatType>> GetSeatTypesAsync(string authToken, Clir clir)
        {
            Uri requestUri = new Uri($"{_authUri}/{clir.ClirName}");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.GetAsync(requestUri))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return JsonConvert.DeserializeObject<List<SeatType>>(await response.Content.ReadAsStringAsync());
                }
                return new List<SeatType>();
            }
        }
        public static async Task<List<Seat>> GetSeatsAsync(string authToken, SeatType seatType)
        {
            Uri requestUri = new Uri($"{_authUri}/{seatType.ClirName}/{seatType.SeatTypeId}");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.GetAsync(requestUri))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return JsonConvert.DeserializeObject<List<Seat>>(await response.Content.ReadAsStringAsync());
                }
                return new List<Seat>();
            }
        }
        public static async Task<QrScanDto> GetSeatFromQRAsync(string authToken, string link)
        {
            Uri requestUri = new Uri(link);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                
                using (var response = await client.GetAsync(requestUri))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return JsonConvert.DeserializeObject<QrScanDto>(await response.Content.ReadAsStringAsync());
                }
                return null;
            }
        }
        public static async Task<byte[]> GetImageAsync(string authToken, SeatType seatType)
        {
            Uri requestUri = new Uri($"{_authUri}/image");
            using (HttpClient client = new HttpClient())
            {
                var imageDto = new ImageRequestDto
                {
                    ClirName = seatType.ClirName,
                    SeatTypeId = seatType.SeatTypeId
                };
                var content = new StringContent(JsonConvert.SerializeObject(imageDto), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.PostAsync(requestUri, content))
                {
                    return JsonConvert.DeserializeObject<byte[]>(await response.Content.ReadAsStringAsync());
                }
            }
        }
        public static async Task<byte[]> GetImageAsync(string authToken, Clir seatType)
        {
            Uri requestUri = new Uri($"{_authUri}/image");
            using (HttpClient client = new HttpClient())
            {
                var imageDto = new ImageRequestDto
                {
                    ClirName = seatType.ClirName,
                    SeatTypeId = ""
                };
                var content = new StringContent(JsonConvert.SerializeObject(imageDto), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.PostAsync(requestUri, content))
                {
                    return JsonConvert.DeserializeObject<byte[]>(await response.Content.ReadAsStringAsync());
                }
            }
        }
    }
}