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

        private HashSet<string> _solvedTsumegosCache;

        public IEnumerable<string> SolvedProblems
        {
            get
            {
                if (_solvedTsumegosCache == null)
                {
                    _solvedTsumegosCache = GetComplexSetting(nameof(this.SolvedProblems),
                        () => new HashSet<string>());
                }
                return _solvedTsumegosCache;
            }
            set
            {
                HashSet<string> list = new HashSet<string>();
                foreach(string s in value)
                {
                    list.Add(s);
                }
                SetComplexSetting(nameof(this.SolvedProblems), list);
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
