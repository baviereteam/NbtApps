const api = '/api/stock/%s';
const detailsSection = document.getElementById('details');
const storesContainer = document.getElementById('store-stocks');
const factoriesContainer = document.getElementById('factory-stocks');
const tradingContainer = document.getElementById('trading-stocks');
const stockTemplate = document.getElementById('stock');
const tradeTemplate = document.getElementById('trade');
const tradeComponentTemplate = document.getElementById('trade-component');
const spinner = document.getElementById('spinner');
const alertBanner = document.getElementById('alert-banner');

const texts = {
    otherAlleysWithDefaultAlley: 'Also in:',
    otherAlleysWithoutDefaultAlley: 'In alleys:',
    defaultAlleyFull: 'In alley:',
    defaultAlleyEmpty: 'None in:',
    bulkButtonWhenClosed: 'Bulk 🔽',
    bulkButtonWhenOpened: 'Bulk 🔼',
    tradeIconWhenClosed: '🔽',
    tradeIconWhenOpened: '🔼',
    incompleteSearchErrorMessage: '⚠ Some chunks could not be read. The results displayed on this page might be incomplete.',
    genericErrorMessage: '💣 Something went terribly wrong. Please call for help'
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
            showAlert(texts.genericErrorMessage);
        });
};

const setSpinnerDisplayed = displayed => {
    spinner.style.display = (displayed ? 'block' : 'none');
};

const handleResponse = (response, stackSize) => {
    if (!response.complete) {
        showAlert(texts.incompleteSearchErrorMessage);
    }
    fillResults(response.stores, storesContainer, stackSize, parseStore);
    fillResults(response.factories, factoriesContainer, stackSize, parseFactory);
    fillResults(response.traders, tradingContainer, null, parseTrading);
}

const showAlert = (message) => {
    alertBanner.textContent = message;
    alertBanner.classList.remove('hidden');
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
    const storeNode = stockTemplate.content.cloneNode(true);

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
        storeNode.querySelector('.bulk.details').remove();

    } else {
        storeNode.querySelector('.otherAlleys h4').innerText = getAlleysIntroText(defaultAlley !== null);

        // Alleys
        otherAlleys.forEach(alley => {
            const badge = createAlleyBadge(alley.name, alley.count, stackSize);
            alleysContainer.appendChild(badge);
        });

        // Bulk "alley"
        if (bulkContainers.length == 0) {
            storeNode.querySelector('.bulk.details').remove();

        } else {
            const badge = createBulkAlleyBadge();
            alleysContainer.appendChild(badge);
            fillBulkAlleyDetails(storeNode.querySelector('.bulk.details tbody'), bulkContainers);
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

const createBulkAlleyBadge = () => {
    const li = document.createElement('li');
    li.classList.add('bulk');

    const button = document.createElement('button');
    button.textContent = texts.bulkButtonWhenClosed;
    button.addEventListener('click', onBulkButtonClick);
    li.appendChild(button);

    return li;
};

const fillBulkAlleyDetails = (tbody, containers) => {
    containers.forEach(container => {
        const row = tbody.insertRow();
        row.insertCell().textContent = container.count;
        row.insertCell().textContent = container.x;
        row.insertCell().textContent = container.y;
        row.insertCell().textContent = container.z;
    });
}

const parseFactory = (data, container, stackSize) => {
    const { name, logo, results } = data;
    let count = 0;

    for (const entry of results) {
        count += entry.count;
    }

    const factoryNode = createFactoryNode(name, logo, count, stackSize);
    container.appendChild(factoryNode);
};

const createFactoryNode = (name, logo, grandTotal, stackSize) => {
    const factoryNode = stockTemplate.content.cloneNode(true);

    // Identity
    factoryNode.querySelector('.storeName').textContent = name;

    if (logo === null) {
        factoryNode.querySelector('.storeLogo').remove();
    } else {
        factoryNode.querySelector('.storeLogo').src += logo;
    }

    // Global count
    factoryNode.querySelector('.itemCount').textContent = grandTotal.toString();

    if (Number.isNaN(stackSize) || grandTotal < stackSize || grandTotal === 0) {
        factoryNode.querySelector('.repartition').remove();
    } else {
        factoryNode.querySelector('.repartition').textContent = `(${stacksAndItemsToText(stackSize, grandTotal)})`;
    }

    factoryNode.querySelector('.alleys').remove();

    return factoryNode;
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

const onBulkButtonClick = (event) => {
    const button = event.target;
    const bulkDetailsDiv = button.closest('.store').querySelectorAll('.bulk.details')[0];

    bulkDetailsDiv.classList.toggle('closed');
    button.innerText = bulkDetailsDiv.classList.contains('closed') ? texts.bulkButtonWhenClosed : texts.bulkButtonWhenOpened;
}

const parseTrading = (data, container, stackSize) => {
    const { id, name, logo, results } = data;

    const node = createTradingSpotNode(id, name, logo, results);
    container.appendChild(node);
}

const createTradingSpotNode = (id, name, logo, trades) => {
    const node = tradeTemplate.content.cloneNode(true);

    // Identity
    node.querySelector('.storeName span').textContent = name;
    node.querySelector('.storeName a').href = `/Shop/Details/${id}`;

    if (logo === null) {
        node.querySelector('.storeLogo').remove();
    } else {
        node.querySelector('.storeLogo').src += logo;
    }

    // Number of trades
    node.querySelector('.itemCount').textContent = trades.length.toString();
    node.querySelector('button').addEventListener('click', onTradesButtonClick);

    if (trades.length == 0) {
        node.querySelector('.trade.details').remove();
        node.querySelector('button').classList.add('empty');

    } else {
        fillTrades(node.querySelector('.trade.details tbody'), trades);
        node.querySelector('.icon').textContent = texts.tradeIconWhenClosed;
    }

    return node;
}

const fillTrades = (tbody, trades) => {
    trades.forEach(trade => {
        const row = tbody.insertRow();
        row.insertCell().appendChild(getTradeComponent(trade.buy1));
        row.insertCell().appendChild(getTradeComponent(trade.buy2));
        row.insertCell().appendChild(getTradeComponent(trade.sell));
        row.insertCell().textContent = `${trade.villager.job} at ${trade.villager.x}, ${trade.villager.y}, ${trade.villager.z}`;
    });
}

const getTradeComponent = (component) => {
    if (component == null) {
        return document.createElement('div');
    }

    const node = tradeComponentTemplate.content.cloneNode(true);
    const a = node.querySelectorAll('.sprite')[0];
    a.dataset.item = component.id;
    node.querySelectorAll('.text')[0].textContent = `${component.quantity} ${component.item}`;

    if (component.enchantments.length > 0) {
        const enchantmentTexts = [];
        component.enchantments.forEach(e => {
            enchantmentTexts.push(`${e.name} ${e.level}`);
        });

        node.querySelectorAll('.details')[0].textContent = enchantmentTexts.join(', ');
    }
    
    return node;
}

const onTradesButtonClick = (event) => {
    const buttonIcon = event.target.closest('button').querySelectorAll('.icon')[0];
    const detailsDiv = buttonIcon.closest('.store').querySelectorAll('.trade.details')[0];

    detailsDiv.classList.toggle('closed');
    buttonIcon.innerText = detailsDiv.classList.contains('closed') ? texts.tradeIconWhenClosed : texts.tradeIconWhenOpened;
}