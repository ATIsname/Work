using System;
using System.Collections.Generic;

namespace TestLibrary.Test
{
    [Serializable]
    public class User
    {
        private long id;
        private string name;
        private string email;
        private List<UserTest> answeredTests;
        private string lastBotMessage;
        private UserTest currentTest;

        public long ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string LastBotMessage { get => lastBotMessage; set => lastBotMessage = value; }
        public UserTest CurrentTest { get => currentTest; set => currentTest = value; }
        public List<UserTest> AnsweredTests { get => answeredTests; set => answeredTests = value; }

        public User(long id, string name, string email, List<UserTest> tests, UserTest currentTest)
        {
            ID = id;
            Name = name;
            Email = email;
            CurrentTest = currentTest;
            AnsweredTests = tests;
        }

        public User(long id)
        {
            this.id = id;
            Name = name;
            Email = email;
            CurrentTest = null;
            AnsweredTests = new List<UserTest>();
        }

        public User() { }

        public void Update(User user)
        {
            ID = user.ID;
            Name = user.Name;
            Email = user.Email;
            CurrentTest = user.CurrentTest;
            LastBotMessage = user.LastBotMessage;
            AnsweredTests = user.AnsweredTests;
        }

        public bool Equals(User other)
        {
            return other.id == id;
        }
    }
}
