
namespace TerraLinkTestTask.Models
{
    public class DocumentsReport : IProgress<int>
    {
        public int ReportProgress { get; set; } = 0;

        public void Report(int value)
        {
            ReportProgress += value;
        }
    }
}
