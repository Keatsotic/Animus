﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;


namespace AnimusEngine
{
    public static class NPCLookUp
    {
        static private GameObject npcObj;

        public static GameObject NpcLUT(string npcName, Vector2 initPosition)
        {
            switch (npcName)
            {
                case "tinyCaro":
                    npcObj = new NPC(initPosition, npcName);
                    break;
                
                default:
#if DEBUG
                    Console.WriteLine("got nuthin, stupid");
#endif                    
                    break;
            }
            return npcObj;
        }
    }
}
