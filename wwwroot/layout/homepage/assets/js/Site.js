$(function () {
    updateCartCount();
});

// Function to update cart count
function updateCartCount() {
    $.ajax({
        url: '/Cart/GetCartItemCount',
        method: 'GET',
        success: function (response) {
            $('.cart-counter').text(response.count);
        },
        error: function () {
            console.error('Failed to fetch cart count');
        }
    });
}


