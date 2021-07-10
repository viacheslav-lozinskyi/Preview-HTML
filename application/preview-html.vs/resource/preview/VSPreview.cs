using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace resource.preview
{
    internal class VSPreview : extension.AnyPreview
    {
        internal class HINT
        {
            public static string DATA_TYPE = "[[[Data Type]]]";
        }

        internal class Anchors
        {
            static public void Execute(atom.Trace context, int level, IHtmlCollection<IHtmlAnchorElement> data)
            {
                if ((data != null) && (data.Length > 0))
                {
                    context.
                        SetComment(GetArraySize(data)).
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[[Anchors]]]");
                    foreach (var a_Context in data)
                    {
                        var a_Name = GetFirstLine(a_Context.GetAttribute("Name"));
                        if (string.IsNullOrEmpty(a_Name) == false)
                        {
                            context.
                                SetComment("[[[Anchor]]]", HINT.DATA_TYPE).
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PARAMETER, level + 1, a_Name);
                            {
                                Send(context, NAME.TYPE.PARAMETER, level + 2, "[[[Download]]]", a_Context.Download);
                                Send(context, NAME.TYPE.PARAMETER, level + 2, "[[[Language]]]", a_Context.TargetLanguage);
                                Send(context, NAME.TYPE.PARAMETER, level + 2, "[[[Relation]]]", a_Context.Relation);
                                Send(context, NAME.TYPE.PARAMETER, level + 2, "[[[Target]]]", a_Context.Target);
                                Send(context, NAME.TYPE.PARAMETER, level + 2, "[[[Type]]]", a_Context.Type);
                            }
                        }
                    }
                }
            }
        }

        internal class Forms
        {
            static public void Execute(atom.Trace context, int level, IHtmlCollection<IHtmlFormElement> data)
            {
                if ((data != null) && (data.Length > 0))
                {
                    context.
                        SetComment(GetArraySize(data)).
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[[Forms]]]");
                    foreach (var a_Context in data)
                    {
                        context.
                            SetUrl(a_Context.Action).
                            SetComment("[[[Form]]]", HINT.DATA_TYPE).
                            Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PARAMETER, level + 1, GetFirstLine(a_Context.Action));
                        {
                        }
                    }
                }
            }
        }

        internal class Images
        {
            static public void Execute(atom.Trace context, int level, IHtmlCollection<IHtmlImageElement> data)
            {
                if ((data != null) && (data.Length > 0))
                {
                    context.
                        SetComment(GetArraySize(data)).
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[[Images]]]");
                    foreach (var a_Context in data)
                    {
                        if (string.IsNullOrEmpty(a_Context.Source) == false)
                        {
                            context.
                                SetUrl(a_Context.Source).
                                SetComment("[[[Image]]]", HINT.DATA_TYPE).
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PARAMETER, level + 1, GetFileName(a_Context.Source));
                            {
                                context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, level + 2);
                                context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, level + 2);
                                context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, level + 2);
                                context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, level + 2);
                            }
                        }
                    }
                }
            }
        }

        internal class Links
        {
            static public void Execute(atom.Trace context, int level, IHtmlCollection<IElement> data)
            {
                if ((data != null) && (data.Length > 0))
                {
                    context.
                        SetComment(GetArraySize(data)).
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[[Links]]]");
                    foreach (var a_Context in data)
                    {
                        var a_Name1 = GetFirstLine(NodeExtensions.Text(a_Context));
                        var a_Name2 = GetFirstLine(NodeExtensions.HyperReference(a_Context, a_Context.GetAttribute("Href"))?.Href);
                        if (string.IsNullOrEmpty(a_Name1))
                        {
                            a_Name1 = GetFirstLine(a_Context.InnerHtml);
                        }
                        if ((string.IsNullOrEmpty(a_Name1) == false) || (string.IsNullOrEmpty(a_Name2) == false))
                        {
                            context.
                                SetUrl(a_Name2).
                                SetComment("[[[Link]]]", HINT.DATA_TYPE).
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PARAMETER, level + 1, a_Name1);
                        }
                    }
                }
            }
        }

        public static string GetArraySize(IEnumerable data)
        {
            var a_Result = 0;
            foreach (var a_Context in data)
            {
                a_Result++;
            }
            return "[[[Found]]]: " + a_Result.ToString();
        }

        public static string GetFileName(string data)
        {
            if (string.IsNullOrEmpty(data) == false)
            {
                var a_Result = data.Replace("\r", "\n").Trim();
                {
                    var a_Index = a_Result.IndexOf("\n");
                    if (a_Index > 0)
                    {
                        a_Result = a_Result.Substring(0, a_Index);
                    }
                }
                {
                    var a_Index = a_Result.IndexOf("?");
                    if (a_Index > 0)
                    {
                        a_Result = a_Result.Substring(0, a_Index);
                    }
                }
                {
                    var a_Index = a_Result.IndexOf("#");
                    if (a_Index > 0)
                    {
                        a_Result = a_Result.Substring(0, a_Index);
                    }
                }
                return Path.GetFileName(a_Result);
            }
            return "";
        }

        public static string GetFirstLine(string data)
        {
            if (string.IsNullOrEmpty(data) == false)
            {
                var a_Result = data.Replace("\r", "\n").Trim();
                {
                    var a_Index = a_Result.IndexOf("\n");
                    if (a_Index > 0)
                    {
                        a_Result = a_Result.Substring(0, a_Index);
                    }
                }
                return a_Result;
            }
            return "";
        }

        public static void Send(atom.Trace context, string type, int level, string name, string value)
        {
            if (string.IsNullOrEmpty(value) == false)
            {
                context.Send(NAME.SOURCE.PREVIEW, type, level, name, value);
            }
        }

        protected override void _Execute(atom.Trace context, int level, string url, string file)
        {
            {
                context.
                    SetFontState(NAME.FONT_STATE.BLINK).
                    SetProgress(NAME.PROGRESS.INFINITE).
                    SendPreview(NAME.TYPE.INFO, url);
            }
            {
                var a_Context = Configuration.Default.WithDefaultLoader(new LoaderOptions() { IsNavigationDisabled = false, IsResourceLoadingEnabled = false });
                {
                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.HEADER, level, "[[[Info]]]");
                    {
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PARAMETER, level + 1, "[[[File Name]]]", url);
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PARAMETER, level + 1, "[[[File Size]]]", (new FileInfo(file)).Length.ToString());
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PARAMETER, level + 1, "[[[Raw Format]]]", "HTML");
                    }
                }
            }
            {
                var a_Context = new Thread(__BrowserThread);
                {
                    a_Context.SetApartmentState(ApartmentState.STA);
                    a_Context.Start(new Tuple<string, int>(url, level));
                }
            }
        }

        private static string __GetUrl(object data)
        {
            var a_Context = data as Tuple<string, int>;
            if (a_Context != null)
            {
                return a_Context.Item1?.ToString();
            }
            return null;
        }

        private static int __GetLevel(object data)
        {
            var a_Context = data as Tuple<string, int>;
            if (a_Context != null)
            {
                return a_Context.Item2;
            }
            return 0;
        }

        private static void __BrowserThread(object context)
        {
            try
            {
                var a_Context = new WebBrowser();
                {
                    a_Context.Tag = context;
                    a_Context.ScrollBarsEnabled = false;
                    a_Context.ScriptErrorsSuppressed = true;
                    a_Context.IsWebBrowserContextMenuEnabled = true;
                    a_Context.AllowNavigation = true;
                    a_Context.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(__DocumentCompleted);
                    a_Context.Url = new Uri(__GetUrl(context), UriKind.RelativeOrAbsolute);
                }
                while (a_Context.Tag != null)
                {
                    Application.DoEvents();
                    Thread.Sleep(50);
                }
                {
                    a_Context.Dispose();
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                atom.Trace.GetInstance().
                    Send(NAME.SOURCE.PREVIEW, NAME.TYPE.EXCEPTION, __GetLevel(context), ex.Message).
                    SetAlignment(NAME.ALIGNMENT.TOP).
                    SendPreview(NAME.TYPE.EXCEPTION, __GetUrl(context));
            }
        }

        private static void __DocumentCompleted(object context, WebBrowserDocumentCompletedEventArgs e)
        {
            var a_Context = (WebBrowser)context;
            if (__GetUrl(a_Context?.Tag) != null)
            {
                var a_Context1 = a_Context.Tag;
                try
                {
                    {
                        a_Context.Tag = true;
                    }
                    {
                        var a_Size = a_Context.Document.Body.ScrollRectangle.Width;
                        {
                            a_Size = Math.Max(a_Size, GetProperty(NAME.PROPERTY.PREVIEW_WIDTH, true));
                            a_Size = Math.Max(a_Size, CONSTANT.OUTPUT_PREVIEW_MIN_WIDTH); // TODO: Remove it
                        }
                        {
                            a_Context.Width = a_Size;
                            a_Context.Height = a_Context.Document.Body.ScrollRectangle.Height;
                        }
                    }
                    {
                        var a_Size = (a_Context.Height + CONSTANT.OUTPUT_PREVIEW_ITEM_HEIGHT - 1) / CONSTANT.OUTPUT_PREVIEW_ITEM_HEIGHT;
                        {
                            a_Size = Math.Max(a_Size, CONSTANT.OUTPUT_PREVIEW_MIN_SIZE);
                        }
                        for (var i = 0; i < a_Size; i++)
                        {
                            atom.Trace.GetInstance().Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, 1);
                        }
                    }
                    {
                        atom.Trace.GetInstance().
                            Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOOTER, __GetLevel(a_Context1), "[[[Document]]]");
                        {
                            //Anchors.Execute(context, __GetLevel(a_Context1) + 1, a_Context1.Anchors);
                            //Forms.Execute(context, __GetLevel(a_Context1) + 1, a_Context1.Forms);
                            //Images.Execute(context, __GetLevel(a_Context1) + 1, a_Context1.Images);
                            //Links.Execute(context, __GetLevel(a_Context1) + 1, a_Context1.Links);
                        }
                    }
                    {
                        atom.Trace.GetInstance().
                            SetFontState(NAME.FONT_STATE.NONE).
                            SetProgress(100).
                            SendPreview(NAME.TYPE.INFO, __GetUrl(a_Context1));
                    }
                    {
                        a_Context.Tag = null;
                    }
                }
                catch (Exception ex)
                {
                    {
                        a_Context.Tag = null;
                    }
                    {
                        atom.Trace.GetInstance().
                            Send(NAME.SOURCE.PREVIEW, NAME.TYPE.EXCEPTION, __GetLevel(a_Context1), ex.Message).
                            SetFontState(NAME.FONT_STATE.NONE).
                            SetProgress(100).
                            SendPreview(NAME.TYPE.EXCEPTION, __GetUrl(a_Context1));
                    }
                }
            }
        }
    };
}
