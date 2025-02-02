﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Controllers;
using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Front.FrontClasses;
using PuertoRicoAPI.Model.Containers;
using PuertoRicoAPI.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace PuertoRicoAPI.Data.DataHandlers
{
    public static class DataInitializer
    {
        public static async Task<StartGameOutput> Initialize(DataContext _context, int numOfPlayers,GameStartInput gsInput) 
        {
            DataGameState newGameState = new DataGameState();
            newGameState.IsDraft = gsInput.IsDraft;
            newGameState.IsBuildingsExpansion = gsInput.IsBuildingsExpansion; 
            newGameState.IsNoblesExpansion = gsInput.IsNoblesExpansion;
            newGameState.IsRoleInProgress = false; 
            newGameState.VictoryPointSupply = new int[]{ 75, 100, 122}[numOfPlayers - 3];
            newGameState.ColonistsOnShip = gsInput.IsNoblesExpansion ? numOfPlayers - 1 : numOfPlayers;
            newGameState.NoblesSupply = gsInput.IsNoblesExpansion ? 19 : 0;
            newGameState.NoblesOnShip = gsInput.IsNoblesExpansion ? 1 : 0;
            newGameState.QuarryCount = 9;
            newGameState.CornSupply = 10;
            newGameState.IndigoSupply = 11;
            newGameState.SugarSupply = 11;
            newGameState.TobaccoSupply = 9;
            newGameState.CoffeeSupply = 9;
            newGameState.CurrentRole = RoleName.NoRole;
            newGameState.Roles = initializeRoles(numOfPlayers);
            newGameState.Players = initializePlayers(numOfPlayers);
            newGameState.Buildings = initializeBuildings(gsInput);
            newGameState.Plantations = initializePlantations();
            newGameState.Ships = initializeShips(numOfPlayers);
            newGameState.TradeHouse = initializeTradeHouse();
            newGameState.CaptainPlayableIndexes = initCaptainPlayableIndexes(numOfPlayers);
            newGameState.CaptainFirstShipment = true;
            newGameState.MayorTookPrivilige = false;
            newGameState.LastGovernor = false;
            newGameState.GuestHouseNextRole = RoleName.NoRole;
            newGameState.GameOver = false;

            switch (numOfPlayers)
            {
                case 3:
                    newGameState.ColonistsSupply = 55;
                    break;
                case 4:
                    newGameState.ColonistsSupply = 75;
                    break;
                case 5:
                    newGameState.ColonistsSupply = 95;
                    break;
                default:
                    break;
            }

            //newGameState.ColonistsSupply = 10; //TTTTTTTTTEEEEEEEEEESSSSSSSSSSSSSTTTTTTTTTTT

            _context.Games.Add(newGameState);

            await _context.SaveChangesAsync();


            StartGameOutput output = new StartGameOutput();
            output.gameState = newGameState;
            output.BuildingTypes = BuildingTypes.getAll();

            return output;
        }

        static List<DataRole> initializeRoles(int numOfPlayers)
        {
            RoleName[] roleNames = { RoleName.Settler, RoleName.Builder, RoleName.Mayor, RoleName.Trader, RoleName.Craftsman, RoleName.Captain ,RoleName.PostCaptain, RoleName.Draft, RoleName.GuestHouse};
            List<DataRole> roles = new List<DataRole>();

            foreach (RoleName roleName in roleNames)
            {
                roles.Add(initRole(roleName));
            }

            if (numOfPlayers > 3)
            {
                roles.Add(initRole(RoleName.Prospector));
            }

            if (numOfPlayers > 4)
            {
                roles.Add(initRole(RoleName.Prospector));
            }

            return roles;
        }

        static DataRole initRole(RoleName type)
        {
            DataRole newRole = new DataRole();
            newRole.Name = type;
            newRole.IsPlayable = true;
            newRole.Bounty = 0;
            newRole.IsFirstIteration = true;
            return newRole;
        }

        static List<DataPlayer> initializePlayers(int numOfPlayers)
        {
            int[][] faceUpPlants = new int[][] {   // adding faceup plantations
                    new int[] {},
                    new int[] {},
                    new int[] {1, 1, 0},             //3 players
                    new int[] {1, 1, 0, 0},          //4 players
                    new int[] {1, 1, 1, 0, 0}        //5 players
                    };

            List<DataPlayer> newPlayers = new List<DataPlayer>();

            for (int i = 0; i < numOfPlayers; i++)
            {
                var newPlayer = initPlayer(i, numOfPlayers - 1);
                var faceUpGoodType = (GoodType)faceUpPlants[numOfPlayers-1][i];

                newPlayer.Doubloons--;  //balanced
                newPlayer.Doubloons += faceUpPlants[numOfPlayers - 1][i];  //balanced

                newPlayer.Plantations.Add(initPlayerPlantation(faceUpGoodType));

                newPlayers.Add(newPlayer);
            }

            return newPlayers;
        }

        static DataPlayerPlantation initPlayerPlantation(GoodType good)
        {
            DataPlayerPlantation newPlantation = new DataPlayerPlantation();
            newPlantation.Good = good;
            newPlantation.Slot = new DataSlot();
            newPlantation.Slot.State = SlotEnum.Vacant;

            return newPlantation;
        }

        static DataPlayer initPlayer(int index, int doubloons)
        {
            DataPlayer newPlayer = new DataPlayer();
            newPlayer.Index = index;
            newPlayer.Doubloons = doubloons; //doubloons
            newPlayer.Colonists = 0;
            newPlayer.Nobles = 0;
            newPlayer.VictoryPoints = 0;
            newPlayer.TookTurn = false;
            newPlayer.Buildings = new List<DataPlayerBuilding>();
            newPlayer.Plantations = new List<DataPlayerPlantation>();
            newPlayer.BuildOrder = 1;
            newPlayer.Goods = new List<DataPlayerGood>();
            newPlayer.Score = 0;
            for(int i = 0; i< 5; i++)
            {
                DataPlayerGood good = new DataPlayerGood();
                good.Type = (GoodType)i;
                good.Quantity = 5;  // change to 0

                newPlayer.Goods.Add(good);
            }
            return newPlayer;
        }

        static List<DataBuilding> initializeBuildings(GameStartInput gsInput)
        {
            var buildingTypes = BuildingTypes.getAll();
            List<DataBuilding> newBuildings = new List<DataBuilding>();

            for (int i = 0; i < buildingTypes.Count; i++)
            {
                var newBuilding = initBuilding(buildingTypes[i],gsInput);
               
                newBuildings.Add(newBuilding);
                // use the index i here as needed
            }

            return newBuildings;
        }

        static DataBuilding initBuilding(BuildingType type, GameStartInput gsInput)
        {
            DataBuilding newBuilding = new DataBuilding();
            newBuilding.Name = type.Name;
            newBuilding.Slots = new List<DataSlot>();
            newBuilding.Quantity = type.StartingQuantity;


            if(type.IsProduction)newBuilding.isDrafted = true;
            if(gsInput.IsNoblesExpansion && type.Expansion == 2)newBuilding.isDrafted=true;
            if(!gsInput.IsNoblesExpansion && type.Expansion == 2)newBuilding.isBlocked = true;
            else newBuilding.isBlocked = false;

            for (int i = 0; i < type.Slots; i++)
            {
                newBuilding.Slots.Add(new DataSlot());
            }

            return newBuilding;
        }

        static List<DataPlantation> initializePlantations()
        {
            List<DataPlantation> newPlantations = new List<DataPlantation>();

            for (int i = 0; i < 10; i++)
            {
                newPlantations.Add(initPlantation(GoodType.Corn));
            }

            for (int i = 0; i < 11; i++)
            {
                newPlantations.Add(initPlantation(GoodType.Indigo));
            }

            for (int i = 0; i < 11; i++)
            {
                newPlantations.Add(initPlantation(GoodType.Sugar));
            }

            for (int i = 0; i < 9; i++)
            {
                newPlantations.Add(initPlantation(GoodType.Tobacco));
            }

            for (int i = 0; i < 9; i++)
            {
                newPlantations.Add(initPlantation(GoodType.Coffee));
            }

            return newPlantations;
        }

        static DataPlantation initPlantation(GoodType good)
        {
            DataPlantation newPlantation = new DataPlantation();
            newPlantation.Good = good;
            newPlantation.Slot = new DataSlot();
            newPlantation.Slot.State = SlotEnum.Vacant;

            return newPlantation;
        }

        static DataTradeHouse initializeTradeHouse()
        {
            DataTradeHouse datatradeHouse = new DataTradeHouse();

            GoodContainer goodCont = new GoodContainer(4);

            datatradeHouse.Goods = goodCont.SerializeGoodsToJson();

            return datatradeHouse;
        }

        static List<DataShip> initializeShips(int numOfPlayers)
        {
            List<DataShip> newShips = new List<DataShip>();

            for (int i = 0; i < 3; i++)
            {
                newShips.Add(initShip(numOfPlayers+1+i));
            }

            newShips.Add(initShip(11));
            newShips.Add(initShip(50));
            newShips.Add(initShip(5));

            return newShips;
        }

        static DataShip initShip(int capacity)
        {
            DataShip newShip = new DataShip();

            newShip.Load = 0;
            newShip.Capacity = capacity;
            newShip.Type = GoodType.NoType;

            return newShip;
        }

        static string initCaptainPlayableIndexes(int numOfPlayers)
        {
            var indexes = new List<bool>();

            for(int i = 0; i < numOfPlayers; i++)
            {
                indexes.Add(true);
            }

            return JsonSerializer.Serialize<List<bool>>(indexes);
        }
    }
}
