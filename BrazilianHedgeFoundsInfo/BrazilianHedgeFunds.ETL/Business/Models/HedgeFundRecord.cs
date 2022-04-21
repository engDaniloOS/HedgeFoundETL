using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrazilianHedgeFunds.ETL.Business.Models
{
    public class HedgeFundRecord
    {
        [Key]
        [Column("ID")]
        public string Id { get; set; }

        [Column("TYPE")]
        public string Type { get; set; }
        
        [Column("CNPJ_FUNDO")]
        public string CNPJ { get; set; }
        
        [Column("DT_COMPTC")]
        public DateTime RecordDate { get; set; }
        
        [Column("VL_TOTAL")]
        public double PortfolioTotalValue { get; set; }
        
        [Column("VL_QUOTA")]
        public double QuotaValue { get; set; }
        
        [Column("VL_PATRIM_LIQ")]
        public double WorthValue { get; set; }
        
        [Column("CAPTC_DIA")]
        public double DayInvestmentsTotalValue { get; set; }
        
        [Column("RESG_DIA")]
        public double DayWithdrawalsTotalValue { get; set; }
        
        [Column("NR_COTST")]
        public int InvestorsTotal { get; set; }

    }
}
