const api = '/api/stock/%s';
const detailsSection = document.getElementById('details');
const storesContainer = document.getElementById('store-stocks');
const factoriesContainer = document.getElementById('factory-stocks');
const template = document.getElementById('stock');
const spinner = document.getElementById('spinner');
const alert = document.getElementById('alert');

const queryStock = () => {
    setSpinnerDisplayed(true);
    const item = detailsSection.dataset.itemid;
    let stackSize = parseInt(detailsSection.dataset.stacksize);

    fetch(api.replace('%s', item))
        .then(response => response.json())
        .then(response => handleResponse(response, stackSize))
        .then(() => setSpinnerDisplayed(false))
        .catch((e) => {
            console.error(e);
            setSpinnerDisplayed(false);
            setAlertDisplayed(true);
        });
};

const setSpinnerDisplayed = displayed => {
    spinner.style.display = (displayed ? 'block' : 'none');
};

const setAlertDisplayed = displayed => {
    alert.style.display = (displayed ? 'block' : 'none');
}

const handleResponse = (response, stackSize) => {
    fillResults(response.stores, storesContainer, stackSize, parseStore);
    fillResults(response.factories, factoriesContainer, stackSize, parseStore);
}

const fillResults = (results, container, stackSize, itemParsingFunction) => {
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }

    results.forEach(item => {
        itemParsingFunction(item, container, stackSize);
    });
};

const parseStore = (data, container, stackSize) => {
    const { name, logo, results } = data;
    let count = 0;

    for (const entry of results) {
        count += entry.count;
    }

    const storeNode = template.content.cloneNode(true);
    storeNode.querySelector('.storeName').textContent = name;
    storeNode.querySelector('.itemCount').textContent = count.toString();

    if (Number.isNaN(stackSize) || count < stackSize) {
        storeNode.querySelector('.repartition').className = 'hidden';
    } else {
        storeNode.querySelector('.stackCount').textContent = Math.floor(count / stackSize);
        storeNode.querySelector('.remainderCount').textContent = count % stackSize;
    }

    if (logo === null) {
        storeNode.querySelector('.storeLogo').remove();
    } else {
        storeNode.querySelector('.storeLogo').src += logo;
    }

    container.appendChild(storeNode);
};