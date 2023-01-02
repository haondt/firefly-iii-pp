namespace Firefly_iii_pp_Runner.API.Models.FireflyIII
{
    public class ManyTransactionsContainerDto
    {
        public List<TransactionDto> Data { get; set; }
        public TransactionListMetadata Meta { get; set; }
    }
}
