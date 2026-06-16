// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll('.smart-date').forEach(el => {
        const utcDate = new Date(el.getAttribute('datetime'));
        el.textContent = utcDate.toLocaleString();
    });
})

