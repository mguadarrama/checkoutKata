namespace Novicap
{
    public class PriceRules
    {
        public Article Article{ get; set; }
        public TypeDiscountEnum TypeDiscount { get; set; }

        public string Strategy { get; set; }
    }
}