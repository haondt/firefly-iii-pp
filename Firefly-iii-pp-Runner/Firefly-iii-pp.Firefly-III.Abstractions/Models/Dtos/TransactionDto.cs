namespace FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos
{
    public class TransactionDto
    {
        public required string Id { get; set; }
        public required TransactionAttributes Attributes { get; set; }
    }
}
