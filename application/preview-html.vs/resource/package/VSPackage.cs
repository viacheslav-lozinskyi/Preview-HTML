
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace resource.package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CONSTANT.GUID)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class PreviewHTML : AsyncPackage
    {
        internal static class CONSTANT
        {
            public const string APPLICATION = "Visual Studio";
            public const string COPYRIGHT = "Copyright (c) 2020-2022 by Viacheslav Lozinskyi. All rights reserved.";
            public const string DESCRIPTION = "Quick preview of HTML, HTM and CSS files";
            public const string GUID = "9DF0A484-5C6F-431A-9F77-2086340F9997";
            public const string NAME = "Preview-HTML";
            public const string VERSION = "1.0.3";
        }

        internal class InfoBarService : IVsInfoBarUIEvents
        {
            private static uint s_Cookie = 0;

            public static void Validate()
            {
                try
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    if (string.IsNullOrEmpty(atom.Trace.GetFailState(CONSTANT.APPLICATION)) == false)
                    {
                        var a_Context1 = Package.GetGlobalService(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
                        var a_Context2 = (IVsInfoBarHost)null;
                        if (a_Context1 != null)
                        {
                            var a_Context3 = Package.GetGlobalService(typeof(SVsShell)) as IVsShell;
                            var a_Context4 = (object)null;
                            if (a_Context3 != null)
                            {
                                a_Context3.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out a_Context4);
                                a_Context2 = (IVsInfoBarHost)a_Context4;
                            }
                        }
                        if (a_Context2 != null)
                        {
                            var a_Context3 = a_Context1.CreateInfoBar(new InfoBarModel(
                                textSpans: new[]
                                {
                                    new InfoBarTextSpan(CONSTANT.NAME, true),
                                    new InfoBarTextSpan(" extension doesn't work without "),
                                    new InfoBarTextSpan("MetaOutput", true),
                                    new InfoBarTextSpan("! Please install it.")
                                },
                                actionItems: new[]
                                {
                                    new InfoBarButton("Install MetaOutput")
                                },
                                image: KnownMonikers.StatusError));
                            {
                                a_Context3.Advise(new InfoBarService(), out s_Cookie);
                                a_Context2.AddInfoBar(a_Context3);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            public void OnClosed(IVsInfoBarUIElement infoBar)
            {
                try
                {
                    infoBar.Unadvise(s_Cookie);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            public void OnActionItemClicked(IVsInfoBarUIElement infoBar, IVsInfoBarActionItem action)
            {
                try
                {
                    Process.Start(atom.Trace.GetFailState(CONSTANT.APPLICATION));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            {
                extension.AnyPreview.Connect();
                extension.AnyPreview.Register(".HTML", new resource.preview.VSPreview());
                extension.AnyPreview.Register(".HTM",  new resource.preview.VSPreview());
            }
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            }
            {
                InfoBarService.Validate();
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            {
                extension.AnyPreview.Disconnect();
                canClose = true;
            }
            return VSConstants.S_OK;
        }
    }
}
