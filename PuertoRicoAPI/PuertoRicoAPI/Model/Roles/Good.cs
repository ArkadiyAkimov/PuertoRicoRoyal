using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Types;

namespace PuertoRicoAPI.Model.Roles
{
    public class Good
    {
        public Good(DataPlayerGood dataPlayerGood) 
        {
            this.Quantity = dataPlayerGood.Quantity;
            this.Type = dataPlayerGood.Type;    
        }
        public int Quantity { get; set; }
        public GoodType Type { get; set; }

        public int GetPrice()
        {
            return (int)Type;
        }
    }
}
