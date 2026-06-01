//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) 2002 - 2004, COMPONENTAGE Software,
// all rights reserved
//
// http://www.componentage.com
// support@componentage.com
//
// All files included into Dialog Workshop.NET source code package,
// remain COMPONENTAGE's exclusive property. Regardless of
// any modifications that you make, you may not distribute
// or publish any source code files.
//---------------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace ComponentAge.Dialogs
{
   /// <summary>
   /// Summary description for DialogConsts.
   /// </summary>
   internal class DialogConsts
   {
      internal const int idUpOneLevel = 40961;
      internal const int idCreateFolder = 40962;
      internal const int idListView = 40963;
      internal const int idDetailsView = 40964;
      internal const int idShowDesktop = 40969;
      internal const int idLastVisitedFolder = 40971;

      internal const string strTemplateDesigner = "Dialog Designer";
      internal const string strPreview = "Preview Dialog";
      internal const string strPreview2 = "Preview Dialog (safe mode)";

      internal const string strAssemblyName = "ComponentAge.Dialogs";

      // Members with identical name(s)
      public const string strPropDesc_DlgItemsCaptions = "Defines custom captions of dialog items (controls).";
      public const string strPropDesc_ActiveDialog = "At run-time, returns currently displayed dialog component.";
      public const string strCtorDesc_DlgConstructor = "Initializes a new instance of the dialog class.";

      // CaCustDlgParams
      //
      public const string strCaCustDlgParams_TypeDesc = "This type contains properties defining dialog's docked forms, startup position, etc.";
      public const string strCaCustDlgParams_StartPosition = "Gets or sets the initial position of the dialog box.";
      public const string strCaCustDlgParams_BottomFormClassName = "Class name of a form which will be instatiated as bottom docked part of the dialog box.";
      public const string strCaCustDlgParams_TopFormClassName = "Class name of a form which will be instatiated as top docked part of the dialog box.";
      public const string strCaCustDlgParams_RightFormClassName = "Class name of a form which will be instatiated as right docked part of the dialog box.";
      public const string strCaCustDlgParams_LeftFormClassName = "Class name of a form which will be instatiated as left docked part of the dialog box.";
      public const string strCaCustDlgParams_BottomForm = "At run-time, gets or sets a form which is used as bottom docked part of the dialog box.";
      public const string strCaCustDlgParams_TopForm = "At run-time, gets or sets a form which is used as top docked part of the dialog box.";
      public const string strCaCustDlgParams_RightForm = "At run-time, gets or sets a form which is used as right docked part of the dialog box.";
      public const string strCaCustDlgParams_LeftForm = "At run-time, gets or sets a form which is used as left docked part of the dialog box.";
      public const string strCaCustDlgParams_PosParams = "Defines additional params which affect the position of the dialog box.";
      public const string strCaCustDlgParams_AutoCreateDockedForms = "If true, docked forms are created automatically according to a form class name.";
      public const string strCaCustDlgParams_Icon = "Gets or sets the icon for the dialog box.";
      public const string strCaCustDlgParams_Size = "Gets or sets the height and width of the dialog window.";
      public const string strCaCustDlgParams_Location = "Gets or sets the coordinates of the upper-left corner of the control relative to the upper-left corner of the Windows Desktop window.";

      // CaDlgPosParams
      //
      public const string strCaDlgPosParams_TypeDesc = "Contains properties defining additional dialog position parameters.";
      public const string strCaDlgPosParams_ShiftY = "Defines vertical adjustment of the dialog box from it's default start position.";
      public const string strCaDlgPosParams_ShiftX = "Defines horizontal adjustment of the dialog box from it's default start position.";
      public const string strCaDlgPosParams_FitToScreen = "If true, dialog box position is adjusted to display dialog window within desktop area.";
      public const string strCaDlgPosParams_CustomLocation = "If CustDlgParams.StartPosition = Custom, specifies initial dialog position in screen coordinates.";

      // CommandEventArgs
      //
      public const string strCommandEventArgs_TypeDesc = "Provides data for the Command event.";
      public const string strCommandEventArgs_Ctor = "Initializes a new instance of the CommandEventArgs class.";
      public const string strCommandEventArgs_CommandID = "Gets the CommandID associated with this command.";

      // CommandCancelEventArgs
      //
      public const string strCommandCancelEventArgs_TypeDesc = "Provides data for a cancelable event.";
      public const string strCommandCancelEventArgs_Ctor = "Initializes a new instance of the CommandCancelEventArgs class.";
      public const string strCommandCancelEventArgs_Cancel = "Gets or sets a value indicating whether the event should be canceled.";

      public const string strCommandCancelEventHandler_TypeDesc = "Represents the method that handles a cancellable command event.";
      public const string strCommandEventHandler_TypeDesc = "Represents the method that handles a command event.";

      // CaDlgItemCaptions
      public const string strCaDlgItemCaptions_TypeDesc = "Specifies custom captions for standard dialog items (labels, buttons, etc.).";
      public const string strCaDlgItemCaptions_OK = "Gets or sets custom caption for OK button.";
      public const string strCaDlgItemCaptions_Cancel = "Gets or sets custom caption for Cancel button.";
      public const string strCaDlgItemCaptions_Help = "Gets or sets custom caption for Help button.";

      // CaCommonDialog
      public const string strCaCommonDialog_TypeDesc = "Specifies the base class used for displaying dialog boxes on the screen.";
      public const string strCaCommonDialog_Ctor = "Initializes a new instance of the CaCommonDialog class.";
      public const string strCaCommonDialog_Show = "Occurs when the dialog opens.";
      public const string strCaCommonDialog_Hide = "Occurs when the dialog closes.";
      public const string strCaCommonDialog_CustDlgParams = "This property exists in all Dialog Workshop components. It contains many nested properties specifying docked forms, initial dialog position, etc.";
      public const string strCaCommonDialog_Title = "Specifies the text in the dialog's title bar.";
      public const string strCaCommonDialog_ActualTitle = "Returns actual text displayed in the dialog's title bar. If Title property is empty, returns system-provided caption.";
      public const string strCaCommonDialog_Handle = "At run-time, gets the handle for this dialog box.";
      public const string strCaCommonDialog_Reset = "Resets all dialog box options to their default values.";
      public const string strCaCommonDialog_ShowHelp = "Gets or sets a value indicating whether a Help button appears in the dialog box.";
      public const string strCaCommonDialog_CloseDialog = "Programmatically closes the dialog box.";
      public const string strCaCommonDialog_Resize = "Occurs when the dialog is resized.";
      public const string strCaCommonDialog_CancelDialog = "Occurs immediately before dialog closes after user presses Esc key or Cancel button.";
      public const string strCaCommonDialog_AcceptDialog = "Occurs immediatelly after user presses the OK button in the dialog box.";

      // CaFileDialogBase
      public const string strCaFileDialogBase_TypeDesc = "Displays a dialog box from which the user can select a file. Use inherited Open/Save dialogs to open or save a file in your application.";
      public const string strCaFileDialogBase_RunDialog = "This member overrides CaCommonDialog.RunDialog.";
      public const string strCaFileDialogBase_Refresh = "Refreshes the contents on the file list view control.";
      public const string strCaFileDialogBase_EnableToolButton = "Enables/disables standard toolbuttons in the dialog box.";
      public const string strCaFileDialogBase_ShowControl = "Shows/hides standard controls in the dialog box.";
      public const string strCaFileDialogBase_FolderChanged = "Occurs when currently selected folder has changed.";
      public const string strCaFileDialogBase_TypeChanged = "Occurs when currently selected file filter has changed.";
      public const string strCaFileDialogBase_SelectionChanged = "Occurs when the selection state of any item or items in the file list view has changed.";
      public const string strCaFileDialogBase_RecentSelectionChanged = "Occurs when an item in the the MRU list selected or changed.";
      public const string strCaFileDialogBase_FilenameTextChanged = "Occurs when the current file name (specified in the drop-down combo box that displays the name of the current file) changes.";
      public const string strCaFileDialogBase_FileOk = "Occurs when the user clicks on the Open or Save button on a file dialog box.";
      public const string strCaFileDialogBase_UpLevelNavigated = "Occurs when the user press 'Up one level' button on the standard toolbar.";
      public const string strCaFileDialogBase_UpLevelNavigating = "Occurs when the user is about to press 'Up one level' button on the standard toolbar.";
      public const string strCaFileDialogBase_LastVisitedFolderNavigated = "Occurs when the user press 'Goto last visited folder' button on the standard toolbar.";
      public const string strCaFileDialogBase_LastVisitedFolderNavigating = "Occurs when the user is about press 'Goto last visited folder' button on the standard toolbar.";
      public const string strCaFileDialogBase_NewFolderCreated = "Occurs when the user press 'New Folder' button on the standard toolbar.";
      public const string strCaFileDialogBase_NewFolderCreating = "Occurs when the user press 'New Folder' button on the standard toolbar.";
      public const string strCaFileDialogBase_DesktopNavigated = "Occurs when the user press 'Desktop' button on the standard toolbar.";
      public const string strCaFileDialogBase_DesktopNavigating = "Occurs when the user is about to press 'Desktop' button on the standard toolbar.";
      public const string strCaFileDialogBase_PlacesBarClicked = "Occurs when the user press a button on the PlacesBar.";
      public const string strCaFileDialogBase_PlacesBarClicking = "Occurs when the user is about to press a button on the PlacesBar.";
      public const string strCaFileDialogBase_ValidateNames = "Gets or sets a value indicating whether the dialog box accepts only valid Win32 file names.";
      public const string strCaFileDialogBase_ShowHelp = "Gets or sets a value indicating whether the Help button is displayed in the file dialog.";
      public const string strCaFileDialogBase_RestoreDirectory = "Gets or sets a value indicating whether the dialog box restores the current directory before closing.";
      public const string strCaFileDialogBase_AddExtension = "Gets or sets a value indicating whether the dialog box automatically adds an extension to a file name if the user omits the extension.";
      public const string strCaFileDialogBase_CheckFileExists = "Gets or sets a value indicating whether the dialog box displays a warning if the user specifies a file name that does not exist.";
      public const string strCaFileDialogBase_CheckPathExists = "Gets or sets a value indicating whether the dialog box displays a warning if the user specifies a path that does not exist.";
      public const string strCaFileDialogBase_DefaultExt = "Gets or sets the default file name extension.";
      public const string strCaFileDialogBase_DereferenceLinks = "Gets or sets a value indicating whether the dialog box returns the location of the file referenced by the shortcut or whether it returns the location of the shortcut (.lnk).";
      public const string strCaFileDialogBase_FileName = "Gets or sets a string containing the file name selected in the file dialog box.";
      public const string strCaFileDialogBase_FileNames = "Gets the file names of all selected files in the dialog box.";
      public const string strCaFileDialogBase_Filter = "Gets or sets the current file name filter string, which determines the choices that appear in the 'Save as file type' or 'Files of type' box in the dialog box.";
      public const string strCaFileDialogBase_FilterIndex = "Gets or sets the index of the filter currently selected in the file dialog box.";
      public const string strCaFileDialogBase_InitialDirectory = "Gets or sets the initial directory displayed by the file dialog box.";
      public const string strCaFileDialogBase_MultiSelect = "If true, allow multiple files to be selected.";
      public const string strCaFileDialogBase_Title = "Gets or sets the file dialog box title.";
      public const string strCaFileDialogBase_FilenameText = "Gets or sets the text displayed in the filename text box.";
      public const string strCaFileDialogBase_ListViewOptions = "Additional file list view options.";
      public const string strCaFileDialogBase_DontAddToRecent = "Prevents the system from adding a link to the selected file in the file system directory that contains the user's most recently used documents(Win2000/XP).";
      public const string strCaFileDialogBase_PlacesBar = "Defines PlacesBar properties.";
      public const string strCaFileDialogBase_SelectedFile = "Gets or sets currently selected file when the dialog box is displayed.";
      public const string strCaFileDialogBase_SelectedFiles = "Gets or sets currently selected files when the dialog box is displayed.";
      public const string strCaFileDialogBase_SelectedFolder = "Gets or sets currently selected folder when the dialog box is displayed.";
      public const string strCaFileDialogBase_GetSelectedFolderAs = "Retrieves the display name for the selected folder (in different formats).";
      public const string strCaFileDialogBase_SelectedSpecialFolder = "Gets or sets currently selected folder as special shell folder.";
      public const string strCaFileDialogBase_CollectGarbage = "If true, GC.Collect is calling after showing dialog to destroy all structures.";
      public const string strCaFileDialogBase_ShowRecentFilesList = "If true, Recent tab is added in the dialog box wich allow to display the list of recently used files.";
      public const string strCaFileDialogBase_RecentFilesListCount = "Gets or sets the number of files displayed on the Recent tab.";
      public const string strCaFileDialogBase_RecentFilesListRegKey = "Gets or sets the registry key path (under HKEY_CURRENT_USER) where Recent files list is stored.";
      public const string strCaFileDialogBase_SelectedTabIndex = "Gets or sets the index of the selected tab in the Browse/Recent tab control displayed if ShowRecentFiles property is true.";

      // CaOpenFileDialog
      public const string strCaOpenFileDialog_TypeDesc = "Represents a common dialog box that displays the control that allows the user to open a file.";
      public const string strCaOpenFileDialog_ShowReadOnly = "Gets or sets a value indicating whether the dialog box contains a read-only check box.";
      public const string strCaOpenFileDialog_ReadOnlyChecked = "Gets or sets a value indicating whether the read-only check box is selected.";
      public const string strCaOpenFileDialog_OpenFile = "Opens the file selected by the user, with read-only permission. The file is specified by the FileName property.";

      // CaSaveFileDialog
      public const string strCaSaveFileDialog_TypeDesc = "Represents a common dialog box that allows the user to specify options for saving a file.";
      public const string strCaSaveFileDialog_CreatePrompt = "Gets or sets a value indicating whether the dialog box prompts the user for permission to create a file if the user specifies a file that does not exist.";
      public const string strCaSaveFileDialog_OverwritePrompt = "Gets or sets a value indicating whether the Save As dialog box displays a warning if the user specifies a file name that already exists.";
      public const string strCaSaveFileDialog_OpenFile = "Opens the file with read/write permission selected by the user.";

      // CaFileDialogItemCaptions
      public const string strCaFileDialogItemCaptions_TypeDesc = "Contains custom texts for a file selection dialog controls.";
      public const string strCaFileDialogItemCaptions_LookIn = "Custom text for the label for the drop-down combo box that displays the current drive or folder, and that allows the user to select a drive or folder to open.";
      public const string strCaFileDialogItemCaptions_ReadOnly = "Custom text for the read-only check box";
      public const string strCaFileDialogItemCaptions_FilesOfType = "Custom text for the label for the drop-down combo box that displays the list of file type filters";
      public const string strCaFileDialogItemCaptions_FileName = "Custom text for the label for the drop-down combo box that displays the name of the current file.";

      // DisplayNameRequestFlags
      public const string strDisplayNameRequestFlags_TypeDesc = "Specifies flags used to retrieve different representations of folders/files.";
      public const string strDisplayNameRequestFlags_Normal = "Full name. The name is relative to the desktop and not to a specific folder. This name is used for generic display.";
      public const string strDisplayNameRequestFlags_InFolder = "Relative name. The name is relative to the folder that is processing it.";
      public const string strDisplayNameRequestFlags_ForEditing = "The name is used for in-place editing when the user renames the item.";
      public const string strDisplayNameRequestFlags_ForAddressBar = "The name is displayed in an address bar combo box.";
      public const string strDisplayNameRequestFlags_ForParsing = "The name is used for parsing. That is, it can be passed to ParseDisplayName to recover the object's PIDL. The form this name takes depends on the particular object.";

      // FileDialogToolButton
      public const string strFileDialogToolButtonEnum_TypeDesc = "This enumeration is used in CaFileDialogBase.EnableToolButton method to enable/disable the appropriate tool button.";

      // FileDialogControl
      public const string strFileDialogControl_TypeDesc = "This enumeration is used in CaFileDialogBase.ShowControl to show/hide the appropriate dialog item.";

      // CaPlacesBarOptions
      public const string strCaPlacesBarOptions_TypeDesc = "This type determines the behavior of a file dialog's PlacesBar.";
      public const string strCaPlacesBarOptions_Visible = "If True, PlacesBar is visible (under Win2000/ME or later).";
      public const string strCaPlacesBarOptions_Captions = "Contains custom captions for PlacesBar items.";

      // CaListViewOptions
      public const string strCaListViewOptions_TypeDesc = "This type determines the behaviour of a dialog's file list view control.";
      public const string strCaListViewOptions_Enabled = "If True, custom list view options specified in ListViewOptions property, have effect. If False, default system settings are used.";
      public const string strCaListViewOptions_GridLines = "If True, grid lines are visible in Report mode.";
      public const string strCaListViewOptions_FlatScrollBars = "If True, flat scrolll bars are used in file list view control.";
      public const string strCaListViewOptions_FullRowSelect = "If True, full row is selected in Report mode";
      public const string strCaListViewOptions_EnableRename = "If True, end user is able to remane files in the file list view control (using F2 key, single mouse click or context menu).";
      public const string strCaListViewOptions_EnableDelete = "If True, end user is able to remane files in the file list view control (using Del key or context menu).";
      public const string strCaListViewOptions_UnderlineCold = "If True, an underline is displayed under all untracked items.";
      public const string strCaListViewOptions_UnderlineHot = "If True, an underline is displayed under the tracked item.";
      public const string strCaListViewOptions_HandPoint = "If True, the mouse cursor turns into a hand.";
      public const string strCaListViewOptions_HotTrack = "Specifies whether list items are highlighted when the mouse passes over them.";
      public const string strCaListViewOptions_PopupOnEmpty = "If True, system shell context menu can be displayed when there is no selection in the file list view.";
      public const string strCaListViewOptions_PopupOnSelection = "If True, system shell context menu can be displayed when there is a selection in the file list view.";

      // DialogPosition enum
      public const string strDialogPosition_TypeDesc = "Specifies the initial position of a dialog box.";

      // CaPrintDialogItemCaptions
      public const string strCaPrintDialogItemCaptions_TypeDesc = "Contains custom texts for print dialog box controls.";
      public const string strCaPrintDialogItemCaptions_GroupPrinter = "Contains custom text for Printer group box.";
      public const string strCaPrintDialogItemCaptions_GroupRange = "Contains custom text for Range group box.";
      public const string strCaPrintDialogItemCaptions_GroupCopies = "Contains custom text for Copies group box.";
      public const string strCaPrintDialogItemCaptions_PrnName = "Contains custom text for Printer's name combo box.";
      public const string strCaPrintDialogItemCaptions_PrnType = "Contains custom text for Printer's type label.";
      public const string strCaPrintDialogItemCaptions_PrnStatus = "Contains custom text for Printer's status label.";
      public const string strCaPrintDialogItemCaptions_PrnWhere = "Contains custom text for Printer's Where label.";
      public const string strCaPrintDialogItemCaptions_PrnComment = "Contains custom text for Printer's Comment label.";
      public const string strCaPrintDialogItemCaptions_PrintToFile = "Contains custom text for 'Print to File' checkbox.";
      public const string strCaPrintDialogItemCaptions_RngAll = "Contains custom text for 'All' label (in Range group).";
      public const string strCaPrintDialogItemCaptions_RngPages = "Contains custom text for 'Pages' label (in Range group).";
      public const string strCaPrintDialogItemCaptions_RngPagesFrom = "Contains custom text for 'From' label (in Range group).";
      public const string strCaPrintDialogItemCaptions_RngPagesTo = "Contains custom text for 'To' label (in Range group).";
      public const string strCaPrintDialogItemCaptions_RngSelection = "Contains custom text for Selection's label.";
      public const string strCaPrintDialogItemCaptions_CopNumbers = "Contains custom text for 'Number of Copies' label.";
      public const string strCaPrintDialogItemCaptions_CopCollate = "Contains custom text for Collate checkbox.";

      // CaPrintDialog
      public const string strCaPrintDialog_TypeDesc = "Allows users to select a printer and choose which portions of the document to print.";
      public const string strCaPrintDialog_RunDialog = "This member overrides CaCommonDialog.RunDialog.";
      public const string strCaPrintDialog_SelectedPrinter = "Returns currently selected printer at run-time.";
      public const string strCaPrintDialog_AllowPrintToFile = "Gets or sets a value indicating whether the Print to file check box is enabled.";
      public const string strCaPrintDialog_AllowSelection = "Gets or sets a value indicating whether the Selection option button is enabled.";
      public const string strCaPrintDialog_AllowSomePages = "Gets or sets a value indicating whether the Pages option button is enabled.";
      public const string strCaPrintDialog_Document = "Gets or sets a value indicating the PrintDocument used to obtain PrinterSettings.";
      public const string strCaPrintDialog_PrinterSettings = "Specifies information about how a document is printed, including the printer that prints it.";
      public const string strCaPrintDialog_PrintToFile = "Gets or sets a value indicating whether the Print to file check box is checked.";
      public const string strCaPrintDialog_ShowNetwork = "Gets or sets a value indicating whether the Network button is displayed.";
      public const string strCaPrintDialog_PrinterOk = "Occurs when the user clicks on the Print button on a print dialog box.";
      public const string strCaPrintDialog_PrinterChanged = "Occurs when printer selected in the dialog is changed.";
      public const string strCaPrintDialog_BeforePrinterSetup = "Occurs before the user presses Properties button for selected printer.";
      public const string strCaPrintDialog_AfterPrinterSetup = "Occurs immediatelly after the Printer Properties dialog is closed.";

      // CaAboutDialogItemCaptions
      public const string strCaAboutDialogItemCaptions_TypeDesc = "Contains custom texts for about dialog box controls.";

      // CaAboutDialog
      public const string strCaAboutDialog_TypeDesc = "Shows standard shell's about dialog box.";
      public const string strCaAboutDialog_Description = "Contains text that will be displayed in the dialog box after the version and copyright information.";
      public const string strCaAboutDialog_FirstLineText = "Contains text that will be displayed on the first line after the text 'Microsoft'.";
      public const string strCaAboutDialog_Icon = "Icon displayed in the dialog box. If this parameter is empty, the Microsoft® Windows® or Windows NT® icon is displayed.";

      // CaColorDialogItemCaptions
      public const string strCaColorDialogItemCaptions_TypeDesc = "Contains custom texts for color dialog box controls.";
      public const string strCaColorDialogItemCaptions_DefineCustomColors = "Contains custom text for 'Define custom colors' button.";
      public const string strCaColorDialogItemCaptions_AddToCustomColors = "Contains custom text for 'Add to custom colors' button.";
      public const string strCaColorDialogItemCaptions_Color = "Contains custom text for 'Color' label.";
      public const string strCaColorDialogItemCaptions_Solid = "Contains custom text for 'Solid' label.";
      public const string strCaColorDialogItemCaptions_Hue = "Contains custom text for 'Hue' label.";
      public const string strCaColorDialogItemCaptions_Sat = "Contains custom text for 'Sat' label.";
      public const string strCaColorDialogItemCaptions_Lum = "Contains custom text for 'Lum' label.";
      public const string strCaColorDialogItemCaptions_Red = "Contains custom text for 'Red' label.";
      public const string strCaColorDialogItemCaptions_Green = "Contains custom text for 'Green' label.";
      public const string strCaColorDialogItemCaptions_Blue = "Contains custom text for 'Blue' label.";

      // CaColorDialog
      public const string strCaColorDialog_TypeDesc = "Represents a common dialog box that displays available colors along with controls that allow the user to define custom colors.";

      // CaFolderDialog
      public const string strCaFolderDialog_TypeDesc = "Represents a common dialog box that displays the control that allows the user to open a file or directory.";
      public const string strCaFolderDialog_ShowEditBox = "Include an edit control in the browse dialog box that allows the user to type the name of an item.";
      public const string strCaFolderDialog_ReturnComputers = "Only return computers. If the user selects anything other than a computer, the OK button is grayed.";
      public const string strCaFolderDialog_ReturnPrinters = "Only return printers. If the user selects anything other than a printer, the OK button is grayed.";
      public const string strCaFolderDialog_DontGoBelowDomain = "Do not include network folders below the domain level in the dialog box's tree view control.";
      public const string strCaFolderDialog_ShowFiles = "If true, the browse dialog box will display files as well as folders.";
      public const string strCaFolderDialog_ReturnOnlyFSDirs = "Only return file system directories.";
      public const string strCaFolderDialog_ReturnFSAncestors = "Only return file system ancestors.";
      public const string strCaFolderDialog_SpecRootFolder = "Specifies the location of the root folder from which to start browsing. Has no effect if RootFolder is not empty.";
      public const string strCaFolderDialog_RootFolder = "Specifies the location of the root folder from which to start browsing.";
      public const string strCaFolderDialog_StatusText = "String that is displayed above the tree view control in the dialog box. This string can be used to specify instructions to the user.";
      public const string strCaFolderDialog_FullFolderName = "Gets a string containing the full folder name(path) selected in the dialog box.";
      public const string strCaFolderDialog_DisplayName = "Gets a string containing the display name(short name) selected in the dialog box.";
      public const string strCaFolderDialog_SelectedFolder = "Gets or sets currently selected folder or file when the dialog box is displayed.";
      public const string strCaFolderDialog_SelectionChanged = "Gets or sets currently selected folder or file when the dialog box is displayed.";
      public const string strCaFolderDialog_FolderOk = "Gets or sets currently selected folder or file when the dialog box is displayed.";
      public const string strCaFolderDialog_NewUserInterface = "If True, the new user interface is used (requires SHELL.DLL version 5.0).";
      public const string strCaFolderDialog_DlgItemsCaptions = "Defines custom captions of dialog items (controls).";
      // FolderDialogControl
      public const string strFolderDialogControl_TypeDesc = "This enumeration is used in CaFolderDialog.ShowControl to show/hide the appropriate dialog item(s).";

      // CaRunDialogItemCaptions
      public const string strCaRunDialogItemCaptions_TypeDesc = "Contains custom texts for run dialog box controls.";
      public const string strCaRunDialogItemCaptions_Browse = "Contains custom texts for 'Browse' button.";

      // CaRunDialog
      public const string strCaRunDialog_TypeDesc = "Represents a common dialog box that displays the control that allows the user to run a program.";
      public const string strCaRunDialog_CalcDir = "If true, the working directory is calculated from the file name.";
      public const string strCaRunDialog_Description = "Gets or sets a string displayed in the dialog box.";
      public const string strCaRunDialog_Icon = "Gets or sets an icon displayed in the dialog box.";
      public const string strCaRunDialog_InitialDirectory = "Specifies the working directory.";
      public const string strCaRunDialog_ShowBrowseButton = "If true, Browse button is visible.";
      public const string strCaRunDialog_ShowLabel = "If true, Edit box label is visible.";
      public const string strCaRunDialog_ShowDefaultItem = "If true, default selection is displayed in the Browse dialog.";

      // CaFontDialogItemCaptions
      public const string strCaFontDialogItemCaptions_TypeDesc = "Contains custom texts for font dialog box controls.";
      public const string strCaFontDialogItemCaptions_Font = "Contains custom text for 'Font' label.";
      public const string strCaFontDialogItemCaptions_FontStyle = "Contains custom text for 'Font Style' label.";
      public const string strCaFontDialogItemCaptions_Size = "Contains custom text for 'Size' label.";
      public const string strCaFontDialogItemCaptions_Effects = "Contains custom text for 'Effects' group box.";
      public const string strCaFontDialogItemCaptions_Sample = "Contains custom text for 'Sample' group box.";
      public const string strCaFontDialogItemCaptions_Strikeout = "Contains custom text for 'Strikeout' check box.";
      public const string strCaFontDialogItemCaptions_Underline = "Contains custom text for 'Underline' check box.";
      public const string strCaFontDialogItemCaptions_SampleText = "Contains text used in Sample group box to display font sample.";
      public const string strCaFontDialogItemCaptions_Color = "Contains custom text for 'Color' label.";
      public const string strCaFontDialogItemCaptions_Script = "Contains custom text for 'Script' label.";
      public const string strCaFontDialogItemCaptions_Apply = "Contains custom text for 'Apply' button.";

      // CaFontDialog
      public const string strCaFontDialog_TypeDesc = "Represents a common dialog box that displays a list of fonts that are currently installed on the system.";

      // CaPickIconDialogItemCaptions
      public const string strCaPickIconDialogItemCaptions_TypeDesc = "Contains custom texts for icon picker dialog box controls.";
      public const string strCaPickIconDialogItemCaptions_Browse = "Contains custom texts for 'Browse' button.";

      // CaPickIconDialog
      public const string strCaPickIconDialog_TypeDesc = "Represents a common dialog box that displays icon picker.";
      public const string strCaPickIconDialog_IconIndex = "The zero based offset of the icon.";
      public const string strCaPickIconDialog_FileName = "The initial filename (icon library).";

      // CaPageSetupDlgItemCaptions
      public const string strCaPageSetupDialogItemCaptions_TypeDesc = "Contains custom texts for page setup dialog box controls.";
      public const string strCaPageSetupDialogItemCaptions_GroupPaper = "Contains custom text for 'Paper' group box.";
      public const string strCaPageSetupDialogItemCaptions_GroupOrientation = "Contains custom text for 'Orientation' group box.";
      public const string strCaPageSetupDialogItemCaptions_GroupMargins = "Contains custom text for 'Margins' group box.";
      public const string strCaPageSetupDialogItemCaptions_PaperSize = "Contains custom text for 'Paper size' label.";
      public const string strCaPageSetupDialogItemCaptions_PaperSource = "Contains custom text for 'Paper source' label.";
      public const string strCaPageSetupDialogItemCaptions_OrientPortrait = "Contains custom text for 'Portrait' check box.";
      public const string strCaPageSetupDialogItemCaptions_OrientLandscape = "Contains custom text for 'Landscape' check box.";
      public const string strCaPageSetupDialogItemCaptions_MarginLeft = "Contains custom text for 'Left' label in Margins group box.";
      public const string strCaPageSetupDialogItemCaptions_MarginTop = "Contains custom text for 'Top' label in Margins group box.";
      public const string strCaPageSetupDialogItemCaptions_MarginRight = "Contains custom text for 'Right' label in Margins group box.";
      public const string strCaPageSetupDialogItemCaptions_MarginBottom = "Contains custom text for 'Bottom' label in Margins group box.";
      public const string strCaPageSetupDialogItemCaptions_Printer = "Contains custom text for 'Printer' label.";
      //public const string strCaPageSetupDialogItemCaptions_ = "";


      // CaPageSetupDialog
      public const string strCaPageSetupDialog_TypeDesc = "Represents a dialog box that allows users to manipulate page settings, including margins and paper orientation.";
      public const string strCaPageSetupDialog_PrinterSettings = "Gets or sets the printer settings the dialog box to modify when the user clicks the Printer button.";
      public const string strCaPageSetupDialog_Document = "Gets or sets a value indicating the PrintDocument to get page settings from.";
      public const string strCaPageSetupDialog_MinMargins = "Gets or sets a value indicating the minimum margins the user is allowed to select.";
      public const string strCaPageSetupDialog_AllowWarning = "If True, the system displays a warning message when there is no default printer.";
      public const string strCaPageSetupDialog_AllowPagePainting = "If True, the dialog box draws the contents of the sample page.";
      public const string strCaPageSetupDialog_AllowMargins = "Enables or disables the margin controls, preventing the user from setting the margins.";
      public const string strCaPageSetupDialog_UseDefaultMinMargins = "If True, default values (allowed by the printer) are used as the minimum allowable widths for the left, top, right, and bottom margins.";
      public const string strCaPageSetupDialog_UseDefaultMargins = "If True, default values are used as the initial widths for the left, top, right, and bottom margins.";
      public const string strCaPageSetupDialog_AllowOrientation = "Gets or sets a value indicating whether the orientation section of the dialog box (landscape vs. portrait) is enabled.";
      public const string strCaPageSetupDialog_AllowPaper = "Gets or sets a value indicating whether the paper section of the dialog box (paper size and paper source) is enabled.";
      public const string strCaPageSetupDialog_AllowPrinter = "Gets or sets a value indicating whether the Printer button is enabled.";
      public const string strCaPageSetupDialog_ShowHelp = "Gets or sets a value indicating whether the Help button is displayed.";
      public const string strCaPageSetupDialog_ShowNetwork = "Gets or sets a value indicating whether the Network button is visible.";
      public const string strCaPageSetupDialog_PageSettings = "Gets or sets a value indicating the page settings to modify.";
      public const string strCaPageSetupDialog_MeasureUnits = "Units of measurement for margins and papersize.";
      public const string strCaPageSetupDialog_PageSetupOk = "Occurs when user press OK button in the PageSetup dialog box.";
      public const string strCaPageSetupDialog_DrawPage = "This event allows you to customize drawing of the sample page in the Page Setup dialog box.";

      // PageRegion
      public const string strPageRegion_TypeDesc = "Defines page parts eventually drawing in CaPageSetupDialog.PagePaint event.";

      // DrawPageEventHandler
      public const string strDrawPageEventHandler_TypeDesc = "Represents the method that will handle the DrawPage event of the PageSetup dialog box.";

      // DrawPageEventArgs
      public const string strDrawPageEventArgs_TypeDesc = "Provides data for the DrawPage event.";
      public const string strDrawPageEventArgs_Graphics = "Indicates the Graphics object used to paint. This property is read-only.";
      public const string strDrawPageEventArgs_Rect = "Gets the rectangle that represents the bounds of the region that is being drawn.";
      public const string strDrawPageEventArgs_PageRegion = "Gets the page region being drawn.";
      public const string strDrawPageEventArgs_Cancel = "Gets or sets a value indicating whether the draw event should be canceled.";

      // Exception captions
      public const string strXCannotUseDockedFormsWithNewUserInterface = "It's not possible to use docked forms in folder dialog with new user interface features. Reset CaFolderDialog.NewUserInterface to false or remove docked forms.";
      public const string strXCannotCreate2ndFileDialog = "Could not create 2nd Dialog Workshop's Open/Save dialog on top of the first one. Use standard Open/Save dialog in such cases.";
      public const string strXSpecifyPageSettings = "Specify PageSettings or Document property.";
      public const string strXFunctionFailed = "{0} function failed. Error code: {1}";

   }

   [EditorBrowsable(EditorBrowsableState.Never)]
   public class LocalizableConsts
   {
#if DUTCH
      public static string strCreatePrompt = "{0} bestand bestaat niet. wilt u deze maken?";
      public static string strCreatePrompt2 = "{0} bestanden bestaan niet. wilt u deze maken?";
      public static string strMoreThanOneNonexistentFile = "het is mogelijk om maar 1 niet bestaand bestand te specificeren";
#else
      public static string strCreatePrompt = "{0} file does not exist. Do you want to create it?";
      public static string strCreatePrompt2 = "{0} files do not exist. Do you want to create these files?";
      public static string strMoreThanOneNonexistentFile = "It's possible to specify only one nonexistent file.";
#endif
   }
}
