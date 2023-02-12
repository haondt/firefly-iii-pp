namespace FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos
{
    public class TransactionUpdateDto
    {
        public bool Apply_rules { get; set; }
        public bool Fire_webhooks { get; set; }
        public List<TransactionPartDto> Transactions { get; set; }
    }
}
