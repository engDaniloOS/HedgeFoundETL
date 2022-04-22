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
        [JsonPropertyName("cnpj_fundo")]
        public string CNPJ { get; set; }

        [Column("DT_COMPTC")]
        [JsonPropertyName("dt_comptc")]
        public DateTime RecordDate { get; set; }

        [Column("VL_TOTAL")]
        [JsonPropertyName("vl_total")]
        public double PortfolioTotalValue { get; set; }

        [Column("VL_QUOTA")]
        [JsonPropertyName("vl_quota")]
        public double QuotaValue { get; set; }

        [Column("VL_PATRIM_LIQ")]
        [JsonPropertyName("vl_patrim_liq")]
        public double WorthValue { get; set; }

        [Column("CAPTC_DIA")]
        [JsonPropertyName("captc_dia")]
        public double DayInvestmentsTotalValue { get; set; }

        [Column("RESG_DIA")]
        [JsonPropertyName("resg_dia")]
        public double DayWithdrawalsTotalValue { get; set; }

        [Column("NR_COTST")]
        [JsonPropertyName("nr_cotst")]
        public int InvestorsTotal { get; set; }
    }
}
