using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;

namespace FileFinder
{

    class TgBot
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var dataBase = new DBManager();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type != UpdateType.Message)
                return;
            if (update.Message!.Type != MessageType.Text)
                return;
            var message = update.Message;
            var messageSplit = message.Text.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (messageSplit[0].ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat, "Привет! Я - бот, созданный для поиска файлов в системе Canvas вуза МИСиС.\n\n" +
                    "Вот команды, которые я поддерживаю:\n" +
                    "/add [ID] - добавляю ID твоих курсов для дальнейшего поиска по ним файлов(вводите все интересующие курсы через пробел)\n" +
                    "/show [ID] - показываю, какие файлы я нашел по указанному ID курса(для каждого курса требуется отдельная проверка)\n\n" +
                    "Убедительная просьба не вводить несуществующие ID. Я стою на дешёвом хостинге и могу работать медленно :(\n" +
                    "Ответ приходит спустя какое-то время, поэтому просто подождите. Если же ответ не приходит уже давно, то напишите моему создателю, возможно я сломался.\n\n" +
                    "По всем вопросам:\n" +
                    "tg: @namigveliev");
                return;
            }

            if (messageSplit[0] == "/add")
            {
                var outputMessage = "Следующие ID успешно добавлены: ";
                var addCounter = 0;
                for (int i = 1; i < messageSplit.Length; i++)
                {
                    var IsIDRelevant = int.TryParse(messageSplit[i], out var id);
                    if (id < 15000) IsIDRelevant = false;
                    if (IsIDRelevant == true)
                    {
                        dataBase.SaveCoursesIDToDB(int.Parse(messageSplit[i]));
                        outputMessage += $"{messageSplit[i]} ";
                        addCounter += 1;
                    }
                    else await botClient.SendTextMessageAsync(message.Chat,$"Пожалуйста, введите релевантный ID курса.\n" +
                        "ID курса должен состоять только из цифр, его можно найти, перейдя на страницу курса и нажав на строку, содержащую ссылку.\n" +
                        "ID будет находиться после ключевого слова \"course\".\n" +
                        "Также ID не должен быть менее 15000.");
                }
                if(addCounter>0)
                    await botClient.SendTextMessageAsync(message.Chat, outputMessage);
                return;
            }

            if(messageSplit[0] == "/show")
            {
                if (messageSplit.Length == 1)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Требуется ввести ID курса после /show");
                    return;
                }
                var IsIDRelevant = int.TryParse(messageSplit[1], out int id);
                List<int> FoundFiles;
                if (IsIDRelevant == true)
                    FoundFiles = dataBase.LoadFilesIDFromDB(id);
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, $"Пожалуйста, введите релевантный ID курса. \"{messageSplit[1]}\" не является ID курса.\n" +
                        "ID курса должен состоять только из цифр, его можно найти, перейдя на страницу курса и нажав на строку, содержащую ссылку.\n" +
                        "ID будет находиться после ключевого слова\"course\".");
                    return;
                }

                var OutputMessage = $"По курсу {messageSplit[1]} найдены файлы:\n";

                if (FoundFiles.Count == 0) await botClient.SendTextMessageAsync(message.Chat, $"Пока по курсу {messageSplit[1]} не найден ни один файл :(");
                else
                {
                    for (int i = 0; i < FoundFiles.Count; i++)
                    {
                        OutputMessage += $"https://lms.misis.ru/courses/{messageSplit[1]}/files/{FoundFiles[i]}\n";
                    }

                    await botClient.SendTextMessageAsync(message.Chat, OutputMessage);
                    return;
                }
            }
            else await botClient.SendTextMessageAsync(message.Chat, "Я вас не понимаю :(\n" +
                "Воспользуйтесь одной из команд меню /start");

        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
