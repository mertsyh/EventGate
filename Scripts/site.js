document.querySelector('.SearchArea1 i').addEventListener('click', function () {
    const parent = this.parentElement;
    parent.classList.toggle('active');
});

const container = document.querySelector('.events-container');
document.getElementById('nextEvent').addEventListener('click', () => {
    container.scrollLeft += 300;
});
document.getElementById('prevEvent').addEventListener('click', () => {
    container.scrollLeft -= 300;
});

