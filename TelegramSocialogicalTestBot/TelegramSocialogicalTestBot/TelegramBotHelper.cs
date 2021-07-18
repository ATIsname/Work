using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Telegram.Bot.Types;
using System.Text.RegularExpressions;
using Telegram.Bot;
using TestLibrary.Test;

namespace TelegramSocialogicalTestBot
{
    internal class TelegramBotHelper
    {
        private string _token;
        private TelegramBotClient _client;

        public TelegramBotHelper(string token)
        {
            _token = token;
        }

        internal async Task GetUpdates()
        {
            try
            {
                _client = new TelegramBotClient(_token);
                var me = await _client.GetMeAsync();
                if (me != null && !string.IsNullOrEmpty(me.Username))
                {
                    string message;
                    int offset = 0;
                    while (true)
                    {
                        try
                        {
                            var updates = await _client.GetUpdatesAsync(offset);
                            if (updates != null && updates.Count() > 0)
                            {
                                foreach (var update in updates)
                                {
                                    TestLibrary.Test.User user = FileUtils.GetTestObject(update.Message.Chat.Id);
                                    if (user == null)
                                    {
                                        await FileUtils.AddTestObject(update.Message.Chat.Id);
                                        user = FileUtils.GetTestObject(update.Message.Chat.Id);
                                    }
                                    message = await GetMessage(update, user);
                                    await _client.SendTextMessageAsync(update.Message.Chat.Id, message);
                                    offset = update.Id + 1;
                                    user.LastBotMessage = message;
                                    await FileUtils.UpdateTestObject(user);
                                    message = "";
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                Console.WriteLine("me is not set");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task<string> GetMessage(Update update, TestLibrary.Test.User user)
        {
            string message = "";
            string updateMessage = update.Message.Text;
            string lastBotMessage = user.LastBotMessage;
            message = "Добрый день! Прежде, чем приступить к опросу, пожалуйста, \n" +
    " введите данные о себе. Введите любое сообщение.";
            if (string.IsNullOrEmpty(lastBotMessage))
            {
                return message;
            }
            if (string.IsNullOrEmpty(user.Name))
            {
                message += "Введите ваше Ф.И.О. (на русском языке)\n";
                if (lastBotMessage.Contains(message) || lastBotMessage.Contains("Ф.И.О. должно быть написано кирилицей\n")
                    || lastBotMessage.Contains("Ф.И.О. должно быть больше 2 букв\n"))
                {
                    Regex regex = new Regex(@"([А-ЯЁ][а-яё]+[\-\s]?){3,}");
                    Match match = regex.Match(updateMessage);
                    if (updateMessage.Trim().Length >= 3)
                    {
                        if (match.Success)
                        {
                            user.Name = updateMessage;
                        }
                        else
                        {
                            message = "Ф.И.О. должно быть написано кирилицей\n" +
                                "Пример: \"Иванов Иван Иванович\"";
                            return message;
                        }
                    }
                    else
                    {
                        message = "Ф.И.О. должно быть больше 2 букв\n";
                        return message;
                    }
                }
                else
                {
                    return message;
                }
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                message = "Введите вашу электронную почту";
                string wrongMailMessage = "Электронная почта введена некорректно";
                if (lastBotMessage.Contains(message) || lastBotMessage.Contains(wrongMailMessage))
                {
                    Regex regex = new Regex(@"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}");
                    Match match = regex.Match(updateMessage);
                    if (match.Success)
                    {
                        user.Email = updateMessage;
                    }
                    else
                    {
                        return wrongMailMessage;
                    }
                }
                else
                {
                    return message;
                }
            }
            UserTest currentTest = user.CurrentTest;
            if (currentTest == null)
            {
                return await GetMessageIfCurrentTestIsNull(user, updateMessage, lastBotMessage);
            }
            else
            {
                return await GetMessageIfCurrentTestIsNotNull(update, user, currentTest);
            }
        }

        private async Task<string> GetMessageIfCurrentTestIsNotNull
            (Update update, TestLibrary.Test.User user, UserTest currentTest)
        {
            try
            {
                var tests = await FileUtils.GetTests();
                var updateMessage = update.Message.Text;
                var lastBotMessage = user.LastBotMessage;
                var message = "";
                var test = tests.FirstOrDefault(o => o.ID == currentTest.TestID);
                var idOfAnswer = currentTest.Answers.FirstOrDefault(o => !o.IsAnswered).AnswerID;
                var currentQuestionContent = test.Questions.FirstOrDefault(o => o.ID == idOfAnswer).QuestionContent;
                var answer = currentTest.Answers.FirstOrDefault(o => !o.IsAnswered);
                var dir = FileUtils.GetUsersAnswersFolderPath + "\\" + test.ID +
                    "_" + test.NameOfTest + "\\" + user.Email + "_" + user.ID + "\\";
                MessageHandler handler;
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
                var filePath = dir + answer.AnswerID;
                var files = Directory.GetFiles(dir);
                var answerFilePath = files.FirstOrDefault(o => o.Contains(answer.AnswerID.ToString() + "."));
                if (System.IO.File.Exists(answerFilePath))
                {
                    System.IO.File.Delete(answerFilePath);
                }
                switch (update.Message.Type)
                {
                    case Telegram.Bot.Types.Enums.MessageType.Voice:
                        var voice = update.Message.Voice;
                        //if (voice.Duration < 60)
                        //{
                            filePath += ".ogg";
                            handler = new MessageHandler(dir, filePath, _client, voice.FileId);
                            await handler.SetMessageFile();
                            break;
                        //}
                        //else
                        //{
                        //    return "Приносим свои извинения, продолжительность \n" +
                        //        "голосового сообщения должна быть меньше минуты.\n" +
                        //        $"Вопрос: \"{currentQuestionContent}\"";
                        //}
                    case Telegram.Bot.Types.Enums.MessageType.Text:
                        var text = update.Message.Text;
                        //if (text.Length < 256)
                        //{
                            filePath += ".txt";
                            handler = new MessageHandler(dir, filePath, _client, text);
                            await handler.SetTxtMessageFile();
                            break;
                        //}
                        //else
                        //{
                        //    return "Приносим свои извинения, длинна \n" +
                        //        "сообщения должна быть меньше 256 симоволов.\n" +
                        //        $"Вопрос: \"{currentQuestionContent}\"";
                        //}
                    case Telegram.Bot.Types.Enums.MessageType.Video:
                        var video = update.Message.Video;
                        //if (video.Duration < 60)
                        //{
                            filePath += ".mp4";
                            handler = new MessageHandler(dir, filePath, _client, video.FileId);
                            await handler.SetMessageFile();
                            break;
                        //}
                        //else
                        //{
                            //return "Приносим свои извинения, продолжительность \n" +
                            //    "видеосообщения должна быть меньше минуты.\n" +
                            //    $"Вопрос: \"{currentQuestionContent}\"";
                        //}
                    default:
                        return "Приносим свои извинения, введенный \n" +
                            "формат сообщения не поддерживается.\n" +
                            $"Вопрос: \"{currentQuestionContent}\"";
                }
                answer.AnswerConetnt = filePath;
                answer.IsAnswered = true;
                if (!currentTest.Answers.Any(o => !o.IsAnswered))
                {
                    user.AnsweredTests.Add(currentTest.Clone() as UserTest);
                    user.CurrentTest = null;
                    return "Спасибо за ваши ответы! Они очень важны для нас! \n" + GetStringMessageWithListOfTests(tests);
                }
                else
                {
                    message = test.Questions.FirstOrDefault(o => o.ID ==
                    currentTest.Answers.FirstOrDefault(o => !o.IsAnswered).AnswerID).QuestionContent;
                }
                return message;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetMessageIfCurrentTestIsNotNull method, exception: " + e.Message);
                return "Ошибка...Что-то пошло не так, обратитесь в службу поддержки.";
            }

        }

        private async Task<string> GetMessageIfCurrentTestIsNull(TestLibrary.Test.User user, string updateMessage, string lastBotMessage)
        {
            List<Test> tests = await FileUtils.GetTests();
            Test test;
            string message = "";
            if (lastBotMessage.Contains("Введите номер опроса:") ||
                lastBotMessage.Contains("Теста с таким номером не существует.") ||
                lastBotMessage.Contains("Номер теста должен состоять из цифр."))
            {
                int id = 0;
                if (Int32.TryParse(updateMessage, out id))
                {
                    test = tests.FirstOrDefault(o => o.ID == id);
                    if (test == null)
                    {
                        message = "Теста с таким номером не существует.";
                        return message;
                    }
                    if (user.AnsweredTests.Any(o => o.TestID == id))
                    {
                        user.AnsweredTests.Remove(user.AnsweredTests.First(o => o.TestID == id));
                    }
                    SetAnswers(test, user);
                    message = test.NameOfTest + "\n";
                    message += test.Questions[0].QuestionContent;
                    return message;
                }
                else
                {
                    message = "Номер теста должен состоять из цифр.";
                    return message;
                }

            }
            if (tests.Count == 0)
            {
                message = "Приносим свои извинения, опросы сейчас не доступны. \n" +
                    " Попробуйте написать нам позже.";
                return message;
            }
            if (tests.Count == 1)
            {
                test = tests.First();
                if (user.AnsweredTests.Any(o => o.TestID == test.ID))
                {
                    user.AnsweredTests.Remove(user.AnsweredTests.First(o => o.TestID == test.ID));
                }
                SetAnswers(test, user);
                message = test.NameOfTest + "\n";
                message += test.Questions[0].QuestionContent;
                return message;
            }
            else
            {
                message += GetStringMessageWithListOfTests(tests);
                return message;
            }
        }

        private void SetAnswers(Test test, TestLibrary.Test.User user)
        {
            List<Answer> answers = new List<Answer>();
            foreach (var item in test.Questions)
            {
                answers.Add(new Answer(item.ID, item.QuestionContent, false));
            }
            user.CurrentTest = new UserTest(test.ID, answers);
        }

        private string GetStringMessageWithListOfTests(List<Test> tests)
        {
            string message = "Введите номер опроса: \n";
            for (int i = 0; i < tests.Count; i++)
            {
                message += tests[i].ID + ". " + tests[i] + "\n";
            }
            return message;
        }
    }
}