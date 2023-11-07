import { createCardInfo, getParam, getRandomNumber } from './modules/helpers.js'

$(async function () {

    let email="";
    function fillSelect() {
        let select = $('.fight__select')
        for (let i = 1; i <= 10; i++) {
            var option = $("<option>").text(i);
            select.append(option);
        }
    }

    async function sendEmail(winPoke, losPoke) {
        console.log(email)
        if (email !== "") {
            console.log(email)
            await fetch("pokemon/email", {
                method: "POST",
                headers: {"Accept": "application/json", "Content-Type": "application/json"},
                body: JSON.stringify({
                    email: email,
                    statistic: {
                        winPoke: winPoke['name'],
                        losPoke: losPoke['name']
                    }
                })
            });
        }
    }
    function postFightResult(winPoke, losPoke) {
        fetch("api/stat", {
            method: "POST",
            headers: {"Accept": "application/json", "Content-Type": "application/json"},
            body: JSON.stringify({
                winPoke: winPoke['name'],
                losPoke: losPoke['name']
            })
        });
    }

    function reprintCards(pokemons) {
        console.log(pokemons)
        $('.content__inner').empty()
        for (const pokemon of pokemons) {
            let card = createCardInfo(pokemon, true)
            if (pokemon['hp'] === 0) {
                card.css({
                    'opacity': '0.5',
                    'background-color': 'rgba(255, 0, 0, 0.3)',
                })
                postFightResult(pokemons.find(x => x['hp']!==0),pokemon)

                sendEmail(pokemons.find(x => x['hp']!==0),pokemon)
            }
            $('.content__inner').append(card)
        }
    }

    function attack(poke1, poke2) {
        poke1['hp'] -= poke2['attackPower']
        poke1['hp'] = poke1['hp'] < 0 ? 0 : poke1['hp']
        $('.fight__history').append(`<div>Удар нанёс ${poke2['name']}<div>`)
        if (poke1['hp'] === 0) {
            $('.attack__btn').prop("disabled", true)
            $('.fight__history').append(`<div><b>Победил:</b> ${poke2['name']}<div>`)
        }
    }

    async function getEnemies() {
        let url = 'pokemon/fight?myPoke=' + JSON.stringify(getParam('myPoke')) + '&enemyPoke=' + JSON.stringify(getParam('enemyPoke'))
        let response = await fetch(url, {
            method: "GET",
            headers: {"Accept": "application/json", "Content-Type": "application/json"},
        });
        if (response.ok) {
            let results = await response.json()
            return results
        }
        return null
    }

    const delay = ms => new Promise(res => setTimeout(res, ms));
    
    let pokemons=await getEnemies()
    
    reprintCards([pokemons[0],pokemons[1]])
    fillSelect()

    $('.back__btn').on('click', function (event) {
        event.preventDefault()
        window.history.replaceState({page: 'index'}, document.title, 'index.html');
        window.close();
    })

    $('.autoBattle__btn').on('click', async function (event) {
        while (pokemons[0]['hp'] > 0 && pokemons[1]['hp'] > 0) {
            let myNum = getRandomNumber(10)
            let enemyNum = getRandomNumber(10)

            if (myNum % 2 === 0 && enemyNum % 2 === 0 || myNum % 2 !== 0 && enemyNum % 2 !== 0) {
                attack(pokemons[1], pokemons[0])
            } else {
                attack(pokemons[0], pokemons[1])
            }
            reprintCards([pokemons[0], pokemons[1]])
            await delay(3000);
        }
    })

    $('.email__btn').on('click', function (event) {
       email = $('.email__input').val()
       console.log(email)
    })

    $('.attack__btn').on('click', async function (event) {
        let myNum = $('.fight__select').val()
        let response = await fetch(`pokemon/fight/${myNum}`, {
            method: "POST",
            headers: {"Accept": "application/json", "Content-Type": "application/json"},
            body: JSON.stringify({
                myPoke: pokemons[0],
                enemyPoke: pokemons[1]
            })
        });
        if (response.ok) {
            pokemons = await response.json()
        }

        reprintCards([pokemons[0], pokemons[1]])
    })
})