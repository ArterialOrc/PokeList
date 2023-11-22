import {randomePokemon, getPokemons, getPokemonInfo, savePokemon} from './modules/poke.js'
import { createPagination } from './modules/createPagination.js'

$(async function () {
    const CONTAINER_ID = '.pagination'
    const pokemons = await getPokemons()

    createPagination(pokemons, CONTAINER_ID)

    $('.content__inner').on('click', '.open__poke', function () {
        let name = $(this).closest(".card").find(".card-title").text()
        let currentPoke = pokemons.find(poke => poke['name'] === name)
        let url = "/info?poke=" + JSON.stringify(currentPoke['id'])

        window.history.replaceState({ page: 'index' }, document.title, window.location.href);
        window.open(url, "_blank");
    });

    $('.content__inner').on('click', '.fight__btn', async function () {
        let name = $(this).closest(".card").find(".card-title").text()
        let myPoke = await getPokemonInfo(pokemons.find(poke => poke['name'] ===  name)['id'])
        let enemyPoke = await randomePokemon()
        console.log(enemyPoke)
        let url = '/fight?myPoke=' + JSON.stringify(myPoke['id']) + '&enemyPoke=' + JSON.stringify(enemyPoke)

        window.history.replaceState({ page: 'index' }, document.title, window.location.href);
        window.open(url, "_blank");
    });

    $('.content__inner').on('click', '.save__btn', async function () {
        let name = $(this).closest(".card").find(".card-title").text()
        let myPokeId = pokemons.find(poke => poke['name'] ===  name)['id']
        savePokemon(myPokeId);
    });

    $('.search__btn').on('input', function () {
        if (pokemons.length > 0) {
            window.stop()   
            let inputVal = $(this).val()
            const result = pokemons.filter(poke => poke['name'].startsWith(inputVal))
            createPagination(result, CONTAINER_ID)
        }
    })
})