using System;

namespace WpfTest
{
    public class Car
    {
        public string Plate { get; }
        public string Specification { get; }
        public string PhoneNumber { get; }
        public string ParkPlace { get; }
        public string Date { get; }
        public string EntryTime { get; }
        public string ExitTime { get; }
        public bool IsExited { get; }

        // Constructor for entry
        public Car(string plate, string specification, string phoneNumber, string parkPlace, string entryTime, string date, bool isExited)
        {
            Plate = plate;
            Specification = specification;
            PhoneNumber = phoneNumber;
            ParkPlace = parkPlace;
            EntryTime = entryTime;
            Date = date;
            IsExited = isExited;

            if (isExited == false) { ExitTime = ""; }
        }

        public double PaymentAmount
        {
            get
            {
                if (!IsExited) return 0;

                if (DateTime.TryParse(EntryTime, out DateTime entry) &&
                    DateTime.TryParse(ExitTime, out DateTime exit))
                {
                    var duration = exit - entry;
                    return Math.Ceiling(duration.TotalHours) * 2.0; // 2 currency units per hour
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
