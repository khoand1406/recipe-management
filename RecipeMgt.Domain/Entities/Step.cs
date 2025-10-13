using System;
using System.Collections.Generic;

namespace RecipeMgt.Domain.Entities;

public partial class Step
{
    public int StepId { get; set; }

    public int RecipeId { get; set; }

    public int StepNumber { get; set; }

    public string Instruction { get; set; }

    public virtual Recipe Recipe { get; set; }
}
