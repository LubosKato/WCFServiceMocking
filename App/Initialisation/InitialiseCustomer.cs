using System;

namespace App.Initialisation
{
    public static class InitialiseCustomer
    {
        public static Customer InitCustomer(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            return new Customer
            {
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };
        }
    }
}