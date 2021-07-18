using System;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot;
using File = System.IO.File;

namespace TelegramSocialogicalTestBot
{
    internal class MessageHandler
    {
        private string dir;
        private string filePath;
        private TelegramBotClient _client;
        private string content;

        internal MessageHandler(string dir, string filePath, TelegramBotClient client, string content)
        {
            this.dir = dir;
            this.filePath = filePath;
            _client = client;
            this.content = content;
        }

        internal async Task SetMessageFile()
        {
            try
            {
                var file = await _client.GetFileAsync(content);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await _client.GetInfoAndDownloadFileAsync(content, fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error downloading: " + ex.Message);
            }
        }

        internal async Task SetTxtMessageFile()
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error downloading: " + ex.Message);
            }
        }
    }
}
