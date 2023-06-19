using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace resource.preview
{
    internal class VSPreview : extension.AnyPreview
    {
        protected override void _Execute(atom.Trace context, int level, string url, string file)
        {
            {
                context.
                    SetTrace(null, NAME.STATE.TRACE.BLINK).
                    SetProgress(CONSTANT.PROGRESS.INFINITE).
                    SendPreview(NAME.EVENT.INFO, url);
            }
            {
                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.HEADER, level, "[[[Info]]]");
                {
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Name]]]", url);
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Size]]]", (new FileInfo(file)).Length.ToString());
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[Raw Format]]]", "HTML");
                }
            }
            {
                context.
                    SetControl(NAME.CONTROL.BROWSER).
                    SetUrl(url).
                    SetCount(10).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.CONTROL, level);
            }
            {
                var a_Context = new Thread(__BrowserThread);
                {
                    a_Context.SetApartmentState(ApartmentState.STA);
                    a_Context.Start(new Tuple<string, string, int>(url, file, level));
                }
            }
        }

        private static void __Execute(atom.Trace context, int level, string url, string file)
        {
            context.
                Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOOTER, level, "[[[Document]]]");
            if (string.IsNullOrEmpty(url) == false)
            {
                var a_Context = new HtmlWeb().Load(url);
                {
                    var a_Context1 = a_Context.DocumentNode.Descendants();
                    {
                        __Execute(context, level + 1, url, file, "[[[Anchors]]]", a_Context1.Where(n => n.Name.ToLower() == "a"));
                        __Execute(context, level + 1, url, file, "[[[Audios]]]", a_Context1.Where(n => n.Name.ToLower() == "audio"));
                        __Execute(context, level + 1, url, file, "[[[Canvases]]]", a_Context1.Where(n => n.Name.ToLower() == "canvas"));
                        __Execute(context, level + 1, url, file, "[[[Forms]]]", a_Context1.Where(n => n.Name.ToLower() == "form"));
                        __Execute(context, level + 1, url, file, "[[[Images]]]", a_Context1.Where(n => n.Name.ToLower() == "img").Concat(a_Context1.Where(n => n.Name.ToLower() == "svg")));
                        __Execute(context, level + 1, url, file, "[[[Links]]]", a_Context1.Where(n => n.Name.ToLower() == "link"));
                        __Execute(context, level + 1, url, file, "[[[Metadata]]]", a_Context1.Where(n => n.Name.ToLower() == "meta"));
                        __Execute(context, level + 1, url, file, "[[[Frames]]]", a_Context1.Where(n => n.Name.ToLower() == "iframe"));
                        __Execute(context, level + 1, url, file, "[[[Scripts]]]", a_Context1.Where(n => n.Name.ToLower() == "script"));
                        __Execute(context, level + 1, url, file, "[[[Styles]]]", a_Context1.Where(n => n.Name.ToLower() == "style"));
                        __Execute(context, level + 1, url, file, "[[[Videos]]]", a_Context1.Where(n => n.Name.ToLower() == "video"));
                    }
                }
                if (a_Context.ParseErrors != null && a_Context.ParseErrors?.Count() > 0)
                {
                    var a_IsFound = false;
                    {
                        context.
                            Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level + 1, "[[[Errors]]]");
                        foreach (var a_Context1 in a_Context.ParseErrors)
                        {
                            if (GetState() == NAME.STATE.WORK.CANCEL)
                            {
                                break;
                            }
                            if (a_IsFound == false)
                            {
                                context.
                                    SetComment("{" + a_Context1.Code.ToString() + "}", "[[[Error Code]]]").
                                    SetUrl(file, a_Context1.Line, a_Context1.LinePosition).
                                    SetUrlPreview(url);
                                a_IsFound = true;
                            }
                            {
                                context.
                                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.ERROR, level + 2, a_Context1.Reason?.Trim());
                            }
                        }
                    }
                }
            }
            {
                context.
                    SetFont(null, 0, NAME.STATE.FONT.NONE).
                    SetProgress(100).
                    SendPreview(NAME.EVENT.INFO, url);
            }
        }

        private static void __Execute(atom.Trace context, int level, string url, string file, string name, IEnumerable<HtmlNode> nodes)
        {
            if ((nodes != null) && (nodes.Count() > 0))
            {
                context.
                    SetFont(null, 0, NAME.STATE.FONT.BOLD).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, name);
                foreach (var a_Context in nodes)
                {
                    var a_Name = "";
                    {
                        var a_Context1 = a_Context.Attributes.FirstOrDefault(n => n.Name == "src");
                        if (a_Context1 != null)
                        {
                            a_Name = a_Context1.Value;
                        }
                    }
                    if (string.IsNullOrEmpty(a_Name))
                    {
                        var a_Context1 = a_Context.Descendants().FirstOrDefault(n => n.Attributes.Contains("src"));
                        if (a_Context1 != null)
                        {
                            a_Name = a_Context1.Attributes?.FirstOrDefault(n => n.Name == "src")?.Value;
                        }
                    }
                    if (string.IsNullOrEmpty(a_Name))
                    {
                        var a_Context1 = a_Context.Attributes.FirstOrDefault(n => n.Name == "href");
                        if (a_Context1 != null)
                        {
                            a_Name = a_Context1.Value;
                        }
                    }
                    if (string.IsNullOrEmpty(a_Name) == false)
                    {
                        context.
                            SetUrlInfo(__GetUrl(url, a_Name));
                    }
                    {
                        context.
                            SetUrl(file, a_Context.Line, a_Context.LinePosition).
                            Send(NAME.SOURCE.PREVIEW, NAME.EVENT.OBJECT, level + 1, __GetText(a_Context.OuterHtml));
                    }
                    if (string.IsNullOrEmpty(__GetControl(a_Context.Name)) == false)
                    {
                        context.
                            SetControl(__GetControl(a_Context.Name)).
                            SetUrlPreview(__GetUrl(url, a_Name)).
                            SetCount(4).
                            Send(NAME.SOURCE.PREVIEW, NAME.EVENT.CONTROL, level + 2);
                    }
                    if (GetState() == NAME.STATE.WORK.CANCEL)
                    {
                        break;
                    }
                }
            }
        }

        private static string __GetControl(string tagName)
        {
            var a_Name = tagName.ToLower();
            {
                if (a_Name == "audio") return NAME.CONTROL.AUDIO;
                if (a_Name == "img") return NAME.CONTROL.PICTURE;
                if (a_Name == "video") return NAME.CONTROL.VIDEO;
            }
            return "";
        }

        private static string __GetText(string text)
        {
            var a_Result = text?.Trim();
            if (string.IsNullOrEmpty(a_Result) == false)
            {
                a_Result = a_Result.Replace("\r", " ");
                a_Result = a_Result.Replace("\n", " ");
                a_Result = a_Result.Replace("\t", " ");
            }
            while (string.IsNullOrEmpty(a_Result) == false)
            {
                var a_Context = a_Result;
                {
                    a_Result = a_Result.Replace("  ", " ");
                    a_Result = a_Result.Replace("> ", ">");
                    a_Result = a_Result.Replace(" <", "<");
                    a_Result = a_Result.Replace(" }", "}");
                    a_Result = a_Result.Replace("} ", "}");
                    a_Result = a_Result.Replace(" {", "{");
                    a_Result = a_Result.Replace("{ ", "{");
                }
                if (a_Context == a_Result)
                {
                    break;
                }
            }
            if (string.IsNullOrEmpty(a_Result) == false)
            {
                var a_Size = GetProperty(NAME.PROPERTY.DEBUGGING_STRING_SIZE, true);
                if (a_Size < a_Result.Length)
                {
                    a_Result = a_Result.Substring(0, a_Size) + "...";
                }
            }
            return a_Result;
        }

        private static string __GetUrl(string baseUrl, string newUrl)
        {
            try
            {
                if ((string.IsNullOrEmpty(baseUrl) == false) && (string.IsNullOrEmpty(newUrl) == false))
                {
                    var a_Context1 = new Uri(baseUrl.Replace("\\", "/"), UriKind.RelativeOrAbsolute);
                    var a_Context2 = new Uri(newUrl.Replace("\\", "/"), UriKind.RelativeOrAbsolute);
                    {
                        return (new Uri(a_Context1, a_Context2))?.ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
            return newUrl;
        }

        private static string __GetUrl(object data)
        {
            var a_Context = data as Tuple<string, string, int>;
            if (a_Context != null)
            {
                return a_Context.Item1?.ToString();
            }
            return null;
        }

        private static string __GetFile(object data)
        {
            var a_Context = data as Tuple<string, string, int>;
            if (a_Context != null)
            {
                return a_Context.Item2?.ToString();
            }
            return null;
        }

        private static int __GetLevel(object data)
        {
            var a_Context = data as Tuple<string, string, int>;
            if (a_Context != null)
            {
                return a_Context.Item3;
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
                if (a_Context.Url == null)
                {
                    a_Context.Navigate(__GetUrl(context));
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
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.EXCEPTION, __GetLevel(context), ex.Message).
                    SetAlignment(NAME.ALIGNMENT.TOP).
                    SendPreview(NAME.EVENT.EXCEPTION, __GetUrl(context));
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
                            //a_Size = Math.Max(a_Size, GetProperty(NAME.PROPERTY.PREVIEW_WIDTH, true));
                            a_Size = Math.Max(a_Size, CONSTANT.OUTPUT.PREVIEW_MIN_WIDTH);
                        }
                        {
                            a_Context.Width = a_Size;
                            a_Context.Height = a_Context.Document.Body.ScrollRectangle.Height;
                        }
                    }
                    {
                        var a_Count = (a_Context.Height + CONSTANT.OUTPUT.PREVIEW_ITEM_HEIGHT + CONSTANT.OUTPUT.PREVIEW_PAGE_INDENT + CONSTANT.OUTPUT.PREVIEW_PAGE_INDENT) / (CONSTANT.OUTPUT.PREVIEW_ITEM_HEIGHT + 1);
                        {
                            a_Count = Math.Max(a_Count, CONSTANT.OUTPUT.PREVIEW_MIN_SIZE);
                        }
                        {
                            atom.Trace.GetInstance().
                                SetCount(a_Count - 2).
                                Send(NAME.SOURCE.PREVIEW, NAME.EVENT.CONTROL, 1);
                        }
                    }
                    {
                        __Execute(atom.Trace.GetInstance(), __GetLevel(a_Context1), __GetUrl(a_Context1), __GetFile(a_Context1));
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
                            Send(NAME.SOURCE.PREVIEW, NAME.EVENT.EXCEPTION, __GetLevel(a_Context1), ex.Message).
                            SetFont(null, 0, NAME.STATE.FONT.NONE).
                            SetProgress(100).
                            SendPreview(NAME.EVENT.EXCEPTION, __GetUrl(a_Context1));
                    }
                }
            }
        }
    }}
