
using System.Security.Cryptography;
using System.Text;

Console.OutputEncoding = System.Text.Encoding.UTF8;
// Получение папки с хранимыми частями

string cur_fileName;
string cur_filepath;
string response1 = "2";
string response2 = "2";
string response3 = "2";
string file1Path = "";
string file2Path = "";
string file3Path = "";
Console.WriteLine("Есть ли доступ к 1 файлу?(1.Да 2.Нет)");
response1 = Console.ReadLine();
if (response1 == "1")
{
    Console.WriteLine("Введите путь к файлу 1");
    file1Path = Console.ReadLine();
}
Console.WriteLine("Есть ли доступ к 2 файлу?(1.Да 2.Нет)");
response2 = Console.ReadLine();
if (response2 == "1")
{
    Console.WriteLine("Введите путь к файлу 2");
    file2Path = Console.ReadLine();
}
Console.WriteLine("Есть ли доступ к 3 файлу?(1.Да 2.Нет)");
response3 = Console.ReadLine();
if (response3 == "1")
{
    Console.WriteLine("Введите путь к файлу 3");
    file3Path = Console.ReadLine();
}

// Проверка файлов на отсутствие
if (response1 == "2")
{
    // Завершение программы если не хватает больше одного файла
    if (response2 == "2" || response3 == "2")
    {
        Console.WriteLine("Больше одного файла утеряно");
        Console.ReadLine();
        Environment.Exit(0);
    }
    Console.WriteLine("Утерян диск 1, производится восстановление");
    //Подгрузка данных

    byte[] data_2_final = await File.ReadAllBytesAsync(file2Path);

    byte[] data_3_final = await File.ReadAllBytesAsync(file3Path);

    byte[] reduce_bytes = new byte[45];
    int j = 0;
    for(int i = data_2_final.Length - 45; i < data_2_final.Length; i++)
    {
        reduce_bytes[j] = data_2_final[i];
        j++;
    }
    string combinedString = Encoding.UTF8.GetString(reduce_bytes).Replace("\0", "");
    string[] parts = combinedString.Split('!');
    int part_size_a = int.Parse(parts[0]);
    int part_size_b = int.Parse(parts[1]);
    int part_size_c = int.Parse(parts[2]);
    string extention = parts[3];
    int reduce = int.Parse(parts[4]);

    // Собрка файла с восстановлнием
    byte[] data = new byte[(part_size_a + part_size_b + part_size_c - 45) * 2 - reduce];
    j = 0;
    byte[] data_2 = new byte[part_size_a];
    for (int i = 0; i < data_2.Length; i++)
    {
        data_2[i] = data_2_final[j];
        j++;
    }
    byte[] data_5 = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_5[i] = data_2_final[j];
        j++;
    }
    byte[] data_34_parity = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_34_parity[i] = data_2_final[j];
        j++;
    }

    j = 0;
    byte[] data_4 = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_4[i] = data_3_final[j];
        j++;
    }
    byte[] data_6 = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_6[i] = data_3_final[j];
        j++;
    }
    byte[] data_12_parity = new byte[part_size_a];
    for (int i = 0; i < part_size_a; i++)
    {
        data_12_parity[i] = data_3_final[j];
        j++;
    }

    byte[] data_1 = SolveParity(data_2, data_12_parity);
    byte[] data_3 = SolveParity(data_4, data_34_parity);

    j = 0;
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

    Console.WriteLine("Введите название файла для сохранения");
    cur_fileName = Console.ReadLine() + extention;
    Console.WriteLine("Введите путь для сохранения файла");
    string path = Console.ReadLine();
    await File.WriteAllBytesAsync(Path.Combine(path, cur_fileName), data);
    Console.WriteLine("Файл собран с восстановления");
} 
else if(response2 == "2")
{
    // Завершение программы если не хватает больше одного файла
    if (response1 == "2" || response3 == "2")
    {
        Console.WriteLine("Больше одного файла утеряно");
        Console.ReadLine();
        Environment.Exit(0);
    }
    Console.WriteLine("Утерян диск _2, производится восстановление");
    // Подгрузка данных
    byte[] data_1_final = await File.ReadAllBytesAsync(file1Path);

    byte[] data_3_final = await File.ReadAllBytesAsync(file3Path);

    byte[] reduce_bytes = new byte[45];
    int j = 0;
    for (int i = data_1_final.Length - 45; i < data_1_final.Length; i++)
    {
        reduce_bytes[j] = data_1_final[i];
        j++;
    }
    string combinedString = Encoding.UTF8.GetString(reduce_bytes).Replace("\0", "");
    string[] parts = combinedString.Split('!');
    int part_size_a = int.Parse(parts[0]);
    int part_size_b = int.Parse(parts[1]);
    int part_size_c = int.Parse(parts[2]);
    string extention = parts[3];
    int reduce = int.Parse(parts[4]);

    // Восстановление файла
    byte[] data = new byte[(part_size_a + part_size_b + part_size_c - 45) * 2 - reduce];
    j = 0;
    byte[] data_1 = new byte[part_size_a];
    for (int i = 0; i < part_size_a; i++)
    {
        data_1[i] = data_1_final[j];
        j++;
    }
    byte[] data_3 = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_3[i] = data_1_final[j];
        j++;
    }
    byte[] data_56_parity = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_56_parity[i] = data_1_final[j];
        j++;
    }

    j = 0;
    byte[] data_4 = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_4[i] = data_3_final[j];
        j++;
    }
    byte[] data_6 = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_6[i] = data_3_final[j];
        j++;
    }
    byte[] data_12_parity = new byte[part_size_a];
    for (int i = 0; i < part_size_a; i++)
    {
        data_12_parity[i] = data_3_final[j];
        j++;
    }

    byte[] data_2 = SolveParity(data_1, data_12_parity);
    byte[] data_5 = SolveParity(data_6, data_56_parity);

    j = 0;
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
    Console.WriteLine("Введите название файла для сохранения");
    cur_fileName = Console.ReadLine() + extention;
    Console.WriteLine("Введите путь для сохранения файла");
    string path = Console.ReadLine();
    await File.WriteAllBytesAsync(Path.Combine(path, cur_fileName), data);
    Console.WriteLine("Файл собран без восстановления");

} else if(response3 == "2")
{
    // Завершение программы если не хватает больше одного файла
    if (response1 == "2" || response2 == "2")
    {
        Console.WriteLine("Больше одного файла утеряно");
        Console.ReadLine();
        Environment.Exit(0);
    }
    Console.WriteLine("Утерян диск _3, производится восстановление");
    // Подгрузка данных
    byte[] data_1_final = await File.ReadAllBytesAsync(file1Path);

    byte[] data_2_final = await File.ReadAllBytesAsync(file2Path);

    byte[] reduce_bytes = new byte[45];
    int j = 0;
    for (int i = data_1_final.Length - 45; i < data_1_final.Length; i++)
    {
        reduce_bytes[j] = data_1_final[i];
        j++;
    }
    string combinedString = Encoding.UTF8.GetString(reduce_bytes).Replace("\0", "");
    string[] parts = combinedString.Split('!');
    int part_size_a = int.Parse(parts[0]);
    int part_size_b = int.Parse(parts[1]);
    int part_size_c = int.Parse(parts[2]);
    string extention = parts[3];
    int reduce = int.Parse(parts[4]);

    // Восстановление файла
    byte[] data = new byte[(part_size_a + part_size_b + part_size_c - 45) * 2 - reduce];
    j = 0;
    byte[] data_1 = new byte[part_size_a];
    for (int i = 0; i < part_size_a; i++)
    {
        data_1[i] = data_1_final[j];
        j++;
    }
    byte[] data_3 = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_3[i] = data_1_final[j];
        j++;
    }
    byte[] data_56_parity = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_56_parity[i] = data_1_final[j];
        j++;
    }

    j = 0;
    byte[] data_2 = new byte[part_size_a];
    for (int i = 0; i < part_size_a; i++)
    {
        data_2[i] = data_2_final[j];
        j++;
    }
    byte[] data_5 = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_5[i] = data_2_final[j];
        j++;
    }
    byte[] data_34_parity = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_34_parity[i] = data_2_final[j];
        j++;
    }

    byte[] data_4 = SolveParity(data_3, data_34_parity);
    byte[] data_6 = SolveParity(data_5, data_56_parity);

    j = 0;
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
    Console.WriteLine("Введите название файла для сохранения");
    cur_fileName = Console.ReadLine() + extention;
    Console.WriteLine("Введите путь для сохранения файла");
    string path = Console.ReadLine();
    await File.WriteAllBytesAsync(Path.Combine(path, cur_fileName), data);
    Console.WriteLine("Файл собран без восстановления");

} else if (response1 == "1" && response2 == "1" && response3 == "1")
{
    Console.WriteLine("Все файлы на месте, производится сборка ");
    // Подгрузка данных
    byte[] data_1_final = await File.ReadAllBytesAsync(file1Path);

    byte[] data_2_final = await File.ReadAllBytesAsync(file2Path);

    byte[] data_3_final = await File.ReadAllBytesAsync(file3Path);

    byte[] reduce_bytes = new byte[45];
    int j = 0;
    for (int i = data_1_final.Length - 45; i < data_1_final.Length; i++)
    {
        reduce_bytes[j] = data_1_final[i];
        j++;
    }
    string combinedString = Encoding.UTF8.GetString(reduce_bytes).Replace("\0", "");
    string[] parts = combinedString.Split('!');
    int part_size_a = int.Parse(parts[0]);
    int part_size_b = int.Parse(parts[1]);
    int part_size_c = int.Parse(parts[2]);
    string extention = parts[3];
    int reduce = int.Parse(parts[4]);

    byte[] data = new byte[(part_size_a + part_size_b + part_size_c - 45) * 2 - reduce];
    // Сборка файла из частей
    j = 0;
    byte[] data_1 = new byte[part_size_a];
    for (int i = 0; i < part_size_a; i++)
    {
        data_1[i] = data_1_final[j];
        j++;
    }
    byte[] data_3 = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_3[i] = data_1_final[j];
        j++;
    }
    j = 0;
    byte[] data_2 = new byte[part_size_a];
    for (int i = 0; i < part_size_a; i++)
    {
        data_2[i] = data_2_final[j];
        j++;
    }
    byte[] data_5 = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_5[i] = data_2_final[j];
        j++;
    }
    j = 0;
    byte[] data_4 = new byte[part_size_b];
    for (int i = 0; i < part_size_b; i++)
    {
        data_4[i] = data_3_final[j];
        j++;
    }
    byte[] data_6 = new byte[part_size_c - 45];
    for (int i = 0; i < part_size_c - 45; i++)
    {
        data_6[i] = data_3_final[j];
        j++;
    }
    j = 0;

    for(int i = 0; i < data_1.Length; i++)
    {
        data[j] = data_1[i];
        j++;
    }
    for (int i = 0; i < data_2.Length; i++)
    {
        data[j] = data_2[i];
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
    Console.WriteLine("Введите название файла для сохранения");
    cur_fileName = Console.ReadLine() + extention;
    Console.WriteLine("Введите путь для сохранения файла");
    string path = Console.ReadLine();
    await File.WriteAllBytesAsync(Path.Combine(path, cur_fileName), data);
    Console.WriteLine("Файл собран без восстановления");
}

Console.WriteLine("Работа программы завершена");
Console.ReadLine();

byte[] SolveParity(byte[] data_1, byte[] data_2)
{
    byte[] parity = new byte[data_1.Length];
    for (int i = 0; i < parity.Length; i++)
        parity[i] = (byte)(data_1[i] ^ data_2[i]);
    return parity;
}
