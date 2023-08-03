using System;
using System.Collections.Generic;
using System.Text;

namespace bot_misis
{
    public class CommandNames
    {
        public const string COMPLAIN_BUT = "Подать жалобу";
        public const string RULES_BUT = "Правила";
        public const string INFO_BUT = "Ползеная инфо.";
        public const string CONTACTS = "Контакты";

        public const string RULES = "Правила пользование ботом\r\n\r\n" +
            "▪️Для подачи жалоб необходимо корректно пройти регистрацию." +
            "\r\n\r\n▪️Жалобы можно отправлять не чаще 1 раза в 30 минут." +
            "\r\n\r\n▪️При подаче фейковых жалоб ваш аккаунт будет заблокирован." +
            "\r\nДальнейшая подача жалоб будет недоступна.\r\n\r\n" +
            "▪️Для разблокировки аккаунта необходимо обратиться к руководству СОО.\r\n\r\n" +
            "▪️Данный бот предназначен для поддержания комфортных условий для работы и отдыха каждого проживающего.\r\n" +
            "Не стоит подавать жалобы на мелкие нарушения с целью подставить другого человека.\r\n\r" +
            "\n▪️Мы гарантируем конфиденциальность вашего обращения.";
        public const string USEFULL_INFO = "Какая-то полезная информация, которую попросят написать))";


        public const string M_ONE = "Металлург-1";
        public const string M_TWO = "Металлург-2";
        public const string M_THREE = "Металлург-3";
        public const string KOMMYNA = "Дом-Коммуна";
        public const string G_ONE = "Горняк-1";
        public const string G_TWO = "Горняк-2";
        public const string D_FIVE = "ДСГ-5";
        public const string D_SIX = "ДСГ-6";

        public const string BACK = "Назад";
        public const string FLOOR = "Этаж";

        public static HashSet<string> HS_OF_DORMITORY = new HashSet<string>() { M_ONE, M_TWO, M_THREE, KOMMYNA, G_ONE, G_TWO, D_FIVE, D_SIX };
        public static HashSet<string> ARRAY_FLOORS = new HashSet<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };
        public static HashSet<string> HS_OF_QUE = new HashSet<string>() { FIR_QUE, SEC_QUE, THI_QUE, FOU_QUE, FIF_QUE };

        public const string VIOL_NOISY = "Шумно";
        public const string VIOL_SMOKE = "Запах курения";
        public const string VIOL_DIRTY = "Грязь";
        public const string VIOL_NOT_SPECIFIED = "Прочие нарушения";
        public const string VIOL = "Violation";

        public const string RETRY_REG = "Пройти регистрацию заново!";
        public const string END_REG = "Закончить регистрацию";

        public const string PASSWORD = "123";
        public const string START = "/start";

        public const string AGAIN_REG = "Сменить права";
        public const string LIST_OF_APPLIC = "Список заявок";
        public const string FOR_SOO = "Для СОО";

        public const string REG_START = "Начать регистрацию";
        public const string FIR_QUE = "First question";
        public const string SEC_QUE = "Second question";
        public const string THI_QUE = "Third question";
        public const string FOU_QUE = "Fourth question";
        public const string FIF_QUE = "Fifth question";

    }
}
