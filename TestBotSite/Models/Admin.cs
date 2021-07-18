using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestBotSite.Models
{
    public class Admin
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public Admin(int iD, string email, string password, string name)
        {
            ID = iD;
            Email = email;
            Password = password;
            Name = name;
        }

        public Admin()
        {

        }
        static public async Task<Admin> GetAdmin(string email, string password)
        {
            string path = @"X:\Работа\TestBotSite\admins.json";
            List<Admin> admins = await GetAdmins(path);
            return admins.FirstOrDefault(o=>o.Email == email && o.Password == password);
        }

        static public async Task<List<Admin>> GetAdmins(string path)
        {
            List<Admin> admins = new List<Admin>();
            //admins.Add(new Admin(1, "dan.lis.2016@mail.ru", "123", "Danik"));
            //using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            //{
            //    await JsonSerializer.SerializeAsync<List<Admin>>(fs, admins);
            //}
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                admins = await JsonSerializer.DeserializeAsync<List<Admin>>(fs);
            }
            return admins;
        }
    }
}
