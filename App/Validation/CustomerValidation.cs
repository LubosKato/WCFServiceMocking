using System;

namespace App.Validation
{
    public class CustomerValidation
    {
        private readonly Customer Customer;

        public CustomerValidation(Customer customer)
        {
            this.Customer = customer;
        }

        public bool ValidateCustomer()
        {
            if (Customer == null)
            {
                //TODO: add error message
                return false;
            }

            if (string.IsNullOrEmpty(Customer.FirstName) || string.IsNullOrEmpty(Customer.LastName))
            {
                //TODO: add custom validation  message MissingFirstOrAndLastName
                return false;
            }

            if (!Customer.EmailAddress.Contains("@") && !Customer.EmailAddress.Contains("."))
            {
                //TODO: add custom validation  message InvalidEmailAddress
                return false;
            }

            DateTime now = DateTime.Now;
            int age = now.Year - Customer.DateOfBirth.Year;
            if (now.Month < Customer.DateOfBirth.Month ||
                (now.Month == Customer.DateOfBirth.Month && now.Day < Customer.DateOfBirth.Day)) age--;

            if (age <= 21)
            {
                //TODO: add custom validation message InvalidAge
                return false;
            }

            return true;
        } 
    }
}