using System.Collections.Generic;

namespace AmandsSense.Sense;

public class ItemsJsonClass
{
    public List<string> RareItems { get; set; } = [];
    public List<string> KappaItems { get; set; } = [];
    public List<string> NonFleaExclude { get; set; } = [];

    public void Validate()
    {
        RareItems ??= [];
        KappaItems ??= [];
        NonFleaExclude ??= [];
    }
}
