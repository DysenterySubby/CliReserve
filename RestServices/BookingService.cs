using CliReserve.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CliReserve.RestServices.Dtos;

namespace CliReserve.RestAPI_Services
{
    internal class BookingService
    {
        private static readonly string _authUri = "https://clireserve.azurewebsites.net/api/book";


        public static async Task<string> BookSeatAsync(string authToken, Seat seat, int stayDuration)
        {
            Uri requestUri = new Uri($"{_authUri}/seat");
            using (HttpClient client = new HttpClient())
            {
                var seatJson = JsonConvert.SerializeObject( new BookingDto
                {
                    SeatId = seat.SeatId,
                    Duration = stayDuration
                });
                var content = new StringContent(seatJson, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.PostAsync(requestUri, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
                return null;
            }
        }
        public static async Task<HttpResponseMessage> UnbookAsync(string authToken)
        {
            Uri requestUri = new Uri($"{_authUri}/cancel");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.PatchAsync(requestUri, new StringContent("", Encoding.UTF8, "application/json")))
                {
                    return response;
                }
            }
        }

        public static async Task<Booking> GetBookingAsync(string authToken)
        {
            Uri requestUri = new Uri($"{_authUri}");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.GetAsync(requestUri))
                {
                    try
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return JsonConvert.DeserializeObject<Booking>(await response.Content.ReadAsStringAsync());

                        }
                    }
                    catch
                    {
                    }
                }
                return null;
            }
        }
        public static async Task<List<BookingHistory>> GetBookingHistoryAsync(string authToken)
        {
            Uri requestUri = new Uri($"{_authUri}/history");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.GetAsync(requestUri))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<List<BookingHistory>>(await response.Content.ReadAsStringAsync());
                    }
                }
                return null;
            }
        }

        public static async Task<HttpResponseMessage> CheckBookingAsync(string authToken, string bookingId)
        {
            Uri requestUri = new Uri($"{_authUri}/check/{bookingId}");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                using (var response = await client.GetAsync(requestUri))
                {
                    return response;
                }

            }
        }

    }
}