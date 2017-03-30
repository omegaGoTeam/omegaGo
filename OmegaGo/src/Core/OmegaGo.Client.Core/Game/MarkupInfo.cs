﻿using OmegaGo.Core.Game.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game
{
    public sealed class MarkupInfo
    {
        private Dictionary<Type, System.Collections.IList> _markups; 
        private int _count;
        
        public MarkupInfo()
        {
            _markups = new Dictionary<Type, System.Collections.IList>();
            _count = 0;
        }

        public bool IsEmpty => _count == 0;

        public void AddMarkup<T>(T markup) where T : IMarkup
        {
            if (!_markups.ContainsKey(typeof(T)))
                _markups[typeof(T)] = new List<T>();

            _markups[typeof(T)].Add(markup);
            _count++;
        }

        public void RemoveMarkup<T>(T markup) where T : IMarkup
        {
            if (!_markups.ContainsKey(typeof(T)))
                throw new ArgumentException("Provided markup does not exist.");

            _markups[typeof(T)].Remove(markup);
            _count--;
        }

        public IEnumerable<T> GetMarkups<T>() where T : IMarkup
        {
            if (!_markups.ContainsKey(typeof(T)))
                return Enumerable.Empty<T>();

            // Should we deal with it being castable to List and then modifiable?
            return (IEnumerable<T>)_markups[typeof(T)];
        }
    }
}
