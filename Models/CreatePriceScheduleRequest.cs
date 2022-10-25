namespace Alyas.Commerce.OrderCloud.DataMigration.Models
{
    using System;
    using System.Collections.Generic;

    public class CreatePriceScheduleRequest : BaseCatalogRequest
    {
        public string Currency { get; set; }
        public DateTime? SaleStart { get; set; }
        public DateTime? SaleEnd { get; set; }
        public List<PriceBreak> PriceBreaks { get; set; } = new List<PriceBreak>();
    }
}
