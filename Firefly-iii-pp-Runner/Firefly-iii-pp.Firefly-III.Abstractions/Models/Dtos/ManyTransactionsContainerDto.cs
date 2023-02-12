namespace FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos
{
    public class ManyTransactionsContainerDto
    {
        public List<TransactionDto> Data { get; set; }
        public TransactionListMetadata Meta { get; set; }
    }
}
