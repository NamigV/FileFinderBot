using System.Threading.Tasks;
using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace FileFinder
{
    class Program
    {
        readonly static ITelegramBotClient bot = new TelegramBotClient("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

        static void Main()
        {
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            bot.StartReceiving(
                TgBot.HandleUpdateAsync,
                TgBot.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );


            var t = Task.Run(async delegate
            {
                await Task.Delay(3600000);
            });

            var dataBase = new DBManager();
            var check = new NewFilesChecker();

            while (true)
            {
                var start_page = dataBase.LoadLastFilesIDFromDB();
                var coursesID = dataBase.LoadCoursesIDFromDB();
                var last_page = check.FindLastRelevantPage(start_page);

                check.CheckForUpdates(dataBase, start_page, last_page, coursesID);
                t.Wait();
            }
        }
    }
}
