using System.Security.Cryptography;
using System.Text;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Введите путь к файлу ");
string filePath = Console.ReadLine();

byte[] fileContent = await File.ReadAllBytesAsync(filePath);
int reduce = 0;
int size = (int)Math.Ceiling((double)fileContent.Length / 6);

if (fileContent.Length % 6 != 0)
{
    reduce = 6 - fileContent.Length % 6;
}
string extension = Path.GetExtension(filePath);
string combined = reduce.ToString() + extension;
byte[] byteArray = Encoding.UTF8.GetBytes(combined);
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
    data_3[i] = fileContent[j];
    j++;
}

byte[] data_4 = new byte[size];
for (int i = 0; i < data_4.Length; i++)
{
    data_4[i] = fileContent[j];
    j++;
}

byte[] data_5 = new byte[size];
for (int i = 0; i < data_5.Length; i++)
{
    data_5[i] = fileContent[j];
    j++;
}

byte[] data_6 = new byte[size];

for (int i = 0; i < data_6.Length; i++)
{
    if (j < fileContent.Length)
    {
        data_6[i] = fileContent[j];
        j++;
    }
    else
    {
        data_6[i] = 0;
        j++;
    }
}

byte[] parity = new byte[fileContent.Length / 6];
string dir = Directory.GetCurrentDirectory();

// Подготовка разделенных файлов

string fileName = Path.GetFileNameWithoutExtension(filePath);
string cur_fileName = fileName + "_A1";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_1);

cur_fileName = fileName + "_B1";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_2);

cur_fileName = fileName + "_A2";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_3);

cur_fileName = fileName + "_C1";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_4);

cur_fileName = fileName + "_B2";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_5);

cur_fileName = fileName + "_C2";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_6);
// Подготовка всех массивов чётности

parity = SolveParity(data_1, data_2);

cur_fileName = fileName + "_A1B1_Parity";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

parity = SolveParity(data_3, data_4);

cur_fileName = fileName + "_A2C1_Parity";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

parity = SolveParity(data_5, data_6);

cur_fileName = fileName + "_B2C2_Parity";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

cur_fileName = fileName + "_Reduce";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), byteArray);

Console.WriteLine("Файлы успешно представлены в виде набора байтов, они схоранены в виде файлов");
Console.ReadLine();

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
