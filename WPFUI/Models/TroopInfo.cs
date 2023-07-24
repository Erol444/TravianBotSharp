using MainCore;
using MainCore.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFUI.Models
{
    public class TroopInfo : ReactiveObject
    {
        public TroopInfo(TroopEnums troop) => Troop = troop;

        public TroopEnums Troop { get; }

        public CroppedBitmap Image => this.GetBitmap();
    }

    public static class TroopInfoExtensions
    {
        public static readonly Dictionary<int, int> ImageOffset = new()
        {
            {1 , 0 },
            {2, 19 },
            {3, 38 },
            {4, 57 },
            {5, 76 },
            {6, 95 },
            {7, 114},
            {8, 133},
            {9, 152},
            {0, 171},
        };

        public static readonly Dictionary<TribeEnums, string> TribeImage = new()
        {
            {TribeEnums.Romans, "roman.png" },
            {TribeEnums.Teutons, "teuton.png" },
            {TribeEnums.Gauls, "gaul.png" },
            {TribeEnums.Nature, "nature.png" },
            {TribeEnums.Natars, "natar.png" },
            {TribeEnums.Egyptians, "egyptian.png" },
            {TribeEnums.Huns, "huns.png" },
        };

        public static int GetImageOffset(TroopEnums troop)
        {
            return ImageOffset[(int)troop % 10];
        }

        public static CroppedBitmap GetBitmap(this TroopInfo troopInfo)
        {
            if (troopInfo.Troop == TroopEnums.None)
            {
                return null;
            }
            else
            {
                var tribe = troopInfo.Troop.GetTribe();
                if (tribe == TribeEnums.Any) return null;
                var pathImage = TribeImage[tribe];
                var sourceImage = new BitmapImage(new Uri($"pack://application:,,,/Resources/{pathImage}"));
                return new CroppedBitmap(sourceImage, new Int32Rect(GetImageOffset(troopInfo.Troop), 0, 16, 16));
            }
        }
    }
}