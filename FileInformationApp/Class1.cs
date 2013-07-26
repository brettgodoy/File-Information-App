using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace FileInformationApp
{
    public class Class1
    {

        List<FileInfo> fileList = new List<FileInfo>();
        List<FileInfo> jpegAndPdfList = new List<FileInfo>();

        public string FindFilesOld(string folderPath) 
        {
            DirectoryInfo findFileDir = new DirectoryInfo(folderPath);
            var mainDirectoryFiles = findFileDir.GetFiles();
            var subdirectories = findFileDir.GetDirectories();
            
            List<FileInfo> files = new List<FileInfo>();

            foreach (FileInfo file in mainDirectoryFiles) 
            {
                files.Add(file);
            }

            
            foreach (DirectoryInfo directory in subdirectories)
            {
                var subDirFiles = directory.GetFiles();
                foreach (FileInfo file in subDirFiles) 
                {
                    files.Add(file);
                }
                
              

            }

            Console.WriteLine(files);
            Console.WriteLine(subdirectories);
            return "End";
        }


        public DirectoryInfo DirInfo(string folderpath) 
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderpath);
            return dirInfo;
        }

        public List<FileInfo> FindFiles(DirectoryInfo dirInfo) 
        {
            foreach (FileInfo f in dirInfo.GetFiles())
            {
                fileList.Add(f);
            }
            foreach (DirectoryInfo d in dirInfo.GetDirectories())
            {
                FindFiles(d);
            }
            return fileList;

        }

        public List<FileInfo> getSpecificFiles(List<FileInfo> fileList) 
        {
            foreach (FileInfo fileInfo in fileList) 
            {
                bool isFileJpeg = isJpeg(fileInfo.FullName);
                bool isFilePdf = isPdf(fileInfo.FullName);
                if (isFileJpeg == true) 
                {
                    jpegAndPdfList.Add(fileInfo);
                }
                else if (isFilePdf == true)
                {
                    jpegAndPdfList.Add(fileInfo);
                }
            }

            return jpegAndPdfList;
        }

        public bool isJpeg(string filename) 
        {
            using (BinaryReader binaryReader = new BinaryReader(File.Open(filename, FileMode.Open))) 
            {
                UInt16 stream = binaryReader.ReadUInt16();
                if (stream == 0xd8FF)
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
        }

        public bool isPdf(string fileName) 
        {
            byte[] buffer = null;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buffer = br.ReadBytes(5);

            var enc = new ASCIIEncoding();
            var header = enc.GetString(buffer);

            if (buffer[0] == 0x25 && buffer[1] == 0x50
                && buffer[2] == 0x44 && buffer[3] == 0x46)
            {
                return header.StartsWith("%PDF-");
            }
            return false;
        }


        public string InputPaths() 
        {
            bool isValidFolderPath = false;
            string path = "";

            while (isValidFolderPath == false)
            {
                Console.WriteLine("Please enter a folder path");
                string folderPath = Console.ReadLine();

                // Check to make sure folderPath is valid
                // Check to make sure csvPath is valid
                // Sanitize input?

                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);

                if (dirInfo.Exists != true)
                {
                    Console.WriteLine("This path does not exist.");
                }
                else
                {
                    isValidFolderPath = true;
                    path = folderPath;
                }
            }

            return path;
        }

        public string getFileAttributes(List<FileInfo> jpegAndPdfFiles) 
        {
            StringBuilder strBldr = new StringBuilder();

            foreach (FileInfo file in jpegAndPdfFiles) 
            {
                strBldr.Append(file.FullName + ", ");
                strBldr.Append(file.Length + ", ");
                strBldr.Append(GetMd5Hash(file.FullName) + ", ");
                strBldr.Append(file.Extension + ", ");
            }

            return strBldr.ToString();
        }

        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public void exportInfo(string info) 
        {
            bool isValidCsvPath = false;
            string outputPath = "";
            DateTime time = DateTime.Now;
            string format = "Mdhmmssyy";           
            var currTime = time.ToString(format); 

            while (isValidCsvPath == false)
            {
                Console.WriteLine("Files found. Please enter a CSV destination path");
                string csvPath = Console.ReadLine();

                DirectoryInfo csvDirInfo = new DirectoryInfo(csvPath);

                if (csvDirInfo.Exists != true)
                {
                    Console.WriteLine("This path does not exist");
                }
                else
                {
                    isValidCsvPath = true;
                    outputPath = csvPath;
                }
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(outputPath + "/fileInfo" + currTime + ".txt");
            file.WriteLine(info);

            file.Close();
        }


        static void Main() 
        {
            Class1 class1 = new Class1();
            DirectoryInfo dirInfo = class1.DirInfo(class1.InputPaths());
            var filesList = class1.FindFiles(dirInfo);
            var jpegAndPdfFiles = class1.getSpecificFiles(filesList);
            if (jpegAndPdfFiles.Count() == 0) 
            {
                Console.WriteLine("No JPEG or PDF files found. Aborting...");
                return;
            }

            var fileAttributes = class1.getFileAttributes(jpegAndPdfFiles);
            class1.exportInfo(fileAttributes);
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
