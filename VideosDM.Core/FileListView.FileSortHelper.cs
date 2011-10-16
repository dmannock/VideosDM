using System;
using System.Collections.Generic;
using System.Text;

namespace VideosDM.Core
{
    public partial class FileListView
    {
        public class FileSortHelper
        {
            FileListView fList;

            public FileListView.FileSort Current
            {
                get { return this.fList.FileSorting; }
            }

            public FileSortHelper(FileListView fileListView)
            {
                this.fList = fileListView;
            }

            public bool HasNext()
            {
                if ((byte)this.fList.FileSorting < Enum.GetValues(typeof(FileListView.FileSort)).Length - 1)
                    return true;
                return false;
            }

            public bool Next()
            {
                if (HasNext())
                {
                    fList.FileSorting = (FileListView.FileSort)((byte)fList.FileSorting + 1);
                    return true;
                }
                else
                {
                    Reset();
                    return false;
                }
            }

            public void Reset()
            {
                this.fList.FileSorting = (FileListView.FileSort)0; //FileSort.AlphabeticalAscending;
            }
        }
    }
}
