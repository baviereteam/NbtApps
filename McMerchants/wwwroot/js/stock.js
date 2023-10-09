const api = '/api/stock/%s';
const detailsSection = document.getElementById('details');
const storesContainer = document.getElementById('store-stocks');
const factoriesContainer = document.getElementById('factory-stocks');
const template = document.getElementById('stock');
const spinner = document.getElementById('spinner');
const alert = document.getElementById('alert');

const texts = {
    otherAlleysWithDefaultAlley: 'Also in:',
    otherAlleysWithoutDefaultAlley: 'In alleys:',
    defaultAlleyFull: 'In alley:',
    defaultAlleyEmpty: 'None in:',
}

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
    fillResults(response.factories, factoriesContainer, stackSize, parseFactory);
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
    const { name, logo, alleys } = data;
    let grandTotal = 0;

    let defaultAlley = null;
    const otherAlleys = [];
    const bulkContainers = [];
    
    alleys.forEach(alley => {
        grandTotal += alley.count;

        if (alley.type === 'default') {
            defaultAlley = alley;

        } else if (alley.type === 'bulk') {
            bulkContainers.push(alley);

        } else {
            otherAlleys.push(alley);
        }
    });

    const storeNode = createStoreNode(name, logo, grandTotal, stackSize, defaultAlley, otherAlleys, bulkContainers);
    container.appendChild(storeNode);
};

const createStoreNode = (name, logo, grandTotal, stackSize, defaultAlley, otherAlleys, bulkContainers) => {
    const storeNode = template.content.cloneNode(true);

    // Identity
    storeNode.querySelector('.storeName').textContent = name;

    if (logo === null) {
        storeNode.querySelector('.storeLogo').remove();
    } else {
        storeNode.querySelector('.storeLogo').src += logo;
    }

    // Global count
    storeNode.querySelector('.itemCount').textContent = grandTotal.toString();

    if (Number.isNaN(stackSize) || grandTotal < stackSize || grandTotal === 0) {
        storeNode.querySelector('.repartition').remove();
    } else {
        storeNode.querySelector('.repartition').textContent = `(${stacksAndItemsToText(stackSize, grandTotal)})`;
    }

    // Default alley
    if (defaultAlley !== null) {
        storeNode.querySelector('.mainAlley h4').innerText = getAlleysIntroText(true, true, defaultAlley.count);

        const defaultAlleyContainer = storeNode.querySelector('.mainAlley ul');
        const defaultAlleyBadge = createAlleyBadge(defaultAlley.name, defaultAlley.count, stackSize);
        defaultAlleyContainer.appendChild(defaultAlleyBadge);

    } else {
        storeNode.querySelector('.mainAlley').remove();
    }

    // Alleys
    const alleysContainer = storeNode.querySelector('.otherAlleys ul');
    if (otherAlleys.length == 0 && bulkContainers.length == 0) {
        storeNode.querySelector('.otherAlleys').remove();

    } else {
        storeNode.querySelector('.otherAlleys h4').innerText = getAlleysIntroText(defaultAlley !== null);

        // Alleys
        otherAlleys.forEach(alley => {
            const badge = createAlleyBadge(alley.name, alley.count, stackSize);
            alleysContainer.appendChild(badge);
        });

        // Bulk "alley"
        if (bulkContainers.length > 0) {
            const badge = createBulkAlleyBadge(bulkContainers);
            alleysContainer.appendChild(badge);
        }
    }

    return storeNode;
};

const createAlleyBadge = (name, count, stackSize) => {
    const alleyItem = document.createElement('li');
    alleyItem.textContent = name;
    alleyItem.title = stacksAndItemsToText(stackSize, count);

    if (count === 0) {
        alleyItem.classList.add('empty');
    }

    return alleyItem;
};

const createBulkAlleyBadge = (containers) => {
    const bulkItem = document.createElement('li');
    bulkItem.classList.add('bulk');
    bulkItem.textContent = "-";
    bulkItem.title =
        containers
            .map(element => `${element.count} in ${element.x},${element.y},${element.z}`)
            .join('\n');

    return bulkItem;
};

const parseFactory = (data, container, stackSize) => {
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

const stacksAndItemsToText = (stackSize, count) => {
    if (Number.isNaN(stackSize) || count < stackSize) {
        return `${count} items`;
    }

    return `${Math.floor(count / stackSize)} stacks, ${count % stackSize} items`;
}

const getAlleysIntroText = (hasDefaultAlley, isDefaultAlley, count) => {
    if (hasDefaultAlley && isDefaultAlley) {
        return (count > 0 ? texts.defaultAlleyFull : texts.defaultAlleyEmpty);
    }

    return (hasDefaultAlley ? texts.otherAlleysWithDefaultAlley : texts.otherAlleysWithoutDefaultAlley);
}

//TODO: function to show the counts on mobile when tapping an alley badge