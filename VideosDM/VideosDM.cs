using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

using NUtility;
using NUtility.Base;
using NUtility.Controls;
using NShared;

using VideosDM.Core;

/*
 * VideosDM Class
 * last updated 16/10/2011
 * By Dan Mannock
 * Private test build
 * 
 * Currently uses the fileListView class to retreiving videos files for display in an npvr plugin for videos.
 * Overrides the NewStyle layout for navigation buttons, file listing, playback
 * 
 */

namespace VideosDM
{
    public class VideosDM : NewStyleButtonListPlugin, IPluginConfiguration, IPluginCallback
    {
        protected FileListView fileListView;
        FileListView.FileListViewDisplay listDisplay = new FileListView.FileListViewDisplay();
        protected List<UiElement> uiElements = new List<UiElement>();
        protected UiStatic sortNameElem;
        protected UiStatic fileNameElem;
        protected int resumeDelay = 0;

        public bool ShowPlaybackStatus { get; protected set; }

        public bool FileListViewLogging { get; protected set; }

        public VideosDM()
        {
            Log.Write("New Instance VideosDM()");
            resumeDelay = SettingsHelper.GetInstance().GetSetting("/Settings/General/PreResumeDelay", 500);
            ShowPlaybackStatus = true;
            FileListViewLogging = true;
            Log.Write("Constructor end");
        }

        /// <summary>
        /// Initialise the file list if not already.
        /// </summary>
        protected void InitialiseFileList()
        {
            string path = Config.ReadConfig().Path;

            try
            {
                fileListView = new FileListView(path);
                fileListView.Logging = FileListViewLogging;
                fileListView.Process();
                SetSortNameText(fileListView.FileSorting.ToString());
                SetFileNameText("");
            }
            catch (DirectoryNotFoundException ex)
            {
                Log.Write(string.Format("Method: Main() {0} Path: {1}", ex.Message, path));
                PluginHelperFactory.GetPluginHelper().ShowMessage("No path has been set. Please go to plugin settings.");
                return;
            }
            Log.Write("Method: InitialiseFileList() : Directory " + path);
        }

        #region NewStyleButtonListPlugin
        protected override void Initialise()
        {
            Log.Write("Method Override: Initialise()");
            base.Initialise();
            Log.Write("Initialise end");
        }

        public override string GetName()
        {
            return "Videos DM";
        }

        public override string GetDescription()
        {
            return "Videos DM Plugin WITH Sorting [test build]";
        }

        /// <summary>
        /// Overriden GetButtonList Method used to return the navigation button actions available
        /// </summary>
        /// <returns>string array of button names</returns>
        protected override string[] GetButtonList()
        {
            return new string[] { "Back", "View", "Sort", "Play All" };
        }

        // supply the skinhelper object on request
        protected override SkinHelper GetSkinHelper()
        {
            return new SkinHelper("VideosDM\\skin.xml");
        }

        /// <summary>
        /// Overridden HandleCommand Method used when a navigation button is activated
        /// </summary>
        /// <param name="command">string name of the button/command</param>
        /// <returns>Bool if an action was completed or not</returns>
        protected override bool HandleCommand(string command)
        {
            Log.Write("Method Override: HandleCommand() : " + command);

            switch (command)
            {
                case "Popup":
                    return true;
                case "View":
                    // toggle list view
                    if (uiList.GetViewMode() == UiList.ViewMode.COVERS)
                        uiList.SetViewMode(UiList.ViewMode.LIST);
                    else
                        uiList.SetViewMode(UiList.ViewMode.COVERS);
                    PopulateList();
                    return true;
                case "Sort":
                    FileListView.FileSortHelper sortHelper = new FileListView.FileSortHelper(fileListView);

                    try
                    {
                        sortHelper.Next();
                        fileListView.Sort(fileListView.FileSorting);
                    }
                    catch (Exception ex)
                    {
                        PluginHelperFactory.GetPluginHelper().ShowMessage(String.Format("Error sorting: {0}\n{1}\n{2}\n{3} " +
                            ex.Message +
                            ex.Source +
                            ex.StackTrace +
                            ex.InnerException));
                    }

                    SetSortNameText(fileListView.FileSorting.ToString());
                    PopulateList();
                    return true;

                case "Play All":
                    PlayVideos();
                    return true;
            }
            return base.HandleCommand(command);
        }

