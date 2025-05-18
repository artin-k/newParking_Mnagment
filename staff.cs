using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTest
{
    public class Staff
    {
        public string FullName { get; }
        public string NationalCode { get; }
        public string Password { get; }
        public string Role { get; }
        public string salary {  get; }
        public string Phone { get; }
        public DateTime JoinDate { get; }

        public Staff(string fullName, string nationalCode, string password, string role, string phone, DateTime joinDate)
        {
            FullName = fullName;
            NationalCode = nationalCode;
            Password = password;
            Role = role;
            switch (Role)
            {
                case "مدیر":
                    salary = "5000000";
                    break;
                case "حراست":
                    salary = "2500000";
                    break;
                case "کارمند":
                    salary = "3500000";
                    break;
                case "نظافتچی":
                    salary = "2000000";
                    break;
                default:
                    salary = "0";
                    MessageBox.Show("سمت به درستی انتخاب نشده است ");
                    break;
            }
            Phone = phone;
            JoinDate = joinDate;
        }

    }

}
