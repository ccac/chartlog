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

namespace SilverlightApplication1
{
    public class LogInfoItem
    {
        private String _dateTime;
        private Types _type;
        private String _content;

        public LogInfoItem()
        {
        }

        public String DateTime
        {
            get
            {
                return _dateTime;
            }
            set
            {
                _dateTime = value;
            }
        }

        public Types Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public String Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        public int GetTotalSize()
        {
            int sum = 0;

            sum += System.Text.Encoding.UTF8.GetByteCount(_dateTime);
            sum += System.Text.Encoding.UTF8.GetByteCount(_type.ToString());
            sum += System.Text.Encoding.UTF8.GetByteCount(_content);

            return sum;
        }
    }
}
