using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSE.Models
{
    public class TextConverter
    {
        private readonly Func<string, string> _convertion;

        public TextConverter(Func<string, string> convertion)
        {
            _convertion = convertion;
        }

        public string ConvertText(string inputText)
        {
            return _convertion(inputText);
        }
    }
}
