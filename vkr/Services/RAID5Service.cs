using System.IO;
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

        public async Task<string> WriteData(string filePath)
        {
            string response = "Ok";
            byte[] fileContent = await File.ReadAllBytesAsync(filePath);
            int j = 0;
            byte[] data_1 = new byte[fileContent.Length / 6];
            for(int i = 0; i < data_1.Length; i++)
            {
                data_1[i] = fileContent[j];
                j++;
            }
            byte[] data_2 = new byte[fileContent.Length / 6];
            for (int i = 0; i < data_2.Length; i++)
            {
                data_2[i] = fileContent[j];
                j++;
            }
            byte[] data_3 = new byte[fileContent.Length / 6];
            for (int i = 0; i < data_3.Length; i++)
            {
                data_3[i] = fileContent[j];
                j++;
            }
            byte[] data_4 = new byte[fileContent.Length / 6];
            for (int i = 0; i < data_4.Length; i++)
            {
                data_4[i] = fileContent[j];
                j++;
            }
            byte[] data_5 = new byte[fileContent.Length / 6];
            for (int i = 0; i < data_5.Length; i++)
            {
                data_5[i] = fileContent[j];
                j++;
            }
            byte[] data_6 = new byte[fileContent.Length / 6];
            for (int i = 0; i < data_6.Length; i++)
            {
                data_6[i] = fileContent[j];
                j++;
            }

            byte[] parity = new byte[fileContent.Length / 6];
            string dir = Directory.GetCurrentDirectory();
            try
            {
                parity = SolveParity(data_1, data_2);

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string cur_fileName = fileName + "A_1";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_1);

                await _yandexdiskService.UploadFile(cur_fileName, Path.Combine(dir, cur_fileName));
                File.Delete(Path.Combine(dir, cur_fileName));

                cur_fileName = fileName + "B_1";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_2);

                await _dropboxService.UploadFile(Path.Combine(dir, cur_fileName), "/" + cur_fileName);
                File.Delete(Path.Combine(dir, cur_fileName));

                cur_fileName = fileName + "C_P";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

                await _googledriveService.UploadFile(Path.Combine(dir, cur_fileName));
                File.Delete(Path.Combine(dir, cur_fileName));

                cur_fileName = fileName + "A_2";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_3);

                await _yandexdiskService.UploadFile(cur_fileName, Path.Combine(dir, cur_fileName));
                File.Delete(Path.Combine(dir, cur_fileName));

                parity = SolveParity(data_3, data_4);

                cur_fileName = fileName + "B_P";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

                await _dropboxService.UploadFile(Path.Combine(dir, cur_fileName), "/" + cur_fileName);
                File.Delete(Path.Combine(dir, cur_fileName));

                cur_fileName = fileName + "C_1";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_4);

                await _googledriveService.UploadFile(Path.Combine(dir, cur_fileName));
                File.Delete(Path.Combine(dir, cur_fileName));

                cur_fileName = fileName + "B_2";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_5);

                await _dropboxService.UploadFile(Path.Combine(dir, cur_fileName), "/" + cur_fileName);
                File.Delete(Path.Combine(dir, cur_fileName));

                cur_fileName = fileName + "C_2";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), data_6);

                await _googledriveService.UploadFile(Path.Combine(dir, cur_fileName));
                File.Delete(Path.Combine(dir, cur_fileName));

                parity = SolveParity(data_5, data_6);

                cur_fileName = fileName + "A_P";
                await File.WriteAllBytesAsync(Path.Combine(dir, cur_fileName), parity);

                await _yandexdiskService.UploadFile(cur_fileName, Path.Combine(dir, cur_fileName));
                File.Delete(Path.Combine(dir, cur_fileName));

                return response;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<byte[]> ReadData(string fileName)
        {
            string curFileName = fileName + "A_1";
            byte[] data_1 = await _yandexdiskService.DownloadFile(curFileName);
            byte[] data = new byte[data_1.Length * 6];
            int j = 0;
            for(int i = 0; i < data_1.Length; i++)
            {
                data[j] = data_1[i];
                j++;
            }
            curFileName = fileName + "B_1";
            byte[] data_2 = await _dropboxService.DownloadFile("/" + curFileName);
            for (int i = 0; i < data_2.Length; i++)
            {
                data[j] = data_2[i];
                j++;
            }
            curFileName = fileName + "A_2";
            byte[] data_3 = await _yandexdiskService.DownloadFile(curFileName);
            for (int i = 0; i < data_3.Length; i++)
            {
                data[j] = data_3[i];
                j++;
            }
            curFileName = fileName + "C_1";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_4 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            for (int i = 0; i < data_4.Length; i++)
            {
                data[j] = data_4[i];
                j++;
            }
            curFileName = fileName + "B_2";
            byte[] data_5 = await _dropboxService.DownloadFile("/" + curFileName);
            for (int i = 0; i < data_5.Length; i++)
            {
                data[j] = data_5[i];
                j++;
            }
            curFileName = fileName + "C_2";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_6 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            for (int i = 0; i < data_6.Length; i++)
            {
                data[j] = data_6[i];
                j++;
            }
            return data;
        }
        public async Task<byte[]> ReadDataWithoutYandex(string fileName)
        {
            string curFileName = fileName + "C_P";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_1 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            curFileName = fileName + "B_1";
            byte[] data_2 = await _dropboxService.DownloadFile("/" + curFileName);
            data_1 = SolveParity(data_1, data_2);
            byte[] data = new byte[data_1.Length * 6];
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
            curFileName = fileName + "B_P";
            byte[] data_3 = await _dropboxService.DownloadFile("/" + curFileName);
            curFileName = fileName + "C_1";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_4 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            data_3 = SolveParity(data_3, data_4);
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
            curFileName = fileName + "B_2";
            byte[] data_5 = await _dropboxService.DownloadFile("/" + curFileName);
            for (int i = 0; i < data_5.Length; i++)
            {
                data[j] = data_5[i];
                j++;
            }
            curFileName = fileName + "C_2";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_6 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            for (int i = 0; i < data_6.Length; i++)
            {
                data[j] = data_6[i];
                j++;
            }
            return data;
        }

        public async Task<byte[]> ReadDataWithoutGoogle(string fileName)
        {
            string curFileName = fileName + "A_1";
            byte[] data_1 = await _yandexdiskService.DownloadFile(curFileName);
            byte[] data = new byte[data_1.Length * 6];
            int j = 0;
            for (int i = 0; i < data_1.Length; i++)
            {
                data[j] = data_1[i];
                j++;
            }
            curFileName = fileName + "B_1";
            byte[] data_2 = await _dropboxService.DownloadFile("/" + curFileName);
            for (int i = 0; i < data_2.Length; i++)
            {
                data[j] = data_2[i];
                j++;
            }
            curFileName = fileName + "A_2";
            byte[] data_3 = await _yandexdiskService.DownloadFile(curFileName);
            curFileName = fileName + "B_P";
            byte[] data_4 = await _dropboxService.DownloadFile("/" + curFileName);
            data_4 = SolveParity(data_3, data_4);
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
            curFileName = fileName + "B_2";
            byte[] data_5 = await _dropboxService.DownloadFile("/" + curFileName);
            curFileName = fileName + "A_P";
            byte[] data_6 = await _yandexdiskService.DownloadFile(curFileName);
            data_6 = SolveParity(data_5, data_6);
            for (int i = 0; i < data_5.Length; i++)
            {
                data[j] = data_5[i];
                j++;
            }
            for (int i = 0; i < data_6.Length; i++)
            {
                data[j] = data_6[i];
                j++;
            }
            return data;
        }

        public async Task<byte[]> ReadDataWithoutDropbox(string fileName)
        {
            string curFileName = fileName + "A_1";
            byte[] data_1 = await _yandexdiskService.DownloadFile(curFileName);
            byte[] data = new byte[data_1.Length * 6];
            curFileName = fileName + "C_P";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_2 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            data_2 = SolveParity(data_1, data_2);
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
            curFileName = fileName + "A_2";
            byte[] data_3 = await _yandexdiskService.DownloadFile(curFileName);
            for (int i = 0; i < data_3.Length; i++)
            {
                data[j] = data_3[i];
                j++;
            }
            curFileName = fileName + "C_1";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_4 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            for (int i = 0; i < data_4.Length; i++)
            {
                data[j] = data_4[i];
                j++;
            }
            curFileName = fileName + "A_P";
            byte[] data_5 = await _yandexdiskService.DownloadFile(curFileName);
            curFileName = fileName + "C_2";
            await _googledriveService.DownloadFile(curFileName, Directory.GetCurrentDirectory() + "/" + curFileName);
            byte[] data_6 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/" + curFileName);
            data_5 = SolveParity(data_5, data_6);
            for (int i = 0; i < data_5.Length; i++)
            {
                data[j] = data_5[i];
                j++;
            }
            for (int i = 0; i < data_6.Length; i++)
            {
                data[j] = data_6[i];
                j++;
            }
            return data;
        }
        public byte[] SolveParity(byte[] data_1, byte[] data_2)
        {
            byte[] parity = new byte[data_1.Length];
            for(int i = 0; i < parity.Length; i++)
                parity[i] = (byte)(data_1[i] ^ data_2[i]);
            return parity;

        }
    }
}
