using System;
using Cucumber.SimpleDb.Session;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Provides a factory for standard ISimpleDbContext implementations.
    /// </summary>
    public sealed class SimpleDbContext
    {
        /// <summary>
        /// Creates a new <c>Cucumber.SimpleDb.ISimpleDbContext</c> instance with the default <c>Cucumber.SimpleDb.ISimpleDbService</c> configuration.
        /// </summary>
        /// <see cref="Cucumber.SimpleDb.ISimpleDbContext"/>
        /// <see cref="Cucumber.SimpleDb.ISimpleDbService"/>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="publicKey"/> or <paramref name="privateKey"/> is null or empty.</exception>
        /// <param name="publicKey">The AWS public key to use.</param>
        /// <param name="privateKey">The AWS private key to use.</param>
        /// <returns>The <c>Cucumber.SimpleDb.ISimpleDbContext</c> instance.</returns>
        public static ISimpleDbContext Create(string publicKey, string privateKey)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                throw new ArgumentNullException("publicKey");
            }
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new ArgumentNullException("privateKey");
            }
            return Create(new SimpleDbRestService(new AwsRestService(publicKey, privateKey, new WebRequestProvider())));
        }

        /// <summary>
        /// Creates a new <c>Cucumber.SimpleDb.ISimpleDbContext</c> instance with the specified <c>Cucumber.SimpleDb.ISimpleDbService</c> configuration.
        /// </summary>
        /// <see cref="Cucumber.SimpleDb.ISimpleDbContext"/>
        /// <see cref="Cucumber.SimpleDb.ISimpleDbService"/>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="service"/> is null.</exception>
        /// <param name="service">The <c>Cucumber.SimpleDb.ISimpleDbService</c> implementation to use.</param>
        /// <returns>The <c>Cucumber.SimpleDb.ISimpleDbContext</c> instance</returns>
        public static ISimpleDbContext Create(ISimpleDbService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            return new SessionSimpleDbContext(service);
        }
    }
}