using System;
using System.Collections.Generic;

namespace TextAdventure
{
    public class Crafting
    {

        public enum CraftingRecepies
        {
            NULL,
            NICE_SOUP,
            WOOD_CHIPPINGS,
            TWIG,
            MANA_POTION,
            HEALTH_POTION,
            NICE_SALAD,
            XP_VIAL,
            STANIMA_POTION
        }

        public Dictionary<CraftingRecepies, Dictionary<Player.Items, int>> recepies = new Dictionary<CraftingRecepies, Dictionary<Player.Items, int>>
        {
            {
                CraftingRecepies.NICE_SOUP,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.PLUM, 20},
                    {Player.Items.APPLE, 10},
                    {Player.Items.BERRY, 5}
                }
            },
            {
                CraftingRecepies.NICE_SALAD,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.PLUM, 25},
                    {Player.Items.APPLE, 25},
                    {Player.Items.BERRY, 25},
                    {Player.Items.EGGPLANT, 5},
                    {Player.Items.HONEY, 5}
                }
            },
            {
                CraftingRecepies.WOOD_CHIPPINGS,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.WOOD, 2}
                }
            },
            {
                CraftingRecepies.TWIG,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.WOOD, 5}
                }
            },
            {
                CraftingRecepies.MANA_POTION,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.PURE_MANA, 50}
                }
            },
            {
                CraftingRecepies.HEALTH_POTION,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.PURE_OXYGEN, 50}
                }
            },
            {
                CraftingRecepies.STANIMA_POTION,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.LIQUID_STANIMA, 10}
                }
            },
            {
                CraftingRecepies.XP_VIAL,
                new Dictionary<Player.Items, int>(){
                    {Player.Items.LIQUID_EXPERIENCE, 5}
                }
            },
        };

        public Dictionary<CraftingRecepies, Player.Items> craftingRewards = new Dictionary<CraftingRecepies, Player.Items>
        {
            { CraftingRecepies.NICE_SOUP, Player.Items.NICE_SOUP },
            { CraftingRecepies.WOOD_CHIPPINGS, Player.Items.WOOD_CHIPPINGS },
            { CraftingRecepies.TWIG, Player.Items.TWIG },
            { CraftingRecepies.MANA_POTION, Player.Items.MANA_POTION },
            { CraftingRecepies.HEALTH_POTION, Player.Items.HEALTH_POTION },
            { CraftingRecepies.NICE_SALAD, Player.Items.NICE_SALAD },
            { CraftingRecepies.XP_VIAL, Player.Items.XP_VIAL},
            { CraftingRecepies.STANIMA_POTION, Player.Items.STANIMA_POTION},
        };

        public void craft(CraftingRecepies recepie, ref Player player)
        {

            var craftingRecepie = recepies[recepie];

            foreach (KeyValuePair<Player.Items, int> item in craftingRecepie)
                player.removeItem(item.Key, false, item.Value);

            if (!craftingRewards.ContainsKey(recepie))
                player.addXP(100);

            player.addItem(craftingRewards[recepie]);
        }

        public Player.Items getReward(CraftingRecepies recepie)
        {

            return (craftingRewards[recepie]);
        }
        public Dictionary<Player.Items, int> getRecepie(CraftingRecepies recepie)
        {

            return (recepies[recepie]);
        }

        public CraftingRecepies getCraftingRecepie(string recepie_name)
        {

            var names = Enum.GetNames(typeof(CraftingRecepies));

            int key = 0;
            foreach (string name in names)
            {

                if (name.ToLower() == recepie_name.ToLower())
                    return (CraftingRecepies)key;

                key++;
            }

            return CraftingRecepies.NULL;
        }

        public bool craftingRecepieExists(string recepie_name)
        {

            var names = Enum.GetNames(typeof(CraftingRecepies));

            foreach (string name in names)
                if (name.ToLower() == recepie_name.ToLower())
                    return true;

            return false;
        }

        public bool canCraft(CraftingRecepies recepie, ref Player player)
        {

            if (!recepies.ContainsKey(recepie))
                return false;

            var craftingRecepie = recepies[recepie];

            foreach (KeyValuePair<Player.Items, int> item in craftingRecepie)
            {

                if (player.hasItem(item.Key) == false)
                    return false;

                if (player.getItemQuantity(item.Key) - item.Value < 0)
                    return false;
            }

            return true;
        }
    }
}
