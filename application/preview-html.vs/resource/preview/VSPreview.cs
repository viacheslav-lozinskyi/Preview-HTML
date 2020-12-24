using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace resource.preview
{
    internal class VSPreview : cartridge.AnyPreview
    {
        internal class HINT
        {
            public static string DATA_TYPE = "[[Data type]]";
        }

        internal class Anchors
        {
            static public void Execute(atom.Trace context, int level, IHtmlCollection<IHtmlAnchorElement> data)
            {
                if ((data != null) && (data.Length > 0))
                {
                    context.
                        SetComment(GetArraySize(data), "").
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[Anchors]]");
                    foreach (var a_Context in data)
                    {
                        var a_Name = GetFirstLine(a_Context.GetAttribute("Name"));
                        if (string.IsNullOrEmpty(a_Name) == false)
                        {
                            context.
                                SetComment("[[Anchor]]", HINT.DATA_TYPE).
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, level + 1, a_Name);
                            {
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Download]]", a_Context.Download);
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Language]]", a_Context.TargetLanguage);
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Relation]]", a_Context.Relation);
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Target]]", a_Context.Target);
                                Send(context, NAME.TYPE.VARIABLE, level + 2, "[[Type]]", a_Context.Type);
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
                        SetComment(GetArraySize(data), "").
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[Forms]]");
                    foreach (var a_Context in data)
                    {
                        context.
                            SetUrl(a_Context.Action, "").
                            SetComment("[[Form]]", HINT.DATA_TYPE).
                            Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, level + 1, GetFirstLine(a_Context.Action));
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
                        SetComment(GetArraySize(data), "").
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[Images]]");
                    foreach (var a_Context in data)
                    {
                        if (string.IsNullOrEmpty(a_Context.Source) == false)
                        {
                            context.
                                SetUrl(a_Context.Source, "").
                                SetComment("[[Image]]", HINT.DATA_TYPE).
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, level + 1, GetFileName(a_Context.Source));
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
                        SetComment(GetArraySize(data), "").
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[Links]]");
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
                                SetUrl(a_Name2, "").
                                SetComment("[[Link]]", HINT.DATA_TYPE).
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, level + 1, a_Name1);
                        }
                    }
                }
            }
        }

        public static string GetArraySize(IEnumerable value)
        {
            var a_Result = 0;
            foreach (var a_Context in value)
            {
                a_Result++;
            }
            return "[[Found]]: " + a_Result.ToString();
        }

        public static string GetFileName(string context)
        {
            if (string.IsNullOrEmpty(context) == false)
            {
                var a_Result = context.Replace("\r", "\n").Trim();
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

        public static string GetFirstLine(string context)
        {
            if (string.IsNullOrEmpty(context) == false)
            {
                var a_Result = context.Replace("\r", "\n").Trim();
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

        protected override void _Execute(atom.Trace context, string url, int level)
        {
            url = "https://www.ixbt.com/";
            {
                var a_Context = Configuration.Default.WithDefaultLoader(new LoaderOptions() { IsNavigationDisabled = false, IsResourceLoadingEnabled = false });
                {
                    var a_Context1 = BrowsingContext.New(a_Context).OpenAsync(url).Result;
                    {
                        context.
                            SetState(NAME.STATE.HEADER).
                            Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[Info]]");
                        {
                            context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, level + 1, "[[File Name]]", url);
                            context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, level + 1, "[[File Size]]", a_Context1.Source?.Length.ToString());
                            context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, level + 1, "[[Raw Format]]", "HTML");
                        }
                    }
                    {
                        var a_Size = GetProperty(NAME.PROPERTY.PREVIEW_MEDIA_SIZE);
                        for (var i = 0; i < a_Size; i++)
                        {
                            context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, level);
                        }
                    }
                    {
                        context.
                            SetState(NAME.STATE.FOOTER).
                            Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, level, "[[Document]]");
                        {
                            Anchors.Execute(context, level + 1, a_Context1.Anchors);
                            Forms.Execute(context, level + 1, a_Context1.Forms);
                            Images.Execute(context, level + 1, a_Context1.Images);
                            Links.Execute(context, level + 1, a_Context1.Links);
                        }
                    }
                }
            }
            {
                var a_Context = new Thread(__BrowserThread);
                {
                    a_Context.SetApartmentState(ApartmentState.STA);
                    a_Context.Start(url);
                }
            }
        }

        private static void __BrowserThread(object context)
        {
            try
            {
                var a_Context1 = new WebBrowser();
                {
                    a_Context1.Tag = true;
                    a_Context1.ScrollBarsEnabled = false;
                    a_Context1.ScriptErrorsSuppressed = true;
                    a_Context1.IsWebBrowserContextMenuEnabled = true;
                    a_Context1.AllowNavigation = true;
                    a_Context1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(__DocumentCompleted);
                    a_Context1.Navigate(context.ToString());
                }
                {
                    Application.Run();
                }
                {
                    a_Context1.Dispose();
                }
            }
            catch (Exception ex)
            {
                ;
            }
        }

        private static void __DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var a_Context = (WebBrowser)sender;
            if (a_Context.Tag != null)
            {
                var a_Name = "r:\\result.png";// GetProxyFile(a_Context.Url.ToString(), "png");
                {
                    a_Context.Tag = null;
                }
                {
                    var a_Size = a_Context.Document.Body.ScrollRectangle.Width;
                    {
                        a_Size = Math.Max(a_Size, GetProperty(NAME.PROPERTY.PREVIEW_WIDTH));
                        a_Size = Math.Max(a_Size, CONSTANT.OUTPUT_PREVIEW_MIN_WIDTH); // TODO: Remove it
                    }
                    {
                        a_Context.Width = a_Size;
                        a_Context.Height = a_Context.Document.Body.ScrollRectangle.Height;
                    }
                    {
                        var a_Context2 = new Bitmap(a_Size, a_Context.Document.Body.ScrollRectangle.Height);
                        {
                            a_Context.DrawToBitmap(a_Context2, new Rectangle(0, 0, a_Context2.Width, a_Context2.Height));
                        }
                        {
                            a_Context2.Save(a_Name, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
                {
                    Application.ExitThread();
                }
                {
                    atom.Trace.GetInstance().
                        SetUrlProxy(a_Name).
                        SendPreview(NAME.TYPE.INFO, a_Context.Url.ToString());
                }
                //{
                //    context.
                //        SetState(NAME.STATE.HEADER).
                //        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, 1, "[[Info]]");
                //    {
                //        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, level + 1, "[[File Name]]", a_Context.Url.ToString());
                //        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, level + 1, "[[File Size]]", a_Context.DocumentStream?.Length.ToString());
                //        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, level + 1, "[[Raw Format]]", "HTML");
                //    }
                //}
                {
                    var a_Size = GetProperty(NAME.PROPERTY.PREVIEW_MEDIA_SIZE);
                    {
                        a_Size = Math.Min(a_Size, a_Context.Document.Body.ScrollRectangle.Height / CONSTANT.OUTPUT_PREVIEW_ITEM_HEIGHT);
                        a_Size = Math.Max(a_Size, CONSTANT.OUTPUT_PREVIEW_MIN_SIZE);
                    }
                    for (var i = 0; i < a_Size; i++)
                    {
                        atom.Trace.GetInstance().Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, 1);
                    }
                }
                {
                    atom.Trace.GetInstance().
                        SetState(NAME.STATE.FOOTER).
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, 1, "[[Document]]");
                    {
                        //atom.Trace.GetInstance().Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, level + 1, "AAAA");
                    }
                }
            }
        }
    };
}
