using LangChain.Base;
using System.Text;
using LangChain.Docstore;

namespace LangChain.DocumentLoaders;

public class TextLoader : BaseLoader
{
    private string filePath;
    private Encoding fileEncoding;
    private bool autoDetectEncoding;
    /// <summary>
    /// Load text file.
    /// </summary>
    /// <param name="filePath">Path to the file to load.</param>
    /// <param name="encoding">File encoding to use. Null for default</param>
    /// <param name="autoDetectEncoding">Whether to try to autodetect the file encoding
    /// if the specified encoding fails.</param>
    public TextLoader(string filePath, Encoding encoding = null, bool autoDetectEncoding = false)
    {
        this.filePath = filePath;
        this.fileEncoding = encoding ?? Encoding.Default;
        this.autoDetectEncoding = autoDetectEncoding;
    }

    public override List<Document> Load()
    {
        var text = "";

        try
        {
            using (var reader = new StreamReader(filePath, fileEncoding))
            {
                text = reader.ReadToEnd();
            }
        }
        catch (DecoderFallbackException)
        {
            if (autoDetectEncoding)
            {
                // todo: change this to a more robust solution
                // bruteforce encoding detection
                var encodings = new[] { Encoding.UTF8, Encoding.ASCII, Encoding.Unicode };
                foreach (var encoding in encodings)
                {
                    try
                    {
                        using (var reader = new StreamReader(filePath, encoding))
                        {
                            text = reader.ReadToEnd();
                        }
                        break;
                    }
                    catch (DecoderFallbackException)
                    {
                        continue;
                    }
                }
            }
            else
            {
                throw new Exception($"Error loading {filePath}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading {filePath}", ex);
        }

        var metadata = new Dictionary<string, object>
        {
            { "source", filePath }
        };

        return new List<Document> { new Document(text, metadata) };
    }
}