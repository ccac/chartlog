using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;

namespace SilverlightApplication1
{
    public class LocalStrogeFacade
    {
        private LocalStrogeFacade()
        {
        }

        public static bool CreateDirectory(String dirname)
        {
            bool isCreated = false;

            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isCreated = iso.DirectoryExists(dirname);
                if (!isCreated)
                {
                    iso.CreateDirectory(dirname);
                    isCreated = true;
                }
            }

            return isCreated;
        }
        public static bool CreatFile(String filename)
        {
            bool isCreated = false;

            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isCreated = iso.FileExists(filename);
                if (!isCreated)
                {
                    IsolatedStorageFileStream fileStream = iso.CreateFile(filename);
                    isCreated = true;
                    fileStream.Close();
                }
            }

            return isCreated;
        }

        public static void WirteFile(String name, String content)
        {
            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoFile.FileExists(name))
                {
                    IsolatedStorageFileStream fStream = isoFile.CreateFile(name);
                    using (StreamWriter sWriter = new StreamWriter(fStream))
                    {
                        sWriter.Write(content);
                    }
                    fStream.Close();
                }
                else
                {
                    bool isCreated = CreatFile(name);

                    WirteFile(name, content);
                }
            }
        }

        public static String ReadFile(String filepath)
        {
            String content = null;

            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.FileExists(filepath))
                {
                    IsolatedStorageFileStream fileStream = iso.OpenFile(filepath, FileMode.Open, FileAccess.Read);

                    StreamReader reader = new StreamReader(fileStream);
                    content = reader.ReadToEnd();

                    reader.Close();
                    fileStream.Close();
                }
            }

            return content;
        }

        public static void DeleteFile(String filepath)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.FileExists(filepath))
                {
                    iso.DeleteFile(filepath);
                }
            }
        }

        public static void DeleteDir(String dir)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.DirectoryExists(dir))
                {
                    iso.DeleteDirectory(dir);
                }
            }
        }

        public static String[] GetFilesName()
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return iso.GetFileNames();
            }
        }

        public static long GetFreeSize()
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return iso.AvailableFreeSpace;
            }
        }

        public static long GetQuota()
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return iso.Quota;
            }
        }

        public static bool IncreaseSize(long newSize)
        {
            bool isIncreased = false;

            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isIncreased =  iso.IncreaseQuotaTo(newSize);
            }

            return isIncreased;
        }
    }
}
