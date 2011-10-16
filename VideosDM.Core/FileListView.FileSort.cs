using System;
using System.Collections.Generic;
using System.Text;

namespace VideosDM.Core
{
    public partial class FileListView
    {
	    public enum FileSort : byte
        {
            AlphabeticalAscending,
            AlphabeticalDescending,
            FileDateAscending,
            FileDateDescending, 
            FileSizeAscending,
            FileSizeDescending
        }
    }
}
