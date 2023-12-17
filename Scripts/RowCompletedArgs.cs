using System;
using System.Collections.Generic;

public class RowCompletedArgs : EventArgs
{
    public RowCompletedArgs(List<float> completedRow)
    {
        CompletedRow = completedRow;
    }

    public List<float> CompletedRow { get; }
}