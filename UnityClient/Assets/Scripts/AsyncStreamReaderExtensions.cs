using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Grpc.Core
{
    /// <summary>
    /// Extension methods for <see cref="IAsyncStreamReader{T}"/>.
    /// </summary>
    public static class AsyncStreamReaderExtensions
    {
        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> that enables reading all of the data from the stream reader.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="streamReader">The stream reader.</param>
        /// <param name="cancellationToken">The cancellation token to use to cancel the enumeration.</param>
        /// <returns>The created async enumerable.</returns>
        public static IAsyncEnumerable<T> ReadAllAsync<T>(this IAsyncStreamReader<T> streamReader, CancellationToken cancellationToken = default)
        {
            if (streamReader == null)
            {
                throw new System.ArgumentNullException(nameof(streamReader));
            }

            return ReadAllAsyncCore(streamReader, cancellationToken);
        }

        private static async IAsyncEnumerable<T> ReadAllAsyncCore<T>(IAsyncStreamReader<T> streamReader, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (await streamReader.MoveNext(cancellationToken).ConfigureAwait(false))
            {
                yield return streamReader.Current;
            }
        }
    }
}
