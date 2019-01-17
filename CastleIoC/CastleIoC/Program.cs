using System;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel;

namespace CastleIoC
{
    class Program
    {
        static void Main(string[] args)
        {
            //By default WC create singleton
            var container = new WindsorContainer();
            //container.Register(Component.For<ShopperCtrInject>());

            //use forward chaining when you card same IMPL for diff interface in case you want a signleton
            //container.Register(Component.For<ICreditCard>().ImplementedBy<MasterCard>()
            //    .Forward<ICreditCard>().ImplementedBy<Visa>());

            //Not good practice, if you already created an instance you attached to interface
            //var visa = new Visa();
            //container.Register(Component.For<ICreditCard>().Instance(visa));

            //Unity allows specificily how to override the contructor injection in the resolve method VS Windsor Castle (WC)in the Register Methods
            //WC automatically going to inject on properties, i.e setter injection if you expose a property with an interface and the container
            //knows about that interface
            //container.Register(Component.For<ICreditCard>().ImplementedBy<MasterCard>().Named("defaultCard").LifeStyle.Transient);
            //container.Register(Component.For<ICreditCard>().ImplementedBy<Visa>());
            //container.Register(Component.For<IShopper>().ImplementedBy<ShopperPropInject>());
            //container.Register(Component.For<IShopper>().ImplementedBy<ShopperCtrInject>().LifestyleTransient());


            //this will resolve all credit card, by we get the first one, i.e  default mastercard unless we specified by name
            //var creditCard = container.ResolveAll<ICreditCard>();

            //e.g logging: we could log whenever a component is created or whenever something is used you could just add it,
            //facility will allow us to insert certain hooks into the lifecycle of the creation of objects and the usage of objects and the container.
            //container.Register(Component.For<IShopper>().ImplementedBy<ShopperCtrInject>()).AddFacility(YourLogic);


            //Install provides a way to put all registration into one place.
            //Install modulizes application so we could register multiple registration in each installer and control what gets installed. 
            //now components registration is delegated to the installer. installer accept multiple registration installer...
            //container.Install(new ShoppingInstaller1(),new ShoppingInstaller2(),...);

            container.Install(new ShoppingInstaller());

            //child containers (not a very widely used feature - NOT recommended) : concept where you could have nested
            //containers and you could use a child container to control scope or just logical separation in the app,
            //and child container will try first to resolve its own registered dependencies and if it can't then it'll check the parent.
            //container.AddChildContainer(childContainer);


            var shopper = container.Resolve<IShopper>();
            shopper.Charge();
            Console.WriteLine(shopper.ChargesForCurrentCard);

            //WC will optimize for effeciency and by default uses a signleton unless we specify the lifecycle.
            var shopper2 = container.Resolve<IShopper>();
            shopper2.Charge();
            Console.WriteLine(shopper2.ChargesForCurrentCard);
            Console.ReadKey();
        }
    }

