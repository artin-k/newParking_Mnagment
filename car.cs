public class Car
{
    public string Plate { get; set; }
    public string Specification { get; set; }
    public string PhoneNumber { get; set; }
    public string Type { get; set; }
    public string VehicleType { get; set; }
    public string ParkPlace { get; set; }
    public string EntryTime { get; set; }
    public string ExitTime { get; set; }
    public bool IsExited { get; set; }
    public string Date { get; set; }
    public int Fee { get; set; }

    public Car(string plate, string specification, string phone, string vehicleType, string parkPlace, string entryTime, string exitTime, bool isExited)
    {
        Plate = plate;
        Specification = specification;
        PhoneNumber = phone;
        VehicleType = vehicleType;
        ParkPlace = parkPlace;
        EntryTime = entryTime;
        ExitTime = exitTime;
        IsExited = isExited;
        Date = DateTime.Now.ToShortDateString();

        if (isExited == false) { ExitTime = ""; }
        
    }

}
