using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHA.Helpers.ImageDataSourceTranslator
{
    public interface IImageDataSourceTranslator
    {
        string GetDataSourceTranslation(string imageExtension, byte[] imageBytes);
    }
}
