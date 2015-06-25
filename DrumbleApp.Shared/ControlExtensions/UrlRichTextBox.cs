using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Enums;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DrumbleApp.Shared.ControlExtensions
{
    public sealed class UrlRichTextBox : RichTextBox
    {
        private const string UrlPattern = @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UrlRichTextBox), new PropertyMetadata(default(string), TextPropertyChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var richTextBox = (UrlRichTextBox)dependencyObject;
            var text = (string)dependencyPropertyChangedEventArgs.NewValue;
            int textPosition = 0;
            var paragraph = new Paragraph();

            var urlMatches = Regex.Matches(text, UrlPattern);
            foreach (Match urlMatch in urlMatches)
            {
                int urlOccurrenceIndex = text.IndexOf(urlMatch.Value, textPosition, StringComparison.Ordinal);

                if (urlOccurrenceIndex == 0)
                {
                    var hyperlink = new Hyperlink
                    {
                        NavigateUri = new Uri(urlMatch.Value),
                        TargetName = "_blank",
                        Foreground = ThemeColourConverter.GetBrushFromTheme(ThemeColour.DrumbleBlue)
                    };
                    hyperlink.Inlines.Add(urlMatch.Value);
                    paragraph.Inlines.Add(hyperlink);
                    textPosition += urlMatch.Value.Length;
                }
                else
                {
                    paragraph.Inlines.Add(text.Substring(textPosition, urlOccurrenceIndex - textPosition));
                    textPosition += urlOccurrenceIndex - textPosition;
                    var hyperlink = new Hyperlink
                    {
                        NavigateUri = new Uri(urlMatch.Value),
                        TargetName = "_blank",
                        Foreground = ThemeColourConverter.GetBrushFromTheme(ThemeColour.DrumbleBlue)
                    };
                    hyperlink.Inlines.Add(urlMatch.Value);
                    paragraph.Inlines.Add(hyperlink);
                    textPosition += urlMatch.Value.Length;
                }
            }

            if (urlMatches.Count == 0)
            {
                paragraph.Inlines.Add(text);
            }
            richTextBox.Blocks.Clear();
            richTextBox.Blocks.Add(paragraph);
        }
    }
}
