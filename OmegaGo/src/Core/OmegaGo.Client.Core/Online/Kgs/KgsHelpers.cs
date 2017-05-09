using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Kgs
{
    public static class KgsHelpers
    {
        public static RulesetType ConvertRuleset(string rules)
        {
            switch (rules)
            {
                case RulesDescription.RulesAga:
                    return RulesetType.AGA;
                case RulesDescription.RulesChinese:
                    return RulesetType.Chinese;
                case RulesDescription.RulesJapanese:
                    return RulesetType.Japanese;
            }
            throw new Exception("This ruleset is not supported in Omega Go.");
        }

        public static bool IsSupportedRuleset(string rules)
        {
            return rules == RulesDescription.RulesAga ||
                   rules == RulesDescription.RulesChinese ||
                   rules == RulesDescription.RulesJapanese;
        }
    }
}
