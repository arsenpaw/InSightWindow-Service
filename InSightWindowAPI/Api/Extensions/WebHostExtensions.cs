using Application.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore;
using System.Security.Cryptography.X509Certificates;

namespace InSightWindowAPI.Extensions
{
    public static class WebHostExtensions
    {
        public static void AddVaultCertificate(this ConfigureWebHostBuilder opt, IConfiguration configuration)
        {
            var vaultConfig = new KeyVault();
            configuration.GetSection("KeyVault").Bind(vaultConfig);

            var credential = new DefaultAzureCredential();
            var certificateClient = new CertificateClient(new Uri(vaultConfig.Url), credential);
            var secretClient = new SecretClient(new Uri(vaultConfig.Url), credential);

            // Retrieve the certificate (public part)
            var certificateResponse = certificateClient.GetCertificate(vaultConfig.CertificateName);

            // Retrieve the private key (secret part)
            var secretResponse = secretClient.GetSecret(certificateResponse.Value.Name);
            var privateKeyBytes = Convert.FromBase64String(secretResponse.Value.Value);

            // Combine the public and private key into a single X509Certificate2 object
            var x509Certificate = new X509Certificate2(privateKeyBytes, (string)null, X509KeyStorageFlags.MachineKeySet);

            opt.ConfigureKestrel(opt =>
            {
                opt.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        httpsOptions.ServerCertificate = x509Certificate;
                    });
            });

        }
    }
}