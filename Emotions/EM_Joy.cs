﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RealSense.Emotions
{
    class EM_Joy : RSModule
    {
        // Default values
        public EM_Joy()
        {
            debug = true;
        }

        /*
         *  1 = inner brow raised -> BrowShift
            2 = outer brow raised -> BrowShift
            4 = brow lowered -> BrowShift
            5 = upper lid raised -> EyelidTight
            6 = cheeck raised -> CheeckRaised (not working)
            7 = lid tightened -> EyelidTight
            9 = nose wrinkled -> NoseWrinkled
            12 = lip corner pulled (up) -> LipCorner
            14 = grübchen -> none
            15 = lip corner lowered -> LipLine
            16 = lower lip lowered ->LowerLipLowered
            20 = lip stretched -> LipStretched
            23 = lip tightened -> LipsTightened
            26 = jaw drop -> JawDrap

            Verachtung (12 (R,L), 14(R,L)
            Trauer (1,4,15,(20?)
            Wut (4,5,6,23)
            Ekel (9,15,16,4)
            Überraschung (1,2,5B,26)
            Freude (6,12, 7)
            Angst (1,2,4,5,6,20,26)

         * */

        public override void Work(Graphics g)
        {
            //Joy --> EyelidTight, LipCorner

            //percentage Joy
            int p_lid = 20;
            int p_lip = 80;

            //lid Value 0 - -100 (Grenze bei lidMax)
            double temp_left = model.AU_Values[typeof(ME_EyelidTight).ToString() + "_left"];
            double temp_right = model.AU_Values[typeof(ME_EyelidTight).ToString() + "_right"];
            double lidValue = temp_left > temp_right ? temp_left : temp_right;
            if (model.Test) lidValue = (temp_left + temp_right) / 2;
            lidValue = lidValue * -1 * p_lid / 100;

            //lip Value 0 - 100
            temp_left = Math.Abs(model.AU_Values[typeof(ME_LipCorner).ToString() + "_left"]);
            temp_right = Math.Abs(model.AU_Values[typeof(ME_LipCorner).ToString() + "_right"]);

            double lipValue = (temp_left + temp_right) / 2;
            lipValue = lipValue * p_lip / 100;



            if (model.AU_Values[typeof(ME_LowerLipLowered).ToString()] < -50)
            {
                lipValue *= 1.5;
            }

            //lipL Value 0 - -100
            double lipLValue = model.AU_Values[typeof(ME_LipLine).ToString()];
            lipLValue = lipLValue * p_lip / 100;

            //brow Value
            temp_left = model.AU_Values[typeof(ME_BrowShift).ToString() + "_left"];
            temp_right = model.AU_Values[typeof(ME_BrowShift).ToString() + "_right"];

            // Falls Corners durch Disgust, auf 0 setzen
            double hDiff = model.DifferenceByAxis(33, 35, Model.AXIS.Y, false) + model.DifferenceByAxis(39, 37, Model.AXIS.Y, false);

            if (hDiff < 0)
            {
                lipValue = 0;
            }

            double finalLipValue = lipValue > lipLValue ? lipValue : lipLValue;

            double joy = lidValue + finalLipValue;// + browValue;

            // subtrac brows down

            double browValue = temp_left > temp_right ? temp_left : temp_right;
            browValue = browValue * -1 * 50 / 100;
            joy -= browValue;

            joy = joy > 0 ? joy : 0;

            model.Emotions["Joy"] = joy;

            // print debug-values 
            if (debug)
            {
                output = "Joy: " + (int)joy + " LipCorner: " + (int)lipValue + " LipLine: " + (int)lipLValue + " Eye: " + (int)lidValue + ", hDiff: " + hDiff; // + " Brow: " + browValue;
            }

        }
    }
}






