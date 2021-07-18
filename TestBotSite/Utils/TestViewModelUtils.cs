using System.Linq;
using TestBotSite.Models;

namespace TestBotSite.Utils
{
    public static class TestViewModelUtils
    {
        public static void SetCurrentQuestion(int idOfQuestion, int idOfTest, ref TestViewModel model)
        {
            model.currentQuestion =
                model.tests.First(o => o.ID == idOfTest).Questions.First(o => o.ID == idOfQuestion);
        }
    }
}