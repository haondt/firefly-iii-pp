namespace Firefly_iii_pp_Runner.API.Models.FireflyIII
{
    public class TransactionUpdateDto
    {
        public bool Apply_rules { get; set; }
        public bool Fire_webhooks { get; set; }
        public List<TransactionPartDto> Transactions { get; set; }
    }
}
