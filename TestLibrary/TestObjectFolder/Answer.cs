using System;

namespace TestLibrary.Test
{
    [Serializable]
    public class Answer : ICloneable
    {
        private int answerID;
        private string answerConetnt;
        bool isAnswered;

        public int AnswerID { get => answerID; set => answerID = value; }
        public string AnswerConetnt { get => answerConetnt; set => answerConetnt = value; }
        public bool IsAnswered { get => isAnswered; set => isAnswered = value; }

        public Answer(int answerID, string answerConetnt, bool isAnswered)
        {
            AnswerID = answerID;
            AnswerConetnt = answerConetnt;
            IsAnswered = isAnswered;
        }

        public Answer()
        {

        }

        public object Clone()
        {
            return new Answer
            {
                AnswerID = this.AnswerID,
                AnswerConetnt = this.AnswerConetnt,
                IsAnswered = this.IsAnswered
            };
        }
    }
}
