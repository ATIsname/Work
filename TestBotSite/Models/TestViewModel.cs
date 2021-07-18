using System.Collections.Generic;
using TestLibrary.Test;

namespace TestBotSite.Models
{
    public class TestViewModel
    {
        public List<Test> tests;
        public Question currentQuestion;
        public Test currentTest;

        public TestViewModel(List<Test> tests, Question currentQuestion, Test currentTest)
        {
            this.tests = tests;
            this.currentTest = currentTest;
            this.currentQuestion = currentQuestion;
        }

        public TestViewModel()
        {
        }
    }
}
