namespace Lab4.Core;
public static class MathAlgorithms
{
    public static long Factorial(int n)
    {
        if (n < 0 || n > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n должно быть от 0 до 20");
        }

        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result = result * i;
        }
        return result;
    }

    public static List<long> Fibonacci(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "n не может быть отрицательным");
        }

        List<long> result = new List<long>();
        long a = 0;
        long b = 1;

        while (a <= n)
        {
            result.Add(a);
            long next = a + b;
            a = b;
            b = next;
        }

        return result;
    }

    // Вариант 4: A = sqrt(x - 54) + cos(x/2) / sin(x^2) - ln(x)
    public static double FunctionA(double x)
    {
        if (x < 54)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "x должен быть не меньше 54");
        }

        double sinX2 = Math.Sin(x * x);

        if (Math.Abs(sinX2) < 1e-12)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "sin(x^2) = 0, деление на ноль");
        }

        return Math.Sqrt(x - 54) + Math.Cos(x / 2) / sinX2 - Math.Log(x);
    }

    // Вариант 4: ln(1+x) = x - x^2/2 + x^3/3 - ...  для -1 < x <= 1.
    public static double LnTaylor(double x, double eps = 1e-9)
    {
        if (x <= -1 || x > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "x должен быть в интервале (-1; 1]");
        }

        // При eps <= 0 цикл станет бесконечным.
        if (eps <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eps), "eps должно быть больше нуля");
        }

        double sum = 0;
        int n = 1; // номер текущего члена ряда

        while (true)
        {
            // Знак чередуется: +, -, +, -, и тд
            double sign;
            if (n % 2 == 1)
            {
                sign = 1;
            }
            else
            {
                sign = -1;
            }

            double term = sign * Math.Pow(x, n) / n;

            if (Math.Abs(term) <= eps)
            {
                break;
            }

            sum = sum + term;
            n = n + 1;
        }

        return sum;
    }
}