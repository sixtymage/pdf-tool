using System.Reflection;

namespace PdfMerger.Helpers
{
  public static class AppInfo
  {
    public static string Version
    {
      get
      {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version != null ? $"v{version.Major}.{version.Minor}" : "v1.1";
      }
    }

    public static string FullVersion
    {
      get
      {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetName().Version?.ToString() ?? "1.1.0.0";
      }
    }

    public static string Title => "PDF Merger";

    public static string TitleWithVersion => $"{Title} {Version}";
  }
}