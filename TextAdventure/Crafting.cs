using System;
using System.Collections.Generic;
using System.Text;

namespace TextAdventure
{
    public class Crafting
    {

        public enum CraftingRecepies
        {
            NULL,
            NICE_SOUP,
            WOOD_CHIPPINGS,
            TWIG
        }

        public Dictionary<CraftingRecepies, Dictionary<Player.Items, int>> recepies = new Dictionary<CraftingRecepies, Dictionary<Player.Items, int>>
        {

        };

        public Dictionary<CraftingRecepies, Player.Items> craftingRewards = new Dictionary<CraftingRecepies, Player.Items>
        {
            { CraftingRecepies.NICE_SOUP, Player.Items.NICE_SOUP },
            { CraftingRecepies.WOOD_CHIPPINGS, Player.Items.WOOD_CHIPPINGS },
            { CraftingRecepies.TWIG, Player.Items.TWIG },
        };

        public Crafting()
        {

            //Hard coded recepies
            recepies.Add(CraftingRecepies.NICE_SOUP, new Dictionary<Player.Items, int>()
            {
                {Player.Items.PLUM, 20},
                {Player.Items.APPLE, 10},
                {Player.Items.BERRY, 5}
            });

            recepies.Add(CraftingRecepies.WOOD_CHIPPINGS, new Dictionary<Player.Items, int>()
            {
                {Player.Items.WOOD, 2}
            });

            recepies.Add(CraftingRecepies.TWIG, new Dictionary<Player.Items, int>()
            {
                {Player.Items.WOOD, 5}
            });
        }

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
