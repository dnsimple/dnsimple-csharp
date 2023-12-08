using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using dnsimple;
using dnsimple.Services;
using NUnit.Framework;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class RegistrarServicesTest
    {
        private const string CheckDomainFixture = "checkDomain/success.http";

        private const string GetDomainPremiumPriceFixture =
            "getDomainPremiumPrice/success.http";

        private const string GetDomainPricesFixture =
            "getDomainPrices/success.http";

        private const string GetDomainPricesFailureFixture =
            "getDomainPrices/failure.http";

        private const string RegisterDomainFixture =
            "registerDomain/success.http";

        private const string GetDomainRegistrationFixture =
            "getDomainRegistration/success.http";

        private const string TransferDomainFixture =
            "transferDomain/success.http";

        private const string TransferDomainMissingAuthCodeFixture =
            "transferDomain/error-missing-authcode.http";

        private const string TransferDomainErrorInDnsimpleFixture =
            "transferDomain/error-indnsimple.http";

        private const string GetDomainTransferFixture =
            "getDomainTransfer/success.http";

        private const string CancelDomainTransferFixture =
            "cancelDomainTransfer/success.http";

        private const string RenewDomainFixture = "renewDomain/success.http";

        private const string GetDomainRenewalFixture = "getDomainRenewal/success.http";

        private const string RenewDomainTooEarlyFixture =
            "renewDomain/error-tooearly.http";

        private const string AuthorizeTransferOutFixture =
            "authorizeDomainTransferOut/success.http";

        private DateTime CreatedAt { get; } = DateTime.ParseExact(
            "2016-12-09T19:35:31Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        private DateTime UpdatedAt { get; } = DateTime.ParseExact(
            "2016-12-09T19:35:31Z", "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.CurrentCulture);

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/check")]
        public void CheckDomain(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(CheckDomainFixture);
            var check = client.Registrar.CheckDomain(accountId, domainName)
                .Data;

            Assert.Multiple(() =>
            {
                Assert.That(check.Domain, Is.EqualTo(domainName));
                Assert.IsTrue(check.Available);
                Assert.IsTrue(check.Premium);

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "bingo.pizza",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/bingo.pizza/prices")]
        public void GetDomainPrices(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainPricesFixture);
            var prices = client.Registrar
                .GetDomainPrices(accountId, domainName).Data;

            Assert.Multiple(() =>
            {
                Assert.That(prices.Domain, Is.EqualTo("bingo.pizza"));
                Assert.That(prices.Premium, Is.EqualTo(true));
                Assert.That(prices.RegistrationPrice, Is.EqualTo(20.0));
                Assert.That(prices.RenewalPrice, Is.EqualTo(20.0));
                Assert.That(prices.TransferPrice, Is.EqualTo(20.0));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "bingo.pineapple",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/bingo.pineapple/prices")]
        public void GetDomainPricesFailure(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainPricesFailureFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            Assert.Multiple(() =>
            {
                Assert.Throws(
                    Is.TypeOf<DnsimpleValidationException>().And.Message
                        .EqualTo(
                            "TLD .PINEAPPLE is not supported"),
                    delegate
                    {
                        client.Registrar
                            .GetDomainPrices(accountId, domainName);
                    });

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }


        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/registrations")]
        public void RegisterDomain(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(RegisterDomainFixture);
            var domain = new DomainRegistrationInput
            {
                RegistrantId = 2,
                WhoisPrivacy = false,
                AutoRenew = false,
                PremiumPrice = ""
            };

            var registeredDomain =
                client.Registrar.RegisterDomain(accountId, domainName, domain)
                    .Data;

            Assert.Multiple(() =>
            {
                Assert.That(registeredDomain.Id, Is.EqualTo(1));
                Assert.That(registeredDomain.DomainId, Is.EqualTo(999));
                Assert.That(registeredDomain.RegistrantId, Is.EqualTo(2));
                Assert.That(registeredDomain.Period, Is.EqualTo(1));
                Assert.That(registeredDomain.State, Is.EqualTo("new"));
                Assert.IsFalse(registeredDomain.AutoRenew);
                Assert.IsFalse(registeredDomain.WhoisPrivacy);
                Assert.That(registeredDomain.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(registeredDomain.UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", 361,
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/registrations/361")]
        public void GetDomainRegistration(long accountId, string domainName,
            long domainRegistrationId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainRegistrationFixture);
            var domain = client.Registrar.GetDomainRegistration(accountId,
                domainName, domainRegistrationId).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domain.Id, Is.EqualTo(361));
                Assert.That(domain.DomainId, Is.EqualTo(104040));
                Assert.That(domain.RegistrantId, Is.EqualTo(2715));
                Assert.That(domain.Period, Is.EqualTo(1));
                Assert.That(domain.State, Is.EqualTo("registering"));
                Assert.IsFalse(domain.AutoRenew);
                Assert.IsFalse(domain.WhoisPrivacy);
                Assert.That(domain.CreatedAt, Is.EqualTo(CreatedAt));
                Assert.That(domain.UpdatedAt, Is.EqualTo(UpdatedAt));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/registrations")]
        public void RegisterDomainWithExtendedAttributes(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(RegisterDomainFixture);
            var domain = new DomainRegistrationInput
            {
                RegistrantId = 2,
                WhoisPrivacy = false,
                AutoRenew = false,
                PremiumPrice = "",
                ExtendedAttributes = new List<TldExtendedAttribute>
                {
                    new TldExtendedAttribute()
                    {
                        Name = "uk_legal_type",
                        Description = "Legal type of registrant contact",
                        Required = false,
                        Options = new List<TldExtendedAttributeOption>
                        { new TldExtendedAttributeOption()
                            {
                                Title = "UK Individual",
                                Value = "IND",
                                Description = "UK Individual (our default value)"
                            }
                        }
                    }
                }
            };

            client.Registrar.RegisterDomain(accountId, domainName, domain);
            var payload = client.PayloadSent();

            Assert.Multiple(() =>
            {
                Assert.That(payload, Does.Contain("uk_legal_type"));
                Assert.That(payload, Does.Contain("UK Individual (our default value)"));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/transfers")]
        public void TransferDomain(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(TransferDomainFixture);

            var transfer = new DomainTransferInput
            {
                RegistrantId = 2,
                AuthCode = "authcode"
            };

            var domain =
                client.Registrar.TransferDomain(accountId, domainName,
                    transfer).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domain.State, Is.EqualTo("transferring"));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes")]
        public void TransferDomainNoAuthCode(long accountId, string domainName)
        {
            var client =
                new MockDnsimpleClient(TransferDomainMissingAuthCodeFixture);

            var transfer = new DomainTransferInput
            {
                RegistrantId = 2
            };


            Assert.Throws(
                Is.TypeOf<DnsimpleException>().And.Message
                    .EqualTo("Please provide an AuthCode"), delegate
                {
                    client.Registrar.TransferDomain(accountId, domainName,
                        transfer);
                });
        }

        [Test]
        [TestCase(1010, "ruby.codes")]
        public void TransferDomainEmptyAuthCode(long accountId,
            string domainName)
        {
            var client =
                new MockDnsimpleClient(TransferDomainMissingAuthCodeFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            var transfer = new DomainTransferInput
            {
                RegistrantId = 2,
                AuthCode = ""
            };


            Assert.Throws(Is.TypeOf<DnsimpleValidationException>(), delegate
            {
                client.Registrar.TransferDomain(accountId, domainName,
                    transfer);
            });
        }

        [Test]
        [TestCase(1010, "google.com")]
        public void TransferDomainErrorInDnsimple(long accountId,
            string domainName)
        {
            var client =
                new MockDnsimpleClient(TransferDomainErrorInDnsimpleFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            var transfer = new DomainTransferInput
            {
                RegistrantId = 2,
                AuthCode = "gimmegoogle"
            };


            Assert.Throws(Is.TypeOf<DnsimpleValidationException>()
                    .And
                    .Message
                    .EqualTo(
                        "The domain google.com is already in DNSimple and cannot be added"),
                delegate
                {
                    client.Registrar.TransferDomain(accountId, domainName,
                        transfer);
                });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/transfers")]
        public void TransferDomainWithExtendedAttributes(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(RegisterDomainFixture);
            var transfer = new DomainTransferInput
            {
                RegistrantId = 2,
                AuthCode = "authcode",
                ExtendedAttributes = new List<TldExtendedAttribute>
                {
                    new TldExtendedAttribute()
                    {
                        Name = "uk_legal_type",
                        Description = "Legal type of registrant contact",
                        Required = false,
                        Options = new List<TldExtendedAttributeOption>
                        { new TldExtendedAttributeOption()
                            {
                                Title = "UK Individual",
                                Value = "IND",
                                Description = "UK Individual (our default value)"
                            }
                        }
                    }
                }
            };

            client.Registrar.TransferDomain(accountId, domainName, transfer);
            var payload = client.PayloadSent();

            Assert.Multiple(() =>
            {
                Assert.That(payload, Does.Contain("uk_legal_type"));
                Assert.That(payload, Does.Contain("UK Individual (our default value)"));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", 361)]
        public void GetDomainTransfer(long accountId, string domainName, long domainTansferId)
        {
            var client = new MockDnsimpleClient(GetDomainTransferFixture);
            var domainTransfer = client.Registrar.GetDomainTransfer(accountId, domainName, domainTansferId).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domainTransfer.Id, Is.EqualTo(361));
                Assert.That(domainTransfer.DomainId, Is.EqualTo(182245));
                Assert.That(domainTransfer.RegistrantId, Is.EqualTo(2715));
                Assert.That(domainTransfer.State, Is.EqualTo("cancelled"));
                Assert.False(domainTransfer.AutoRenew);
                Assert.False(domainTransfer.WhoisPrivacy);
                Assert.That(domainTransfer.StatusDescription, Is.EqualTo("Canceled by customer"));
                Assert.That(domainTransfer.CreatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-05T18:08:00Z")));
                Assert.That(domainTransfer.UpdatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-05T18:10:01Z")));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", 361)]
        public void CancelDomainTransfer(long accountId, string domainName, long domainTansferId)
        {
            var client = new MockDnsimpleClient(CancelDomainTransferFixture);
            var domainTransfer = client.Registrar.CancelDomainTransfer(accountId, domainName, domainTansferId).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domainTransfer.Id, Is.EqualTo(361));
                Assert.That(domainTransfer.DomainId, Is.EqualTo(182245));
                Assert.That(domainTransfer.RegistrantId, Is.EqualTo(2715));
                Assert.That(domainTransfer.State, Is.EqualTo("transferring"));
                Assert.False(domainTransfer.AutoRenew);
                Assert.False(domainTransfer.WhoisPrivacy);
                Assert.IsNull(domainTransfer.StatusDescription);
                Assert.That(domainTransfer.CreatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-05T18:08:00Z")));
                Assert.That(domainTransfer.UpdatedAt, Is.EqualTo(Convert.ToDateTime("2020-06-05T18:08:04Z")));
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/renewals")]
        public void RenewDomain(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(RenewDomainFixture);
            var renewal = new DomainRenewalInput
            {
                Period = 1,
                PremiumPrice = "14"
            };

            var domain = client.Registrar
                .RenewDomain(accountId, domainName, renewal).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domain.State, Is.EqualTo("new"));
                Assert.That(domain.Period, Is.EqualTo(1));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com", 1,
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/example.com/renewals/1")]
        public void GetDomainRenewal(long accountId, string domainName, long domainRenewalId,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainRenewalFixture);
            var domainRenewal = client.Registrar.GetDomainRenewal(accountId, domainName, domainRenewalId).Data;

            Assert.Multiple(() =>
            {
                Assert.That(domainRenewal.Id, Is.EqualTo(1));
                Assert.That(domainRenewal.DomainId, Is.EqualTo(999));
                Assert.That(domainRenewal.Period, Is.EqualTo(1));
                Assert.That(domainRenewal.State, Is.EqualTo("renewed"));
                Assert.That(domainRenewal.CreatedAt, Is.EqualTo(Convert.ToDateTime("2016-12-09T19:46:45Z")));
                Assert.That(domainRenewal.UpdatedAt, Is.EqualTo(Convert.ToDateTime("2016-12-12T19:46:45Z")));

                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }

        [Test]
        [TestCase(1010, "example.com")]
        public void RenewDomainTooEarly(long accountId, string domainName)
        {
            var client = new MockDnsimpleClient(RenewDomainTooEarlyFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            var renewal = new DomainRenewalInput
            {
                Period = 1,
                PremiumPrice = "14"
            };

            Assert.Throws(Is.TypeOf<DnsimpleValidationException>()
                    .And
                    .Message
                    .EqualTo("example.com may not be renewed at this time"),
                delegate
                {
                    client.Registrar.RenewDomain(accountId, domainName,
                        renewal);
                });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/authorize_transfer_out")]
        public void AuthorizeDomainTransferOut(long accountId,
            string domainName, string expectedUrl)
        {
            var client = new MockDnsimpleClient(AuthorizeTransferOutFixture);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(
                    delegate
                    {
                        client.Registrar.TransferDomainOut(accountId,
                            domainName);
                    }
                );
                Assert.That(client.RequestSentTo(), Is.EqualTo(expectedUrl));
            });
        }
    }
}
