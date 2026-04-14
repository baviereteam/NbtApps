const texts = {
	incompleteSearchErrorMessage: '⚠ Some chunks could not be read. The results displayed on this page might be incomplete.',
	genericErrorMessage: '💣 Something went terribly wrong. Please call for help',
    workzoneTutorialMessage: 'ℹ To include your temporary storage for this project ("workzone"), create a WorldEdit selection in the game and type ',
    workzoneCommand: '/bom {id}',
};

const itemsContainer = document.getElementById('bom-items');
const hideCompleteCheckbox = document.getElementById('hide-complete');
const hideStoredCheckbox = document.getElementById('hide-stored');
const hideCheckedCheckbox = document.getElementById('hide-checked');

const loadItemsApi = '/api/bom/{id}/items';
const computeAvailabilitiesApi = '/api/bom/{id}/available';

const onPageReady = (bomId, workzoneTutorialEnabled) => {
	const workzone = getWorkzoneQueryParams();

	if (workzone === null) {
		if (workzoneTutorialEnabled) {
			showWorkzoneTutorial(bomId);
		}
		loadBomItems(bomId);
	} else {
		computeAvailabilities(bomId, workzone);
	}

	document.getElementById('compute').addEventListener('click', () => {
		computeAvailabilities(bomId, workzone);
	});

	hideCompleteCheckbox.addEventListener('change', updateFilters);
	hideStoredCheckbox.addEventListener('change', updateFilters);
	hideCheckedCheckbox.addEventListener('change', updateFilters);
}

const showWorkzoneTutorial = (bomId) => {
    const code = document.createElement('code');
    code.textContent = texts.workzoneCommand.replace('{id}', bomId);
    setInfoNodes(texts.workzoneTutorialMessage, code);
}

const getWorkzoneQueryParams = () => {
	const windowQuery = new URLSearchParams(location.search);
	const workzoneParams = {
		dimension: null,
		startx: null,
		starty: null,
		startz: null,
		endx: null,
		endy: null,
		endz: null
	};

	try {
		Object.keys(workzoneParams).forEach(key => {
			if (windowQuery.has(key)) {
				workzoneParams[key] = windowQuery.get(key);
			}
			else {
				throw new Error('Window query is incomplete.');
			}
		});
		return workzoneParams;

	} catch {
		return null;
	}
}

const loadBomItems = (bomId) => {
    return executeBomQuery(loadItemsApi.replace('{id}', bomId));
}
const computeAvailabilities = (bomId, workzone) => {
	let url = computeAvailabilitiesApi.replace('{id}', bomId);

	if (workzone !== null) {
		const params = new URLSearchParams(workzone);
		url = `${url}?${params}`;
	}

	return executeBomQuery(url);
}

const executeBomQuery = (url) => {
	setSpinnerDisplayed(true);

	fetch(url)
		.then(response => response.json())
		.then(response => handleResponse(response))
		.then(() => {
            setSpinnerDisplayed(false);
            document.getElementById('details').classList.remove('hidden');
        })
		.catch((e) => {
			console.error(e);
			setSpinnerDisplayed(false);
			showAlert(texts.genericErrorMessage);
		});
};

const handleResponse = (response) => {
	itemsContainer.textContent = '';

	if (!response.complete) {
		showAlert(texts.incompleteSearchErrorMessage);
	}

	displayBomLines(response.items);
};

const displayBomLines = (lines) => {   
	lines.forEach(line => {
		const element = new BomEntry();
		element.setData(line.itemName, line.requiredQuantity, line.stackSize, line.availability);
		itemsContainer.appendChild(element);
	});
};

const updateFilters = () => {
	toggleClass('hide-complete', hideCompleteCheckbox.checked);
	toggleClass('hide-stored', hideStoredCheckbox.checked);
	toggleClass('hide-checked', hideCheckedCheckbox.checked);
};

const toggleClass = (className, condition) => {
	if (condition) {
		itemsContainer.classList.add(className);
	} else {
		itemsContainer.classList.remove(className);
	}
	}