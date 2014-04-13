// Portions copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See MicrosoftOpenTechnologiesLicense.txt in the project root for license information.
// This class has been modified from the original version at http://goo.gl/zxTLwD.

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Async.Linq
{
    /// <summary>
    /// Defines methods to create and asynchronously execute queries that are described by an
    /// <see cref="IQueryable" /> object.
    /// This interface is used to interact with Entity Framework queries and shouldn't be implemented by custom classes.
    /// </summary>
    public interface IAsyncQueryProvider : IQueryProvider
    {
        /// <summary>
        /// Asynchronously executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression"> An expression tree that represents a LINQ query. </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the value that results from executing the specified query.
        /// </returns>
        Task<TResult> ExecuteScalarAsync<TResult>(Expression expression);

        /// <summary>
        /// Asynchronously executes the strongly-typed query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TResult"> The type of the value that results from executing the query. </typeparam>
        /// <param name="expression"> An expression tree that represents a LINQ query. </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the value that results from executing the specified query.
        /// </returns>
        Task<IEnumerable<TResult>> ExecuteAsync<TResult>(Expression expression);
    }
}