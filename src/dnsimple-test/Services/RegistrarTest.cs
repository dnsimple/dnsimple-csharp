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

        private const string GetDomainPremiumPriceFailureFixture =
            "getDomainPremiumPrice/failure.http";

        private const string GetDomainPricesFixture =
            "getDomainPrices/success.http";

        private const string GetDomainPricesFailureFixture =
            "getDomainPrices/failure.http";

        private const string RegisterDomainFixture =
            "registerDomain/success.http";

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
                Assert.AreEqual(domainName, check.Domain);
                Assert.IsTrue(check.Available);
                Assert.IsTrue(check.Premium);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", PremiumPriceCheckAction.Registration,
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/ruby.codes/premium_price?action=registration")]
        public void GetDomainPremiumPrice(long accountId, string domainName,
            PremiumPriceCheckAction action, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetDomainPremiumPriceFixture);
            var premiumPrice = client.Registrar
                .GetDomainPremiumPrice(accountId, domainName, action).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual("109.00", premiumPrice.PremiumPrice);
                Assert.AreEqual(action.ToString().ToLower(),
                    premiumPrice.Action);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "dnsimple.com", PremiumPriceCheckAction.Registration,
            "https://api.sandbox.dnsimple.com/v2/1010/registrar/domains/dnsimple.com/premium_price?action=registration")]
        public void GetDomainPremiumPriceFailure(long accountId,
            string domainName, PremiumPriceCheckAction action,
            string expectedUrl)
        {
            var client =
                new MockDnsimpleClient(GetDomainPremiumPriceFailureFixture);
            client.StatusCode(HttpStatusCode.BadRequest);

            Assert.Multiple(() =>
            {
                Assert.Throws(
                    Is.TypeOf<DnsimpleValidationException>().And.Message
                        .EqualTo(
                            "`example.com` is not a premium domain for registration"),
                    delegate
                    {
                        client.Registrar
                            .GetDomainPremiumPrice(accountId, domainName,
                                action);
                    });

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual("bingo.pizza", prices.Domain);
                Assert.AreEqual(true, prices.Premium);
                Assert.AreEqual(20.0, prices.RegistrationPrice);
                Assert.AreEqual(20.0, prices.RenewalPrice);
                Assert.AreEqual(20.0, prices.TransferPrice);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(1, registeredDomain.Id);
                Assert.AreEqual(999, registeredDomain.DomainId);
                Assert.AreEqual(2, registeredDomain.RegistrantId);
                Assert.AreEqual(1, registeredDomain.Period);
                Assert.AreEqual("new", registeredDomain.State);
                Assert.IsFalse(registeredDomain.AutoRenew);
                Assert.IsFalse(registeredDomain.WhoisPrivacy);
                Assert.AreEqual(CreatedAt, registeredDomain.CreatedAt);
                Assert.AreEqual(UpdatedAt, registeredDomain.UpdatedAt);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                StringAssert.Contains("uk_legal_type", payload);
                StringAssert.Contains("UK Individual (our default value)", payload);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual("transferring", domain.State);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                StringAssert.Contains("uk_legal_type", payload);
                StringAssert.Contains("UK Individual (our default value)", payload);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(361, domainTransfer.Id);
                Assert.AreEqual(182245, domainTransfer.DomainId);
                Assert.AreEqual(2715, domainTransfer.RegistrantId);
                Assert.AreEqual("cancelled", domainTransfer.State);
                Assert.False(domainTransfer.AutoRenew);
                Assert.False(domainTransfer.WhoisPrivacy);
                Assert.AreEqual("Canceled by customer", domainTransfer.StatusDescription);
                Assert.AreEqual(Convert.ToDateTime("2020-06-05T18:08:00Z"), domainTransfer.CreatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-06-05T18:10:01Z"), domainTransfer.UpdatedAt);
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
                Assert.AreEqual(361, domainTransfer.Id);
                Assert.AreEqual(182245, domainTransfer.DomainId);
                Assert.AreEqual(2715, domainTransfer.RegistrantId);
                Assert.AreEqual("transferring", domainTransfer.State);
                Assert.False(domainTransfer.AutoRenew);
                Assert.False(domainTransfer.WhoisPrivacy);
                Assert.IsNull(domainTransfer.StatusDescription);
                Assert.AreEqual(Convert.ToDateTime("2020-06-05T18:08:00Z"), domainTransfer.CreatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-06-05T18:08:04Z"), domainTransfer.UpdatedAt);
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
                Assert.AreEqual("new", domain.State);
                Assert.AreEqual(1, domain.Period);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
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
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}
