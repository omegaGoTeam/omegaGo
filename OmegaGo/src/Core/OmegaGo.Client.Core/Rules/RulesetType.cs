namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Type of the ruleset.
    /// There are many official rulesets for playing Go. These vary in significant ways, such as the method used to count the final score, counting the compensation (komi), atd..
    /// </summary>
    public enum RulesetType
    {
        /// <summary>
        /// These are used by the American Go Association.
        /// Rules: http://www.usgo.org/files/pdf/completerules.pdf
        /// </summary>
        AGA,
        /// <summary>
        /// This is the other major set of rules in widespread use, also known as "area" rules.
        /// Rules: https://docs.google.com/document/d/1bslniW1jrpUAs2o5xJJiXKol8v-E0-mxqowa3LZ8Kt0/edit
        /// </summary>
        Chinese,
        /// <summary>
        /// These are rules used in Japan, sometimes known as "territory" rules. 
        /// Rules: http://www.cs.cmu.edu/~wjh/go/rules/Japanese.html
        /// </summary>
        Japanese
    }
}
