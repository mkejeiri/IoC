using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;

namespace SimpleIoC
{
    class Program
    {
        static void Main(string[] args)
        {
            Resolver resolver = new Resolver();
            //resolver.Register<IShopper, Shopper>();
            resolver.Register<ICreditCard, MasterCard>();
            resolver.Register<IShopper, Shopper>();
            //var shopper = resolver.Resolve<IShopper>();
            var shopper = resolver.Resolve<IShopper>();
            shopper.Charge();



            //for (int i = 0; i < 200; i++)
            //{
            //    var shoppers = new Shopper(resolver.ResolveCreditCard());
            //    shoppers.Charge();
            //}
            //ICreditCard creditCard = new MasterCard();
            //ICreditCard othCreditCard = new Visa();
            //var shopper = new Shopper(creditCard);
            //shopper.Charge();
            //shopper = new Shopper(othCreditCard);
            //shopper.Charge();


            Console.ReadKey();
        }
    }

    public class Resolver 
    {
        private IDictionary<Type,Type> dependencyMap = new Dictionary<Type, Type>();
        
        public T Resolve<T>()
        {
            return (T)Resolve(typeof (T));
        }

        private object Resolve(Type typeToResolve)
        {
            Type resolvedType = null;
            try
            {
                resolvedType = dependencyMap[typeToResolve];
            }
            catch
            {
               throw new Exception($"Couldn't resolve type {typeToResolve.FullName}");
            }

            //this isn't robust, we pick up the 1st ctr
            var firstConstructor = resolvedType.GetConstructors().FirstOrDefault();
            var constructorParams = firstConstructor.GetParameters();
            if (constructorParams.Count() == 0)
            {
                return Activator.CreateInstance(  resolvedType);
            }

            IList<object> parameters = new List<object>();
            foreach (var paramToResolve in constructorParams)
            {
                parameters.Add(Resolve(paramToResolve.ParameterType));
            }

            return firstConstructor.Invoke(parameters.ToArray());
        } 

        public ICreditCard ResolveCreditCard()
        {
            if (new Random().Next(2) == 1)
            {
                return new MasterCard();
            }
            return new Visa();
        }

        public void Register<TFrom, TTo>()
        {
            dependencyMap.Add(typeof(TFrom), typeof(TTo));
        }
    }

    public interface IShopper
    {
        void Charge();
    }

    public class Visa : ICreditCard
    {
        public string ChargeCard()
        {
            return "Chaging visa...";
        }
    }

    public class MasterCard : ICreditCard
    {
        public string ChargeCard()
        {
            return "Charging master card!...";
        }
    }

    public class Shopper : IShopper
    {
        private readonly ICreditCard _creditCard;

        public Shopper(ICreditCard creditCard)
        {
            _creditCard = creditCard;
        }

        public void Charge()
        {
            var chargeMessage = _creditCard.ChargeCard();
            Console.WriteLine(chargeMessage);
        }
    }

    public interface ICreditCard
    {
        string ChargeCard();
    }
}
