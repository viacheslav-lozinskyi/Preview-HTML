
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace resource.package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(CONSTANT.NAME, CONSTANT.DESCRIPTION, CONSTANT.VERSION)]
    [Guid(CONSTANT.GUID)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class PreviewHTMLPackage : AsyncPackage
    {
        internal static class CONSTANT
        {
            public const string COPYRIGHT = "Copyright (c) 2020 by Viacheslav Lozinskyi. All rights reserved.";
            public const string DESCRIPTION = "Quick preview of HTML and HTM files";
            public const string EXTENSION1 = ".HTML";
            public const string EXTENSION2 = ".HTM";
            public const string GUID = "9DF0A484-5C6F-431A-9F77-2086340F9997";
            public const string NAME = "Preview-HTML";
            public const string VERSION = "1.0.1";
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            {
                cartridge.AnyPreview.Connect();
                cartridge.AnyPreview.Register(cartridge.AnyPreview.MODE.PREVIEW, CONSTANT.EXTENSION1, new resource.preview.VSPreview());
                cartridge.AnyPreview.Register(cartridge.AnyPreview.MODE.PREVIEW, CONSTANT.EXTENSION2, new resource.preview.VSPreview());
            }
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            {
                cartridge.AnyPreview.Disconnect();
                canClose = true;
            }
            return VSConstants.S_OK;
        }
    }
}
