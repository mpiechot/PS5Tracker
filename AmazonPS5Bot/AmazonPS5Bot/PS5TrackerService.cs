﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Mail;
using System.Timers;
using PS5Bot.Websitetracker;
using NAudio.Wave;
using System.Threading;
using Newtonsoft.Json;

namespace PS5Bot
{
    public class PS5TrackerService
    {
        private readonly System.Timers.Timer timer;
        private readonly Random rand;
        private List<WebsiteTracker> websitesToTrack;
        private List<string> mailAddresses;
        private AccountInfo info;
        //private string ps5Name = "PS5 Konsole Sony PlayStation 5 - Standard Edition, 825 GB, 4K, HDR (Mit Laufwerk)";
        //private Logger log = LogManager.GetCurrentClassLogger();

        //private string amazon_test_url = @"https://www.amazon.de/s?k=Playstation+guthaben&__mk_de_DE=ÅMÅŽÕÑ&ref=nb_sb_noss_2";

        public PS5TrackerService()
        {
            InitSearchSpace();

            websitesToTrack = new List<WebsiteTracker>()
            {
                new AmazonTracker(),
                new MediaMarktTracker(),
                new SaturnTracker(),
                new AlternateTracker(),
                new EuronicsTracker(),
               // new OTTOTracker(@"https://www.otto.de/technik/gaming/playstation/ps5/")
            };
            info = JsonConvert.DeserializeObject<AccountInfo>(File.ReadAllText(@"D:\GitHub\PS5Tracker\AmazonPS5Bot\mail.json"));

            
            Console.WriteLine($"info: {info.address}");
            mailAddresses = new List<string>(info.notifyAddresses);
            // {
            //     "marco.piechotta@gmail.com",
            //     // "soeren.janssen1@gmail.com",
            //     //"Chrcur23@gmail.com",
            //     //"bene_butz@web.de"
            // };
            PlaySentSound();
            SubscribeToTracker();

            rand = new Random();
            timer = new System.Timers.Timer(1000) { AutoReset = true };
            timer.Elapsed += OnTimeout;
        }

        private void SubscribeToTracker()
        {
            foreach(WebsiteTracker tracker in websitesToTrack)
            {
                tracker.InStockEvent += OnInStockEvent;
            }
        }

        private void InitSearchSpace()
        {
            ProductFinder.searchPatterns.Add("(Playstation|PlayStation|PS)\\s{0,1}5 (Spielekonsole|Konsole).*(Standard Edition|Bundle)");
            //ProductFinder.searchPatterns.Add("PSN Guthaben.*50 EUR");
        }

        private void OnTimeout(object sender, ElapsedEventArgs e)
        {
            int nextInterval = rand.Next(1, 5);
            timer.Interval = TimeSpan.FromSeconds(nextInterval).TotalMilliseconds;

            foreach(WebsiteTracker tracker in websitesToTrack)
            {
                tracker.CheckWebsite();
            }
        }

        private void OnInStockEvent(object sender, Product product)
        {
            Console.WriteLine($"{DateTime.Now} | PS5 at: {product.URL}");
            PlaySentSound();
            foreach(string mail in mailAddresses)
            {
                SendMail(mail, product);
            }
        }

        private void SendMail(string toAddressString, Product product)
        {
            MailAddress fromAddress = new MailAddress(info.address, "PS5 Tracker");
            string fromPassword = info.pw;
            
            string name = toAddressString.Substring(0, toAddressString.IndexOf("@"));
            name = name.Replace(".", " ");
            MailAddress toAddress = new MailAddress(toAddressString, name);

            const string subject = "PS5 is available!";
            string body = "PS5 Available at " + product.Shop + "\n" +
                          "- Found at: " + product.URL;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        private static void PlaySentSound()
        {
            using (var audioFile = new AudioFileReader(@"C:\Windows\Media\chord.wav"))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public void Start()
        {
            timer.Start();
        }
        public void Stop()
        {
            timer.Stop();
        }
    }
}
