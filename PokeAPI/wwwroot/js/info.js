import { createCardInfo, getParam } from "./modules/helpers.js"
import {getPokemonInfo, randomePokemon} from "./modules/poke.js"

$(async function () {
    function printPokemon(poke) {
        $('.center__cards').append(createCardInfo(poke, false, true))
    }

    let pokemon;

    function getPoke(poke) {
        pokemon = poke
    }

    let pokeParam = getParam('poke')
    await getPokemonInfo(pokeParam).then(result => getPoke(result))
    printPokemon(pokemon)
    console.log(pokemon)

    $('.back__btn').on('click', function (event) {
        event.preventDefault()
        window.history.replaceState({page: 'index'}, document.title, 'index.html');
        window.close();
    })

    $('.fight__btn').on('click', async function (event) {
        let enemyPoke = await randomePokemon()
        let url = '/fight?myPoke=' + JSON.stringify(pokemon) + '&enemyPoke=' + JSON.stringify(enemyPoke)
        window.open(url, "_self");
    })
})