using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/*
 * FileListView Class
 * last updated 16/10/2011
 * By Dan Mannock
 * Private test build
 * 
 * Currently works on a directory retreiving file info for display in an npvr plugin for videos.
 * Allows sorting which isn't available in the built-in video browser
 * Limits files by extension (currently set for videos for testing)
 * 
 */

namespace VideosDM.Core
{
    public partial class FileListView
    {
        protected List<DirectoryInfo> dirsList = new List<DirectoryInfo>();
        protected List<FileInfo> linkList = new List<FileInfo>();
        protected List<FileInfo> filesList = new List<FileInfo>();
        protected Stack<string> directoryStack = new Stack<string>();
        protected List<string> validExtensions = new List<string> { ".avi", ".flv", ".m4v", ".mov", ".mp4", ".mpeg", ".mpg", ".mkv", ".ts" };
        protected FileListViewDisplay dispFormat = new FileListViewDisplay();
        protected string dir;
        protected FileSort sorting = FileSort.AlphabeticalAscending;

        #region props
        public string WorkingDirectory
        {
            get { return dir; }
            protected set {
                if (Directory.Exists(value))
                    dir = value;
                else
                    throw new DirectoryNotFoundException();
            }
        }

        public FileSort FileSorting
        {
            get { return sorting; }
            set { sorting = value; }
        }

        public bool HasProcessed
        {
            get { return (HasDirs || HasFiles || HasLinks); }
        }

        public bool HasDirs
        {
            get { return (dirsList != null && dirsList.Count > 0); }
        }

        public bool HasLinks
        {
            get { return (linkList != null && linkList.Count > 0); }
        }

        public bool HasFiles
        {
            get { return (filesList != null && filesList.Count > 0); }
        }

