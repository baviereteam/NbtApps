const api = '/api/stock/%s';
const container = document.getElementById('stocks');
const template = document.getElementById('stock');
const spinner = document.getElementById('spinner');

const queryStock = () => {
    setSpinnerDisplayed(true);
    const item = container.dataset.itemid;
    let stackSize = parseInt(container.dataset.stacksize);

    fetch(api.replace('%s', item))
        .then(response => response.json())
        .then(response => fillResults(response, stackSize))
        .then(() => setSpinnerDisplayed(false));
};

const setSpinnerDisplayed = displayed => {
    spinner.style.display = (displayed ? 'block' : 'none');
};

const setAlertDisplayed = displayed => {
    alert.style.display = (displayed ? 'block' : 'none');
}

const fillResults = (results, stackSize) => {
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }

    results.forEach(item => {
        parseStore(item.name, item.results, stackSize);
    });
};

const parseStore = (name, data, stackSize) => {
    let count = 0;

    for (const entry of data) {
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

    container.appendChild(storeNode);
};