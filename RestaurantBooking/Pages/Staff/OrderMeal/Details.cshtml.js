document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.btn-menu.add-order').forEach(button => {
        button.addEventListener('click', function (event) {
            event.preventDefault();
            const menuItem = button.closest('.menu-item');
            const menuId = menuItem.querySelector('input[name="MenuId"]').value;
            const menuImg = menuItem.querySelector('img.menu-img').src;
            const menuName = menuItem.querySelector('.menu-content a').textContent;
            const menuPrice = menuItem.querySelector('.menu-content span').textContent;
            const number = menuItem.querySelector('.quantity p').textContent;
            console.log(number);
            const menuIngredients = menuItem.querySelector('.menu-ingredients').textContent.trim();

            let selectedContainer = document.querySelector('.selected-item');
            let existingItem = selectedContainer.querySelector(`.menu-item[data-id="${menuId}"]`);

            if (existingItem) {
                let quantitySup = existingItem.querySelector('.menu-content a sup');
                let quantity = parseInt(quantitySup ? quantitySup.textContent.slice(1) : '1') + 1;
                if (quantitySup) {
                    quantitySup.textContent = `x${quantity}`;
                } else {
                    existingItem.querySelector('.menu-content a').innerHTML += `<sup>x${quantity}</sup>`;
                }
            } else {
                let newItem = document.createElement('div');
                newItem.className = 'col-lg-12 menu-item';
                newItem.setAttribute('data-id', menuId);
                newItem.innerHTML = `
                    <img src="${menuImg}" class="menu-img" alt="">
                    <input type="hidden" name="MenuId" value="${menuId}"/>
                    <div class="menu-content">
                        <a href="#">${menuName}</a><span>${menuPrice}</span>
                    </div>
                `;
                selectedContainer.appendChild(newItem);
            }
        });
    });
});