    public class ShoppingInstaller : IWindsorInstaller
    {
        public ShoppingInstaller(){}

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ICreditCard>().ImplementedBy<MasterCard>().Named("defaultCard").LifestyleTransient());
            container.Register(Component.For<IShopper>().ImplementedBy<ShopperCtrInject>().LifestyleTransient());
        }
    }

    public interface IPaymentCard
    {
        string ChargeCard();
    }


    public interface IShopper
    {
        void Charge();
        int ChargesForCurrentCard { get; }
    }

    public class IDEAL : IPaymentCard
    {
        public string ChargeCard()
        {
            return "Chaging debit...";
        }

        int ChargeCount { get; }
    }

    public class Visa : ICreditCard, IPaymentCard
    {
        public string ChargeCard()
        {
            return "Chaging visa...";
        }

        public int ChargeCount { get; set; } = 100;
    }

    public class MasterCard : ICreditCard
    {
        public int ChargeCount { get; set; }
        public string ChargeCard()
        {
            ChargeCount++;
            return "Charging master card!...";
        }
    }

    public class ShopperCtrInject : IShopper
    {
        private readonly ICreditCard _creditCard;
        public ShopperCtrInject(ICreditCard creditCard)
        {
            _creditCard = creditCard;
        }

        public int ChargesForCurrentCard
        {
            get { return _creditCard.ChargeCount; }
        }

        public void Charge()
        {
            var chargeMessage = _creditCard.ChargeCard();
            Console.WriteLine($"ShopperCtrInject-charge:  {chargeMessage}");
        }
    }


    public class ShopperPropInject : IShopper
    {
        public ICreditCard CreditCard {  get; set; }

        public int ChargesForCurrentCard
        {
            get { return CreditCard.ChargeCount; }
        }

        public void Charge()
        {
            var chargeMessage = CreditCard.ChargeCard();
            Console.WriteLine($"ShopperPropInject-charge:  {chargeMessage}");
        }
    }


    public interface ICreditCard
    {
        string ChargeCard();
        int ChargeCount { get; set; }
    }



    /******************************************************
      For readability purpose
    ******************************************************/
    public class READMEClass
    {
        private void README()
        {
            //By default WC create singleton
            var container = new WindsorContainer();
            container.Register(Component.For<ShopperCtrInject>());

            //use forward chaining when you card same IMPL for diff interface in case you want a signleton
            container.Register(Component.For<ICreditCard>().ImplementedBy<MasterCard>()
                .Forward<ICreditCard>().ImplementedBy<Visa>());

            //Not good practice, if you already created an instance you attached to interface
            var visa = new Visa();
            container.Register(Component.For<ICreditCard>().Instance(visa));

            //Unity allows specificily how to override the contructor injection in the resolve method VS Windsor Castle (WC)in the Register Methods
            //WC automatically going to inject on properties, i.e setter injection if you expose a property with an interface and the container
            //knows about that interface
            container.Register(Component.For<ICreditCard>().ImplementedBy<MasterCard>().Named("defaultCard").LifeStyle.Transient);
            container.Register(Component.For<ICreditCard>().ImplementedBy<Visa>().Named("backupCard").LifestyleScoped());
            container.Register(Component.For<ICreditCard>().ImplementedBy<Visa>().Named("backupCard2").LifestylePerWebRequest());
            container.Register(Component.For<IShopper>().ImplementedBy<ShopperPropInject>());
            container.Register(Component.For<IShopper>().ImplementedBy<ShopperCtrInject>().LifestyleTransient());


            //this will resolve all credit card, by we get the first one, i.e  default mastercard unless we specified by name
            var creditCard = container.ResolveAll<ICreditCard>();

            //e.g logging: we could log whenever a component is created or whenever something is used you could just add it,
            //facility will allow us to insert certain hooks into the lifecycle of the creation of objects and the usage of objects and the container.
            //container.Register(Component.For<IShopper>().ImplementedBy<ShopperCtrInject>()).AddFacility(YourLogic());


            //Install provides a way to put all registration into one place.
            //Install modulizes application so we could register multiple registration in each installer and control what gets installed. 
            //now components registration is delegated to the installer. installer accept multiple registration installer...
            container.Install(new ShoppingInstaller(), new ShoppingInstaller2(), new ShoppingInstaller3());

            container.Install(new ShoppingInstaller());

            //child containers (not a very widely used feature - NOT recommended) : concept where you could have nested
            //containers and you could use a child container to control scope or just logical separation in the app,
            //and child container will try first to resolve its own registered dependencies and if it can't then it'll check the parent.
            var childContainer =  new WindsorContainer();
            container.AddChildContainer(childContainer);


            var shopper = container.Resolve<IShopper>();
            shopper.Charge();
            Console.WriteLine(shopper.ChargesForCurrentCard);

            //WC will optimize for effeciency and by default uses a signleton unless we specify the lifecycle.
            var shopper2 = container.Resolve<IShopper>();
            shopper2.Charge();
            Console.WriteLine(shopper2.ChargesForCurrentCard);
            Console.ReadKey();
        }

    }

    public class ShoppingInstaller3 : ShoppingInstaller { }
    public class ShoppingInstaller2 : ShoppingInstaller { }
}
