using System;
using System.ServiceModel;
using App;
using App.Validation;
using NUnit.Framework;
using Rhino.Mocks;

namespace AppTests
{
    [TestFixture]
    public class CustomerServiceTests : TestBase
    {
        private CustomerService service;
        private ICompanyRepository companyRepository;
        private ICustomerDataAccessProvider provider;
        private CustomerValidation customerValidation;

        private ServiceHost host;
        private MockRepository mockRepository;
        private PartialMockService serviceMock;

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();
            mockRepository = new MockRepository();
            serviceMock = mockRepository.PartialMock<PartialMockService>();
            
            try
            {
                host = new ServiceHost(serviceMock);
                host.AddServiceEndpoint(typeof(ICustomerCreditService), new BasicHttpBinding(), "http://localhost:8091/Service.svc");
                host.Open();
            }
            finally
            {
                if(host != null && host.State != CommunicationState.Faulted)
                {
                    ((IDisposable)host).Dispose();
                }
            }

            this.companyRepository = MockRepository.GenerateStrictMock<ICompanyRepository>();
            this.provider = MockRepository.GenerateMock<ICustomerDataAccessProvider>();
            this.service = new CustomerService(companyRepository, serviceMock, provider);
        }

        [Test]
        public void AddCustomerToGoldCompany()
        {
            this.companyRepository.Stub(cr => cr.GetById(1)).Return(base.GoldCompany);
            this.provider.Stub(p => p.AddCustomer(base.ValidCustomer));
            this.service = new CustomerService(companyRepository, serviceMock, provider);
            Assert.IsTrue(service.AddCustomer(ValidCustomer.FirstName, ValidCustomer.LastName, ValidCustomer.EmailAddress, ValidCustomer.DateOfBirth, 1));
            Assert.IsFalse(service.Customer.HasCreditLimit);
        }

        [Test]
        public void AddCustomerToSilverCompany()
        {
            serviceMock.Expect(x => x.GetCreditLimit("John", "Doe", ValidCustomer.DateOfBirth))
                            .IgnoreArguments()
                            .Return(300);
            mockRepository.ReplayAll();
            this.companyRepository.Stub(cr => cr.GetById(1)).Return(base.SilverCompany);
            this.provider.Stub(p => p.AddCustomer(base.ValidCustomer));
            Assert.IsTrue(service.AddCustomer(ValidCustomer.FirstName, ValidCustomer.LastName, ValidCustomer.EmailAddress, ValidCustomer.DateOfBirth, 1));
            Assert.IsTrue(this.service.Customer.HasCreditLimit);
            Assert.AreEqual(this.service.Customer.CreditLimit, 600);
        }

        [Test]
        public void AddCustomerToSilverCompanyLimitLessThen500()
        {
            serviceMock.Expect(x => x.GetCreditLimit("John", "Doe", ValidCustomer.DateOfBirth))
                            .IgnoreArguments()
                            .Return(100);
            mockRepository.ReplayAll();
            this.companyRepository.Stub(cr => cr.GetById(1)).Return(base.SilverCompany);
            Assert.IsFalse(service.AddCustomer(ValidCustomer.FirstName, ValidCustomer.LastName, ValidCustomer.EmailAddress, ValidCustomer.DateOfBirth, 1));
            Assert.IsTrue(this.service.Customer.HasCreditLimit);
            Assert.AreEqual(this.service.Customer.CreditLimit, 200);
        }

        [Test]
        public void AddCustomerToBronzeCompany()
        {
            serviceMock.Expect(x => x.GetCreditLimit("John", "Doe", ValidCustomer.DateOfBirth))
                            .IgnoreArguments()
                            .Return(600);
            mockRepository.ReplayAll();
            this.companyRepository.Stub(cr => cr.GetById(1)).Return(base.BrozneCompany);
            this.provider.Stub(p => p.AddCustomer(base.ValidCustomer));
            Assert.IsTrue(service.AddCustomer(ValidCustomer.FirstName, ValidCustomer.LastName, ValidCustomer.EmailAddress, ValidCustomer.DateOfBirth, 1));
            Assert.IsTrue(this.service.Customer.HasCreditLimit);
            Assert.AreEqual(this.service.Customer.CreditLimit, 600);
        }

        [Test]
        public void AddCustomerToBronzeCompanyLessThen500()
        {
            serviceMock.Expect(x => x.GetCreditLimit("John", "Doe", ValidCustomer.DateOfBirth))
                            .IgnoreArguments()
                            .Return(100);
            mockRepository.ReplayAll();
            this.companyRepository.Stub(cr => cr.GetById(1)).Return(base.BrozneCompany);
            Assert.IsFalse(service.AddCustomer(ValidCustomer.FirstName, ValidCustomer.LastName, ValidCustomer.EmailAddress, ValidCustomer.DateOfBirth, 1));
            Assert.IsTrue(this.service.Customer.HasCreditLimit);
            Assert.AreEqual(this.service.Customer.CreditLimit, 100);
        }

        [Test]
        public void ValidateCustomerNotInitialisedCustomer()
        {
            //TODO : if there were messages implemented then check for error message
            this.customerValidation = new CustomerValidation(null);
            Assert.IsFalse(this.customerValidation.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerValid()
        {
            this.customerValidation = new CustomerValidation(base.ValidCustomer);
            Assert.IsTrue(this.customerValidation.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerInValidAge()
        {
            this.customerValidation = new CustomerValidation(base.InvalidCustomerAge);
            //TODO: check for validation message InvalidAge
            Assert.IsFalse(this.customerValidation.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerMissingFirstName()
        {
            this.customerValidation = new CustomerValidation(base.InvalidCustomerFirstName);
            //TODO: check for validation message MissingFirstOrAndLastName
            Assert.IsFalse(this.customerValidation.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerMissingLastName()
        {
            this.customerValidation = new CustomerValidation(base.InvalidCustomerLastName);
            //TODO: check for validation message MissingFirstOrAndLastName
            Assert.IsFalse(this.customerValidation.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerMissingFirstAndLastName()
        {
            this.customerValidation = new CustomerValidation(base.InvalidCustomerFirstNameAndLastName);
            //TODO: check for validation message MissingFirstOrAndLastName
            Assert.IsFalse(this.customerValidation.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerInvalidEmail()
        {
            this.customerValidation = new CustomerValidation(base.InvalidCustomerEmail);
            //TODO: check for validation message InvalidEmail
            Assert.IsFalse(this.customerValidation.ValidateCustomer());
        }
    }
}