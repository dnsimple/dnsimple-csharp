using System;
using System.Collections.Generic;
using System.Linq;
using dnsimple.Services;
using dnsimple.Services.ListOptions;
using NUnit.Framework;
using RestSharp;
using Pagination = dnsimple.Services.ListOptions.Pagination;

namespace dnsimple_test.Services
{
    [TestFixture]
    public class CertificatesTest
    {
        private MockResponse _response;

        private const string ListCertificatesFixture =
            "listCertificates/success.http";

        private const string GetCertificateFixture =
            "getCertificate/success.http";

        private const string DownloadCertificateFixture =
            "downloadCertificate/success.http";

        private const string GetCertificatePrivateKeyFixture =
            "getCertificatePrivateKey/success.http";

        private const string PurchaseLetsEncryptCertificateFixture =
            "purchaseLetsencryptCertificate/success.http";

        private const string IssueLetsEncryptCertificateFixture =
            "issueLetsencryptCertificate/success.http";

        private const string PurchaseRenewalLetsEncryptCertificateFixture =
            "purchaseRenewalLetsencryptCertificate/success.http";

        private const string IssueRenewalLetsEncryptCertificateFixture =
            "issueRenewalLetsencryptCertificate/success.http";

        [SetUp]
        public void Initialize()
        {
            var loader = new FixtureLoader("v2", ListCertificatesFixture);
            _response = new MockResponse(loader);
        }

