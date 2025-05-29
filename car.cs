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

        // Constructor for entry
        public Car(string plate, string specification, string phoneNumber, string parkPlace, string entryTime, string date)
        {
            Plate = plate;
            Specification = specification;
            PhoneNumber = phoneNumber;
            ParkPlace = parkPlace;
            EntryTime = entryTime;
            Date = date;
        }

        // Constructor for exit
        public Car(string exitTime)
        {
            ExitTime = exitTime;
        }
    }
}