        public override List<UiElement> GetRenderList()
        {
            List<UiElement> renderList = base.GetRenderList();
            //nothing to add on first call of this lifecycle method
            //until 'Activate' is called, then the ui is ready and fileListView is created
            if (this.fileListView != null) 
            {
                renderList.AddRange(sortNameElem.GetRenderList());
                renderList.AddRange(fileNameElem.GetRenderList());
            }
            return renderList;
        }

        public override bool NeedsRendering()
        {
            return base.NeedsRendering() || this.needsRendering;
        }

        protected override void PopulateList()
        {
            //nothing to add on first call of this lifecycle method
            //until 'Activate' is called, then the ui is ready and fileListView is created
            if (this.fileListView == null)
            {
                Log.Write("Method Override: PopulateList() : fileListView is null");
                return;
            }

            List<UiList.ListObject> listObjects = new List<UiList.ListObject>();
            foreach (string s in fileListView.GetList())
            {
                UiList.ListItem item = new UiList.ListItem();
                item["@name"] = s;
                listObjects.Add(item);
            }
            Log.Write("Method Override: PopulateList() : Objects " + listObjects.Count);
            uiList.SetListObjects(listObjects, 0);
        }

        public override void SelectedItem(UiList.ListObject selectedObject)
        {
            base.SelectedItem(selectedObject);

            string selectedItem = selectedObject.properties["@name"].ToString();
            SetFileNameText(listDisplay.IsPrevDirString(selectedItem) ? "" : selectedItem);
        }

        public override void Activate()
        {
            Log.Write("Method Override: Activate()");
            InitialiseFileList();
            this.initialised = false;
            base.Activate();
        }

        /// <summary>
        /// Overriden ActivateItem Method used when an item is selected via double click or enter
        /// </summary>
        /// <param name="selectedObject">the object that has been selected</param>
        public override void ActivateItem(UiList.ListObject selectedObject)
        {
            // list object from double clicking or pressing enter on an item
            string selected = selectedObject["@name"].ToString();
            Log.Write("Method Override: ActivateItem() : SelectedObject " + selected);

            //up a dir level
            if (listDisplay.IsPrevDirString(selected))
            {
                fileListView.PreviousDirectory();
                fileListView.Process();
                PopulateList();
            }
            else if (listDisplay.IsLink(selected))
            {
                selected = listDisplay.FromLink(selected);
                try
                {
                    fileListView.OpenDirectory(selected);
                }
                catch (FileNotFoundException ex)
                {
                    PluginHelperFactory.GetPluginHelper().ShowMessage(ex.Message);
                }
                PopulateList();
            }
            else if (listDisplay.IsDirectory(selected))
            {
                selected = listDisplay.FromDirectory(selected);
                fileListView.OpenDirectory(selected);
                PopulateList();
            }
            else if (selected.Length > 0)
            {
                if (!fileListView.ValidFileExtension(selected))
                {
                    PluginHelperFactory.GetPluginHelper().ShowMessage("Invalid filename or extension.");
                    return;
                }

                string playbackFile = fileListView.GetFullFileName(selected);

                if (playbackFile == null)
                {
                    PluginHelperFactory.GetPluginHelper().ShowMessage("File not found: \r\n" + Path.GetFileNameWithoutExtension(selected));
                    return;
                }

                int playbackDuration;
                int playbackPosition = PlaybackPositionHelper.GetPlaybackPosition(playbackFile, out playbackDuration);

                if (playbackPosition >= 10 && playbackPosition + 10 < playbackDuration)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable["@message"] = "Do you want to resume or restart playback for file: \r\n" + Path.GetFileName(playbackFile);
                    SimpleMessageBox msgBox = new SimpleMessageBox(hashtable, "Resume", "Restart", this);
                    PluginHelperFactory.GetPluginHelper().ActivatePopup(msgBox);
                    return;
                }

                PlayVideo(playbackFile);
            }
        }
        #endregion

