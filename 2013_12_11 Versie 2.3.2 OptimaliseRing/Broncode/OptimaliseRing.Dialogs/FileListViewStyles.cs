using System;
using System.ComponentModel;

namespace ComponentAge.Dialogs
{
    [Description("Specifies how list items are displayed in the file list view control.")]
	public enum FileListViewStyle
	{
        [Description("Dialog component does not change file list view in any way.")]
        Default,
        [Description("Each item appears on a separate line with further information about each item arranged in columns.")]
        Details,
        [Description("Each item appears as a full-sized icon with a label below it.")]
        LargeIcon,
        [Description("Each item appears as a small icon with a label to its right. Items are arranged in columns with no column headers.")]
        List,
        [Description("Each item appears as a small icon with a label to its right.")]
        SmallIcon,
        [Description("Each item appears as a small preview of the file.")]
        Thumbnails,
        [Description("Items are tiled.")]
        Tiles
	}
}
