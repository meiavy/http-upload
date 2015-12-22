using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HttpGetClient
{
    class DataModel:INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String visibility = "Collapsed";

        public String Visibility
        {
            get { return visibility; }
            set 
            {
                visibility = value;
                if (this.PropertyChanged != null)
                {
                    // 只要属性发生改变。就会通知我们的目标。目标也同样更新
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Visibility"));
                }
            }
        }

        private int fileSize = 0;

        public int FileSize
        {
            get { return fileSize; }
            set 
            {
                fileSize = value;
                if (this.PropertyChanged != null)
                {
                    // 只要属性发生改变。就会通知我们的目标。目标也同样更新
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FileSize"));
                }
            }
        }

        private int uploadSize = 0;

        public int UploadSize
        {
            get { return uploadSize; }
            set 
            { 
                uploadSize = value;
                if (this.PropertyChanged != null)
                {
                    // 只要属性发生改变。就会通知我们的目标。目标也同样更新
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("UploadSize"));
                }
            }
        }

    }
}
