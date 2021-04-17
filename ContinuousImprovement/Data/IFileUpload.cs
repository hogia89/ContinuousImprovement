using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousImprovement.Data
{
    public interface IFileUpload
    {
        Task Upload(IFileListEntry file,string fileName,string beforeOrAfter);
    }
}
