using Newtonsoft.Json;
using NUnit.Framework;

namespace Novicap
{
    [TestFixture]
    public class CheckoutTestClass
    {

        public List<Article> articlesMaster;
        public List<PriceRules> priceRules;


        [SetUp]
        public void SetUp()
        {
            getArticlesMasterData();
            getPriceRulesData();
        }



        /*
            Items: VOUCHER, TSHIRT, MUG
            Total: 32.50€
        */
        [Test]
        public void BasedOn3ArticlesReturnsSumPrice()
        {
            var checkout = new Checkout(priceRules, articlesMaster);
            checkout.Scan("VOUCHER");
            checkout.Scan("TSHIRT");
            checkout.Scan("MUG");
            Assert.AreEqual(checkout.totalAmmount, 32.5);
        }




        /*
         * The marketing department wants a 2-for-1 special on VOUCHER items.
            Items: VOUCHER, TSHIRT, VOUCHER
            Total: 25.00€
        */
        [Test]
        public void FirstScenario2x1Discount()
        {

            var checkout = new Checkout(priceRules, articlesMaster);
            checkout.Scan("VOUCHER");
            checkout.Scan("TSHIRT");
            checkout.Scan("VOUCHER");

            Assert.AreEqual(checkout.countOfArticlesWithDiscount, 3);
            Assert.AreEqual(checkout.totalDiscountAmmount, 5);
            Assert.AreEqual(checkout.totalAmmount, 25);

        }




        /*
         The CFO insists that the best way to increase sales is with (tiny) 
         discounts on bulk purchases. If you buy 3 or more TSHIRT items, 
         the price per unit should be 19.00€.
         Items: TSHIRT, TSHIRT, TSHIRT, VOUCHER, TSHIRT
         Total: 81.00€
        */
        [Test]
        public void SecondScenarioWithMoreThan3ArticlesTinyDoscount()
        {

            var checkout = new Checkout(priceRules, articlesMaster);
            checkout.Scan("VOUCHER");
            checkout.Scan("TSHIRT");
            checkout.Scan("TSHIRT");
            checkout.Scan("TSHIRT");
            checkout.Scan("TSHIRT");

            Assert.AreEqual(checkout.countOfArticlesWithDiscount, 5);
            Assert.AreEqual(checkout.totalDiscountAmmount, 4);
            Assert.AreEqual(checkout.totalAmmount, 81);

        }


        /*      
          Items: VOUCHER, TSHIRT, VOUCHER, VOUCHER, MUG, TSHIRT, TSHIRT
          Total: 74.50€
        */
        [Test]
        public void ThirdScenarioAllDiscountsTogether()
        {
            var checkout = new Checkout(priceRules, articlesMaster);
            checkout.Scan("VOUCHER");
            checkout.Scan("VOUCHER");
            checkout.Scan("TSHIRT");
            checkout.Scan("TSHIRT");
            checkout.Scan("MUG");
            checkout.Scan("VOUCHER");
            checkout.Scan("TSHIRT");

            Assert.AreEqual(checkout.countOfArticlesWithDiscount, 6);
            Assert.AreEqual(checkout.totalDiscountAmmount, 8);
            Assert.AreEqual(checkout.totalAmmount, 74.5);


        }





        // Moved this first to json if not exists, and read it if exists,
        // to avoid errors of not configured enviroment
        private void getArticlesMasterData()
        {
            string filename = @"articlesMaster.json";
            if (File.Exists(filename))
            {
                articlesMaster = (List<Article>)JsonConvert.DeserializeObject<List<Article>>(
                    File.ReadAllText(filename)
                    );
            }
            else
            {
                articlesMaster = new List<Article>()
                {
                    new Article(){ Name = "VOUCHER", Price = 5},
                    new Article(){ Name = "TSHIRT", Price = 20},
                    new Article(){ Name = "MUG", Price = 7.5m},
                };
                File.WriteAllText(filename, JsonConvert.SerializeObject(articlesMaster));
            }
        }

        



        private void getPriceRulesData()
        {

            string filename = @"priceRules.json";
            if (File.Exists(filename))
            {
                priceRules = (List<PriceRules>)JsonConvert.DeserializeObject<List<PriceRules>>(
                    File.ReadAllText(filename)
                    );
            }
            else
            {
                priceRules = new List<PriceRules>()
                {
                    new PriceRules(){ Article = articlesMaster.First(a=>a.Name=="VOUCHER") , TypeDiscount = TypeDiscountEnum.Buy2x1},
                    new PriceRules(){ Article = articlesMaster.First(a=>a.Name=="TSHIRT") , TypeDiscount = TypeDiscountEnum.Buy3AndGetDiscount},

                };

                File.WriteAllText(filename, JsonConvert.SerializeObject(priceRules));
            }





        }
    }









}
