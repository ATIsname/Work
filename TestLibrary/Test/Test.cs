using System;
using System.Collections.Generic;

namespace TestLibrary.Test
{
    [Serializable]
    public class Test : IEquatable<Test>
    {
        private int id;
        private string nameOfTest;
        private List<Question> questions;
        private bool inUse;

        public int ID { get => id; set => id = value; }

        public string NameOfTest { get => nameOfTest; set => nameOfTest = value; }

        public List<Question> Questions { get => questions; set => questions = value; }

        public bool InUse { get => inUse; set => inUse = value; }

        public Test(string nameOfTest, List<Question> questions, int id, bool inUse)
        {
            ID = id;
            NameOfTest = nameOfTest;
            Questions = questions;
            InUse = inUse;
        }

        public Test() { }

        public bool Equals(Test other)
        {
            return other.id == id || other.NameOfTest == NameOfTest;
        }

        public static bool ObjectIsTest(object obj)
        {
            return obj is Test;
        }

        public bool TestIsNotNull()
        {
            if (this.ID == 0 || this.NameOfTest == null)
            {
                return false;
            }

            return true;
        }
    }
}
