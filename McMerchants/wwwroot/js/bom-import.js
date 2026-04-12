const texts = {
	genericErrorMessage: '💣 Something went terribly wrong. Please call for help',
	continueButtonWhenErrors: 'or save this BOM as is',
};

const spinner = document.getElementById('spinner');
const form = document.getElementById('form');
const nameField = document.getElementById('name-field');
const fileField = document.getElementById('file-field');
const resultsContainer = document.getElementById('results');
const rejectedLinesContainer = document.getElementById('rejected-lines');
const api = '/api/bom/create';
let createdBomId;

const setSpinnerDisplayed = displayed => {
	spinner.style.display = (displayed ? 'block' : 'none');
};

const importBom = () => {
	const file = fileField.files[0];

	if (!file) {
		showImportFormError('No file selected.');
		return;
	}
	if (!file.type.startsWith('text')) {
		showImportFormError('Please select a CSV file.');
		return;
	}

	setSpinnerDisplayed(true);
	form.classList.add('hidden');

	file
		.text()
		.then(text => {
			return {
				name: nameField.value,
				data: text
			};
		})
		.then(body => sendImportRequest(body))
		.then(response => handleResponse(response))
		.catch((e) => {
			console.error(e);
			setSpinnerDisplayed(false);
			showImportFormError(texts.genericErrorMessage);
		});
};

const showImportFormError = (message) => {
	document.getElementById('error').textContent = message;
	setSpinnerDisplayed(false);
	form.classList.remove('hidden');
};

const sendImportRequest = (body) => {
	let url = api;
	if (createdBomId !== undefined) {
		url += `?replace=${createdBomId}`;
	}

	const request = new Request(url, {
		method: 'POST',
		body: JSON.stringify(body),
		headers: {
			'Content-Type': 'application/json',
		},
	});

	return fetch(request)
		.then(response => response.json());
};

const handleResponse = (response) => {
	createdBomId = response.id;
	resultsContainer.querySelector('#bom-name').textContent = response.name;
	resultsContainer.querySelector('#rejected-lines-count').textContent = response.rejected.length;
	resultsContainer.querySelector('#actions a').href = resultsContainer.querySelector('#actions a').href.replace('/0', `/${createdBomId}`);

	if (response.rejected.length > 0) {
		resultsContainer.querySelector('#retry-step').classList.remove('hidden');
		resultsContainer.querySelector('#actions a').textContent = texts.continueButtonWhenErrors;
		displayUnreadableLines(response.rejected);
	}

	setSpinnerDisplayed(false);
	resultsContainer.classList.remove('hidden');
};

const displayUnreadableLines = (lines) => {
	lines.forEach(line => {
		const code = document.createElement('code');
		code.textContent = line.line;

		const desc = document.createElement('p');
		desc.textContent = line.cause;
		const li = document.createElement('li');
		li.appendChild(code);
		li.appendChild(desc);

		rejectedLinesContainer.appendChild(li);
	});
}

const resetForm = () => {
	form.reset();
	resultsContainer.classList.add('hidden');
	form.classList.remove('hidden');
}