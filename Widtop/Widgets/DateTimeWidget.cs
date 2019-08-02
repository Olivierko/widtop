﻿using System;
using System.Drawing;

namespace Widtop.Widgets
{
    public class DateTimeWidget : Widget
    {
        private static Rectangle Area => new Rectangle(640, 770, 220, 200);
        private static Font PrimaryFont => new Font("Agency FB", 72);
        private static Font SecondaryFont => new Font("Agency FB", 30);
        private static SolidBrush PrimaryBrush => new SolidBrush(Color.White);
        private static SolidBrush SecondaryBrush => new SolidBrush(Color.DimGray);
        private static StringFormat PrimaryFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };
        private static StringFormat SecondaryFormat => new StringFormat { LineAlignment = StringAlignment.Far, Alignment = StringAlignment.Far };

        private static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        public override void Render(Graphics graphics)
        {
            var now = DateTime.Now;
            var daySuffix = GetDaySuffix(now.Day);

            //graphics.FillRectangle(new SolidBrush(Color.Yellow), Area);

            graphics.DrawString(
                $"{now:HH:mm}", 
                PrimaryFont, 
                PrimaryBrush, 
                Area, 
                PrimaryFormat
            );

            graphics.DrawString(
                $"{now:dddd}{Environment.NewLine}{now.Day}{daySuffix} {now:MMMM}", 
                SecondaryFont, 
                SecondaryBrush, 
                Area, 
                SecondaryFormat
            );
        }
    }
}