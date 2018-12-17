using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionTool.Model.CRYPTON
{
    interface IKeyExpander
    {
        byte[] Expand(byte[] sourceKey);
    }
}
