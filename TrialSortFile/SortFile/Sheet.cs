using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SortFile
{
    public class Sheet
    {
        public bool IsValidFile { get; set; }
        public string ErrorMessege { get; set; }
        public string PathSavedFile { get; set; }
        
        public virtual List<User> ReadFile(string path)
        {

            var users = new List<User>();
            if (File.Exists(path) && Path.GetExtension(path) == ".txt")
            {
                try
                {
                    using (var reader = File.OpenText(path))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                var fullName = line.Split(',');
                                if (fullName.Length == 2)
                                {
                                    var user = new User()
                                    {
                                        LastName = fullName[0].Trim(),
                                        FirstName = fullName[1].Trim()
                                    };
                                    users.Add(user);
                                    IsValidFile = true;
                                }
                                else
                                {
                                    IsValidFile = false;
                                    ErrorMessege = string.Format(Resources.ErrorMessege.WrongDelimitation);
                                    return null;
                                }
                            }
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    IsValidFile = false;
                    ErrorMessege = string.Format(Resources.ErrorMessege.SystemError, ex.Message);
                }
            }
            else
            {
                IsValidFile = false;
                ErrorMessege = string.Format(Resources.ErrorMessege.WrongFile);
            }
            return users;
        }

        public virtual List<User> SaveFile(string path)
        {
            var users = ReadFile(path).OrderBy(or => or.LastName).ToList();
            var fileName = Path.GetFileNameWithoutExtension(path);
            fileName = string.Format("{0}-sorted.txt", fileName);
            PathSavedFile = Path.GetFullPath(Path.Combine(path, @"..\",fileName));
            if (IsValidFile && Directory.Exists(Directory.GetParent(PathSavedFile).FullName))
            {
                if (File.Exists(PathSavedFile))
                {
                    fileName = string.Format("{0}-{1}.txt", fileName, DateTime.Now.ToString("YYmmddhhmmss"));
                    PathSavedFile = Path.GetFullPath(Path.Combine(path, @"..\", fileName));
                }
                using (TextWriter tw = new StreamWriter(PathSavedFile))
                {
                    foreach (User user in users)
                    {
                        tw.WriteLine("{0},{1}", user.LastName, user.FirstName);
                    }
                    tw.Close();
                }
            }
            return users;
        }
    }
}
