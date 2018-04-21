using System;
using System.IO;
using System.Linq;
using Notifications.Wpf;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LearnWorld
{
    class WordsManager
    {
        private readonly NotificationManager notificationManager = new NotificationManager();
        Random rand = new Random();
        public bool work;

        public char Divider { get; set; }

        public int DisplayTime { get; set; }

        public int DisplayInterval { get; set; }

        public WordsManager(char divider, int displayTime, int displayInterval)
        {
            Divider = divider;
            DisplayTime = displayTime * 1000;
            DisplayInterval = displayInterval * 1000;
        }

        public void ShowWords(string unlearnedFile, MainWindow mw)
        {
            work = true;
            
            Task task = Task.Run(() =>
            {
                while (work)
                {
                    string line = "";
                    string[] wordAndTranslate = new string[2];
                    try
                    {
                        int countOfLines = File.ReadLines(unlearnedFile).Count();
                        line = File.ReadLines(unlearnedFile).ElementAt(rand.Next(1, countOfLines));
                        wordAndTranslate = line.Split(new char[] { Divider }, StringSplitOptions.RemoveEmptyEntries);

                        notificationManager.Show(new NotificationContent
                        {
                            Title = wordAndTranslate[0],
                            Message = wordAndTranslate[1],
                            Type = NotificationType.Information
                        }, expirationTime: TimeSpan.FromMilliseconds(DisplayTime));
                    }
                    catch(FileNotFoundException)
                    {
                        MessageBox.Show("Can't find the file", "Error");
                        mw.ErrorHandler();
                        work = false;
                        return;
                    }
                    catch(IndexOutOfRangeException)
                    {
                        MessageBox.Show("Bad divider", "Error");
                        mw.ErrorHandler();
                        work = false;
                        return;
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(DisplayInterval));
                }
            }); 
        }

        public void Stop()
        {
            work = false;
        }
    }
}
