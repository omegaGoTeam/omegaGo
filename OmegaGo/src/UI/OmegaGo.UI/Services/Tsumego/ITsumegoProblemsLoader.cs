using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Tsumego
{
    public interface ITsumegoProblemsLoader
    {
        Task<IReadOnlyCollection<TsumegoProblemInfo>> GetProblemListAsync();
        Task<TsumegoProblem> GetProblemAsync(TsumegoProblemInfo problemInfo);
    }
}