        [Test]
        public void CertificatesResponse()
        {
            var certificate =
                new PaginatedResponse<Certificate>(_response).Data;
        
            var expectedCSR =
                "-----BEGIN CERTIFICATE REQUEST-----\n" +
                "MIICYDCCAUgCAQAwGzEZMBcGA1UEAwwQd3d3Mi5kbnNpbXBsZS51czCCASIwDQYJ\n" +
                "KoZIhvcNAQEBBQADggEPADCCAQoCggEBAMjXrephLTu7OKVQ6F3LhmLkL6NL3ier\n" +
                "1qaWPtJBbkBuzJIn8gmSG+6xGmywB6GKvP2IVkPQhPBpfc8wsTd26rbSBHnRIQal\n" +
                "tk+W4aQZyIeXFARY+cRvpjeAtmpX0vwZkDMoEyhFomBfGxVfx6tSqdGlR88/x0By\n" +
                "y5u7+xwkY+4jMt+wZi+wpXsScumB6DAC1PTYRvNFQy7Gcjqrc3EdzPsn3c9kLCNO\n" +
                "3GCPJoWmT5Rtyd7FxjJiSIf7BDOi12BnblpSLwGvtu6Wrl+u9LJLj8zeCACwUiQG\n" +
                "uvnP2lAl2YacNAgpql6C2eEnFjIub7Ul1QMUImQSDVy5dMd/UGQrOb0CAwEAAaAA\n" +
                "MA0GCSqGSIb3DQEBCwUAA4IBAQA8oVxOrZCGeSFmKpNV4oilzPOepTVSWxXa19T7\n" +
                "zD/azh6j6RBLZPpG4TFbpvjecum+1V7Y8ypIcwhRtlh5/zSbfJkjJsdCdZU9XZat\n" +
                "T5YkOaxuCUCDajpRiyyKhHvrloTPKPXe5ygCq/Q23xm//VrXKArLSWVB9qWS6gDV\n" +
                "k0y3/mIlTQ3mTgfYQySc3MPXvIgUoqmB8Ajfq1n3hSLgb1/OoKNfeVEWsON116cq\n" +
                "bXvl63+XzPubj6KWZXZH/jhrs53fuLq3xyeeuOaPrn+2VceBVt4DCC9n0JS5wepl\n" +
                "HDoVxtWTTNeJdP5xFB5V1KI+D4FEFBUGnQABEvajpU3vljh3\n" +
                "-----END CERTIFICATE REQUEST-----\n";

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, certificate.Count);
                Assert.AreEqual(101973, certificate.First().Id);
                Assert.AreEqual(14279, certificate.First().DomainId);
                Assert.AreEqual(11435, certificate.First().ContactId);
                Assert.AreEqual("www2.dnsimple.us", certificate.First().CommonName);
                Assert.AreEqual(1, certificate.First().Years);
                Assert.AreEqual(expectedCSR, certificate.First().Csr);
                Assert.AreEqual("issued", certificate.First().State);
                Assert.IsFalse(certificate.First().AutoRenew);
                Assert.IsEmpty(certificate.First().AlternateNames);
                Assert.AreEqual("letsencrypt", certificate.First().AuthorityIdentifier);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T20:15:09Z"), certificate.First().CreatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T20:30:08Z"), certificate.First().UpdatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-09-16T19:30:07Z"), certificate.First().ExpiresAt);
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/certificates")]
        public void ListCertificates(long accountId, string domainName,
            string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListCertificatesFixture);
            var response =
                client.Certificates.ListCertificates(accountId, domainName);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, response.Data.Count);
                Assert.AreEqual(1, response.Pagination.CurrentPage);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/certificates?sort=id:asc%2ccommon_name:desc%2cexpiration:asc&per_page=42&page=7")]
        public void ListCertificatesWithOptions(long accountId,
            string domainName, string expectedUrl)
        {
            var client = new MockDnsimpleClient(ListCertificatesFixture);
            var options = new CertificatesListOptions
                {
                    Pagination = new Pagination
                    {
                        PerPage = 42,
                        Page = 7
                    }
                }.SortById(Order.asc)
                .SortByCommonName(Order.desc)
                .SortByExpiration(Order.asc);

            var response =
                client.Certificates.ListCertificates(accountId, domainName,
                    options);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, response.Data.Count);
                Assert.AreEqual(1, response.Pagination.CurrentPage);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "bingo.pizza", 101967,
            "https://api.sandbox.dnsimple.com/v2/1010/domains/bingo.pizza/certificates/101967")]
        public void GetCertificate(long accountId, string domainName,
            long certificateId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(GetCertificateFixture);
            var certificate =
                client.Certificates.GetCertificate(accountId, domainName,
                    certificateId).Data;

            var expectedCertificate =
                "-----BEGIN CERTIFICATE REQUEST-----\n" +
                "MIICmTCCAYECAQAwGjEYMBYGA1UEAwwPd3d3LmJpbmdvLnBpenphMIIBIjANBgkq\n" +
                "hkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAw4+KoZ9IDCK2o5qAQpi+Icu5kksmjQzx\n" +
                "5o5g4B6XhRxhsfHlK/i3iU5hc8CONjyVv8j82835RNsiKrflnxGa9SH68vbQfcn4\n" +
                "IpbMz9c+Eqv5h0Euqlc3A4DBzp0unEu5QAUhR6Xu1TZIWDPjhrBOGiszRlLQcp4F\n" +
                "zy6fD6j5/d/ylpzTp5v54j+Ey31Bz86IaBPtSpHI+Qk87Hs8DVoWxZk/6RlAkyur\n" +
                "XDGWnPu9n3RMfs9ag5anFhggLIhCNtVN4+0vpgPQ59pqwYo8TfdYzK7WSKeL7geu\n" +
                "CqVE3bHAqU6dLtgHOZfTkLwGycUh4p9aawuc6fsXHHYDpIL8s3vAvwIDAQABoDow\n" +
                "OAYJKoZIhvcNAQkOMSswKTAnBgNVHREEIDAeggtiaW5nby5waXp6YYIPd3d3LmJp\n" +
                "bmdvLnBpenphMA0GCSqGSIb3DQEBCwUAA4IBAQBwOLKv+PO5hSJkgqS6wL/wRqLh\n" +
                "Q1zbcHRHAjRjnpRz06cDvN3X3aPI+lpKSNFCI0A1oKJG7JNtgxX3Est66cuO8ESQ\n" +
                "PIb6WWN7/xlVlBCe7ZkjAFgN6JurFdclwCp/NI5wBCwj1yb3Ar5QQMFIZOezIgTI\n" +
                "AWkQSfCmgkB96d6QlDWgidYDDjcsXugQveOQRPlHr0TsElu47GakxZdJCFZU+WPM\n" +
                "odQQf5SaqiIK2YaH1dWO//4KpTS9QoTy1+mmAa27apHcmz6X6+G5dvpHZ1qH14V0\n" +
                "JoMWIK+39HRPq6mDo1UMVet/xFUUrG/H7/tFlYIDVbSpVlpVAFITd/eQkaW/\n" +
                "-----END CERTIFICATE REQUEST-----\n";


            Assert.Multiple(() =>
            {
                Assert.AreEqual(101967, certificate.Id);
                Assert.AreEqual(289333, certificate.DomainId);
                Assert.AreEqual(2511, certificate.ContactId);
                Assert.AreEqual("www.bingo.pizza", certificate.CommonName);
                Assert.AreEqual(1, certificate.Years);
                Assert.AreEqual(expectedCertificate, certificate.Csr);
                Assert.AreEqual("issued", certificate.State);
                Assert.IsFalse(certificate.AutoRenew);
                CollectionAssert.IsEmpty(certificate.AlternateNames);
                Assert.AreEqual("letsencrypt", certificate.AuthorityIdentifier);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T18:54:17Z"), certificate.CreatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T19:10:14Z"), certificate.UpdatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-09-16T18:10:13Z"), certificate.ExpiresAt);
                
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", 1,
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/certificates/1/download")]
        public void DownloadCertificate(long accountId, string domainName,
            long certificateId, string expectedUrl)
        {
            var client = new MockDnsimpleClient(DownloadCertificateFixture);
            var certificate =
                client.Certificates.DownloadCertificate(accountId, domainName,
                    certificateId).Data;
            var serverCertificate =
                "-----BEGIN CERTIFICATE-----\nMIIE7TCCA9WgAwIBAgITAPpTe4O3vju" +
                "Q9L4gLsogi/ukujANBgkqhkiG9w0BAQsF\nADAiMSAwHgYDVQQDDBdGYWtlI" +
                "ExFIEludGVybWVkaWF0ZSBYMTAeFw0xNjA2MTEx\nNzQ4MDBaFw0xNjA5MDk" +
                "xNzQ4MDBaMBkxFzAVBgNVBAMTDnd3dy53ZXBwb3MubmV0\nMIIBIjANBgkqh" +
                "kiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtzCcMfWoQRt5AMEY0HUb\n2GaraL1" +
                "GsWOo6YXdPfe+YDvtnmDw23NcoTX7VSeCgU9M3RKs19AsCJcRNTLJ2dmD\nr" +
                "AuyCTud9YTAaXQcTOLUhtO8T8+9AFVIva2OmAlKCR5saBW3JaRxW7V2aHEd/" +
                "d1s\ns1CvNOO7jNppc9NwGSnDHcn3rqNv/U3MaU0gpJJRqsKkvcLU6IHJGgx" +
                "yQ6AbpwJD\nIqBnzkjHu2IuhGEbRuMjyWLA2qtsjyVlfPotDxUdVouUQpz7d" +
                "GHUFrLR7ma8QAYu\nOfl1ZMyrc901HGMa7zwbnFWurs3fed7vAosTRZIjnn7" +
                "2/3Wo7L9RiMB+vwr3NX7c\n9QIDAQABo4ICIzCCAh8wDgYDVR0PAQH/BAQDA" +
                "gWgMB0GA1UdJQQWMBQGCCsGAQUF\nBwMBBggrBgEFBQcDAjAMBgNVHRMBAf8" +
                "EAjAAMB0GA1UdDgQWBBRh9q/3Zxbk4yA/\nt7j+8xA+rkiZBTAfBgNVHSMEG" +
                "DAWgBTAzANGuVggzFxycPPhLssgpvVoOjB4Bggr\nBgEFBQcBAQRsMGowMwY" +
                "IKwYBBQUHMAGGJ2h0dHA6Ly9vY3NwLnN0Zy1pbnQteDEu\nbGV0c2VuY3J5c" +
                "HQub3JnLzAzBggrBgEFBQcwAoYnaHR0cDovL2NlcnQuc3RnLWlu\ndC14MS5" +
                "sZXRzZW5jcnlwdC5vcmcvMCUGA1UdEQQeMByCCndlcHBvcy5uZXSCDnd3\nd" +
                "y53ZXBwb3MubmV0MIH+BgNVHSAEgfYwgfMwCAYGZ4EMAQIBMIHmBgsrBgEEA" +
                "YLf\nEwEBATCB1jAmBggrBgEFBQcCARYaaHR0cDovL2Nwcy5sZXRzZW5jcnl" +
                "wdC5vcmcw\ngasGCCsGAQUFBwICMIGeDIGbVGhpcyBDZXJ0aWZpY2F0ZSBtY" +
                "Xkgb25seSBiZSBy\nZWxpZWQgdXBvbiBieSBSZWx5aW5nIFBhcnRpZXMgYW5" +
                "kIG9ubHkgaW4gYWNjb3Jk\nYW5jZSB3aXRoIHRoZSBDZXJ0aWZpY2F0ZSBQb" +
                "2xpY3kgZm91bmQgYXQgaHR0cHM6\nLy9sZXRzZW5jcnlwdC5vcmcvcmVwb3N" +
                "pdG9yeS8wDQYJKoZIhvcNAQELBQADggEB\nAEqMdWrmdIyQxthWsX3iHmM2h" +
                "/wXwEesD0VIaA+Pq4mjwmKBkoPSmHGQ/O4v8RaK\nB6gl8v+qmvCwwqC1SkB" +
                "mm+9C2yt/P6WhAiA/DD+WppYgJWfcz2lEKrgufFlHPukB\nDzE0mJDuXm09Q" +
                "TApWlaTZWYfWKY50T5uOT/rs+OwGFFCO/8o7v5AZRAHos6uzjvq\nAtFZj/F" +
                "EnXXMjSSlQ7YKTXToVpnAYH4e3/UMsi6/O4orkVz82ZfhKwMWHV8dXlRw\nt" +
                "QaemFWTjGPgSLXJAtQO30DgNJBHX/fJEaHv6Wy8TF3J0wOGpzGbOwaTX8YAm" +
                "EzC\nlzzjs+clg5MN5rd1g4POJtU=\n-----END CERTIFICATE-----\n";

            var chainCertificate =
                "-----BEGIN CERTIFICATE-----\nMIIEqzCCApOgAwIBAgIRAIvhKg5ZRO0" +
                "8VGQx8JdhT+UwDQYJKoZIhvcNAQELBQAw\nGjEYMBYGA1UEAwwPRmFrZSBMR" +
                "SBSb290IFgxMB4XDTE2MDUyMzIyMDc1OVoXDTM2\nMDUyMzIyMDc1OVowIjE" +
                "gMB4GA1UEAwwXRmFrZSBMRSBJbnRlcm1lZGlhdGUgWDEw\nggEiMA0GCSqGS" +
                "Ib3DQEBAQUAA4IBDwAwggEKAoIBAQDtWKySDn7rWZc5ggjz3ZB0\n8jO4xti" +
                "3uzINfD5sQ7Lj7hzetUT+wQob+iXSZkhnvx+IvdbXF5/yt8aWPpUKnPym\no" +
                "LxsYiI5gQBLxNDzIec0OIaflWqAr29m7J8+NNtApEN8nZFnf3bhehZW7AxmS" +
                "1m0\nZnSsdHw0Fw+bgixPg2MQ9k9oefFeqa+7Kqdlz5bbrUYV2volxhDFtnI" +
                "4Mh8BiWCN\nxDH1Hizq+GKCcHsinDZWurCqder/afJBnQs+SBSL6MVApHt+d" +
                "35zjBD92fO2Je56\ndhMfzCgOKXeJ340WhW3TjD1zqLZXeaCyUNRnfOmWZV8" +
                "nEhtHOFbUCU7r/KkjMZO9\nAgMBAAGjgeMwgeAwDgYDVR0PAQH/BAQDAgGGM" +
                "BIGA1UdEwEB/wQIMAYBAf8CAQAw\nHQYDVR0OBBYEFMDMA0a5WCDMXHJw8+E" +
                "uyyCm9Wg6MHoGCCsGAQUFBwEBBG4wbDA0\nBggrBgEFBQcwAYYoaHR0cDovL" +
                "29jc3Auc3RnLXJvb3QteDEubGV0c2VuY3J5cHQu\nb3JnLzA0BggrBgEFBQc" +
                "wAoYoaHR0cDovL2NlcnQuc3RnLXJvb3QteDEubGV0c2Vu\nY3J5cHQub3JnL" +
                "zAfBgNVHSMEGDAWgBTBJnSkikSg5vogKNhcI5pFiBh54DANBgkq\nhkiG9w0" +
                "BAQsFAAOCAgEABYSu4Il+fI0MYU42OTmEj+1HqQ5DvyAeyCA6sGuZdwjF\nU" +
                "GeVOv3NnLyfofuUOjEbY5irFCDtnv+0ckukUZN9lz4Q2YjWGUpW4TTu3ieTs" +
                "aC9\nAFvCSgNHJyWSVtWvB5XDxsqawl1KzHzzwr132bF2rtGtazSqVqK9E07" +
                "sGHMCf+zp\nDQVDVVGtqZPHwX3KqUtefE621b8RI6VCl4oD30Olf8pjuzG4J" +
                "KBFRFclzLRjo/h7\nIkkfjZ8wDa7faOjVXx6n+eUQ29cIMCzr8/rNWHS9pYG" +
                "GQKJiY2xmVC9h12H99Xyf\nzWE9vb5zKP3MVG6neX1hSdo7PEAb9fqRhHkqV" +
                "sqUvJlIRmvXvVKTwNCP3eCjRCCI\nPTAvjV+4ni786iXwwFYNz8l3PmPLCyQ" +
                "XWGohnJ8iBm+5nk7O2ynaPVW0U2W+pt2w\nSVuvdDM5zGv2f9ltNWUiYZHJ1" +
                "mmO97jSY/6YfdOUH66iRtQtDkHBRdkNBsMbD+Em\n2TgBldtHNSJBfB3pm9F" +
                "blgOcJ0FSWcUDWJ7vO0+NTXlgrRofRT6pVywzxVo6dND0\nWzYlTWeUVsO40" +
                "xJqhgUQRER9YLOLxJ0O6C8i0xFxAMKOtSdodMB3RIwt7RFQ0uyt\nn5Z5Mqk" +
                "YhlMI3J1tPRTp1nEt9fyGspBOO05gi148Qasp+3N+svqKomoQglNoAxU=\n-" +
                "----END CERTIFICATE-----";

            Assert.Multiple(() =>
            {
                Assert.AreEqual(serverCertificate, certificate.ServerCertificate);
                Assert.IsNull(certificate.RootCertificate);
                Assert.Contains(chainCertificate, certificate.IntermediateCertificates);

                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "ruby.codes", 1,
            "https://api.sandbox.dnsimple.com/v2/1010/domains/ruby.codes/certificates/1/private_key")]
        public void GetCertificatePrivateKey(long accountId, string domainName,
            long certificateId, string expectedUrl)
        {
            var client =
                new MockDnsimpleClient(GetCertificatePrivateKeyFixture);
            var privateKey =
                client.Certificates.GetCertificatePrivateKey(accountId,
                    domainName, certificateId).Data;

            var expectedPrivateKey =
                "-----BEGIN RSA PRIVATE KEY-----\nMIIEowIBAAKCAQEAtzCcMfWoQRt" +
                "5AMEY0HUb2GaraL1GsWOo6YXdPfe+YDvtnmDw\n23NcoTX7VSeCgU9M3RKs1" +
                "9AsCJcRNTLJ2dmDrAuyCTud9YTAaXQcTOLUhtO8T8+9\nAFVIva2OmAlKCR5" +
                "saBW3JaRxW7V2aHEd/d1ss1CvNOO7jNppc9NwGSnDHcn3rqNv\n/U3MaU0gp" +
                "JJRqsKkvcLU6IHJGgxyQ6AbpwJDIqBnzkjHu2IuhGEbRuMjyWLA2qts\njyV" +
                "lfPotDxUdVouUQpz7dGHUFrLR7ma8QAYuOfl1ZMyrc901HGMa7zwbnFWurs3" +
                "f\ned7vAosTRZIjnn72/3Wo7L9RiMB+vwr3NX7c9QIDAQABAoIBAEQx32Olz" +
                "K34GTKT\nr7Yicmw7xEGofIGa1Q2h3Lut13whsxKLif5X0rrcyqRnoeibacS" +
                "+qXXrJolIG4rP\nTl8/3wmUDQHs5J+6fJqFM+fXZUCP4AFiFzzhgsPBsVyd0" +
                "KbWYYrZ0qU7s0ttoRe+\nTGjuHgIe3ip1QKNtx2Xr50YmytDydknmro79J5G" +
                "frub1l2iA8SDm1eBrQ4SFaNQ2\nU709pHeSwX8pTihUX2Zy0ifpr0O1wYQjG" +
                "LneMoG4rrNQJG/z6iUdhYczwwt1kDRQ\n4WkM2sovFOyxbBfoCQ3Gy/eem7O" +
                "XfjNKUe47DAVLnPkKbqL/3Lo9FD7kcB8K87Ap\nr/vYrl0CgYEA413RAk757" +
                "1w5dM+VftrdbFZ+Yi1OPhUshlPSehavro8kMGDEG5Ts\n74wEz2X3cfMxauM" +
                "pMrBk/XnUCZ20AnWQClK73RB5fzPw5XNv473Tt/AFmt7eLOzl\nOcYrhpEHe" +
                "gtsD/ZaljlGtPqsjQAL9Ijhao03m1cGB1+uxI7FgacdckcCgYEAzkKP\n6xu" +
                "9+WqOol73cnlYPS3sSZssyUF+eqWSzq2YJGRmfr1fbdtHqAS1ZbyC5fZVNZY" +
                "V\nml1vfXi2LDcU0qS04JazurVyQr2rJZMTlCWVET1vhik7Y87wgCkLwKpbw" +
                "amPDmlI\n9GY+fLNEa4yfAOOpvpTJpenUScxyKWH2cdYFOOMCgYBhrJnvffI" +
                "NC/d64Pp+BpP8\nyKN+lav5K6t3AWd4H2rVeJS5W7ijiLTIq8QdPNayUyE1o" +
                "+S8695WrhGTF/aO3+ZD\nKQufikZHiQ7B43d7xL7BVBF0WK3lateGnEVyh7d" +
                "IjMOdj92Wj4B6mv2pjQ2VvX/p\nAEWVLCtg24/+zL64VgxmXQKBgGosyXj1Z" +
                "u2ldJcQ28AJxup3YVLilkNje4AXC2No\n6RCSvlAvm5gpcNGE2vvr9lX6YBK" +
                "dl7FGt8WXBe/sysNEFfgmm45ZKOBCUn+dHk78\nqaeeQHKHdxMBy7utZWdgS" +
                "qt+ZS299NgaacA3Z9kVIiSLDS4V2VeW7riujXXP/9TJ\nnxaRAoGBAMWXOfN" +
                "VzfTyrKff6gvDWH+hqNICLyzvkEn2utNY9Q6WwqGuY9fvP/4Z\nXzc48AOBz" +
                "Ur8OeA4sHKJ79sJirOiWHNfD1swtvyVzsFZb6moiNwD3Ce/FzYCa3lQ\nU8b" +
                "lTH/uqpR2pSC6whzJ/lnSdqHUqhyp00000000000000000000\n-----END " +
                "RSA PRIVATE KEY-----\n";

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedPrivateKey, privateKey.PrivateKey);
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "bingo.pizza",
            "https://api.sandbox.dnsimple.com/v2/1010/domains/bingo.pizza/certificates/letsencrypt")]
        public void PurchaseLetsEncryptCertificate(long accountId,
            string domainName, string expectedUrl)
        {
            var client =
                new MockDnsimpleClient(PurchaseLetsEncryptCertificateFixture);
            var certificateAttributes = new LetsencryptCertificateAttributes
            {
                ContactId = 11,
                AutoRenew = false,
                Name = "SuperCertificate",
                AlternateNames = new List<string>{"docs.bingo.pizza", "status.bingo.pizza"}
            };

            var certificateOrdered =
                client.Certificates.PurchaseLetsEncryptCertificate(accountId,
                    domainName, certificateAttributes).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(101967, certificateOrdered.Id);
                Assert.AreEqual(101967, certificateOrdered.CertificateId);
                Assert.AreEqual("new", certificateOrdered.State);
                Assert.IsFalse(certificateOrdered.AutoRenew);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T18:54:17Z"), certificateOrdered.CreatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T18:54:17Z"), certificateOrdered.UpdatedAt);

                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "bingo.pizza", 101967,
            "https://api.sandbox.dnsimple.com/v2/1010/domains/bingo.pizza/certificates/letsencrypt/101967/issue")]
        public void IssueLetsEncryptCertificate(long accountId,
            string domainName, long certificateId, string expectedUrl)
        {
            var client =
                new MockDnsimpleClient(IssueLetsEncryptCertificateFixture);
            var certificate = client.Certificates
                .IssueLetsEncryptCertificate(accountId, domainName,
                    certificateId).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(101967, certificate.Id);
                Assert.AreEqual(289333, certificate.DomainId);
                Assert.AreEqual(2511, certificate.ContactId);
                Assert.AreEqual("www.bingo.pizza", certificate.CommonName);
                Assert.AreEqual(1, certificate.Years);
                Assert.IsNull(certificate.Csr);
                Assert.AreEqual("requesting", certificate.State);
                Assert.IsFalse(certificate.AutoRenew);
                CollectionAssert.IsEmpty(certificate.AlternateNames);
                Assert.AreEqual("letsencrypt", certificate.AuthorityIdentifier);

                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "bingo.pizza", 101967,
            "https://api.sandbox.dnsimple.com/v2/1010/domains/bingo.pizza/certificates/letsencrypt/101967/renewals")]
        public void PurchaseLetsEncryptCertificateRenewal(long accountId,
            string domainName, long certificateId, string expectedUrl)
        {
            var client =
                new MockDnsimpleClient(
                    PurchaseRenewalLetsEncryptCertificateFixture);
            var certificateAttributes = new LetsencryptCertificateAttributes
            {
                AutoRenew = false
            };
            var renewalPurchased =
                client.Certificates.PurchaseLetsEncryptCertificateRenewal(
                    accountId, domainName, certificateId, certificateAttributes).Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(65082, renewalPurchased.Id);
                Assert.AreEqual(101967, renewalPurchased.OldCertificateId);
                Assert.AreEqual(101972, renewalPurchased.NewCertificateId);
                Assert.AreEqual("new", renewalPurchased.State);
                Assert.IsFalse(renewalPurchased.AutoRenew);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T19:56:20Z"), renewalPurchased.CreatedAt);
                Assert.AreEqual(Convert.ToDateTime("2020-06-18T19:56:20Z"), renewalPurchased.UpdatedAt);

                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }

        [Test]
        [TestCase(1010, "bingo.pizza", 101967, 65082,
            "https://api.sandbox.dnsimple.com/v2/1010/domains/bingo.pizza/certificates/letsencrypt/101967/renewals/65082/issue")]
        public void IssueLetsEncryptCertificateRenewal(long accountId,
            string domainName, long certificateId, long certificateRenewalId,
            string expectedUrl)
        {
            var client =
                new MockDnsimpleClient(
                    IssueRenewalLetsEncryptCertificateFixture);
            var renewalIssued =
                client.Certificates.IssueLetsEncryptCertificateRenewal(
                        accountId, domainName, certificateId,
                        certificateRenewalId)
                    .Data;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(101972, renewalIssued.Id);
                Assert.AreEqual(289333, renewalIssued.DomainId);
                Assert.AreEqual(2511, renewalIssued.ContactId);
                Assert.AreEqual("www.bingo.pizza", renewalIssued.CommonName);
                Assert.AreEqual(1, renewalIssued.Years);
                Assert.IsNull(renewalIssued.Csr);
                Assert.AreEqual("requesting", renewalIssued.State);
                Assert.IsFalse(renewalIssued.AutoRenew);
                CollectionAssert.IsEmpty(renewalIssued.AlternateNames);
                Assert.AreEqual("letsencrypt", renewalIssued.AuthorityIdentifier);

                Assert.AreEqual(Method.POST, client.HttpMethodUsed());
                Assert.AreEqual(expectedUrl, client.RequestSentTo());
            });
        }
    }
}
