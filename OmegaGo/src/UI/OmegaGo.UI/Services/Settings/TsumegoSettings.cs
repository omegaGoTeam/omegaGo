using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Settings
{
    public class TsumegoSettings : SettingsGroup
    {
        public TsumegoSettings(ISettingsService service) : base("Tsumego", service)
        {
        }

        private List<string> _solvedTsumegosCache;

        public IEnumerable<string> SolvedProblems
        {
            get
            {
                if (_solvedTsumegosCache == null)
                {
                    _solvedTsumegosCache = GetComplexSetting(nameof(SolvedProblems),
                        () => new List<string>());
                }
                return _solvedTsumegosCache;
            }
            set
            {
                var list = value.ToList();                
                SetComplexSetting(nameof(SolvedProblems), list);
                _solvedTsumegosCache = list;
            }           
        }
        
        public bool ShowPossibleMoves
        {
            get { return GetSetting(nameof(ShowPossibleMoves), () => true); }
            set { SetSetting(nameof(ShowPossibleMoves), value); }
        }
    }
}
