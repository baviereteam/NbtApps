const bomEntryTemplateContent = document.getElementById('bom-entry-template').content;
const storedAvailabilityTemplateContent = document.getElementById('bom-avail-stored').content;
const producedAvailabilityTemplateContent = document.getElementById('bom-avail-produced').content;

const stacksAndItemsToText = (stackSize, count) => {
    if (Number.isNaN(stackSize) || count < stackSize) {
        return `${count} items`;
    }

    return `(${Math.floor(count / stackSize)} stacks, ${count % stackSize} items)`;
}

class BomEntry extends HTMLElement {
    #itemName = null;
    #quantity = null;
    #stackSize = null;
    #availability = null;

    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
        this.shadowRoot.appendChild(document.importNode(bomEntryTemplateContent, true));
    }

    static observedAttributes = ["item-name", "quantity", "stack-size", "availability"];

    attributeChangedCallback(name, oldValue, newValue) {
        switch (name) {
            case "item-name":
                this.#itemName = newValue;
                this.updateInfo();
                break;
            case "stack-size":
                this.#stackSize = newValue;
                this.updateInfo();
                break;
            case "quantity":
                this.#quantity = newValue;
                this.updateInfo();
                this.updateAvailability();
                break;
            case "availability":
                this.#availability = JSON.parse(newValue);
                this.updateAvailability();
                break;

            default:
                break;
        }

        if (this.#itemName !== null && this.#quantity !== null && this.#stackSize !== null) {
            this.updateInfo();
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
        this.shadowRoot.querySelector('.item-name').textContent = this.#itemName;
        this.shadowRoot.querySelector('.item-count').textContent = this.#quantity;
        this.shadowRoot.querySelector('.info > .repartition').textContent = this.stacksAndItemsToText(this.#stackSize, this.#quantity);
    }

    updateAvailability() {
        let missing = this.#quantity;

        if (this.#availability.workzone !== null) {
            const workzoneElement = this.shadowRoot.querySelector('.workzone');
            workzoneElement.removeClass('hidden');
            workzoneElement.querySelector('.count').textContent = this.#availability.workzone;
            missing -= workzone;
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

        this.#availability.traders.forEach(trader => {
            this.addProducedAtLine(trader);
        });

        if (missing < 0) {  // if there's more items in storage than we need
            missing = 0;
        }
        this.shadowRoot.querySelector('.missing > .count').textContent = missing;
    }

    addStoredLine(count, location) {
        const newLine = document.importNode(storedAvailabilityTemplateContent, true);
        newLine.querySelector('.count').textContent = count;
        newLine.querySelector('.location').textContent = location;
        this.shadowRoot.querySelector('.stocks').appendChild(newLine);
    }
    addProducedAtLine(location) {
        const newLine = document.importNode(producedAvailabilityTemplateContent, true);
        newLine.querySelector('.location').textContent = location;
        this.shadowRoot.querySelector('.stocks').appendChild(newLine);
    }

    stacksAndItemsToText(stackSize, count) {
        if (Number.isNaN(stackSize) || count < stackSize) {
            return `${count} items`;
        }

        return `(${Math.floor(count / stackSize)} stacks, ${count % stackSize} items)`;
    }
}

customElements.define("bom-entry", BomEntry);