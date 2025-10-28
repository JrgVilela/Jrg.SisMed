using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    public class Address : EntityBase
    {
        private readonly int MaxStreetLength = 200;
        private readonly int MaxNumberLength = 20;
        private readonly int MaxComplementLength = 100;
        private readonly int MaxZipCodeLength = 10;
        private readonly int MaxCityLength = 100;
        private readonly int MaxStateLength = 50;

        public string Street { get; private set; } = string.Empty;
        public string Number { get; private set; } = string.Empty;
        public string? Complement { get; private set; }
        public string ZipCode { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;

        #region Constructors
        internal Address() { }
        public Address(string street, string number, string? complement, string zipCode, string city, string state)
        {
            Validate();

            Update(street, number, complement, zipCode, city, state);
        }
        #endregion

        #region Public Methods
        public void Update(string street, string number, string? complement, string zipCode, string city, string state)
        {
            Validate();
            Street = street;
            Number = number;
            Complement = complement;
            ZipCode = zipCode.FormatCep();
            City = city;
            State = state;
        }

        #endregion

        #region Private Methods
        public void Validate()
        {
            Normalize();

            var v = new ValidationCollector();

            v.When(Street.IsNullOrWhiteSpace(), "Street is required.");
            v.When(Street.Length > MaxStreetLength, $"Street must be at most {MaxStreetLength} characters long.");
            v.When(Number.IsNullOrWhiteSpace(), "Number is required.");
            v.When(Number.Length > MaxNumberLength, $"Number must be at most {MaxNumberLength} characters long.");
            v.When(Complement != null && Complement.Length > MaxComplementLength, $"Complement must be at most {MaxComplementLength} characters long.");
            v.When(ZipCode.IsNullOrWhiteSpace(), "ZipCode is required.");
            v.When(ZipCode.Length > MaxZipCodeLength, $"ZipCode must be at most {MaxZipCodeLength} characters long.");
            v.When(City.IsNullOrWhiteSpace(), "City is required.");
            v.When(City.Length > MaxCityLength, $"City must be at most {MaxCityLength} characters long.");
            v.When(State.IsNullOrWhiteSpace(), "State is required.");
            v.When(State.Length > MaxStateLength, $"State must be at most {MaxStateLength} characters long.");
            v.ThrowIfAny();

        }

        public void Normalize()
        {
            Street = Street.Trim();
            Number = Number.Trim();
            if (Complement != null)
                Complement = Complement.Trim();
            ZipCode = ZipCode.Trim();
            City = City.Trim();
            State = State.Trim();
        }
        #endregion
    }
}
