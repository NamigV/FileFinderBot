using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FileFinder
{
    class NewFilesChecker
    {
        public void CheckForUpdates(DBManager dataBase, int start_page, int last_page, List<int> CoursesID)
        {
            var AlreadyDefinedFiles = dataBase.LoadDefinedFilesIDFromDB();
            Console.WriteLine("ПРОВЕРЯЮ ПО КУРСАМ");
            for (int i = 0; i < CoursesID.Count; i++)
            {
                Parallel.For(start_page, last_page + 1, (int fileID, ParallelLoopState pState) =>
                {
                    if(!AlreadyDefinedFiles.Exists(x => x == fileID))
                    {
                        try
                        {
                            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.misis.ru/courses/" + CoursesID[i] + "/files/" + fileID);
                            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                            if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                            {
                                AlreadyDefinedFiles.Add(fileID);
                                dataBase.SaveFilesIDToDB(CoursesID[i], fileID);
                                Console.WriteLine("https://lms.misis.ru/courses/" + CoursesID[i] + "/files/" + fileID);
                            }
                            myHttpWebResponse.Close();
                        }
                        catch (WebException)
                        {

                        }
                    }
                });
            }
            dataBase.SaveLastFileIDToDB(last_page);
        }

        public int FindLastRelevantPage(int last_page)
        {
            var counterOfExceptions = 0;
            var IsPageRelevant = true;
            Console.WriteLine($"Я ищу последнюю страницу, начиная с {last_page} страницы");
            while(IsPageRelevant)
            {
                try
                {
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://lms.misis.ru/files/" + last_page);
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    last_page++;
                    counterOfExceptions = 0;
                }
                catch (WebException)
                {
                    last_page++;
                    counterOfExceptions++;
                    if(counterOfExceptions == 50)
                    {
                        IsPageRelevant = false;
                        last_page -= 50;
                    }
                }
            }

            return last_page;
        }
    }
}
