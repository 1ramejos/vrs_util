using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Text;

namespace vrsutils
{
    public class RndmKeyGenerator
    {
        private string Key_Letters;

        private string Key_Numbers;

        private int Key_Chars;

        private char[] LettersArray;

        private char[] NumbersArray;

        protected internal string KeyLetters
        {
            set
            {
                Key_Letters = value;
            }
        }

        protected internal string KeyNumbers
        {
            set
            {
                Key_Numbers = value;
            }
        }

        protected internal int KeyChars
        {
            set
            {
                Key_Chars = value;
            }
        }

        [DebuggerNonUserCode]
        public RndmKeyGenerator()
        {
        }

        public string Generate()
        {
            StringBuilder stringBuilder = new StringBuilder();
            LettersArray = Key_Letters.ToCharArray();
            NumbersArray = Key_Numbers.ToCharArray();
            int key_Chars = Key_Chars;
            int num = 1;
            while (true)
            {
                int num2 = num;
                int num3 = key_Chars;
                if (num2 > num3)
                {
                    break;
                }
                VBMath.Randomize();
                float num4 = VBMath.Rnd();
                short num5 = -1;
                if (checked((int)Math.Round(num4 * 111f)) % 2 == 0)
                {
                    while (num5 < 0)
                    {
                        num5 = Convert.ToInt16((float)LettersArray.GetUpperBound(0) * num4);
                    }
                    string value = LettersArray[num5].ToString();
                    if (checked((int)Math.Round((float)num5 * num4 * 99f)) % 2 != 0)
                    {
                        value = LettersArray[num5].ToString();
                        value = value.ToUpper();
                    }
                    stringBuilder.Append(value);
                }
                else
                {
                    while (num5 < 0)
                    {
                        num5 = Convert.ToInt16((float)NumbersArray.GetUpperBound(0) * num4);
                    }
                    stringBuilder.Append(NumbersArray[num5]);
                }
                num = checked(num + 1);
            }
            return stringBuilder.ToString();
        }
    }
}