        #region video methods
        private void PlayVideo(string filePath, bool resume = false)
        {
            if (!File.Exists(filePath))
            {
                Log.Write("Method: PlayVideo() : File Doesn't Exist " + filePath);
                return;
            }

            Log.Write("Method: PlayVideo() : File " + filePath);
            PluginHelperFactory.GetPluginHelper().PlayVideoFile(filePath);

            if (resume)
            {
                int playbackDuration;
                int playbackPosition = PlaybackPositionHelper.GetPlaybackPosition(filePath, out playbackDuration);

                IPlaybackProxy playbackProxy = PluginHelperFactory.GetPluginHelper().GetPlaybackProxy();
                if (playbackProxy == null)
                    return;

                Log.Write("Method: PlayVideo() : Resume Playback " + playbackPosition);
                playbackProxy.SetPosition((double)playbackPosition);
            }
        }

        private void PlayVideos(List<string> fileNames = null)
        {
            if (fileNames == null)
            {
                fileNames = fileListView.GetFileList();
            }

            List<string> fullFilePaths = new List<string>();
            foreach (string file in fileNames)
            {
                fullFilePaths.Add(fileListView.GetFullFileName(file));
            }

            Log.Write("Method: PlayVideos() : Videos Count " + fullFilePaths.Count);
            PluginHelperFactory.GetPluginHelper().PlayVideoFiles(fullFilePaths);
        }

        private float GetVideoPlaybackPercentage(string filePath)
        {
            if (!File.Exists(filePath))
                return -1F;

            int playbackDuration;
            int playbackPosition = PlaybackPositionHelper.GetPlaybackPosition(filePath, out playbackDuration);


            if (playbackPosition < 10)
                return 0F;

            if (playbackPosition + 10 > playbackDuration)
                return 100F;

            if (playbackDuration <= 0) //divide by zero
                return 0;

            return (playbackPosition / playbackDuration);

        }
        #endregion

        #region additional ui skin methods
        private void SetSortNameText(string text)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add((object)"@message", (object)text);
            sortNameElem = new UiStatic("SortName", hashtable, this.GetSkinHelper());
        }

        private void SetFileNameText(string text)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add((object)"@message", (object)text);
            fileNameElem = new UiStatic("FileName", hashtable, this.GetSkinHelper());
        }
        #endregion

        #region IPluginCallback Members

        public void PluginCallback(object source, string command, object args)
        {
            Log.Write(String.Format("Method: PluginCallback() : source {0} : command {1} : args {2}", source, command, args));

            UiList.ListObject selectedObject = uiList.GetSelectedObject();
            string selected = selectedObject["@name"].ToString();

            if (string.IsNullOrEmpty(selected))
            {
                return;
            }

            if (source is SimpleMessageBox)
            {
                string playbackFile = this.fileListView.GetFullFileName(selected);

                if (command.Equals("Resume"))
                {
                    Log.Write("Method: PluginCallback() : SimpleMessageBox : Resume Playback " +playbackFile );
                    PlayVideo(playbackFile, true);
                    return;
                }
                else if (command.Equals("Restart"))
                {
                    Log.Write("Method: PluginCallback() : SimpleMessageBox : Restart Playback " + playbackFile);
                    PlaybackPositionHelper.DeletePlaybackPosition(playbackFile);
                    PlayVideo(playbackFile);
                    return;
                }


            }
        }

        #endregion

        #region IPluginConfiguration Members

        public SettingsPage GetSettingsPage()
        {
            return new SettingsForm();
        }

        #endregion
    }
}
