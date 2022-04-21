using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BrazilianHedgeFunds.API.Services.Models
{
    public class HedgeFundRecord
    {
        [Key]
        [Column("ID")]
        [JsonIgnore()]
        public string Id { get; set; }

        [Column("TYPE")]
        [JsonIgnore()]
        public string Type { get; set; }

        [Column("CNPJ_FUNDO")]
        [JsonPropertyName("CNPJ_FUNDO")]
        public string CNPJ { get; set; }

        [Column("DT_COMPTC")]
        [JsonPropertyName("DT_COMPTC")]
        public DateTime RecordDate { get; set; }

        [Column("VL_TOTAL")]
        [JsonPropertyName("VL_TOTAL")]
        public double PortfolioTotalValue { get; set; }

        [Column("VL_QUOTA")]
        [JsonPropertyName("VL_TOTAL")]
        public double QuotaValue { get; set; }

        [Column("VL_PATRIM_LIQ")]
        [JsonPropertyName("VL_PATRIM_LIQ")]
        public double WorthValue { get; set; }

        [Column("CAPTC_DIA")]
        [JsonPropertyName("CAPTC_DIA")]
        public double DayInvestmentsTotalValue { get; set; }

        [Column("RESG_DIA")]
        [JsonPropertyName("RESG_DIA")]
        public double DayWithdrawalsTotalValue { get; set; }

        [Column("NR_COTST")]
        [JsonPropertyName("NR_COTST")]
        public int InvestorsTotal { get; set; }
    }
}
