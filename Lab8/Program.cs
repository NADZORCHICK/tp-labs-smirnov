using System.Diagnostics;
using System.Text;

const int N = 30_000;

var swTotal = Stopwatch.StartNew();

// --- 1. Генерация данных ---
var rnd = new Random(42);      // фиксированное зерно: результат                  
var buyers = new List<string>();
var amounts = new List<int>();
for (int i = 0; i < N; i++)
{
    buyers.Add("user" + rnd.Next(0, 5000));
    amounts.Add(rnd.Next(1, 10_000));
}

// --- 2. Список уникальных покупателей ---
var sw = Stopwatch.StartNew();
var uniqueSet = new HashSet<string>();
var unique = new List<string>();
for (int i = 0; i < buyers.Count; i++)
{
    // Add вернёт true, только если такого покупателя ещё не было
    if (uniqueSet.Add(buyers[i]))
        unique.Add(buyers[i]);
}
Console.WriteLine($"Уникальных покупателей: {unique.Count}  ({sw.ElapsedMilliseconds} мс)");

// --- 3. Текстовый отчёт ---
sw.Restart();
var reportBuilder = new StringBuilder();
for (int i = 0; i < buyers.Count; i++)
{
    reportBuilder.Append(buyers[i]).Append(';').Append(amounts[i]).Append(Environment.NewLine);
}
string report = reportBuilder.ToString();
Console.WriteLine($"Отчёт: {report.Length} символов  ({sw.ElapsedMilliseconds} мс)");

// --- 4. Сортировка сумм ---
sw.Restart();
var sorted = new List<int>(amounts);
sorted.Sort();
Console.WriteLine($"Медианная сумма: {sorted[sorted.Count / 2]}  ({sw.ElapsedMilliseconds} мс)");
// --- 5. Сумма покупок каждого покупателя ---
sw.Restart();
var sums = new Dictionary<string, long>();
for (int i = 0; i < buyers.Count; i++)
{
    long current;
    sums.TryGetValue(buyers[i], out current);   // если покупателя ещё нет, current будет 0
    sums[buyers[i]] = current + amounts[i];
}

var totals = new List<(string Buyer, long Total)>();
foreach (string b in unique)
{
    totals.Add((b, sums[b]));
}
Console.WriteLine($"Максимум потратил: {totals.MaxBy(t => t.Total).Buyer}  ({sw.ElapsedMilliseconds} мс)");
Console.WriteLine($"ИТОГО: {swTotal.ElapsedMilliseconds} мс");