using System.Security.Cryptography;
using System.Text;
Console.OutputEncoding = System.Text.Encoding.UTF8;
// Запрос пути к файлу
Console.WriteLine("Введите путь к файлу ");
string filePath = Console.ReadLine();
// Считывание файла в виде набора байт
byte[] fileContent = await File.ReadAllBytesAsync(filePath);
int reduce = 0;

Console.WriteLine("Введите делитель для первого среза (например что бы он занимал 1/5 введите 5)");

int part_portional_a = int.Parse(Console.ReadLine());

Console.WriteLine("Введите делитель для второго среза (например что бы он занимал 1/5 введите 5)");

int part_portional_b = int.Parse(Console.ReadLine());

if(1.0 / part_portional_a + 1.0 / part_portional_b >= 1)
{
    Console.WriteLine("Сумма частей превышает размер файла");
    Environment.Exit(0);
}

int part_size_a = fileContent.Length / part_portional_a;
int part_size_b = fileContent.Length / part_portional_b;
int part_size_c = 18;

if (part_size_a + part_size_b < fileContent.Length)
{
    part_size_c += (int)Math.Ceiling((fileContent.Length - (part_size_a + part_size_b) * 2) / 2.0);
}

if (fileContent.Length % 2 != 0)
{
    reduce = 1;
}

string extension = Path.GetExtension(filePath);
string combined = part_size_a.ToString() + "!" + part_size_b.ToString() + "!" + part_size_c.ToString() + "!" + reduce;
// Сохранение информации в виде набора байт
byte[] byteArray_1 = Encoding.UTF8.GetBytes(combined);
byte[] byteArray = new byte[18];
for(int i = 0; i <  byteArray.Length; i++)
{
    if(i <  byteArray_1.Length)
        byteArray[i] = byteArray_1[i];
    else byteArray[i] = 0;
}
int j = 0;
// Представление файла в виде массивов байт
byte[] data_1 = new byte[part_size_a];
for (int i = 0; i < data_1.Length; i++)
{
    if (j < fileContent.Length)
    {
        data_1[i] = fileContent[j];
        j++;
    }
    else
    {
        data_1[i] = 0;
        j++;
    }
}

byte[] data_2 = new byte[part_size_a];
for (int i = 0; i < data_2.Length; i++)
{
    if (j < fileContent.Length)
    {
        data_2[i] = fileContent[j];
        j++;
    }
    else
    {
        data_2[i] = 0;
        j++;
    }
}

byte[] data_3 = new byte[part_size_b];
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

byte[] data_4 = new byte[part_size_b];
for (int i = 0; i < data_4.Length; i++)
{
    if (j < fileContent.Length)
    {
        data_4[i] = fileContent[j];
        j++;
    }
    else
    {
        data_4[i] = 0;
        j++;
    }
}

byte[] data_5 = new byte[part_size_c - 18];
for (int i = 0; i < data_5.Length; i++)
{
    if (j < fileContent.Length)
    {
        data_5[i] = fileContent[j];
        j++;
    }
    else
    {
        data_5[i] = 0;
        j++;
    }
}

byte[] data_6 = new byte[part_size_c - 18];

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
// Расчет массивов четности
// И заполнение итоговых файлов

byte[] data_1_final = new byte[part_size_a + part_size_b + part_size_c];
j = 0;

for (int i = 0; i < data_1.Length; i++)
{
    data_1_final[j] = data_1[i];
    j++;
}

for (int i = 0; i < data_3.Length; i++)
{
    data_1_final[j] = data_3[i];
    j++;
}

byte[] data_56_parity = SolveParity(data_5, data_6);

for (int i = 0; i < data_56_parity.Length; i++)
{
    data_1_final[j] = data_56_parity[i];
    j++;
}

