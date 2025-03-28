using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Android.Widget;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using Android.Nfc.Tech;
using ZXing.Mobile;
using System.Collections.Generic;
using CliReserve.Activities;
using CliReserve.RestAPI_Services;
using Android.Content;

namespace CliReserve.Fragments
{
    public class QRFragment : ZXingScannerFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (hidden)
                StopScanning();
            else
                Scan();
        }
        public override void OnPause()
        {
            StopScanning();

            base.OnPause();
        }
        public override void OnResume()
        {
            base.OnResume();
            Scan();
        }

        void Scan()
        {
            var opts = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZXing.BarcodeFormat> {
                    ZXing.BarcodeFormat.QR_CODE
                },
            };

            StartScanning( async result =>
            {
                // Null result means scanning was cancelled
                if (result == null || string.IsNullOrEmpty(result.Text))
                {
                    Toast.MakeText(Context, "Scanning Cancelled", ToastLength.Long).Show();
                    return;
                }
                // Otherwise, proceed with result

                string authToken = AccountService.GetToken(Context);

                var qrObject = await ClirServices.GetSeatFromQRAsync(authToken, result.Text);

                var seatsJson = JsonConvert.SerializeObject(qrObject.Seats);
                var seatTypesJson = JsonConvert.SerializeObject(qrObject.SeatTypes);

                Intent intent = new Intent(Context, typeof(ReservationActivity));
                intent.PutExtra("SeatTypes", seatTypesJson);
                intent.PutExtra("Seats", seatsJson);
                intent.PutExtra("SeatId", qrObject.SeatId);
                StartActivity(intent);

            }, opts);
        }
    }
}