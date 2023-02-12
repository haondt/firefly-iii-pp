namespace FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos
{
    public class TransactionPartDto : IEquatable<TransactionPartDto>
    {
        public string Type { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Source_id { get; set; }
        public string Source_name { get; set; }
        public string Destination_name { get; set; }
        public List<string> Tags { get; set; }
        public string Original_source { get; set; } 
        public string Import_hash_v2 { get; set; }
        public string Notes { get; set; }
        public string Category_name { get; set; }
        public string Bill_name { get; set; }

        public override int GetHashCode()
        {
            return (Type, Date, Amount, Description, Source_id, Destination_name,
                Tags.Aggregate(19, (h, t) => h * 31 +t.GetHashCode()),
                Original_source, Import_hash_v2, Notes, Category_name, Bill_name).GetHashCode();
        }

        public bool Equals(TransactionPartDto? other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is TransactionPartDto dto)
                return Equals(dto);
            return false;
        }
    }
}
