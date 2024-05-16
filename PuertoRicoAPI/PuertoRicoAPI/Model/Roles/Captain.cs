using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Models;
using PuertoRicoAPI.Types;
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
            base.mainLoop();
            if (gs.CurrentRole != Name) return;

            if (gs.CaptainPlayableIndexes.All(x => x == false))
            {
                this.endRole();
                return;
            }

            if (!gs.CaptainPlayableIndexes[gs.CurrentPlayerIndex] || !this.checkIfHasValidGoods())
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

        public bool checkIfHasValidGoods()
        {
            var player = gs.getCurrPlayer();
            if(player.Goods.Sum(x=> x.Quantity) == 0) return false;
            if (player.CanUseWharf) return true;
            

            var playerTypes = player.GetUniqueGoodTypes();

            for(int i=0; i < gs.Ships.Count - 1; i++)
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
            for(int i=0; i<gs.Ships.Count -1; i++)
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

        public bool TryAddGoodsToShip(int shipIndex, GoodType type)
        {
            Console.WriteLine("ship index: " +  shipIndex + " can use wharf: " + gs.getCurrPlayer().CanUseWharf);
            if (shipIndex == 3 && !gs.getCurrPlayer().CanUseWharf) return false;
            var ship = gs.Ships[shipIndex];

            if (!ship.IsEmpty() && ship.Type != type) return false;
            if(ship.IsEmpty() && gs.Ships.Any(x=>x.Type == type) && shipIndex != 3) return false;

            var relGoodCount = gs.getCurrPlayer().GetGoodCount(type);
            int goodsShipped = ship.TryAddGoods(relGoodCount,type);
            if (goodsShipped == 0) return false;

            this.GivePlayerVictoryPoints(gs.getCurrPlayer(), goodsShipped);
            gs.getCurrPlayer().GetGood(type).Quantity -= goodsShipped;

            if(goodsShipped > 0)
            {
                if (shipIndex == 3)
                {
                    gs.getCurrPlayer().CanUseWharf = false;
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

            if (player.hasBuilding(BuildingName.Harbor,true))
            {
                totalVp++;
            }

            if(player.CheckForPriviledge() && gs.CaptainFirstShipment)
            {
                gs.CaptainFirstShipment = false;
                totalVp++;
            }

            player.VictoryPoints += totalVp;
            gs.VictoryPointSupply -= totalVp;
            if (gs.VictoryPointSupply <= 0) gs.LastGovernor = true;
        }
    }
}
