// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener('click', function (event) {
    const ellipsis = event.target.closest('.expandable-ellipsis');

    if (!ellipsis) {
        return;
    }

    const isExpanded = ellipsis.classList.toggle('expanded');
    ellipsis.setAttribute('aria-expanded', isExpanded.toString());
});

document.addEventListener('keydown', function (event) {
    if (event.key !== 'Enter' && event.key !== ' ') {
        return;
    }

    const ellipsis = event.target.closest('.expandable-ellipsis');

    if (!ellipsis) {
        return;
    }

    event.preventDefault();

    const isExpanded = ellipsis.classList.toggle('expanded');
    ellipsis.setAttribute('aria-expanded', isExpanded.toString());
});

