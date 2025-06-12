using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using vkr.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace vkr.Services
{
    public class RAID5Service
    {
        private readonly DropboxService _dropboxService;
        private readonly GoogleDriveService _googledriveService;
        private readonly YandexDiskService _yandexdiskService;

        public RAID5Service(DropboxService dropboxService, GoogleDriveService googledriveService, YandexDiskService yandexdiskService)
        {
            _dropboxService = dropboxService;
            _googledriveService = googledriveService;
            _yandexdiskService = yandexdiskService;
        }

        public async Task<string> WriteData(byte[] fileContent, string origFileName, int option, string key)
        {
            string[] partsConfig = key.Split(',');
            int part_portional_a = int.Parse(partsConfig[0]);
            int part_portional_b = int.Parse(partsConfig[1]);

            byte[] originalSize = BitConverter.GetBytes(fileContent.Length);
            int part_size_a = fileContent.Length / 100 * part_portional_a / 2;
            int part_size_b = fileContent.Length / 100 * part_portional_b / 2;
            int part_size_c = 0;

            string response = "Ok";

            if (part_size_a + part_size_b < fileContent.Length)
            {
                part_size_c += (int)Math.Ceiling((fileContent.Length - (part_size_a + part_size_b) * 2) / 2.0);
            }

            int reduce = 0;
            if (fileContent.Length % 2 != 0)
            {
                reduce = 1;
            }

            string extension = "";
            string combined = part_size_a.ToString() + "!" + part_size_b.ToString() + "!" + part_size_c.ToString() + "!" + extension + "!" + reduce;
            byte[] byteArray = Encoding.UTF8.GetBytes(combined);

            int j = 0;
            byte[] data_1 = new byte[part_size_a];
            for (int i = 0; i < part_size_a; i++)
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
            for (int i = 0; i < part_size_a; i++)
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
            for (int i = 0; i < part_size_b; i++)
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
            for (int i = 0; i < part_size_b; i++)
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
            byte[] data_5 = new byte[part_size_c];
            for (int i = 0; i < part_size_c; i++)
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
            byte[] data_6 = new byte[part_size_c];

            for (int i = 0; i < part_size_c; i++)
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

            j = 0;
            byte[] data_a = new byte[part_size_a + part_size_b + part_size_c + 4];
            for (int i = 0; i < part_size_a; i++)
            {
                data_a[j] = data_1[i];
                j++;
            }
            for (int i = 0; i < part_size_b; i++)
            {
                data_a[j] = data_3[i];
                j++;
            }
            byte[] data_56_parity = SolveParity(data_5, data_6);

            for (int i = 0; i < data_56_parity.Length; i++)
            {
                data_a[j] = data_56_parity[i];
                j++;
            }
            for (int i = 0; i < 4; i++)
            {
                data_a[j] = originalSize[i];
                j++;
            }

            j = 0;
            byte[] data_b = new byte[part_size_a + part_size_b + part_size_c + 4];
            for (int i = 0; i < part_size_a; i++)
            {
                data_b[j] = data_2[i];
                j++;
            }
            byte[] data_34_parity = SolveParity(data_3, data_4);

            for (int i = 0; i < data_34_parity.Length; i++)
            {
                data_b[j] = data_34_parity[i];
                j++;
            }
            for (int i = 0; i < data_5.Length; i++)
            {
                data_b[j] = data_5[i];
                j++;
            }
            for (int i = 0; i < 4; i++)
            {
                data_b[j] = originalSize[i];
                j++;
            }

            j = 0;
            byte[] data_parity = new byte[part_size_a + part_size_b + part_size_c + 4];
            byte[] parity = SolveParity(data_1, data_2);
            for (int i = 0; i < part_size_a; i++)
            {
                data_parity[j] = parity[i];
                j++;
            }
            for (int i = 0; i < data_4.Length; i++)
            {
                data_parity[j] = data_4[i];
                j++;
            }

            for (int i = 0; i < data_6.Length; i++)
            {
                data_parity[j] = data_6[i];
                j++;
            }

            for (int i = 0; i < 4; i++)
            {
                data_parity[j] = originalSize[i];
                j++;
            }

            string dir = Directory.GetCurrentDirectory();
            if (option == 1)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(origFileName);
                    string cur_fileName = fileName + "_A";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_a);

                    await _yandexdiskService.UploadFile(cur_fileName, Path.Combine(dir, cur_fileName));
                    File.Delete(Path.Combine(dir, cur_fileName));

                    cur_fileName = fileName + "_B";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_b);

                    await _dropboxService.UploadFile(Path.Combine(dir, cur_fileName), "/" + cur_fileName);
                    File.Delete(Path.Combine(dir, cur_fileName));

                    cur_fileName = fileName + "_C";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_parity);

                    await _googledriveService.UploadFile(Path.Combine(dir, cur_fileName));
                    File.Delete(Path.Combine(dir, cur_fileName));

                    return response;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else if (option == 2)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(origFileName);
                    string cur_fileName = fileName + "_A";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_a);

                    await _googledriveService.UploadFile(Path.Combine(dir, cur_fileName));
                    File.Delete(Path.Combine(dir, cur_fileName));

                    cur_fileName = fileName + "_B";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_b);

                    await _dropboxService.UploadFile(Path.Combine(dir, cur_fileName), "/" + cur_fileName);
                    File.Delete(Path.Combine(dir, cur_fileName));

                    cur_fileName = fileName + "_C";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_parity);

                    await _yandexdiskService.UploadFile(cur_fileName, Path.Combine(dir, cur_fileName));
                    File.Delete(Path.Combine(dir, cur_fileName));

                    return response;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else if (option == 3)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(origFileName);
                    string cur_fileName = fileName + "_A";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_a);

                    await _googledriveService.UploadFile(Path.Combine(dir, cur_fileName));
                    File.Delete(Path.Combine(dir, cur_fileName));

                    cur_fileName = fileName + "_B";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_b);

                    await _yandexdiskService.UploadFile(cur_fileName, Path.Combine(dir, cur_fileName));
                    File.Delete(Path.Combine(dir, cur_fileName));

                    cur_fileName = fileName + "_C";
                    await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_parity);

                    await _dropboxService.UploadFile(Path.Combine(dir, cur_fileName), "/" + cur_fileName);
                    File.Delete(Path.Combine(dir, cur_fileName));

                    return response;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return response;
        }
        public async Task<byte[]> ReadData(string fileName, int option, string key)
        {
            string curFileName = fileName;

            if (option == 1)
            {
                curFileName = fileName + "_A";
                byte[] data_a = await _yandexdiskService.DownloadFile(curFileName);
                curFileName = fileName + "_B";
                byte[] data_b = await _dropboxService.DownloadFile("/" + curFileName);
                curFileName = fileName + "_C";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_parity = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for(int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_par = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_par[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }
                counter = 0;
                byte[] data_par_a = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_par_a[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for(int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }

                return data;

            } else if(option == 2)
            {
                curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                curFileName = fileName + "_B";
                byte[] data_b = await _dropboxService.DownloadFile("/" + curFileName);
                curFileName = fileName + "_C";
                byte[] data_parity = await _yandexdiskService.DownloadFile(curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_par = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_par[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }
                counter = 0;
                byte[] data_par_a = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_par_a[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }

                return data;

            } else if (option == 3)
            {
                curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                curFileName = fileName + "_B";
                byte[] data_b = await _yandexdiskService.DownloadFile(curFileName);
                curFileName = fileName + "_C";
                byte[] data_parity = await _dropboxService.DownloadFile("/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_par = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_par[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }
                counter = 0;
                byte[] data_par_a = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_par_a[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }

                return data;
            }
            byte[] datsa = new byte[1];
            return datsa;
        }
        public async Task<byte[]> ReadDataWithoutYandex(string fileName, int option, string key)
        {
            string curFileName = fileName;
            if (option == 1)
            {
                curFileName = fileName + "_B";
                byte[] data_b = await _dropboxService.DownloadFile("/" + curFileName);
                curFileName = fileName + "_C";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_parity = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_34_parity = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_34_parity[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_12_parity = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_12_parity[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data_1 = SolveParity(data_12_parity, data_2);
                byte[] data_3 = SolveParity(data_34_parity, data_4);

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }
                return data;
            } else if(option == 2)
            {
                curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                curFileName = fileName + "_B";
                byte[] data_b = await _dropboxService.DownloadFile("/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;

                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }
                byte[] data_56_parity = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_56_parity[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_34_parity = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_34_parity[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }
                byte[] data_6 = SolveParity(data_56_parity, data_5);
                byte[] data_4 = SolveParity(data_34_parity, data_3);
                if (data_6[data_6.Length - 5] == 0)
                    reduce = 1;
                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }

                return data;

            } else if(option == 3)
            {
                curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                curFileName = fileName + "_C";
                byte[] data_parity = await _dropboxService.DownloadFile("/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_a.Length - 4; i < data_a.Length; i++)
                {
                    originalSizeBytes[counter] = data_a[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }
                byte[] data_56_parity = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_56_parity[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_12_parity = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_12_parity[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data_2 = SolveParity(data_12_parity, data_1);
                byte[] data_5 = SolveParity(data_56_parity, data_6);

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }
                return data;
            }

            byte[] datsa = new byte[1];
            return datsa;
        }

        public async Task<byte[]> ReadDataWithoutGoogle(string fileName, int option, string key)
        {
            string curFileName = fileName;
            if (option == 1)
            {
                curFileName = fileName + "_A";
                byte[] data_a = await _yandexdiskService.DownloadFile(curFileName);
                curFileName = fileName + "_B";
                byte[] data_b = await _dropboxService.DownloadFile("/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;

                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }
                byte[] data_56_parity = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_56_parity[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_34_parity = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_34_parity[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }
                byte[] data_6 = SolveParity(data_56_parity, data_5);
                byte[] data_4 = SolveParity(data_34_parity, data_3);
                if (data_6[data_6.Length - 5] == 0)
                    reduce = 1;
                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }

                return data;
            }
            else if (option == 2)
            {
                curFileName = fileName + "_C";
                byte[] data_parity = await _yandexdiskService.DownloadFile(curFileName);
                curFileName = fileName + "_B";
                byte[] data_b = await _dropboxService.DownloadFile("/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_34_parity = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_34_parity[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_12_parity = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_12_parity[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data_1 = SolveParity(data_12_parity, data_2);
                byte[] data_3 = SolveParity(data_34_parity, data_4);

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }
                return data;

            }
            else if (option == 3)
            {
                curFileName = fileName + "_B";
                byte[] data_b = await _yandexdiskService.DownloadFile(curFileName);
                curFileName = fileName + "_C";
                byte[] data_parity = await _dropboxService.DownloadFile("/" + curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_34_parity = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_34_parity[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_12_parity = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_12_parity[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data_1 = SolveParity(data_12_parity, data_2);
                byte[] data_3 = SolveParity(data_34_parity, data_4);

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }
            }

            byte[] datsa = new byte[1];
            return datsa;
        }

        public async Task<byte[]> ReadDataWithoutDropbox(string fileName, int option, string key)
        {
            string curFileName = fileName;
            if (option == 1)
            {
                curFileName = fileName + "_A";
                byte[] data_a = await _yandexdiskService.DownloadFile(curFileName);
                curFileName = fileName + "_C";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_parity = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_a.Length - 4; i < data_a.Length; i++)
                {
                    originalSizeBytes[counter] = data_a[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }
                byte[] data_56_parity = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_56_parity[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_12_parity = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_12_parity[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data_2 = SolveParity(data_12_parity, data_1);
                byte[] data_5 = SolveParity(data_56_parity, data_6);

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }
                return data;
            }
            else if (option == 2)
            {
                curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                curFileName = fileName + "_C";
                byte[] data_parity = await _yandexdiskService.DownloadFile(curFileName);
                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_a.Length - 4; i < data_a.Length; i++)
                {
                    originalSizeBytes[counter] = data_a[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;
                if (data_parity[data_parity.Length - 5] == 0)
                    reduce = 1;
                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }
                byte[] data_56_parity = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_56_parity[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_12_parity = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_12_parity[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_4 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_4[i] = data_parity[counter];
                    counter++;
                }
                byte[] data_6 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_6[i] = data_parity[counter];
                    counter++;
                }

                byte[] data_2 = SolveParity(data_12_parity, data_1);
                byte[] data_5 = SolveParity(data_56_parity, data_6);

                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }
                return data;

            }
            else if (option == 3)
            {
                curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                curFileName = fileName + "_B";
                byte[] data_b = await _yandexdiskService.DownloadFile(curFileName);

                string[] partsConfig = key.Split(',');

                byte[] originalSizeBytes = new byte[4];
                int counter = 0;
                for (int i = data_b.Length - 4; i < data_b.Length; i++)
                {
                    originalSizeBytes[counter] = data_b[i];
                    counter++;
                }
                int originalSize = BitConverter.ToInt32(originalSizeBytes, 0);
                int part_size_a = originalSize / 100 * int.Parse(partsConfig[0]) / 2;
                int part_size_b = originalSize / 100 * int.Parse(partsConfig[1]) / 2;
                int part_size_c = 0;
                int reduce = 0;

                if (part_size_a + part_size_b < originalSize)
                {
                    part_size_c += (int)Math.Ceiling((originalSize - (part_size_a + part_size_b) * 2) / 2.0);
                }

                counter = 0;
                byte[] data_1 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_1[i] = data_a[counter];
                    counter++;
                }
                byte[] data_3 = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_3[i] = data_a[counter];
                    counter++;
                }
                byte[] data_56_parity = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_56_parity[i] = data_a[counter];
                    counter++;
                }

                counter = 0;
                byte[] data_2 = new byte[part_size_a];
                for (int i = 0; i < part_size_a; i++)
                {
                    data_2[i] = data_b[counter];
                    counter++;
                }
                byte[] data_34_parity = new byte[part_size_b];
                for (int i = 0; i < part_size_b; i++)
                {
                    data_34_parity[i] = data_b[counter];
                    counter++;
                }
                byte[] data_5 = new byte[part_size_c];
                for (int i = 0; i < part_size_c; i++)
                {
                    data_5[i] = data_b[counter];
                    counter++;
                }
                byte[] data_6 = SolveParity(data_56_parity, data_5);
                byte[] data_4 = SolveParity(data_34_parity, data_3);
                if (data_6[data_6.Length - 5] == 0)
                    reduce = 1;
                byte[] data = new byte[(part_size_a + part_size_b + part_size_c) * 2 - reduce];
                counter = 0;
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_1[i];
                    counter++;
                }
                for (int i = 0; i < part_size_a; i++)
                {
                    data[counter] = data_2[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_3[i];
                    counter++;
                }
                for (int i = 0; i < part_size_b; i++)
                {
                    data[counter] = data_4[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c; i++)
                {
                    data[counter] = data_5[i];
                    counter++;
                }
                for (int i = 0; i < part_size_c - reduce; i++)
                {
                    data[counter] = data_6[i];
                    counter++;
                }

                return data;
            }

            byte[] datsa = new byte[1];
            return datsa;
        }

        public async Task<string> GetFileExtention(string fileName, int option)
        {
            string extention = "";
            if(option == 1)
            {
                string curFileName = fileName + "_A";
                byte[] data_a = await _yandexdiskService.DownloadFile(curFileName);
                byte[] bytes = new byte[45];
                int counter = 0;
                for (int i = data_a.Length - 45; i < data_a.Length; i++)
                {
                    bytes[counter] = data_a[i];
                    counter++;
                }
                string combinedString = Encoding.UTF8.GetString(bytes).Replace("\0", "");
                string[] parts = combinedString.Split('!');
                extention = parts[3];

            } else if(option == 2)
            {
                string curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] bytes = new byte[45];
                int counter = 0;
                for (int i = data_a.Length - 45; i < data_a.Length; i++)
                {
                    bytes[counter] = data_a[i];
                    counter++;
                }
                string combinedString = Encoding.UTF8.GetString(bytes).Replace("\0", "");
                string[] parts = combinedString.Split('!');
                extention = parts[3];
            } else
            {
                string curFileName = fileName + "_A";
                await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] data_a = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
                byte[] bytes = new byte[45];
                int counter = 0;
                for (int i = data_a.Length - 45; i < data_a.Length; i++)
                {
                    bytes[counter] = data_a[i];
                    counter++;
                }
                string combinedString = Encoding.UTF8.GetString(bytes).Replace("\0", "");
                string[] parts = combinedString.Split('!');
                extention = parts[3];
            }
            return extention;
        }

        public byte[] SolveParity(byte[] data_1, byte[] data_2)
        {
            byte[] parity = new byte[data_1.Length];
            for (int i = 0; i < parity.Length; i++)
                parity[i] = (byte)(data_1[i] ^ data_2[i]);
            return parity;

        }
    }
}
