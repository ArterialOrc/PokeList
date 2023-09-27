from flask import Flask, redirect, request, render_template
import requests

app = Flask(__name__)

def get_limit():
    url = "https://pokeapi.co/api/v2/pokemon/"
    response = requests.get(url)
    if response.status_code == 200:
        data = response.json()
        return data['count']
    return 0


def get_list_pokemons():
    limit = get_limit()
    url = f"https://pokeapi.co/api/v2/pokemon?limit={limit}"
    response = requests.get(url)
    if response.status_code == 200:
        data = response.json()
        return [pokemon['name'] for pokemon in data['results']]
    return []


def get_pokemon(pokemon_name):
    url = f"https://pokeapi.co/api/v2/pokemon/{pokemon_name.lower()}"
    response = requests.get(url)

    if response.status_code == 200:
        data = response.json()
        return [data['name']]
    else:
        return [f"Покемон с именем {pokemon_name} не найден."]


@app.route('/', methods=["GET", "POST"])
def index():
    if request.method == "POST":
        pokemon_name = request.form["pokemon_name"].strip().lower()
        if pokemon_name:
            pokemon = get_pokemon(pokemon_name)
            return render_template("index.html", pokemon_list=pokemon)
    pokemon_list = get_list_pokemons()
    return render_template("index.html", pokemon_list=pokemon_list)



if __name__ == '__main__':
    app.run()


# See PyCharm help at https://www.jetbrains.com/help/pycharm/
