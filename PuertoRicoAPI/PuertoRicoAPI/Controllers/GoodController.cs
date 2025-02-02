﻿using Microsoft.AspNetCore.Mvc;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Data.DataHandlers;
using PuertoRicoAPI.Data;
using PuertoRicoAPI.Model.Roles;
using PuertoRicoAPI.Model.ModelHandlers;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Types;
using PuertoRicoAPI.Sockets;
using Microsoft.AspNetCore.SignalR;
using PuertoRicoAPI.Models;
using System.Numerics;

namespace PuertoRicoAPI.Controllers
{
    public class GoodInput
    {
        public int GoodId { get; set; }
        public int ShipIndex { get; set; }
        public int DataGameId { get; set; }
        public int PlayerIndex { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class GoodController : ControllerBase
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        private readonly DataContext _context;
        public GoodController(DataContext context, IHubContext<UpdateHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<DataGameState>> PostGood(GoodInput goodInput)
        {
            DataPlayerGood dataGood = await DataFetcher
                .getDataGood(_context, goodInput.GoodId);

            DataGameState dataGameState = await DataFetcher
              .getDataGameState(_context, goodInput.DataGameId);

            GameState gs = await ModelFetcher
             .getGameState(_context, goodInput.DataGameId);

            if (goodInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");

            var currentRole = gs.getCurrentRole();

            var good = gs.getCurrPlayer().GetGood(dataGood.Type);

            switch(gs.CurrentRole)
            {
                case RoleName.Trader:
                        Console.WriteLine("Try Sell GoodId: " + goodInput.GoodId);
                    if ((currentRole as Trader).TrySellGood(good))
                    {
                        (currentRole as Trader).mainLoop();
                    }
                    else return Ok("Can't sell this secific good");
                    break;
                case RoleName.Captain:
                    var captain = (currentRole as Captain);
                    if (goodInput.ShipIndex >= 4) return Ok("something went wrong");
                    else if (captain.TryAddGoodsToShip(goodInput.ShipIndex, good.Type))
                    {
                        captain.mainLoop();
                    }
                    break;
                case RoleName.PostCaptain:
                    return Ok("post captain good");
                case RoleName.Builder:
                    return Ok("No black market found");
                case RoleName.Craftsman:
                    var craftsman = (currentRole as Craftsman);
                    if (craftsman.CheckSupplyGood(good.Type))
                    {
                        bool canProduceChosenGood = false;

                        Player player = gs.getCurrPlayer();

                        int[] prodArray = craftsman.getProductionArray(player);

                        if (good.Type == GoodType.Corn && prodArray[0] > 0) canProduceChosenGood = true;
                        if (good.Type == GoodType.Indigo && prodArray[1] > 0) canProduceChosenGood = true;
                        if (good.Type == GoodType.Sugar && prodArray[2] > 0) canProduceChosenGood = true;
                        if (good.Type == GoodType.Tobacco && prodArray[3] > 0) canProduceChosenGood = true;
                        if (good.Type == GoodType.Coffee && prodArray[4] > 0) canProduceChosenGood = true;

                        if (canProduceChosenGood) { 
                            craftsman.GiveSupplyGood(good.Type);
                            Console.WriteLine("can produce chosen good: {0}", canProduceChosenGood);

                            bool hasLibrary = player.hasActiveBuilding(BuildingName.Library);

                            if (hasLibrary && (player.getBuilding(BuildingName.Library).EffectAvailable == false))
                            {
                                craftsman.endRole();
                                Console.WriteLine("library out of charge");
                            }

                            if (hasLibrary && player.getBuilding(BuildingName.Library).EffectAvailable)
                            {
                                player.getBuilding(BuildingName.Library).EffectAvailable = false;
                                Console.WriteLine("used library");
                            }

                            if(!hasLibrary) craftsman.endRole();
                        }
                    }
                    else return Ok("Good Insufficient");
                    break;
                default:
                    return Ok("Nothing to do with this good");
            }

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }

        [HttpPost("tradingPost")]
        public async Task<ActionResult<DataGameState>> PostGoodTradingPost(GoodInput goodInput)
        {
            DataPlayerGood dataGood = await DataFetcher
                .getDataGood(_context, goodInput.GoodId);

            DataGameState dataGameState = await DataFetcher
              .getDataGameState(_context, goodInput.DataGameId);

            GameState gs = await ModelFetcher
             .getGameState(_context, goodInput.DataGameId);

            if (goodInput.PlayerIndex != gs.CurrentPlayerIndex) return Ok("wait your turn, bitch");
            if (gs.CurrentRole != RoleName.Trader) return Ok("hi");

            var currentRole = gs.getCurrentRole();

            Player player = gs.getCurrPlayer();

            var good = gs.getCurrPlayer().GetGood(dataGood.Type);

            if (player.GetGoodCount(good.Type) == 0) return Ok("You don't have haha");

            if (player.hasActiveBuilding(BuildingName.TradingPost)) (currentRole as Trader).SellToTradingPost(good);

            await DataFetcher.Update(dataGameState, gs);

            await _context.SaveChangesAsync();

            await UpdateHub.SendUpdate(dataGameState, _hubContext);

            return Ok("Succes");
        }


    }
}
