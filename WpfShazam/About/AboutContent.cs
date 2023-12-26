using System.Collections.Generic;

namespace WpfShazam.About;

// Content displayed on About tab
public class AboutContent
{
    public string Error { get; set; } = string.Empty;
    public bool HasError => !string.IsNullOrWhiteSpace(Error);
    public List<AboutParagraph> ParagraphList { get; set; } = new List<AboutParagraph>();
}

public class AboutParagraph
{
    public string Header { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}
