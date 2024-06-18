using Microsoft.Owin.Security;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Model.deployables;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Types;
using System.Numerics;

namespace PuertoRicoAPI.Models
{
    public class Player
    {
        public Player(DataPlayer dataPlayer, GameState gs)
        {
            this.gs = gs;
            this.Index = dataPlayer.Index;
            this.Doubloons = dataPlayer.Doubloons;
            this.Colonists = dataPlayer.Colonists;
            this.Nobles = dataPlayer.Nobles;
            this.VictoryPoints = dataPlayer.VictoryPoints;
            this.TookTurn = dataPlayer.TookTurn;
            this.BuildOrder = dataPlayer.BuildOrder;
            this.Score = dataPlayer.Score;

            this.Buildings = new List<Building>();
            for(int i = 0; i < dataPlayer.Buildings.Count; i++)
            {
                this.Buildings.Add(new Building(dataPlayer.Buildings[i], gs));
            }
            
            this.Plantations = new List<Plantation>();
            for (int i = 0; i < dataPlayer.Plantations.Count; i++)
            {
                this.Plantations.Add(new Plantation(dataPlayer.Plantations[i]));
            }

            this.Goods = new List<Good>();
            for (int i = 0; i < dataPlayer.Goods.Count; i++)
            {
                this.Goods.Add(new Good(dataPlayer.Goods[i]));
            }
        }
        public GameState gs { get; set; }
        public int Index { get; set; }
        public int Doubloons { get; set; }
        public int Colonists { get; set; }
        public int Nobles { get; set; }
        public int VictoryPoints { get; set; }
        public bool TookTurn { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Plantation> Plantations { get; set; }
        public int BuildOrder { get; set; }
        public List<Good> Goods { get; set; }
        public double Score { get; set; }
        public void chargePlayer(int amount)
        {
            Doubloons = Doubloons - amount;
        }

        public int GetGoodCount(GoodType goodType)
        {
            return GetGood(goodType).Quantity;
        }

        public Good GetGood(GoodType goodType)
        {
            return this.Goods.Where(x => x.Type == goodType).ToList()[0];
        }

        public List<GoodType> GetUniqueGoodTypes()
        {
            var uniqueGoodTypes = new List<GoodType>();

            this.Goods.ForEach(good =>
            {
                if (good.Quantity > 0) uniqueGoodTypes.Add(good.Type);
            });

            return uniqueGoodTypes;
        }

        public int countActiveQuarries()
        {
            int count = 0;
            foreach(Plantation plantation in this.Plantations)
            {
               if(plantation.Good == Types.GoodType.Quarry && (plantation.SlotState != SlotEnum.Vacant))
                {
                    count++;
                }
            }

            return count;  
        }

        public int countForests()
        {
            return this.Plantations.Count(plantation => plantation.Good == GoodType.Forest); 
        }

        public Building getBuilding(BuildingName name)
        {
            return Buildings.FirstOrDefault(building => building.Type.Name == name);
        }

        public bool hasActiveBuilding(BuildingName name) 
        { 
            Building building = getBuilding(name);
            if (building == null) return false;
            return building.Slots.Contains(SlotEnum.Colonist) || building.Slots.Contains(SlotEnum.Noble);
        }

        public bool hasVacancyAtBuilding(BuildingName name)
        {
            Building building = getBuilding(name);
            if (building == null) return false;
            return building.Slots.Contains(SlotEnum.Vacant);
        }


        public int freeBuildingTiles()
        {
            int freeSlots = 12;
            foreach(Building building in this.Buildings)
            {
                freeSlots -= building.Type.size;
            }
            return freeSlots;
        }

        public int freePlantationTiles()
        {
            int freeSlots = 12;
            foreach (Plantation plantation in this.Plantations)
            {
                freeSlots --;
            }
            return freeSlots;
        }

        public bool CheckForPriviledge()
        {
            return this.Index == this.gs.PrivilegeIndex;
        }

        

        public void CalculateScore()
        {
            Score += VictoryPoints;

            foreach(Building building in Buildings)
            {
                Score += building.Type.VictoryScore;
                
                if(building.Type.size == 2)
                {
                    if (building.Slots.Contains(SlotEnum.Colonist) || building.Slots.Contains(SlotEnum.Noble))
                    {
                        Score += CalculateLargeBuildingBonus(building.Type);
                    }
                }
            }

            Score += (0.001 * Doubloons);
            Score += (0.001 * Goods.Sum(x => x.Quantity));

        }

        public int CountColonists()
        {
            int count = 0;
            foreach(Building building in Buildings)
            {
                count += building.Slots.Sum(x=> (x == SlotEnum.Colonist) ? 1 : 0 );
            }
            foreach(Plantation plantation in Plantations)
            {
                if(plantation.Good != GoodType.Forest) count += plantation.SlotState == SlotEnum.Colonist ? 1 : 0;
            }
            return count+Colonists;
        }


        public int CountNobles()
        {
            int count = 0;
            foreach (Building building in Buildings)
            {
                count += building.Slots.Sum(x => (x == SlotEnum.Noble) ? 1 : 0);
            }
            foreach (Plantation plantation in Plantations)
            {
                if (plantation.Good != GoodType.Forest) count += plantation.SlotState == SlotEnum.Noble ? 1 : 0;
            }
            return count + Nobles;
        }



        public int CalculateLargeBuildingBonus(BuildingType type)
        {
            switch (type.Name)
            {
                case BuildingName.CityHall:
                    return this.Buildings.Sum(x => x.Type.Color == Utility.ColorName.violet ? 1 : 0);

                case BuildingName.Fortress:
                    return CountColonists() / 3;

                case BuildingName.CustomsHouse:
                    return VictoryPoints / 4;

                case BuildingName.GuildHall:
                    return Buildings.Sum(x => x.Type.Color != Utility.ColorName.violet ? (x.Type.Slots > 1 ? 2 : 1) :0);

                case BuildingName.RoyalGarden:
                    return CountNobles();

                case BuildingName.Residence:
                    return Math.Max(Plantations.Count - 5, 4);
                case BuildingName.Cloister:
                                int[] plantationCounts = new int[5] { 0, 0, 0, 0, 0 };
                                foreach (Plantation plantation in this.Plantations)
                                {
                                switch (plantation.Good)
                                {
                                case Types.GoodType.Corn:
                                plantationCounts[0]++;
                                break;
                                case Types.GoodType.Indigo:
                                plantationCounts[1]++;
                                break;
                                case Types.GoodType.Sugar:
                                plantationCounts[2]++;
                                break;
                                case Types.GoodType.Tobacco:
                                plantationCounts[3]++;
                                break;
                                case Types.GoodType.Coffee:
                                plantationCounts[4]++;
                                break;
                                default:
                                break;
                                }
                                }

                                int trios = 0;
                                for(int i = 0; i< 5; i++)
                                {
                                trios += plantationCounts[i] / 3;
                                }
                        switch(trios)
                            {
                                case 0:
                                 return 0;
                                case 1:
                                return 1;
                                case 2:
                                return 3;
                                case 3:
                                return 6;
                                case 4:
                                return 10;
                          }
                             return 0;
                    default:
                        return 0;
            }
        }
    }
}
