using System.Collections;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Введите путь в директорию, в которой хранятся сгенерированные файлы: ");

string dir = Console.ReadLine();
dir = Path.Combine(dir, "");

Console.WriteLine("Введите название изначального файла без расширения: ");

string fileName = Console.ReadLine();
string cur_fileName;
string cur_filepath;

cur_fileName = fileName + "_Reduce";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] reduce_bytes = await File.ReadAllBytesAsync(cur_filepath);
string combinedString = Encoding.UTF8.GetString(reduce_bytes);

int reduce = int.Parse(combinedString.Substring(0, 1));
string extention = combinedString.Substring(1);

Console.WriteLine("Имеются следующие способы востановления: ");
Console.WriteLine("1. Востонавить с учетом потери диска А (Без файлов A1 A2)");
Console.WriteLine("2. Востонавить с учетом потери диска B (Без файлов B1 B2)");
Console.WriteLine("3. Востонавить с учетом потери диска C (Без файлов C1 C2)");
Console.WriteLine("Введите выбранный вариант: ");
int choose = int.Parse(Console.ReadLine());

if (choose == 1)
{

    cur_fileName = fileName + "_A1B1_Parity";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_12_parity = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_A2C1_Parity";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_34_parity = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_B1";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_2 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_B2";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_5 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_C1";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_4 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_C2";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_6 = await File.ReadAllBytesAsync(cur_filepath);

    byte[] data = new byte[data_2.Length * 6 - reduce];
    byte[] par_data = SolveParity(data_12_parity, data_2);
    int j = 0;
    for (int i = 0; i < par_data.Length; i++)
    {
        data[j] = par_data[i];
        j++;
    }
    for (int i = 0; i < data_2.Length; i++)
    {
        data[j] = data_2[i];
        j++;
    }
    par_data = SolveParity(data_34_parity, data_4);
    for (int i = 0; i < par_data.Length; i++)
    {
        data[j] = par_data[i];
        j++;
    }
    for (int i = 0; i < data_4.Length; i++)
    {
        data[j] = data_4[i];
        j++;
    }
    for (int i = 0; i < data_5.Length; i++)
    {
        data[j] = data_5[i];
        j++;
    }
    for (int i = 0; i < data_6.Length - reduce; i++)
    {
        data[j] = data_6[i];
        j++;
    }
    cur_fileName = fileName + "_final_recovered" + extention;
    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data);
} 
else if(choose == 2)
{
    cur_fileName = fileName + "_A1B1_Parity";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_12_parity = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_B2C2_Parity";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_56_parity = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_A1";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_1 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_A2";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_3 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_C1";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_4 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_C2";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_6 = await File.ReadAllBytesAsync(cur_filepath);

    byte[] data = new byte[data_1.Length * 6 - reduce];
    byte[] par_data = SolveParity(data_1, data_12_parity);
    int j = 0;
    for (int i = 0; i < data_1.Length; i++)
    {
        data[j] = data_1[i];
        j++;
    }
    for (int i = 0; i < par_data.Length; i++)
    {
        data[j] = par_data[i];
        j++;
    }
    for (int i = 0; i < data_3.Length; i++)
    {
        data[j] = data_3[i];
        j++;
    }
    for (int i = 0; i < data_4.Length; i++)
    {
        data[j] = data_4[i];
        j++;
    }
    par_data = SolveParity(data_56_parity, data_6);
    for (int i = 0; i < par_data.Length; i++)
    {
        data[j] = par_data[i];
        j++;
    }
    for (int i = 0; i < data_6.Length - reduce; i++)
    {
        data[j] = data_6[i];
        j++;
    }
    cur_fileName = fileName + "_final_recovered" + extention;
    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data);

} else if(choose == 3)
{

    cur_fileName = fileName + "_A2C1_Parity";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_34_parity = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_B2C2_Parity";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_56_parity = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_A1";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_1 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_A2";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_3 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_B1";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_2 = await File.ReadAllBytesAsync(cur_filepath);

    cur_fileName = fileName + "_B2";
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_5 = await File.ReadAllBytesAsync(cur_filepath);

    byte[] data = new byte[data_1.Length * 6 - reduce];
    int j = 0;
    for (int i = 0; i < data_1.Length; i++)
    {
        data[j] = data_1[i];
        j++;
    }
    for (int i = 0; i < data_2.Length; i++)
    {
        data[j] = data_2[i];
        j++;
    }
    byte[] par_data = SolveParity(data_3, data_34_parity);
    for (int i = 0; i < data_3.Length; i++)
    {
        data[j] = data_3[i];
        j++;
    }
    for (int i = 0; i < par_data.Length; i++)
    {
        data[j] = par_data[i];
        j++;
    }
    par_data = SolveParity(data_56_parity, data_5);
    for (int i = 0; i < data_5.Length; i++)
    {
        data[j] = data_5[i];
        j++;
    }
    for (int i = 0; i < par_data.Length - reduce; i++)
    {
        data[j] = par_data[i];
        j++;
    }
    cur_fileName = fileName + "_final_recovered" + extention;
    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data);
    Console.WriteLine("Файл сохранен в следующую директорию: {0}", dir);
}

Console.WriteLine("Вы хотите сравнить хеш-суммы изначального файла и полученного в процессе востановления?: ");
Console.WriteLine("1. Да");
Console.WriteLine("2. Нет");
Console.WriteLine("Введите выбранный вариант: ");

choose = int.Parse(Console.ReadLine());

if (choose == 1)
{
    cur_fileName = fileName + "_final_recovered" + extention;
    cur_filepath = Path.Combine(dir, cur_fileName);
    byte[] data_recovered = await File.ReadAllBytesAsync(cur_filepath);
    Console.WriteLine("Введите путь к изначальному файлу: ");
    string filePath = Console.ReadLine();
    byte[] fileContent = await File.ReadAllBytesAsync(filePath);

    Console.WriteLine("Результат проверки полученного из восстановленных частей файла и изначального файла: ");

    Console.WriteLine();

    string hashOriginal = GetSha256(fileContent);
    string hashRestored = GetSha256(data_recovered);

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
    Console.ReadLine();
}
else if (choose == 2)
{
    Console.WriteLine("Работа программы завершена.");
    Console.ReadLine();
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
