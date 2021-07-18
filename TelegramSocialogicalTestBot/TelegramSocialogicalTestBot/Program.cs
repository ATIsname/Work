namespace TelegramSocialogicalTestBot
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            TelegramBotHelper telegramBotHelper = new 
                TelegramBotHelper(token: "1737195701:AAEToBhRYiSKo5NgPC7BGvVhAvtZw00TQZI");
            await telegramBotHelper.GetUpdates();
        }
    }
}
