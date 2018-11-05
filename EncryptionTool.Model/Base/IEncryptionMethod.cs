namespace EncryptionTool.Model
{
    public interface IEncryptionMethod
    {
        byte[] Encode(byte[] text);

        byte[] Decode(byte[] text);
    }
}
