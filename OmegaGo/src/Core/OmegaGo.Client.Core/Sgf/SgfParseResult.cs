using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents the result of SGF parsing
    /// Informs the caller about possible warnings during the parsing
    /// </summary>
    internal class SgfParseResult<T>
    {
        /// <summary>
        /// Creates the parse result
        /// </summary>
        /// <param name="status">Status of parsing</param>
        /// <param name="parsedResult">The parsed part of the tree</param>
        public SgfParseResult( SgfParseStatus status, T parsedResult )
        {
            if ( parsedResult == null ) throw new ArgumentNullException( nameof( parsedResult ) );
            Status = status;
            Result = parsedResult;
        }

        /// <summary>
        /// Status of the parsing
        /// </summary>
        public SgfParseStatus Status { get; }

        /// <summary>
        /// Root of the parsed SGF tree
        /// </summary>
        public T Result { get; }
    }

    internal class SgfParseResult
    {
        /// <summary>
        /// Performs an union of child parsing results and sets the "worst" outcome as the root result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parsedResult"></param>
        /// <param name="childResults"></param>
        /// <returns></returns>
        public static SgfParseResult<TResult> Union<TResult, TChild>( TResult parsedResult, params SgfParseResult<TChild>[] childResults )
        {
            SgfParseStatus status = SgfParseStatus.Success;
            foreach ( var result in childResults )
            {
                if ( result.Status > status )
                {
                    status = result.Status;
                }
            }
            return new SgfParseResult<TResult>( status, parsedResult );
        }
    }
}
