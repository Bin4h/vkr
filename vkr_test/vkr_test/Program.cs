using System.Security.Cryptography;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Введите путь к файлу ");
string filePath = Console.ReadLine();

byte[] fileContent = await File.ReadAllBytesAsync(filePath);
int reduce = 0;
int size = (int)Math.Ceiling((double)fileContent.Length / 3);
if (fileContent.Length % 3 != 0)
{
    reduce = 3 - fileContent.Length % 3;
}
int j = 0;

byte[] data_1 = new byte[size];
for (int i = 0; i < data_1.Length; i++)
{
    data_1[i] = fileContent[j];
    j++;
}
byte[] data_2 = new byte[size];
for (int i = 0; i < data_2.Length; i++)
{
    data_2[i] = fileContent[j];
    j++;
}

byte[] data_3 = new byte[size];

for (int i = 0; i < data_3.Length; i++)
{
    if (j < fileContent.Length)
    {
        data_3[i] = fileContent[j];
        j++;
    }
    else
    {
        data_3[i] = 0;
        j++;
    }
}

byte[] parity = new byte[fileContent.Length / 6];
string dir = Directory.GetCurrentDirectory();

// Подготовка разделенных файлов

string fileName = Path.GetFileNameWithoutExtension(filePath);
string cur_fileName = fileName + "_1";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_1);

cur_fileName = fileName + "_2";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_2);

cur_fileName = fileName + "_3";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_3);

// Подготовка всех массивов чётности

parity = SolveParity(data_1, data_2);

cur_fileName = fileName + "_12_Parity";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

parity = SolveParity(data_2, data_3);

cur_fileName = fileName + "_23_Parity";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

parity = SolveParity(data_1, data_3);

cur_fileName = fileName + "_13_Parity";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

// Подгружаются данные чётности

byte[] data_recovered = new byte[data_1.Length * 3 - reduce];

cur_fileName = fileName + "_12_Parity";

string cur_filepath = Path.Combine(dir, cur_fileName);

byte[] data_12_parity = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_23_Parity";

cur_filepath = Path.Combine(dir, cur_fileName);

byte[] data_23_parity = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_13_Parity";

cur_filepath = Path.Combine(dir, cur_fileName);

byte[] data_13_parity = await File.ReadAllBytesAsync(cur_filepath);

// Подгружаются разделенные данные

cur_fileName = fileName + "_1";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_11 = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_2";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_22 = await File.ReadAllBytesAsync(cur_filepath);

cur_fileName = fileName + "_3";
cur_filepath = Path.Combine(dir, cur_fileName);
byte[] data_33 = await File.ReadAllBytesAsync(cur_filepath);

byte[] data_1_recovered = SolveParity(data_12_parity, data_22);
byte[] data_2_recovered = SolveParity(data_23_parity, data_33);
byte[] data_3_recovered = SolveParity(data_13_parity, data_11);

j = 0;

for (int i = 0; i < data_1_recovered.Length; i++)
{
    data_recovered[j] = data_1_recovered[i];
    j++;
}

for (int i = 0; i < data_2_recovered.Length; i++)
{
    data_recovered[j] = data_2_recovered[i];
    j++;
}

for (int i = 0; i < data_3_recovered.Length - reduce; i++)
{
    data_recovered[j] = data_3_recovered[i];
    j++;
}

cur_fileName = fileName + "_final_recovered";
await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_recovered);

Console.WriteLine("Файлы успешно разделены в виде набора байтов");

byte[] SolveParity(byte[] data_1, byte[] data_2)
{
    byte[] parity = new byte[data_1.Length];
    for (int i = 0; i < parity.Length; i++)
        parity[i] = (byte)(data_1[i] ^ data_2[i]);
    return parity;
}

static string GetSha256(byte[] data)
{
    using var sha256 = SHA256.Create();
    var hash = sha256.ComputeHash(data);
    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
}
