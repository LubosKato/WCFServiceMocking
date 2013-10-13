using System;
using App;
using App.Initialisation;
using NUnit.Framework;

namespace AppTests
{
    [TestFixture]
    public class TestBase
    {
        protected Customer ValidCustomer;
        protected Customer InvalidCustomerAge;
        protected Customer InvalidCustomerEmail;
        protected Customer InvalidCustomerFirstName;
        protected Customer InvalidCustomerLastName;
        protected Customer InvalidCustomerFirstNameAndLastName;

        protected Company GoldCompany;
        protected Company SilverCompany;
        protected Company BrozneCompany;

        [SetUp]
        public virtual void TestSetup()
        {
            ValidCustomer = InitialiseCustomer.InitCustomer("John", "Doe", "john.doe@email.com", new DateTime(1979, 08, 30));
            InvalidCustomerAge = InitialiseCustomer.InitCustomer("John", "Doe", "john.doe@email.com", new DateTime(1999, 08, 30));
            InvalidCustomerEmail = InitialiseCustomer.InitCustomer("John", "Doe", "email", new DateTime(1979, 08, 30));
            InvalidCustomerFirstName = InitialiseCustomer.InitCustomer(string.Empty, "Doe", "email.com", new DateTime(1979, 08, 30));
            InvalidCustomerLastName = InitialiseCustomer.InitCustomer("John", string.Empty, "email.com", new DateTime(1979, 08, 30));
            InvalidCustomerFirstNameAndLastName = InitialiseCustomer.InitCustomer(string.Empty, string.Empty, "email.com", new DateTime(1979, 08, 30));

            GoldCompany = new Company { Classification = Classification.Gold, Id  =1, Name = "GoldCompany" };
            SilverCompany = new Company { Classification = Classification.Silver, Id = 2, Name = "SilverCompany" };
            BrozneCompany = new Company { Classification = Classification.Bronze, Id = 3, Name = "BronzeCompany" };
        }
    }
}
