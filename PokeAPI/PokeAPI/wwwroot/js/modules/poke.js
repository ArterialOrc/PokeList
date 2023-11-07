import { getRandomNumber } from "./helpers.js"
export {getNamesList, randomePokemon,getPokemons,savePokemon }

async function getCount() {
    let url = `https://pokeapi.co/api/v2/pokemon`
    let response = await fetch(url)

    if (response.ok) {
        let result = await response.json()
        return result['count']
    }
    return 0
}

async function randomePokemon() {
    let response = await fetch(`pokemon/random`, {
        method: "GET",
        headers: { "Accept": "application/json", "Content-Type": "application/json" }
    });
    if (response.ok){
        let results = await response.json()
        console.log(results)
        return results
    }
    return null
}

export async function getPokemonInfo(id) {
    let response = await fetch(`pokemon/${id}`, {
        method: "GET",
        headers: { "Accept": "application/json", "Content-Type": "application/json" }
    });
    if (response.ok){
        let results = await response.json()
        return results
    }
    return null
}



async function getNamesList() {
    let count = await getCount()
    let url = `https://pokeapi.co/api/v2/pokemon?limit=${count}`
    let response = await fetch(url)

    if (response.ok) {
        let result = await response.json()
        return result['results']
    }
    return []
}

async function getPokemons() {
    let response = await fetch("pokemon/list", {
        method: "GET",
        headers: { "Accept": "application/json", "Content-Type": "application/json" }
    });
    if (response.ok){
        let results = await response.json()
        return results
    }
    return null
}

async function savePokemon(id){
    fetch(`pokemon/save/${id}`, {
        method: "POST",
        headers: {"Accept": "application/json", "Content-Type": "application/json"}
    });
}