namespace telephonedirectory.Models
{
    public class TelephoneEntry
    {
        public int id { get;set; }
        public required string name { get; set; }
        public string? address { get; set; }
        public required string telephone { get; set; }
        public string? country { get; set; } 
        public string? province { get; set; }
        public string? district { get; set; }

    }
}
