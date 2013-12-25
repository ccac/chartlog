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
using System.Collections.Generic;
using System.Text;

namespace SilverlightApplication1
{
    public enum Types { Log, Warning, Error };

    public class LogInfoHandler
    {
        private LogInfoHandler()
        {
            InitializeFileNameList();
        }

        private static LogInfoHandler _intance = null;

        private const String FILE_NAME_PREFIX = "Chartlog";
        private const String FILE_TYPE = ".txt";

        private const long EXTRA_SIZE = 1024 * 1024;
        private const long FIFTH_KB = 50 * 1024;
        private const long HUNDRED_KB = 100 * 1024;

        private List<String> _fileNameList = new List<string>();
        private List<LogInfoItem> _logList = new List<LogInfoItem>();

        private void InitializeFileNameList()
        {
            this._fileNameList.Clear();

            String[] nameList = LocalStrogeFacade.GetFilesName();

            foreach (String name in nameList)
            {
                if (name.Contains(FILE_NAME_PREFIX))
                {
                    this._fileNameList.Add(name);
                }
            }
        }

        private long GetSizeOfList(List<LogInfoItem> list)
        {
            int sum = 0;

            foreach (LogInfoItem item in list)
            {
                sum += item.GetTotalSize();
            }

            return sum;
        }
        /// <summary>
        /// This function will get a sign from file's name, which marks every session.
        /// </summary>
        /// <param name="name">e.g. Chartlog10_1</param>
        /// <returns>e.g 10</returns>
        private String GetFileSign(String name)
        {
            String fileName = name;
            fileName = fileName.Replace(FILE_NAME_PREFIX, "");
            fileName = fileName.Replace(FILE_TYPE, "");

            int index = fileName.IndexOf("_");
            int length = fileName.Length;
            //int count = 0;

            //char[] charlist = fileName.ToCharArray();
            //for (int n = index; n < length; n++)
            //{
            //    count++;
            //}
            //fileName = fileName.Remove(index, count);

            fileName = fileName.Substring(0, index);
            return fileName;
        }
        /// <summary>
        /// It gets last one's sign from all of files.
        /// </summary>
        /// <returns>the Maximal number</returns>
        private int GetLastFileSign()
        {
            int max = 0;
            String firstOne = _fileNameList[0];
            max = Convert.ToInt32(GetFileSign(firstOne));

            for (int i = 1; i < _fileNameList.Count; i++)
            {
                String name = _fileNameList[i];
                int sign = Convert.ToInt32(GetFileSign(name));
                if (sign > max)
                {
                    max = sign;
                }
            }

            return max;
        }
        /// <summary>
        /// It makes file's prefix, e.g "Chartlog10", "Chartlog11"
        /// </summary>
        /// <returns></returns>
        private String MakePrefixName()
        {
            String name = null;

            if (_fileNameList.Count == 0)
            {
                name = FILE_NAME_PREFIX + "1";
            }
            else
            {
                int newSign = GetLastFileSign();
                newSign++;
                name = FILE_NAME_PREFIX + newSign.ToString();
            }

            return name;
        }

        public void WirteToLogList(String dateTime, Types type, String content)
        {
            try
            {
                LogInfoItem item = new LogInfoItem();
                item.DateTime = dateTime;
                item.Type = type;
                item.Content = content;

                this._logList.Add(item);
            }
            catch (Exception e)
            {
            }
        }
        public void WirteToFile()
        {
            try
            {
                int partSign = 1;
                StringBuilder sumContent = new StringBuilder();

                long freeSize = LocalStrogeFacade.GetFreeSize();
                long actualSize = GetSizeOfList(_logList);

                if (freeSize < actualSize)
                {
                    long gapSize = actualSize - freeSize;
                    long newSize = LocalStrogeFacade.GetQuota() + gapSize + EXTRA_SIZE;

                    bool increased = LocalStrogeFacade.IncreaseSize(newSize);
                    if (increased)
                    {
                        WirteToFile();
                    }
                }
                else
                {
                    StringBuilder itemContent = new StringBuilder();

                    foreach (LogInfoItem item in _logList)
                    {
                        itemContent.Append(item.DateTime);
                        itemContent.AppendLine();
                        itemContent.Append(item.Type);
                        itemContent.AppendLine();
                        itemContent.Append(item.Content);

                        int size = item.GetTotalSize();
                        if (size > FIFTH_KB)
                        {
                            String fileName = MakePrefixName() + "_" + partSign.ToString() + FILE_TYPE;
                            partSign++;

                            LocalStrogeFacade.WirteFile(fileName, itemContent.ToString());
                            itemContent = itemContent.Remove(0, itemContent.Length);
                        }
                        else
                        {
                            sumContent.Append(itemContent.ToString());
                            sumContent.AppendLine();

                            int curSize = System.Text.Encoding.UTF8.GetByteCount(sumContent.ToString());
                            if (curSize > FIFTH_KB && curSize < HUNDRED_KB)
                            {
                                String filePath = MakePrefixName() + "_" + partSign.ToString() + FILE_TYPE;
                                partSign++;

                                LocalStrogeFacade.WirteFile(filePath, sumContent.ToString());
                                sumContent = sumContent.Remove(0, sumContent.Length);
                            }

                            itemContent = itemContent.Remove(0, itemContent.Length);
                        }
                    }

                    if (sumContent.Length > 0)
                    {
                        String lastFile = MakePrefixName() + "_" + partSign.ToString() + FILE_TYPE;
                        partSign++;

                        LocalStrogeFacade.WirteFile(lastFile, sumContent.ToString());
                        sumContent = sumContent.Remove(0, sumContent.Length);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public String GetLogCurSession()
        {
            try
            {
                StringBuilder content = new StringBuilder();

                foreach (LogInfoItem item in this._logList)
                {
                    content.Append(item.DateTime);
                    content.AppendLine();
                    content.Append(item.Type);
                    content.AppendLine();
                    content.Append(item.Content);
                }

                return content.ToString();
            }
            catch(Exception e)
            {
                return e.ToString();
            }
        }
        public String GetLogPreSession()
        {
            try
            {
                StringBuilder content = new StringBuilder();
                String lastPrefixSign = FILE_NAME_PREFIX + GetLastFileSign().ToString();

                foreach (String name in _fileNameList)
                {
                    if (name.Contains(lastPrefixSign))
                    {
                        String oneFileContent = LocalStrogeFacade.ReadFile(name);
                        content.Append(oneFileContent);
                        content.AppendLine();
                    }
                }

                return content.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public void ClearAllFiles()
        {
            try
            {
                InitializeFileNameList();

                foreach (String name in _fileNameList)
                {
                    LocalStrogeFacade.DeleteFile(name);
                }
            }
            catch (Exception e)
            {
            }
        }

        public static LogInfoHandler GetHandlerInstance()
        {
            if (_intance == null)
            {
                _intance = new LogInfoHandler();
            }

            return _intance;
        }
    }
}
