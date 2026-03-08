using System;
using System.Collections.Generic;
using System.Text;

namespace Stock.models
{
    internal class ItemManifest
    {

        public string Code { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }

        public double ExpectedQuantity { get; set; }
        public double CountedQuantity { get; set; }


        public string Status => $"{CountedQuantity} / {ExpectedQuantity}";
    }
}
