namespace Firefly_iii_pp_Runner.API.Models.FireflyIII
{
    public class TransactionAttributes
    {
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public string Group_title { get; set; }
        public List<TransactionPartDto> Transactions { get; set; }
    }
}
