using Common.Domain;

namespace Domain.Models
{
    public class Product : Aggregate
    {
        protected Product() { }

        public Product(
            string name,
            string description,
            double buyPrice,
            double sellPrice,
            Guid supplierID,
            EUnitSale units,
            Guid id = default
            )
        {
            Id = id;
            Name = name;
            Description = description;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
            SupplierID = supplierID;
            Quantity = 0;
            Units = units;

        }

        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; }
        public virtual double BuyPrice { get; protected set; }
        public virtual double SellPrice { get; protected set; }
        public virtual Guid SupplierID { get; protected set; }
        public virtual double Quantity { get; protected set; }
        public virtual EUnitSale Units { get; protected set; }
    }
}
