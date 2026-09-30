using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Hezium.Memory;

public sealed partial class BigArray<T>
{
    internal struct Enumerator : IEnumerator<T>
    {
        private readonly BigArray<T> _array;
        private nint _offset;

        internal Enumerator(BigArray<T> array)
        {
            _array = array;
            _offset = -1;
        }

        public readonly T Current => _array[_offset];

        readonly object? IEnumerator.Current => Current;

        public readonly void Dispose() { }

        public bool MoveNext()
        {
            if (_offset < _array._length - 1)
            {
                _offset++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _offset = -1;
        }
    }
}
