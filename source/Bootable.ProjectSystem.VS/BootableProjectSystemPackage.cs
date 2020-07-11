using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Bootable.ProjectSystem.VS
{
    [Guid(PackageGuid)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    internal sealed class BootableProjectSystemPackage : AsyncPackage
    {
        private const string PackageGuid = "8ad46f5a-ea8b-4e63-819c-35f794681ede";

        public const string BootableProjectSystemCommandSet = "0667853f-43a4-4472-bd77-a1ae2502f352";
        public const long PublishBootableProjectContextMenuCmdId = 0x100;
    }
}
