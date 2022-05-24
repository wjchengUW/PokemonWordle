﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonWordle_BackEnd.Models;
using System.Diagnostics;
using Newtonsoft.Json;

namespace PokemonWordle_BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameLogicController : ControllerBase
    {

        private readonly ILogger<GameLogicController> _logger;

        public GameLogicController(ILogger<GameLogicController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public GameLogic GetPokemon()
        {
            // connect to PokemonDB and retrieve a random pokemon
            PokemonDBContext pokemonContext = HttpContext.RequestServices.GetService(typeof(PokemonDBContext)) as PokemonDBContext;
            Pokemon pokemon = pokemonContext.GetRandomPokemon();

            GameLogic gL = new GameLogic
            {
                CorrectAnswer = pokemon.Name,
                Hints = new List<Object> { 
                    pokemon.PokedexNumber, 
                    pokemon.Type1, pokemon.Type2, 
                    pokemon.Region, 
                    pokemon.Classification }
            };

            Debug.WriteLine(pokemon.Name);

            return gL;
        }

        [HttpGet("pokemon={pokemon}")]
        public GuessResult CheckValidAnswer([FromRoute] string pokemon)
        {
            // get information from PokemonDB
            PokemonDBContext pokemonContext = HttpContext.RequestServices.GetService(typeof(PokemonDBContext)) as PokemonDBContext;
            bool isValid = pokemonContext.isValid(pokemon);
            GuessResult gr = new GuessResult { isValid = isValid};
            return gr;
        }

        [HttpGet("player={playerID}")]
        public Player GetPlayer([FromRoute] string playerID)
        {
            // get information from PlayerDB
            PlayerDBContext playerContext = HttpContext.RequestServices.GetService(typeof(PlayerDBContext)) as PlayerDBContext;
            Player player = playerContext.getPlayer(playerID);

            return player;
        }

        [HttpPut("player={playerID}&result={guessedCorrectly}")]
        public void UpdatePlayerStats([FromRoute] string playerID, [FromRoute] bool guessedCorrectly)
        {
            // Post new/updated player stats into playerDB
            PlayerDBContext playerContext = HttpContext.RequestServices.GetService(typeof(PlayerDBContext)) as PlayerDBContext;
            playerContext.updatePlayerStats(playerID, guessedCorrectly);

            return;
        }
    }
}
