using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Domain.Entities.Documentation;

namespace VeloPortal.Application.Interfaces.Documentation
{
    public interface IDocInfDet
    {
        Task<bool> UploadDocument(List<DocInfDet> docInfDets);
    }
}
