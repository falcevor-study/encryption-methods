using System.Linq;

namespace EncryptionTool.Model
{
    class AdditiveGenerator : IGenerator
    {
        private byte[] _register;

        private int _current = -1;

        private int _a;
        private int _b;
        private int _M;

        private bool _isShift;

        public AdditiveGenerator(byte[] register, int a, int b, int M)
        {
            _register = register.Append((byte)0).ToArray();
            _current = _register.Length - 1;
            _a = a;
            _b = b;
            _M = M;
        }

        public byte GetValue()
        {
            return _register[_current];
        }

        public bool IsShift()
        {
            return _isShift;
        }

        public void MoveNext()
        {
            _current = _current + 1 == _register.Length ? 0 : _current + 1;
            _register[_current] = CalcNextValue();
        }

        private byte CalcNextValue()
        {
            int a = _current - _a < 0? _register.Length + _current - _a : _current - _a;
            int b = _current - _b < 0? _register.Length + _current - _b : _current - _b;
            _isShift = ((_register[a] + _register[b]) / _M) == 1;
            return (byte)((_register[a] + _register[b]) % _M);
        }
    }
}
