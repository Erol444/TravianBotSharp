using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbsCrossPlatform.Models.Enums
{
    public enum HeroItemCategory
    {
        Helmet,
        Weapon,
        Left,
        Armor,
        Boots,
        Horse,
        Resource,
        Stackable,
        NonStackable,
        Others,
    }

    public enum HeroItemEnum
    {
        Others_None_0 = 0,
        Helmet_Experience_1,
        Helmet_Experience_2,
        Helmet_Experience_3,
        Helmet_Regeneration_1,
        Helmet_Regeneration_2,
        Helmet_Regeneration_3,
        Helmet_CulturePoints_1,
        Helmet_CulturePoints_2,
        Helmet_CulturePoints_3,
        Helmet_Cavalry_1,
        Helmet_Cavalry_2,
        Helmet_Cavalry_3,
        Helmet_Infantry_1,
        Helmet_Infantry_2,
        Helmet_Infantry_3,

        // Roman weapons
        Weapon_Legionnaire_1, // =16

        Weapon_Legionnaire_2,
        Weapon_Legionnaire_3,
        Weapon_Praetorian_1,
        Weapon_Praetorian_2,
        Weapon_Praetorian_3,
        Weapon_Imperian_1,
        Weapon_Imperian_2,
        Weapon_Imperian_3,
        Weapon_EquitesImperatoris_1,
        Weapon_EquitesImperatoris_2,
        Weapon_EquitesImperatoris_3,
        Weapon_EquitesCaesaris_1,
        Weapon_EquitesCaesaris_2,
        Weapon_EquitesCaesaris_3,

        // Gaul weapons
        Weapon_Phalanx_1, // =31

        Weapon_Phalanx_2,
        Weapon_Phalanx_3,
        Weapon_Swordsman_1,
        Weapon_Swordsman_2,
        Weapon_Swordsman_3,
        Weapon_TheutatesThunder_1,
        Weapon_TheutatesThunder_2,
        Weapon_TheutatesThunder_3,
        Weapon_Druidrider_1,
        Weapon_Druidrider_2,
        Weapon_Druidrider_3,
        Weapon_Haeduan_1,
        Weapon_Haeduan_2,
        Weapon_Haeduan_3,

        // Teuton weapons
        Weapon_Clubswinger_1, // =46

        Weapon_Clubswinger_2,
        Weapon_Clubswinger_3,
        Weapon_Spearman_1,
        Weapon_Spearman_2,
        Weapon_Spearman_3,
        Weapon_Axeman_1,
        Weapon_Axeman_2,
        Weapon_Axeman_3,
        Weapon_Paladin_1,
        Weapon_Paladin_2,
        Weapon_Paladin_3,
        Weapon_TeutonicKnight_1,
        Weapon_TeutonicKnight_2,
        Weapon_TeutonicKnight_3,

        // Left-hand items
        Left_Map_1, // =61

        Left_Map_2,
        Left_Map_3,
        Left_Pennant_1,
        Left_Pennant_2,
        Left_Pennant_3,
        Left_Standard_1,
        Left_Standard_2,
        Left_Standard_3,
        Left_Spyglasses_1, // Aren't in the game
        Left_Spyglasses_2, // Aren't in the game
        Left_Spyglasses_3, // Aren't in the game
        Left_Pouch_1,
        Left_Pouch_2,
        Left_Pouch_3,
        Left_Shield_1,
        Left_Shield_2,
        Left_Shield_3,
        Left_Horn_1,
        Left_Horn_2,
        Left_Horn_3,

        // Armors
        Armor_Regeneration_1, // =82

        Armor_Regeneration_2,
        Armor_Regeneration_3,
        Armor_Scale_1,
        Armor_Scale_2,
        Armor_Scale_3,
        Armor_Breastplate_1,
        Armor_Breastplate_2,
        Armor_Breastplate_3,
        Armor_Segmented_1,
        Armor_Segmented_2,
        Armor_Segmented_3,

        // Boots
        Boots_Regeneration_1, // =94

        Boots_Regeneration_2,
        Boots_Regeneration_3,
        Boots_Mercenery_1,
        Boots_Mercenery_2,
        Boots_Mercenery_3,
        Boots_Spurs_1,
        Boots_Spurs_2,
        Boots_Spurs_3,

        // Horses
        Horse_Horse_1, // =103

        Horse_Horse_2,
        Horse_Horse_3,

        // Others
        Stackable_Ointment_0, // =106

        Stackable_Scroll_0,
        Stackable_Bucket_0,
        Stackable_Tablets_0,
        Stackable_Book_0, // =110
        Stackable_Artwork_0,
        Stackable_SmallBandage_0,
        Stackable_BigBandage_0,
        Stackable_Cage_0,

        // Egyptian weapons
        Weapon_SlaveMilitia_1, // =115

        Weapon_SlaveMilitia_2,
        Weapon_SlaveMilitia_3,
        Weapon_AshWarden_1,
        Weapon_AshWarden_2,
        Weapon_AshWarden_3,
        Weapon_KhopeshWarrior_1,
        Weapon_KhopeshWarrior_2,
        Weapon_KhopeshWarrior_3,
        Weapon_AnhurGuard_1,
        Weapon_AnhurGuard_2,
        Weapon_AnhurGuard_3,
        Weapon_ReshephChariot_1,
        Weapon_ReshephChariot_2,
        Weapon_ReshephChariot_3,

        // Hun weapons
        Weapon_Mercenary_1, // =130

        Weapon_Mercenary_2,
        Weapon_Mercenary_3,
        Weapon_Bowman_1,
        Weapon_Bowman_2,
        Weapon_Bowman_3,
        Weapon_SteppeRider_1,
        Weapon_SteppeRider_2,
        Weapon_SteppeRider_3,
        Weapon_Marksman_1,
        Weapon_Marksman_2,
        Weapon_Marksman_3,
        Weapon_Marauder_1,
        Weapon_Marauder_2,
        Weapon_Marauder_3,

        Resource_Wood_0,
        Resource_Clay_0,
        Resource_Iron_0,
        Resource_Crop_0 // =148
    }
}