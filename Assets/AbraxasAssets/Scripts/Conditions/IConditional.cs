using System.Collections.Generic;

namespace Abraxas.Stones.Conditions
{
    internal interface IConditional
    {
        public List<ICondition> Conditions { get; set; }
    }
}
