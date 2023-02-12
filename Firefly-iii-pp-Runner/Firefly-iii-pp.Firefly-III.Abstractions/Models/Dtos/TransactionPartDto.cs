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
            var hash = new HashCode();
            hash.Add(Type);
            hash.Add(Amount);
            hash.Add(Description);
            hash.Add(Source_id);
            hash.Add(Destination_name);
            hash.Add((Tags ?? new List<string>()).Aggregate(new HashCode(), (hc, s) =>
            {
                hc.Add(s);
                return hc;
            }).ToHashCode());
            hash.Add(Original_source);
            hash.Add(Import_hash_v2);
            hash.Add(Notes);
            hash.Add(Category_name);
            hash.Add(Bill_name);
            return hash.ToHashCode();
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
