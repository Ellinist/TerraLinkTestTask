using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TerraLinkTestTask.Interfaces
{
    public interface IExternalSystemConnector
    {
        Task SendDocuments(IReadOnlyCollection<Document> documents, CancellationToken cancellationToken);
    }
}
