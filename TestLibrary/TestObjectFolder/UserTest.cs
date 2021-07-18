using System;
using System.Collections.Generic;

namespace TestLibrary.Test
{
    [Serializable]
    public class UserTest : ICloneable
    {
        private int testID;
        private List<Answer> answers;

        public int TestID { get => testID; set => testID = value; }
        public List<Answer> Answers { get => answers; set => answers = value; }

        public UserTest(int testID)
        {
            TestID = testID;
            Answers = new List<Answer>();
        }

        public UserTest(int testID, List<Answer> answers)
        {
            TestID = testID;
            Answers = answers;
        }

        public UserTest() { }

        public object Clone()
        {
            List<Answer> answers = new List<Answer>();
            foreach (var item in Answers)
            {
                answers.Add(item.Clone() as Answer);
            }
            return new UserTest
            {
                TestID = this.TestID,
                Answers = answers
            };
        }
    }
}
