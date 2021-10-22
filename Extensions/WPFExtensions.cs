using AutomationControls.WPF.Adorners;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AutomationControls
{
    namespace Extensions
    {
        public static partial class Extensions
        {

            

            #region System.Windows

            public static ContentAdorner ApplyContentAdorner(this UIElement toAdorn, UIElement ele)
            {
                ContentAdorner adorner = new ContentAdorner(toAdorn);
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(toAdorn);
                if (layer == null) return null;
                adorner.Content = ele;
                toAdorn.ClearAdorners();
                layer.Add(adorner);
                ele.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.Escape) { toAdorn.ClearAdorners(); }
                };

                return adorner;
                //toAdorn.MouseLeave += delegate { ele.Visibility = System.Windows.Visibility.Hidden; };
            }

            public static Window ToWindow(this UIElement ele)
            {
                Window w = new Window() { ContextMenu = new ContextMenu() };
                w.Content = ele;
                w.ShowActivated = true;
                w.Show();
                return w;
            }

            #region Animations

            public static FrameworkElement Animate_Spin(this FrameworkElement source, double from, double to, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation anim = new DoubleAnimation(from, to, duration);
                sb.Children.Add(anim);
                Storyboard.SetTarget(anim, source);
                Storyboard.SetTargetProperty(anim, new PropertyPath("(FrameworkElement.RenderTransform).(RotateTransform.Angle)"));
                sb.Begin();
                return source;
            }

            public static FrameworkElement AnimateHeight(this FrameworkElement source, double startingHeight, double endingHeight, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = startingHeight;
                animation.To = endingHeight;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(FrameworkElement.Height)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            public static FrameworkElement AnimateWidth(this FrameworkElement source, double startingWidth, double endingWidth, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = startingWidth;
                animation.To = endingWidth;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(FrameworkElement.Width)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            public static Control AnimateBackgroundOpacity(this Control source, double startingOpacity, double endingOpacity, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = startingOpacity;
                animation.To = endingOpacity;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(Control.Background)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            public static UIElement AnimateBackgroundOpacity(this UIElement source, double startingOpacity, double endingOpacity, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = startingOpacity;
                animation.To = endingOpacity;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(UIElement.Opacity)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            public static Panel AnimateBackgroundColor(this Panel source, Color start, Color end, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                ColorAnimation animation = new ColorAnimation();

                animation.From = start;
                animation.To = end;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(Panel.Background).(SolidColorBrush.Color)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            public static FrameworkElement FocusParent(this FrameworkElement source)
            {
                FrameworkElement parent = (FrameworkElement)source.Parent;
                while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
                {
                    parent = (FrameworkElement)parent.Parent;
                }

                DependencyObject scope = FocusManager.GetFocusScope(source);
                FocusManager.SetFocusedElement(scope, parent as IInputElement);
                return parent;
            }

            #endregion

            #endregion

            #region System.Windows.Controls

            public static void ExpandAll(this ItemsControl items)
            {
                foreach (object obj in items.Items)
                {
                    ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                    if (childControl != null)
                    {
                        ExpandAll(childControl);
                    }
                    TreeViewItem item = childControl as TreeViewItem;
                    if (item != null)
                        item.IsExpanded = true;
                }
            }

            public static string Text(this RichTextBox rtb)
            {
                TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                string text = tr.Text;
                return text;
            }

            public static void TextPrepend(this RichTextBox rtb, string s, Color clr = default(Color), double fontSize = 18)
            {
                //Brush brush = new SolidColorBrush(clr);
                //FlowDocument fd = rtb.Document;
                //Paragraph  para = new Paragraph();
                Brush brush = new SolidColorBrush(clr);
                FlowDocument fd = rtb.Document;
                var res = fd.Blocks.Where(x => x.GetType() == typeof(Paragraph));
                Paragraph para;
                if (res.Count() == 0) para = new Paragraph();
                else para = (Paragraph)res.ToArray()[0];
                para.FontSize = fontSize; //para.Foreground = brush;

                Run run = new Run(s) { Foreground = brush };
                if (para.Inlines.Count() == 0) para.Inlines.Add(run);
                else para.Inlines.InsertBefore(para.Inlines.First(), run);

                //   if(fd.Blocks.Count() == 0) fd.Blocks.Add(para);
                //  else fd.Blocks.InsertBefore(fd.Blocks.First(), para);


                rtb.Document = fd;

            }

            public static void TextAppend(this RichTextBox rtb, string s, Color clr = default(Color), double fontSize = 18)
            {
                Brush brush = new SolidColorBrush(clr);
                FlowDocument fd = rtb.Document;
                var res = fd.Blocks.Where(x => x.GetType() == typeof(Paragraph));
                Paragraph para;
                if (res.Count() == 0) para = new Paragraph();
                else para = (Paragraph)res.ToArray()[0];
                para.FontSize = fontSize; //para.Foreground = brush;

                Span span = new Span();
                Run run = new Run(s) { Foreground = brush };
                //span.Inlines.Add(run);
                para.Inlines.Add(run);
                fd.Blocks.Add(para);
                rtb.Document = fd;

            }

            public static void TextClear(this RichTextBox rtb)
            {
                rtb.Document.Blocks.Clear();
            }

            public static void ClearAdorners(this UIElement source)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(source);
                if (layer == null) return;
                Adorner[] toRemoveArray = layer.GetAdorners(source);
                if (toRemoveArray == null) return;
                foreach (var v in layer.GetAdorners(source))
                {
                    if (v != null)
                    {
                        layer.Remove(v);
                    }
                }
            }

            public static TextRange FindWordFromPosition(this TextPointer position, string word)
            {
                while (position != null)
                {
                    if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        string textRun = position.GetTextInRun(LogicalDirection.Forward);

                        // Find the starting index of any substring that matches "word".
                        int indexInRun = textRun.IndexOf(word);
                        if (indexInRun >= 0)
                        {
                            TextPointer start = position.GetPositionAtOffset(indexInRun);
                            TextPointer end = start.GetPositionAtOffset(word.Length);
                            return new TextRange(start, end);
                        }
                    }
                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                }
                // position will be null if "word" is not found.
                return null;
            }

            public static Task<TcpClient> AcceptTcpClientAsync(this TcpListener listener, CancellationToken token)
            {
                try
                {
                    return listener.AcceptTcpClientAsync();
                }
                catch
                {
                    return null;
                }
            }

            public static void ClearText(this RichTextBox rtb) { new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text = String.Empty; }

            public static void HighlightWord(this RichTextBox box, string text, Color color)
            {
                if (String.IsNullOrEmpty(text)) return;
                TextRange tr = null;
                TextPointer tp = box.Document.ContentStart;
                do
                {
                    tr = tp.FindWordFromPosition(text);
                    if (tr == null) break;
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
                    tp = tr.End;
                } while (tr != null);
            }

            public static void AppendText(this RichTextBox box, string text, Color color)
            {
                BrushConverter bc = new BrushConverter();
                TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
                tr.Text = text;
                try
                {
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(color.ToString()));
                }
                catch (FormatException) { }

            }

            public static void AddRange(this ItemCollection ic, IList<object> lst)
            {
                foreach (var v in lst) { ic.Add(v); }
            }

            public static BitmapImage To_Media_BitmapImage(this Image source) { return (BitmapImage)source.Source; }

            public static Image To_Controls_Image(this ImageSource source, string FilePath)
            {
                return new Image { Source = new BitmapImage(new Uri(FilePath)) };
            }

            public static System.Drawing.Bitmap ToDrawingBitmap(this ImageSource source, string FilePath = null)
            {
                var bs = (source as BitmapSource).Clone();
                MemoryStream ms = new MemoryStream();
                var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bs));
                encoder.Save(ms);
                ms.Flush();
                return ((System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms));
            }

            public static System.Drawing.Bitmap ToDrawingBitmap(this ImageSource source, int height, int width)
            {
                var bs = (source as BitmapSource).Clone();

                MemoryStream ms = new MemoryStream();
                var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bs));
                encoder.Save(ms);
                ms.Flush();
                var b = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms);
                System.Drawing.Bitmap nb = new System.Drawing.Bitmap(b, width, height);

                return nb;
            }

            #region animation

            public static TextBlock AnimateSpin(this TextBlock source, String text)
            {
                if (text == null) return source;
                if (text.Length > 50)
                {
                    source.Text = text;
                    return source;
                }
                int startPos = source.Text.Length;
                source.Text += text;
                Storyboard sb = new Storyboard();
                // sb.RepeatBehavior = RepeatBehavior.Forever;
                // sb.AutoReverse = true;

                source.TextEffects = new TextEffectCollection();

                for (int i = 0; i < source.Text.Length; i++)
                {
                    DoubleAnimation spinAnimation = new DoubleAnimation();
                    spinAnimation.To = 0;
                    spinAnimation.AccelerationRatio = 0.5;
                    spinAnimation.AutoReverse = true;
                    spinAnimation.DecelerationRatio = 0.5;
                    spinAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
                    spinAnimation.RepeatBehavior = RepeatBehavior.Forever;

                    DoubleAnimation anim1 = new DoubleAnimation(200, 1, TimeSpan.FromMilliseconds(200));

                    TransformGroup transGrp = new TransformGroup();
                    transGrp.Children.Add(new RotateTransform());
                    transGrp.Children.Add(new ScaleTransform());

                    TextEffect effect = new TextEffect();
                    effect.PositionStart = i;
                    effect.PositionCount = 1;
                    effect.Transform = transGrp;

                    source.TextEffects.Add(effect);

                    double totalMs = spinAnimation.Duration.TimeSpan.TotalMilliseconds;
                    double offset = totalMs / 10;
                    double resolvedOffset = offset + i;
                    spinAnimation.BeginTime = TimeSpan.FromMilliseconds(resolvedOffset);

                    String path = String.Format("TextEffects[{0}].Transform.Children[0].Angle", i);
                    PropertyPath propPath = new PropertyPath(path);

                    Storyboard.SetTarget(spinAnimation, source);
                    Storyboard.SetTargetProperty(spinAnimation, propPath);

                    String path2 = String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", i);
                    PropertyPath propPath2 = new PropertyPath(path2);

                    Storyboard.SetTarget(anim1, source);
                    Storyboard.SetTargetProperty(anim1, propPath2);

                    sb.Children.Add(spinAnimation);
                    sb.Children.Add(anim1);
                    sb.Begin();
                }
                return source;
            }

            public static TextBlock AnimateColor(this TextBlock source, String text, TimeSpan duration, Color fromColor, Color toColor, TimeSpan beginOffset = default(TimeSpan), bool reverseAnimation = false)
            {
                if (text == null) return source;
                Storyboard sb = new Storyboard();
                sb.BeginTime = beginOffset;
                ColorAnimation animation = new ColorAnimation();
                if (reverseAnimation) sb.AutoReverse = true;
                animation.From = fromColor;
                animation.To = toColor;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(TextBlock.Foreground).(SolidColorBrush.Color)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            public static TextBlock AnimateWidth(this TextBlock source, double startingWidth, double endingWidth, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = startingWidth;
                animation.To = endingWidth;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(TextBlock.Width)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            public static TextBlock AnimateHeight(this TextBlock source, double startingHeight, double endingHeight, TimeSpan duration)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = startingHeight;
                animation.To = endingHeight;
                animation.Duration = new Duration(duration);
                PropertyPath pp = new PropertyPath("(TextBlock.Height)");

                Storyboard.SetTarget(animation, source);
                Storyboard.SetTargetProperty(animation, pp);
                sb.Children.Add(animation);
                sb.Begin();
                return source;
            }

            #endregion

            #endregion

            #region System.Windows.Media

            public static byte[] BitmapSourceToArray(this BitmapSource bitmapSource)
            {
                // Stride = (width) x (bytes per pixel)
                int stride = (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
                byte[] pixels = new byte[(int)bitmapSource.PixelHeight * stride];
                bitmapSource.CopyPixels(pixels, stride, 0);
                return pixels;
            }


            public static BitmapImage ToBitmapImage(this BitmapSource bitmapSource)
            {
                // Stride = (width) x (bytes per pixel)
                int stride = (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
                byte[] pixels = new byte[(int)bitmapSource.PixelHeight * stride];
                bitmapSource.CopyPixels(pixels, stride, 0);
                //WriteableBitmap wbm = new WriteableBitmap();
                //wbm.WritePixels(new Int32Rect(0, 0, bitmapSource.PixelWidth, bitmapSource.PixelHeight), pixels, stride, 0);
                BitmapImage ret = new BitmapImage();
                using (MemoryStream stream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(stream);
                    ret.BeginInit();
                    ret.CacheOption = BitmapCacheOption.OnLoad;
                    ret.StreamSource = stream;
                    ret.EndInit();
                    ret.Freeze();
                }
                return ret;
            }

            public static int stride(this BitmapSource bitmapSource)
            {
                return (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            }

            public static Brush Random(this Brush brush)
            {
                Random rnd = new Random();
                Type brushesType = typeof(Brushes);
                PropertyInfo[] properties = brushesType.GetProperties();
                int random = rnd.Next(properties.Length);
                brush = (Brush)properties[random].GetValue(null, null);
                return brush;
            }

            public static System.Windows.Media.ImageSource To_System_Windows_Media_ImageSource(this System.Drawing.Bitmap sender)
            {

                IntPtr hBitmap = sender.GetHbitmap();
                System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                return WpfBitmap;
            }

            public static BitmapImage ToBitmapImage(this System.Drawing.Bitmap sender)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                MemoryStream ms = new MemoryStream();
                sender.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }

            #region "WriteableBitmap"

            public static WriteableBitmap ToWriteableBitmap(this System.Drawing.Bitmap sender)
            {
                WriteableBitmap ret = new WriteableBitmap(sender.Width, sender.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
                BitmapData Data = sender.LockBits(new System.Drawing.Rectangle(0, 0, sender.Width, sender.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                IntPtr ptr = Data.Scan0;
                var stride = Data.Stride;
                var buffersize = sender.Height * stride;
                try
                {
                    ret.WritePixels(new Int32Rect(0, 0, sender.Width, sender.Height), ptr, buffersize, stride);
                }
                catch { }
                return ret;
            }

            public static void SaveFile(this WriteableBitmap source, string filename)
            {
                if (filename != string.Empty)
                {
                    using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                    {
                        JpegBitmapEncoder encoder5 = new JpegBitmapEncoder();
                        //  PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                        encoder5.Frames.Add(BitmapFrame.Create(source));
                        encoder5.Save(stream5);
                        stream5.Close();
                    }
                }
            }

            #endregion

            public class PixelData
            {
                public byte red { get; set; }
                public byte green { get; set; }
                public byte blue { get; set; }
                public byte alpha { get; set; }
            }

            public static List<PixelData> GetPixels(this BitmapSource img)
            {
                List<PixelData> ret = new List<PixelData>();
                int stride = img.PixelWidth * 4;
                int size = img.PixelHeight * stride;
                byte[] pixels = new byte[size];
                img.CopyPixels(pixels, stride, 0);

                //  int index = y * stride + 4 * x;
                for (int i = 0; i < pixels.Count(); i += 4)
                {
                    PixelData data = new PixelData();
                    data.red = pixels[i];
                    data.green = pixels[i + 1];
                    data.blue = pixels[i + 2];
                    data.alpha = pixels[i + 3];
                    ret.Add(data);
                }
                return ret;
            }

            //public static double Intensity(this Color color)
            //{
            //    return (double)(color.R + color.G + color.B) / (3 * 255);
            //}

            //public static double Brightness(this Color color)
            //{
            //    return (double)Math.Max(Math.Max(color.R, color.G), color.B) / 255;
            //}

            //public static double SaturationHSB(this Color color)
            //{
            //    var max = (double)Math.Max(Math.Max(color.R, color.G), color.B) / 255;
            //    if(max == 0) return 0;
            //    var min = (double)Math.Min(Math.Min(color.R, color.G), color.B) / 255;
            //    return (max - min) / max;
            //}

            //public static double Lightness(this Color color)
            //{
            //    var max = (double)Math.Max(Math.Max(color.R, color.G), color.B) / 255;
            //    var min = (double)Math.Min(Math.Min(color.R, color.G), color.B) / 255;
            //    return (max + min) / 2;
            //}

            //public static double Chroma(this Color color)
            //{
            //    var max = (double)Math.Max(Math.Max(color.R, color.G), color.B) / 255;
            //    var min = (double)Math.Min(Math.Min(color.R, color.G), color.B) / 255;
            //    return max - min;
            //}

            //public static double SaturationHSL(this Color color)
            //{
            //    var max = (double)Math.Max(Math.Max(color.R, color.G), color.B) / 255;
            //    var min = (double)Math.Min(Math.Min(color.R, color.G), color.B) / 255;
            //    var chroma = max - min;

            //    var lightness = (max + min) / 2;
            //    if(lightness <= .5)
            //    {
            //        return chroma / (2 * lightness);
            //    }
            //    return chroma / (2 - 2 * lightness);
            //}

            //public static Color WithAlpha(this Color color, byte alpha)
            //{
            //    return Color.FromArgb(alpha, color.R, color.G, color.B);
            //}

            //public static Color WithR(this Color color, byte r)
            //{
            //    return Color.FromArgb(color.A, r, color.G, color.B);
            //}

            //public static Color WithG(this Color color, byte g)
            //{
            //    return Color.FromArgb(color.A, color.R, g, color.B);
            //}

            //public static Color WithB(this Color color, byte b)
            //{
            //    return Color.FromArgb(color.A, color.R, color.G, b);
            //}

            #endregion

            public static DataGrid ToDataGrid(this IEnumerable<object> lst) { return new DataGrid() { ItemsSource = lst }; }
            public static ListBox ToListBox(this IEnumerable<object> lst) { return new ListBox() { ItemsSource = lst }; }
            public static ComboBox ToComboBox(this IEnumerable<object> lst) { return new ComboBox() { ItemsSource = lst }; }

        }
    }
}