byte[] data_sypher_1 = new byte[byteArray.Length];
j -= 18;
for (int i = 0; i < byteArray.Length; i++)
{
    if (i < byteArray_1.Length)
    {
        data_sypher_1[i] = data_1_final[j];
        j++;
    }
    else
    {
        data_sypher_1[i] = 0;
        j++;
    }
}
if (byteArray.Length < 18)
    j += 18 - byteArray.Length;
byte[] data_syphered_1 = SolveParity(byteArray, data_sypher_1);

for (int i = 0; i < 18; i++)
{
    if (i < data_syphered_1.Length)
    {
        data_1_final[j] = data_syphered_1[i];
        j++;
    }
    else
    {
        data_1_final[j] = 0;
        j++;
    }
}

byte[] data_2_final = new byte[part_size_a + part_size_b + part_size_c];
j = 0;

for (int i = 0; i < data_2.Length; i++)
{
    data_2_final[j] = data_2[i];
    j++;
}

byte[] data_34_parity = SolveParity(data_3, data_4);

for (int i = 0; i < data_34_parity.Length; i++)
{
    data_2_final[j] = data_34_parity[i];
    j++;
}

for (int i = 0; i < data_5.Length; i++)
{
    data_2_final[j] = data_5[i];
    j++;
}

byte[] data_sypher_2 = new byte[byteArray.Length];
j -= 18;
for (int i = 0; i < byteArray.Length; i++)
{
    if (i < byteArray_1.Length)
    {
        data_sypher_2[i] = data_2_final[j];
        j++;
    }
    else
    {
        data_sypher_2[i] = 0;
        j++;
    }
}
if (byteArray.Length < 18)
    j += 18 - byteArray.Length;
byte[] data_syphered_2 = SolveParity(byteArray, data_sypher_2);

for (int i = 0; i < 18; i++)
{
    if (i < data_syphered_2.Length)
    {
        data_2_final[j] = data_syphered_2[i];
        j++;
    }
    else
    {
        data_2_final[j] = 0;
        j++;
    }
}

byte[] data_3_final = new byte[part_size_a + part_size_b + part_size_c];
j = 0;

byte[] data_12_parity = SolveParity(data_1, data_2);

for (int i = 0; i < data_12_parity.Length; i++)
{
    data_3_final[j] = data_12_parity[i];
    j++;
}

for (int i = 0; i < data_4.Length; i++)
{
    data_3_final[j] = data_4[i];
    j++;
}

for (int i = 0; i < data_6.Length; i++)
{
    data_3_final[j] = data_6[i];
    j++;
}

byte[] data_sypher_3 = new byte[byteArray.Length];
j -= 18;
for (int i = 0; i < byteArray.Length; i++)
{
    if (i < byteArray_1.Length)
    {
        data_sypher_3[i] = data_3_final[j];
        j++;
    }
    else
    {
        data_sypher_3[i] = 0;
        j++;
    }
}
if (byteArray.Length < 18)
    j += 18 - byteArray.Length;
byte[] data_syphered_3 = SolveParity(byteArray, data_sypher_3);

for (int i = 0; i < 18; i++)
{
    if (i < data_syphered_3.Length)
    {
        data_3_final[j] = data_syphered_3[i];
        j++;
    }
    else
    {
        data_3_final[j] = 0;
        j++;
    }
}

string dir = Directory.GetCurrentDirectory();

// Сохранение разделенных файлов

string fileName = Path.GetFileNameWithoutExtension(filePath);
string cur_fileName = fileName + "_1";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_1_final);

cur_fileName = fileName + "_2";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_2_final);

cur_fileName = fileName + "_3";

await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_3_final);

Console.WriteLine("Файлы успешно представлены в виде набора байтов, они схоранены в виде файлов");
Console.ReadLine();

byte[] SolveParity(byte[] data_1, byte[] data_2)
{
    byte[] parity = new byte[data_1.Length];
    for (int i = 0; i < parity.Length; i++)
        parity[i] = (byte)(data_1[i] ^ data_2[i]);
    return parity;
}

