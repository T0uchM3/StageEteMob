﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Google.Android.Material.Tabs;
using StageEteMob.Resources.script;
using Android.Content.Res;
using Android.Graphics;
using static Android.Views.View;
using System.Net.Mail;

namespace StageEteMob
{
    public class SetFrag : AndroidX.Fragment.App.Fragment
    {
        AppCompatEditText nomET, telET, paysET, mailTF;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var fragmentView = inflater.Inflate(Resource.Layout.content_set, container, false);

            MaterialButton btn = fragmentView.FindViewById<MaterialButton>(Resource.Id.materialButton1);
            btn.Click += initEvent;

            return fragmentView;
        }
        private void initEvent(object sender, EventArgs eventArgs)
        {
            nomET = View.FindViewById<AppCompatEditText>(Resource.Id.nomTF);
            telET = View.FindViewById<AppCompatEditText>(Resource.Id.telTF);
            paysET = View.FindViewById<AppCompatEditText>(Resource.Id.paysTF);
            mailTF = View.FindViewById<AppCompatEditText>(Resource.Id.mailTF);
            TextView clientNameTV = View.FindViewById<TextView>(Resource.Id.clientNameTV);
            TextView clientMailTV = View.FindViewById<TextView>(Resource.Id.clientMailTV);
            TextView clientPNTV = View.FindViewById<TextView>(Resource.Id.clientPNTV);
            TextView clientLocTV = View.FindViewById<TextView>(Resource.Id.clientLocTV);

            bool allGood = false;

            if (nomET.Text.Length > 0)
            {
                clientNameTV.SetTextColor(Color.Rgb(22, 22, 22));
                allGood = true;
            }
            else
            {
                clientNameTV.SetTextColor(Color.Red);
                allGood = false;
            }
            bool pointExist = false;
            try
            {
                var mail = new MailAddress(mailTF.Text);

                pointExist = mail.Host.Contains(".");
            }
            catch
            {
                pointExist = false;
            }
            if (pointExist)
            {
                clientMailTV.SetTextColor(Color.Rgb(22, 22, 22));
                allGood = true;

            }
            else
            {
                clientMailTV.SetTextColor(Color.Red);
                allGood = false;

            }
            if (telET.Text.Length >= 8)
            {
                clientPNTV.SetTextColor(Color.Rgb(22, 22, 22));
                allGood = true;

            }
            else
            {
                clientPNTV.SetTextColor(Color.Red);
                allGood = false;

            }
            if (paysET.Text.Length > 0)
            {
                clientLocTV.SetTextColor(Color.Rgb(22, 22, 22));
                allGood = true;

            }
            else
            {
                clientLocTV.SetTextColor(Color.Red);
                allGood = false;

            }


            if (!allGood)
                return;


            Client client = new Client();


            client.Tel = telET.Text;
            client.Nom = nomET.Text;
            client.Adresse = paysET.Text;
            client.Mail = mailTF.Text;
            client.Mf = "";

            var json = JsonConvert.SerializeObject(client);

            Console.WriteLine("### serialized object output:: " + json);

            midSync(json);
        }
        private async void midSync(string val)
        {
            await SetAPI(val);
        }



        private async Task SetAPI(string valueToSend)
        {
            string uri = "";
            //only for testing with current emulator
            if (Build.Hardware.Contains("ranchu"))
                uri = "https://10.0.2.2:44317/api/Client/Post";
            else
                uri = "https://192.168.1.2:45456/api/Client/Post";

            //bypassing SSLHandshakeException
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            var client = new HttpClient(clientHandler);
            var httpContent = new StringContent("");

            var result = await client.PostAsync(uri + "?mobClient=" + valueToSend, httpContent);
            //this how to retrieve the returned string from the POST call
            var fromPost = await result.Content.ReadAsStringAsync();
            Console.WriteLine(fromPost);
            if (result.IsSuccessStatusCode)
            {
                var tokenJson = await result.Content.ReadAsStringAsync();
            }
            else
                Console.WriteLine("************** FAILED: " + result.ReasonPhrase);


            GlobVars.comingFromSetFrag = true;
            Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.containerView, new SearchFrag()).Commit();

        }
    }
}