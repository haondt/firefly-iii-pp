namespace FireflyIIIpp.FireflyIII.Abstractions.Models
{
    public class TransactionListMetadata
    {
        public PaginationData Pagination { get; set; }
    }

    public class PaginationData
    {
        /// <summary>
        /// Total number of transactions
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Number of transactions in this page
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Maxmimum number of transactions per page
        /// </summary>
        public int Per_page { get; set; }
        public int Current_page { get; set; }
        public int Total_pages { get; set; }
    }
}
