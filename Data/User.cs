using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Файл: User.cs
namespace FTSControl.Data
{
    public static class User
    {
        public static int Id { get; set; }
        public static string Login { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string Patronymic { get; set; }
        public static int RoleID { get; set; }
        public static int StatusID { get; set; }

        public static string FullName => $"{FirstName} {Patronymic} {LastName}".Trim();

        public static void Clear()
        {
            Id = 0;
            Login = "";
            FirstName = "";
            LastName = "";
            Patronymic = "";
            RoleID = 0;
            StatusID = 0;
        }
    }
}