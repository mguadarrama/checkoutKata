namespace Novicap
{
    internal class Checkout
    {
        public decimal totalAmmount;
        private List<Article> articlesMaster;
        private List<PriceRules> priceRules;

        public int countOfArticlesWithDiscount; 
        public decimal totalDiscountAmmount;
        private List<Article> articlesInBasket;
        private List<Discount> discountsItems;
        private Article currentArticleReaded;
        private PriceRules currentDiscountToApply;


        // "The interface looks like this", means "must be like this" ?
        // articlesMaster could be in a database
        // question: could a checkout exists without what's receiving in constructor ?
        public Checkout(List<PriceRules> priceRules, List<Article> articlesMaster)
        {
            this.priceRules = priceRules ?? throw new ArgumentNullException(nameof(priceRules));
            this.articlesMaster = articlesMaster ?? throw new ArgumentNullException(nameof(articlesMaster));
            articlesInBasket = new List<Article>();
            discountsItems = new List<Discount>();
        }



        // I thought that could be a good idea to have an object that represent a visual invoice in a supermarket,
        // where you see all the articles on the top, and at the bottom, all the discounts
        public void Scan(string barcode)
        {
            currentArticleReaded =  articlesMaster.FirstOrDefault(a => a.Name == barcode);
            if (currentArticleReaded != null)
            {
                articlesInBasket.Add(currentArticleReaded);
                currentDiscountToApply = priceRules.FirstOrDefault(a => a.Article.Name == currentArticleReaded.Name);
                if (currentDiscountToApply != null)
                {
                    CalculateDiscounts();
                }
                totalDiscountAmmount = discountsItems.Sum(a => a.Ammount);
                totalAmmount = articlesInBasket.Sum(a => a.Price) - discountsItems.Sum(a => a.Ammount);
            }
            else
            {
                // TODO: should be an autogeneration of articles readed that not exists in articlesMaster ?
                throw new NotImplementedException(); 
            }
        }

        private void CalculateDiscounts()
        {
            countOfArticlesWithDiscount++;

            if (currentDiscountToApply.TypeDiscount ==  TypeDiscountEnum.Buy2x1 &  
                TotalArticlesOfThisKindInBasket(currentArticleReaded) % 2 == 0)
            {
                Buy2x1Strategy();
            }
            else if (currentDiscountToApply.TypeDiscount == TypeDiscountEnum.Buy3AndGetDiscount) 
            {
                Buy3Strategy();
            }
        }

   
        private void Buy2x1Strategy()
        {
            Discount discount = GetDiscount();
            discount.Ammount = (TotalArticlesOfThisKindInBasket(currentArticleReaded) / 2) * currentArticleReaded.Price;  // Shoud we move this to json file ?
        }

      


        private void Buy3Strategy()
        {
            if (TotalArticlesOfThisKindInBasket(currentArticleReaded) >= 3)   // Shoud we move this to json file ?
            {
                Discount discount = GetDiscount();
                discount.Ammount = TotalArticlesOfThisKindInBasket(currentArticleReaded) * 1; // Shoud we move this to json file ?
            }
        }






        private int TotalArticlesOfThisKindInBasket(Article article)
        {
            return articlesInBasket.Count(a => a.Name == article.Name);
        }













        private Discount GetDiscount()
        {
            var discount = discountsItems.FirstOrDefault(a => a.Article.Name == currentArticleReaded.Name);
            if (discount == null)
            {
                discount = new Discount();
                discountsItems.Add(discount);
            }
            discount.Article = currentArticleReaded;
            discount.Description = currentDiscountToApply.TypeDiscount.ToString();
            return discount;
        }


    }
}   