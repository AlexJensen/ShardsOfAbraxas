using System.Collections.Generic;
using System;

[Serializable]
public class ConditionData
{
    public string ConditionType;
    public bool IsTrigger;
    public List<TargetData> Targets;  // Serialize targets, if any
    public List<ConditionData> NestedConditions; // Serialize nested conditions, if any
}
