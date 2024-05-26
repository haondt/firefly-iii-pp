namespace FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos
{
    public class ManyTransactionsContainerDto
    {
        public required List<TransactionDto> Data { get; set; }
        public required TransactionListMetadata Meta { get; set; }
    }
}
