export { getRandomNumber, createCardInfo, getParam }

function getRandomNumber(max) {
    return Math.floor(Math.random() * max + 1);
}

function getParam(paramName) {
    let urlParams = new URLSearchParams(window.location.search)
    let dataParam = urlParams.get(paramName)
    if (dataParam) {
        return JSON.parse(dataParam)
    }
    return null
}

function createCardInfo(poke, isFight = false, isHisPage = false) {
    return $(`
    <div style="display: flex; flex-direction: row">
        <img class="card-img-top" src="${poke['image']}" alt="${poke['name']}" style="height: 300px; width: 300px;" />
        <div style="display: flex; flex-direction: column;">
            <span class="card-title">${poke['name']}</span>
            <span>HP = ${poke['hp']}</span>
            <span>Attack = ${poke['attackPower']}</span>
            ${isHisPage ? `<span>Weight = ${poke['weight']}</span>\n  <span>Height = ${poke['height']}</span>`:''}
        </div>
        ${isFight ? '' : '<a href="#" class="btn btn-primary fight__btn">Select</a>'}
    </div>
    `)
}