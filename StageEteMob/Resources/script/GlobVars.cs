﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StageEteMob.Resources.script
{
    internal static class GlobVars
    {
        public static bool devisNameDone = false;
        public static bool devisClientDone = false;
        public static bool devisArticleDone = false;
        public static string userJson { get; set; }
        public static string devisName { get; set; }
        public static Client client { get; set; }

        public static List<Client> listClient = new List<Client>();
        public static List<Article> listArticle = new List<Article>();
        public static List<Article> selectListArticle = new List<Article>();

    }
}