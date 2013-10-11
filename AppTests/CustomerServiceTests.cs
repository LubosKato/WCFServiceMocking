using System;
using System.ServiceModel;
using App;
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
            this.provider = MockRepository.GenerateStrictMock<ICustomerDataAccessProvider>();
            this.service = new CustomerService(companyRepository, serviceMock, provider);
        }

        [Test]
        public void AddCustomerToGoldCompany()
        {
            this.companyRepository.Stub(cr => cr.GetById(1)).Return(base.GoldCompany);
            this.provider.Stub(p => p.AddCustomer(base.ValidCustomer));
            this.service = new CustomerService(companyRepository, serviceMock, provider);
            service.InitCustomer(base.ValidCustomer);
            Assert.IsTrue(service.AddCustomer(1));
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
            service.InitCustomer(base.ValidCustomer);
            Assert.IsTrue(this.service.AddCustomer(1));
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
            this.service.InitCustomer(base.ValidCustomer);
            Assert.IsFalse(this.service.AddCustomer(1));
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
            this.service.InitCustomer(base.ValidCustomer);
            Assert.IsTrue(this.service.AddCustomer(1));
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
            this.service.InitCustomer(base.ValidCustomer);
            Assert.IsFalse(this.service.AddCustomer(1));
            Assert.IsTrue(this.service.Customer.HasCreditLimit);
            Assert.AreEqual(this.service.Customer.CreditLimit, 100);
        }

        [Test]
        public void ValidateCustomerNotInitialisedCustomer()
        {
            //TODO : if there were messages implemented then check for error message
            Assert.IsFalse(this.service.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerValid()
        {
            this.service.InitCustomer(base.ValidCustomer);
            Assert.IsTrue(this.service.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerInValidAge()
        {
            this.service.InitCustomer(base.InvalidCustomerAge);
            //TODO: check for validation message InvalidAge
            Assert.IsFalse(this.service.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerMissingFirstName()
        {
            this.service.InitCustomer(base.InvalidCustomerFirstName);
            //TODO: check for validation message MissingFirstOrAndLastName
            Assert.IsFalse(this.service.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerMissingLastName()
        {
            this.service.InitCustomer(base.InvalidCustomerLastName);
            //TODO: check for validation message MissingFirstOrAndLastName
            Assert.IsFalse(this.service.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerMissingFirstAndLastName()
        {
            this.service.InitCustomer(base.InvalidCustomerFirstNameAndLastName);
            //TODO: check for validation message MissingFirstOrAndLastName
            Assert.IsFalse(this.service.ValidateCustomer());
        }

        [Test]
        public void ValidateCustomerInvalidEmail()
        {
            this.service.InitCustomer(base.InvalidCustomerEmail);
            //TODO: check for validation message InvalidEmail
            Assert.IsFalse(this.service.ValidateCustomer());
        }
    }
}