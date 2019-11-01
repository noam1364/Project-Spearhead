using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using Project_Spearhead;


public static class Global
{
    public static ContentManager cm;
    public static SpriteBatch sb;
    public static int winWidth, winHeight;
    public static Obsticle[] rock;
    public static IMainClass game;
    public static string fileUrl = "C:/workspaces/C# workspace/Project Spearhead/Project Spearhead/AI";
    public static string leaderboardFile = "C:/workspaces/C# workspace/Project Spearhead/Project Spearhead" +
               "/Content/bin/Windows/Leaderboard.txt";
    public enum gameFlow
    {
        startMenu, gameOn, gameOver
    };
    public static Vector<double> getMask(Bird b)
    {
        int scale = 10,width = winWidth/scale,height = winHeight/scale;
        Vector<double> mask = Vector<double>.Build.Dense((width/2)*height);
        for(int i=0;i<width/2;i++)
        {
            for(int j=0;j<height;j++)
            {
                foreach(Obsticle r in rock) ///rock is 1
                    if(r.contains(i*scale, j*scale))
                        mask[i*height+j] = 1;
                    if(b.contains(i*scale, j*scale))
                        mask[i * height + j] = -1;
            }
        }
        return mask;
    }
    public interface IMainClass
    {
        void restart();
    }
}



       