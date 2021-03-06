using Android.App;
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
using StageEteMob.Resources.script;
using Google.Android.Material.Tabs;
using System.Timers;

namespace StageEteMob
{
    public class SignInFrag : AndroidX.Fragment.App.Fragment
    {
        struct incUser
        {
            public string login;
            public string password;
            public string code;
        };
        TextView feedbackOut;
        Timer timer = new Timer(5000);
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var fragmentView = inflater.Inflate(Resource.Layout.content_signin, container, false);

            TabLayout tab = Activity.FindViewById<TabLayout>(Resource.Id.tabLayout1);
            //don't want to see the tab navigation at sign in
            tab.Visibility = ViewStates.Invisible;

            MaterialButton btn = fragmentView.FindViewById<MaterialButton>(Resource.Id.signinBtn);
            btn.Click += initEvent;

            feedbackOut = fragmentView.FindViewById<TextView>(Resource.Id.feedbackTV);


            return fragmentView;
        }
        private void initEvent(object sender, EventArgs eventArgs)
        {
            AppCompatEditText login = View.FindViewById<AppCompatEditText>(Resource.Id.login);
            AppCompatEditText password = View.FindViewById<AppCompatEditText>(Resource.Id.password);

            //temp auth assignment, just so I don't have to write it evertime
            login.Text = TempAuth.logpwd;
            password.Text = TempAuth.logpwd;



            var auth = new
            {
                login = login.Text,
                password = password.Text
            };

            var json = JsonConvert.SerializeObject(auth);
            //Console.WriteLine("### serialized object output:: " + json);

            midSync(json);
            Console.WriteLine(login.Text + " / " + password.Text);
        }

        private async void midSync(string val)
        {
            //await CallAPI();

            await SetAPI(val);
        }

      
        private async Task SetAPI(string valueToSend)
        {
            string uri = "";
            //only for testing with current emulator
            if (Build.Hardware.Contains("ranchu"))
                uri = "https://10.0.2.2:44317/api/User";
            else
                uri = "https://192.168.1.2:45456/api/User";

            //bypassing SSLHandshakeException
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            var client = new HttpClient(clientHandler);
            var httpContent = new StringContent("");

            var result = await client.PostAsync(uri + "?mobSign=" + valueToSend, httpContent);
            //this how to retrieve the returned string from the POST call
            var fromPost = await result.Content.ReadAsStringAsync();

            //if cred are wrong or user doesnt exist
            if (fromPost == "\"none\"")
            {
                Console.WriteLine("************** FAILED ");
                feedbackOut.Text = "User not found";
                
                timer.Elapsed += OnTimeOut;
                timer.AutoReset = true;
                timer.Enabled= true;
                return;
            }

            //no backslashes, for deserialization
            var JsonString = JsonConvert.DeserializeObject<string>(fromPost);

            //make the json better formatted and save it
            string prettyJson = JToken.Parse(JsonString).ToString(Formatting.Indented);
            GlobVars.userJson = prettyJson;

            //deserialize into struct
            incUser user = JsonConvert.DeserializeObject<incUser>(JsonString);
            Console.WriteLine("************** SUCCESS ");

            //Activity instrad fragmentView cause it seems like Activity has global access to all resources?
            TabLayout tab = Activity.FindViewById<TabLayout>(Resource.Id.tabLayout1);
            //make the tablayout in MainaAtivity visible now
            tab.Visibility = ViewStates.Visible;
            //replace the login fragment
            Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.containerView, new SetFrag()).Commit();

            if (result.IsSuccessStatusCode)
            {
                var tokenJson = await result.Content.ReadAsStringAsync();
            }
            else
                Console.WriteLine("************** FAILED: " + result.ReasonPhrase);
        }

        void OnTimeOut(Object source, ElapsedEventArgs e)
        {
            feedbackOut.Text = "";
        }
    }
}