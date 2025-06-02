using System.Windows;

public class Staff
{
    public string Name { get; set; }
    public string NationalCode { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime JoinDate { get; set; }
    public string Salary { get; set; }

    public Staff(string name, string password, string nationalCode, string role, string phoneNumber, DateTime joinDate)
    {
        Name = name;
        NationalCode = nationalCode;
        Password = password;
        Role = role;
        PhoneNumber = phoneNumber;
        JoinDate = joinDate;

        // Salary based on role
        switch (Role.ToLower())
        {
            case "worker":
                Salary = "5000000";
                break;
            default:
                Salary = "0";
                MessageBox.Show("سمت به درستی انتخاب نشده است");
                break;
        }
    }
}
