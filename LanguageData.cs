﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PronoesProMod
{
    class LanguageData
    {

        public static Dictionary<string, string> englishSentences = new Dictionary<string, string>()
        {

            //Location names
            {"69420","Crafted Town" },

            //NPCs

            //1: PRO
            {"Pronoespro_MAIN","Pronoespro" },
            {"Pronoespro_SUPER","Forgotten dreamer" },
            {"Pronoespro_SUB","or Prono/Pro to his friends" },
            {"prono_welcome_0","Welcome, ghost of Hallownest to my dream world.\nA paradise for me and anyone that manages to get here" },
            {"prono_welcome_1","Or something like that" },
            {"prono_welcome_2","Me and my partner here wanted to help you in your journey" },
            {"prono_welcome_3","If you need anything, be not afraid to ask" },
            {"prono_towncreated_0","This town is the labour of a lot of work from Dee and I..." },
            {"prono_towncreated_1","Honestly, I forgot how the world even looked like until you came to Hallownest" },
            {"prono_towncreated_2","Well, I just hope you enjoy it here... if you even can, with no mind and all that" },

            {"prono_upgrade_charm_dash_0","Oh, look at that charm! 'Dashmaster', right?" },
            {"prono_upgrade_charm_dash_1","Let me upgrade it for you, so you can explore with more ease" },
            {"prono_upgrade_charm_dash_2","OK, now it's done. Have fun dashing around with it!" },
            {"prono_upgrade_charm_dream_shield_0","Oh, look at that charm! 'Dream Shield'?" },
            {"prono_upgrade_charm_dream_shield_1","This could be a little different, let me have a go at it" },
            {"prono_upgrade_charm_dream_shield_2","There you go!, now it's... different" },

            {"prono_dreamnail_0","What a wonderfull tool, wish I had something interesting to say." },

            //Interactables

            { "UltimateBench1","It's a flat sign, it's not even a bench!"},
            { "UltimateBench2","It doesn't even look good to sit on"},

            //Spells

            //1: Souls
            {"Soul_sawblade_0","Help of Sawblades" },
            {"Soul_sawblade_0_desc","Summon a sawblade that sticks to walls and shreds your foes.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },
            
            {"Soul_sawblade_1","Path of Sawblades" },
            {"Soul_sawblade_1_desc","Summon a couple sawblades that stick to walls and shred your foes.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Soul_nailmaster_0","Nailmaster's Aid" },
            {"Soul_nailmaster_0_desc","Summon two nails to hit your enemies from afar.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Soul_nailmaster_1","Nailmaster's Soul" },
            {"Soul_nailmaster_1_desc","Summon three nails to hit your enemies from afar.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Soul_apple_0", "Apple Slice"},
            {"Soul_apple_0_desc", "Summon a slice of apple... Don't know why.\n \nRequires SOUL to conjure, strike enemies to gather SOUL."},
            
            {"Soul_apple_1", "Apple Pie"},
            {"Soul_apple_1_desc", "Summons an apple pie... Again, why?\n \nRequires SOUL to conjure, strike enemies to gather SOUL."},

            {"Soul_Dee_0","Dee-Big explosion" },
            {"Soul_Dee_0_desc","Summon a Dee clone to hit on your enemies.\n \nRequires SOUL to conjure, strike enemies to gather SOUL."},

            {"Soul_Dee_1","Dee-Big explosion" },
            {"Soul_Dee_1_desc","Summon a Dee clone to explode on your enemies.\n \nRequires SOUL to conjure, strike enemies to gather SOUL."},

            //2: Dives
            {"Dive_sawblade_0","Sawblade's Begining" },
            {"Dive_sawblade_0_desc","Creates two sawblades to shred your enemies when you hit the ground.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Dive_sawblade_1","Sawblade's Ending" },
            {"Dive_sawblade_1_desc","Creates two big sawblades to shred your enemies when you hit the ground.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Dive_nailmaster_0","Nailmaster's Fall" },
            {"Dive_nailmaster_0_desc","Summons four nails that fall alongside you.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Dive_nailmaster_1","Nailmaster's Request" },
            {"Dive_nailmaster_1_desc","Summons four nails that fall alongside you and creates a giant one when you hit the ground.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Dive_apple_0","Apple Fall"},
            {"Dive_apple_0_desc","Apple Fall\n \nRequires SOUL to conjure, strike enemies to gather SOUL."},

            {"Dive_apple_1","Gravity Rush"},
            {"Dive_apple_1_desc","Because an apple fell on a guy's head, and that's gravity and this is...\n \nRequires SOUL to conjure, strike enemies to gather SOUL."},

            //3: Shriek
            {"Shriek_sawblade_0","Sawblade's Lament" },
            {"Shriek_sawblade_0_desc","Creates sawblades I guess...?\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Shriek_sawblade_1","Sawblade May Cry" },
            {"Shriek_sawblade_1_desc","Creates bigger sawblades I guess...?\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Shriek_nailmaster_0","Nailmaster's Wrath" },
            {"Shriek_nailmaster_0_desc","Creates a barrage of nails to stab your enemies.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Shriek_nailmaster_1","Nailmaster's Wrath" },
            {"Shriek_nailmaster_1_desc","Creates a giant manifestation of nails to pierce your enemies.\nPIERCE THE HEAVENS!\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Shriek_apple_0","Aplple's Call" },
            {"Shriek_apple_0_desc","Summon an apple that can be hit towards enemies.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            {"Shriek_apple_1","Aplple's Summoning" },
            {"Shriek_apple_1_desc","Summon a big apple that can be send flying towards enemies.\n \nRequires SOUL to conjure, strike enemies to gather SOUL." },

            //Upgrades

            //1: Dashmaster
            {"DashmasterUpgrade_Name","Upgraded Dashmaster" },
            {"DashmasterUpgrade_Desc","Bears the likeness of an eccentric bug known only as 'The Dashmaster', but even weirder.\n\nThe bearer will be able to dash more often and slightly up or directly downwards, and crystal dash will charge faster. Perfect for those who want to move around at the speed of sound!" },

            {"DeeShieldUpgrade_Name","Dee Shield" },
            {"DeeShieldUpgrade_Desc","Dream Shield, but now with a 100% more Dee!\n\nWill summon a Dream Shield that can be targetted and will shoot out a wave of light in it's direction. Can be used downwards to get a big jump fast." }
        };

    }
}
