﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;

namespace RealSense
{

    /**
     * This class was buld just to surveillance the cleaning person in our Office. Because they swiped of our notes at the whiteboard
     * GOTTCHA! 
     * 
     * @author David Rosenbusch 
     */ 
    class SurveillanceModule : RSModule
    {
        bool ignore = true;
        SoundPlayer[] simpleSound = { new SoundPlayer(@"C:\Users\David\Downloads\surv\fw3.wav") ,
        new SoundPlayer(@"C:\Users\David\Downloads\surv\fw2.wav") ,
        new SoundPlayer(@"C:\Users\David\Downloads\surv\fw1.wav") ,
        new SoundPlayer(@"C:\Users\David\Downloads\surv\fw4.wav") };

        public void i()
        {
            foreach (SoundPlayer sp in simpleSound) sp.Load();
        }
        int waitDef = 45, wait = 0, num = 0, DifferentPixels = 0; Color secondColor, firstColor;
        public Bitmap first, second;
        public override void Work(Graphics g)
        {
            try
            {
                DifferentPixels = 0;
                if (first == null) first = second;

                for (int i = 0; i < 640; ++i)
                {
                    for (int j = 300; j < 480; ++j)
                    {
                        secondColor = second.GetPixel(i, j);
                        firstColor = first.GetPixel(i, j);

                        DifferentPixels += Math.Abs(firstColor.R - secondColor.R);
                        DifferentPixels += Math.Abs(firstColor.G - secondColor.G);
                        DifferentPixels += Math.Abs(firstColor.B - secondColor.B);
                    }
                }
                int diff = DifferentPixels / 100000;
                Console.WriteLine(diff);
                bool change = diff > 10;
                first = second;
                wait--;
                if (change)
                {
                    if (ignore) { ignore = false; return; }
                    first.Save("F:\\surv\\" + DateTime.Now.ToString("h_mm_ss") + ".png");
                    model.View.save = 30;
                    if (wait <= 0)
                    {
                        simpleSound[num++].Play();
                        wait = waitDef;
                        Console.WriteLine("Reacting");
                        if (num == 4) num = 0;
                    }
                }
            }
            catch (Exception e)
            { }

        }
    }
}