using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestLibrary.Test
{
    public static class FileUtils
    {
        public static string GetBotSolutionPath => new DirectoryInfo
            (AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Parent.FullName;

        public static string GetSiteSolutionPath => new DirectoryInfo
            (AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Parent.FullName;

        public static string GetSiteProjectPath => GetSiteSolutionPath + "\\TestBotSite";

        public static string GetTestDataPath => GetSiteProjectPath + "\\TestData";

        public static string GetTestsPath => GetSiteProjectPath
            + "\\SocialogicalTests\\tests.json";

        public static string GetUsersAnswersFolderPath => GetTestDataPath + "\\UsersAnswers";

        public static string GetUsersAnswersPath => GetTestDataPath + "\\users.json";

        public static async Task<List<Test>> GetTests()
        {

            string path = GetTestsPath;
            List<Test> tests = new List<Test>() { };
            await CreateFileIfNotExists(path, tests);
            WaitForFile(path);
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (var streamR = new StreamReader(fs))
                {
                    tests = JsonConvert.DeserializeObject<List<Test>>(await streamR.ReadToEndAsync());
                }
                if (tests == null)
                {
                    using (var stream = new StreamWriter(path, false, System.Text.Encoding.Default))
                    {
                        await stream.WriteAsync(JsonConvert.SerializeObject(new List<Test>()));
                    }
                }
            }
            return tests;
        }

        public static async Task SetTests(List<Test> tests)
        {
            string path = GetTestsPath;
            await CreateFileIfNotExists(path, tests);
            WaitForFile(path);
            using (var stream = new StreamWriter(path))
            {
                await stream.WriteAsync(JsonConvert.SerializeObject(tests));
            }
        }

        public static User GetTestObject(long id)
        {
            return GetTestObjects(GetUsersAnswersPath).Result.
                Where(o => o.ID == id).FirstOrDefault();
        }

        public static async Task<List<User>> GetTestObjects(string path)
        {
            List<User> testObjects = new List<User>();
            await CreateFileIfNotExists(path, testObjects);
            WaitForFile(path);
            using (var streamR = new StreamReader(path))
            {
                testObjects = JsonConvert.
                    DeserializeObject<List<User>>(await streamR.ReadToEndAsync());
            }
            return testObjects;
        }

        public static async Task UpdateTestObject(User user)
        {
            string path = GetUsersAnswersPath;
            List<User> testObjects = await GetTestObjects(path);
            int index = testObjects.IndexOf(testObjects.First(o => o.ID == user.ID));
            if (index != -1)
                testObjects[index] = user;
            await CreateFileIfNotExists(path, testObjects);
            WaitForFile(path);
            using (var stream = new StreamWriter(path))
            {
                await stream.WriteAsync(JsonConvert.SerializeObject(testObjects));
            }
        }

        public static async Task AddTestObject(long id)
        {
            string path = GetUsersAnswersPath;
            List<User> testObjects = await GetTestObjects(path);
            testObjects.Add(new User(id));
            await CreateFileIfNotExists(path, testObjects);
            WaitForFile(path);
            using (var stream = new StreamWriter(path))
            {
                await stream.WriteAsync(JsonConvert.SerializeObject(testObjects));
            }
        }

        public static async Task CreateFileIfNotExists<T>(string path, T value)
        {
            if (!File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (var streamW = new StreamWriter(fs))
                    {
                        await streamW.WriteAsync(JsonConvert.SerializeObject(value));
                    }
                }
            }
        }

        public static void WaitForFile(string filename)
        {
            while (!IsFileReady(filename)) { }
        }

        public static bool IsFileReady(string filename)
        {
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void CreateFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}