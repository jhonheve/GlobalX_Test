using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SortFile;
using System.Reflection;
using System.IO;

namespace TestSortFile
{
    [TestClass]
    public class SheetTest
    {
        private Sheet sheet;
        private string path = Environment.CurrentDirectory + "\\users.txt";
        private string pathFileWithoutDelimitation = Environment.CurrentDirectory + "\\usersWithoutFormat.txt";
        private static Random random = new Random();
        private List<User> TestUsers { get; set; }


        #region PrivateMethod
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private List<User> UserToTest(int rows)
        {
            var path = string.Empty;
            var users = new List<User>();
            for (int i = 0; i < rows; i++)
            {
                var user = new User()
                {
                    FirstName = RandomString(10),
                    LastName = RandomString(10)
                };
                users.Add(user);
            }
            return users;
        }

        private void Mock_UserFile(int rows)
        {
            TestUsers = UserToTest(rows);
            using (TextWriter tw = new StreamWriter(path))
            {
                foreach (User user in TestUsers)
                {
                    tw.WriteLine("{0},{1}", user.LastName, user.FirstName);
                }
                tw.Close();
            }
        }

        private void Mock_UserFileWithoutDelimitation(int rows)
        {
            var users = UserToTest(rows);
            using (TextWriter tw = new StreamWriter(pathFileWithoutDelimitation))
            {
                foreach (User user in users)
                {
                    tw.WriteLine("{0}-{1}", user.LastName, user.FirstName);
                }
                tw.Close();
            }
        }
        #endregion
        
        [TestInitialize]
        public void TestInitialize()
        {
            Mock_UserFile(1000);
            sheet = new Sheet();
        }

        [TestCleanup]
        public void CleanResource()
        {
            File.Delete(path);
            if (File.Exists(sheet.PathSavedFile))
            {
                File.Delete(sheet.PathSavedFile);
            }
            if (File.Exists(pathFileWithoutDelimitation))
            {
                File.Delete(pathFileWithoutDelimitation);
            }
        }

        [TestMethod]
        public void ReadUserFile_Test()
        {
            var result = sheet.ReadFile(path);
            Assert.IsTrue(sheet.IsValidFile);
            Assert.AreEqual(TestUsers.Count, result.Count);
        }

        [TestMethod]
        public void SaveUserFile_Test()
        {
            var result = sheet.SaveFile(path);
            var orderedByAsc = result.OrderBy(usr => usr.LastName).ToList();
            var isSort = result.SequenceEqual(orderedByAsc);
            Assert.IsTrue(isSort);
        }

        [TestMethod]
        public void ReadWithoutFile_Test()
        {
            string dummyPath = Environment.CurrentDirectory + "\\DummyUserFile.txt";
            var result = sheet.ReadFile(dummyPath);
            Assert.IsFalse(sheet.IsValidFile);
            Assert.AreEqual(sheet.ErrorMessege, "The file does not exist or it does not have a correct format.");
        }

        [TestMethod]
        public void ReadUserFileWithoutDelimitation_Test()
        {
            Mock_UserFileWithoutDelimitation(100);
            var result = sheet.ReadFile(pathFileWithoutDelimitation);
            Assert.IsFalse(sheet.IsValidFile);
            Assert.AreEqual(sheet.ErrorMessege, "The file is not properly delimited");
        }
    }
}
