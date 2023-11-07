export { createPagination }
import { getPokemonInfo } from './poke.js'


function createCard(pokemon) {
    return $(`
    <div class="card mb-3" style="width: 30%; height: 30%">
        <div class="row g-0 card__inner">
            <div class="col-md-4">
                <a class="open__poke" href="#">
                    <img src="${pokemon['image']}" class="img-fluid rounded-start" alt="${pokemon['name']}">
                </a>
            </div>
            <div class="col-md-8">
                <div class="card-body">
                    <a class="open__poke" href="#">
                        <h5 class="card-title">${pokemon['name']}</h5>
                    </a>
                    <p class="card-text">
                        HP = ${pokemon['hp']} <br/>
                        Attack = ${pokemon['attackPower']}
                    </p>
                    <button type="button" class="btn btn-dark fight__btn">Select</button>
                    <button type="button" class="btn btn-dark save__btn">Save</button>
                </div>
            </div>
        </div>
    </div>
    `)
}

function createPagination(sources, paginationId) {
    let container = $(paginationId)
    let options = {
        dataSource: sources,
        pageSize: 9,
        callback: async function (response, pagination) {
            container.next().empty()
            $.each(response, async function (index, item) {
                let pokemon = await getPokemonInfo(item['id'])
                let card = createCard(pokemon)
                container.next().append(card)
            });
        }
    };
    container.pagination(options);
}