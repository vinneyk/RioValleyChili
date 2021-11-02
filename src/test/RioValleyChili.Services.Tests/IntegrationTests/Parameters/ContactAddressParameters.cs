﻿using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class ContactAddressParameters : IContactAddressReturn
    {
        public string ContactAddressKey { get; set; }
        public string AddressDescription { get; set; }
        public Address Address { get; set; }
    }
}