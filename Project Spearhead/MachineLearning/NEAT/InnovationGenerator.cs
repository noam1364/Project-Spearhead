﻿public static class InnovationGenerator
{
    private static int innovation = 0;
    public static int GetInnovation()
    {
        innovation++;
        return innovation;
    }
    public static void SetInnovation(int inno)
    {
        if(inno>innovation)
        {
            innovation = inno;
        }
    }
}