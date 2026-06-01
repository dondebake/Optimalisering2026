using System;
using System.Text;
using System.ComponentModel;

namespace ComponentAge.Dialogs
{
    [Description("Identifies the special shell or system folder.")]
    public enum SpecialFolder
    {
        Desktop = 0x0000,        // <desktop>
        Internet = 0x0001,        // Internet Explorer (icon on desktop)
        Programs = 0x0002,        // Start Menu\Programs
        Controls = 0x0003,        // My Computer\Control Panel
        Printers = 0x0004,        // My Computer\Printers
        Personal = 0x0005,        // My Documents
        Favorites = 0x0006,        // <user name>\Favorites
        Startup = 0x0007,        // Start Menu\Programs\Startup
        Recent = 0x0008,        // <user name>\Recent
        SendTo = 0x0009,        // <user name>\SendTo
        BitBucket = 0x000a,        // <desktop>\Recycle Bin
        StartMenu = 0x000b,        // <user name>\Start Menu
        MyDocuments = 0x000c,        // logical "My Documents" desktop icon
        MyMusic = 0x000d,        // "My Music" folder
        MyVideo = 0x000e,        // "My Videos" folder
        DesktopDirectory = 0x0010,        // <user name>\Desktop
        Drives = 0x0011,        // My Computer
        Network = 0x0012,        // Network Neighborhood (My Network Places)
        NetHood = 0x0013,        // <user name>\nethood
        Fonts = 0x0014,        // windows\fonts
        Templates = 0x0015,
        CommonStartMenu = 0x0016,        // All Users\Start Menu
        CommonPrograms = 0X0017,        // All Users\Start Menu\Programs
        CommonStartup = 0x0018,        // All Users\Startup
        CommonDesktopDirectory = 0x0019,        // All Users\Desktop
        AppData = 0x001a,        // <user name>\Application Data
        PrintHood = 0x001b,        // <user name>\PrintHood
        LocalAppData = 0x001c,        // <user name>\Local Settings\Applicaiton Data (non roaming)
        AltStartup = 0x001d,        // non localized startup
        CommonAltStartup = 0x001e,        // non localized common startup
        CommonFavorites = 0x001f,
        InternetCache = 0x0020,
        Cookies = 0x0021,
        History = 0x0022,
        CommonAppData = 0x0023,        // All Users\Application Data
        Windows = 0x0024,        // GetWindowsDirectory()
        System = 0x0025,        // GetSystemDirectory()
        ProgramFiles = 0x0026,        // C:\Program Files
        MyPictures = 0x0027,        // C:\Program Files\My Pictures
        Profile = 0x0028,        // USERPROFILE
        SystemX86 = 0x0029,        // x86 system directory on RISC
        ProgramFilesX86 = 0x002a,        // x86 C:\Program Files on RISC
        None = 0x1000,        // NONE
    }
}
