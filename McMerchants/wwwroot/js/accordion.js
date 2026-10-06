const buttonCloseText = '🔼 Hide', buttonOpenText = '🔽 Show';
const classWhenClosed = 'closed';

const initAccordions = () => {
    const accordions = document.getElementsByClassName('accordion-header');

    for (const header of accordions) {
        const button = header.getElementsByTagName('button').item(0);
        const content = header.nextElementSibling;
        if (content != null) {
            updateButtonText(button, content);
            button.addEventListener('click', (event) => {
                toggleAccordion(button, content);
            })
        }
    }
}

const toggleAccordion = (button, content) => {
    content.classList.toggle(classWhenClosed);
    updateButtonText(button, content);
}

const updateButtonText = (button, content) => {
    if (content.classList.contains(classWhenClosed)) {
        button.innerText = button.dataset?.open ?? buttonOpenText;
    } else {
        button.innerText = button.dataset?.close ?? buttonCloseText;
    }
}