        public bool IsRoot
        {
            get { return Path.GetPathRoot(dir).Equals(dir, StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool HasPrevDirectory
        {
            get 
            {
                return (this.directoryStack != null && this.directoryStack.Count > 0) || (!this.IsRoot && !this.LockRoot);
            }
        }

        public FileListViewDisplay DisplayFormat
        {
            get { return this.dispFormat; }
            set { this.dispFormat = value; }
        }

        //don't allow going back to directories above root
        public bool LockRoot { get; set; }
        public bool Logging { get; set; }

        /// <summary>
        /// [obsolete]
        /// public VaidExpressions Property
        /// </summary>
        /// <remarks>
        /// Over complicated property for adding to the valid extensions, 
        /// is currently used to return the list of valid extension types
        /// throws warnings due to obsolete attribute to remind that this needs work
        /// </remarks>
        [Obsolete("To be replaced")]
        public List<string> ValidExtensions
        {
            set {
                if (value.Count == 0)
                    return;

                foreach (string s in value)
                {
                    s.TrimStart('.');

                    if (String.IsNullOrEmpty(s) || s.Length > 5)
                        return;

                    if (s.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '\v' }) != -1)
                        return;

                    if  (s.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                        return;

                    if (validExtensions.Contains(s))
                        return;

                    validExtensions.Add(s);
                }
            }

            get { return validExtensions; }
        }
        #endregion

        /// <summary>
        /// FileListView Constructor
        /// </summary>
        /// <param name="directory">base directory for filelistview to process</param>
        public FileListView(string directory)
        {
            //set dir first to exit execution if exception is thrown
            WorkingDirectory = directory;
            LockRoot = true;
        }

        #region sorting
        /// <summary>
        /// Sort Method delegates sorting of the files info depending on the sort option selected
        /// </summary>
        /// <param name="sortBy">Enum of sort options available</param>
        public void Sort(FileSort sortBy)
        {
            if (!HasProcessed)
                return;

            FileListViewComparer comparer = new FileListViewComparer(sortBy);
            filesList.Sort(comparer);
            this.FileSorting = sortBy;
        }

        public void Sort()
        {
            Sort(this.FileSorting);
        }
        #endregion

        #region file navigation
        /// <summary>
        /// Process Method builds the file info for the working directory
        /// </summary>
        public void Process()
        {
            if (!Directory.Exists(dir))
                InitalDirectory();

            ResetLists();

            foreach (string directory in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
            {
                DirectoryInfo di = new DirectoryInfo(directory);

                if ((di.Attributes & (FileAttributes.System | FileAttributes.Hidden)) == (FileAttributes)0)
                    dirsList.Add(new DirectoryInfo(directory));
            }

            foreach (string link in Directory.GetFiles(dir, "*.lnk", SearchOption.TopDirectoryOnly))
            {
                if (LinkPathExists(link))
                    linkList.Add(new FileInfo(link));
            }

            foreach (string file in Directory.GetFiles(dir))
            {
                if (validExtensions.Contains(Path.GetExtension(file)))
                    filesList.Add(new FileInfo(file));
            }

            Sort();
        }

        /// <summary>
        /// resets to the initial directory
        /// </summary>
        public void InitalDirectory()
        {
            //prevent infinite loop when Process is called
            //and the initial working dir has been deleted
            try
            {
                this.WorkingDirectory = dir;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }

            this.directoryStack = null;
            Process();
        }

        public void PreviousDirectory()
        {
            if (this.directoryStack != null && this.directoryStack.Count > 0)
            {
                this.WorkingDirectory = this.directoryStack.Pop();
            }
            else if (!IsRoot && !this.LockRoot)
            {
                this.WorkingDirectory = Directory.GetParent(this.WorkingDirectory).FullName;
            }
            ResetLists();
        }

        /// <summary>
        /// opens a directory in the current directory list
        /// </summary>
        /// <param name="directoryName"></param>
        public void OpenDirectory(string directoryName)
        {
            string tmp = GetLinkPath(directoryName);

            if (tmp == null)
                tmp = GetDirPath(directoryName);

            if (tmp != null)
            {
                this.directoryStack.Push(this.WorkingDirectory);
                this.WorkingDirectory = tmp;
                Process();
            }
        }
        #endregion

        private void ResetLists()
        {
            dirsList.Clear();
            linkList.Clear();
            filesList.Clear();
        }

        /// <summary>
        /// GetList Method Returns a list of filename strings
        /// </summary>
        /// <returns>String list of file names</returns>
        public List<string> GetList()
        {
            List<string> list = new List<string>();

            if (HasPrevDirectory)
                list.Add(this.DisplayFormat.PREVIOUS_DIRECTORY);

            foreach (DirectoryInfo di in dirsList)
            {
                list.Add(this.DisplayFormat.ToDirectory(di.Name));
                
            }

            foreach (FileInfo li in linkList)
            {
                list.Add(this.DisplayFormat.ToLink(li.Name));
            }

            foreach (FileInfo fi in filesList)
            {
                list.Add(fi.Name);
            }

            return list;
        }

        public List<string> GetFileList()
        {
            List<string> list = new List<string>();

            foreach (FileInfo fi in filesList)
                list.Add(fi.Name);

            return list;
        }

        #region file name/path methods
        /// <summary>
        /// GetFullPath Method Returns the full path from a file name only
        /// </summary>
        /// <param name="name">takes 1 string parameter of the file name including extension</param>
        /// <returns>String of the full file path</returns>
        public string GetFullFileName(string name)
        {
            if (!HasProcessed)
                return null;

            foreach (FileInfo file in filesList)
            {
                if (file.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) && file.Exists)
                {
                    return file.FullName;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks to see if the file extansion is in the valid extensions collection
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool ValidFileExtension(string fileName)
        {
            return validExtensions.Contains(Path.GetExtension(fileName));
        }

        public bool LinkPathExists(string fullName)
        {
            ShellLink.ShellShortcut shortcut = new ShellLink.ShellShortcut(fullName);
            if (shortcut == null || shortcut.Path == null || shortcut.ShellLink == null)
                return false;
            return (Directory.Exists(shortcut.Path));
        }

        public string GetLinkPath(string name)
        {
            if (!HasProcessed)
                return null;

            foreach (FileInfo link in linkList)
            {
                if (link.Exists && link.Name == name)
                {
                    ShellLink.ShellShortcut shortcut = new ShellLink.ShellShortcut(link.FullName);
                    if (Directory.Exists(shortcut.Path))
                    {
                        return shortcut.Path;
                    }
                    else
                    {
                        throw new FileNotFoundException("Linked file does not exist: " + link.FullName);
                    }
                }
            }
            return null;
        }

        public string GetDirPath(string name)
        {
            if (!HasProcessed)
                return null;

            foreach (DirectoryInfo d in dirsList)
            {
                if (d.Exists && d.Name == name)
                {
                    return d.FullName;
                }
            }
            return null;
        }
        #endregion
    }
}
