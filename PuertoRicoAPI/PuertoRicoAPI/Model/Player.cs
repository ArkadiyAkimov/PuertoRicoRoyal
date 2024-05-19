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
            this.VictoryPoints = dataPlayer.VictoryPoints;
            this.CanUseHacienda = dataPlayer.CanUseHacienda;
            this.CanUseHospice = dataPlayer.CanUseHospice;
            this.HospiceTargetPlantation = dataPlayer.HospiceTargetPlantation;
            this.CanUseWharf = dataPlayer.CanUseWharf;
            this.CanUseSmallWarehouse = dataPlayer.CanUseSmallWarehouse;
            this.CanUseLargeWarehouse = dataPlayer.CanUseLargeWarehouse;
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
        public int VictoryPoints { get; set; }
        public bool CanUseHacienda { get; set; }
        public bool CanUseHospice { get; set; }
        public GoodType HospiceTargetPlantation { get; set; }
        public bool CanUseWharf { get; set; }
        public bool CanUseSmallWarehouse { get; set; }
        public bool CanUseLargeWarehouse { get; set; }
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
               if(plantation.Good == Types.GoodType.Quarry && plantation.IsOccupied)
                {
                    count++;
                }
            }

            return count;  
        }

        public bool hasBuilding(BuildingName name, bool checkOccupied = false)
        {
            foreach(Building building in this.Buildings)
            {
                if(building.Type.Name == name && (!checkOccupied || building.Slots[0]))
                {
                    return true;
                }
            }
            return false;
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
                    if (building.Slots[0])
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
                count += building.Slots.Sum(x=> x?1:0);
            }
            foreach(Plantation plantation in Plantations)
            {
                count += plantation.IsOccupied ? 1 : 0;
            }
            return count+Colonists;
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

                case BuildingName.Residence:
                    return Math.Max(Plantations.Count - 5, 4);

                default:
                    return 0;
            }
        }
    }
}
