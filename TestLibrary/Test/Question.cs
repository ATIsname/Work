using System;

namespace TestLibrary.Test
{
    [Serializable]
    public class Question : IEquatable<Question>
    {
        private int id;
        private string questionContent;

        public int ID { get => id; set => id = value; }
        public string QuestionContent { get => questionContent; set => questionContent = value; }

        public Question(string nameOfQuestion, int id)
        {
            QuestionContent = nameOfQuestion;
            this.id = id;
        }

        public Question() { }

        public bool Equals(Question other)
        {
            return other.QuestionContent == QuestionContent;
        }

        public static bool ObjectIsQuestion(object obj)
        {
            return obj is Question;
        }

        public bool QuestionIsNotNull()
        {
            if (this.ID == 0 || this.QuestionContent == null)
            {
                return false;
            }

            return true;
        }
    }
}