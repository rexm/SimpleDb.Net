using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Exposes statistics about interacting with SimpleDb
    /// See: http://docs.aws.amazon.com/AmazonSimpleDB/latest/DeveloperGuide/SDB_API_CommonResponseElements.html
    /// </summary>
	public interface ISimpleDbStatistics
	{
        /// <summary>
        /// Gets the box usage for the most recently performed operation in the current context.
        /// </summary>
        decimal LastOperationUsage { get; }

        /// <summary>
        /// Gets the total box usage for all operations performed in the current context.
        /// </summary>
        decimal TotalContextUsage { get; }

        /// <summary>
        /// Gets the ID of the most recently performed operation in the current context.
        /// </summary>
        string LastOperationId { get; }

        /// <summary>
        /// Gets the count of SimpleDB operations performed in the current context.
        /// </summary>
        int OperationCount { get; }
	}
}
