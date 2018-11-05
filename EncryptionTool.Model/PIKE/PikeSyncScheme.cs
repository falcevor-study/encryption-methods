namespace EncryptionTool.Model
{
    class PikeSyncScheme : ISyncScheme
    {
        private IGenerator _gen1;
        private IGenerator _gen2;
        private IGenerator _gen3;

        public PikeSyncScheme(IGenerator gen1, IGenerator gen2, IGenerator gen3)
        {
            _gen1 = gen1;
            _gen2 = gen2;
            _gen3 = gen3;
        }

        public void ShiftGenerators()
        {
            if (_gen1.IsShift() == _gen2.IsShift() && _gen2.IsShift() == _gen3.IsShift())
            {
                _gen1.MoveNext();
                _gen2.MoveNext();
                _gen3.MoveNext();
                return;
            }

            if (_gen1.IsShift() == _gen2.IsShift())
            {
                _gen1.MoveNext();
                _gen2.MoveNext();
                return;
            }

            if (_gen1.IsShift() == _gen3.IsShift())
            {
                _gen1.MoveNext();
                _gen3.MoveNext();
                return;
            }

            if (_gen2.IsShift() == _gen3.IsShift())
            {
                _gen2.MoveNext();
                _gen3.MoveNext();
                return;
            }
        }
    }
}
