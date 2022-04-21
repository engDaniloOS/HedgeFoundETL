using System.Collections.Generic;

namespace BrazilianHedgeFunds.ETL.Business.Models
{
    public static class ProcessError
    {
        private static List<string> _erros;

        public static List<string> Errors {
            get {
                if (_erros == null)
                    return new List<string>();
                else 
                    return _erros;
            }
            set { _erros = value; } 
        }
    }
}
