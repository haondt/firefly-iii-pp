using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.FireflyIII.Abstractions.Models.Dtos
{
    public class CreateTransactionDto
    {
        [JsonProperty("error_if_duplicate_hash")]
        public bool ErrorIfDuplicateHash { get; set; } = true;
        [JsonProperty("apply_rules")]
        public bool ApplyRules { get; set; } = false;
        [JsonProperty("fire_webhooks")]
        public bool FireWebhooks { get; set; } = true;
        [JsonProperty("group_title")]
        public string? GroupTitle { get; set; }
        public List<TransactionPartDto> Transactions { get; set; } = [];
    }
}
