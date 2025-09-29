using PdfSharp.Fonts;

namespace Ovia.Services
{
    public class CustomFontResolver : IFontResolver
    {
        public byte[]? GetFont(string faceName)
        {
            if (faceName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                // Specify the relative or absolute path to the Arial font file
                string fontFilePath = "Fonts/ArialTh.ttf"; // Update this with the correct path

                // Load the font file and return its bytes
                return File.ReadAllBytes(fontFilePath);
            }
            // Return null if the requested font is not found
            return null;


        }

        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            return new FontResolverInfo("Arial");
        }
    }
}
