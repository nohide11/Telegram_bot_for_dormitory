using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.Enums;
using bot_misis.Entities;
using System.Globalization;
using bot_misis.Services;

namespace bot_misis
{
    class Program
    {
        static TelegramBotClient botClient = new TelegramBotClient("5868755837:AAFB5-0SOKR5ZnV3JSvaUw9TqiRvOUPnPuM");
        static string TEMP_ROOM = "";
        static string CORPUS = "";
        static string LAST_CALL = "/start";

        static async Task Main(string[] args)
        {
            var me = await botClient.GetMeAsync();
            botClient.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message.Text != null)
            {
                if (CheckReg(message))
                {
                    if (message.Text == CommandNames.START || message.Text == CommandNames.BACK)
                    {
                        var perm = CheckPrmissions(message);
                        if (!perm)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Выберите действия⬇",
                                replyMarkup: GetButtonsChoose());
                            return;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Выберите действия⬇",
                                replyMarkup: GetButtonsChooseForSOO());
                            return;
                        }
                    }

                    if (LAST_CALL == CommandNames.FOR_SOO)
                    {
                        using (var db = new Services.Context())
                        {
                            var data = (from d in db.Violations select d).Where(x => x.Corpus == message.Text);
                            var list = data.ToList();

                            string mesToSend = "";

                            foreach (var item in list)
                            {
                                mesToSend += item + "\n";
                            }

                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: mesToSend,
                                replyMarkup: new ReplyKeyboardMarkup(CommandNames.BACK) { ResizeKeyboard = true });

                            LAST_CALL = CommandNames.START;
                        }
                    }

                    StartCommand(message);

                    if (LAST_CALL == CommandNames.COMPLAIN_BUT || LAST_CALL == CommandNames.FLOOR || LAST_CALL == CommandNames.VIOL)
                    {
                        if (CommandNames.HS_OF_DORMITORY.Contains(message.Text))
                        {
                            CORPUS = message.Text;
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Выберите этаж⬇",
                                replyMarkup: GetButtonsFloors());
                            return;
                        }
                        else if (CommandNames.ARRAY_FLOORS.Contains(message.Text))
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Выберите комнату⬇",
                                replyMarkup: GetButtonsRooms(message.Text));
                            LAST_CALL = CommandNames.FLOOR;
                            return;
                        }
                        else if (LAST_CALL == CommandNames.FLOOR)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Выберите вид нарушения⬇",
                                replyMarkup: GetButtonsType());
                            TEMP_ROOM = message.Text;
                            LAST_CALL = CommandNames.VIOL;
                        }
                        else if (LAST_CALL == CommandNames.VIOL)
                        {
                            using (var db = new Services.Context())
                            {
                                var viol = new Violations
                                {
                                    UserId = message.Chat.Id,
                                    User = message.Chat.FirstName,
                                    Corpus = CORPUS,
                                    Room = Convert.ToInt16(TEMP_ROOM),
                                    Type = message.Text,
                                    Data = DateTime.Now
                                };
                                db.Violations.Add(viol);
                                db.SaveChanges();
                            }

                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Выше заявление отправлено! \n Вскоре оно будет обработано",
                                replyMarkup: new ReplyKeyboardMarkup(CommandNames.BACK) { ResizeKeyboard = true });

                            LAST_CALL = CommandNames.START;
                        }
                    }
                }
                else
                {
                    if (LAST_CALL == CommandNames.START)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Необходима регистрация!",
                            replyMarkup: new ReplyKeyboardMarkup(CommandNames.REG_START) { ResizeKeyboard = true });
                        LAST_CALL = CommandNames.REG_START;
                        return;
                    }
                    else if (CommandNames.HS_OF_QUE.Contains(LAST_CALL) || message.Text == CommandNames.REG_START)
                    {
                        RegFuncAsync(message);
                        return;
                    }
                }
            }
        }

        private static DateTime EndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        private static DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        private static async Task StartCommand(Message message)
        {
            switch (message.Text)
            {
                case CommandNames.COMPLAIN_BUT:
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Выберите корпус, в котором произошло нарушение!⬇",
                        parseMode: ParseMode.Html,
                        replyMarkup: GetButtonsDormitory());
                    LAST_CALL = CommandNames.COMPLAIN_BUT;
                    break;
                case CommandNames.RULES_BUT:
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: CommandNames.RULES);
                    break;
                case CommandNames.INFO_BUT:
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: CommandNames.USEFULL_INFO);
                    break;
                case CommandNames.CONTACTS:
                    await botClient.SendContactAsync(
                    chatId: message.Chat.Id,
                    phoneNumber: "+7 985 237 6689",
                    firstName: "Решение тех. проблем",
                    vCard: "BEGIN:VCARD\n" +
                    "VERSION:3.0\n" +
                    "ORG: Решение технических проблем в будение дни\n" +
                    "TEL;TYPE=voice,work,pref:+7 985 237 6689\n" +
                    "EMAIL:m2@misis.ru\n" +
                    "END:VCARD");
                    await botClient.SendContactAsync(
                    chatId: message.Chat.Id,
                    phoneNumber: "+7 495 333 5010",
                    firstName: "Решение тех.проб. в выходные",
                    vCard: "BEGIN:VCARD\n" +
                    "VERSION:3.0\n" +
                    "ORG: Решение срочных проблем в выходные дни\n" +
                    "TEL;TYPE=voice,work,pref:+7 495 333 5010\n" +
                    "EMAIL:m2@misis.ru\n" +
                    "END:VCARD");
                    break;
                case CommandNames.FOR_SOO:
                    using (var db = new Services.Context())
                    {
                        var customer = db.Users.Where(x => x.Id.Equals(message.Chat.Id)).FirstOrDefault();

                        if (customer.Permissions)
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Выберите общежитие в котором нужно узнать о нарушениях⬇",
                            parseMode: ParseMode.Html,
                            replyMarkup: GetButtonsDormitory());

                            LAST_CALL = CommandNames.FOR_SOO;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "У вас не достаточно прав!");
                        }
                    }
                    break;
                case CommandNames.AGAIN_REG:
                    string whoIsWho = "";
                    using (var db = new Services.Context())
                    {
                        var customer = db.Users.Where(x => x.Id.Equals(message.Chat.Id)).FirstOrDefault();

                        if (customer.Permissions)
                        {
                            customer.Permissions = false;
                            whoIsWho = "Пользователья";
                        }
                        else
                        {
                            customer.Permissions = true;
                            whoIsWho = "Отрядника";
                        }
                        db.SaveChanges();
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Права успешно изменены на {whoIsWho}"
                        );
                    break;
            }
        }
        
        private static async Task RegFuncAsync(Message message)
        {
            switch (LAST_CALL)
            {
                case CommandNames.REG_START:
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Для того, чтобы зарегестрировать необходимо\n" +
                        " ответить на несколькол вопросов");
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "\U00000031. Сколько этажей в корпусе-Б?");
                    LAST_CALL = CommandNames.SEC_QUE;
                    break;
                case CommandNames.SEC_QUE:
                    if (message.Text == "16")
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "\U00000032. Сколько комнат на этаже в М-2?");
                        LAST_CALL = CommandNames.THI_QUE;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Вы ответили не верно, пока!",
                        replyMarkup: new ReplyKeyboardMarkup(CommandNames.RETRY_REG) { ResizeKeyboard = true });
                        LAST_CALL = CommandNames.START;
                    }
                    break;
                case CommandNames.THI_QUE:
                    if (message.Text == "18")
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "\U00000033. Максимальное количество человек, которое может проживать в блоке?");
                        LAST_CALL = CommandNames.FOU_QUE;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Вы ответили не верно, пока!",
                        replyMarkup: new ReplyKeyboardMarkup(CommandNames.RETRY_REG) { ResizeKeyboard = true });
                        LAST_CALL = CommandNames.START;
                    }
                    break;
                case CommandNames.FOU_QUE:
                    if (message.Text == "5")
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Регистрация прошла успешно!\n" +
                            "Если вы состоите в опер. отряде, то напишите пароль. В противном случае нажмите на кнопку!⬇",
                            replyMarkup: new ReplyKeyboardMarkup(CommandNames.END_REG) { ResizeKeyboard = true });
                        LAST_CALL = CommandNames.FIF_QUE;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Вы ответили не верно, пока!",
                        replyMarkup: new ReplyKeyboardMarkup(CommandNames.RETRY_REG) { ResizeKeyboard = true });
                        LAST_CALL = CommandNames.START;
                    }
                    break;
                case CommandNames.FIF_QUE:
                    bool per = false;
                    if (message.Text == CommandNames.PASSWORD)
                    {
                        per = true;
                    }

                    using (var db = new Services.Context())
                    {
                        var newUser = new Users
                        {
                            Id = message.Chat.Id,
                            Name = message.Chat.FirstName,
                            Permissions = per,
                            BanCond = false
                        };
                        db.Users.Add(newUser);
                        db.SaveChanges();
                    }

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Приступим!⬇",
                        replyMarkup: new ReplyKeyboardMarkup(CommandNames.START) { ResizeKeyboard = true });
                    LAST_CALL = CommandNames.START;
                    break;
            }
        }

        private static bool CheckReg(Message message)
        {
            bool result = false;

            using (var db = new Services.Context())
            {
                var sel = db.Users.Where(x => x.Name.Equals(message.Chat.FirstName));

                if (sel.Count() != 0)
                {
                    result = true;
                }
            }

            return result;
        }

        private static bool CheckPrmissions(Message message)
        {
            bool result = false;

            using (var db = new Services.Context())
            {
                var sel = db.Users.Where(x => x.Name.Equals(message.Chat.FirstName)).Where(x => x.Permissions.Equals(true));

                if (sel.Count() != 0)
                {
                    result = true;
                }
            }

            return result;
        }

        private static IReplyMarkup GetButtonsType()
        {
            return new ReplyKeyboardMarkup
                (
                    new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.VIOL_NOISY), new KeyboardButton(CommandNames.VIOL_SMOKE)},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.VIOL_DIRTY), new KeyboardButton(CommandNames.VIOL_NOT_SPECIFIED)},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.BACK)},
                    }
                )
            { ResizeKeyboard = true };
        }

        private static IReplyMarkup GetButtonsChoose()
        {
            return new ReplyKeyboardMarkup
                (
                    new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.COMPLAIN_BUT), new KeyboardButton(CommandNames.RULES_BUT)},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.INFO_BUT), new KeyboardButton(CommandNames.CONTACTS), new KeyboardButton(CommandNames.AGAIN_REG)},
                    }
                )
            { ResizeKeyboard = true };
        }

        private static IReplyMarkup GetButtonsChooseForSOO()
        {
            return new ReplyKeyboardMarkup
                (
                    new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.COMPLAIN_BUT), new KeyboardButton(CommandNames.RULES_BUT)},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.INFO_BUT), new KeyboardButton(CommandNames.CONTACTS), new KeyboardButton(CommandNames.AGAIN_REG), new KeyboardButton(CommandNames.FOR_SOO)},
                    }
                )
            { ResizeKeyboard = true };
        }

        private static IReplyMarkup GetButtonsFloors()
        {
            return new ReplyKeyboardMarkup
                (
                    new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> {
                            new KeyboardButton("1"), new KeyboardButton("2"), new KeyboardButton("3"), new KeyboardButton("4")},
                        new List<KeyboardButton> {
                           new KeyboardButton("5"), new KeyboardButton("6"), new KeyboardButton("7"), new KeyboardButton("8")},
                        new List<KeyboardButton> {
                            new KeyboardButton("9"), new KeyboardButton("10"), new KeyboardButton("11"), new KeyboardButton("12")},
                        new List<KeyboardButton> {
                            new KeyboardButton("13"), new KeyboardButton("14"), new KeyboardButton("15"), new KeyboardButton("16")},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.BACK)},
                    }
                )
            { ResizeKeyboard = true };
        }

        private static IReplyMarkup GetButtonsRooms(string floor)
        {
            return new ReplyKeyboardMarkup
                (
                    new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> {
                            new KeyboardButton(floor + "01"), new KeyboardButton(floor + "02"), new KeyboardButton(floor + "03"), new KeyboardButton(floor + "04"), new KeyboardButton(floor + "05")},
                        new List<KeyboardButton> {
                           new KeyboardButton(floor + "06"), new KeyboardButton(floor + "07"), new KeyboardButton(floor + "08"), new KeyboardButton(floor + "09"), new KeyboardButton(floor + "10")},
                        new List<KeyboardButton> {
                            new KeyboardButton(floor + "11"), new KeyboardButton(floor + "12"), new KeyboardButton(floor + "13"), new KeyboardButton(floor + "14"), new KeyboardButton(floor + "15")},
                        new List<KeyboardButton> {
                            new KeyboardButton(floor + "16"), new KeyboardButton(floor + "17"), new KeyboardButton(floor + "18"), new KeyboardButton(CommandNames.FLOOR + "-" + floor), new KeyboardButton(CommandNames.BACK)}
                    }
                )
            { ResizeKeyboard = true };
        }

        private static IReplyMarkup GetButtonsDormitory()
        {
            return new ReplyKeyboardMarkup
                (
                    new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.M_ONE), new KeyboardButton(CommandNames.M_TWO), new KeyboardButton(CommandNames.M_THREE)},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.G_ONE), new KeyboardButton(CommandNames.G_TWO), new KeyboardButton(CommandNames.KOMMYNA)},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.D_FIVE), new KeyboardButton(CommandNames.D_SIX)},
                        new List<KeyboardButton> {
                            new KeyboardButton(CommandNames.BACK)},
                    }
                )
            { ResizeKeyboard = true };
        }

        private static Task Error(ITelegramBotClient botClient, Exception update, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
