using System;
using System.Collections.Generic;
using System.Text;

namespace VideosDM.Core
{
    public partial class FileListView
    {
        public class FileListViewDisplay
        {
            protected readonly string DIRECTORY_PREFIX;
            protected readonly string DIRECTORY_SUFFIX;

            protected readonly string LINK_PREFIX;
            protected readonly string LINK_SUFFIX;

            public readonly string PREVIOUS_DIRECTORY;

            public FileListViewDisplay(string DIR_PREFIX = "[", string DIR_SUFFIX = "]",
                                    string LNK_PREFIX = "[", string LNK_SUFFIX = "*]",
                                    string PREV_DIR = "..")
            {
                this.DIRECTORY_PREFIX = DIR_PREFIX;
                this.DIRECTORY_SUFFIX = DIR_SUFFIX;
                this.LINK_PREFIX = LNK_PREFIX;
                this.LINK_SUFFIX = LNK_SUFFIX;
                this.PREVIOUS_DIRECTORY = PREV_DIR;
            }

            public bool IsDirectory(string str)
            {
                return (str.StartsWith(DIRECTORY_PREFIX) && str.EndsWith(DIRECTORY_SUFFIX));
            }

            public string FromDirectory(string str)
            {
                return (IsDirectory(str) ?
                    str.TrimStart(DIRECTORY_PREFIX.ToCharArray()).TrimEnd(DIRECTORY_SUFFIX.ToCharArray()) :
                    str);
            }

            public string ToDirectory(string str)
            {
                return (IsDirectory(str) ? str : DIRECTORY_PREFIX + str + DIRECTORY_SUFFIX);
            }

            public bool IsLink(string str)
            {
                return (str.StartsWith(LINK_PREFIX) && str.EndsWith(LINK_SUFFIX));
            }

            public string FromLink(string str)
            {
                return (IsLink(str) ?
                    str.TrimStart(LINK_PREFIX.ToCharArray()).TrimEnd(LINK_SUFFIX.ToCharArray()) :
                    str);
            }

            public string ToLink(string str)
            {
                return (IsLink(str) ? str : LINK_PREFIX + str + LINK_SUFFIX);
            }

            public bool IsPrevDirString(string str)
            {
                return (str.Equals(PREVIOUS_DIRECTORY));
            }
        }
    }
}
