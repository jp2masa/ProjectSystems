using System;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Bootable.ProjectSystem.VS
{
    internal static class BootableImagesMonikers
    {
        private static readonly Guid ManifestGuid = new Guid("4792d34f-c4d6-431b-9edb-54937e343990");

        private const int IsoPublishProviderIconId = 0;
        private const int UsbPublishProviderIconId = 1;

        public static ImageMoniker IsoPublishProviderIcon => new ImageMoniker { Guid = ManifestGuid, Id = IsoPublishProviderIconId };
        public static ImageMoniker UsbPublishProviderIcon => new ImageMoniker { Guid = ManifestGuid, Id = UsbPublishProviderIconId };
    }
}
