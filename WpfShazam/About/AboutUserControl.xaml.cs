using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfShazam.About;

public partial class AboutUserControl : UserControl
{
    private bool _isFirstLoaded;

    public AboutUserControl()
    {
        InitializeComponent();

        Loaded += AboutUserControl_Loaded;
    }

    private void AboutUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is AboutViewModel aboutViewModel)
        {
            if (!_isFirstLoaded)
            {
                _isFirstLoaded = true;

                BuildAboutRichTextBox(aboutViewModel);
                return;
            }

            aboutViewModel.OnAboutTabActivated();
        }
    }

    private void BuildAboutRichTextBox(AboutViewModel aboutViewModel)
    {
        var aboutTabContent = aboutViewModel.AboutTabContent;
        var flowDocument = new FlowDocument();
        Paragraph docParagraph;
        if (aboutTabContent.HasError)
        {
            docParagraph = new Paragraph { FontSize = 18 };
            docParagraph.Inlines.Add(new Run(aboutTabContent.Error));
            flowDocument.Blocks.Add(docParagraph);
        }
        else
        {
            foreach (var paragraph in aboutTabContent.ParagraphList)
            {
                docParagraph = new Paragraph();
                docParagraph.Inlines.Add(new Bold(new Run(paragraph.Header) { FontSize = 20 }));
                docParagraph.Inlines.Add(new Run(paragraph.Detail) { FontSize = 18 });
                flowDocument.Blocks.Add(docParagraph);
            }
        }
        AboutRichTextBox.Document = flowDocument;
    }
}