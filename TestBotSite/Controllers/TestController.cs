using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestBotSite.Models;
using TestBotSite.Utils;
using TestLibrary.Test;

namespace TestBotSite.Controllers
{
    public class TestController : Controller
    {
        public async Task<ActionResult> Index(int idOfQuestion, int idOfTest)
        {
            try
            {
                TestViewModel model = await GetModel(idOfQuestion, idOfTest);
                return View("Index", model);
            }
            catch (FormatException)
            {
                TestViewModel model = await GetModel(idOfQuestion, idOfTest);
                return View("Index", model);
            }
        }

        private async Task<TestViewModel> GetModel(int idOfQuestion, int idOfTest)
        {
            List<Test> tests = await FileUtils.GetTests();
            if (tests == null)
            {
                return new TestViewModel(new List<Test>(), null, null);
            }
            if (tests.Any())
            {
                Test currentTest = tests.FirstOrDefault(o => o.ID == idOfTest);
                if (currentTest != null)
                {
                    Question currentQuestion = currentTest.Questions.FirstOrDefault(o => o.ID == idOfQuestion);
                    if (currentQuestion != null)
                    {
                        return new TestViewModel(tests, currentQuestion, currentTest);
                    }
                    return new TestViewModel(tests, null, currentTest);
                }
            }
            return new TestViewModel(tests, null, null);
        }

        public async Task<IActionResult> CreateTest()
        {
            try
            {
                List<Test> tests = await FileUtils.GetTests();
                int testId;
                if (tests == null)
                {
                    tests = new List<Test>();
                }
                if (tests.Any())
                    testId = tests.Max(o => o.ID);
                else
                    testId = 0;
                ++testId;
                string testName = $"Test №{testId}";
                int questionId = 1;
                string questionContent = $"Question №{questionId}";
                List<Question> questions = new List<Question>() { new Question(questionContent, questionId) };
                Test test = new Test(testName, questions, testId, false);
                tests.Add(test);
                await FileUtils.SetTests(tests);
                return RedirectToAction(nameof(Index), new { idOfTest = testId });
            }
            catch
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> CreateQuestion(int testId)
        {
            try
            {
                List<Test> tests = await FileUtils.GetTests();
                if (tests == null || tests.FirstOrDefault() == null)
                {
                    return RedirectToAction(nameof(CreateTest));
                }
                Test test = tests.FirstOrDefault(o => o.ID == testId);
                ThrowExceptionIfTestIsNull(test);
                int questionId = test.Questions.Max(o => o.ID);
                ++questionId;
                string questionContent = $"Question №{questionId}";
                test.Questions.Add(new Question(questionContent, questionId));
                await FileUtils.SetTests(tests);
                return RedirectToAction(nameof(Index), new { idOfTest = testId, idOfQuestion = questionId });
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> EditTestName()
        {
            int testID = 1;
            try
            {
                int testId = Convert.ToInt32(Request.Form["currentTest.ID"].ToString());
                string name = Request.Form["currentTest.NameOfTest"].ToString();
                testID = testId;
                List<Test> tests = await FileUtils.GetTests();
                Test test = tests.FirstOrDefault(o => o.ID == testId);
                test.NameOfTest = name;
                await FileUtils.SetTests(tests);
                return RedirectToAction(nameof(Index), new { idOfTest = testID, idOfQuestion = 1 });
            }
            catch (FormatException)
            {
                return RedirectToAction(nameof(Index), new { idOfTest = testID, idOfQuestion = 1 });
            }
        }

        public async Task<IActionResult> EditQuestionContent()
        {
            int testID = 1;
            int questionID = 1;
            try
            {
                int testId = Convert.ToInt32(Request.Form["currentTest.ID"].ToString());
                int questionId = Convert.ToInt32(Request.Form["currentQuestion.ID"].ToString());
                string content = Request.Form["currentQuestion.QuestionContent"].ToString();
                testID = testId;
                questionID = questionId;
                List<Test> tests = await FileUtils.GetTests();
                Test test = tests.FirstOrDefault(o => o.ID == testId);
                Question question = test.Questions.FirstOrDefault(o => o.ID == questionID);
                question.QuestionContent = content;
                await FileUtils.SetTests(tests);
                return RedirectToAction(nameof(Index), new { idOfTest = testID, idOfQuestion = questionID });
            }
            catch (FormatException)
            {
                return RedirectToAction(nameof(Index), new { idOfTest = testID, idOfQuestion = questionID });
            }
        }

        private void ThrowExceptionIfQuestionIsNull(Question question, Test test)
        {
            if (question == null)
            {
                throw new Exception($"There is not such question in {test.NameOfTest} test");
            }
        }

        private void ThrowExceptionIfTestIsNull(Test test)
        {
            if (test == null)
            {
                throw new Exception("There is no such test");
            }
        }

        public async Task<IActionResult> DeleteTest(int testId)
        {
            try
            {
                List<Test> tests = await FileUtils.GetTests();
                if (tests == null)
                {
                    throw new Exception("Тесты отсутствуют");
                }
                if (tests.Any(o => o.ID == testId))
                    tests.Remove(tests.FirstOrDefault(o => o.ID == testId));
                else
                    throw new Exception();
                await FileUtils.SetTests(tests);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> DeleteQuestion(int testId, int questionId)
        {
            try
            {
                List<Test> tests = await FileUtils.GetTests();
                if (tests == null)
                {
                    throw new Exception("Тесты отсутствуют");
                }
                Test test = tests.FirstOrDefault(o => o.ID == testId);
                if (test == null)
                    throw new Exception();
                Question question = test.Questions.FirstOrDefault(o => o.ID == questionId);
                if (question == null)
                    throw new Exception();
                if(test.Questions.Count() < 2)
                {
                    return RedirectToAction(nameof(DeleteTest), new { testId });
                }
                test.Questions.Remove(question);
                await FileUtils.SetTests(tests);
                return RedirectToAction(nameof(Index), new { idOfTest = testId });
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
