using FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos;

namespace FireflyIIIpp.FireflyIII.Abstractions.Models
{
    public class TransactionAttributes
    {
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public string Group_title { get; set; }
        public List<TransactionPartDto> Transactions { get; set; }
    }
}
