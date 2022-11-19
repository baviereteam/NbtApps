const api = '/api/stock/%s';
const container = document.getElementById('stocks');
const template = document.getElementById('stock');
const spinner = document.getElementById('spinner');

const queryStock = () => {
    setSpinnerDisplayed(true);
    const item = container.dataset.itemid;

    fetch(api.replace('%s', item))
        .then(response => response.json())
        .then(response => fillResults(response))
        .then(() => setSpinnerDisplayed(false));
};

const setSpinnerDisplayed = displayed => {
    spinner.style.display = (displayed ? 'block' : 'none');
};

const setAlertDisplayed = displayed => {
    alert.style.display = (displayed ? 'block' : 'none');
}

const fillResults = results => {
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }

    results.forEach(item => {
        parseStore(item.name, item.results);
    });
};

const parseStore = (name, data) => {
    let count = 0;

    for (const entry of data) {
        count += entry.count;
    }

    const storeNode = template.content.cloneNode(true);
    storeNode.querySelector('.storeName').textContent = name;
    storeNode.querySelector('.itemCount').textContent = count.toString();
    container.appendChild(storeNode);
};