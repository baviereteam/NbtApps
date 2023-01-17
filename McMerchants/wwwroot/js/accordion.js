const buttonCloseText = '🔼 Hide', buttonOpenText = '🔽 Show';
const classWhenClosed = 'closed';

const initAccordions = () => {
    const accordions = document.getElementsByClassName('accordion-header');

    for (const header of accordions) {
        const button = header.getElementsByTagName('button').item(0);
        button.innerText = buttonCloseText;
        button.addEventListener('click', (event) => {
            toggleAccordion(event.target, event.target.parentElement.nextElementSibling);
        })
    }
}

const toggleAccordion = (button, content) => {
    content.classList.toggle(classWhenClosed);
    button.innerText = content.classList.contains(classWhenClosed) ? buttonOpenText : buttonCloseText;
}