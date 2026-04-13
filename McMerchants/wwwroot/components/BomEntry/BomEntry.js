const bomEntryTemplateContent = document.getElementById('bom-entry-template').content;
const storedAvailabilityTemplateContent = document.getElementById('bom-avail-stored').content;
const producedAvailabilityTemplateContent = document.getElementById('bom-avail-produced').content;

const STATUS_COMPLETE = 2, STATUS_STORED = 1, STATUS_MISSING = 0;
const statusLabels = ['missing', 'available', 'complete'];

class BomEntry extends HTMLLIElement {
    #itemName = null;
    #quantity = null;
    #stackSize = null;
    #availability = null;
    #status = null;

    constructor() {
        super();
        this.appendChild(document.importNode(bomEntryTemplateContent, true));
        this.querySelector('input[type="checkbox"]').addEventListener('change', this.onCheckboxChecked.bind(this));
    }

    onCheckboxChecked(event) {
        if (event.target.checked) {
            this.classList.add('checked');
        } else {
            this.classList.remove('checked');
        }
    }

    setData(itemName, quantity, stackSize, availability) {
        this.#itemName = itemName;
        this.#quantity = quantity;
        this.#stackSize = stackSize;
        this.#availability = availability;

        this.updateInfo();
        this.updateAvailability();
    }

    updateInfo() {
        this.querySelector('.item-name').textContent = this.#itemName;
        this.querySelector('.item-count').textContent = this.#quantity;
        this.querySelector('.info > .repartition').textContent = this.stacksAndItemsToText(this.#stackSize, this.#quantity);
    }

    updateAvailability() {
        let missing = this.#quantity;
        this.#status = STATUS_COMPLETE;

        if (this.#availability.workzone !== null) {
            missing -= this.#availability.workzone;

            const workzoneElement = this.querySelector('.workzone');
            workzoneElement.classList.remove('hidden');
            workzoneElement.querySelector('.count').textContent = this.#availability.workzone;

            if (missing > 0) {
                const remainderElement = this.querySelector('.remainder');
                remainderElement.classList.remove('hidden');
                remainderElement.querySelector('.count').textContent = missing;
                remainderElement.querySelector('.repartition').textContent = this.stacksAndItemsToText(this.#stackSize, missing);
            }
        }

        if (missing > 0) {
            this.#status = STATUS_STORED;
        }

        this.#availability.stores.forEach(store => {
            this.addStoredLine(store.count, store.name);
            missing -= store.count;
        });

        this.#availability.factories.forEach(factory => {
            if (factory.count > 0) {
                this.addStoredLine(factory.count, factory.name);
                missing -= factory.count;
            } else {
                this.addProducedAtLine(factory.name);
            }
        });

        if (missing > 0) {
            this.#status = STATUS_MISSING;
        }

        this.#availability.traders.forEach(trader => {
            this.addProducedAtLine(trader);
        });

        if (missing < 0) {  // if there's more items in storage than we need
            missing = 0;
        }
        this.querySelector('.missing > .count').textContent = missing;
        if (missing > 0) {
            this.querySelector('.missing > .repartition').textContent = this.stacksAndItemsToText(this.#stackSize, missing);
        }

        this.updateStatusIndicator();
    }

    updateStatusIndicator() {
        this.dataset.status = statusLabels[this.#status];
        this.querySelector('.status').dataset.status = statusLabels[this.#status];
    }

    addStoredLine(count, location) {
        const newLine = document.importNode(storedAvailabilityTemplateContent, true);
        newLine.querySelector('.count').textContent = count;
        newLine.querySelector('.location').textContent = location;
        this.querySelector('.stocks').appendChild(newLine);
    }
    addProducedAtLine(location) {
        const newLine = document.importNode(producedAvailabilityTemplateContent, true);
        newLine.querySelector('.location').textContent = location;
        this.querySelector('.stocks').appendChild(newLine);
    }

    stacksAndItemsToText(stackSize, count) {
        if (Number.isNaN(stackSize) || count < stackSize) {
            return `(${count} items)`;
        }

        return `(${Math.floor(count / stackSize)} stacks, ${count % stackSize} items)`;
    }
}

customElements.define("bom-entry", BomEntry, { extends: 'li' });