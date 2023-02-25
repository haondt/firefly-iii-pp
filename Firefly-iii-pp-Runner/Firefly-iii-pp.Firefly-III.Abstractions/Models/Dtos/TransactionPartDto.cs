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
        public string Destination_id { get; set; }
        public string Destination_name { get; set; }
        public List<string> Tags { get; set; }
        public string Original_source { get; set; } 
        public string Import_hash_v2 { get; set; }
        public string Notes { get; set; }
        public string Category_name { get; set; }
        public string Bill_name { get; set; }
        public string Budget_name { get; set; }

        private HashCode GetHashCodeWithoutAccounts()
        {
            var hash = new HashCode();
            hash.Add(Type);
            hash.Add(Amount);
            hash.Add(Description);
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
            hash.Add(Budget_name);
            return hash;
        }

        public override int GetHashCode()
        {
            var hash = GetHashCodeWithoutAccounts();
            hash.Add(Source_id);
            hash.Add(Source_name);
            hash.Add(Destination_id);
            hash.Add(Destination_name);
            return hash.ToHashCode();
        }

        public bool Equals(TransactionPartDto? other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        public bool IsEquivalentTo(TransactionPartDto? other)
        {
            if (other == null)
                return false;

            if (GetHashCodeWithoutAccounts().ToHashCode() != other.GetHashCodeWithoutAccounts().ToHashCode())
                return false;

            return CompareIds(Source_id, Source_name, other.Source_id, other.Source_name)
                && CompareIds(Destination_id, Destination_name, other.Destination_id, other.Destination_name);
        }

        private bool CompareIds(string id1, string name1, string id2, string name2)
        {
            var a = !string.IsNullOrWhiteSpace(id1);
            var b = !string.IsNullOrWhiteSpace(name1);
            var c = !string.IsNullOrWhiteSpace(id2);
            var d = !string.IsNullOrWhiteSpace(name2);

            if (a)
            {
                if (b)
                {
                    if (c)
                    {
                        if (d)
                            return id1 == id2 && name1 == name2;
                        return id1 == id2;
                    }
                    else // !c
                    {
                        if (d)
                            return name1 == name2;
                        return false;
                    }
                }
                else // !b
                {
                    if (c)
                        return id1 == id2;
                    else // not c
                        return false;
                }
            }
            else // !a
            {
                if (b)
                {
                    if (d)
                        return name1 == name2;
                    return false;
                }
                else // !b
                {
                    if (c)
                        return false;
                    return !d;
                }
            }
        }

        public void JoinIds(TransactionPartDto target)
        {
            var (sid, sn) = JoinIds(Source_id, Source_name, target.Source_id, target.Source_name);
            var (did, dn) = JoinIds(Destination_id, Destination_name, target.Destination_id, target.Destination_name);
            target.Source_id = sid;
            target.Source_name = sn;
            target.Destination_id = did;
            target.Destination_name = dn;
        }

        private (string Id, string Name) JoinIds(string id1, string name1, string id2, string name2)
        {
            var a = !string.IsNullOrWhiteSpace(id1);
            var b = !string.IsNullOrWhiteSpace(name1);
            var c = !string.IsNullOrWhiteSpace(id2);
            var d = !string.IsNullOrWhiteSpace(name2);

            var i = id1 == id2;
            var n = name1 == name2;

            var error = $"Cannot parse transacion id from {id1}:{name1} to {id2}:{name2}";

            if (a)
            {
                if (b)
                {
                    if (c)
                    {
                        if (d)
                        {
                            if (i)
                            {
                                if (!n)
                                    throw new ArgumentException(error);
                            }
                            else
                            {
                                if (n)
                                    throw new ArgumentException(error);
                            }
                        }
                    }
                    else // !c
                    {
                        if (!d)
                            throw new ArgumentException(error);
                    }
                }
                else // !b
                {
                    if (c)
                        if (d)
                            if (i)
                                throw new ArgumentException(error);
                }
            }
            else // !a
            {
                if (b)
                {
                    if (c)
                    {
                        if (d)
                            if (n)
                                throw new ArgumentException(error);
                    }
                    else // !c
                    { 
                        if (!d) 
                            throw new ArgumentException(error); 
                    }
                }
            }

            return (id2, name2);
        }

        public override bool Equals(object? obj)
        {
            if (obj is TransactionPartDto dto)
                return Equals(dto);
            return false;
        }
    }
}
