﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF Text value
    /// </summary>
    public class SgfTextValue : SgfSimplePropertyValueBase<string>
    {
        /// <summary>
        /// Creates a Text value
        /// </summary>
        /// <param name="value"></param>
        public SgfTextValue( string value ) : base( value ) {}

        /// <summary>
        /// Text value
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Text;

        /// <summary>
        /// Parses Text value
        /// </summary>
        /// <returns>Parsed instance of SGF Text value</returns>
        public static ISgfPropertyValue Parse( string value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes SGF Text to SGF
        /// </summary>
        /// <returns>SGF serialized Text value</returns>
        public override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
