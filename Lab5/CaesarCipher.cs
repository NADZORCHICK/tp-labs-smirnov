using System;
using System.Text;
public class CaesarCipher
{
    private const string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

    public const int AlphabetSize = 33;

    // Сдвигает каждую букву текста на shift позиций.
    private string Shift(string text, int shift)
    {
        StringBuilder result = new StringBuilder(text.Length);

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            int index = Alphabet.IndexOf(char.ToLower(c));

            if (index == -1)
            {
                // Это не русская буква — оставляем как есть.
                result.Append(c);
            }
            else
            {
                int newIndex = (index + shift) % AlphabetSize;
                if (newIndex < 0)
                {
                    // Для отрицательного сдвига остаток может получиться отрицательным.
                    newIndex = newIndex + AlphabetSize;
                }

                char newChar = Alphabet[newIndex];
                if (char.IsUpper(c))
                {
                    newChar = char.ToUpper(newChar);
                }
                result.Append(newChar);
            }
        }

        return result.ToString();
    }

    public string Encrypt(string text, int key)
    {
        return Shift(text, key);
    }

    // Расшифровка(сдвиг в обратную сторону).
    public string Decrypt(string text, int key)
    {
        return Shift(text, -key);
    }
}