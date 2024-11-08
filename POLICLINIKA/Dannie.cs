using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POLICLINIKA
{
    internal class Dannie
    {
        public static string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;initial catalog=user29_db;user id=user29_db;password=user29;MultipleActiveResultSets=True;App=EntityFramework";
        public static int id_user = 0;
        public static int role = 0;
        public static string Login = "";
        public static string Imya = "";
        public static string Otchestvo = "";
        public static string Familiya = "";
    }
}
