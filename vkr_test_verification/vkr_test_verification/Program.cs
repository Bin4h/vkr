using System.Security.Cryptography;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Введите путь к директории, в которой хранятся сгенерированные файлы: ");

string dir = Console.ReadLine();
dir = Path.Combine(dir, "");

Console.WriteLine("Введите путь к изначальному файлу: ");
string filePath = Console.ReadLine();
string fileName = Path.GetFileNameWithoutExtension(filePath);
byte[] fileContent = await File.ReadAllBytesAsync(filePath);

string cur_fileName = fileName + "_12_Parity";
string cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_12_parity = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_23_Parity";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_23_parity = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_13_Parity";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_13_parity = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_1";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_11 = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_2";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_22 = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_3";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_33 = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_final_recovered";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_recovered = await File.ReadAllBytesAsync(cur_filepath);

Console.WriteLine("Результат проверки 1 разделенной части и восстановленной при помощи массива чётности: ");
Console.WriteLine();
byte[] data_1_recovered = SolveParity(data_12_parity, data_22);

string hashOriginal = GetSha256(data_11);
string hashRestored = GetSha256(data_1_recovered);

Console.WriteLine(hashOriginal);
Console.WriteLine(hashRestored);
Console.WriteLine();

if (hashOriginal == hashRestored)
{
    Console.WriteLine("Изначальный и восстановленный Hash совпадает");
}
else
{
    Console.WriteLine("Изначальный и восстановленный Hash не совпадает");
}
Console.WriteLine("Результат проверки 2 разделенной части и восстановленной при помощи массива чётности: ");
Console.WriteLine();
byte[] data_2_recovered = SolveParity(data_23_parity, data_33);

hashOriginal = GetSha256(data_22);
hashRestored = GetSha256(data_2_recovered);

Console.WriteLine(hashOriginal);
Console.WriteLine(hashRestored);
Console.WriteLine();

if (hashOriginal == hashRestored)
{
    Console.WriteLine("Изначальный и восстановленный Hash совпадает");
}
else
{
    Console.WriteLine("Изначальный и восстановленный Hash не совпадает");
}
Console.WriteLine("Результат проверки 3 разделенной части и восстановленной при помощи массива чётности: ");
Console.WriteLine();
byte[] data_3_recovered = SolveParity(data_13_parity, data_11);

hashOriginal = GetSha256(data_33);
hashRestored = GetSha256(data_3_recovered);

Console.WriteLine(hashOriginal);
Console.WriteLine(hashRestored);
Console.WriteLine();

if (hashOriginal == hashRestored)
{
    Console.WriteLine("Изначальный и восстановленный Hash совпадает");
}
else
{
    Console.WriteLine("Изначальный и восстановленный Hash не совпадает");
}

Console.WriteLine("Результат проверки полученного из восстановленных частей файла и изначального файла: ");

Console.WriteLine();

hashOriginal = GetSha256(fileContent);
hashRestored = GetSha256(data_recovered);

Console.WriteLine(hashOriginal);
Console.WriteLine(hashRestored);
Console.WriteLine();

cur_fileName = fileName + "_final_recovered";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_recovered);

if (hashOriginal == hashRestored)
{
    Console.WriteLine("Изначальный и восстановленный Hash совпадает");
}
else
{
    Console.WriteLine("Изначальный и восстановленный Hash не совпадает");
}
static string GetSha256(byte[] data)
{
    using var sha256 = SHA256.Create();
    var hash = sha256.ComputeHash(data);
    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
}

byte[] SolveParity(byte[] data_1, byte[] data_2)
{
    byte[] parity = new byte[data_1.Length];
    for (int i = 0; i < parity.Length; i++)
        parity[i] = (byte)(data_1[i] ^ data_2[i]);
    return parity;
}
