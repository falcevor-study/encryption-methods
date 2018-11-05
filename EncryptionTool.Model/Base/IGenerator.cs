namespace EncryptionTool.Model
{
    public interface IGenerator
    {
        byte GetValue();
        void MoveNext();
        bool IsShift();
    }
}
