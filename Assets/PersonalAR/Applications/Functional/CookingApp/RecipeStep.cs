using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecipeStep
{
    public KitchenLocation LocationType;
    [TextArea]
    public string Instruction;
    // public RecipeStep NextStep;

    // public RecipeStep(KitchenLocation locationType, string instruction, RecipeStep nextStep)
    // {
    //     this.LocationType = locationType;
    //     this.Instruction = instruction;
    //     this.NextStep = nextStep;
    // }
}

// [Serializable]
// public class GetStep : RecipeStep
// {
//     public GetStep(KitchenLocation location, string[] items, RecipeStep nextStep) :
//            base(location, string.Empty, nextStep)
//     {
//         StringWriter writer = new StringWriter();
//         writer.WriteLine("Get:");
//         foreach(string item in items)
//         {
//             writer.WriteLine($"  ☐ {item}");
//         }
//         base.Instruction = writer.ToString();
//     }
// }
