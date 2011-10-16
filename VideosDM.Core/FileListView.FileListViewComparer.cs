using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VideosDM.Core
{
    public partial class FileListView
    {
        public class FileListViewComparer : IComparer<FileInfo>
        {
            private FileSort sortOrder = FileSort.AlphabeticalAscending;

            public FileSort SortingOrder
            {
                set { this.sortOrder = value; }
            }

            public FileListViewComparer(FileSort SortingOrder)
            {
                this.sortOrder = SortingOrder;
            }

            public int Compare(FileInfo obj1, FileInfo obj2)
            {
                FileInfo f1 = (FileInfo)obj1;
                FileInfo f2 = (FileInfo)obj2;

                switch (sortOrder)
                {
                    default:
                    case FileSort.AlphabeticalAscending:
                        return f1.Name.CompareTo(f2.Name);
                    case FileSort.AlphabeticalDescending:
                        return f1.Name.CompareTo(f2.Name) * -1;
                    case FileSort.FileDateAscending:
                        return f1.CreationTimeUtc.CompareTo(f2.CreationTimeUtc);
                    case FileSort.FileDateDescending:
                        return f1.CreationTimeUtc.CompareTo(f2.CreationTimeUtc) * -1;
                    case FileSort.FileSizeAscending:
                        return f1.Length.CompareTo(f2.Length);
                    case FileSort.FileSizeDescending:
                        return f1.Length.CompareTo(f2.Length) * -1;
                }
            }
        }
    }
}
