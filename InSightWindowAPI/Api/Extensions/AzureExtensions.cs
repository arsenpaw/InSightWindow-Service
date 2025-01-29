using Application.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace InSightWindowAPI.Extensions
{
    public static class AzureExtensions
    {

        public static void AddAzureCredentialsFromAppsettingToEnv(this IConfiguration configuration)
        {
            var identityCredentials = new AzureIdentityConfig();
            configuration.GetSection("AzureIdentity").Bind(identityCredentials);
            
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", identityCredentials.TenantId);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", identityCredentials.ClientId);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", identityCredentials.ClientSecret);
        }
        
        /// <summary>
        /// DefaultAzureCredentialOptions Explicit use env variable as credential to Azure. So before call this method, you need to set the env variable 
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="vaultConfig">V</param>
        public static void AddVaultCertificate(this ConfigureWebHostBuilder opt, KeyVault vaultConfig)
        {

           // var credential = new ClientSecretCredential(vaultConfig.TenantId, vaultConfig.ClientId, vaultConfig.ClientSecret);
           var options = new DefaultAzureCredentialOptions
           {
               ExcludeEnvironmentCredential = false,
               ExcludeManagedIdentityCredential = true,
               ExcludeVisualStudioCredential = true,
               ExcludeVisualStudioCodeCredential = false,
               ExcludeAzureCliCredential = true,
           };
            var credential = new DefaultAzureCredential(options);
            
            var certificateClient = new CertificateClient(new Uri(vaultConfig.Url), credential);
            var secretClient = new SecretClient(new Uri(vaultConfig.Url), credential);

            // Retrieve the certificate (public part)
            var certificateResponse = certificateClient.GetCertificate(vaultConfig.SslCertificateName);

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