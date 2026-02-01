const btnHamburgerMobile = document.getElementById('btnHamburgerMobile');
const hamburgerMenuMobile = document.getElementById('hamburgerMenuMobile');
const closeHamburgerMobile = document.getElementById('closeHamburgerMobile');

btnHamburgerMobile.addEventListener('click', () => {
    hamburgerMenuMobile.classList.remove('d-none');
    void hamburgerMenuMobile.offsetWidth;
    hamburgerMenuMobile.style.opacity = '1';
    hamburgerMenuMobile.style.transform = 'translateY(0)';
});

closeHamburgerMobile.addEventListener('click', () => {
    hamburgerMenuMobile.style.opacity = '0';
    hamburgerMenuMobile.style.transform = 'translateY(-20px)';
    setTimeout(() => {
        hamburgerMenuMobile.classList.add('d-none');
    }, 400);
});

// Fecha o menu ao clicar em qualquer link interno
hamburgerMenuMobile.querySelectorAll('a').forEach(link => {
    link.addEventListener('click', () => {
        hamburgerMenuMobile.style.opacity = '0';
        hamburgerMenuMobile.style.transform = 'translateY(-20px)';
        setTimeout(() => {
            hamburgerMenuMobile.classList.add('d-none');
        }, 400);
    });
});