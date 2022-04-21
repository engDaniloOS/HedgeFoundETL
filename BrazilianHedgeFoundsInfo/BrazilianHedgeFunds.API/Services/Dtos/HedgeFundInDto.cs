using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BrazilianHedgeFunds.API.Services.Dtos
{
    public class HedgeFundInDto
    {
        [Required]
        [JsonPropertyName("cnpj")]
        public string CNPJ { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("page_index")]
        public int PageIndex { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
}
