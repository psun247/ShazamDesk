using System;
using System.Linq;
using System.IO;
using System.Reflection;
using WpfShazam.Settings;
using WpfShazam.Main;

namespace WpfShazam.About;

public partial class AboutViewModel : BaseViewModel
{
    public AboutViewModel(ILocalSettingsService localsettingsService)
                            : base(localsettingsService)
    {
        LoadContentForAboutTab();
    }

    public AboutContent AboutTabContent { get; } = new AboutContent();

    public void OnAboutTabActivated()
    {
        AppSettings.SelectedTabName = AppSettings.AboutTabName;
    }

    private void LoadContentForAboutTab()
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            string[] names = asm.GetManifestResourceNames();
            string? path = names.FirstOrDefault(x => x == "WpfShazam.Assets.AboutTabContent.txt");
            if (path != null)
            {
                using (Stream? stream = asm.GetManifestResourceStream(path))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string[] lines = reader.ReadToEnd().Split("\r\n").Where(x => !x.StartsWith("--")).ToArray();
                            AboutParagraph? paragraph = null;
                            foreach (string line in lines)
                            {
                                if (line.StartsWith("=="))
                                {
                                    paragraph = new AboutParagraph
                                    {
                                        Header = line.Remove(0, 2)
                                    };
                                    AboutTabContent.ParagraphList.Add(paragraph);
                                }
                                else if (paragraph != null)
                                {
                                    paragraph.Detail = $"{paragraph.Detail}\r\n{line}";
                                }
                            }
                        }
                    }
                }
            }

            if (AboutTabContent.ParagraphList.Count == 0)
            {
                AboutTabContent.Error = "About content not found";
            }
        }
        catch (Exception ex)
        {
            AboutTabContent.Error = $"Error: {ex.Message}";
        }
    }
}
