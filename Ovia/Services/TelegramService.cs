using Telegram.Bot;
using Telegram.Bot.Exceptions;


namespace Ovia.Services
{
    public class TelegramService
    {
        private readonly TelegramBotClient _botClient;

        public TelegramService(TelegramBotClient botClient)
        {
            this._botClient = botClient;   
        }

        public TelegramService()
        {
            var botToken = "6935466790:AAHBegNUuZw8DK2bvNYVfruK4MUGl626l9E";

           


            _botClient = new TelegramBotClient(botToken);


            var x = _botClient.GetMeAsync().Result;
            Console.Write($"Hiiiiiiiiii I am  : {x.Id}   and my name is :{x.FirstName} ");


           
        }


        public async Task SendMessageAsync(string chatId, string message)
        {
            try
            {
                await _botClient.SendTextMessageAsync(chatId, message);
            }
            catch (ApiRequestException ex)
            {
                // Handle Telegram API request exceptions
                Console.WriteLine($"Telegram API error: {ex.Message}");
                throw; // Rethrow the exception to be handled by the caller
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw; // Rethrow the exception to be handled by the caller
            }
        }













        //public async Task SendMessageAsync(string chatId, string message)
        //{
        //    try
        //    {
        //        await _botClient.SendTextMessageAsync(chatId, message);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions, log errors, etc.
        //        Console.WriteLine($"Error sending message: {ex.Message}");
        //    }
        //}


    }
}
