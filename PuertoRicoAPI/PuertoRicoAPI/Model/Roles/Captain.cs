using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model.Containers;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;
using System.Numerics;
using System.Text.Json;

namespace PuertoRicoAPI.Model.Roles
{
    public class Captain : Role
    {
        public Captain(DataRole dataRole, GameState gs) : base(dataRole, gs)
        {
           
        }

        public override void mainLoop()
        {
            if (IsFirstIteration)
            {
                initializeBuildingEffects(BuildingName.Wharf, true);
                initializeBuildingEffects(BuildingName.SmallWharf, true);

                if (gs.getCurrPlayer().hasActiveBuilding(BuildingName.Lighthouse) // lighthouse 1 coint for privilege
                        && gs.getCurrPlayer().CheckForPriviledge())
                {
                    gs.getCurrPlayer().chargePlayer(-1);
                }

                foreach (Player player in gs.Players)   // reset building effects
                {

                    if (player.hasActiveBuilding(BuildingName.UnionHall))
                    {
                        this.scoreUnionHall(player);
                    }
                }; 
                gs.CaptainFirstShipment = true;
            }

            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            if (gs.CaptainPlayableIndexes.All(index => index == false))
            {
                this.endRole();
                return;
            }


            if (!gs.CaptainPlayableIndexes[gs.CurrentPlayerIndex] 
                || (gs.getCurrPlayer().Goods.Sum(x => x.Quantity) == 0)
                || (!this.checkIfCanShipAnyGoods() && !this.canUseSmallWharf()
                && !this.canUseWharf() && !this.canUseGuestHouse()))
            {
                gs.CaptainPlayableIndexes[gs.CurrentPlayerIndex] = false;
                this.mainLoop();
            }
        }

        public override void endRole()
        {
            base.endRole();
            this.OffloadShips();
            for(int i=0; i< gs.CaptainPlayableIndexes.Count; i++) //reset playable indexes
            {
                gs.CaptainPlayableIndexes[i] = true;
            } 
            Console.WriteLine("indexes: " + JsonSerializer.Serialize(gs.CaptainPlayableIndexes));
            gs.getRole(RoleName.PostCaptain).mainLoop();
        }

        public void scoreUnionHall(Player player)
        {
            int totalVp = 0;

            foreach(Good good in player.Goods)
            {
                totalVp += (int)Math.Floor((double)good.Quantity / 2);
            }

            player.VictoryPoints += totalVp;
            gs.VictoryPointSupply -= totalVp;
        }

        public bool canUseGuestHouse()
        {
            Player player = gs.getCurrPlayer();

            if (player.hasActiveBuilding(BuildingName.GuestHouse)
            && (player.hasVacancyAtBuilding(BuildingName.Wharf) || player.hasVacancyAtBuilding(BuildingName.SmallWharf))){
                return true;
            } else return false;
        }

        public bool canUseWharf()
        {
            var player = gs.getCurrPlayer();

            return player.hasActiveBuilding(BuildingName.Wharf)
                   && player.getBuilding(BuildingName.Wharf).EffectAvailable;
        }

        public bool canUseSmallWharf()
        {
            var player = gs.getCurrPlayer();

            return player.hasActiveBuilding(BuildingName.SmallWharf)
                && player.getBuilding(BuildingName.SmallWharf).EffectAvailable;
        }
        public bool checkIfCanShipAnyGoods()
        {
            var player = gs.getCurrPlayer();
            if(player.Goods.Sum(x=> x.Quantity) == 0) return false;

            var playerTypes = player.GetUniqueGoodTypes();

            for(int i=0; i < gs.Ships.Count - 2; i++)
            {
                var ship = gs.Ships[i];

                if(!ship.IsEmpty())
                {
                    if(playerTypes.Contains(ship.Type) && ship.IsFull())
                    {
                        playerTypes.Remove(ship.Type);
                    }
                    else if (playerTypes.Contains(ship.Type)) return true;
                }
            }
            bool anyShipEmpty = false;
            for(int i=0; i<gs.Ships.Count -2; i++)
            {
                if (gs.Ships[i].IsEmpty()) anyShipEmpty = true;
            }
            if(!anyShipEmpty) return false;
            return playerTypes.Count > 0;
        }

        public void OffloadShips()
        {
            gs.Ships.ForEach(ship =>
            {
                if(ship.IsFull())
                {
                    gs.GetGoodCount(ship.Type, -ship.Capacity);
                    ship.ResetShip();
                }
            });
        }


        public void shipGoodsSmallWharf(GoodType[] goodsToShip)
        {
            Player player = gs.getCurrPlayer();

            int totalGoodsShipped = 0;

            foreach(GoodType goodType in goodsToShip)
            {
                if (player.Goods[(int)goodType].Quantity > 0)
                {
                    player.Goods[(int)goodType].Quantity--;
                    gs.GetGoodCount(goodType, -1 );
                    totalGoodsShipped++;
                }
            }

            player.getBuilding(BuildingName.SmallWharf).EffectAvailable = false;

            int totalVP = (int)Math.Floor((double)totalGoodsShipped/2);

            GivePlayerVictoryPoints(player, totalVP);
        }
        public bool TryAddGoodsToShip(int shipIndex, GoodType type)
        {
            Player player = gs.getCurrPlayer();

            Console.WriteLine("ship index: " +  shipIndex + " can use wharf: " + canUseWharf());
            if (shipIndex == 3 && !canUseWharf()) return false;
            var ship = gs.Ships[shipIndex];


            if (!ship.IsEmpty() && ship.Type != type) return false;
            if(ship.IsEmpty() && gs.Ships.Any(x=>x.Type == type) && shipIndex != 3) return false;

            var relGoodCount = player.GetGoodCount(type);
            int goodsShipped = ship.TryAddGoods(relGoodCount,type);
            if (goodsShipped == 0) return false;

            this.GivePlayerVictoryPoints(player, goodsShipped);
            player.GetGood(type).Quantity -= goodsShipped;

            if(goodsShipped > 0)
            {
                if (shipIndex == 3)
                {
                    if(player.hasActiveBuilding(BuildingName.Wharf))
                        player.getBuilding(BuildingName.Wharf).EffectAvailable = false;
                    gs.GetGoodCount(ship.Type, -ship.Goods.Count);
                    ship.ResetShip();
                }
                else ship.Type = type;
                return true;
            }
            return false;
        }

        public void GivePlayerVictoryPoints(Player player, int goodsShipped)
        {
            int totalVp = goodsShipped;

            if (player.hasActiveBuilding(BuildingName.Harbor))
            {
                totalVp++;
            }

            if (player.hasActiveBuilding(BuildingName.Lighthouse))
            {
                player.chargePlayer(-1);
            }


            if (player.CheckForPriviledge() && gs.CaptainFirstShipment)
            {
                gs.CaptainFirstShipment = false;
                totalVp++;
                if (player.hasActiveBuilding(BuildingName.Library)) totalVp++;
            }

            player.VictoryPoints += totalVp;
            gs.VictoryPointSupply -= totalVp;
            if (gs.VictoryPointSupply <= 0) gs.LastGovernor = true;
        }
    }
}
