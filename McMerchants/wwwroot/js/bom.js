const texts = {
	incompleteSearchErrorMessage: '⚠ Some chunks could not be read. The results displayed on this page might be incomplete.',
	genericErrorMessage: '💣 Something went terribly wrong. Please call for help',
    workzoneTutorialMessage: 'ℹ To include your temporary storage for this project ("workzone"), create a WorldEdit selection in the game and type ',
    workzoneCommand: '/mcm bom {id}',
};

const itemsContainer = document.getElementById('bom-items');

const loadItemsApi = '/api/bom/{id}/items';
const computeAvailabilitiesApi = '/api/bom/{id}/available';

const showWorkzoneTutorial = (bomId) => {
    const code = document.createElement('code');
    code.textContent = texts.workzoneCommand.replace('{id}', bomId);
    setInfoNodes(texts.workzoneTutorialMessage, code);
}

const loadBomItems = (bomId) => {
    return executeBomQuery(loadItemsApi.replace('{id}', bomId));
}
const computeAvailabilities = (bomId) => {
	return executeBomQuery(computeAvailabilitiesApi.replace('{id}', bomId));
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
		const element = document.createElement('bom-entry');
		element.setData(line.itemName, line.requiredQuantity, line.stackSize, line.availability);

		const li = document.createElement('li');
		li.appendChild(element);
		itemsContainer.appendChild(li);
	});
};