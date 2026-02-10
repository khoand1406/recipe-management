using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Worker
{
    public interface IRelateBgJob
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
